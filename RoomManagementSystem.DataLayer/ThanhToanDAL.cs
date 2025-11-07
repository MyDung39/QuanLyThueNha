using Microsoft.Data.SqlClient;
using System;
using System.Data;

namespace RoomManagementSystem.DataLayer
{
    public class ThanhToanDAL
    {
        private readonly Database db = new Database();

        // Lấy danh sách thanh toán
        public DataTable GetDanhSachThanhToan()
        {
            string query = "SELECT * FROM ThanhToan";
            return db.ExecuteQuery(query);
        }

        // Tự động sinh mã thanh toán (TT001, TT002, ...)
        public string TaoMaThanhToanTuDong()
        {
            string query = "SELECT MAX(MaThanhToan) FROM ThanhToan";
            object result = db.ExecuteScalar(query);

            if (result == DBNull.Value || result == null)
                return "TT001";

            string lastCode = result.ToString(); // ví dụ: "TT009"
            int numberPart = int.Parse(lastCode.Substring(2)); // => 9
            numberPart++; // => 10
            return "TT" + numberPart.ToString("D3"); // => "TT010"
        }

        // Thêm mới thanh toán
        public bool ThemThanhToan(ThanhToan thanhToan)
        {
            string maThanhToan = TaoMaThanhToanTuDong();

            // Tính trạng thái tự động
            decimal soTienDaThanhToan = thanhToan.SoTienDaThanhToan;
            string trangThai;
            if (soTienDaThanhToan <= 0)
                trangThai = "Chưa trả";
            else if (soTienDaThanhToan < thanhToan.TongCongNo)
                trangThai = "Trả một phần";
            else
                trangThai = "Đã trả";

            string query = @"
        INSERT INTO ThanhToan (
            MaThanhToan, MaPhong, MaHoaDon, MaHopDong, MaThongBaoPhi, 
            TongCongNo, SoTienDaThanhToan, NgayHanThanhToan, PhuongThucThanhToan, TrangThai, 
            NgayTao, NgayCapNhat
        )
        VALUES (
            @MaThanhToan, @MaPhong, @MaHoaDon, @MaHopDong, @MaThongBaoPhi,
            @TongCongNo, @SoTienDaThanhToan, @NgayHanThanhToan, @PhuongThucThanhToan, @TrangThai,
            GETDATE(), GETDATE()
        )";

            SqlParameter[] parameters = new SqlParameter[]
            {
        new SqlParameter("@MaThanhToan", maThanhToan),
        new SqlParameter("@MaPhong", thanhToan.MaPhong ?? (object)DBNull.Value),
        new SqlParameter("@MaHoaDon", thanhToan.MaHoaDon ?? (object)DBNull.Value),
        new SqlParameter("@MaHopDong", thanhToan.MaHopDong ?? (object)DBNull.Value),
        new SqlParameter("@MaThongBaoPhi", thanhToan.MaThongBaoPhi ?? (object)DBNull.Value),
        new SqlParameter("@TongCongNo", thanhToan.TongCongNo),
        new SqlParameter("@SoTienDaThanhToan", soTienDaThanhToan),
        new SqlParameter("@NgayHanThanhToan", thanhToan.NgayHanThanhToan ?? (object)DBNull.Value),
        new SqlParameter("@PhuongThucThanhToan",
            string.IsNullOrEmpty(thanhToan.PhuongThucThanhToan) ? "Tiền mặt" : thanhToan.PhuongThucThanhToan),
        new SqlParameter("@TrangThai", trangThai)
            };

            return db.ExecuteNonQuery(query, parameters) > 0;
        }
        // Cập nhật thanh toán
        public bool CapNhatThanhToan(string maThanhToan, decimal soTien, string phuongThuc)
        {
            string query = @"
        UPDATE ThanhToan 
        SET 
            SoTienDaThanhToan = SoTienDaThanhToan + @SoTien,
            PhuongThucThanhToan = @PhuongThuc,
            NgayCapNhat = GETDATE()
        WHERE MaThanhToan = @MaThanhToan;

        -- Cập nhật lại trạng thái dựa theo số tiền
        UPDATE ThanhToan
        SET TrangThai = 
            CASE 
                WHEN SoTienDaThanhToan = 0 THEN N'Chưa trả'
                WHEN SoTienDaThanhToan < TongCongNo THEN N'Trả một phần'
                WHEN SoTienDaThanhToan >= TongCongNo THEN N'Đã trả'
            END
        WHERE MaThanhToan = @MaThanhToan;
    ";

            SqlParameter[] parameters = new SqlParameter[]
            {
        new SqlParameter("@SoTien", soTien),
        new SqlParameter("@PhuongThuc", phuongThuc),
        new SqlParameter("@MaThanhToan", maThanhToan)
            };

            return db.ExecuteNonQuery(query, parameters) > 0;
        }

        // Lấy thanh toán theo phòng
        public DataTable GetThanhToanTheoPhong(string maPhong)
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

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@MaPhong", maPhong)
            };

            return db.ExecuteQuery(query, parameters);
        }
    }
}
