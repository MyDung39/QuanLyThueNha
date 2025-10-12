using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoomManagementSystem.DataLayer
{
    public class HopDongDAL
    {
        private string connect = "Data Source=LAPTOP-5FKFDEEM;Initial Catalog=QLTN;Integrated Security=True;Encrypt=True;Trust Server Certificate=True";

        // Thêm hợp đồng
        public bool InsertHopDong(HopDong hopDong)
        {
            using (SqlConnection c = new SqlConnection(connect))
            {
                c.Open();
                string qr = @"INSERT INTO HopDong (MaHopDong, MaPhong, MaNguoiThue, ChuNha, TienCoc, NgayBatDau, NgayKetThuc, FileDinhKem, TrangThai, GhiChu, NgayTao, NgayCapNhat)
                              VALUES (@MaHopDong, @MaPhong, @MaNguoiThue, @ChuNha, @TienCoc, @NgayBatDau, @NgayKetThuc, @FileDinhKem, @TrangThai, @GhiChu, GETDATE(), GETDATE())";

                SqlCommand cmd = new SqlCommand(qr, c);
                cmd.Parameters.AddWithValue("@MaHopDong", hopDong.MaHopDong);
                cmd.Parameters.AddWithValue("@MaPhong", hopDong.MaPhong);
                cmd.Parameters.AddWithValue("@MaNguoiThue", hopDong.MaNguoiThue);
                cmd.Parameters.AddWithValue("@ChuNha", hopDong.ChuNha ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@TienCoc", hopDong.TienCoc);
                cmd.Parameters.AddWithValue("@NgayBatDau", hopDong.NgayBatDau);
                cmd.Parameters.AddWithValue("@NgayKetThuc", hopDong.NgayKetThuc);
                cmd.Parameters.AddWithValue("@FileDinhKem", hopDong.FileDinhKem ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@TrangThai", hopDong.TrangThai ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@GhiChu", hopDong.GhiChu ?? (object)DBNull.Value);

                int result = cmd.ExecuteNonQuery();
                return result > 0;
            }
        }

        // Cập nhật hợp đồng
        public bool UpdateHopDong(HopDong hopDong)
        {
            using (SqlConnection c = new SqlConnection(connect))
            {
                c.Open();
                string qr = @"UPDATE HopDong 
                              SET MaPhong = @MaPhong,
                                  MaNguoiThue = @MaNguoiThue,
                                  ChuNha = @ChuNha,
                                  TienCoc = @TienCoc,
                                  NgayBatDau = @NgayBatDau,
                                  NgayKetThuc = @NgayKetThuc,
                                  FileDinhKem = @FileDinhKem,
                                  TrangThai = @TrangThai,
                                  GhiChu = @GhiChu,
                                  NgayCapNhat = GETDATE()
                              WHERE MaHopDong = @MaHopDong";

                SqlCommand cmd = new SqlCommand(qr, c);
                cmd.Parameters.AddWithValue("@MaHopDong", hopDong.MaHopDong);
                cmd.Parameters.AddWithValue("@MaPhong", hopDong.MaPhong);
                cmd.Parameters.AddWithValue("@MaNguoiThue", hopDong.MaNguoiThue);
                cmd.Parameters.AddWithValue("@ChuNha", hopDong.ChuNha ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@TienCoc", hopDong.TienCoc);
                cmd.Parameters.AddWithValue("@NgayBatDau", hopDong.NgayBatDau);
                cmd.Parameters.AddWithValue("@NgayKetThuc", hopDong.NgayKetThuc);
                cmd.Parameters.AddWithValue("@FileDinhKem", hopDong.FileDinhKem ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@TrangThai", hopDong.TrangThai ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@GhiChu", hopDong.GhiChu ?? (object)DBNull.Value);

                int result = cmd.ExecuteNonQuery();
                return result > 0;
            }
        }

        // Xóa hợp đồng
        public bool DeleteHopDong(string maHopDong)
        {
            using (SqlConnection c = new SqlConnection(connect))
            {
                c.Open();
                string qr = "DELETE FROM HopDong WHERE MaHopDong = @MaHopDong";
                SqlCommand cmd = new SqlCommand(qr, c);
                cmd.Parameters.AddWithValue("@MaHopDong", maHopDong);

                int result = cmd.ExecuteNonQuery();
                return result > 0;
            }
        }

        // Lấy tất cả hợp đồng
        public List<HopDong> GetAllHopDong()
        {
            List<HopDong> ds = new List<HopDong>();

            using (SqlConnection c = new SqlConnection(connect))
            {
                c.Open();
                string qr = "SELECT * FROM HopDong";
                SqlCommand cmd = new SqlCommand(qr, c);
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    HopDong hd = new HopDong()
                    {
                        MaHopDong = Convert.ToString(reader["MaHopDong"]),
                        MaPhong = Convert.ToString(reader["MaPhong"]),
                        MaNguoiThue = Convert.ToString(reader["MaNguoiThue"]),
                        ChuNha = Convert.ToString(reader["ChuNha"]),
                        TienCoc = Convert.ToSingle(reader["TienCoc"]),
                        NgayBatDau = Convert.ToDateTime(reader["NgayBatDau"]),
                        NgayKetThuc = reader["NgayKetThuc"] is DBNull ? default(DateTime) : Convert.ToDateTime(reader["NgayKetThuc"]),
                        FileDinhKem = Convert.ToString(reader["FileDinhKem"]),
                        TrangThai = Convert.ToString(reader["TrangThai"]),
                        GhiChu = Convert.ToString(reader["GhiChu"]),
                        NgayTao = Convert.ToDateTime(reader["NgayTao"]),
                        NgayCapNhat = Convert.ToDateTime(reader["NgayCapNhat"])
                    };
                    ds.Add(hd);
                }
            }

            return ds;
        }

        // Lấy hợp đồng theo mã
        public HopDong? GetHopDongById(string maHopDong)
        {
            using (SqlConnection c = new SqlConnection(connect))
            {
                c.Open();
                string qr = "SELECT * FROM HopDong WHERE MaHopDong = @MaHopDong";
                SqlCommand cmd = new SqlCommand(qr, c);
                cmd.Parameters.AddWithValue("@MaHopDong", maHopDong);

                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    return new HopDong()
                    {
                        MaHopDong = Convert.ToString(reader["MaHopDong"]),
                        MaPhong = Convert.ToString(reader["MaPhong"]),
                        MaNguoiThue = Convert.ToString(reader["MaNguoiThue"]),
                        ChuNha = Convert.ToString(reader["ChuNha"]),
                        TienCoc = Convert.ToSingle(reader["TienCoc"]),
                        NgayBatDau = Convert.ToDateTime(reader["NgayBatDau"]),
                        NgayKetThuc = reader["NgayKetThuc"] is DBNull ? default(DateTime) : Convert.ToDateTime(reader["NgayKetThuc"]),
                        FileDinhKem = Convert.ToString(reader["FileDinhKem"]),
                        TrangThai = Convert.ToString(reader["TrangThai"]),
                        GhiChu = Convert.ToString(reader["GhiChu"]),
                        NgayTao = Convert.ToDateTime(reader["NgayTao"]),
                        NgayCapNhat = Convert.ToDateTime(reader["NgayCapNhat"])
                    };
                }
                return null;
            }
        }

        // Tạo thông báo hết hạn (cho ThongBaoHan)
        public bool InsertThongBaoHan(string maThongBao, string maHopDong, string noiDung, DateTime ngayThongBao, string trangThai)
        {
            using (SqlConnection c = new SqlConnection(connect))
            {
                c.Open();
                string qr = @"INSERT INTO ThongBaoHan (MaThongBao, MaHopDong, NoiDung, NgayThongBao, TrangThai, NgayTao)
                              VALUES (@MaThongBao, @MaHopDong, @NoiDung, @NgayThongBao, @TrangThai, GETDATE())";

                SqlCommand cmd = new SqlCommand(qr, c);
                cmd.Parameters.AddWithValue("@MaThongBao", maThongBao);
                cmd.Parameters.AddWithValue("@MaHopDong", maHopDong);
                cmd.Parameters.AddWithValue("@NoiDung", noiDung);
                cmd.Parameters.AddWithValue("@NgayThongBao", ngayThongBao);
                cmd.Parameters.AddWithValue("@TrangThai", trangThai);

                int result = cmd.ExecuteNonQuery();
                return result > 0;
            }
        }
    }
}