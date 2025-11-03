using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RoomManagementSystem.DataLayer;

namespace RoomManagementSystem.BusinessLayer
{
    public class QuanLyNguoiThue
    {
        NguoiThueDAL nt = new NguoiThueDAL();

        // Nhập thông tin nguoi thue
        public bool ThemNguoiThue(NguoiThue a)
        {
            // Kiểm tra nghiệp vụ
            if (string.IsNullOrEmpty(a.MaPhong))
                throw new Exception("Mã phòng không được để trống!");

            if (string.IsNullOrEmpty(a.HoTen))
                throw new Exception("Họ tên không được để trống!");

            if (string.IsNullOrEmpty(a.SoGiayTo))
                throw new Exception("Số giấy tờ (CCCD/CMND) không được để trống!");

            if (a.NgayBatDauThue == DateTime.MinValue)
                throw new Exception("Ngày bắt đầu thuê không hợp lệ!");

            if (string.IsNullOrEmpty(a.TrangThaiThue))
                a.TrangThaiThue = "Đang ở"; // Gán giá trị mặc định

            List<NguoiThue> dsNguoiHienTai = nt.getNguoiThueByPhong(a.MaPhong)
                                              .Where(x => x.TrangThaiThue == "Đang ở")
                                              .ToList();

            if (dsNguoiHienTai.Count == 0)
            {
                a.VaiTro = "Chủ hợp đồng";
            }
            else
            {
                a.VaiTro = "Người ở cùng";
            }

            // Gán ngày tạo và Mã tự động
            a.NgayTao = DateTime.Now;
            a.MaNguoiThue = nt.AutoMaNguoiThue(); // Gọi DAL để lấy mã mới

            return nt.ThemNguoiThue(a);
        }

        //Cập nhật thông tin/tình trạng thuê
        public bool CapNhatNguoiThue(NguoiThue a)
        {
            // Kiểm tra nghiệp vụ
            if (string.IsNullOrEmpty(a.MaNguoiThue))
                throw new Exception("Mã người thuê không được để trống!");

            if (string.IsNullOrEmpty(a.HoTen))
                throw new Exception("Họ tên không được để trống!");

            if (string.IsNullOrEmpty(a.VaiTro) || (a.VaiTro != "Chủ hợp đồng" && a.VaiTro != "Người ở cùng"))
                throw new Exception("Vai trò phải là 'Chủ hợp đồng' hoặc 'Người ở cùng'!");

            return nt.CapNhatNguoiThue(a);
        }

        //Tra ve danh sach nguoi thue
        public List<NguoiThue> getAll()
        {
            return nt.getAllNguoiThue();
        }

        //Tra ve danh sach nguoi thue theo phong
        public List<NguoiThue> getByMaPhong(string maPhong)
        {
            if (string.IsNullOrEmpty(maPhong))
                throw new Exception("Mã phòng không hợp lệ!");

            return nt.getNguoiThueByPhong(maPhong);
        }
    }
}