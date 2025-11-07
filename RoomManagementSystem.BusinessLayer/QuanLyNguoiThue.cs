using System;
using System.Collections.Generic;
using System.Linq;
using RoomManagementSystem.DataLayer;

namespace RoomManagementSystem.BusinessLayer
{
    public class QuanLyNguoiThue
    {
        NguoiThueDAL nt = new NguoiThueDAL();

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
            if (string.IsNullOrEmpty(a.MaNguoiThue))
                throw new Exception("Mã người thuê không được để trống!");

            if (string.IsNullOrEmpty(a.HoTen))
                throw new Exception("Họ tên không được để trống!");

            return nt.CapNhatNguoiThue(a);
        }

        //Tra ve danh sach nguoi thue
        public List<NguoiThue> getAll()
        {
            return nt.getAllNguoiThue();
        }
    }
}