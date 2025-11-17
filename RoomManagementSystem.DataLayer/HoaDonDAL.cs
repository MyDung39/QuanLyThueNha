

using Microsoft.Data.SqlClient;
using System;
using System.Data;

namespace RoomManagementSystem.DataLayer
{
    public class HoaDonDAL
    {
        private readonly string connectionString = DbConfig.ConnectionString;

        public string GetOrCreateByPhongThoiKy(string maPhong, string thoiKy)
        {
            using var conn = new SqlConnection(connectionString);
            conn.Open();

            // Kiểm tra tồn tại
            using (var getCmd = new SqlCommand(@"SELECT TOP 1 MaHoaDon FROM HoaDon WHERE MaPhong=@MaPhong AND ThoiKy=@ThoiKy ORDER BY NgayTao DESC", conn))
            {
                getCmd.Parameters.AddWithValue("@MaPhong", maPhong);
                getCmd.Parameters.AddWithValue("@ThoiKy", thoiKy);
                var existing = getCmd.ExecuteScalar()?.ToString();
                if (!string.IsNullOrEmpty(existing)) return existing;
            }

            // Tạo mới
            string newId = GenerateMaHoaDon(conn);
            using (var ins = new SqlCommand(@"INSERT INTO HoaDon(MaHoaDon, MaPhong, ThoiKy, NgayTao) VALUES(@MaHoaDon, @MaPhong, @ThoiKy, GETDATE())", conn))
            {
                ins.Parameters.AddWithValue("@MaHoaDon", newId);
                ins.Parameters.AddWithValue("@MaPhong", maPhong);
                ins.Parameters.AddWithValue("@ThoiKy", thoiKy);
                ins.ExecuteNonQuery();
            }
            return newId;
        }

        private string GenerateMaHoaDon(SqlConnection conn)
        {
            string qr = "SELECT ISNULL(MAX(CAST(SUBSTRING(MaHoaDon, 4, LEN(MaHoaDon) - 3) AS INT)), 0) + 1 FROM HoaDon";
            using (var cmd = new SqlCommand(qr, conn))
            {
                int nextNumber = Convert.ToInt32(cmd.ExecuteScalar());
                return "HDN" + nextNumber.ToString("D3");
            }
        }

        // [Yêu cầu 5] Hàm tính số người lưu trú
        public int CountTenantsInRoom(string maPhong)
        {
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                // Gọi Function đã có trong SQL Script: fn_TinhSoNguoiHienTai 
                string qr = "SELECT dbo.fn_TinhSoNguoiHienTai(@MaPhong)";
                using (var cmd = new SqlCommand(qr, conn))
                {
                    cmd.Parameters.AddWithValue("@MaPhong", maPhong);
                    object result = cmd.ExecuteScalar();
                    return result != null && result != DBNull.Value ? Convert.ToInt32(result) : 0;
                }
            }
        }
    }
}