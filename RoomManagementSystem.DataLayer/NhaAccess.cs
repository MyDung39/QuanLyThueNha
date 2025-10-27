using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoomManagementSystem.DataLayer
{
    public class NhaAccess
    {
        string connect = "Data Source=LAPTOP-JH9IJG9F\\SQLEXPRESS;Initial Catalog=QLTN;Integrated Security=True;Encrypt=True;Trust Server Certificate=True";
        // Phương thức mở rộng để xử lý an toàn giá trị từ DB
        private static string ToSafeString(object value)
        {
            return value == null || value is DBNull ? string.Empty : value.ToString() ?? string.Empty;
        }
        //Them thong tin can nha
        public Boolean registerHouse(string MaNha, string DiaChi, int SoPhong, int TongSoPhongHienTai, string GhiChu)
        {
            //Them thong tin vao Table Nha trong csdl
            using (SqlConnection c = new SqlConnection(connect))
            {
                c.Open();
                string a = "SELECT MaNguoiDung FROM NguoiDung";
                SqlCommand b = new SqlCommand(a, c);
                var MaNguoiDung = b.ExecuteScalar();

                string qr = @"INSERT INTO Nha (MaNguoiDung,MaNha, DiaChi, TongSoPhong, TongSoPhongHienTai, GhiChu, NgayTao, NgayCapNhat) 
                          VALUES (@MaNguoiDung,@MaNha, @DiaChi, @TongSoPhong, @TongSoPhongHienTai, @GhiChu, GETDATE(), GETDATE())";
                SqlCommand q = new SqlCommand(qr, c);
                q.Parameters.AddWithValue("@MaNguoiDung", MaNguoiDung.ToString());
                q.Parameters.AddWithValue("@MaNha", MaNha);
                q.Parameters.AddWithValue("@DiaChi", DiaChi);
                q.Parameters.AddWithValue("@TongSoPhong", SoPhong);
                q.Parameters.AddWithValue("@TongSoPhongHienTai", TongSoPhongHienTai);
                q.Parameters.AddWithValue("@GhiChu", GhiChu);
                
                int result = q.ExecuteNonQuery();
                return result > 0; // Trả về true nếu thêm thành công 
            }
        }
        //Cap nhat thong tin can nha
        public Boolean updateHouse(string MaNha, string DiaChi, int SoPhong, int TongSoPhongHienTai, string GhiChu)
        {
            using (SqlConnection c = new SqlConnection(connect))
            {
                c.Open();
                string a = "SELECT MaNguoiDung FROM NguoiDung";
                SqlCommand b = new SqlCommand(a, c);
                var MaNguoiDung = b.ExecuteScalar();

                string qr = @"UPDATE Nha SET MaNguoiDung = @MaNguoiDung,DiaChi = @DiaChi,TongSoPhong = @TongSoPhong,TongSoPhongHienTai = @TongSoPhongHienTai,GhiChu = @GhiChu,NgayCapNhat = GETDATE()  WHERE MaNha = @MaNha";
                SqlCommand q = new SqlCommand(qr, c);
                q.Parameters.AddWithValue("@MaNguoiDung", MaNguoiDung.ToString());
                q.Parameters.AddWithValue("@MaNha", MaNha);
                q.Parameters.AddWithValue("@DiaChi", DiaChi);
                q.Parameters.AddWithValue("@TongSoPhong", SoPhong);
                q.Parameters.AddWithValue("@TongSoPhongHienTai", TongSoPhongHienTai);
                q.Parameters.AddWithValue("@GhiChu", GhiChu);
                int result = q.ExecuteNonQuery();
                return result > 0; // Trả về true nếu  thành công 
            }
        }

        // Lấy tất cả phòng
        public List<Nha> getAllHouse()
        {
            List<Nha> ds = new List<Nha>();

            using (SqlConnection c = new SqlConnection(connect))
            {
                c.Open();
                string qr = "SELECT MaNha, MaNguoiDung, DiaChi, TongSoPhong, TongSoPhongHienTai, GhiChu, NgayTao, NgayCapNhat FROM Nha";
                SqlCommand cmd = new SqlCommand(qr, c);
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    // Lấy giá trị an toàn
                    string maNha = ToSafeString(reader["MaNha"]);
                    string maNguoiDung = ToSafeString(reader["MaNguoiDung"]);
                    string diaChi = ToSafeString(reader["DiaChi"]);
                    string ghiChu = ToSafeString(reader["GhiChu"]);

                    // Xử lý giá trị số và ngày tháng có thể là DBNull
                    int tongSoPhong = reader["TongSoPhong"] is DBNull ? 0 : int.Parse(reader["TongSoPhong"].ToString()!);
                    int tongSoPhongHienTai = reader["TongSoPhongHienTai"] is DBNull ? 0 : int.Parse(reader["TongSoPhongHienTai"].ToString()!);

                    // Sử dụng DateTime.TryParse an toàn hơn nếu cột có thể NULL
                    DateTime ngayTao = reader["NgayTao"] is DBNull ? DateTime.MinValue : DateTime.Parse(reader["NgayTao"].ToString()!);
                    DateTime ngayCapNhat = reader["NgayCapNhat"] is DBNull ? DateTime.MinValue : DateTime.Parse(reader["NgayCapNhat"].ToString()!);


                    Nha n = new Nha()
                    {
                        MaNha = maNha,
                        MaNguoiDung = maNguoiDung,
                        DiaChi = diaChi,
                        TongSoPhong = tongSoPhong,
                        TongSoPhongHienTai = tongSoPhong,
                        GhiChu = ghiChu,
                        NgayTao = ngayTao,
                        NgayCapNhat = ngayCapNhat
                    };
                    ds.Add(n);
                }
            }

            return ds;
        }
    }
}
