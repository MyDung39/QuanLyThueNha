using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoomManagementSystem.DataLayer
{
    public class PhongDAL
    {
        private string connect = "Data Source=LAPTOP-JH9IJG9F\\SQLEXPRESS;Initial Catalog=QLTN;Integrated Security=True;Encrypt=True;Trust Server Certificate=True";

        // Phương thức mở rộng để xử lý an toàn giá trị từ DB
        private static string ToSafeString(object value)
        {
            return value == null || value is DBNull ? string.Empty : value.ToString() ?? string.Empty;
        }

        // Thêm phòng
        public bool InsertPhong(Phong phong)
        {
            using (SqlConnection c = new SqlConnection(connect))
            {
                c.Open();
                string qr = @"INSERT INTO Phong (MaPhong,MaNha, LoaiPhong, DienTich, GiaThue, TrangThai, GhiChu, NgayTao, NgayCapNhat)
                              VALUES (@MaPhong,@MaNha, @LoaiPhong, @DienTich, @GiaThue, @TrangThai, @GhiChu, GETDATE(), GETDATE())";

                SqlCommand cmd = new SqlCommand(qr, c);
                // Đảm bảo không truyền null cho tham số SQL
                cmd.Parameters.AddWithValue("@MaPhong", phong.MaPhong ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@MaNha", phong.MaNha ?? (object)DBNull.Value); 
                cmd.Parameters.AddWithValue("@LoaiPhong", phong.LoaiPhong ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@DienTich", phong.DienTich); 
                cmd.Parameters.AddWithValue("@GiaThue", phong.GiaThue); 
                cmd.Parameters.AddWithValue("@TrangThai", phong.TrangThai ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@GhiChu", phong.GhiChu ?? (object)DBNull.Value);

                int result = cmd.ExecuteNonQuery();
                return result > 0;
            }
        }

        // Cập nhật phòng
        public bool UpdatePhong(Phong phong)
        {
            using (SqlConnection c = new SqlConnection(connect))
            {
                c.Open();
                string qr = @"UPDATE Phong 
                              SET LoaiPhong = @LoaiPhong,
                                  DienTich = @DienTich,
                                  GiaThue = @GiaThue,
                                  TrangThai = @TrangThai,
                                  GhiChu = @GhiChu,
                                  NgayCapNhat = GETDATE()
                              WHERE MaPhong = @MaPhong";

                SqlCommand cmd = new SqlCommand(qr, c);
                cmd.Parameters.AddWithValue("@MaPhong", phong.MaPhong ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@LoaiPhong", phong.LoaiPhong ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@DienTich", phong.DienTich);
                cmd.Parameters.AddWithValue("@GiaThue", phong.GiaThue);
                cmd.Parameters.AddWithValue("@TrangThai", phong.TrangThai ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@GhiChu", phong.GhiChu ?? (object)DBNull.Value);

                int result = cmd.ExecuteNonQuery();
                return result > 0;
            }
        }

        // Xóa phòng
        public bool DeletePhong(string maPhong)
        {
            using (SqlConnection c = new SqlConnection(connect))
            {
                c.Open();
                string qr = "DELETE FROM Phong WHERE MaPhong = @MaPhong";
                SqlCommand cmd = new SqlCommand(qr, c);
                cmd.Parameters.AddWithValue("@MaPhong", maPhong ?? (object)DBNull.Value);

                int result = cmd.ExecuteNonQuery();
                return result > 0;
            }
        }

        // Lấy tất cả phòng
        public List<Phong> GetAllPhong()
        {
            List<Phong> ds = new List<Phong>();

            using (SqlConnection c = new SqlConnection(connect))
            {
                c.Open();
                string qr = "SELECT MaPhong, MaNha, LoaiPhong, DienTich, GiaThue, TrangThai, SoNguoiHienTai, GhiChu, NgayTao, NgayCapNhat FROM Phong";
                SqlCommand cmd = new SqlCommand(qr, c);
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    // Lấy giá trị an toàn
                    string maPhong = ToSafeString(reader["MaPhong"]);
                    string maNha = ToSafeString(reader["MaNha"]);
                    string loaiPhong = ToSafeString(reader["LoaiPhong"]);
                    string trangThai = ToSafeString(reader["TrangThai"]);
                    string ghiChu = ToSafeString(reader["GhiChu"]);

                    // Xử lý giá trị số và ngày tháng có thể là DBNull
                    float dienTich = reader["DienTich"] is DBNull ? 0.0f : float.Parse(reader["DienTich"].ToString()!);
                    float giaThue = reader["GiaThue"] is DBNull ? 0.0f : float.Parse(reader["GiaThue"].ToString()!);
                    int soNguoiHienTai = reader["SoNguoiHienTai"] is DBNull ? 0 : int.Parse(reader["SoNguoiHienTai"].ToString()!);

                    // Sử dụng DateTime.TryParse an toàn hơn nếu cột có thể NULL
                    DateTime ngayTao = reader["NgayTao"] is DBNull ? DateTime.MinValue : DateTime.Parse(reader["NgayTao"].ToString()!);
                    DateTime ngayCapNhat = reader["NgayCapNhat"] is DBNull ? DateTime.MinValue : DateTime.Parse(reader["NgayCapNhat"].ToString()!);


                    Phong p = new Phong()
                    {
                        MaPhong = maPhong,
                        MaNha = maNha, 
                        LoaiPhong = loaiPhong,
                        DienTich = dienTich,
                        GiaThue = giaThue,
                        TrangThai = trangThai,
                        SoNguoiHienTai = soNguoiHienTai,
                        GhiChu = ghiChu,
                        NgayTao = ngayTao,
                        NgayCapNhat = ngayCapNhat
                    };
                    ds.Add(p);
                }
            }

            return ds;
        }

        // Lấy phòng theo mã
        public Phong? GetPhongById(string maPhong) 
        {
            using (SqlConnection c = new SqlConnection(connect))
            {
                c.Open();
                string qr = "SELECT MaPhong, MaNha, LoaiPhong, DienTich, GiaThue, TrangThai, SoNguoiHienTai, GhiChu, NgayTao, NgayCapNhat FROM Phong WHERE MaPhong = @MaPhong";
                SqlCommand cmd = new SqlCommand(qr, c);
                cmd.Parameters.AddWithValue("@MaPhong", maPhong ?? (object)DBNull.Value);

                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    string maPhongResult = ToSafeString(reader["MaPhong"]);
                    string maNha = ToSafeString(reader["MaNha"]);
                    string loaiPhong = ToSafeString(reader["LoaiPhong"]);
                    string trangThai = ToSafeString(reader["TrangThai"]);
                    string ghiChu = ToSafeString(reader["GhiChu"]);

                    float dienTich = reader["DienTich"] is DBNull ? 0.0f : float.Parse(reader["DienTich"].ToString()!);
                    float giaThue = reader["GiaThue"] is DBNull ? 0.0f : float.Parse(reader["GiaThue"].ToString()!);
                    int soNguoiHienTai = reader["SoNguoiHienTai"] is DBNull ? 0 : int.Parse(reader["SoNguoiHienTai"].ToString()!);

                    DateTime ngayTao = reader["NgayTao"] is DBNull ? DateTime.MinValue : DateTime.Parse(reader["NgayTao"].ToString()!);
                    DateTime ngayCapNhat = reader["NgayCapNhat"] is DBNull ? DateTime.MinValue : DateTime.Parse(reader["NgayCapNhat"].ToString()!);

                    return new Phong()
                    {
                        MaPhong = maPhongResult,
                        MaNha = maNha,
                        LoaiPhong = loaiPhong,
                        DienTich = dienTich,
                        GiaThue = giaThue,
                        TrangThai = trangThai,
                        SoNguoiHienTai = soNguoiHienTai,
                        GhiChu = ghiChu,
                        NgayTao = ngayTao,
                        NgayCapNhat = ngayCapNhat
                    };
                }
                return null;
            }
        }
    }
}