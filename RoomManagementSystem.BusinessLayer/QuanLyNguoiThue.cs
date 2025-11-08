using System;
using System.Collections.Generic;
using System.Linq;
using RoomManagementSystem.DataLayer;

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

            return nt.ThemNguoiThue(a);
        }

        //Cập nhật thông tin
        public bool CapNhatNguoiThue(NguoiThue a)
        {
            if (string.IsNullOrEmpty(a.SoGiayTo))
                throw new Exception("Số giấy tờ (CCCD/CMND) không được để trống!");

            if (string.IsNullOrEmpty(a.HoTen))
                throw new Exception("Họ tên không được để trống!");

            return nt.CapNhatNguoiThue(a);
        }

        //Tra ve danh sach nguoi thue
    /*    public List<NguoiThue> getAll()
        {
            return nt.getAllNguoiThue();
        }
    */


        // ✅ VIẾT LẠI HOÀN TOÀN PHƯƠNG THỨC NÀY
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


        public bool XoaNguoiThue(string maNguoiThue)
        {
            if (string.IsNullOrEmpty(maNguoiThue))
                throw new Exception("Mã người thuê không hợp lệ!");

            return _nguoiThueDAL.XoaNguoiThue(maNguoiThue);
        }
    }
}