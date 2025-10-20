using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoomManagementSystem.DataLayer
{
    public class BaoTriDAL
    {
        string connection = "Data Source=LAPTOP-5FKFDEEM;Initial Catalog=QLTN;Integrated Security=True;Encrypt=True;Trust Server Certificate=True";

        // Thêm mới yêu cầu bảo trì
        public void Insert(BaoTri baoTri)
        {
            using (SqlConnection conn = new SqlConnection(connection))
            {
                string query = @"INSERT INTO BaoTri
                                (MaBaoTri, MaPhong, MaNguoiThue, NguonYeuCau, MoTa, 
                                 TrangThaiXuLy, NgayYeuCau, NgayHoanThanh, ChiPhi, 
                                 NgayTao, NgayCapNhat)
                                VALUES 
                                (@MaBaoTri, @MaPhong, @MaNguoiThue, @NguonYeuCau, @MoTa, 
                                 @TrangThaiXuLy, @NgayYeuCau, @NgayHoanThanh, @ChiPhi, 
                                 GETDATE(), GETDATE())";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@MaBaoTri", baoTri.MaBaoTri);
                cmd.Parameters.AddWithValue("@MaPhong", baoTri.MaPhong);
                cmd.Parameters.AddWithValue("@MaNguoiThue", (object)baoTri.MaNguoiThue ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@NguonYeuCau", baoTri.NguonYeuCau);
                cmd.Parameters.AddWithValue("@MoTa", baoTri.MoTa);
                cmd.Parameters.AddWithValue("@TrangThaiXuLy", baoTri.TrangThaiXuLy);
                cmd.Parameters.AddWithValue("@NgayYeuCau", baoTri.NgayYeuCau);
                cmd.Parameters.AddWithValue("@NgayHoanThanh", (object)baoTri.NgayHoanThanh ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@ChiPhi", baoTri.ChiPhi);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        // Lấy danh sách tất cả yêu cầu bảo trì
        public List<BaoTri> GetAll()
        {
            List<BaoTri> list = new List<BaoTri>();
            using (SqlConnection conn = new SqlConnection(connection))
            {
                string sql = "SELECT * FROM BaoTri ORDER BY NgayYeuCau DESC";
                SqlCommand cmd = new SqlCommand(sql, conn);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    list.Add(new BaoTri
                    {
                        MaBaoTri = reader["MaBaoTri"].ToString(),
                        MaPhong = reader["MaPhong"].ToString(),
                        MaNguoiThue = reader["MaNguoiThue"] == DBNull.Value ? null : reader["MaNguoiThue"].ToString(),
                        NguonYeuCau = reader["NguonYeuCau"].ToString(),
                        MoTa = reader["MoTa"].ToString(),
                        TrangThaiXuLy = reader["TrangThaiXuLy"].ToString(),
                        NgayYeuCau = Convert.ToDateTime(reader["NgayYeuCau"]),
                        NgayHoanThanh = reader["NgayHoanThanh"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["NgayHoanThanh"]),
                        ChiPhi = Convert.ToDecimal(reader["ChiPhi"]),
                        NgayTao = Convert.ToDateTime(reader["NgayTao"]),
                        NgayCapNhat = Convert.ToDateTime(reader["NgayCapNhat"])
                    });
                }
            }
            return list;
        }

        // Cập nhật trạng thái xử lý
        public void UpdateTrangThai(string maBaoTri, string trangThai, DateTime? ngayHoanThanh)
        {
            using (SqlConnection conn = new SqlConnection(connection))
            {
                string sql = @"UPDATE BaoTri 
                               SET TrangThaiXuLy = @TrangThaiXuLy, 
                                   NgayHoanThanh = @NgayHoanThanh, 
                                   NgayCapNhat = GETDATE() 
                               WHERE MaBaoTri = @MaBaoTri";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@TrangThaiXuLy", trangThai);
                cmd.Parameters.AddWithValue("@NgayHoanThanh", (object)ngayHoanThanh ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@MaBaoTri", maBaoTri);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        // Cập nhật chi phí sửa chữa
        public void UpdateChiPhi(string maBaoTri, decimal chiPhi)
        {
            using (SqlConnection conn = new SqlConnection(connection))
            {
                string sql = @"UPDATE BaoTri 
                               SET ChiPhi = @ChiPhi, 
                                   NgayCapNhat = GETDATE() 
                               WHERE MaBaoTri = @MaBaoTri";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@ChiPhi", chiPhi);
                cmd.Parameters.AddWithValue("@MaBaoTri", maBaoTri);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        // Lấy dữ liệu báo cáo chi phí bảo trì theo tháng & năm 
        public DataTable GetBaoCaoChiPhiThang(int thang, int nam)
        {
            // Báo cáo chi phí bảo trì đã hoàn thành trong tháng
            string query = @"
                SELECT 
                    MaPhong,
                    COUNT(*) AS SoLanHoanThanh,
                    SUM(ChiPhi) AS TongChiPhiBaoTri
                FROM BaoTri
                WHERE MONTH(NgayHoanThanh) = @Thang
                  AND YEAR(NgayHoanThanh) = @Nam
                  AND TrangThaiXuLy = N'Hoàn tất'
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

        // Lấy thông tin chi tiết 1 yêu cầu
        public BaoTri? GetById(string maBaoTri)
        {
            BaoTri? bt = null;
            using (SqlConnection conn = new SqlConnection(connection))
            {
                string sql = "SELECT * FROM BaoTri WHERE MaBaoTri = @MaBaoTri";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@MaBaoTri", maBaoTri);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    bt = new BaoTri
                    {
                        MaBaoTri = reader["MaBaoTri"].ToString(),
                        MaPhong = reader["MaPhong"].ToString(),
                        MaNguoiThue = reader["MaNguoiThue"] == DBNull.Value ? null : reader["MaNguoiThue"].ToString(),
                        NguonYeuCau = reader["NguonYeuCau"].ToString(),
                        MoTa = reader["MoTa"].ToString(),
                        TrangThaiXuLy = reader["TrangThaiXuLy"].ToString(),
                        NgayYeuCau = Convert.ToDateTime(reader["NgayYeuCau"]),
                        NgayHoanThanh = reader["NgayHoanThanh"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["NgayHoanThanh"]),
                        ChiPhi = Convert.ToDecimal(reader["ChiPhi"]),
                        NgayTao = Convert.ToDateTime(reader["NgayTao"]),
                        NgayCapNhat = Convert.ToDateTime(reader["NgayCapNhat"])
                    };
                }
            }
            return bt;
        }
    }
}