using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoomManagementSystem.DataLayer
{
    public class CongNo
    {
        Database db = new Database();

        // Trong file RoomManagementSystem.DataLayer.CongNo.cs

        public DataTable GetDanhSachCongNo()
        {
            using (SqlConnection conn = new SqlConnection(DbConfig.ConnectionString))
            {
                // Thêm ISNULL vào các trường tiền bạc để tránh lỗi
                string query = @"
            SELECT 
                nt.MaNguoiThue,
                nt.HoTen,
                p.MaPhong,
                t.MaThanhToan,
                ISNULL(t.TongCongNo, 0) AS TongCongNo,          -- Sửa dòng này
                ISNULL(t.SoTienDaThanhToan, 0) AS SoTienDaThanhToan, -- Sửa dòng này
                ISNULL(t.SoTienConLai, 0) AS SoTienConLai,      -- Sửa dòng này
                t.NgayHanThanhToan,
                t.TrangThai
            FROM ThanhToan t
            INNER JOIN HopDong hd ON t.MaHopDong = hd.MaHopDong
            INNER JOIN HopDong_NguoiThue hnt ON hd.MaHopDong = hnt.MaHopDong AND hnt.VaiTro = N'Chủ hợp đồng'
            INNER JOIN NguoiThue nt ON hnt.MaNguoiThue = nt.MaNguoiThue
            INNER JOIN Phong p ON t.MaPhong = p.MaPhong
            WHERE t.SoTienConLai > 0
               OR t.TrangThai = N'Chưa trả'
            ORDER BY t.NgayHanThanhToan ASC;
        ";

                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
        }

        public DataTable GetLichSuThanhToan(string maKhach)
        {
            using (SqlConnection conn = new SqlConnection(DbConfig.ConnectionString))
            {
                string query = @"
                    SELECT 
                        t.MaThanhToan,
                        t.NgayTao,
                        t.PhuongThucThanhToan,
                        t.SoTienDaThanhToan,
                        t.SoTienConLai,
                        t.TrangThai
                    FROM ThanhToan t
                    INNER JOIN HopDong hd ON t.MaHopDong = hd.MaHopDong
                    INNER JOIN HopDong_NguoiThue hnt ON hd.MaHopDong = hnt.MaHopDong
                    WHERE hnt.MaNguoiThue = @MaKhach
                    ORDER BY t.NgayTao DESC;
                ";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@MaKhach", maKhach);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
        }
    }
}