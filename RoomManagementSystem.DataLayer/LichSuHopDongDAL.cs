using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;

namespace RoomManagementSystem.DataLayer
{
    public class LichSuHopDongDAL
    {
        Database db = new Database();

        public bool Insert(LichSuHopDong lichSu)
        {
            string sql = @"INSERT INTO LichSuHopDong 
                           (MaHopDong, MaNguoiThayDoi, HanhDong, NoiDungThayDoi)
                           VALUES
                           (@MaHopDong, @MaNguoiThayDoi, @HanhDong, @NoiDungThayDoi)";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@MaHopDong", lichSu.MaHopDong),
                new SqlParameter("@MaNguoiThayDoi", lichSu.MaNguoiThayDoi),
                new SqlParameter("@HanhDong", lichSu.HanhDong),
                new SqlParameter("@NoiDungThayDoi", lichSu.NoiDungThayDoi ?? (object)DBNull.Value)
            };

            return db.ExecuteNonQuery(sql, parameters) > 0;
        }

        public List<LichSuHopDong> GetByContractId(string maHopDong)
        {
            List<LichSuHopDong> list = new List<LichSuHopDong>();
            string sql = @"SELECT ls.*, nd.TenTaiKhoan 
                           FROM LichSuHopDong ls
                           LEFT JOIN NguoiDung nd ON ls.MaNguoiThayDoi = nd.MaNguoiDung
                           WHERE ls.MaHopDong = @MaHopDong
                           ORDER BY ls.NgayThayDoi DESC";

            SqlParameter[] parameters = new SqlParameter[] { new SqlParameter("@MaHopDong", maHopDong) };
            DataTable dt = db.ExecuteQuery(sql, parameters);

            foreach (DataRow row in dt.Rows)
            {
                list.Add(new LichSuHopDong
                {
                    MaLichSu = Convert.ToInt32(row["MaLichSu"]),
                    MaHopDong = row["MaHopDong"].ToString(),
                    NgayThayDoi = Convert.ToDateTime(row["NgayThayDoi"]),
                    MaNguoiThayDoi = row["MaNguoiThayDoi"].ToString(),
                    HanhDong = row["HanhDong"].ToString(),
                    NoiDungThayDoi = row["NoiDungThayDoi"]?.ToString(),
                    TenNguoiThayDoi = row["TenTaiKhoan"]?.ToString() ?? "Không rõ"
                });
            }
            return list;
        }


        // HÀM ĐÃ SỬA
        public bool Insert(LichSuHopDong lichSu, SqlConnection conn, SqlTransaction tran)
        {
            // 1. Lấy câu SQL chính xác từ hàm Insert gốc của bạn
            string qr = @"INSERT INTO LichSuHopDong 
                        (MaHopDong, MaNguoiThayDoi, HanhDong, NoiDungThayDoi)
                      VALUES
                        (@MaHopDong, @MaNguoiThayDoi, @HanhDong, @NoiDungThayDoi)";

            // Tự tạo SqlCommand, không dùng 'db'
            using (SqlCommand cmd = new SqlCommand(qr, conn, tran))
            {
                // 2. Thêm các tham số (Parameters) từ hàm Insert gốc
                cmd.Parameters.Add(new SqlParameter("@MaHopDong", lichSu.MaHopDong));
                cmd.Parameters.Add(new SqlParameter("@MaNguoiThayDoi", lichSu.MaNguoiThayDoi));
                cmd.Parameters.Add(new SqlParameter("@HanhDong", lichSu.HanhDong));
                cmd.Parameters.Add(new SqlParameter("@NoiDungThayDoi", lichSu.NoiDungThayDoi ?? (object)DBNull.Value));

                // 3. Thực thi
                return cmd.ExecuteNonQuery() > 0;
            }
        }



        public bool DeleteByContractId(string maHopDong, SqlConnection conn, SqlTransaction tran)
        {
            string qr = "DELETE FROM LichSuHopDong WHERE MaHopDong = @MaHopDong";
            using (SqlCommand cmd = new SqlCommand(qr, conn, tran))
            {
                cmd.Parameters.AddWithValue("@MaHopDong", maHopDong);
                cmd.ExecuteNonQuery(); // Xóa 0 hoặc nhiều dòng đều được
                return true;
            }
        }


    }
}