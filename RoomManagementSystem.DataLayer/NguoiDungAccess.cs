using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;


namespace RoomManagementSystem.DataLayer
{
    public class NguoiDungAccess
    {
        string connect = "Data Source=DESKTOP-4JTJGR2\\SQLEXPRESS;Initial Catalog=QLTN;Integrated Security=True;Encrypt=True;Trust Server Certificate=True";
        
        //Kiem tra tai khoan, mat khau
        public Boolean checkDangNhap(string email, string matkhau)
        {
            using (SqlConnection c = new SqlConnection(connect))
            {
                c.Open();   
                string querry = "SELECT COUNT(*) FROM NguoiDung WHERE TenDangNhap=@email AND MatKhau=matkhau";
                SqlCommand cmd = new SqlCommand(querry, c);
                cmd.Parameters.AddWithValue("@email", email);
                cmd.Parameters.AddWithValue("@matkhau", matkhau);
                int count = (int)cmd.ExecuteScalar(); // Lấy giá trị COUNT(*)
                return count > 0; // true nếu tìm thấy, false nếu không
            }
        }

        //Kiem tra mail nguoi dung
        public Boolean Mail(string mail)
        {
            using (SqlConnection c = new SqlConnection(connect))
            {
                c.Open();
                string querry = "SELECT COUNT(*) FROM NguoiDung WHERE TenDangNhap=@mail";
                SqlCommand cmd = new SqlCommand(querry, c);
                cmd.Parameters.AddWithValue("@mail", mail);;
                int count = (int)cmd.ExecuteScalar(); // Lấy giá trị COUNT(*)
                return count > 0; // true nếu tìm thấy, false nếu không
            }
        }
        
        //Cap nhat lai du lieu NguoiDung
        public bool UpdatePassword(String matkhau)
        {
            using (SqlConnection conn = new SqlConnection(connect))
            {
                conn.Open();
                string query = "UPDATE NguoiDung SET MatKhau=@matkhau, NgayCapNhat=GETDATE()";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@matkhau", matkhau);
                return cmd.ExecuteNonQuery() > 0; // true nếu update thành công
            }
        }

        //Tra ve thong tin nguoi dung
        public NguoiDung GetNguoiDung()
        {
            using (SqlConnection conn = new SqlConnection(connect))
            {
                conn.Open();
                string q = "SELECT * FROM NguoiDung";
                SqlCommand c = new SqlCommand(q, conn);
                using (var r = c.ExecuteReader())
                {
                    if (r.Read())
                    {
                        return new NguoiDung
                        {
                            MaNguoiDung = r["MaNguoiDung"].ToString(),
                            TenDangNhap = r["TenDangNhap"].ToString(),
                            TenTaiKhoan = r["TenTaiKhoan"].ToString(),
                            MatKhau = r["MatKhau"].ToString(),
                            Sdt = r["Sdt"].ToString(),
                            PhuongThucDN = r["PhuongThucDN"].ToString(),
                            TrangThai = r["TrangThai"].ToString(),
                            NgayTao = Convert.ToDateTime(r["NgayTao"]),
                            NgaySaoLuu = Convert.ToDateTime(r["NgaySaoLuu"]),
                            NgayCapNhat = Convert.ToDateTime(r["NgayCapNhat"])
                        };
                    }
                }
            }
            return null;
        }

    }
}
