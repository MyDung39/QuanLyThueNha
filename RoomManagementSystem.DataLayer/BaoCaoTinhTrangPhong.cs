using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoomManagementSystem.DataLayer
{
    public class BaoCaoTinhTrangPhong
    {
        Database dt = new Database();
        // 1. Phòng trống
        public DataTable GetPhongTrong()
        {
            string sql = "SELECT* FROM Phong WHERE TrangThai=N'Trống'";
            return dt.ExecuteQuery(sql);
        }

        // 2. Phòng đang thuê
        public DataTable GetPhongDangThue()
        {
            string sql = @"
                SELECT p.MaPhong, p.GiaThue, n.HoTen,hd.NgayBatDau, hd.NgayKetThuc
                FROM Phong p
                JOIN HopDong hd ON p.MaPhong = hd.MaPhong
                JOIN HopDong_NguoiThue hdn ON hd.MaHopDong = hdn.MaHopDong
                JOIN NguoiThue n ON hdn.MaNguoiThue = n.MaNguoiThue -- Sửa dòng này
                WHERE p.TrangThai = N'Đang thuê' 
                  AND hd.TrangThai = N'Hiệu lực'
                  AND hdn.VaiTro = N'Chủ hợp đồng'"; // Chỉ lấy chủ hợp đồng để tránh lặp
            return dt.ExecuteQuery(sql);
        }

        //3. Phòng dự kiến trống (hợp đồng sắp hết hạn)
        public DataTable GetPhongSapTrong()
        {
            string sql = @"
                SELECT p.MaPhong,n.HoTen, hd.NgayKetThuc
                FROM Phong p
                JOIN HopDong hd ON p.MaPhong = hd.MaPhong
                JOIN HopDong_NguoiThue hdn ON hd.MaHopDong = hdn.MaHopDong
                JOIN NguoiThue n ON hdn.MaNguoiThue = n.MaNguoiThue -- Sửa dòng này
                WHERE hd.NgayKetThuc BETWEEN GETDATE() AND DATEADD(day, 7, GETDATE())
                  AND hd.TrangThai = N'Hiệu lực'
                  AND hdn.VaiTro = N'Chủ hợp đồng'";
            return dt.ExecuteQuery(sql);
        }

        // 4. Phòng đang bảo trì
        public DataTable GetPhongBaoTri()
        {
            string sql = "SELECT MaPhong, GiaThue, GhiChu FROM Phong WHERE TrangThai=N'Bảo trì'";
            return dt.ExecuteQuery(sql);
        }

        // 5. Tỷ lệ lấp đầy (%)
        public object GetTyLeLapDay()
        {
            string sql = "SELECT CAST(COUNT(CASE WHEN TrangThai=N'Đang thuê' THEN 1 END)*100.0/COUNT(*) AS DECIMAL(5,2)) FROM Phong";
            return dt.ExecuteScalar(sql);
        }
    }
}