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
                    p.MaPhong,
                    p.GiaThue,
                    p.TrangThai,
                    t.TrangThai,
                    t.NgayhanThanhToan,
                    nt.HoTen
                FROM Phong p
                LEFT JOIN NguoiThue nt ON p.MaPhong = nt.MaPhong
                LEFT JOIN ThanhToan t ON p.MaPhong = t.MaPhong 
                       AND MONTH(t.NgayHanThanhToan) = @Thang AND YEAR(t.NgayHanThanhToan) = @Nam
                ";

            SqlParameter[] parameters = {
                new SqlParameter("@Thang", thang),
                new SqlParameter("@Nam", nam)
            };

            return db.ExecuteQuery(sql, parameters);
        }
    }
}
