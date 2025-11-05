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
        public bool CapNhatNguoiThue(NguoiThue nt)
        {
            string sql = @"UPDATE NguoiThue
                           SET
                           HoTen=@HoTen, 
                           SoDienThoai=@Sdt, 
                           Email=@Email,
                           SoGiayTo=@SoGiayTo,
                           NgayTao= @NgayTao, 
                           NgayCapNhat=GETDATE()
                           WHERE MaNguoiThue=@MaNguoiThue";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@HoTen", nt.HoTen),
                new SqlParameter("@Sdt", nt.Sdt ??(object) DBNull.Value),
                new SqlParameter("@Email", nt.Email ??(object) DBNull.Value),
                new SqlParameter("@SoGiayTo", nt.SoGiayTo ??(object) DBNull.Value),
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
    }
}