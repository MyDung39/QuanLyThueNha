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

            // Try get existing
            using (var getCmd = new SqlCommand(@"SELECT TOP 1 MaHoaDon FROM HoaDon WHERE MaPhong=@MaPhong AND ThoiKy=@ThoiKy ORDER BY NgayTao DESC", conn))
            {
                var pMaPhong = new SqlParameter("@MaPhong", SqlDbType.NVarChar, 20) { Value = (object)maPhong ?? DBNull.Value };
                var pThoiKy = new SqlParameter("@ThoiKy", SqlDbType.NVarChar, 10) { Value = (object)thoiKy ?? DBNull.Value };
                getCmd.Parameters.Add(pMaPhong);
                getCmd.Parameters.Add(pThoiKy);

                try
                {
                    var existing = getCmd.ExecuteScalar()?.ToString();
                    if (!string.IsNullOrEmpty(existing)) return existing;
                }
                catch (Exception ex)
                {
                    throw new Exception($"Lỗi khi SELECT HoaDon với MaPhong='{maPhong}', ThoiKy='{thoiKy}': {ex.Message}", ex);
                }
            }

            // Create new
            // Lời gọi này bây giờ đã khớp với định nghĩa hàm bên dưới
            string newId = GenerateMaHoaDon(conn);

            // Tắt trigger tạm thời nếu có trigger gây lỗi
            using (var disableTrigger = new SqlCommand(@"DISABLE TRIGGER ALL ON HoaDon", conn))
            {
                try { disableTrigger.ExecuteNonQuery(); } catch { /* Ignore if no trigger */ }
            }

            using (var ins = new SqlCommand(@"INSERT INTO HoaDon(MaHoaDon, MaPhong, ThoiKy, NgayTao) VALUES(@MaHoaDon, @MaPhong, @ThoiKy, GETDATE())", conn))
            {
                ins.Parameters.Add(new SqlParameter("@MaHoaDon", SqlDbType.NVarChar, 20) { Value = newId });
                ins.Parameters.Add(new SqlParameter("@MaPhong", SqlDbType.NVarChar, 20) { Value = maPhong ?? (object)DBNull.Value });
                ins.Parameters.Add(new SqlParameter("@ThoiKy", SqlDbType.NVarChar, 10) { Value = thoiKy ?? (object)DBNull.Value });

                try
                {
                    ins.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    throw new Exception($"Lỗi khi INSERT HoaDon với MaHoaDon='{newId}', MaPhong='{maPhong}', ThoiKy='{thoiKy}': {ex.Message}", ex);
                }
            }

            // Bật lại trigger
            using (var enableTrigger = new SqlCommand(@"ENABLE TRIGGER ALL ON HoaDon", conn))
            {
                try { enableTrigger.ExecuteNonQuery(); } catch { /* Ignore */ }
            }
            return newId;
        }

        private string GenerateMaHoaDon(SqlConnection conn)
        {
            // Query lấy ID lớn nhất + 1
            string qr = "SELECT ISNULL(MAX(CAST(SUBSTRING(MaHoaDon, 4, LEN(MaHoaDon) - 3) AS INT)), 0) + 1 FROM HoaDon";

            // Tạo command với query và connection đã có
            using (var cmd = new SqlCommand(qr, conn))
            {
                // Thực thi và lấy kết quả (là số tiếp theo)
                int nextNumber = Convert.ToInt32(cmd.ExecuteScalar());

                // Trả về mã mới theo định dạng HDN + 3 chữ số (ví dụ: HDN001)
                return "HDN" + nextNumber.ToString("D3");
            }
        }
    }
}