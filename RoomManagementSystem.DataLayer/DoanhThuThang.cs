using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoomManagementSystem.DataLayer
{
    public class DoanhThuThang
    {
        Database db = new Database();

        public DataTable BaoCaoDoanhThuThang(string thoiKy)
        {
            string sql = @"
                SELECT 
                    p.MaPhong, 
                    nt.HoTen AS NguoiThue,
                    p.GiaThue AS TienThue,

                    tt.TrangThai AS TrangThaiThanhToan,
                    ISNULL(tt.TongCongNo, 0) AS TongTien
                FROM Phong p
                JOIN NguoiThue nt ON nt.MaPhong = p.MaPhong AND nt.VaiTro = N'Chủ hợp đồng'
                LEFT JOIN ThanhToan tt ON tt.MaPhong = p.MaPhong AND tt.TrangThai IN (N'Đã trả', N'Chưa trả', N'Trả một phần')
                WHERE p.TrangThai = N'Đang thuê' AND tt.MaHoaDon IN (
                    SELECT MaHoaDon FROM HoaDon WHERE ThoiKy = @ThoiKy
                )";

            SqlParameter[] parameters = {
                new SqlParameter("@ThoiKy", thoiKy)
                };

            return db.ExecuteQuery(sql, parameters);
        }
    }
}
