using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoomManagementSystem.DataLayer
{
    public class PhongDAL
    {
        // Thay bằng một đối tượng của lớp Database
        Database db = new Database();

        // Phương thức mở rộng để xử lý an toàn giá trị từ DB
        private static string ToSafeString(object value)
        {
            return value == null || value is DBNull ? string.Empty : value.ToString() ?? string.Empty;
        }

        // Tạo mã phòng tự động
        public string AutoMaPhong()
        {
            string qr = "SELECT ISNULL(MAX(CAST(SUBSTRING(MaPhong, 6, LEN(MaPhong) - 5) AS INT)), 0) + 1 FROM Phong";
            int nextNumber = Convert.ToInt32(db.ExecuteScalar(qr));
            return "PHONG" + nextNumber.ToString("D3");
        }
        //Lấy trạng thái từ Hợp đồng
        public string TrangThaiPhong(string maPhong)
        {
            string qr = @"SELECT COUNT(*) 
                  FROM HopDong 
                  WHERE MaPhong = @maphong 
                    AND (NgayKetThuc IS NULL OR NgayKetThuc > GETDATE())";

            SqlParameter[] parameters = { new SqlParameter("@maphong", maPhong) };
            object? result = db.ExecuteScalar(qr, parameters);

            int count = Convert.ToInt32(result);

            return count > 0 ? "Đang thuê" : "Trống";
        }
        //Lấy số người hiện tại trong phòng
        public int SoNguoiHienTai(string maPhong)
        {
            string qr = @"SELECT COUNT(*) 
                  FROM NguoiThue 
                  WHERE MaPhong = @maphong 
                    AND (NgayDonRa IS NULL OR NgayDonRa > GETDATE())";
            SqlParameter[] parameters = { new SqlParameter("@maphong", maPhong) };
            object? result = db.ExecuteScalar(qr, parameters);
            int count = Convert.ToInt32(result);
            return count;
        }
        // Thêm phòng
        public bool InsertPhong(Phong phong)
        {
            string TrangThai=TrangThaiPhong(phong.MaPhong);
            string qr = @"INSERT INTO Phong (MaPhong,MaNha, LoaiPhong, DienTich, GiaThue, TrangThai, GhiChu, NgayTao, NgayCapNhat)
                          VALUES (@MaPhong,@MaNha, @LoaiPhong, @DienTich, @GiaThue, @TrangThai, @GhiChu, GETDATE(), GETDATE())";

            // Tạo mảng SqlParameter
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@MaPhong", phong.MaPhong ?? (object)DBNull.Value),
                new SqlParameter("@MaNha", phong.MaNha ?? (object)DBNull.Value),
                new SqlParameter("@LoaiPhong", phong.LoaiPhong ?? (object)DBNull.Value),
                new SqlParameter("@DienTich", phong.DienTich),
                new SqlParameter("@GiaThue", phong.GiaThue),
                new SqlParameter("@TrangThai", "Trống"), // gán mặc định,
                new SqlParameter("@GhiChu", phong.GhiChu ?? (object)DBNull.Value)
            };

            // Gọi ExecuteNonQuery từ lớp Database
            int result = db.ExecuteNonQuery(qr, parameters);
            return result > 0;
        }

        // Cập nhật phòng
        public bool UpdatePhong(Phong phong)
        {
            string qr = @"UPDATE Phong 
                          SET LoaiPhong = @LoaiPhong,
                              DienTich = @DienTich,
                              GiaThue = @GiaThue,
                              TrangThai = @TrangThai,
                              SoNguoiHienTai = @sn,
                              GhiChu = @GhiChu,
                              NgayCapNhat = GETDATE()
                          WHERE MaPhong = @MaPhong";
            string TrangThai = TrangThaiPhong(phong.MaPhong);
            int sn = SoNguoiHienTai(phong.MaPhong);
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@MaPhong", phong.MaPhong ?? (object)DBNull.Value),
                new SqlParameter("@LoaiPhong", phong.LoaiPhong ?? (object)DBNull.Value),
                new SqlParameter("@DienTich", phong.DienTich),
                new SqlParameter("@GiaThue", phong.GiaThue),
                new SqlParameter("@TrangThai",TrangThai ?? (object)DBNull.Value),
                new SqlParameter("@sn", sn),
                new SqlParameter("@GhiChu", phong.GhiChu ?? (object)DBNull.Value)
            };

            int result = db.ExecuteNonQuery(qr, parameters);
            return result > 0;
        }

        // Xóa phòng
        public bool DeletePhong(string maPhong)
        {
            string qr = "DELETE FROM Phong WHERE MaPhong = @MaPhong";
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@MaPhong", maPhong ?? (object)DBNull.Value)
            };

            int result = db.ExecuteNonQuery(qr, parameters);
            return result > 0;
        }

        // Lấy tất cả phòng trong nha
        public List<Phong> GetAllPhong(string MaNha)
        {
            List<Phong> ds = new List<Phong>();
            string qr = "SELECT * FROM Phong WHERE MaNha=@Manha";
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@Manha", MaNha ?? (object)DBNull.Value)
            };
            // Dùng ExecuteQuery và nhận về DataTable
            DataTable dt = db.ExecuteQuery(qr,parameters);

            // Duyệt qua DataTable để đọc dữ liệu
            foreach (DataRow row in dt.Rows)
            {
                string maPhong = ToSafeString(row["MaPhong"]);
                string maNha = ToSafeString(row["MaNha"]);
                string loaiPhong = ToSafeString(row["LoaiPhong"]);
                string trangThai = ToSafeString(row["TrangThai"]);
                string ghiChu = ToSafeString(row["GhiChu"]);

                // Dùng Convert.To... để an toàn hơn khi lấy từ DataRow
                float dienTich = row["DienTich"] is DBNull ? 0.0f : Convert.ToSingle(row["DienTich"]);
                float giaThue = row["GiaThue"] is DBNull ? 0.0f : Convert.ToSingle(row["GiaThue"]);
                int soNguoiHienTai = row["SoNguoiHienTai"] is DBNull ? 0 : Convert.ToInt32(row["SoNguoiHienTai"]);

                DateTime ngayTao = row["NgayTao"] is DBNull ? DateTime.MinValue : Convert.ToDateTime(row["NgayTao"]);
                DateTime ngayCapNhat = row["NgayCapNhat"] is DBNull ? DateTime.MinValue : Convert.ToDateTime(row["NgayCapNhat"]);

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

            return ds;
        }

        // Lấy phòng theo mã
        public Phong? GetPhongById(string maPhong)
        {
            string qr = "SELECT MaPhong, MaNha, LoaiPhong, DienTich, GiaThue, TrangThai, SoNguoiHienTai, GhiChu, NgayTao, NgayCapNhat FROM Phong WHERE MaPhong = @MaPhong";
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@MaPhong", maPhong ?? (object)DBNull.Value)
            };

            DataTable dt = db.ExecuteQuery(qr, parameters);

            if (dt.Rows.Count > 0)
            {
                DataRow row = dt.Rows[0]; // Lấy dòng đầu tiên

                string maPhongResult = ToSafeString(row["MaPhong"]);
                string maNha = ToSafeString(row["MaNha"]);
                string loaiPhong = ToSafeString(row["LoaiPhong"]);
                string trangThai = ToSafeString(row["TrangThai"]);
                string ghiChu = ToSafeString(row["GhiChu"]);

                float dienTich = row["DienTich"] is DBNull ? 0.0f : Convert.ToSingle(row["DienTich"]);
                float giaThue = row["GiaThue"] is DBNull ? 0.0f : Convert.ToSingle(row["GiaThue"]);
                int soNguoiHienTai = row["SoNguoiHienTai"] is DBNull ? 0 : Convert.ToInt32(row["SoNguoiHienTai"]);

                DateTime ngayTao = row["NgayTao"] is DBNull ? DateTime.MinValue : Convert.ToDateTime(row["NgayTao"]);
                DateTime ngayCapNhat = row["NgayCapNhat"] is DBNull ? DateTime.MinValue : Convert.ToDateTime(row["NgayCapNhat"]);

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