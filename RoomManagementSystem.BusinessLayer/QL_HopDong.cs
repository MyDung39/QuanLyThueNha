using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RoomManagementSystem.DataLayer;
using Microsoft.Data.SqlClient;

namespace RoomManagementSystem.BusinessLayer
{
    public class QL_HopDong
    {
        HopDongDAL hdDAL = new HopDongDAL();

        // Thêm hợp đồng
        public bool ThemHopDong(HopDong hd)
        {
            if (string.IsNullOrEmpty(hd.MaHopDong) || string.IsNullOrEmpty(hd.MaPhong) || string.IsNullOrEmpty(hd.MaNguoiThue))
            {
                throw new Exception("Mã hợp đồng, phòng hoặc người thuê không được để trống");
            }
            return hdDAL.InsertHopDong(hd);
        }

        // Tra ve danh sach hop dong hien co
        public List<HopDong> DanhSachHopDong()
        {
            return hdDAL.GetAllHopDong();
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

        // Tao thong bao het han neu sap het han (logic: < 30 ngay tu ngay hien tai)
        public bool TaoThongBaoHetHan(string maHopDong)
        {
            HopDong hd = hdDAL.GetHopDongById(maHopDong);
            if (hd == null)
            {
                throw new Exception("Hợp đồng không tồn tại");
            }

            DateTime ngayHienTai = DateTime.Now; // Hoac su dung ngay hien tai: new DateTime(2025, 10, 12);
            if (hd.NgayKetThuc != default(DateTime) && (hd.NgayKetThuc - ngayHienTai).TotalDays < 30 && (hd.NgayKetThuc - ngayHienTai).TotalDays > 0)
            {
                string maThongBao = "TB" + Guid.NewGuid().ToString().Substring(0, 8); // Generate ma thong bao don gian
                string noiDung = $"Hợp đồng {maHopDong} sắp hết hạn vào {hd.NgayKetThuc.ToShortDateString()}";
                DateTime ngayThongBao = ngayHienTai;
                string trangThai = "Chưa thông báo";

                return hdDAL.InsertThongBaoHan(maThongBao, maHopDong, noiDung, ngayThongBao, trangThai);
            }
            return false; // Khong can thong bao
        }
    }
}
