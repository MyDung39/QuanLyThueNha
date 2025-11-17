using Microsoft.Data.SqlClient;
using System;
using System.Data;

namespace RoomManagementSystem.DataLayer
{
    public class LoiNhuan
    {
        private readonly string connectionString = DbConfig.ConnectionString;

        public DataTable GetLoiNhuanThang(string thoiKy)
        {
            DataTable dt = new DataTable();

            // thoiKy dạng "11/2025". Cần tách ra để xử lý trong SQL hoặc dùng chuỗi trực tiếp nếu DB lưu dạng chuỗi
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                // Logic:
                // 1. DoanhThu: Tổng tiền đã thanh toán của các hóa đơn thuộc kỳ này VÀ trạng thái = 'Đã trả'
                // 2. ChiPhi: Tổng (Điện + Nước trong Chi tiết hóa đơn) + (Chi phí Bảo trì trong tháng)

                string query = @"
                    SELECT 
                        p.MaPhong,
                        
                        -- 1. TÍNH DOANH THU (Chỉ lấy khi Đã trả)
                        ISNULL((
                            SELECT SUM(tt.SoTienDaThanhToan)
                            FROM ThanhToan tt
                            JOIN HoaDon hd ON tt.MaHoaDon = hd.MaHoaDon
                            WHERE hd.MaPhong = p.MaPhong 
                              AND hd.ThoiKy = @ThoiKy
                              AND tt.TrangThai = N'Đã trả'
                        ), 0) AS DoanhThu,

                        -- 2. TÍNH CHI PHÍ (Điện + Nước + Bảo trì)
                        (
                            -- Chi phí Điện (DV1) + Nước (DV2) từ Hóa đơn
                            ISNULL((
                                SELECT SUM(ct.ThanhTien)
                                FROM ChiTietHoaDon ct
                                JOIN HoaDon hd ON ct.MaHoaDon = hd.MaHoaDon
                                WHERE hd.MaPhong = p.MaPhong
                                  AND hd.ThoiKy = @ThoiKy
                                  AND ct.MaDichVu IN ('DV1', 'DV2') -- Chỉ tính Điện, Nước là Chi phí vận hành
                            ), 0)
                            +
                            -- Chi phí Bảo trì (Dựa trên tháng/năm của Ngày yêu cầu)
                            ISNULL((
                                SELECT SUM(bt.ChiPhi)
                                FROM BaoTri bt
                                WHERE bt.MaPhong = p.MaPhong
                                  AND FORMAT(bt.NgayYeuCau, 'MM/yyyy') = @ThoiKy
                            ), 0)
                        ) AS ChiPhi

                    FROM Phong p
                    ORDER BY p.MaPhong;
                ";

                using var cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@ThoiKy", thoiKy);

                using var da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }

            // 3. TÍNH LỢI NHUẬN (Cột tính toán)
            if (!dt.Columns.Contains("LoiNhuan"))
                dt.Columns.Add("LoiNhuan", typeof(decimal));

            foreach (DataRow row in dt.Rows)
            {
                decimal doanhThu = row["DoanhThu"] == DBNull.Value ? 0 : Convert.ToDecimal(row["DoanhThu"]);
                decimal chiPhi = row["ChiPhi"] == DBNull.Value ? 0 : Convert.ToDecimal(row["ChiPhi"]);

                // Lợi nhuận = Doanh thu - Chi phí
                row["LoiNhuan"] = doanhThu - chiPhi;
            }

            return dt;
        }
    }
}
