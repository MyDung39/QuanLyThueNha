using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoomManagementSystem.DataLayer
{
    public class CongNo
    {
        private readonly Database db = new Database();

        // Lấy danh sách công nợ (những người còn nợ tiền)
        public DataTable GetDanhSachCongNo()
        {
            string query = @"
                SELECT 
                    nt.MaNguoiThue,
                    nt.HoTen,
                    p.MaPhong,
                    t.MaThanhToan,
                    t.TongCongNo,
                    t.SoTienDaThanhToan,
                    (t.TongCongNo - t.SoTienDaThanhToan) AS SoTienConLai,
                    t.NgayHanThanhToan,
                    t.TrangThai
                FROM ThanhToan t
                INNER JOIN HopDong hd ON t.MaHopDong = hd.MaHopDong
                INNER JOIN HopDong_NguoiThue hdnt ON hd.MaHopDong = hdnt.MaHopDong
                INNER JOIN NguoiThue nt ON hdnt.MaNguoiThue = nt.MaNguoiThue
                INNER JOIN Phong p ON hd.MaPhong = p.MaPhong
                WHERE (t.TongCongNo - t.SoTienDaThanhToan) > 0
                AND hdnt.VaiTro = N'Chủ hợp đồng' 
                ORDER BY t.NgayHanThanhToan ASC;
            ";

            return db.ExecuteQuery(query);
        }

        // 🔹 Lấy lịch sử thanh toán theo mã người thuê
        public DataTable GetLichSuThanhToan(string maNguoiThue)
        {
            string query = @"
                SELECT 
                    t.MaThanhToan,
                    t.NgayTao,
                    t.PhuongThucThanhToan,
                    t.SoTienDaThanhToan,
                    (t.TongCongNo - t.SoTienDaThanhToan) AS SoTienConLai,
                    t.TrangThai
                FROM ThanhToan t
                INNER JOIN HopDong_NguoiThue hd ON t.MaHopDong = hd.MaHopDong
                WHERE hd.MaNguoiThue = @MaNguoiThue
                ORDER BY t.NgayTao DESC;
            ";

            SqlParameter[] parameters =
            {
                new SqlParameter("@MaNguoiThue", maNguoiThue)
            };

            return db.ExecuteQuery(query, parameters);
        }
    }
}
