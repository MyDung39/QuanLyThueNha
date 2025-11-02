using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;

namespace RoomManagementSystem.DataLayer
{
    public class HopDongDAL
    {
        // Khởi tạo đối tượng Database để sử dụng các hàm của nó
        Database db = new Database();

        // Tạo mã hợp đồng tự động
        public string AutoMaHD()
        {
            // Gọi hàm ExecuteScalar từ lớp Database
            string qr = "SELECT ISNULL(MAX(CAST(SUBSTRING(MaHopDong, 3, LEN(MaHopDong) - 2) AS INT)), 0) + 1 FROM HopDong";
            int nextNumber = Convert.ToInt32(db.ExecuteScalar(qr)); // Không cần tham số

            return "HD" + nextNumber.ToString("D3");
        }

        // Thêm hợp đồng
        public bool InsertHopDong(HopDong hopDong)
        {
            string qr = @"INSERT INTO HopDong (MaHopDong, MaPhong, MaNguoiThue, ChuNha, TienCoc, NgayBatDau, ThoiHan, TrangThai, GhiChu, NgayTao, NgayCapNhat)
                          VALUES (@MaHopDong, @MaPhong, @MaNguoiThue, @ChuNha, @TienCoc, @NgayBatDau, @ThoiHan, @TrangThai, @GhiChu, GETDATE(), GETDATE())";

            // Chuẩn bị mảng SqlParameter
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@MaHopDong", hopDong.MaHopDong),
                new SqlParameter("@MaPhong", hopDong.MaPhong),
                new SqlParameter("@MaNguoiThue", hopDong.MaNguoiThue),
                new SqlParameter("@ChuNha", hopDong.ChuNha ?? (object)DBNull.Value),
                new SqlParameter("@TienCoc", hopDong.TienCoc),
                new SqlParameter("@NgayBatDau", hopDong.NgayBatDau),
                new SqlParameter("@ThoiHan", hopDong.ThoiHan),
                new SqlParameter("@TrangThai", hopDong.TrangThai ?? (object)DBNull.Value),
                new SqlParameter("@GhiChu", hopDong.GhiChu ?? (object)DBNull.Value)
            };

            // Gọi hàm ExecuteNonQuery từ lớp Database
            int result = db.ExecuteNonQuery(qr, parameters);
            return result > 0;
        }

        // Cập nhật hợp đồng
        public bool UpdateHopDong(HopDong hopDong)
        {
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

            // Chuẩn bị mảng SqlParameter
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@MaHopDong", hopDong.MaHopDong),
                new SqlParameter("@MaPhong", hopDong.MaPhong),
                new SqlParameter("@MaNguoiThue", hopDong.MaNguoiThue),
                new SqlParameter("@ChuNha", hopDong.ChuNha ??(object) DBNull.Value),
                new SqlParameter("@TienCoc", hopDong.TienCoc),
                new SqlParameter("@NgayBatDau", hopDong.NgayBatDau),
                new SqlParameter("@ThoiHan", hopDong.ThoiHan),
                new SqlParameter("@TrangThai", hopDong.TrangThai ??(object) DBNull.Value),
                new SqlParameter("@GhiChu", hopDong.GhiChu ??(object) DBNull.Value)
            };

            // Gọi hàm ExecuteNonQuery từ lớp Database
            int result = db.ExecuteNonQuery(qr, parameters);
            return result > 0;
        }

        // Xóa hợp đồng
        public bool DeleteHopDong(string maHopDong)
        {
            string qr = "DELETE FROM HopDong WHERE MaHopDong = @MaHopDong";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@MaHopDong", maHopDong)
            };

            // Gọi hàm ExecuteNonQuery từ lớp Database
            int result = db.ExecuteNonQuery(qr, parameters);
            return result > 0;
        }

        // Lấy tất cả hợp đồng (Chuyển sang dùng DataTable)
        public List<HopDong> GetAllHopDong()
        {
            List<HopDong> ds = new List<HopDong>();
            string qr = "SELECT * FROM HopDong";

            // Gọi hàm ExecuteQuery để lấy về DataTable
            DataTable dt = db.ExecuteQuery(qr);

            // Duyệt qua DataTable (thay vì SqlDataReader)
            foreach (DataRow reader in dt.Rows)
            {
                HopDong hd = new HopDong()
                {
                    MaHopDong = reader["MaHopDong"]?.ToString(),
                    MaPhong = reader["MaPhong"]?.ToString(),
                    MaNguoiThue = reader["MaNguoiThue"]?.ToString(),
                    ChuNha = reader["ChuNha"]?.ToString(),
                    TienCoc = Convert.ToDecimal(reader["TienCoc"]),
                    NgayBatDau = Convert.ToDateTime(reader["NgayBatDau"]),
                    ThoiHan = Convert.ToInt32(reader["ThoiHan"]),
                    NgayKetThuc = reader["NgayKetThuc"] == DBNull.Value ? default(DateTime) : Convert.ToDateTime(reader["NgayKetThuc"]),
                    TrangThai = reader["TrangThai"]?.ToString(),
                    GhiChu = reader["GhiChu"]?.ToString(),
                    NgayTao = Convert.ToDateTime(reader["NgayTao"]),
                    NgayCapNhat = Convert.ToDateTime(reader["NgayCapNhat"])
                };
                ds.Add(hd);
            }

            return ds;
        }

        // Lấy hợp đồng theo mã (Chuyển sang dùng DataTable)
        public HopDong? GetHopDongById(string maHopDong)
        {
            string qr = "SELECT * FROM HopDong WHERE MaHopDong = @MaHopDong";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@MaHopDong", maHopDong)
            };

            // Lấy về DataTable
            DataTable dt = db.ExecuteQuery(qr, parameters);

            // Kiểm tra xem DataTable có dữ liệu không
            if (dt.Rows.Count > 0)
            {
                DataRow reader = dt.Rows[0]; // Lấy dòng đầu tiên
                return new HopDong()
                {
                    MaHopDong = reader["MaHopDong"]?.ToString(),
                    MaPhong = reader["MaPhong"]?.ToString(),
                    MaNguoiThue = reader["MaNguoiThue"]?.ToString(),
                    ChuNha = reader["ChuNha"]?.ToString(),
                    TienCoc = Convert.ToDecimal(reader["TienCoc"]),
                    NgayBatDau = Convert.ToDateTime(reader["NgayBatDau"]),
                    ThoiHan = Convert.ToInt32(reader["ThoiHan"]),
                    NgayKetThuc = reader["NgayKetThuc"] == DBNull.Value ? default(DateTime) : Convert.ToDateTime(reader["NgayKetThuc"]),
                    TrangThai = reader["TrangThai"]?.ToString(),
                    GhiChu = reader["GhiChu"]?.ToString(),
                    NgayTao = Convert.ToDateTime(reader["NgayTao"]),
                    NgayCapNhat = Convert.ToDateTime(reader["NgayCapNhat"])
                };
            }
            return null;
        }

        // Kiểm tra người thuê có phải là chủ hợp đồng không
        public bool IsChuHopDong(string maNguoiThue)
        {
            string qr = "SELECT VaiTro FROM NguoiThue WHERE MaNguoiThue = @MaNguoiThue";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@MaNguoiThue", maNguoiThue)
            };

            // Gọi hàm ExecuteScalar
            object result = db.ExecuteScalar(qr, parameters);
            if (result != null && result.ToString() == "Chủ hợp đồng")
            {
                return true;
            }
            return false;
        }

        // Lấy thông tin chi tiết của hợp đồng để xuất ra file (Chuyển sang dùng DataTable)
        public HopDongXemIn? GetInHD(string maHopDong)
        {
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

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@MaHopDong", maHopDong)
            };

            // Lấy về DataTable
            DataTable dt = db.ExecuteQuery(qr, parameters);

            if (dt.Rows.Count > 0)
            {
                DataRow reader = dt.Rows[0]; // Lấy dòng đầu tiên
                return new HopDongXemIn()
                {
                    TenNguoiThue = reader["HoTen"]?.ToString(),
                    CccdNguoiThue = reader["SoGiayTo"]?.ToString(),
                    NgayDonVao = Convert.ToDateTime(reader["NgayDonVao"]),
                    NgayBatDau = Convert.ToDateTime(reader["NgayBatDau"]),
                    NgayKetThuc = reader["NgayKetThuc"] == DBNull.Value ? default(DateTime) : Convert.ToDateTime(reader["NgayKetThuc"]),
                    ThoiHan = Convert.ToInt32(reader["ThoiHan"]),
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

        // Tạo thông báo hết hạn (cho ThongBaoHan)
        public bool InsertThongBaoHan(string maThongBao, string maHopDong, string noiDung, DateTime ngayThongBao, string trangThai)
        {
            string qr = @"INSERT INTO ThongBaoHan (MaThongBao, MaHopDong, NoiDung, NgayThongBao, TrangThai, NgayTao)
                          VALUES (@MaThongBao, @MaHopDong, @NoiDung, @NgayThongBao, @TrangThai, GETDATE())";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@MaThongBao", maThongBao),
                new SqlParameter("@MaHopDong", maHopDong),
                new SqlParameter("@NoiDung", noiDung),
                new SqlParameter("@NgayThongBao", ngayThongBao),
                new SqlParameter("@TrangThai", trangThai)
            };

            // Gọi hàm ExecuteNonQuery
            int result = db.ExecuteNonQuery(qr, parameters);
            return result > 0;
        }

        public string? GetMaNguoiThueBySoGiayTo(string soGiayTo)
        {
            string qr = "SELECT MaNguoiThue FROM NguoiThue WHERE SoGiayTo = @SoGiayTo";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@SoGiayTo", soGiayTo)
            };

            // Gọi hàm ExecuteScalar
            object result = db.ExecuteScalar(qr, parameters);
            return result?.ToString();
        }
    }
}