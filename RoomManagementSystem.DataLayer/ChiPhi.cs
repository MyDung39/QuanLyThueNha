using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoomManagementSystem.DataLayer
{
    public class BaoCaoChiPhi
    {
        public string MaPhong { get; set; }
        public decimal ChiPhiDien { get; set; }
        public decimal ChiPhiNuoc { get; set; }
        public decimal ChiPhiInternet { get; set; }
        public decimal ChiPhiRac { get; set; }
        public decimal ChiPhiBaoTri { get; set; }

        public decimal Tong => ChiPhiDien + ChiPhiNuoc + ChiPhiInternet + ChiPhiRac + ChiPhiBaoTri;
    }
    public class BaoCaoChiPhiDAL
    {
        private string connectionString =
            @"Data Source=LAPTOP-JH9IJG9F\SQLEXPRESS;Initial Catalog=QLTN;Integrated Security=True;Encrypt=True;Trust Server Certificate=True";

        public DataTable GetChiPhiThang(string thoiKy)
        {
            DataTable dt = new DataTable();

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = @"
                        SELECT
                            hd.MaPhong AS [Mã Phòng],
                            SUM(CASE WHEN cthd.MaDichVu = 'DV1' THEN cthd.ThanhTien ELSE 0 END) AS [Chi phí điện],
                            SUM(CASE WHEN cthd.MaDichVu = 'DV2' THEN cthd.ThanhTien ELSE 0 END) AS [Chi phí nước],
                            SUM(CASE WHEN cthd.MaDichVu = 'DV3' THEN cthd.ThanhTien ELSE 0 END) AS [Chi phí Internet],
                            SUM(CASE WHEN cthd.MaDichVu = 'DV4' THEN cthd.ThanhTien ELSE 0 END) AS [Chi phí rác],
                            SUM(ISNULL(bt.ChiPhi,0)) AS [Chi phí bảo trì]
                        FROM HoaDon hd
                        LEFT JOIN ChiTietHoaDon cthd ON cthd.MaHoaDon = hd.MaHoaDon
                        LEFT JOIN BaoTri bt ON bt.MaPhong = hd.MaPhong
                            AND FORMAT(bt.NgayYeuCau,'MM/yyyy') = @ThoiKy
                        WHERE hd.ThoiKy = @ThoiKy
                        GROUP BY hd.MaPhong
                        ORDER BY hd.MaPhong;
                        ";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@ThoiKy", thoiKy);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }

            // ✅ Thêm cột Tổng (auto tính)
            dt.Columns.Add("Tổng", typeof(decimal));

            foreach (DataRow row in dt.Rows)
            {
                decimal tong = 0;
                for (int i = 1; i <= 5; i++) // 5 cột chi phí
                {
                    tong += Convert.ToDecimal(row[i]);
                }
                row["Tổng"] = tong;
            }

            return dt;
        }
    }
}
