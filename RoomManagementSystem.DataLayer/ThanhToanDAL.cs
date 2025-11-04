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
        private string connectionString = "Data Source=LAPTOP-JH9IJG9F\\SQLEXPRESS;Initial Catalog=QLTN;Integrated Security=True;Encrypt=True;Trust Server Certificate=True";
        // Lấy danh sách thanh toán
        public DataTable GetDanhSachThanhToan()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT * FROM ThanhToan";
                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
        }
        //Tự động sinh mã thanh toán (TT001, TT002, ...)
        public string TaoMaThanhToanTuDong()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT MAX(MaThanhToan) FROM ThanhToan";
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                object result = cmd.ExecuteScalar();

                if (result == DBNull.Value || result == null)
                    return "TT001";

                string lastCode = result.ToString(); // ví dụ: "TT009"
                int numberPart = int.Parse(lastCode.Substring(2)); // => 9
                numberPart++; // => 10
                return "TT" + numberPart.ToString("D3"); // => "TT010"
            }
        }
        //Them mới thanh toán
        public bool ThemThanhToan(ThanhToan thanhToan)
        {
            string maThanhToan = TaoMaThanhToanTuDong(); // mã tự tạo

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"INSERT INTO ThanhToan (
                            MaThanhToan, MaPhong, MaHoaDon, MaHopDong, MaThongBaoPhi, 
                            TongCongNo, NgayHanThanhToan, PhuongThucThanhToan, TrangThai, 
                            NgayTao, NgayCapNhat, SoTienDaThanhToan)
                         VALUES (
                            @MaThanhToan, @MaPhong, @MaHoaDon, @MaHopDong, @MaThongBaoPhi,
                            @TongCongNo, @NgayHanThanhToan, @PhuongThucThanhToan, @TrangThai,
                            GETDATE(), GETDATE(), @SoTienDaThanhToan)";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@MaThanhToan", maThanhToan);
                cmd.Parameters.AddWithValue("@MaPhong", thanhToan.MaPhong ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@MaHoaDon", thanhToan.MaHoaDon ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@MaHopDong", thanhToan.MaHopDong ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@MaThongBaoPhi", thanhToan.MaThongBaoPhi ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@TongCongNo", thanhToan.TongCongNo);
                cmd.Parameters.AddWithValue("@NgayHanThanhToan", thanhToan.NgayHanThanhToan ?? (object)DBNull.Value);

                cmd.Parameters.AddWithValue("@PhuongThucThanhToan",
                    string.IsNullOrEmpty(thanhToan.PhuongThucThanhToan) ? "Tiền mặt" : thanhToan.PhuongThucThanhToan);

                cmd.Parameters.AddWithValue("@TrangThai", thanhToan.TrangThai ?? "Chưa trả");
                cmd.Parameters.AddWithValue("@SoTienDaThanhToan",
                    thanhToan.SoTienDaThanhToan >= 0 ? thanhToan.SoTienDaThanhToan : 0);

                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }
        // Ghi nhận thanh toán
        public bool CapNhatThanhToan(string maThanhToan, decimal soTien, string phuongThuc, string ghiChu)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"UPDATE ThanhToan 
                                 SET SoTienDaThanhToan = SoTienDaThanhToan + @SoTien,
                                     PhuongThucThanhToan = @PhuongThuc,
                                     NgayCapNhat = GETDATE()
                                 WHERE MaThanhToan = @MaThanhToan";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@SoTien", soTien);
                cmd.Parameters.AddWithValue("@PhuongThuc", phuongThuc);
                cmd.Parameters.AddWithValue("@MaThanhToan", maThanhToan);
                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }
        //Lấy thanh toán theo phòng 
        public DataTable GetThanhToanTheoPhong(string maPhong)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"
                    SELECT 
                        tt.MaThanhToan,
                        tt.MaPhong,
                        tt.TongCongNo,
                        tt.SoTienDaThanhToan,
                        (tt.TongCongNo - tt.SoTienDaThanhToan) AS SoTienConLai,
                        tt.PhuongThucThanhToan,
                        tt.NgayTao,
                        tt.NgayCapNhat
                    FROM ThanhToan tt
                    JOIN Phong p ON tt.MaPhong = p.MaPhong
                    JOIN HopDong hd ON tt.MaHopDong = hd.MaHopDong
                    JOIN NguoiThue nt ON hd.MaNguoiThue = nt.MaNguoiThue
                    WHERE tt.MaPhong = @MaPhong";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@MaPhong", maPhong);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
        }

    }
}