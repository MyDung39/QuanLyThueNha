using Microsoft.Data.SqlClient;
using System;

namespace RoomManagementSystem.DataLayer
{
    public class ChiTietHoaDonDAL
    {
        private readonly string connectionString = DbConfig.ConnectionString;

        public void UpsertByMaHoaDonMaDichVu(string maHoaDon, string maDichVu, decimal soLuong, string dvt, decimal donGia)
        {
            using var conn = new SqlConnection(connectionString);
            conn.Open();

            // Check exists
            using (var chk = new SqlCommand("SELECT COUNT(*) FROM ChiTietHoaDon WHERE MaHoaDon=@MaHoaDon AND MaDichVu=@MaDichVu", conn))
            {
                chk.Parameters.AddWithValue("@MaHoaDon", maHoaDon);
                chk.Parameters.AddWithValue("@MaDichVu", maDichVu);
                int count = Convert.ToInt32(chk.ExecuteScalar());
                if (count > 0)
                {
                    using var upd = new SqlCommand(@"UPDATE ChiTietHoaDon
                                SET SoLuong=@SoLuong, DVT=@DVT, DonGia=@DonGia
                              WHERE MaHoaDon=@MaHoaDon AND MaDichVu=@MaDichVu", conn);
                    upd.Parameters.AddWithValue("@MaHoaDon", maHoaDon);
                    upd.Parameters.AddWithValue("@MaDichVu", maDichVu);
                    upd.Parameters.AddWithValue("@SoLuong", soLuong);
                    upd.Parameters.AddWithValue("@DVT", (object?)dvt ?? DBNull.Value);
                    upd.Parameters.AddWithValue("@DonGia", donGia);
                    upd.ExecuteNonQuery();
                }
                else
                {
                    using var ins = new SqlCommand(@"INSERT INTO ChiTietHoaDon(MaHoaDon, MaDichVu, SoLuong, DVT, DonGia)
                             VALUES(@MaHoaDon, @MaDichVu, @SoLuong, @DVT, @DonGia)", conn);
                    ins.Parameters.AddWithValue("@MaHoaDon", maHoaDon);
                    ins.Parameters.AddWithValue("@MaDichVu", maDichVu);
                    ins.Parameters.AddWithValue("@SoLuong", soLuong);
                    ins.Parameters.AddWithValue("@DVT", (object?)dvt ?? DBNull.Value);
                    ins.Parameters.AddWithValue("@DonGia", donGia);
                    ins.ExecuteNonQuery();
                }
            }
        }
    }
}
