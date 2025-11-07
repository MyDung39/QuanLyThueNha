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

        public DataTable BaoCaoDoanhThuThang(int thang, int nam)
        {
            string sql = @"
                SELECT 
                    p.*,
                    tt.TrangThai,
                    STRING_AGG(nt.HoTen, ', ') AS DanhSachKhach,
                    SUM(tt.TongCongNo) AS TongDanhThuThucKien,
                    SUM(CASE WHEN tt.TrangThai = N'Đã trả' THEN tt.TongCongNo ELSE 0 END) AS TongDaThanhToan
                FROM Phong p
                LEFT JOIN HopDong hd ON p.MaPhong = hd.MaPhong AND hd.TrangThai = N'Hiệu lực'
                LEFT JOIN HopDong_NguoiThue hdnt ON hd.MaHopDong = hdnt.MaHopDong
                LEFT JOIN NguoiThue nt ON hdnt.MaNguoiThue = nt.MaNguoiThue 
                LEFT JOIN ThanhToan tt ON p.MaPhong = tt.MaPhong 
                       AND MONTH(tt.NgayHanThanhToan) = @Thang AND YEAR(tt.NgayHanThanhToan) = @Nam
                WHERE p.TrangThai=N'Đang thuê'
                GROUP BY
                p.MaPhong, 
                p.MaNha, 
                p.LoaiPhong, 
                p.DienTich, 
                p.GiaThue, 
                p.TrangThai, 
                p.SoNguoiHienTai, 
                p.GhiChu, 
                p.NgayTao,
                p.NgayCapNhat,
                tt.TrangThai
                ORDER BY p.MaPhong
                            ";

            SqlParameter[] parameters = {
                new SqlParameter("@Thang", thang),
                new SqlParameter("@Nam", nam)
            };

            return db.ExecuteQuery(sql, parameters);
        }
    }
}
