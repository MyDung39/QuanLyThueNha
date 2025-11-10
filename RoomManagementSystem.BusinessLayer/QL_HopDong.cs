using RoomManagementSystem.DataLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;

// Xóa các using không cần thiết ở đây như Spire.Doc, Globalization, IO nếu có

namespace RoomManagementSystem.BusinessLayer
{
    public class QL_HopDong
    {
        private readonly HopDongDAL _hdDAL = new HopDongDAL();
        private readonly PhongDAL _phongDAL = new PhongDAL();
        private readonly NguoiThueDAL _nguoiThueDAL = new NguoiThueDAL();
        private readonly LichSuHopDongDAL _lichSuDAL = new LichSuHopDongDAL();
        private readonly ThongBaoHanDAL _thongBaoDAL = new ThongBaoHanDAL();

        /// <summary>
        /// Tạo mới một hợp đồng và ghi lại lịch sử.
        /// </summary>
        public string TaoHopDong(HopDong hopDong, string maNguoiThueChuHopDong)
        {
            if (hopDong == null || string.IsNullOrEmpty(hopDong.MaPhong) || string.IsNullOrEmpty(maNguoiThueChuHopDong))
            {
                throw new ArgumentException("Thông tin hợp đồng hoặc người thuê không hợp lệ.");
            }

            using (var scope = new TransactionScope())
            {
                // 1. Tạo mã và chèn hợp đồng chính
                hopDong.MaHopDong = _hdDAL.AutoMaHD();
                if (!_hdDAL.InsertHopDong(hopDong)) throw new Exception("Không thể lưu hợp đồng vào CSDL.");

                // 2. Chèn chi tiết người thuê (chủ hợp đồng)
                var chiTiet = new HopDong_NguoiThue
                {
                    MaHopDong = hopDong.MaHopDong,
                    MaNguoiThue = maNguoiThueChuHopDong,
                    VaiTro = "Chủ hợp đồng",
                    TrangThaiThue = "Đang ở",
                    NgayBatDauThue = hopDong.NgayBatDau,
                    NgayDonVao = hopDong.NgayBatDau
                };
                if (!_hdDAL.InsertHopDongNguoiThue(chiTiet)) throw new Exception("Không thể thêm chi tiết người thuê vào hợp đồng.");

                // 3. Cập nhật trạng thái phòng thành "Đang thuê"
                if (!_phongDAL.UpdateRoomStatus(hopDong.MaPhong, "Đang thuê")) throw new Exception("Không thể cập nhật trạng thái của phòng.");

                // 4. Ghi lịch sử hành động "Tạo mới"
                var lichSu = new LichSuHopDong
                {
                    MaHopDong = hopDong.MaHopDong,
                    MaNguoiThayDoi = hopDong.ChuNha,
                    HanhDong = "Tạo mới hợp đồng",
                    NoiDungThayDoi = $"Tạo hợp đồng cho phòng {hopDong.MaPhong}."
                };
                _lichSuDAL.Insert(lichSu);

                scope.Complete();
                return hopDong.MaHopDong;
            }
        }

        /// <summary>
        /// Cập nhật thông tin của một hợp đồng và ghi lại các thay đổi vào bảng lịch sử.
        /// </summary>
        public bool CapNhatHopDong(HopDong hopDongDaChinhSua, string maNguoiDung)
        {
            // Lấy thông tin hợp đồng CŨ từ CSDL để so sánh
            HopDong hopDongCu = _hdDAL.GetHopDongById(hopDongDaChinhSua.MaHopDong);
            if (hopDongCu == null) throw new Exception("Hợp đồng không tồn tại để cập nhật.");

            using (var scope = new TransactionScope())
            {
                // 1. Cập nhật bản ghi HopDong chính trong CSDL
                if (!_hdDAL.UpdateHopDong(hopDongDaChinhSua))
                {
                    throw new Exception("Cập nhật thông tin hợp đồng thất bại.");
                }

                // 2. So sánh dữ liệu cũ và mới để tạo mô tả thay đổi
                string noiDungThayDoi = GenerateChangeLog(hopDongCu, hopDongDaChinhSua);

                // 3. Nếu có sự thay đổi, ghi lại vào bảng lịch sử
                if (!string.IsNullOrEmpty(noiDungThayDoi))
                {
                    var lichSu = new LichSuHopDong
                    {
                        MaHopDong = hopDongDaChinhSua.MaHopDong,
                        MaNguoiThayDoi = maNguoiDung,
                        HanhDong = "Cập nhật thông tin",
                        NoiDungThayDoi = noiDungThayDoi
                    };
                    if (!_lichSuDAL.Insert(lichSu))
                    {
                        throw new Exception("Không thể ghi lại lịch sử cập nhật.");
                    }
                }

                scope.Complete();
            }
            return true;
        }

        // Hàm trợ giúp để tạo chuỗi mô tả các thay đổi
        private string GenerateChangeLog(HopDong oldData, HopDong newData)
        {
            var changes = new List<string>();
            if (oldData.TienCoc != newData.TienCoc) changes.Add($"Tiền cọc: {oldData.TienCoc:N0} -> {newData.TienCoc:N0}.");
            if (oldData.ThoiHan != newData.ThoiHan) changes.Add($"Thời hạn: {oldData.ThoiHan} tháng -> {newData.ThoiHan} tháng.");
            if (oldData.NgayBatDau.Date != newData.NgayBatDau.Date) changes.Add($"Ngày bắt đầu: {oldData.NgayBatDau:dd/MM/yyyy} -> {newData.NgayBatDau:dd/MM/yyyy}.");
            if (oldData.GhiChu != newData.GhiChu) changes.Add("Cập nhật ghi chú.");
            return string.Join("\n", changes);
        }

        /// <summary>
        /// Lấy toàn bộ lịch sử thay đổi của một hợp đồng.
        /// </summary>
        public List<LichSuHopDong> GetContractHistory(string maHopDong)
        {
            return _lichSuDAL.GetByContractId(maHopDong);
        }

        /// <summary>
        /// Lấy danh sách các hợp đồng còn hiệu lực và tên người thuê tương ứng.
        /// </summary>
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
            return _hdDAL.DeleteHopDong(maHopDong);
        }

        public string GetContractFilePath(string maHopDong)
        {
            var contract = _hdDAL.GetHopDongById(maHopDong);
            if (contract == null || string.IsNullOrEmpty(contract.FileDinhKem)) return null;
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            return Path.Combine(baseDirectory, "Templates", contract.FileDinhKem);
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

    }
}