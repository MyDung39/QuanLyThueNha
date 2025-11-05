using RoomManagementSystem.DataLayer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Globalization;
using Spire.Doc;
using Spire.Doc.Documents;


namespace RoomManagementSystem.BusinessLayer
{
    public class QL_HopDong
    {
        HopDongDAL hdDAL = new HopDongDAL();

        public bool ThemHopDong(HopDong hd, string maChuHopDong)
        {
            if (string.IsNullOrEmpty(hd.MaPhong))
            {
                throw new Exception("Mã phòng không được để trống");
            }
            if (string.IsNullOrEmpty(maChuHopDong))
            {
                throw new Exception("Mã chủ hợp đồng không được để trống");
            }

            hd.ChuNha = "ND001"; // Mặc định
            string newMaHD = hdDAL.AutoMaHD();
            hd.MaHopDong = newMaHD;

            // 1. Thêm hợp đồng chính
            bool success = hdDAL.InsertHopDong(hd);
            if (!success)
            {
                throw new Exception("Thêm hợp đồng thất bại do lỗi CSDL (Bảng HopDong).");
            }

            // 2. Thêm chủ hợp đồng vào bảng trung gian
            HopDong_NguoiThue ct = new HopDong_NguoiThue
            {
                MaHopDong = hd.MaHopDong,
                MaNguoiThue = maChuHopDong,
                VaiTro = "Chủ hợp đồng",
                TrangThaiThue = "Đang ở",
                NgayDonVao = hd.NgayBatDau, // Ngày dọn vào = ngày bắt đầu HĐ
                NgayBatDauThue = hd.NgayBatDau
            };

            bool successCT = hdDAL.InsertHopDongNguoiThue(ct);
            if (!successCT)
            {
                throw new Exception("Thêm hợp đồng thất bại do lỗi CSDL (Bảng HopDong_NguoiThue).");
            }

            return true;
        }

        // Tra ve danh sach hop dong hien co
        public List<HopDong> DanhSachHopDong()
        {
            return hdDAL.GetAllHopDong();
        }

        // Tìm hợp đồng theo mã phòng
        public List<HopDong> TimHopDongTheoPhong(string maPhong)
        {
            if (string.IsNullOrEmpty(maPhong))
            {
                throw new Exception("Mã phòng không được để trống");
            }
            return hdDAL.GetHopDongByMaPhong(maPhong);
        }

        // Cap nhat hop dong (trang thai, ngay ket thuc,...)
        public bool CapNhatHopDong(HopDong hd)
        {
            if (string.IsNullOrEmpty(hd.MaHopDong))
            {
                throw new Exception("Mã hợp đồng không được để trống");
            }
            return hdDAL.UpdateHopDong(hd);
        }

        // Xoa hop dong
        public bool XoaHopDong(string maHopDong)
        {
            if (string.IsNullOrEmpty(maHopDong))
            {
                throw new Exception("Mã hợp đồng không được để trống");
            }
            return hdDAL.DeleteHopDong(maHopDong);
        }

        // Lay thong tin chi tiet cua hop dong
        public HopDongXemIn? LayChiTietHopDong(string maHopDong)
        {
            if (string.IsNullOrEmpty(maHopDong))
            {
                throw new Exception("Mã hợp đồng không được để trống");
            }

            // Gọi hàm GetInHD đã được viết lại trong DAL
            return hdDAL.GetInHD(maHopDong);
        }

        // Xuat hop dong ra file PDF
        public bool XuatHopDongRaPdf(string maHopDong, string outputPath)
        {
            // 1. Lấy dữ liệu chi tiết của hợp đồng
            HopDongXemIn? data = hdDAL.GetInHD(maHopDong);
            if (data == null)
            {
                Console.WriteLine("Lỗi: Không tìm thấy thông tin hợp đồng để xuất file.");
                return false;
            }

            // Lấy tên tệp mẫu từ dữ liệu và xây dựng đường dẫn động
            if (string.IsNullOrEmpty(data.FileDinhKem))
            {
                Console.WriteLine("Lỗi: Hợp đồng này không có tệp mẫu được định nghĩa trong CSDL.");
                return false;
            }

            string templateFileName = data.FileDinhKem;
            // Lấy đường dẫn thư mục gốc của ứng dụng (ví dụ: .../bin/Debug/netX.X)
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            // Kết hợp để tạo đường dẫn đầy đủ đến tệp mẫu trong thư mục Templates
            string templatePath = Path.Combine(baseDirectory, "Templates", templateFileName);


            if (!File.Exists(templatePath))
            {
                Console.WriteLine($"Lỗi: File mẫu không tồn tại tại đường dẫn: {templatePath}");
                return false;
            }

            try
            {
                // 2. Tải file Word mẫu bằng Spire.Doc
                Document document = new Document();
                document.LoadFromFile(templatePath);

                // 3. Tạo một Dictionary chứa các placeholder và giá trị tương ứng
                var culture = new CultureInfo("vi-VN");
                Dictionary<string, string> replacements = new Dictionary<string, string>
                {
                    { "{{NgayBatDau}}", $"{data.NgayBatDau.Day:00}" },
                    { "{{ThangBatDau}}", $"{data.NgayBatDau.Month:00}" },
                    { "{{NamBatDau}}", data.NgayBatDau.Year.ToString() },
                    { "{{TenBenB}}", data.TenNguoiThue ?? "Không có dữ liệu" },
                    { "{{CccdBenB}}", data.CccdNguoiThue ?? "Không có dữ liệu" },
                    { "{{TongSoPhong}}", data.TongSoPhong.ToString() },
                    { "{{DiaChi}}", data.DiaChiNha ?? "Không có dữ liệu" },
                    { "{{NgayDonVao}}", $"{data.NgayDonVao.Day:00}" },
                    { "{{ThangDonVao}}", $"{data.NgayDonVao.Month:00}" },
                    { "{{NamDonVao}}", data.NgayDonVao.Year.ToString() },
                    { "{{TienCoc}}", data.TienCoc.ToString("N0", culture) },
                    { "{{GiaThue}}", data.GiaThue.ToString("N0", culture) },
                    { "{{DienTich}}", data.DienTich.ToString("N2", culture) },
                    { "{{ThoiHan}}", $"{data.ThoiHan} tháng" },
                    { "{{MaHD}}", data.MaHopDong ?? "N/A" },
                    { "{{MaPhong}}", data.MaPhong ?? "N/A" },
                    { "{{SoNguoiHienTai}}", (data.ThanhVien.Count + 1).ToString() } // +1 (chủ hợp đồng)
                };

                // Thay thế tất cả các placeholder trong văn bản
                foreach (var item in replacements)
                {
                    document.Replace(item.Key, item.Value, false, true);
                }
                // Điền dữ liệu vào Bảng trong Phụ Lục
                if (document.Sections[0].Tables.Count > 1)
                {
                    // Lấy bảng từ Section đầu tiên
                    Table table = document.Sections[0].Tables[1] as Table;
                    var roommates = data.ThanhVien;

                    int dataRowTemplateCount = 3; // 3 dòng trống có sẵn trong mẫu
                    int dataRowStartIndex = 1;    // Dòng 0 là header

                    for (int i = 0; i < roommates.Count; i++)
                    {
                        var roommate = roommates[i];
                        TableRow dataRow;

                        if (i < dataRowTemplateCount)
                        {
                            // Sử dụng các dòng trống có sẵn
                            dataRow = table.Rows[dataRowStartIndex + i];
                            // Xóa văn bản trống (nếu có) trước khi thêm
                            dataRow.Cells[0].Paragraphs[0].Text = "";
                            dataRow.Cells[1].Paragraphs[0].Text = "";
                            dataRow.Cells[2].Paragraphs[0].Text = "";
                        }
                        else
                        {
                            // Thêm dòng mới nếu nhiều hơn 3 người
                            dataRow = table.AddRow(true); // true = sao chép định dạng
                        }

                        // Điền dữ liệu
                        dataRow.Cells[0].Paragraphs[0].AppendText((i + 1).ToString());
                        dataRow.Cells[1].Paragraphs[0].AppendText(roommate.HoTen ?? "");
                        dataRow.Cells[2].Paragraphs[0].AppendText(roommate.Cccd ?? "");
                    }

                    // Xóa các dòng mẫu không sử dụng (nếu số người < 3)
                    if (roommates.Count < dataRowTemplateCount)
                    {
                        // Lặp ngược để tránh lỗi index
                        for (int i = dataRowTemplateCount - 1; i >= roommates.Count; i--)
                        {
                            table.Rows.RemoveAt(dataRowStartIndex + i);
                        }
                    }
                    else
                    {
                        Console.WriteLine("Cảnh báo: Bảng Phụ lục sẽ không được điền.");
                    }
                }

                // 4. Lưu tài liệu ra file PDF (Spire.Doc có phương thức riêng cho việc này)
                document.SaveToFile(outputPath, FileFormat.PDF);

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Đã xảy ra lỗi khi xuất file PDF: {ex.Message}");
                return false;
            }
        }

        // Tim ma nguoi thue bang so giay to
        public string? TimMaNguoiThueBangSoGiayTo(string soGiayTo)
        {
            if (string.IsNullOrEmpty(soGiayTo))
            {
                return null;
            }
            // Gọi xuống DAL để thực hiện truy vấn
            return hdDAL.GetMaNguoiThueBySoGiayTo(soGiayTo);
        }

        // Tao thong bao het han neu sap het han (logic: < 30 ngay tu ngay hien tai)
        public bool TaoThongBaoHetHan(string maHopDong)
        {
            HopDong? hd = hdDAL.GetHopDongById(maHopDong);
            if (hd == null)
            {
                throw new Exception("Hợp đồng không tồn tại");
            }

            DateTime ngayHienTai = DateTime.Now;
            // Kiểm tra xem hợp đồng có ngày kết thúc không và có sắp hết hạn không
            if (hd.NgayKetThuc != default(DateTime) && (hd.NgayKetThuc - ngayHienTai).TotalDays < 30 && (hd.NgayKetThuc - ngayHienTai).TotalDays > 0)
            {
                string maThongBao = "TB" + Guid.NewGuid().ToString().Substring(0, 8);
                string noiDung = $"Hợp đồng {maHopDong} sắp hết hạn vào {hd.NgayKetThuc:dd/MM/yyyy}";
                DateTime ngayThongBao = ngayHienTai;
                string trangThai = "Chưa thông báo";

                return hdDAL.InsertThongBaoHan(maThongBao, maHopDong, noiDung, ngayThongBao, trangThai);
            }
            return false;
        }
    }
}