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
        Database dt= new Database();
        // 1. Phòng trống
        public DataTable GetPhongTrong()
        {
            string sql = "SELECT * FROM Phong WHERE TrangThai=N'Trống'";
            return dt.ExecuteQuery(sql);
        }

        // 2. Phòng đang thuê
        public DataTable GetPhongDangThue()
        {
            string sql = @"
                SELECT p.*
                FROM Phong p
                JOIN HopDong hd ON p.MaPhong = hd.MaPhong
                JOIN NguoiThue n ON hd.MaNguoiThue = n.MaNguoiThue
                WHERE p.TrangThai=N'Đang thuê'";
            return dt.ExecuteQuery(sql);
        }

         //3. Phòng dự kiến trống (hợp đồng sắp hết hạn)
        public DataTable GetPhongSapTrong()
        {
            string sql = @"
                SELECT p.*
                FROM Phong p
                JOIN HopDong hd ON p.MaPhong = hd.MaPhong
                JOIN NguoiThue n ON hd.MaNguoiThue = n.MaNguoiThue
                WHERE hd.NgayKetThuc BETWEEN GETDATE() AND DATEADD(day,7,GETDATE())";
            return dt.ExecuteQuery(sql);
        }

        // 4. Phòng đang bảo trì
        public DataTable GetPhongBaoTri()
        {
            string sql = "SELECT * FROM Phong WHERE TrangThai='BaoTri'";
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
