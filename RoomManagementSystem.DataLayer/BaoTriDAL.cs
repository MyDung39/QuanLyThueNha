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
        // Khởi tạo đối tượng Database để sử dụng các hàm của nó
        Database db = new Database();

        // Tạo mã bảo trì tự động
        public string AutoMaBT()
        {
            // Gọi hàm ExecuteScalar từ lớp Database
            string qr = "SELECT ISNULL(MAX(CAST(SUBSTRING(MaBaoTri, 3, LEN(MaBaoTri) - 2) AS INT)), 0) + 1 FROM BaoTri";
            int nextNumber = Convert.ToInt32(db.ExecuteScalar(qr)); // Không cần tham số

            return "BT" + nextNumber.ToString("D3");
        }

        // Thêm nhanh 1 bản ghi chi phí bảo trì theo phòng và thời kỳ (đánh dấu Hoàn tất)
        public void InsertChiPhiBaoTri(string maPhong, DateTime thoiKyDate, decimal chiPhi)
        {
            string ma = AutoMaBT();
            string sql = @"INSERT INTO BaoTri
                            (MaBaoTri, MaPhong, MaNguoiThue, MoTa, TrangThaiXuLy, NgayYeuCau, NgayHoanThanh, ChiPhi, NgayTao, NgayCapNhat)
                           VALUES
                            (@MaBaoTri, @MaPhong, NULL, @MoTa, N'Hoàn tất', @Ngay, @Ngay, @ChiPhi, GETDATE(), GETDATE())";

            SqlParameter[] ps = new SqlParameter[]
            {
                new SqlParameter("@MaBaoTri", ma),
                new SqlParameter("@MaPhong", maPhong),
                new SqlParameter("@MoTa", $"Chi phí bảo trì kỳ {thoiKyDate:MM/yyyy}"),
                new SqlParameter("@Ngay", thoiKyDate),
                new SqlParameter("@ChiPhi", chiPhi)
            };

            db.ExecuteNonQuery(sql, ps);
        }

        // Thêm mới yêu cầu bảo trì
        public void Insert(BaoTri baoTri)
        {
            string query = @"INSERT INTO BaoTri
                                (MaBaoTri, MaPhong, MaNguoiThue, MoTa, 
                                 TrangThaiXuLy, NgayYeuCau, NgayHoanThanh, ChiPhi, 
                                 NgayTao, NgayCapNhat)
                                VALUES 
                                (@MaBaoTri, @MaPhong, @MaNguoiThue, @MoTa, 
                                 @TrangThaiXuLy, @NgayYeuCau, @NgayHoanThanh, @ChiPhi, 
                                 GETDATE(), GETDATE())";

            // Chuẩn bị mảng SqlParameter
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@MaBaoTri", baoTri.MaBaoTri),
                new SqlParameter("@MaPhong", baoTri.MaPhong),
                new SqlParameter("@MaNguoiThue", baoTri.MaNguoiThue ??(object) DBNull.Value), // Xử lý nếu MaNguoiThue là null
                new SqlParameter("@MoTa", baoTri.MoTa),
                new SqlParameter("@TrangThaiXuLy", baoTri.TrangThaiXuLy),
                new SqlParameter("@NgayYeuCau", baoTri.NgayYeuCau),
                new SqlParameter("@NgayHoanThanh", baoTri.NgayHoanThanh ??(object) DBNull.Value),
                new SqlParameter("@ChiPhi", baoTri.ChiPhi)
            };

            // Gọi hàm ExecuteNonQuery từ lớp Database
            db.ExecuteNonQuery(query, parameters);
        }

        // Lấy danh sách tất cả yêu cầu bảo trì
        public List<BaoTri> GetAll()
        {
            List<BaoTri> list = new List<BaoTri>();
            string sql = @"
                SELECT 
                    bt.*, 
                    nt.HoTen AS TenNguoiThue 
                FROM BaoTri bt
                LEFT JOIN NguoiThue nt ON bt.MaNguoiThue = nt.MaNguoiThue
                ORDER BY bt.NgayYeuCau DESC";

            // Gọi hàm ExecuteQuery để lấy về DataTable
            DataTable dt = db.ExecuteQuery(sql);

            // Duyệt qua DataTable (thay vì SqlDataReader)
            foreach (DataRow reader in dt.Rows)
            {
                list.Add(new BaoTri
                {
                    MaBaoTri = reader["MaBaoTri"].ToString(),
                    MaPhong = reader["MaPhong"].ToString(),
                    MaNguoiThue = reader["MaNguoiThue"] == DBNull.Value ? null : reader["MaNguoiThue"].ToString(),
                    TenNguoiThue = reader["TenNguoiThue"] == DBNull.Value ? null : reader["TenNguoiThue"].ToString(),
                    MoTa = reader["MoTa"].ToString(),
                    TrangThaiXuLy = reader["TrangThaiXuLy"].ToString(),
                    NgayYeuCau = Convert.ToDateTime(reader["NgayYeuCau"]),
                    NgayHoanThanh = reader["NgayHoanThanh"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["NgayHoanThanh"]),
                    ChiPhi = Convert.ToDecimal(reader["ChiPhi"]),
                    NgayTao = Convert.ToDateTime(reader["NgayTao"]),
                    NgayCapNhat = Convert.ToDateTime(reader["NgayCapNhat"])
                });
            }
            return list;
        }

        // Cập nhật trạng thái xử lý
        public void UpdateTrangThai(string maBaoTri, string trangThai, DateTime? ngayHoanThanh)
        {
            string sql = @"UPDATE BaoTri 
                               SET TrangThaiXuLy = @TrangThaiXuLy, 
                                   NgayHoanThanh = @NgayHoanThanh, 
                                   NgayCapNhat = GETDATE() 
                               WHERE MaBaoTri = @MaBaoTri";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@TrangThaiXuLy", trangThai),
                new SqlParameter("@NgayHoanThanh", ngayHoanThanh ??(object) DBNull.Value),
                new SqlParameter("@MaBaoTri", maBaoTri)
            };

            db.ExecuteNonQuery(sql, parameters);
        }

        // Cập nhật chi phí sửa chữa
        public void UpdateChiPhi(string maBaoTri, decimal chiPhi)
        {
            string sql = @"UPDATE BaoTri 
                               SET ChiPhi = @ChiPhi, 
                                   NgayCapNhat = GETDATE() 
                               WHERE MaBaoTri = @MaBaoTri";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@ChiPhi", chiPhi),
                new SqlParameter("@MaBaoTri", maBaoTri)
            };

            db.ExecuteNonQuery(sql, parameters);
        }

        // Lấy dữ liệu báo cáo chi phí bảo trì theo tháng & năm 
        public DataTable GetBaoCaoChiPhiThang(int thang, int nam)
        {
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

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@Thang", thang),
                new SqlParameter("@Nam", nam)
            };

            // Hàm này đơn giản là trả về kết quả từ ExecuteQuery
            return db.ExecuteQuery(query, parameters);
        }

        // Lấy thông tin chi tiết 1 yêu cầu
        public BaoTri? GetById(string maBaoTri)
        {
            BaoTri? bt = null;
            string sql = @"
                SELECT 
                    bt.*, 
                    nt.HoTen AS TenNguoiThue 
                FROM BaoTri bt
                LEFT JOIN NguoiThue nt ON bt.MaNguoiThue = nt.MaNguoiThue
                WHERE bt.MaBaoTri = @MaBaoTri";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@MaBaoTri", maBaoTri)
            };

            // Lấy về DataTable
            DataTable dt = db.ExecuteQuery(sql, parameters);

            // Kiểm tra xem DataTable có dữ liệu không
            if (dt.Rows.Count > 0)
            {
                DataRow reader = dt.Rows[0]; // Lấy dòng đầu tiên
                bt = new BaoTri
                {
                    MaBaoTri = reader["MaBaoTri"].ToString(),
                    MaPhong = reader["MaPhong"].ToString(),
                    MaNguoiThue = reader["MaNguoiThue"] == DBNull.Value ? null : reader["MaNguoiThue"].ToString(),
                    TenNguoiThue = reader["TenNguoiThue"] == DBNull.Value ? null : reader["TenNguoiThue"].ToString(),
                    MoTa = reader["MoTa"].ToString(),
                    TrangThaiXuLy = reader["TrangThaiXuLy"].ToString(),
                    NgayYeuCau = Convert.ToDateTime(reader["NgayYeuCau"]),
                    NgayHoanThanh = reader["NgayHoanThanh"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["NgayHoanThanh"]),
                    ChiPhi = Convert.ToDecimal(reader["ChiPhi"]),
                    NgayTao = Convert.ToDateTime(reader["NgayTao"]),
                    NgayCapNhat = Convert.ToDateTime(reader["NgayCapNhat"])
                };
            }
            return bt;
        }

        public DataTable GetNguoiThueByPhong(string maPhong)
        {
            string query = @"
                SELECT 
                    nt.MaNguoiThue, 
                    nt.HoTen 
                FROM NguoiThue nt
                JOIN HopDong_NguoiThue hdnt ON nt.MaNguoiThue = hdnt.MaNguoiThue
                JOIN HopDong hd ON hdnt.MaHopDong = hd.MaHopDong
                WHERE 
                    hd.MaPhong = @MaPhong 
                    AND hd.TrangThai = N'Hiệu lực' 
                    AND hdnt.TrangThaiThue = N'Đang ở'";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@MaPhong", maPhong)
            };

            return db.ExecuteQuery(query, parameters);
        }


        public bool Delete(List<string> maBaoTris)
        {
            if (maBaoTris == null || !maBaoTris.Any())
            {
                return false;
            }

            // Tạo danh sách các tham số động (@p0, @p1, @p2, ...)
            var parameters = new List<SqlParameter>();
            var parameterNames = new List<string>();
            for (int i = 0; i < maBaoTris.Count; i++)
            {
                string paramName = $"@p{i}";
                parameterNames.Add(paramName);
                parameters.Add(new SqlParameter(paramName, maBaoTris[i]));
            }

            // Xây dựng câu lệnh SQL với IN clause
            string sql = $"DELETE FROM BaoTri WHERE MaBaoTri IN ({string.Join(", ", parameterNames)})";

            return db.ExecuteNonQuery(sql, parameters.ToArray()) > 0;
        }


    }
}