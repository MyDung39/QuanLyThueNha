using Microsoft.Data.SqlClient;
using System;
using System.Data;

namespace RoomManagementSystem.DataLayer
{
    public class DoanhThuThang
    {
        Database db = new Database();

        // Tính tổng doanh thu thực tế (tiền đã về túi)
        public decimal TinhTongDoanhThuThang(int thang, int nam)
        {
            string thoiKy = $"{thang:D2}/{nam}"; // Định dạng thành "11/2025"

            string sql = @"
                SELECT SUM(tt.SoTienDaThanhToan) 
                FROM ThanhToan tt
                JOIN HoaDon hd ON tt.MaHoaDon = hd.MaHoaDon
                WHERE hd.ThoiKy = @ThoiKy AND tt.TrangThai = N'Đã trả'";

            SqlParameter[] parameters = {
                new SqlParameter("@ThoiKy", thoiKy)
            };

            try
            {
                object result = db.ExecuteScalar(sql, parameters);
                if (result != null && result != DBNull.Value)
                {
                    return Convert.ToDecimal(result);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi tính tổng doanh thu: " + ex.Message);
            }
            return 0;
        }

        // Lấy danh sách chi tiết báo cáo (Quan trọng cho View)
        public DataTable BaoCaoDoanhThuThang(int thang, int nam)
        {
            string thoiKy = $"{thang:D2}/{nam}"; // Tạo chuỗi thời kỳ ví dụ: "11/2025"

            // SQL logic:
            // 1. Bắt đầu từ HoaDon (vì báo cáo dựa trên hóa đơn đã xuất trong tháng).
            // 2. JOIN Phong, HopDong, NguoiThue để lấy thông tin hiển thị.
            // 3. LEFT JOIN ThanhToan để lấy trạng thái (dùng LEFT vì có thể chưa tạo phiếu thanh toán hoặc chưa trả).
            string sql = @"
                SELECT
                    ROW_NUMBER() OVER(ORDER BY p.MaPhong) AS STT,
                    p.MaPhong AS [Phòng],
                    nt.HoTen AS [Người thuê],
                    
                    -- Lấy Tổng công nợ của hóa đơn làm 'Tiền thuê' (Doanh thu dự kiến)
                    -- Hoặc nếu bạn chỉ muốn hiển thị Giá Thuê gốc của phòng thì dùng p.GiaThue
                    p.GiaThue AS [Tiền thuê], 

                    CASE 
                        WHEN tt.TrangThai IS NULL THEN N'Chưa trả'
                        ELSE tt.TrangThai 
                    END AS [Tình trạng thanh toán]

                FROM HoaDon hd
                JOIN Phong p ON hd.MaPhong = p.MaPhong
                -- Lấy thông tin người thuê hiện tại (Chủ hợp đồng)
                LEFT JOIN HopDong hdong ON p.MaPhong = hdong.MaPhong AND hdong.TrangThai = N'Hiệu lực'
                LEFT JOIN HopDong_NguoiThue hnt ON hdong.MaHopDong = hnt.MaHopDong AND hnt.VaiTro = N'Chủ hợp đồng'
                LEFT JOIN NguoiThue nt ON hnt.MaNguoiThue = nt.MaNguoiThue
                -- Lấy thông tin thanh toán
                LEFT JOIN ThanhToan tt ON hd.MaHoaDon = tt.MaHoaDon

                WHERE hd.ThoiKy = @ThoiKy
            ";

            SqlParameter[] parameters = {
                new SqlParameter("@ThoiKy", thoiKy)
            };

            try
            {
                DataTable result = db.ExecuteQuery(sql, parameters);
                return result ?? new DataTable();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi lấy báo cáo: " + ex.Message);
                return new DataTable();
            }
        }
    }
}