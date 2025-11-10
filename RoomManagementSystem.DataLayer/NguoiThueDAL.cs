using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;

namespace RoomManagementSystem.DataLayer
{
    public class NguoiThueDAL
    {
        Database db = new Database();

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
                           (MaNguoiThue, HoTen, SoDienThoai, Email, SoGiayTo, NgayTao, NgayCapNhat)
                           VALUES
                           (@MaNguoiThue, @HoTen, @SoDienThoai, @Email, @SoGiayTo, GETDATE(), GETDATE())";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@MaNguoiThue", nt.MaNguoiThue),
                new SqlParameter("@HoTen", nt.HoTen),
                new SqlParameter("@SoDienThoai", nt.Sdt ?? (object)DBNull.Value),
                new SqlParameter("@Email", nt.Email ??(object) DBNull.Value),
                new SqlParameter("@SoGiayTo", nt.SoGiayTo ??(object) DBNull.Value)
            };

            return db.ExecuteNonQuery(sql, parameters) > 0;
        }

        //Cap nhat thong tin nguoi thue
        /*   public bool CapNhatNguoiThue(NguoiThue nt)
           {
               string sql = @"UPDATE NguoiThue
                              SET
                              HoTen=@HoTen, 
                              SoDienThoai=@Sdt, 
                              Email=@Email,
                              SoGiayTo=@SoGiayTo,

                              NgayCapNhat=GETDATE()
                              WHERE MaNguoiThue=@MaNguoiThue";

               SqlParameter[] parameters = new SqlParameter[]
               {
                   new SqlParameter("@MaNguoiThue", nt.MaNguoiThue),
                   new SqlParameter("@HoTen", nt.HoTen),
                   new SqlParameter("@Sdt", nt.Sdt ??(object) DBNull.Value),
                   new SqlParameter("@Email", nt.Email ??(object) DBNull.Value),
                   new SqlParameter("@SoGiayTo", nt.SoGiayTo ??(object) DBNull.Value),
                   //new SqlParameter("@NgayTao", nt.NgayTao),
                   new SqlParameter("@MaNguoiThue", nt.MaNguoiThue)
               };

               return db.ExecuteNonQuery(sql, parameters) > 0;
           }
        */


        public bool CapNhatNguoiThue(NguoiThue nt)
        {
            string sql = @"UPDATE NguoiThue
                           SET
                               HoTen=@HoTen, 
                               SoDienThoai=@SoDienThoai, 
                               Email=@Email,
                               SoGiayTo=@SoGiayTo,
                               NgayCapNhat=GETDATE()
                           WHERE MaNguoiThue=@MaNguoiThue";

            SqlParameter[] parameters = new SqlParameter[]
            {
                // Chỉ khai báo mỗi tham số một lần
                new SqlParameter("@MaNguoiThue", nt.MaNguoiThue),
                new SqlParameter("@HoTen", nt.HoTen ?? (object)DBNull.Value),
                new SqlParameter("@SoDienThoai", nt.Sdt ?? (object)DBNull.Value),
                new SqlParameter("@Email", nt.Email ?? (object)DBNull.Value),
                new SqlParameter("@SoGiayTo", nt.SoGiayTo ?? (object)DBNull.Value)
            };

            return db.ExecuteNonQuery(sql, parameters) > 0;
        }

        //Tra ve danh sach nguoi thue
        public List<NguoiThue> getAllNguoiThue()
        {
            List<NguoiThue> ds = new List<NguoiThue>();
            string q = "SELECT * FROM NguoiThue";
            DataTable dt = db.ExecuteQuery(q);

            foreach (DataRow reader in dt.Rows)
            {
                NguoiThue nt = new NguoiThue
                {
                    MaNguoiThue = reader["MaNguoiThue"].ToString(),
                    HoTen = reader["HoTen"].ToString(),
                    Sdt = reader["SoDienThoai"].ToString(),
                    Email = reader["Email"].ToString(),
                    SoGiayTo = reader["SoGiayTo"].ToString(),
                    NgayTao = Convert.ToDateTime(reader["NgayTao"]),
                    NgayCapNhat = Convert.ToDateTime(reader["NgayCapNhat"]),
                };
                ds.Add(nt);
            }
            return ds;
        }


        public bool XoaNguoiThue(string maNguoiThue)
        {
            // Trước tiên xóa các bản ghi liên quan trong HopDong_NguoiThue
            string qr_hopdong = "DELETE FROM HopDong_NguoiThue WHERE MaNguoiThue = @MaNguoiThue";
            SqlParameter[] param_hopdong = new SqlParameter[]
            {
            new SqlParameter("@MaNguoiThue", maNguoiThue)
            };
            db.ExecuteNonQuery(qr_hopdong, param_hopdong);

            // Sau đó xóa bản ghi trong NguoiThue
            string qr_nguoithue = "DELETE FROM NguoiThue WHERE MaNguoiThue = @MaNguoiThue";
            SqlParameter[] param_nguoithue = new SqlParameter[]
            {
            new SqlParameter("@MaNguoiThue", maNguoiThue)
            };

            return db.ExecuteNonQuery(qr_nguoithue, param_nguoithue) > 0;
        }


        public List<NguoiThue> GetByRoomId(string maPhong)
        {
            List<NguoiThue> list = new List<NguoiThue>();
            string sql = @"SELECT nt.* 
                   FROM NguoiThue nt
                   JOIN HopDong_NguoiThue hnt ON nt.MaNguoiThue = hnt.MaNguoiThue
                   JOIN HopDong h ON hnt.MaHopDong = h.MaHopDong
                   WHERE h.MaPhong = @MaPhong AND h.TrangThai = N'Hiệu lực'";
            SqlParameter[] parameters = new SqlParameter[] { new SqlParameter("@MaPhong", maPhong) };
            DataTable dt = db.ExecuteQuery(sql, parameters);

            foreach (DataRow row in dt.Rows)
            {
                list.Add(new NguoiThue { /* ... điền các thuộc tính ... */ });
            }
            return list;
        }

    }
}