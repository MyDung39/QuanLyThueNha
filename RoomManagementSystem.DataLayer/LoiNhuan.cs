using Microsoft.Data.SqlClient;
using System.Data;

namespace RoomManagementSystem.DataLayer
{
    public class LoiNhuan
    {
        private readonly string connectionString = DbConfig.ConnectionString;

        public DataTable GetLoiNhuanThang(string thoiKy)
        {
            DataTable dt = new DataTable();

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = @"
                    SELECT 
                        p.MaPhong AS MaPhong,
                        -- Doanh thu: tổng tiền thuê (nếu có thanh toán trong kỳ)
                        ISNULL(SUM(CASE WHEN t.MaThanhToan IS NOT NULL THEN p.GiaThue ELSE 0 END), 0) AS DoanhThu,
                        -- Chi phí: tổng các dịch vụ trong hóa đơn + tổng chi phí bảo trì
                        ISNULL(SUM(CASE WHEN cthd.MaDichVu IN ('DV1','DV2','DV3','DV4') THEN cthd.ThanhTien ELSE 0 END), 0)
                        + ISNULL(SUM(ISNULL(bt.ChiPhi,0)),0) AS ChiPhi
                    FROM Phong p
                    LEFT JOIN HoaDon hd 
                        ON hd.MaPhong = p.MaPhong 
                       AND hd.ThoiKy = @ThoiKy   -- dạng 'MM/yyyy'
                    LEFT JOIN ChiTietHoaDon cthd 
                        ON cthd.MaHoaDon = hd.MaHoaDon
                    LEFT JOIN BaoTri bt 
                        ON bt.MaPhong = p.MaPhong
                       AND FORMAT(bt.NgayYeuCau, 'MM/yyyy') = @ThoiKy
                    LEFT JOIN ThanhToan t  
                        ON p.MaPhong = t.MaPhong
                       AND FORMAT(t.NgayHanThanhToan, 'MM/yyyy') = @ThoiKy
                    GROUP BY p.MaPhong, p.GiaThue
                    ORDER BY p.MaPhong;
                ";

                using var cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@ThoiKy", thoiKy);

                using var da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }

            // Thêm cột LợiNhuan = DoanhThu - ChiPhi
            if (!dt.Columns.Contains("LoiNhuan"))
                dt.Columns.Add("LoiNhuan", typeof(decimal));

            foreach (DataRow row in dt.Rows)
            {
                decimal doanhThu = row["DoanhThu"] == DBNull.Value ? 0 : Convert.ToDecimal(row["DoanhThu"]);
                decimal chiPhi = row["ChiPhi"] == DBNull.Value ? 0 : Convert.ToDecimal(row["ChiPhi"]);
                row["LoiNhuan"] = doanhThu - chiPhi;
            }

            return dt;
        }
    }
}
