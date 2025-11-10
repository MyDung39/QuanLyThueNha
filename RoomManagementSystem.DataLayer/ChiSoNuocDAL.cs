using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;

namespace RoomManagementSystem.DataLayer
{
    public class ChiSoNuoc
    {
        public string MaChiSoNuoc { get; set; }
        public string MaDichVu { get; set; }
        public string LoaiDongHo { get; set; }
        public DateTime? NgayGhiThangTruoc { get; set; }
        public decimal? ChiSoThangTruoc { get; set; }
        public DateTime? NgayGhiThangNay { get; set; }
        public decimal? ChiSoThangNay { get; set; }
        public decimal? MucTieuThu { get; set; }
        public string NguonThuThap { get; set; }
        public DateTime? NgayTao { get; set; }
        public DateTime? NgayCapNhat { get; set; }
    }

    public class ChiSoNuocDAL
    {
        private readonly string connectionString = DbConfig.ConnectionString;

        public ChiSoNuoc GetById(string maChiSoNuoc)
        {
            using var conn = new SqlConnection(connectionString);
            const string q = "SELECT * FROM ChiSoNuoc WHERE MaChiSoNuoc=@id";
            using var cmd = new SqlCommand(q, conn);
            cmd.Parameters.AddWithValue("@id", maChiSoNuoc);
            conn.Open();
            using var r = cmd.ExecuteReader();
            if (!r.Read()) return null;
            return Map(r);
        }

        public List<ChiSoNuoc> GetAll()
        {
            var list = new List<ChiSoNuoc>();
            using var conn = new SqlConnection(connectionString);
            const string q = "SELECT * FROM ChiSoNuoc ORDER BY NgayCapNhat DESC";
            using var cmd = new SqlCommand(q, conn);
            conn.Open();
            using var r = cmd.ExecuteReader();
            while (r.Read()) list.Add(Map(r));
            return list;
        }

        public bool Insert(ChiSoNuoc e)
        {
            using var conn = new SqlConnection(connectionString);
            const string q = @"INSERT INTO ChiSoNuoc(
                                MaChiSoNuoc, MaDichVu, LoaiDongHo, NgayGhiThangTruoc, ChiSoThangTruoc,
                                NgayGhiThangNay, ChiSoThangNay, MucTieuThu, NguonThuThap, NgayTao, NgayCapNhat)
                              VALUES(@MaChiSoNuoc, @MaDichVu, @LoaiDongHo, @NgayGhiThangTruoc, @ChiSoThangTruoc,
                                     @NgayGhiThangNay, @ChiSoThangNay, @MucTieuThu, @NguonThuThap, GETDATE(), GETDATE())";
            using var cmd = new SqlCommand(q, conn);
            BindParams(cmd, e, includeKeys: true);
            conn.Open();
            return cmd.ExecuteNonQuery() > 0;
        }

        public bool Update(ChiSoNuoc e)
        {
            using var conn = new SqlConnection(connectionString);
            const string q = @"UPDATE ChiSoNuoc SET 
                                MaDichVu=@MaDichVu, LoaiDongHo=@LoaiDongHo, NgayGhiThangTruoc=@NgayGhiThangTruoc, ChiSoThangTruoc=@ChiSoThangTruoc,
                                NgayGhiThangNay=@NgayGhiThangNay, ChiSoThangNay=@ChiSoThangNay, MucTieuThu=@MucTieuThu,
                                NguonThuThap=@NguonThuThap, NgayCapNhat=GETDATE()
                              WHERE MaChiSoNuoc=@MaChiSoNuoc";
            using var cmd = new SqlCommand(q, conn);
            BindParams(cmd, e, includeKeys: true);
            conn.Open();
            return cmd.ExecuteNonQuery() > 0;
        }

        public bool Delete(string maChiSoNuoc)
        {
            using var conn = new SqlConnection(connectionString);
            const string q = "DELETE FROM ChiSoNuoc WHERE MaChiSoNuoc=@id";
            using var cmd = new SqlCommand(q, conn);
            cmd.Parameters.AddWithValue("@id", maChiSoNuoc);
            conn.Open();
            return cmd.ExecuteNonQuery() > 0;
        }

        private static ChiSoNuoc Map(SqlDataReader r)
        {
            return new ChiSoNuoc
            {
                MaChiSoNuoc = r["MaChiSoNuoc"].ToString(),
                MaDichVu = r["MaDichVu"].ToString(),
                LoaiDongHo = r["LoaiDongHo"].ToString(),
                NgayGhiThangTruoc = r["NgayGhiThangTruoc"] == DBNull.Value ? null : (DateTime?)Convert.ToDateTime(r["NgayGhiThangTruoc"]),
                ChiSoThangTruoc = r["ChiSoThangTruoc"] == DBNull.Value ? null : (decimal?)Convert.ToDecimal(r["ChiSoThangTruoc"]),
                NgayGhiThangNay = r["NgayGhiThangNay"] == DBNull.Value ? null : (DateTime?)Convert.ToDateTime(r["NgayGhiThangNay"]),
                ChiSoThangNay = r["ChiSoThangNay"] == DBNull.Value ? null : (decimal?)Convert.ToDecimal(r["ChiSoThangNay"]),
                MucTieuThu = r["MucTieuThu"] == DBNull.Value ? null : (decimal?)Convert.ToDecimal(r["MucTieuThu"]),
                NguonThuThap = r["NguonThuThap"].ToString(),
                NgayTao = r["NgayTao"] == DBNull.Value ? null : (DateTime?)Convert.ToDateTime(r["NgayTao"]),
                NgayCapNhat = r["NgayCapNhat"] == DBNull.Value ? null : (DateTime?)Convert.ToDateTime(r["NgayCapNhat"])
            };
        }

        private static void BindParams(SqlCommand cmd, ChiSoNuoc e, bool includeKeys)
        {
            if (includeKeys)
            {
                cmd.Parameters.AddWithValue("@MaChiSoNuoc", (object?)e.MaChiSoNuoc ?? DBNull.Value);
            }
            cmd.Parameters.AddWithValue("@MaDichVu", (object?)e.MaDichVu ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@LoaiDongHo", (object?)e.LoaiDongHo ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@NgayGhiThangTruoc", (object?)e.NgayGhiThangTruoc ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@ChiSoThangTruoc", (object?)e.ChiSoThangTruoc ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@NgayGhiThangNay", (object?)e.NgayGhiThangNay ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@ChiSoThangNay", (object?)e.ChiSoThangNay ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@MucTieuThu", (object?)e.MucTieuThu ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@NguonThuThap", (object?)e.NguonThuThap ?? DBNull.Value);
        }
    }
}
