using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoomManagementSystem.DataLayer
{
    public class ThanhToanDAL
    {
        string connection = "Data Source=LAPTOP-5FKFDEEM;Initial Catalog=QLTN;Integrated Security=True;Encrypt=True;Trust Server Certificate=True";

        // Thêm mới thanh toán
        public void Insert(ThanhToan thanhToan)
        {
            using (SqlConnection conn = new SqlConnection(connection))
            {
                string query = @"INSERT INTO ThanhToan
                                (MaThanhToan, MaPhong, MaHoaDon, MaHopDong, MaThongBaoPhi,
                                 TongCongNo, NgayHanThanhToan, PhuongThucThanhToan, TrangThai, NgayTao, NgayCapNhat)
                                VALUES (@MaThanhToan, @MaPhong, @MaHoaDon, @MaHopDong, @MaThongBaoPhi,
                                 @TongCongNo, @NgayHanThanhToan, @PhuongThucThanhToan, @TrangThai, GETDATE(), GETDATE())";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@MaThanhToan", thanhToan.MaThanhToan);
                cmd.Parameters.AddWithValue("@MaPhong", thanhToan.MaPhong);
                cmd.Parameters.AddWithValue("@MaHoaDon", thanhToan.MaHoaDon );
                cmd.Parameters.AddWithValue("@MaHopDong", thanhToan.MaHopDong);
                cmd.Parameters.AddWithValue("@MaThongBaoPhi", thanhToan.MaThongBaoPhi);
                cmd.Parameters.AddWithValue("@TongCongNo", thanhToan.TongCongNo);
                cmd.Parameters.AddWithValue("@NgayHanThanhToan", thanhToan.NgayHanThanhToan);
                cmd.Parameters.AddWithValue("@PhuongThucThanhToan", string.IsNullOrEmpty(thanhToan.PhuongThucThanhToan) ? "Tiền mặt" : thanhToan.PhuongThucThanhToan);
                cmd.Parameters.AddWithValue("@TrangThai", thanhToan.TrangThai);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        // Lấy danh sách tất cả thanh toán
        public List<ThanhToan> GetAll()
        {
            List<ThanhToan> list = new List<ThanhToan>();
            using (SqlConnection conn = new SqlConnection(connection))
            {
                string sql = "SELECT * FROM ThanhToan";
                SqlCommand cmd = new SqlCommand(sql, conn);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    list.Add(new ThanhToan
                    {
                        MaThanhToan = reader["MaThanhToan"].ToString(),
                        MaPhong = reader["MaPhong"].ToString(),
                        MaHopDong = reader["MaHopDong"].ToString(),
                        PhuongThucThanhToan = reader["PhuongThucThanhToan"].ToString(),
                        NgayHanThanhToan = Convert.ToDateTime(reader["NgayHanThanhToan"]),
                        TrangThai = reader["TrangThai"].ToString(),
                        NgayTao = Convert.ToDateTime(reader["NgayTao"]),
                        NgayCapNhat = Convert.ToDateTime(reader["NgayCapNhat"])
                    });
                }
            }
            return list;
        }

        // Cập nhật trạng thái thanh toán
        public void UpdateTrangThai(string maThanhToan, string trangThai)
        {
            using (SqlConnection conn = new SqlConnection(connection))
            {
                string sql = "UPDATE ThanhToan SET TrangThai = @TrangThai, NgayCapNhat = @NgayCapNhat WHERE MaThanhToan = @MaThanhToan";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@TrangThai", trangThai);
                cmd.Parameters.AddWithValue("@NgayCapNhat", DateTime.Now);
                cmd.Parameters.AddWithValue("@MaThanhToan", maThanhToan);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        // Lấy dữ liệu báo cáo theo tháng & năm
        public DataTable GetBaoCaoThang(int thang, int nam)
        {
            string query = @"
                SELECT 
                    MaPhong,
                    SUM(TongCongNo) AS TongTienPhaiTra,
                    COUNT(*) AS SoHoaDon,
                    SUM(CASE WHEN TrangThai = N'Đã trả' THEN 1 ELSE 0 END) AS SoHoaDonDaTra,
                    SUM(CASE WHEN TrangThai = N'Chưa trả' THEN 1 ELSE 0 END) AS SoHoaDonChuaTra
                FROM ThanhToan
                WHERE MONTH(NgayHanThanhToan) = @Thang
                  AND YEAR(NgayHanThanhToan) = @Nam
                GROUP BY MaPhong";

            using (SqlConnection conn = new SqlConnection(connection))
            {
                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                da.SelectCommand.Parameters.AddWithValue("@Thang", thang);
                da.SelectCommand.Parameters.AddWithValue("@Nam", nam);

                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
        }

        public DataTable BienLai(string maThanhToan)
        {
            using (SqlConnection conn = new SqlConnection(connection))
            {
                string qr = "SELECT * FROM ThanhToan WHERE MaThanhToan = @mtt";
                SqlCommand cmd = new SqlCommand(qr, conn);
                cmd.Parameters.AddWithValue("@mtt", maThanhToan);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
        }

    }
}
