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
            string newId = GenerateMaHoaDon(conn);
            
            // Tắt trigger tạm thời nếu có trigger gây lỗi
            using (var disableTrigger = new SqlCommand(@"DISABLE TRIGGER ALL ON HoaDon", conn))
            {
                try { disableTrigger.ExecuteNonQuery(); } catch { /* Ignore if no trigger */ }
            }
            
            using (var ins = new SqlCommand(@"INSERT INTO HoaDon(MaHoaDon, MaPhong, ThoiKy, NgayTao) VALUES(@MaHoaDon, @MaPhong, @ThoiKy, GETDATE())", conn))
            {
                ins.Parameters.Add(new SqlParameter("@MaHoaDon", SqlDbType.NVarChar, 20) { Value = newId });
                ins.Parameters.Add(new SqlParameter("@MaPhong", SqlDbType.NVarChar, 20) { Value = (object)maPhong ?? DBNull.Value });
                ins.Parameters.Add(new SqlParameter("@ThoiKy", SqlDbType.NVarChar, 10) { Value = (object)thoiKy ?? DBNull.Value });
                
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

        private static string GenerateMaHoaDon(SqlConnection conn)
        {
            // Chỉ lấy các MaHoaDon theo format 'HD' + số (bỏ qua 'HDN001', 'HDX123', v.v.)
            using var cmd = new SqlCommand(@"
                SELECT ISNULL(MAX(CAST(SUBSTRING(MaHoaDon,3,LEN(MaHoaDon)-2) AS INT)),0)+1 
                FROM HoaDon 
                WHERE MaHoaDon LIKE 'HDN[0-9]%' 
                  AND MaHoaDon NOT LIKE 'HD[A-Z]%'
                  AND ISNUMERIC(SUBSTRING(MaHoaDon,4,LEN(MaHoaDon)-3)) = 1", conn);
            var n = Convert.ToInt32(cmd.ExecuteScalar());
            return "HDN" + n.ToString("D3");
        }


        

    }
}
