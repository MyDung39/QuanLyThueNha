using DocumentFormat.OpenXml.Bibliography;
using RoomManagementSystem.DataLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace RoomManagementSystem.BusinessLayer
{
    public class QuanLyNguoiThue
    {
        NguoiThueDAL nt = new NguoiThueDAL();

        private readonly NguoiThueDAL _nguoiThueDAL = new NguoiThueDAL();//bổ sung
        private readonly HopDongDAL _hopDongDAL = new HopDongDAL();//bổ sung

        // Nhập thông tin nguoi thue
        public bool ThemNguoiThue(NguoiThue a)
        {
            // Kiểm tra nghiệp vụ cơ bản
            if (string.IsNullOrEmpty(a.HoTen))
                throw new Exception("Họ tên không được để trống!");

            if (string.IsNullOrEmpty(a.SoGiayTo))
                throw new Exception("Số giấy tờ (CCCD/CMND) không được để trống!");

            if (string.IsNullOrEmpty(a.MaNguoiThue))
            {
                a.MaNguoiThue = nt.AutoMaNguoiThue();
            }
            // Kiểm tra SĐT
            if (!Regex.IsMatch(a.Sdt, @"^\d{10}$"))
                throw new Exception("Số điện thoại phải gồm đúng 10 chữ số!");

            // Kiểm tra CCCD
            if (!Regex.IsMatch(a.SoGiayTo, @"^\d{12}$"))
                throw new Exception("CCCD phải gồm đúng 12 chữ số!");

            // Kiểm tra Email
            if (!Regex.IsMatch(a.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                throw new Exception("Email không hợp lệ (phải chứa ký tự '@').");

            if (string.IsNullOrEmpty(a.MaNguoiThue))
            {
                a.MaNguoiThue = nt.AutoMaNguoiThue();
            }

            return nt.ThemNguoiThue(a);
        }

        //Cập nhật thông tin
        public bool CapNhatNguoiThue(NguoiThue a)
        {
            // Kiểm tra nghiệp vụ cơ bản
            if (string.IsNullOrEmpty(a.HoTen))
                throw new Exception("Họ tên không được để trống!");

            if (string.IsNullOrEmpty(a.SoGiayTo))
                throw new Exception("Số giấy tờ (CCCD/CMND) không được để trống!");

            if (string.IsNullOrEmpty(a.MaNguoiThue))
            {
                a.MaNguoiThue = nt.AutoMaNguoiThue();
            }
            // Kiểm tra SĐT
            if (!Regex.IsMatch(a.Sdt, @"^\d{10}$"))
                throw new Exception("Số điện thoại phải gồm đúng 10 chữ số!");

            // Kiểm tra CCCD
            if (!Regex.IsMatch(a.SoGiayTo, @"^\d{12}$"))
                throw new Exception("CCCD phải gồm đúng 12 chữ số!");

            // Kiểm tra Email
            if (!Regex.IsMatch(a.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                throw new Exception("Email không hợp lệ (phải chứa ký tự '@').");

            return nt.CapNhatNguoiThue(a);
        }

        //Tra ve danh sach nguoi thue
        /*    public List<NguoiThue> getAll()
            {
                return nt.getAllNguoiThue();
            }
        */


        // ✅ VIẾT LẠI HOÀN TOÀN PHƯƠNG THỨC NÀY
        /*
        public List<NguoiThue> getAll()
        {
            // 1. Lấy tất cả dữ liệu gốc
            var allTenants = _nguoiThueDAL.getAllNguoiThue();
            // Bạn cần tạo phương thức này trong HopDongDAL
            var allContractDetails = _hopDongDAL.GetAllHopDongNguoiThue();

            // 2. Lặp qua từng người thuê và điền thông tin phụ
            foreach (var tenant in allTenants)
            {
                // Tìm thông tin thuê gần nhất của người này
                var latestRental = allContractDetails
                    .Where(cd => cd.MaNguoiThue == tenant.MaNguoiThue)
                    .OrderByDescending(cd => cd.NgayBatDauThue)
                    .FirstOrDefault();

                if (latestRental != null)
                {
                    // Nếu tìm thấy, gán dữ liệu vào các thuộc tính "phụ"
                    tenant.NgayBatDauThue = latestRental.NgayBatDauThue;
                    tenant.NgayDonRa = latestRental.NgayDonRa;
                    tenant.TrangThaiThue = latestRental.TrangThaiThue;
                }
                else
                {
                    // Nếu không tìm thấy, gán giá trị mặc định
                    tenant.TrangThaiThue = "Chưa có hợp đồng";
                }
            }
            return allTenants; // Trả về danh sách đã được bổ sung thông tin
        }
        */

        public List<NguoiThue> getAll()
        {
            // 1. Lấy tất cả dữ liệu gốc từ DAL
            List<NguoiThue> allTenants = _nguoiThueDAL.getAllNguoiThue();
            List<HopDong> allContracts = _hopDongDAL.GetAllHopDong();
            List<HopDong_NguoiThue> allContractDetails = _hopDongDAL.GetAllHopDongNguoiThue();

            if (allTenants == null || !allTenants.Any())
            {
                return new List<NguoiThue>();
            }

            // 2. Tối ưu việc tra cứu hợp đồng bằng Dictionary
            var contractDict = allContracts.ToDictionary(c => c.MaHopDong);

            // 3. Lặp qua từng người thuê để điền thông tin
            foreach (var tenant in allTenants)
            {
                var latestRentalDetail = allContractDetails
                    .Where(cd => cd.MaNguoiThue == tenant.MaNguoiThue)
                    .OrderByDescending(cd => cd.NgayBatDauThue)
                    .FirstOrDefault();

                if (latestRentalDetail != null)
                {
                    if (contractDict.TryGetValue(latestRentalDetail.MaHopDong, out HopDong contract))
                    {
                        tenant.NgayBatDauThue = contract.NgayBatDau;

                        // ✅ THAY ĐỔI QUAN TRỌNG: TÍNH TOÁN NGÀY KẾT THÚC BẰNG CODE C#
                        // Lấy ngày bắt đầu cộng với số tháng trong thời hạn hợp đồng.
                        tenant.NgayDonRa = contract.NgayBatDau.AddMonths(contract.ThoiHan);

                        tenant.TrangThaiThue = latestRentalDetail.TrangThaiThue;
                    }
                    else
                    {
                        tenant.TrangThaiThue = "HĐ không tồn tại";
                    }
                }
                else
                {
                    tenant.TrangThaiThue = "Chưa có hợp đồng";
                }
            }
            return allTenants;
        }


        public bool XoaNguoiThue(string maNguoiThue)
        {
            if (string.IsNullOrEmpty(maNguoiThue))
                throw new Exception("Mã người thuê không hợp lệ!");
            bool conHan = _nguoiThueDAL.KiemTraHopDongConHan(maNguoiThue);
            if (conHan)
            {
                throw new Exception("Không thể xóa người thuê vì hợp đồng vẫn còn thời hạn!");
            }

            return _nguoiThueDAL.XoaNguoiThue(maNguoiThue);
        }


        public List<NguoiThue> GetTenantsWithoutActiveContract()
        {
            var allTenants = _nguoiThueDAL.getAllNguoiThue();
            // Bạn cần tạo phương thức này trong HopDongDAL
            var activeContractDetails = _hopDongDAL.GetActiveContractDetails();
            var tenantIdsWithContract = activeContractDetails.Select(cd => cd.MaNguoiThue).Distinct().ToList();
            return allTenants.Where(t => !tenantIdsWithContract.Contains(t.MaNguoiThue)).ToList();
        }


        public List<NguoiThue> GetTenantsByRoom(string maPhong)
        {
            if (string.IsNullOrEmpty(maPhong))
            {
                return new List<NguoiThue>();
            }
            // Bạn cần tạo phương thức GetByRoomId trong NguoiThueDAL
            return _nguoiThueDAL.GetByRoomId(maPhong);
        }

    }
}