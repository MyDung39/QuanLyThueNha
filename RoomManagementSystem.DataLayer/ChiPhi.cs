using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoomManagementSystem.DataLayer
{
    public class BaoCaoChiPhi
    {
        public string MaPhong { get; set; }
        public decimal ChiPhiDien { get; set; }
        public decimal ChiPhiNuoc { get; set; }
        public decimal ChiPhiBaoTri { get; set; }

        public decimal Tong => ChiPhiDien + ChiPhiNuoc + ChiPhiBaoTri;
    }
    public class BaoCaoChiPhiDAL
    {
        private readonly string connectionString = DbConfig.ConnectionString;

        public DataTable GetChiPhiThang(int month, int year)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("STT", typeof(int));
            dt.Columns.Add("MaPhong", typeof(string)); // Cột Phòng
            dt.Columns.Add("LoaiChiPhi", typeof(string));
            dt.Columns.Add("SoTien", typeof(decimal));

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                // 1️⃣ Lấy danh sách Hóa đơn trong tháng
                string getHoaDon = @"SELECT MaHoaDon, MaPhong FROM HoaDon 
                             WHERE MONTH(CONVERT(datetime, '01/' + ThoiKy, 103)) = @Thang
                               AND YEAR(CONVERT(datetime, '01/' + ThoiKy, 103)) = @Nam";

                SqlCommand cmdHoaDon = new SqlCommand(getHoaDon, con);
                cmdHoaDon.Parameters.AddWithValue("@Thang", month);
                cmdHoaDon.Parameters.AddWithValue("@Nam", year);

                List<(string MaHoaDon, string MaPhong)> hoaDonList = new List<(string, string)>();
                using (SqlDataReader r = cmdHoaDon.ExecuteReader())
                {
                    while (r.Read())
                        hoaDonList.Add((r["MaHoaDon"].ToString(), r["MaPhong"].ToString()));
                }

                int stt = 1;

                // 2️⃣ Lấy chi tiết: CHỈ LẤY ĐIỆN (DV1) VÀ NƯỚC (DV2)
                foreach (var hd in hoaDonList)
                {
                    // ⚠️ LƯU Ý QUAN TRỌNG: Thêm điều kiện AND MaDichVu IN ('DV1', 'DV2')
                    string getChiTiet = @"SELECT MaDichVu, SoLuong, DonGia FROM ChiTietHoaDon
                                  WHERE MaHoaDon=@MaHoaDon 
                                  AND SoLuong > 0
                                  AND MaDichVu IN ('DV1', 'DV2')";

                    SqlCommand cmdCT = new SqlCommand(getChiTiet, con);
                    cmdCT.Parameters.AddWithValue("@MaHoaDon", hd.MaHoaDon);

                    using (SqlDataReader r = cmdCT.ExecuteReader())
                    {
                        while (r.Read())
                        {
                            string maDv = r["MaDichVu"].ToString();
                            string loai = maDv == "DV1" ? "Điện" : "Nước"; // Chỉ còn 2 trường hợp này

                            decimal soTien = Convert.ToDecimal(r["SoLuong"]) * Convert.ToDecimal(r["DonGia"]);

                            // Thêm vào bảng: STT | Mã Phòng | Loại | Số Tiền
                            dt.Rows.Add(stt++, hd.MaPhong, loai, soTien);
                        }
                    }
                }

                // 3️⃣ Lấy phí Bảo Trì (Giữ nguyên)
                string getBaoTri = @"SELECT MaPhong, MoTa, ChiPhi FROM BaoTri
                             WHERE MONTH(NgayYeuCau)=@Thang AND YEAR(NgayYeuCau)=@Nam
                               AND ChiPhi>0";
                SqlCommand cmdBT = new SqlCommand(getBaoTri, con);
                cmdBT.Parameters.AddWithValue("@Thang", month);
                cmdBT.Parameters.AddWithValue("@Nam", year);

                using (SqlDataReader r = cmdBT.ExecuteReader())
                {
                    while (r.Read())
                    {
                        string loai = "Bảo trì: " + r["MoTa"].ToString();
                        decimal soTien = Convert.ToDecimal(r["ChiPhi"]);
                        string maPhong = r["MaPhong"].ToString();

                        dt.Rows.Add(stt++, maPhong, loai, soTien);
                    }
                }
            }

            return dt;
        }
    }
}
