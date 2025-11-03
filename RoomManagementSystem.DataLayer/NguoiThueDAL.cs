using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoomManagementSystem.DataLayer
{
    public class NguoiThueDAL
    {
        // Khởi tạo đối tượng Database để sử dụng các hàm của nó
        Database db = new Database();

        // Tạo mã người thuê tự động (ví dụ: NT001)
        public string AutoMaNguoiThue()
        {
            string qr = "SELECT ISNULL(MAX(CAST(SUBSTRING(MaNguoiThue, 3, LEN(MaNguoiThue) - 2) AS INT)), 0) + 1 FROM NguoiThue";
            int nextNumber = Convert.ToInt32(db.ExecuteScalar(qr));
            return "NT" + nextNumber.ToString("D3");
        }


        //Nhập thông tin nguoi thue
        public bool ThemNguoiThue(NguoiThue nt)
        {
            string sql = @"INSERT INTO NguoiThue
                           (MaNguoiThue, MaPhong, HoTen, SoDienThoai, Email, SoGiayTo,
                            NgayBatDauThue, TrangThaiThue, 
                            NgayDonVao, NgayDonRa, VaiTro, NgayTao, NgayCapNhat)
                           VALUES
                           (@MaNguoiThue, @MaPhong, @HoTen, @SoDienThoai, @Email, @SoGiayTo,
                            @NgayBatDauThue, @TrangThaiThue, 
                            @NgayDonVao, @NgayDonRa, @VaiTro, @NgayTao, GETDATE())";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@MaNguoiThue", nt.MaNguoiThue),
                new SqlParameter("@MaPhong", nt.MaPhong),
                new SqlParameter("@HoTen", nt.HoTen),
                new SqlParameter("@SoDienThoai", (object)nt.Sdt ?? DBNull.Value),
                new SqlParameter("@Email", (object)nt.Email ?? DBNull.Value),
                new SqlParameter("@SoGiayTo", nt.SoGiayTo),
                new SqlParameter("@NgayBatDauThue", nt.NgayBatDauThue),
                new SqlParameter("@TrangThaiThue", nt.TrangThaiThue),
                new SqlParameter("@NgayDonVao", (object?)nt.NgayDonVao ?? DBNull.Value),
                new SqlParameter("@NgayDonRa", (object?)nt.NgayDonRa ?? DBNull.Value),
                new SqlParameter("@VaiTro", nt.VaiTro),
                new SqlParameter("@NgayTao", nt.NgayTao)
            };

            return db.ExecuteNonQuery(sql, parameters) > 0;
        }

        //Cap nhat thong tin nguoi thue, tinh trang thue, ngay tra phong
        public bool CapNhatNguoiThue(NguoiThue nt)
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
                           VaiTro=@VaiTro,
                           NgayTao= @NgayTao, 
                           NgayCapNhat=GETDATE()
                           WHERE MaNguoiThue=@MaNguoiThue";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@MaPhong", nt.MaPhong),
                new SqlParameter("@HoTen", nt.HoTen),
                new SqlParameter("@Sdt", (object)nt.Sdt ?? DBNull.Value),
                new SqlParameter("@Email", (object)nt.Email ?? DBNull.Value),
                new SqlParameter("@SoGiayTo", nt.SoGiayTo),
                new SqlParameter("@NgayBatDauThue", nt.NgayBatDauThue),
                new SqlParameter("@TrangThaiThue", nt.TrangThaiThue),
                new SqlParameter("@NgayDonVao", (object?)nt.NgayDonVao ?? DBNull.Value),
                new SqlParameter("@NgayDonRa", (object?)nt.NgayDonRa ?? DBNull.Value),
                new SqlParameter("@VaiTro", nt.VaiTro),
                new SqlParameter("@NgayTao", nt.NgayTao),
                new SqlParameter("@MaNguoiThue", nt.MaNguoiThue)
            };

            return db.ExecuteNonQuery(sql, parameters) > 0;
        }

        //Tra ve danh sach nguoi thue
        public List<NguoiThue> getAllNguoiThue()
        {
            List<NguoiThue> ds = new List<NguoiThue>();
            string q = "SELECT * FROM NguoiThue";

            // Gọi hàm ExecuteQuery để lấy về DataTable
            DataTable dt = db.ExecuteQuery(q);

            // Duyệt qua DataTable (thay vì SqlDataReader)
            foreach (DataRow reader in dt.Rows)
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
                    VaiTro = reader["VaiTro"].ToString(),
                    NgayTao = Convert.ToDateTime(reader["NgayTao"]),
                    NgayCapNhat = Convert.ToDateTime(reader["NgayCapNhat"]),
                };

                ds.Add(nt);
            }
            return ds;
        }


        //Tra ve danh sach nguoi thue theo phong
        public List<NguoiThue> getNguoiThueByPhong(string maPhong)
        {
            List<NguoiThue> ds = new List<NguoiThue>();
            string q = "SELECT * FROM NguoiThue WHERE MaPhong=@MaPhong";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@MaPhong", maPhong)
            };

            DataTable dt = db.ExecuteQuery(q, parameters);

            foreach (DataRow reader in dt.Rows)
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
                    VaiTro = reader["VaiTro"].ToString(),
                    NgayTao = Convert.ToDateTime(reader["NgayTao"]),
                    NgayCapNhat = Convert.ToDateTime(reader["NgayCapNhat"]),
                };

                ds.Add(nt);
            }
            return ds;
        }
    }
}
