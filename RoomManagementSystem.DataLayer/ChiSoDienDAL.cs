using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;

namespace RoomManagementSystem.DataLayer
{
    public class ChiSoDien
    {
        public string MaChiSoDien { get; set; }
        public string MaDichVu { get; set; }
        public decimal? DonGia { get; set; }
        public DateTime? NgayGhiThangTruoc { get; set; }
        public decimal? ChiSoThangTruoc { get; set; }
        public DateTime? NgayGhiThangNay { get; set; }
        public decimal? ChiSoThangNay { get; set; }
        public decimal? MucTieuThu { get; set; }
        public decimal? ThanhTien { get; set; }
        public string NguonThuThap { get; set; }
        public DateTime? NgayTao { get; set; }
        public DateTime? NgayCapNhat { get; set; }
    }

    public class ChiSoDienDAL
    {
        private readonly string connectionString = DbConfig.ConnectionString;

        public ChiSoDien GetById(string maChiSoDien)
        {
            using var conn = new SqlConnection(connectionString);
            string q = @"SELECT * FROM ChiSoDien WHERE MaChiSoDien=@id";
            using var cmd = new SqlCommand(q, conn);
            cmd.Parameters.AddWithValue("@id", maChiSoDien);
            conn.Open();
            using var r = cmd.ExecuteReader();
            if (!r.Read()) return null;
            return Map(r);
        }

        public List<ChiSoDien> GetAll()
        {
            var list = new List<ChiSoDien>();
            using var conn = new SqlConnection(connectionString);
            string q = @"SELECT * FROM ChiSoDien ORDER BY NgayCapNhat DESC";
            using var cmd = new SqlCommand(q, conn);
            conn.Open();
            using var r = cmd.ExecuteReader();
            while (r.Read()) list.Add(Map(r));
            return list;
        }

        public bool Insert(ChiSoDien e)
        {
            using var conn = new SqlConnection(connectionString);
            string q = @"INSERT INTO ChiSoDien(
                            MaChiSoDien, MaDichVu, DonGia, NgayGhiThangTruoc, ChiSoThangTruoc,
                            NgayGhiThangNay, ChiSoThangNay, MucTieuThu, ThanhTien, NguonThuThap, NgayTao, NgayCapNhat)
                         VALUES(@MaChiSoDien, @MaDichVu, @DonGia, @NgayGhiThangTruoc, @ChiSoThangTruoc,
                                @NgayGhiThangNay, @ChiSoThangNay, @MucTieuThu, @ThanhTien, @NguonThuThap, GETDATE(), GETDATE())";
            using var cmd = new SqlCommand(q, conn);
            BindParams(cmd, e, includeKeys: true);
            conn.Open();
            return cmd.ExecuteNonQuery() > 0;
        }

        public bool Update(ChiSoDien e)
        {
            using var conn = new SqlConnection(connectionString);
            string q = @"UPDATE ChiSoDien SET 
                            MaDichVu=@MaDichVu, DonGia=@DonGia, NgayGhiThangTruoc=@NgayGhiThangTruoc, ChiSoThangTruoc=@ChiSoThangTruoc,
                            NgayGhiThangNay=@NgayGhiThangNay, ChiSoThangNay=@ChiSoThangNay, MucTieuThu=@MucTieuThu,
                            ThanhTien=@ThanhTien, NguonThuThap=@NguonThuThap, NgayCapNhat=GETDATE()
                         WHERE MaChiSoDien=@MaChiSoDien";
            using var cmd = new SqlCommand(q, conn);
            BindParams(cmd, e, includeKeys: true);
            conn.Open();
            return cmd.ExecuteNonQuery() > 0;
        }

        public bool Delete(string maChiSoDien)
        {
            using var conn = new SqlConnection(connectionString);
            string q = @"DELETE FROM ChiSoDien WHERE MaChiSoDien=@id";
            using var cmd = new SqlCommand(q, conn);
            cmd.Parameters.AddWithValue("@id", maChiSoDien);
            conn.Open();
            return cmd.ExecuteNonQuery() > 0;
        }

        private static ChiSoDien Map(SqlDataReader r)
        {
            return new ChiSoDien
            {
                MaChiSoDien = r["MaChiSoDien"].ToString(),
                MaDichVu = r["MaDichVu"].ToString(),
                DonGia = r["DonGia"] == DBNull.Value ? null : (decimal?)Convert.ToDecimal(r["DonGia"]),
                NgayGhiThangTruoc = r["NgayGhiThangTruoc"] == DBNull.Value ? null : (DateTime?)Convert.ToDateTime(r["NgayGhiThangTruoc"]),
                ChiSoThangTruoc = r["ChiSoThangTruoc"] == DBNull.Value ? null : (decimal?)Convert.ToDecimal(r["ChiSoThangTruoc"]),
                NgayGhiThangNay = r["NgayGhiThangNay"] == DBNull.Value ? null : (DateTime?)Convert.ToDateTime(r["NgayGhiThangNay"]),
                ChiSoThangNay = r["ChiSoThangNay"] == DBNull.Value ? null : (decimal?)Convert.ToDecimal(r["ChiSoThangNay"]),
                MucTieuThu = r["MucTieuThu"] == DBNull.Value ? null : (decimal?)Convert.ToDecimal(r["MucTieuThu"]),
                ThanhTien = r["ThanhTien"] == DBNull.Value ? null : (decimal?)Convert.ToDecimal(r["ThanhTien"]),
                NguonThuThap = r["NguonThuThap"].ToString(),
                NgayTao = r["NgayTao"] == DBNull.Value ? null : (DateTime?)Convert.ToDateTime(r["NgayTao"]),
                NgayCapNhat = r["NgayCapNhat"] == DBNull.Value ? null : (DateTime?)Convert.ToDateTime(r["NgayCapNhat"])
            };
        }

        private static void BindParams(SqlCommand cmd, ChiSoDien e, bool includeKeys)
        {
            if (includeKeys)
            {
                cmd.Parameters.AddWithValue("@MaChiSoDien", (object?)e.MaChiSoDien ?? DBNull.Value);
            }
            cmd.Parameters.AddWithValue("@MaDichVu", (object?)e.MaDichVu ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@DonGia", (object?)e.DonGia ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@NgayGhiThangTruoc", (object?)e.NgayGhiThangTruoc ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@ChiSoThangTruoc", (object?)e.ChiSoThangTruoc ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@NgayGhiThangNay", (object?)e.NgayGhiThangNay ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@ChiSoThangNay", (object?)e.ChiSoThangNay ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@MucTieuThu", (object?)e.MucTieuThu ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@ThanhTien", (object?)e.ThanhTien ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@NguonThuThap", (object?)e.NguonThuThap ?? DBNull.Value);
        }



        public ChiSoDien GetByMaPhongThoiKy(string maPhong, string thoiKy)
        {
            using var conn = new SqlConnection(connectionString);
            string q = @"SELECT TOP 1 * FROM ChiSoDien 
                 WHERE MaDichVu=@MaPhong AND FORMAT(NgayGhiThangNay,'MM/yyyy')=@ThoiKy
                 ORDER BY NgayGhiThangNay DESC";
            using var cmd = new SqlCommand(q, conn);
            cmd.Parameters.AddWithValue("@MaPhong", maPhong);
            cmd.Parameters.AddWithValue("@ThoiKy", thoiKy);
            conn.Open();
            using var r = cmd.ExecuteReader();
            return r.Read() ? Map(r) : null;
        }


    }
}
