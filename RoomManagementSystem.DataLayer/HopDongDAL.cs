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
            string qr = @"INSERT INTO HopDong (MaHopDong, MaPhong, ChuNha, TienCoc, NgayBatDau, ThoiHan, TrangThai, GhiChu, NgayTao, NgayCapNhat)
                          VALUES (@MaHopDong, @MaPhong, @ChuNha, @TienCoc, @NgayBatDau, @ThoiHan, @TrangThai, @GhiChu, GETDATE(), GETDATE())";

            // Chuẩn bị mảng SqlParameter
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@MaHopDong", hopDong.MaHopDong),
                new SqlParameter("@MaPhong", hopDong.MaPhong),
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

        // Thêm chi tiết người thuê vào hợp đồng
        public bool InsertHopDongNguoiThue(HopDong_NguoiThue ct)
        {
            string qr = @"INSERT INTO HopDong_NguoiThue (MaHopDong, MaNguoiThue, VaiTro, TrangThaiThue, NgayDonVao, NgayBatDauThue)
                          VALUES (@MaHopDong, @MaNguoiThue, @VaiTro, @TrangThaiThue, @NgayDonVao, @NgayBatDauThue)";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@MaHopDong", ct.MaHopDong),
                new SqlParameter("@MaNguoiThue", ct.MaNguoiThue),
                new SqlParameter("@VaiTro", ct.VaiTro),
                new SqlParameter("@TrangThaiThue", ct.TrangThaiThue),
                new SqlParameter("@NgayDonVao", ct.NgayDonVao ?? (object)DBNull.Value),
                new SqlParameter("@NgayBatDauThue", ct.NgayBatDauThue ??(object) DBNull.Value)
            };
            return db.ExecuteNonQuery(qr, parameters) > 0;
        }

        // Cập nhật hợp đồng
        public bool UpdateHopDong(HopDong hopDong)
        {
            string qr = @"UPDATE HopDong 
                          SET MaPhong = @MaPhong,
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

        // Lấy tất cả hợp đồng
        /*    public List<HopDong> GetAllHopDong()
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
        */

        public List<HopDong> GetAllHopDong()
        {
            List<HopDong> ds = new List<HopDong>();
            // ✅ THÊM CỘT FileDinhKem VÀO CÂU SELECT
            string qr = "SELECT MaHopDong, MaPhong, ChuNha, TienCoc, NgayBatDau, ThoiHan, NgayKetThuc, TrangThai, GhiChu, NgayTao, NgayCapNhat, FileDinhKem FROM HopDong";
            DataTable dt = db.ExecuteQuery(qr);

            foreach (DataRow reader in dt.Rows)
            {
                HopDong hd = new HopDong()
                {
                    MaHopDong = reader["MaHopDong"]?.ToString(),
                    MaPhong = reader["MaPhong"]?.ToString(),
                    ChuNha = reader["ChuNha"]?.ToString(),
                    TienCoc = Convert.ToDecimal(reader["TienCoc"]),
                    NgayBatDau = Convert.ToDateTime(reader["NgayBatDau"]),
                    ThoiHan = Convert.ToInt32(reader["ThoiHan"]),
                    NgayKetThuc = reader["NgayKetThuc"] == DBNull.Value ? default : Convert.ToDateTime(reader["NgayKetThuc"]),

                    // ✅ THÊM DÒNG NÀY ĐỂ ĐỌC TÊN FILE
                    FileDinhKem = reader["FileDinhKem"]?.ToString(),

                    TrangThai = reader["TrangThai"]?.ToString(),
                    GhiChu = reader["GhiChu"]?.ToString(),
                    NgayTao = Convert.ToDateTime(reader["NgayTao"]),
                    NgayCapNhat = Convert.ToDateTime(reader["NgayCapNhat"])
                };
                ds.Add(hd);
            }
            return ds;
        }

        // Lấy hợp đồng theo mã hợp đồng
        /*
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
        */

        public HopDong? GetHopDongById(string maHopDong)
        {
            // ✅ THÊM CỘT FileDinhKem VÀO CÂU SELECT
            string qr = "SELECT MaHopDong, MaPhong, ChuNha, TienCoc, NgayBatDau, ThoiHan, NgayKetThuc, TrangThai, GhiChu, NgayTao, NgayCapNhat, FileDinhKem FROM HopDong WHERE MaHopDong = @MaHopDong";
            SqlParameter[] parameters = new SqlParameter[] { new SqlParameter("@MaHopDong", maHopDong) };
            DataTable dt = db.ExecuteQuery(qr, parameters);

            if (dt.Rows.Count > 0)
            {
                DataRow reader = dt.Rows[0];
                return new HopDong()
                {
                    MaHopDong = reader["MaHopDong"]?.ToString(),
                    MaPhong = reader["MaPhong"]?.ToString(),
                    ChuNha = reader["ChuNha"]?.ToString(),
                    TienCoc = Convert.ToDecimal(reader["TienCoc"]),
                    NgayBatDau = Convert.ToDateTime(reader["NgayBatDau"]),
                    ThoiHan = Convert.ToInt32(reader["ThoiHan"]),
                    NgayKetThuc = reader["NgayKetThuc"] == DBNull.Value ? default : Convert.ToDateTime(reader["NgayKetThuc"]),

                    // ✅ THÊM DÒNG NÀY ĐỂ ĐỌC TÊN FILE
                    FileDinhKem = reader["FileDinhKem"]?.ToString(),

                    TrangThai = reader["TrangThai"]?.ToString(),
                    GhiChu = reader["GhiChu"]?.ToString(),
                    NgayTao = Convert.ToDateTime(reader["NgayTao"]),
                    NgayCapNhat = Convert.ToDateTime(reader["NgayCapNhat"])
                };
            }
            return null;
        }

        // Lấy hợp đồng theo mã phòng
        /*
        public List<HopDong> GetHopDongByMaPhong(string maPhong)
        {
            List<HopDong> ds = new List<HopDong>();
            string qr = "SELECT * FROM HopDong WHERE MaPhong = @MaPhong";

            // Cần tạo và truyền tham số @MaPhong cho câu truy vấn
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@MaPhong", maPhong)
            };

            // Gọi hàm ExecuteQuery để lấy về DataTable, truyền tham số vào
            DataTable dt = db.ExecuteQuery(qr, parameters);

            // Duyệt qua DataTable (thay vì SqlDataReader)
            foreach (DataRow reader in dt.Rows)
            {
                HopDong hd = new HopDong()
                {
                    MaHopDong = reader["MaHopDong"]?.ToString(),
                    MaPhong = reader["MaPhong"]?.ToString(),
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
        */


        public List<HopDong> GetHopDongByMaPhong(string maPhong)
        {
            List<HopDong> ds = new List<HopDong>();
            // ✅ SỬA LẠI: SELECT * để lấy tất cả các cột
            string qr = "SELECT * FROM HopDong WHERE MaPhong = @MaPhong";

            SqlParameter[] parameters = new SqlParameter[]
            {
        new SqlParameter("@MaPhong", maPhong)
            };
            DataTable dt = db.ExecuteQuery(qr, parameters);

            foreach (DataRow reader in dt.Rows)
            {
                HopDong hd = new HopDong()
                {
                    // ✅ ĐẢM BẢO ĐỌC ĐỦ CÁC CỘT CẦN THIẾT CHO LỊCH SỬ
                    MaHopDong = reader["MaHopDong"]?.ToString(),
                    MaPhong = reader["MaPhong"]?.ToString(),
                    NgayBatDau = Convert.ToDateTime(reader["NgayBatDau"]),
                    NgayKetThuc = reader["NgayKetThuc"] == DBNull.Value ? default : Convert.ToDateTime(reader["NgayKetThuc"]),
                    TienCoc = Convert.ToDecimal(reader["TienCoc"]),
                    NgayCapNhat = Convert.ToDateTime(reader["NgayCapNhat"]), // <<== DÒNG QUAN TRỌNG
                                                                             // Các thuộc tính khác nếu cần
                    ChuNha = reader["ChuNha"]?.ToString(),
                    ThoiHan = Convert.ToInt32(reader["ThoiHan"]),
                    TrangThai = reader["TrangThai"]?.ToString(),
                    GhiChu = reader["GhiChu"]?.ToString(),
                    NgayTao = Convert.ToDateTime(reader["NgayTao"])
                };
                ds.Add(hd);
            }
            return ds;
        }


        // Lấy thông tin chi tiết của hợp đồng để xuất ra file
        public HopDongXemIn? GetInHD(string maHopDong)
        {
            // Query 1: Lấy thông tin Hợp đồng, Phòng, Nhà, và CHỦ HỢP ĐỒNG
            string qr = @"SELECT 
                        nt.HoTen, nt.SoGiayTo, nt.SoDienThoai, nt.Email,
                        ct.NgayDonVao,
                        hd.MaHopDong, hd.NgayBatDau, hd.NgayKetThuc, hd.ThoiHan, hd.TienCoc, hd.FileDinhKem, hd.GhiChu, hd.NgayTao,
                        p.MaPhong, p.GiaThue, p.DienTich,
                        n.DiaChi
                      FROM HopDong hd
                      JOIN Phong p ON hd.MaPhong = p.MaPhong
                      JOIN Nha n ON p.MaNha = n.MaNha
                      JOIN HopDong_NguoiThue ct ON hd.MaHopDong = ct.MaHopDong
                      JOIN NguoiThue nt ON ct.MaNguoiThue = nt.MaNguoiThue
                      WHERE hd.MaHopDong = @MaHopDong AND ct.VaiTro = N'Chủ hợp đồng'";

            SqlParameter[] parameters = new SqlParameter[] { new SqlParameter("@MaHopDong", maHopDong) };
            DataTable dt = db.ExecuteQuery(qr, parameters);

            if (dt.Rows.Count > 0)
            {
                DataRow reader = dt.Rows[0];

                // ✅ ĐỐI TƯỢNG ĐÃ CẬP NHẬT ĐỂ ĐỌC THÊM CỘT
                HopDongXemIn hdXemIn = new HopDongXemIn()
                {
                    MaHopDong = reader["MaHopDong"]?.ToString(),
                    MaPhong = reader["MaPhong"]?.ToString(),
                    TenNguoiThue = reader["HoTen"]?.ToString(),
                    CccdNguoiThue = reader["SoGiayTo"]?.ToString(),
                    SdtNguoiThue = reader["SoDienThoai"]?.ToString(),
                    EmailNguoiThue = reader["Email"]?.ToString(),
                    GhiChuHopDong = reader["GhiChu"]?.ToString(),
                    NgayTao = Convert.ToDateTime(reader["NgayTao"]),
                    NgayDonVao = Convert.ToDateTime(reader["NgayDonVao"]),
                    NgayBatDau = Convert.ToDateTime(reader["NgayBatDau"]),
                    NgayKetThuc = reader["NgayKetThuc"] == DBNull.Value ? default : Convert.ToDateTime(reader["NgayKetThuc"]),
                    ThoiHan = Convert.ToInt32(reader["ThoiHan"]),
                    TienCoc = Convert.ToDecimal(reader["TienCoc"]),
                    FileDinhKem = reader["FileDinhKem"]?.ToString(),
                    GiaThue = Convert.ToDecimal(reader["GiaThue"]),
                    DienTich = Convert.ToDecimal(reader["DienTich"]),
                    DiaChiNha = reader["DiaChi"]?.ToString(),
                    ThanhVien = new List<ThanhVienCungPhong>()
                };

                // Query 2: Lấy thông tin NGƯỜI Ở CÙNG
                string qr_thanhvien = @"SELECT nt.HoTen, nt.SoGiayTo 
                                    FROM HopDong_NguoiThue ct
                                    JOIN NguoiThue nt ON ct.MaNguoiThue = nt.MaNguoiThue 
                                    WHERE ct.MaHopDong = @MaHopDong 
                                    AND ct.VaiTro = N'Người ở cùng'";

                SqlParameter[] param_thanhvien = new SqlParameter[]
                {
                    new SqlParameter("@MaHopDong", maHopDong)
                };

                DataTable dt_thanhvien = db.ExecuteQuery(qr_thanhvien, param_thanhvien);

                foreach (DataRow tvRow in dt_thanhvien.Rows)
                {
                    hdXemIn.ThanhVien.Add(new ThanhVienCungPhong
                    {
                        HoTen = tvRow["HoTen"]?.ToString(),
                        Cccd = tvRow["SoGiayTo"]?.ToString()
                    });
                }

                return hdXemIn;
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



        public List<HopDong_NguoiThue> GetAllHopDongNguoiThue()
        {
            List<HopDong_NguoiThue> ds = new List<HopDong_NguoiThue>();
            string qr = "SELECT MaHopDong, MaNguoiThue, VaiTro, TrangThaiThue, NgayDonVao, NgayDonRa, NgayBatDauThue FROM HopDong_NguoiThue";
            DataTable dt = db.ExecuteQuery(qr);

            foreach (DataRow row in dt.Rows)
            {
                ds.Add(new HopDong_NguoiThue
                {
                    MaHopDong = row["MaHopDong"].ToString(),
                    MaNguoiThue = row["MaNguoiThue"].ToString(),
                    VaiTro = row["VaiTro"].ToString(),
                    TrangThaiThue = row["TrangThaiThue"].ToString(),
                    NgayDonVao = row["NgayDonVao"] as DateTime?,
                    NgayDonRa = row["NgayDonRa"] as DateTime?,
                    NgayBatDauThue = row["NgayBatDauThue"] as DateTime?
                });
            }
            return ds;
        }


        public List<HopDong_NguoiThue> GetActiveContractDetails()
        {
            List<HopDong_NguoiThue> list = new List<HopDong_NguoiThue>();
            string sql = @"SELECT hnt.MaNguoiThue 
                   FROM HopDong_NguoiThue hnt
                   JOIN HopDong h ON hnt.MaHopDong = h.MaHopDong
                   WHERE h.TrangThai = N'Hiệu lực'";
            DataTable dt = db.ExecuteQuery(sql);
            foreach (DataRow row in dt.Rows)
            {
                list.Add(new HopDong_NguoiThue { MaNguoiThue = row["MaNguoiThue"].ToString() });
            }
            return list;
        }

        public bool UpdateContractStatus(string maHopDong, string newStatus)
        {
            string sql = "UPDATE HopDong SET TrangThai = @TrangThai, NgayCapNhat = GETDATE() WHERE MaHopDong = @MaHopDong";
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@TrangThai", newStatus),
                new SqlParameter("@MaHopDong", maHopDong)
            };
            return db.ExecuteNonQuery(sql, parameters) > 0;
        }



        public List<HopDong_NguoiThue> GetTenantsByContractId(string maHopDong)
        {
            List<HopDong_NguoiThue> ds = new List<HopDong_NguoiThue>();
            string qr = "SELECT * FROM HopDong_NguoiThue WHERE MaHopDong = @MaHopDong";
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@MaHopDong", maHopDong)
            };
            DataTable dt = db.ExecuteQuery(qr, parameters);

            foreach (DataRow row in dt.Rows)
            {
                ds.Add(new HopDong_NguoiThue
                {
                    MaHopDong = row["MaHopDong"].ToString(),
                    MaNguoiThue = row["MaNguoiThue"].ToString(),
                    VaiTro = row["VaiTro"].ToString(),
                    TrangThaiThue = row["TrangThaiThue"].ToString(),
                    NgayDonVao = row["NgayDonVao"] as DateTime?,
                    NgayDonRa = row["NgayDonRa"] as DateTime?,
                    NgayBatDauThue = row["NgayBatDauThue"] as DateTime?
                });
            }
            return ds;
        }


        public HopDong? GetActiveContractByTenantId(string maNguoiThue)
        {
            // Câu truy vấn này JOIN 3 bảng để tìm MaPhong từ MaNguoiThue
            string qr = @"SELECT TOP 1 h.* 
                  FROM HopDong h
                  JOIN HopDong_NguoiThue hnt ON h.MaHopDong = hnt.MaHopDong
                  WHERE hnt.MaNguoiThue = @MaNguoiThue AND h.TrangThai = N'Hiệu lực'
                  ORDER BY h.NgayBatDau DESC"; // Lấy hợp đồng mới nhất nếu có nhiều

            SqlParameter[] parameters = new SqlParameter[]
            {
        new SqlParameter("@MaNguoiThue", maNguoiThue)
            };

            DataTable dt = db.ExecuteQuery(qr, parameters);

            if (dt.Rows.Count > 0)
            {
                DataRow reader = dt.Rows[0];
                // Sử dụng lại logic đọc HopDong từ các phương thức khác của bạn
                return new HopDong()
                {
                    MaHopDong = reader["MaHopDong"]?.ToString(),
                    MaPhong = reader["MaPhong"]?.ToString(),
                    // ... điền các thuộc tính khác
                };
            }
            return null;
        }



        // HÀM MỚI: Dùng cho giao dịch
        public bool InsertHopDong(HopDong hopDong, SqlConnection conn, SqlTransaction tran)
        {
            string qr = @"INSERT INTO HopDong (MaHopDong, MaPhong, ChuNha, TienCoc, NgayBatDau, ThoiHan, TrangThai, GhiChu, NgayTao, NgayCapNhat)
                  VALUES (@MaHopDong, @MaPhong, @ChuNha, @TienCoc, @NgayBatDau, @ThoiHan, @TrangThai, @GhiChu, GETDATE(), GETDATE())";

            SqlParameter[] parameters = new SqlParameter[]
            {
        new SqlParameter("@MaHopDong", hopDong.MaHopDong),
        new SqlParameter("@MaPhong", hopDong.MaPhong),
        new SqlParameter("@ChuNha", hopDong.ChuNha ?? (object)DBNull.Value),
        new SqlParameter("@TienCoc", hopDong.TienCoc),
        new SqlParameter("@NgayBatDau", hopDong.NgayBatDau),
        new SqlParameter("@ThoiHan", hopDong.ThoiHan),
        new SqlParameter("@TrangThai", hopDong.TrangThai ?? (object)DBNull.Value),
        new SqlParameter("@GhiChu", hopDong.GhiChu ?? (object)DBNull.Value)
            };

            // Tự tạo SqlCommand, không dùng 'db'
            using (SqlCommand cmd = new SqlCommand(qr, conn, tran)) //Sử dụng conn và tran
            {
                cmd.Parameters.AddRange(parameters);
                return cmd.ExecuteNonQuery() > 0;
            }
        }



        // HÀM MỚI: Dùng cho giao dịch
        public bool InsertHopDongNguoiThue(HopDong_NguoiThue ct, SqlConnection conn, SqlTransaction tran)
        {
            string qr = @"INSERT INTO HopDong_NguoiThue (MaHopDong, MaNguoiThue, VaiTro, TrangThaiThue, NgayDonVao, NgayBatDauThue)
                  VALUES (@MaHopDong, @MaNguoiThue, @VaiTro, @TrangThaiThue, @NgayDonVao, @NgayBatDauThue)";

            SqlParameter[] parameters = new SqlParameter[]
            {
        new SqlParameter("@MaHopDong", ct.MaHopDong),
        new SqlParameter("@MaNguoiThue", ct.MaNguoiThue),
        new SqlParameter("@VaiTro", ct.VaiTro),
        new SqlParameter("@TrangThaiThue", ct.TrangThaiThue),
        new SqlParameter("@NgayDonVao", ct.NgayDonVao ?? (object)DBNull.Value),
        new SqlParameter("@NgayBatDauThue", ct.NgayBatDauThue ??(object) DBNull.Value)
            };

            // Tự tạo SqlCommand, không dùng 'db'
            using (SqlCommand cmd = new SqlCommand(qr, conn, tran)) //Sử dụng conn và tran
            {
                cmd.Parameters.AddRange(parameters);
                return cmd.ExecuteNonQuery() > 0;
            }
        }




        // HÀM MỚI: Dùng cho giao dịch (Cần cho việc cập nhật)
        public bool UpdateHopDong(HopDong hopDong, SqlConnection conn, SqlTransaction tran)
        {
            string qr = @"UPDATE HopDong 
                  SET MaPhong = @MaPhong, ChuNha = @ChuNha, TienCoc = @TienCoc,
                      NgayBatDau = @NgayBatDau, ThoiHan = @ThoiHan, TrangThai = @TrangThai,
                      GhiChu = @GhiChu, NgayCapNhat = GETDATE()
                  WHERE MaHopDong = @MaHopDong";

            SqlParameter[] parameters = new SqlParameter[]
            {
        new SqlParameter("@MaHopDong", hopDong.MaHopDong),
        new SqlParameter("@MaPhong", hopDong.MaPhong),
        new SqlParameter("@ChuNha", hopDong.ChuNha ??(object) DBNull.Value),
        new SqlParameter("@TienCoc", hopDong.TienCoc),
        new SqlParameter("@NgayBatDau", hopDong.NgayBatDau),
        new SqlParameter("@ThoiHan", hopDong.ThoiHan),
        new SqlParameter("@TrangThai", hopDong.TrangThai ??(object) DBNull.Value),
        new SqlParameter("@GhiChu", hopDong.GhiChu ??(object) DBNull.Value)
            };

            // Tự tạo SqlCommand, không dùng 'db'
            using (SqlCommand cmd = new SqlCommand(qr, conn, tran)) //Sử dụng conn và tran
            {
                cmd.Parameters.AddRange(parameters);
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool DeleteHopDong(HopDong hopDong, SqlConnection conn, SqlTransaction tran)
        {
            string qr = "DELETE FROM HopDong WHERE MaHopDong = @MaHopDong";
            using (SqlCommand cmd = new SqlCommand(qr, conn, tran))
            {
                cmd.Parameters.AddWithValue("@MaHopDong", hopDong.MaHopDong);
                return cmd.ExecuteNonQuery() > 0;
            }
        }


        public bool DeleteNguoiThueByContractId(string maHopDong, SqlConnection conn, SqlTransaction tran)
        {
            string qr = "DELETE FROM HopDong_NguoiThue WHERE MaHopDong = @MaHopDong";
            using (SqlCommand cmd = new SqlCommand(qr, conn, tran))
            {
                cmd.Parameters.AddWithValue("@MaHopDong", maHopDong);
                cmd.ExecuteNonQuery(); // Xóa 0 hoặc nhiều dòng đều được, không cần kiểm tra > 0
                return true;
            }
        }


    }
}