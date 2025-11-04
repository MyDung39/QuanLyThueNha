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
        public string con = "Data Source=LAPTOP-JH9IJG9F\\SQLEXPRESS;Initial Catalog=QLTN;Integrated Security=True;Encrypt=True;Trust Server Certificate=True";
        public DataTable GetDanhSachCongNo()
        {
            using(SqlConnection conn = new SqlConnection(con))
            {
                string query = @"
                SELECT 
                    kt.MaNguoiThue,
                    kt.HoTen,
                    p.MaPhong,
                    t.MaThanhToan,
                    t.TongCongNo,
                    t.SoTienDaThanhToan,
                    t.SoTienConLai,
                    t.NgayHanThanhToan,
                    t.TrangThai
                FROM ThanhToan t
                INNER JOIN HopDong hd ON t.MaHopDong = hd.MaHopDong
                INNER JOIN NguoiThue kt ON hd.MaNguoiThue = kt.MaNguoiThue
                INNER JOIN Phong p ON hd.MaPhong = p.MaPhong
                WHERE t.SoTienConLai > 0
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
            using (SqlConnection conn = new SqlConnection(con))
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
                WHERE hd.MaKhachThue = @MaKhach
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
