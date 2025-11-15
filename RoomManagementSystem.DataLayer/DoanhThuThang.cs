using Microsoft.Data.SqlClient;
using System;
using System.Data;

namespace RoomManagementSystem.DataLayer
{
    public class DoanhThuThang
    {
        Database db = new Database();

        /// <summary>
        /// Lấy báo cáo doanh thu tháng (ĐÃ SỬA LỖI: Luôn trả về DataTable hợp lệ và đúng tên cột)
        /// </summary>
        /// 


        public decimal TinhTongDoanhThuThang(int thang, int nam)
        {
            // Câu truy vấn này chỉ trả về một con số duy nhất: tổng doanh thu
            string sql = @"
            SELECT 
                SUM(tt.SoTienDaThanhToan) 
            FROM 
                ThanhToan tt
            WHERE 
                MONTH(tt.NgayHanThanhToan) = @Thang AND YEAR(tt.NgayHanThanhToan) = @Nam
                AND tt.TrangThai = N'Đã trả';
        ";

            SqlParameter[] parameters = {
            new SqlParameter("@Thang", thang),
            new SqlParameter("@Nam", nam)
        };

            try
            {
                // Sử dụng ExecuteScalar vì chúng ta chỉ mong đợi một giá trị duy nhất
                object result = db.ExecuteScalar(sql, parameters);

                // Kiểm tra kết quả và chuyển đổi
                if (result != null && result != DBNull.Value)
                {
                    return Convert.ToDecimal(result);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi khi tính tổng doanh thu: " + ex.Message);
            }

            return 0; // Trả về 0 nếu có lỗi hoặc không có doanh thu
        }


        public DataTable BaoCaoDoanhThuThang(int thang, int nam)
        {
            // ✅ SỬA LẠI SQL: Thêm ROW_NUMBER() để tạo cột STT
            // và dùng AS để đặt lại tên cột cho khớp với ViewModel/View
            string sql = @"
            SELECT
                ROW_NUMBER() OVER(ORDER BY p.MaPhong) AS STT,
                p.MaPhong AS [Phòng],
                nt.HoTen AS [Người thuê],
                p.GiaThue AS [Tiền thuê],
                tt.TrangThai AS [Tình trạng thanh toán]
            FROM
                ThanhToan tt
            JOIN Phong p ON tt.MaPhong = p.MaPhong
            JOIN HopDong hd ON p.MaPhong = hd.MaPhong AND hd.TrangThai = N'Hiệu lực'
            JOIN HopDong_NguoiThue hnt ON hd.MaHopDong = hnt.MaHopDong AND hnt.VaiTro = N'Chủ hợp đồng'
            JOIN NguoiThue nt ON hnt.MaNguoiThue = nt.MaNguoiThue
            WHERE
                MONTH(tt.NgayHanThanhToan) = @Thang AND YEAR(tt.NgayHanThanhToan) = @Nam;
        ";

            // ✅ THÊM ĐOẠN CODE NÀY ĐỂ TẠO THAM SỐ
            SqlParameter[] parameters = {
        new SqlParameter("@Thang", thang),
        new SqlParameter("@Nam", nam)
    };
            try
            {
                DataTable result = db.ExecuteQuery(sql, parameters);

                // ✅ THÊM KIỂM TRA NULL: Đảm bảo không bao giờ trả về null
                if (result == null)
                {
                    // Nếu có lỗi ở lớp Database, tạo một bảng trống
                    return new DataTable();
                }

                return result;
            }
            catch (Exception ex)
            {
                // Ghi log lỗi nếu cần
                Console.WriteLine("Lỗi trong DAL khi lấy báo cáo doanh thu: " + ex.Message);
                // Trả về một bảng trống khi có lỗi để ứng dụng không bị crash
                return new DataTable();
            }
        }
    }
}