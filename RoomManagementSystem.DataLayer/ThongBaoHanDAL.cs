using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;

namespace RoomManagementSystem.DataLayer
{
    public class ThongBaoHanDAL
    {
        Database db = new Database();

        /// <summary>
        /// Lấy tất cả các thông báo hạn của một hợp đồng cụ thể.
        /// </summary>
        /// <param name="maHopDong">Mã hợp đồng cần tìm thông báo.</param>
        /// <returns>Danh sách các thông báo.</returns>
        public List<ThongBaoHan> GetByContractId(string maHopDong)
        {
            List<ThongBaoHan> list = new List<ThongBaoHan>();
            string sql = "SELECT * FROM ThongBaoHan WHERE MaHopDong = @MaHopDong ORDER BY NgayThongBao DESC";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@MaHopDong", maHopDong)
            };

            DataTable dt = db.ExecuteQuery(sql, parameters);

            foreach (DataRow row in dt.Rows)
            {
                list.Add(new ThongBaoHan
                {
                    MaThongBao = row["MaThongBao"].ToString(),
                    MaHopDong = row["MaHopDong"].ToString(),
                    NoiDung = row["NoiDung"].ToString(),
                    NgayThongBao = Convert.ToDateTime(row["NgayThongBao"]),
                    TrangThai = row["TrangThai"].ToString(),
                    NgayTao = Convert.ToDateTime(row["NgayTao"])
                });
            }
            return list;
        }

        // Bạn có thể thêm các phương thức khác ở đây nếu cần, ví dụ: Insert, Update...
    }
}