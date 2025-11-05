using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoomManagementSystem.DataLayer
{
    public class ThongBaoPhi
    {
        public string MaThongBaoPhi { get; set; }
        public string MaPhong { get; set; }
        public string ThoiKy { get; set; }
        public decimal TongTien { get; set; }
        public string FileDinhKem { get; set; }
        public DateTime? NgayGui { get; set; }
        public string TrangThai { get; set; }
        public DateTime NgayTao { get; set; }
        public DateTime NgayCapNhat { get; set; }
    }
    public class ThongBaoPhiDAL
    {
        Database db = new Database();

        // Tạo mã tự động: TBP001
        public string AutoMaThongBaoPhi()
        {
            string qr = "SELECT ISNULL(MAX(CAST(SUBSTRING(MaThongBaoPhi, 4, LEN(MaThongBaoPhi) - 3) AS INT)), 0) + 1 FROM ThongBaoPhi";
            int nextNumber = Convert.ToInt32(db.ExecuteScalar(qr));
            return "TBP" + nextNumber.ToString("D3");
        }

        // Thêm thông báo phí
        public bool ThemThongBaoPhi(ThongBaoPhi tb)
        {
            string sql = @"
                INSERT INTO ThongBaoPhi 
                (MaThongBaoPhi, MaPhong, ThoiKy, TongTien, FileDinhKem, NgayGui, TrangThai, NgayTao, NgayCapNhat)
                VALUES 
                (@MaThongBaoPhi, @MaPhong, @ThoiKy, @TongTien, @FileDinhKem, @NgayGui, @TrangThai, GETDATE(), GETDATE())";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@MaThongBaoPhi", tb.MaThongBaoPhi),
                new SqlParameter("@MaPhong", tb.MaPhong),
                new SqlParameter("@ThoiKy", tb.ThoiKy),
                new SqlParameter("@TongTien", tb.TongTien),
                new SqlParameter("@FileDinhKem", (object)tb.FileDinhKem ?? DBNull.Value),
                new SqlParameter("@NgayGui", (object?)tb.NgayGui ?? DBNull.Value),
                new SqlParameter("@TrangThai", tb.TrangThai)
            };

            return db.ExecuteNonQuery(sql, parameters) > 0;
        }

        // Cập nhật thông báo phí
        public bool CapNhatThongBaoPhi(ThongBaoPhi tb)
        {
            string sql = @"
                UPDATE ThongBaoPhi SET
                    MaPhong=@MaPhong,
                    ThoiKy=@ThoiKy,
                    TongTien=@TongTien,
                    FileDinhKem=@FileDinhKem,
                    NgayGui=@NgayGui,
                    TrangThai=@TrangThai,
                    NgayCapNhat=GETDATE()
                WHERE MaThongBaoPhi=@MaThongBaoPhi";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@MaThongBaoPhi", tb.MaThongBaoPhi),
                new SqlParameter("@MaPhong", tb.MaPhong),
                new SqlParameter("@ThoiKy", tb.ThoiKy),
                new SqlParameter("@TongTien", tb.TongTien),
                new SqlParameter("@FileDinhKem", (object)tb.FileDinhKem ?? DBNull.Value),
                new SqlParameter("@NgayGui", (object?)tb.NgayGui ?? DBNull.Value),
                new SqlParameter("@TrangThai", tb.TrangThai)
            };

            return db.ExecuteNonQuery(sql, parameters) > 0;
        }

        // Lấy toàn bộ thông báo phí
        public List<ThongBaoPhi> GetAllThongBaoPhi()
        {
            List<ThongBaoPhi> ds = new();
            DataTable dt = db.ExecuteQuery("SELECT * FROM ThongBaoPhi");

            foreach (DataRow row in dt.Rows)
            {
                ds.Add(RowToThongBaoPhi(row));
            }

            return ds;
        }

        // Lấy thông báo theo phòng
        public ThongBaoPhi GetThongBaoPhiByPhong(string maPhong)
        {
            ThongBaoPhi ds = new();

            string sql = "SELECT * FROM ThongBaoPhi WHERE MaPhong=@MaPhong";
            SqlParameter[] p =
            {
                new SqlParameter("@MaPhong", maPhong)
            };

            DataTable dt = db.ExecuteQuery(sql, p);

            if (dt.Rows.Count == 0)
                return null;

            ds = RowToThongBaoPhi(dt.Rows[0]);

            return ds;
        }

        // Chuyển DataRow -> model
        private ThongBaoPhi RowToThongBaoPhi(DataRow row)
        {
            return new ThongBaoPhi
            {
                MaThongBaoPhi = row["MaThongBaoPhi"].ToString(),
                MaPhong = row["MaPhong"].ToString(),
                ThoiKy = row["ThoiKy"].ToString(),
                TongTien = Convert.ToDecimal(row["TongTien"]),
                FileDinhKem = row["FileDinhKem"].ToString(),
                NgayGui = row["NgayGui"] == DBNull.Value ? null : Convert.ToDateTime(row["NgayGui"]),
                TrangThai = row["TrangThai"].ToString(),
                NgayTao = Convert.ToDateTime(row["NgayTao"]),
                NgayCapNhat = Convert.ToDateTime(row["NgayCapNhat"])
            };
        }
    }
}
