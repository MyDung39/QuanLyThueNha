using RoomManagementSystem.DataLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Data.SqlClient;
using System.Transactions;
using System.IO;
using Spire.Doc;
using Spire.Doc.Documents;
using System.Globalization;
using Spire.Doc.Fields;

namespace RoomManagementSystem.BusinessLayer
{
    public class QL_HopDong
    {
        private readonly HopDongDAL _hdDAL = new HopDongDAL();
        private readonly PhongDAL _phongDAL = new PhongDAL();
        private readonly NguoiThueDAL _nguoiThueDAL = new NguoiThueDAL();
        private readonly LichSuHopDongDAL _lichSuDAL = new LichSuHopDongDAL();
        private readonly ThongBaoHanDAL _thongBaoDAL = new ThongBaoHanDAL();
        public string TaoHopDong(HopDong hopDong, string maNguoiThueChuHopDong, List<string> maNguoiThueOCung)
        {
            if (hopDong == null || string.IsNullOrEmpty(hopDong.MaPhong) || string.IsNullOrEmpty(maNguoiThueChuHopDong))
            {
                throw new ArgumentException("Thông tin hợp đồng hoặc người thuê không hợp lệ.");
            }

            string connectionString = DbConfig.ConnectionString;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // 1. Tạo mã và chèn hợp đồng chính
                        hopDong.MaHopDong = _hdDAL.AutoMaHD();
                        if (!_hdDAL.InsertHopDong(hopDong, connection, transaction))
                            throw new Exception("Không thể lưu hợp đồng vào CSDL.");
                        // 2a. Chèn chi tiết người thuê (CHỦ HỢP ĐỒNG)
                        var chiTiet = new HopDong_NguoiThue
                        {
                            MaHopDong = hopDong.MaHopDong,
                            MaNguoiThue = maNguoiThueChuHopDong,
                            VaiTro = "Chủ hợp đồng",
                            TrangThaiThue = "Đang ở",
                            NgayBatDauThue = hopDong.NgayBatDau,
                            NgayDonVao = hopDong.NgayBatDau
                        };
                        if (!_hdDAL.InsertHopDongNguoiThue(chiTiet, connection, transaction))
                            throw new Exception("Không thể thêm chi tiết chủ hợp đồng.");
                        // 2b. Chèn chi tiết NGƯỜI Ở CÙNG
                        foreach (var maNguoiOCung in maNguoiThueOCung)
                        {
                            var chiTietOCung = new HopDong_NguoiThue
                            {
                                MaHopDong = hopDong.MaHopDong,
                                MaNguoiThue = maNguoiOCung,
                                VaiTro = "Người ở cùng",
                                TrangThaiThue = "Đang ở",
                                NgayBatDauThue = hopDong.NgayBatDau,
                                NgayDonVao = hopDong.NgayBatDau
                            };
                            if (!_hdDAL.InsertHopDongNguoiThue(chiTietOCung, connection, transaction))
                                throw new Exception($"Không thể thêm chi tiết người ở cùng: {maNguoiOCung}.");
                        }

                        // 3. Cập nhật trạng thái phòng
                        if (!_phongDAL.UpdateRoomStatus(hopDong.MaPhong, "Đang thuê", connection, transaction))
                            throw new Exception("Không thể cập nhật trạng thái của phòng.");
                        // 4. Ghi lịch sử hành động
                        var lichSu = new LichSuHopDong
                        {
                            MaHopDong = hopDong.MaHopDong,
                            MaNguoiThayDoi = hopDong.ChuNha,
                            HanhDong = "Tạo mới hợp đồng",
                            NoiDungThayDoi = $"Tạo hợp đồng cho phòng {hopDong.MaPhong}. Chủ HĐ: {maNguoiThueChuHopDong}. {maNguoiThueOCung.Count} người ở cùng."
                        };
                        _lichSuDAL.Insert(lichSu, connection, transaction);

                        transaction.Commit();
                        return hopDong.MaHopDong;
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }


        // SỬA ĐỔI (Cho Yêu cầu 2: Thêm danh sách người thuê mới)
        public bool CapNhatHopDong(HopDong hopDongDaChinhSua, string maNguoiDung, List<string> maNguoiThueMoi)
        {
            HopDong hopDongCu = _hdDAL.GetHopDongById(hopDongDaChinhSua.MaHopDong);
            if (hopDongCu == null) throw new Exception("Hợp đồng không tồn tại để cập nhật.");

            // ✅ CHỈNH SỬA: Tải danh sách người thuê VÀO BỘ NHỚ TRƯỚC khi bắt đầu Transaction
            // Điều này tránh việc gọi một phương thức DAL không hỗ trợ transaction (getAllNguoiThue)
            // từ bên trong một transaction đang chạy.
            var allTenantsDict = _nguoiThueDAL.getAllNguoiThue().ToDictionary(t => t.MaNguoiThue, t => t.HoTen);

            string connectionString = DbConfig.ConnectionString;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // 1. Cập nhật hợp đồng chính
                        if (!_hdDAL.UpdateHopDong(hopDongDaChinhSua, connection, transaction))
                        {
                            throw new Exception("Cập nhật thông tin hợp đồng thất bại.");
                        }

                        // 2. Lấy nội dung thay đổi (tiền cọc, ngày...)
                        string noiDungThayDoi = GenerateChangeLog(hopDongCu, hopDongDaChinhSua);

                        // 3. THÊM MỚI (Xử lý người thuê mới)
                        if (maNguoiThueMoi != null && maNguoiThueMoi.Count > 0)
                        {
                            List<string> tenNguoiMoiList = new List<string>();
                            foreach (var maNguoiMoi in maNguoiThueMoi)
                            {
                                var chiTietMoi = new HopDong_NguoiThue
                                {
                                    MaHopDong = hopDongDaChinhSua.MaHopDong,
                                    MaNguoiThue = maNguoiMoi,
                                    VaiTro = "Người ở cùng",
                                    TrangThaiThue = "Đang ở",
                                    NgayBatDauThue = hopDongDaChinhSua.NgayBatDau,
                                    NgayDonVao = DateTime.Today // Giả định ngày dọn vào là hôm nay
                                };
                                if (!_hdDAL.InsertHopDongNguoiThue(chiTietMoi, connection, transaction))
                                    throw new Exception($"Không thể thêm chi tiết người ở cùng: {maNguoiMoi}.");

                                // ✅ CHỈNH SỬA: Tra cứu tên từ danh sách đã tải trước đó
                                string tenantName = allTenantsDict.TryGetValue(maNguoiMoi, out var name) ? name : maNguoiMoi; // Lấy tên từ bộ nhớ đệm
                                tenNguoiMoiList.Add(tenantName);
                            }

                            if (tenNguoiMoiList.Count > 0)
                            {
                                noiDungThayDoi += $"\nThêm người ở cùng: {string.Join(", ", tenNguoiMoiList)}.";
                            }
                        }
                        // KẾT THÚC THÊM MỚI

                        if (!string.IsNullOrEmpty(noiDungThayDoi))
                        {
                            var lichSu = new LichSuHopDong
                            {
                                MaHopDong = hopDongDaChinhSua.MaHopDong,
                                MaNguoiThayDoi = maNguoiDung,
                                HanhDong = "Cập nhật thông tin",
                                NoiDungThayDoi = noiDungThayDoi
                            };
                            if (!_lichSuDAL.Insert(lichSu, connection, transaction))
                            {
                                throw new Exception("Không thể ghi lại lịch sử cập nhật.");
                            }
                        }

                        transaction.Commit();
                        return true;
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        private string GenerateChangeLog(HopDong oldData, HopDong newData)
        {
            var changes = new List<string>();
            if (oldData.TienCoc != newData.TienCoc) changes.Add($"Tiền cọc: {oldData.TienCoc:N0} -> {newData.TienCoc:N0}.");
            if (oldData.ThoiHan != newData.ThoiHan) changes.Add($"Thời hạn: {oldData.ThoiHan} tháng -> {newData.ThoiHan} tháng.");
            if (oldData.NgayBatDau.Date != newData.NgayBatDau.Date) changes.Add($"Ngày bắt đầu: {oldData.NgayBatDau:dd/MM/yyyy} -> {newData.NgayBatDau:dd/MM/yyyy}.");
            if (oldData.GhiChu != newData.GhiChu) changes.Add("Cập nhật ghi chú.");
            return string.Join("\n", changes);
        }

        public List<LichSuHopDong> GetContractHistory(string maHopDong)
        {
            return _lichSuDAL.GetByContractId(maHopDong);
        }

        public Dictionary<HopDong, string> GetContractsWithTenantNames()
        {
            var allContracts = _hdDAL.GetAllHopDong()
                                     .Where(c => c.TrangThai == "Hiệu lực")
                                     .ToList();
            var allContractDetails = _hdDAL.GetAllHopDongNguoiThue();
            var allTenants = _nguoiThueDAL.getAllNguoiThue();

            var tenantDict = allTenants.ToDictionary(t => t.MaNguoiThue, t => t.HoTen);
            var result = new Dictionary<HopDong, string>();
            foreach (var contract in allContracts)
            {
                var ownerDetail = allContractDetails.FirstOrDefault(cd => cd.MaHopDong == contract.MaHopDong && cd.VaiTro == "Chủ hợp đồng");
                string tenantName = ownerDetail != null && tenantDict.TryGetValue(ownerDetail.MaNguoiThue, out var name) ? name : "Không xác định";
                result.Add(contract, tenantName);
            }
            return result;
        }

        public List<HopDong_NguoiThue> GetAllHopDongNguoiThue()
        {
            return _hdDAL.GetAllHopDongNguoiThue();
        }

        public HopDongXemIn? LayChiTietHopDong(string maHopDong)
        {
            return _hdDAL.GetInHD(maHopDong);
        }

        public bool XoaHopDong(string maHopDong)
        {
            if (string.IsNullOrEmpty(maHopDong)) throw new Exception("Mã hợp đồng không được để trống");
            HopDong hopDongCanXoa = _hdDAL.GetHopDongById(maHopDong);
            if (hopDongCanXoa == null) throw new Exception("Không tìm thấy hợp đồng để xóa.");
            string maPhong = hopDongCanXoa.MaPhong;
            string connectionString = DbConfig.ConnectionString;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        _lichSuDAL.DeleteByContractId(maHopDong, connection, transaction);
                        _hdDAL.DeleteNguoiThueByContractId(maHopDong, connection, transaction);
                        if (!_hdDAL.DeleteHopDong(hopDongCanXoa, connection, transaction))
                        {
                            throw new Exception("Xóa bản ghi hợp đồng chính thất bại.");
                        }
                        if (!_phongDAL.UpdateRoomStatus(maPhong, "Trống", connection, transaction))
                        {
                            throw new Exception("Cập nhật trạng thái phòng thất bại.");
                        }
                        transaction.Commit();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw new Exception($"Lỗi khi xóa hợp đồng: {ex.Message}");
                    }
                }
            }
        }

        public string GetContractFilePath(string maHopDong)
        {
            var contract = _hdDAL.GetHopDongById(maHopDong);
            string templateName = contract?.FileDinhKem ?? "mau-hop-dong-thue-nha-o.docx";

            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            return Path.Combine(baseDirectory, "Templates", templateName);
        }

        public List<ThongBaoHan> GetNotificationsByContractId(string maHopDong)
        {
            if (string.IsNullOrEmpty(maHopDong))
            {
                return new List<ThongBaoHan>();
            }
            return _thongBaoDAL.GetByContractId(maHopDong);
        }

        public HopDong? GetActiveContractByTenant(string maNguoiThue)
        {
            if (string.IsNullOrEmpty(maNguoiThue))
            {
                return null;
            }
            return _hdDAL.GetActiveContractByTenantId(maNguoiThue);
        }


        public MemoryStream GetMergedContractDocument(string maHopDong)
        {
            // 1. Lấy dữ liệu
            HopDongXemIn data = _hdDAL.GetInHD(maHopDong);
            if (data == null)
            {
                throw new Exception("Không tìm thấy dữ liệu chi tiết cho hợp đồng.");
            }

            // 2. Lấy đường dẫn file mẫu
            string templatePath = GetContractFilePath(maHopDong);
            if (string.IsNullOrEmpty(templatePath) || !File.Exists(templatePath))
            {
                templatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Templates", "mau-hop-dong-thue-nha-o.docx");
                if (!File.Exists(templatePath))
                    throw new Exception($"Không tìm thấy file mẫu tại: {templatePath}. (Lưu ý: File phải nằm trong thư mục bin/Debug/Templates, không phải thư mục source code)");
            }

            // 3. Mở tài liệu và điền dữ liệu
            Document document = new Document();
            document.LoadFromFile(templatePath);
            ReplacePlaceholders(document, data);
            FillRoommatesTable(document, data);

            // 4. Lưu ra MemoryStream
            MemoryStream ms = new MemoryStream();
            try
            {
                document.SaveToStream(ms, FileFormat.Rtf);
                ms.Position = 0;
                return ms;
            }
            catch (Exception)
            {
                ms.Dispose();
                throw;
            }
        }

        // (Yêu cầu 1: Xuất PDF)
        /// <summary>
        /// Tạo file hợp đồng (đã điền thông tin) dưới dạng PDF
        /// </summary>
        public MemoryStream GetMergedContractDocumentAsPdf(string maHopDong)
        {
            // 1. Lấy dữ liệu
            HopDongXemIn data = _hdDAL.GetInHD(maHopDong);
            if (data == null)
            {
                throw new Exception("Không tìm thấy dữ liệu chi tiết cho hợp đồng.");
            }

            // 2. Lấy đường dẫn file mẫu
            string templatePath = GetContractFilePath(maHopDong);
            if (string.IsNullOrEmpty(templatePath) || !File.Exists(templatePath))
            {
                templatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Templates", "mau-hop-dong-thue-nha-o.docx");
                if (!File.Exists(templatePath))
                    throw new Exception($"Không tìm thấy file mẫu tại: {templatePath}.");
            }

            // 3. Mở tài liệu và điền dữ liệu (Tái sử dụng logic cũ)
            Document document = new Document();
            document.LoadFromFile(templatePath);
            ReplacePlaceholders(document, data);
            FillRoommatesTable(document, data);

            // 4. Lưu ra MemoryStream DƯỚI DẠNG PDF
            MemoryStream ms = new MemoryStream();
            try
            {
                document.SaveToStream(ms, FileFormat.PDF); // <-- THAY ĐỔI
                ms.Position = 0;
                return ms;
            }
            catch (Exception)
            {
                ms.Dispose();
                throw;
            }
        }

        private void ReplacePlaceholders(Document document, HopDongXemIn data)
        {
            CultureInfo culture = new CultureInfo("vi-VN");
            DateTime ngayKy = data.NgayTao;
            DateTime ngayDonVao = data.NgayDonVao;

            var replacements = new Dictionary<string, string>
            {
                {"{{NgayBatDau}}", ngayKy.Day.ToString()},
                {"{{ThangBatDau}}", ngayKy.Month.ToString()},
                {"{{NamBatDau}}", ngayKy.Year.ToString()},
                {"{{TenBenB}}", data.TenNguoiThue ?? ""},
                {"{{CccdBenB}}", data.CccdNguoiThue ?? ""},
                {"{{TongSoPhong}}", data.TongSoPhong.ToString()},
                {"{{DiaChi}}", data.DiaChiNha ?? ""},
                {"{{DienTich}}", data.DienTich.ToString("N1", culture)},
                {"{{NgayDonVao}}", ngayDonVao.Day.ToString()},
                {"{{ThangDonVao}}", ngayDonVao.Month.ToString()},
                {"{{NamDonVao}}", ngayDonVao.Year.ToString()},
                {"{{ThoiHan}}", data.ThoiHan.ToString()},
                {"{{TienCoc}}", data.TienCoc.ToString("N0", culture)},
                {"{{GiaThue}}", data.GiaThue.ToString("N0", culture)},
                {"{{MaHD}}", data.MaHopDong ?? ""},
                {"{{MaPhong}}", data.MaPhong ?? ""}
            };
            foreach (var item in replacements)
            {
                document.Replace(item.Key, item.Value ?? "", false, true);
            }
        }

        /// <summary>
        /// Trợ giúp: Điền dữ liệu vào bảng người ở cùng trong Phụ lục
        /// </summary>
        private void FillRoommatesTable(Document document, HopDongXemIn data)
        {
            try
            {
                // 1. Cập nhật tổng số người
                int totalPeople = data.ThanhVien.Count + 1; // +1 là chủ hợp đồng
                document.Replace("{{SoNguoiHienTai}}", totalPeople.ToString(), false, true);
                // 2. Tìm section cuối cùng (Phụ lục)
                if (document.Sections.Count == 0) return;
                Section appendixSection = document.Sections[document.Sections.Count - 1];

                // Bảng [0] có thể là bảng chữ ký, bảng chúng ta cần là bảng cuối cùng 
                if (appendixSection.Tables.Count == 0) return;
                Table table = appendixSection.Tables[appendixSection.Tables.Count - 1] as Table;

                if (table == null || table.Rows.Count < 2)
                {
                    // Cần ít nhất 1 hàng header (index 0) và 1 hàng mẫu (index 1)
                    return;
                }

                // 3. Lấy hàng mẫu (hàng thứ 2, index 1) và SAO LƯU NÓ
                //    Chúng ta Clone() nó để giữ lại định dạng
                TableRow templateRow = table.Rows[1].Clone() as TableRow;
                // 4. XÓA TẤT CẢ các hàng dữ liệu mẫu (từ index 1 trở đi)
                //    Template của bạn có 3 hàng mẫu, phải xóa hết
                while (table.Rows.Count > 1) // Giữ lại hàng header (index 0)
                {
                    table.Rows.RemoveAt(1);
                }

                // 5. Hàm trợ giúp "setText" để gán text an toàn
                //    Tránh lỗi NullReference khi gán text cho ô rỗng
                Action<TableRow, int, string> setText = (row, cellIndex, text) =>
                {
                    if (row.Cells.Count > cellIndex && row.Cells[cellIndex].Paragraphs.Count > 0)
                    {
                        Paragraph para = row.Cells[cellIndex].Paragraphs[0];
                        // Xóa nội dung cũ (nếu có, như số "1", "2", "3" từ template)
                        para.ChildObjects.Clear();
                        // Thêm nội dung mới
                        para.AppendText(text ?? "");
                    }
                };
                // 6. Điền dữ liệu thật
                if (data.ThanhVien.Count == 0)
                {
                    // Nếu không có người ở cùng, thêm một hàng ghi chú
                    TableRow emptyRow = templateRow.Clone() as TableRow;
                    setText(emptyRow, 0, "1");
                    setText(emptyRow, 1, "(Không có người ở cùng)");
                    setText(emptyRow, 2, "");
                    table.Rows.Add(emptyRow);
                }
                else
                {
                    // Nếu có người ở cùng, lặp và thêm
                    int stt = 1;
                    foreach (var member in data.ThanhVien)
                    {
                        TableRow newRow = templateRow.Clone() as TableRow; // Clone từ bản sao lưu

                        setText(newRow, 0, stt.ToString());
                        setText(newRow, 1, member.HoTen);
                        setText(newRow, 2, member.Cccd);

                        table.Rows.Add(newRow); // Thêm hàng mới đã điền
                        stt++;
                    }
                }
            }
            catch (Exception)
            {
                // Bỏ qua nếu có lỗi điền bảng (ví dụ: template bị sửa cấu trúc)
            }
        }

        // (Yêu cầu 3: Kiểm tra hết hạn)
        /// <summary>
        /// Quét tất cả hợp đồng còn hiệu lực, tạo thông báo nếu 7 ngày nữa hết hạn
        /// </summary>
        public void KiemTraVaTaoThongBaoHetHan()
        {
            try
            {
                var allContracts = _hdDAL.GetAllHopDong()
                                         .Where(c => c.TrangThai == "Hiệu lực")
                                         .ToList();

                // Lấy tất cả thông báo đã có để tránh trùng lặp
                var allExistingNotifs = _hdDAL.GetAllThongBaoHan();
                var existingNotifIds = allExistingNotifs.Select(n => n.MaThongBao).ToHashSet();

                foreach (var contract in allContracts)
                {
                    // Tính ngày hết hạn
                    DateTime ngayHetHan = contract.NgayBatDau.AddMonths(contract.ThoiHan);

                    // Tạo mã thông báo duy nhất
                    string maThongBao = $"EXP-{contract.MaHopDong}-{ngayHetHan:yyyyMMdd}";

                    // Điều kiện: Sắp hết hạn (trong 7 ngày) VÀ chưa có thông báo
                    if (ngayHetHan > DateTime.Today &&
                        (ngayHetHan - DateTime.Today).TotalDays <= 7 &&
                        !existingNotifIds.Contains(maThongBao))
                    {
                        string noiDung = $"Hợp đồng {contract.MaHopDong} (Phòng {contract.MaPhong}) sẽ hết hạn vào ngày {ngayHetHan:dd/MM/yyyy}.";

                        // Sử dụng hàm đã có trong HopDongDAL
                        _hdDAL.InsertThongBaoHan(maThongBao, contract.MaHopDong, noiDung, DateTime.Today, "Chưa xem");
                    }
                }
            }
            catch (Exception)
            {
                // Bỏ qua lỗi (ví dụ: lỗi kết nối CSDL khi chạy ngầm)
            }
        }
    }
}

