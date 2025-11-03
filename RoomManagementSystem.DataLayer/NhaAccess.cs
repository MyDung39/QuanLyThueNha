using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoomManagementSystem.DataLayer
{
    public class NhaAccess
    {
        // Thêm đối tượng Database
        Database db = new Database();

        // Phương thức mở rộng để xử lý an toàn giá trị từ DB
        private static string ToSafeString(object value)
        {
            return value == null || value is DBNull ? string.Empty : value.ToString() ?? string.Empty;
        }

        // Tạo mã nhà tự động
        public string AutoMaNha()
        {
            string qr = "SELECT ISNULL(MAX(CAST(SUBSTRING(MaNha, 4, LEN(MaNha) - 3) AS INT)), 0) + 1 FROM Nha";
            int nextNumber = Convert.ToInt32(db.ExecuteScalar(qr));
            return "NHA" + nextNumber.ToString("D3");
        }

        //Them thong tin can nha
        public Boolean registerHouse(string MaNha, string DiaChi, int SoPhong, int TongSoPhongHienTai, string GhiChu)
        {
            // Dùng ExecuteScalar để lấy MaNguoiDung
            string a = "SELECT MaNguoiDung FROM NguoiDung"; // Lưu ý: Câu query này sẽ lấy MaNguoiDung ĐẦU TIÊN tìm thấy. Hãy đảm bảo đây là logic bạn muốn.
            var MaNguoiDung = db.ExecuteScalar(a);

            // Nếu không tìm thấy người dùng, có thể trả về false
            if (MaNguoiDung == null)
            {
                return false;
            }

            string qr = @"INSERT INTO Nha (MaNguoiDung,MaNha, DiaChi, TongSoPhong, TongSoPhongHienTai, GhiChu, NgayTao, NgayCapNhat) 
                          VALUES (@MaNguoiDung,@MaNha, @DiaChi, @TongSoPhong, @TongSoPhongHienTai, @GhiChu, GETDATE(), GETDATE())";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@MaNguoiDung", MaNguoiDung.ToString()),
                new SqlParameter("@MaNha", MaNha),
                new SqlParameter("@DiaChi", DiaChi),
                new SqlParameter("@TongSoPhong", SoPhong),
                new SqlParameter("@TongSoPhongHienTai", TongSoPhongHienTai),
                new SqlParameter("@GhiChu", GhiChu)
            };

            // Dùng ExecuteNonQuery
            int result = db.ExecuteNonQuery(qr, parameters);
            return result > 0; // Trả về true nếu thêm thành công 
        }

        //Cap nhat thong tin can nha
        public Boolean updateHouse(string MaNha, string DiaChi, int SoPhong, int TongSoPhongHienTai, string GhiChu)
        {
            string a = "SELECT MaNguoiDung FROM NguoiDung"; // Tương tự, đây là MaNguoiDung đầu tiên
            var MaNguoiDung = db.ExecuteScalar(a);

            if (MaNguoiDung == null)
            {
                return false;
            }

            string qr = @"UPDATE Nha SET MaNguoiDung = @MaNguoiDung,DiaChi = @DiaChi,TongSoPhong = @TongSoPhong,TongSoPhongHienTai = @TongSoPhongHienTai,GhiChu = @GhiChu,NgayCapNhat = GETDATE()  WHERE MaNha = @MaNha";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@MaNguoiDung", MaNguoiDung.ToString()),
                new SqlParameter("@MaNha", MaNha),
                new SqlParameter("@DiaChi", DiaChi),
                new SqlParameter("@TongSoPhong", SoPhong),
                new SqlParameter("@TongSoPhongHienTai", TongSoPhongHienTai),
                new SqlParameter("@GhiChu", GhiChu)
            };

            int result = db.ExecuteNonQuery(qr, parameters);
            return result > 0; // Trả về true nếu thành công 
        }

        // Lấy tất cả nhà
        public List<Nha> getAllHouse()
        {
            List<Nha> ds = new List<Nha>();
            string qr = "SELECT MaNha, MaNguoiDung, DiaChi, TongSoPhong, TongSoPhongHienTai, GhiChu, NgayTao, NgayCapNhat FROM Nha";

            // Dùng ExecuteQuery và DataTable
            DataTable dt = db.ExecuteQuery(qr);

            foreach (DataRow row in dt.Rows)
            {
                string maNha = ToSafeString(row["MaNha"]);
                string maNguoiDung = ToSafeString(row["MaNguoiDung"]);
                string diaChi = ToSafeString(row["DiaChi"]);
                string ghiChu = ToSafeString(row["GhiChu"]);

                int tongSoPhong = row["TongSoPhong"] is DBNull ? 0 : Convert.ToInt32(row["TongSoPhong"]);
                int tongSoPhongHienTai = row["TongSoPhongHienTai"] is DBNull ? 0 : Convert.ToInt32(row["TongSoPhongHienTai"]);

                DateTime ngayTao = row["NgayTao"] is DBNull ? DateTime.MinValue : Convert.ToDateTime(row["NgayTao"]);
                DateTime ngayCapNhat = row["NgayCapNhat"] is DBNull ? DateTime.MinValue : Convert.ToDateTime(row["NgayCapNhat"]);

                Nha n = new Nha()
                {
                    MaNha = maNha,
                    MaNguoiDung = maNguoiDung,
                    DiaChi = diaChi,
                    TongSoPhong = tongSoPhong,
                    // Sửa lỗi logic nhỏ: Gán đúng biến tongSoPhongHienTai
                    TongSoPhongHienTai = tongSoPhongHienTai,
                    GhiChu = ghiChu,
                    NgayTao = ngayTao,
                    NgayCapNhat = ngayCapNhat
                };
                ds.Add(n);
            }

            return ds;
        }
    }
}