using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;

namespace RoomManagementSystem.DataLayer
{
    public class HopDongDAL
    {
        private string connect = "Data Source=LAPTOP-5FKFDEEM;Initial Catalog=QLTN;Integrated Security=True;Encrypt=True;Trust Server Certificate=True";

        // Tạo mã hợp đồng tự động
        public string AutoMaHD()
        {
            using (SqlConnection c = new SqlConnection(connect))
            {
                c.Open();
                // Lấy số tiếp theo từ SEQUENCE và định dạng nó
                string qr = "SELECT ISNULL(MAX(CAST(SUBSTRING(MaHopDong, 3, LEN(MaHopDong) - 2) AS INT)), 0) + 1 FROM HopDong"; ;
                SqlCommand cmd = new SqlCommand(qr, c);
                int nextNumber = Convert.ToInt32(cmd.ExecuteScalar());

                return "HD" + nextNumber.ToString("D3");

            }
        }

        // Thêm hợp đồng
        public bool InsertHopDong(HopDong hopDong)
        {
            using (SqlConnection c = new SqlConnection(connect))
            {
                c.Open();
                string qr = @"INSERT INTO HopDong (MaHopDong, MaPhong, MaNguoiThue, ChuNha, TienCoc, NgayBatDau, ThoiHan, TrangThai, GhiChu, NgayTao, NgayCapNhat)
                              VALUES (@MaHopDong, @MaPhong, @MaNguoiThue, @ChuNha, @TienCoc, @NgayBatDau, @ThoiHan, @TrangThai, @GhiChu, GETDATE(), GETDATE())";

                SqlCommand cmd = new SqlCommand(qr, c);
                cmd.Parameters.AddWithValue("@MaHopDong", hopDong.MaHopDong);
                cmd.Parameters.AddWithValue("@MaPhong", hopDong.MaPhong);
                cmd.Parameters.AddWithValue("@MaNguoiThue", hopDong.MaNguoiThue);
                cmd.Parameters.AddWithValue("@ChuNha", hopDong.ChuNha ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@TienCoc", hopDong.TienCoc);
                cmd.Parameters.AddWithValue("@NgayBatDau", hopDong.NgayBatDau);
                cmd.Parameters.AddWithValue("@ThoiHan", hopDong.ThoiHan); // Thêm ThoiHan
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
                                  ThoiHan = @ThoiHan,
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
                cmd.Parameters.AddWithValue("@ThoiHan", hopDong.ThoiHan); // Thêm ThoiHan
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
                        MaHopDong = reader["MaHopDong"]?.ToString(),
                        MaPhong = reader["MaPhong"]?.ToString(),
                        MaNguoiThue = reader["MaNguoiThue"]?.ToString(),
                        ChuNha = reader["ChuNha"]?.ToString(),
                        TienCoc = Convert.ToDecimal(reader["TienCoc"]),
                        NgayBatDau = Convert.ToDateTime(reader["NgayBatDau"]),
                        ThoiHan = Convert.ToInt32(reader["ThoiHan"]), // Thêm đọc ThoiHan
                        NgayKetThuc = reader["NgayKetThuc"] is DBNull ? default(DateTime) : Convert.ToDateTime(reader["NgayKetThuc"]),
                        TrangThai = reader["TrangThai"]?.ToString(),
                        GhiChu = reader["GhiChu"]?.ToString(),
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
                        MaHopDong = reader["MaHopDong"]?.ToString(),
                        MaPhong = reader["MaPhong"]?.ToString(),
                        MaNguoiThue = reader["MaNguoiThue"]?.ToString(),
                        ChuNha = reader["ChuNha"]?.ToString(),
                        TienCoc = Convert.ToDecimal(reader["TienCoc"]),
                        NgayBatDau = Convert.ToDateTime(reader["NgayBatDau"]),
                        ThoiHan = Convert.ToInt32(reader["ThoiHan"]), // Thêm đọc ThoiHan
                        NgayKetThuc = reader["NgayKetThuc"] is DBNull ? default(DateTime) : Convert.ToDateTime(reader["NgayKetThuc"]),
                        TrangThai = reader["TrangThai"]?.ToString(),
                        GhiChu = reader["GhiChu"]?.ToString(),
                        NgayTao = Convert.ToDateTime(reader["NgayTao"]),
                        NgayCapNhat = Convert.ToDateTime(reader["NgayCapNhat"])
                    };
                }
                return null;
            }
        }

        // Kiểm tra người thuê có phải là chủ hợp đồng không
        public bool IsChuHopDong(string maNguoiThue)
        {
            using (SqlConnection c = new SqlConnection(connect))
            {
                c.Open();
                string qr = "SELECT VaiTro FROM NguoiThue WHERE MaNguoiThue = @MaNguoiThue";
                SqlCommand cmd = new SqlCommand(qr, c);
                cmd.Parameters.AddWithValue("@MaNguoiThue", maNguoiThue);

                object result = cmd.ExecuteScalar();
                if (result != null && result.ToString() == "Chủ hợp đồng")
                {
                    return true;
                }
                return false;
            }
        }


        // Lấy thông tin chi tiết của hợp đồng để xuất ra file
        public HopDongXemIn? GetInHD(string maHopDong)
        {
            using (SqlConnection c = new SqlConnection(connect))
            {
                c.Open();
                string qr = @"SELECT 
                        nt.HoTen,
                        nt.SoGiayTo,
                        nt.NgayDonVao,
                        hd.NgayBatDau,
                        hd.NgayKetThuc,
                        hd.ThoiHan,
                        hd.TienCoc,
                        hd.FileDinhKem,
                        p.GiaThue,
                        p.DienTich,
                        n.DiaChi,
                        n.TongSoPhong
                      FROM HopDong hd
                      JOIN NguoiThue nt ON hd.MaNguoiThue = nt.MaNguoiThue
                      JOIN Phong p ON hd.MaPhong = p.MaPhong
                      JOIN Nha n ON p.MaNha = n.MaNha
                      WHERE hd.MaHopDong = @MaHopDong";

                SqlCommand cmd = new SqlCommand(qr, c);
                cmd.Parameters.AddWithValue("@MaHopDong", maHopDong);

                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    return new HopDongXemIn()
                    {
                        TenNguoiThue = reader["HoTen"]?.ToString(),
                        CccdNguoiThue = reader["SoGiayTo"]?.ToString(),
                        NgayDonVao = Convert.ToDateTime(reader["NgayDonVao"]),
                        NgayBatDau = Convert.ToDateTime(reader["NgayBatDau"]),
                        NgayKetThuc = reader["NgayKetThuc"] is DBNull ? default(DateTime) : Convert.ToDateTime(reader["NgayKetThuc"]),
                        ThoiHan = Convert.ToInt32(reader["ThoiHan"]), // Thêm đọc ThoiHan
                        TienCoc = Convert.ToDecimal(reader["TienCoc"]),
                        FileDinhKem = reader["FileDinhKem"]?.ToString(),
                        GiaThue = Convert.ToDecimal(reader["GiaThue"]),
                        DienTich = Convert.ToDecimal(reader["DienTich"]),
                        DiaChiNha = reader["DiaChi"]?.ToString(),
                        TongSoPhong = Convert.ToInt32(reader["TongSoPhong"])
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

        public string? GetMaNguoiThueBySoGiayTo(string soGiayTo)
        {
            using (SqlConnection c = new SqlConnection(connect))
            {
                c.Open();
                string qr = "SELECT MaNguoiThue FROM NguoiThue WHERE SoGiayTo = @SoGiayTo";
                SqlCommand cmd = new SqlCommand(qr, c);
                cmd.Parameters.AddWithValue("@SoGiayTo", soGiayTo);

                object result = cmd.ExecuteScalar();
                return result?.ToString();
            }
        }
    }
}