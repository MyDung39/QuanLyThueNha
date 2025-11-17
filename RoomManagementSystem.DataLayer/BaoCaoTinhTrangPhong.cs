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
            string sql = "SELECT MaPhong, GiaThue FROM Phong WHERE TrangThai=N'Trống'";
            return dt.ExecuteQuery(sql);
        }

        // 2. Phòng đang thuê
        // ✅ ĐÃ SỬA: Bỏ JOIN bảng NguoiThue để tránh lặp lại MaPhong theo số lượng người ở
        public DataTable GetPhongDangThue()
        {
            // Chỉ lấy thông tin Phòng và Hợp đồng (để giữ lại ngày tháng cho xuất Excel nếu cần)
            // Đảm bảo mỗi Hợp đồng/Phòng chỉ hiện 1 dòng
            string sql = @"
                SELECT p.MaPhong, p.GiaThue, hd.NgayBatDau, hd.NgayKetThuc
                FROM Phong p
                JOIN HopDong hd ON p.MaPhong = hd.MaPhong
                WHERE p.TrangThai = N'Đang thuê' 
                AND hd.TrangThai = N'Hiệu lực'";

            return dt.ExecuteQuery(sql);
        }

        // 3. Phòng dự kiến trống (hợp đồng sắp hết hạn)
        // (Gợi ý: Bạn cũng nên xem xét sửa hàm này tương tự nếu danh sách "Dự kiến" cũng bị lặp)
        public DataTable GetPhongSapTrong()
        {
            string sql = @"
                SELECT p.MaPhong, n.HoTen, hd.NgayKetThuc
                FROM Phong p
                JOIN HopDong hd ON p.MaPhong = hd.MaPhong
                JOIN HopDong_NguoiThue hdnt ON hd.MaHopDong = hdnt.MaHopDong
                JOIN NguoiThue n ON hdnt.MaNguoiThue = n.MaNguoiThue
                WHERE hd.NgayKetThuc BETWEEN GETDATE() AND DATEADD(day,7,GETDATE())";
            return dt.ExecuteQuery(sql);
        }

        // 4. Phòng đang bảo trì
        public DataTable GetPhongBaoTri()
        {
            string sql = "SELECT MaPhong, GiaThue, GhiChu FROM Phong WHERE TrangThai='BaoTri'";
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