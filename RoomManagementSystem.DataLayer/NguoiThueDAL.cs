using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoomManagementSystem.DataLayer
{
    public class NguoiThueDAL
    {
        private string connect = "Data Source=LAPTOP-5FKFDEEM;Initial Catalog=QLTN;Integrated Security=True;Encrypt=True;Trust Server Certificate=True";
        //Nhập thông tin nguoi thue
        public bool ThemNguoiThue(NguoiThue nt)
        {
            using (SqlConnection conn = new SqlConnection(connect))
            {
                conn.Open();
                string sql = @"INSERT INTO NguoiThue
                               (MaNguoiThue, MaPhong, HoTen, SoDienThoai, Email, SoGiayTo,
                                NgayBatDauThue, TrangThaiThue, 
                                NgayDonVao, NgayDonRa, NgayTao, NgayCapNhat)
                               VALUES
                               (@MaNguoiThue, @MaPhong, @HoTen, @SoDienThoai, @Email, @SoGiayTo,
                                @NgayBatDauThue, @TrangThaiThue, 
                                @NgayDonVao, @NgayDonRa, @NgayTao, GETDATE())";
                SqlCommand cmd = new SqlCommand(sql, conn);

                cmd.Parameters.AddWithValue("@MaNguoiThue", nt.MaNguoiThue);
                cmd.Parameters.AddWithValue("@MaPhong", nt.MaPhong);
                cmd.Parameters.AddWithValue("@HoTen", nt.HoTen);
                cmd.Parameters.AddWithValue("@SoDienThoai", nt.Sdt);
                cmd.Parameters.AddWithValue("@Email", nt.Email);
                cmd.Parameters.AddWithValue("@SoGiayTo", nt.SoGiayTo);
                cmd.Parameters.AddWithValue("@NgayBatDauThue", nt.NgayBatDauThue);
                cmd.Parameters.AddWithValue("@TrangThaiThue", nt.TrangThaiThue);
                cmd.Parameters.AddWithValue("@NgayDonVao", nt.NgayDonVao);
                cmd.Parameters.AddWithValue("@NgayDonRa", (object?)nt.NgayDonRa ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@NgayTao", nt.NgayTao);
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        //Cap nhat thong tin nguoi thue, tinh trang thue, ngay tra phong
        public bool CapNhatNguoiThue(NguoiThue nt)
        {
            using (SqlConnection conn = new SqlConnection(connect))
            {
                string sql = @"UPDATE NguoiThue
                               SET
                               MaPhong=@MaPhong, 
                               HoTen=@HoTen, 
                               SoDienThoai=@Sdt, 
                               Email=@Email,
                               SoGiayTo=@SoGiayTo,
                               NgayBatDauThue=@NgayBatDauThue,
                               TrangThaiThue=@TrangThaiThue,
                               NgayDonVao= @NgayDonVao, 
                               NgayDonRa=@NgayDonRa, 
                               NgayTao= @NgayTao, 
                               NgayCapNhat=GETDATE()
                               WHERE MaNguoiThue=@MaNguoiThue";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@MaNguoiThue", nt.MaNguoiThue);
                cmd.Parameters.AddWithValue("@MaPhong", nt.MaPhong);
                cmd.Parameters.AddWithValue("@HoTen", nt.HoTen);
                cmd.Parameters.AddWithValue("@Sdt", nt.Sdt);
                cmd.Parameters.AddWithValue("@Email", nt.Email);
                cmd.Parameters.AddWithValue("@SoGiayTo", nt.SoGiayTo);
                cmd.Parameters.AddWithValue("@NgayBatDauThue", nt.NgayBatDauThue);
                cmd.Parameters.AddWithValue("@TrangThaiThue", nt.TrangThaiThue);
                cmd.Parameters.AddWithValue("@NgayDonVao", nt.NgayDonVao);
                cmd.Parameters.AddWithValue("@NgayDonRa", (object?)nt.NgayDonRa ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@NgayTao", nt.NgayTao);
                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        //Tra ve danh sach nguoi thue
        public List<NguoiThue> getAllNguoiThue()
        {
            List<NguoiThue> ds = new List<NguoiThue>();
            using (SqlConnection c = new SqlConnection(connect))
            {
                string q = "SELECT * FROM NguoiThue";
                c.Open();
                SqlCommand cmd = new SqlCommand(q, c);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    NguoiThue nt = new NguoiThue
                    {
                        MaNguoiThue = reader["MaNguoiThue"].ToString(),
                        MaPhong = reader["MaPhong"].ToString(),
                        HoTen = reader["HoTen"].ToString(),
                        Sdt = reader["SoDienThoai"].ToString(),
                        Email = reader["Email"].ToString(),
                        SoGiayTo = reader["SoGiayTo"].ToString(),
                        NgayBatDauThue = Convert.ToDateTime(reader["NgayBatDauThue"]),
                        TrangThaiThue = reader["TrangThaiThue"].ToString(),
                        NgayDonVao = reader["NgayDonVao"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["NgayDonVao"]),
                        NgayDonRa = reader["NgayDonRa"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["NgayDonRa"]),
                        NgayTao = Convert.ToDateTime(reader["NgayTao"]),
                        NgayCapNhat = Convert.ToDateTime(reader["NgayCapNhat"]),
                    };

                    ds.Add(nt);
                }
            }
            return ds;
        }


        ////Tra ve danh sach nguoi thue theo phong
        public List<NguoiThue> getNguoiThueByPhong(NguoiThue a)
        {
            List<NguoiThue> ds = new List<NguoiThue>();
            using (SqlConnection c = new SqlConnection(connect))
            {
                string q = "SELECT * FROM NguoiThue WHERE MaPhong=@MaPhong";
                c.Open();
                SqlCommand cmd = new SqlCommand(q, c);
                cmd.Parameters.AddWithValue("@MaPhong", a.MaPhong);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    NguoiThue nt = new NguoiThue
                    {
                        MaNguoiThue = reader["MaNguoiThue"].ToString(),
                        MaPhong = reader["MaPhong"].ToString(),
                        HoTen = reader["HoTen"].ToString(),
                        Sdt = reader["SoDienThoai"].ToString(),
                        Email = reader["Email"].ToString(),
                        SoGiayTo = reader["SoGiayTo"].ToString(),
                        NgayBatDauThue = Convert.ToDateTime(reader["NgayBatDauThue"]),
                        TrangThaiThue = reader["TrangThaiThue"].ToString(),

                        // === SỬA LỖI Ở ĐÂY ===
                        // Phương thức này cũng cần sửa
                        NgayDonVao = reader["NgayDonVao"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["NgayDonVao"]),
                        NgayDonRa = reader["NgayDonRa"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["NgayDonRa"]),

                        NgayTao = Convert.ToDateTime(reader["NgayTao"]),
                        NgayCapNhat = Convert.ToDateTime(reader["NgayCapNhat"]),

                    };

                    ds.Add(nt);
                }
            }
            return ds;
        }
    }
}
