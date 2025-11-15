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
        public decimal ChiPhiInternet { get; set; }
        public decimal ChiPhiRac { get; set; }
        public decimal ChiPhiBaoTri { get; set; }

        public decimal Tong => ChiPhiDien + ChiPhiNuoc + ChiPhiInternet + ChiPhiRac + ChiPhiBaoTri;
    }
    public class BaoCaoChiPhiDAL
    {
        private readonly string connectionString = DbConfig.ConnectionString; // Đảm bảo lớp DbConfig của bạn đúng



        public DataTable GetChiPhiThang(string thoiKy) // thoiKy có định dạng "MM/yyyy"
        {
            DataTable dt = new DataTable();

            // Phân tích chuỗi "MM/yyyy" thành tháng và năm để truy vấn an toàn
            if (!DateTime.TryParseExact(thoiKy, "MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime date))
            {
                return dt; // Trả về bảng trống nếu định dạng sai
            }
            int thang = date.Month;
            int nam = date.Year;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                // ✅ CÂU TRUY VẤN ĐÚNG: Chỉ lấy từ BaoTri, trả về dạng "dài"
                string query = @"
                    SELECT
                        ROW_NUMBER() OVER(ORDER BY NgayYeuCau) AS STT,
                        N'Bảo trì: ' + MoTa AS LoaiChiPhi, -- Tên cột phải là 'LoaiChiPhi'
                        ChiPhi AS SoTien                 -- Tên cột phải là 'SoTien'
                    FROM
                        BaoTri
                    WHERE
                        MONTH(NgayYeuCau) = @Thang AND YEAR(NgayYeuCau) = @Nam
                        AND ChiPhi > 0;
                ";

                // Sử dụng đúng tham số @Thang và @Nam
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@Thang", thang);
                cmd.Parameters.AddWithValue("@Nam", nam);

                try
                {
                    con.Open();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                }
                catch (Exception ex)
                {
                    // Luôn ghi log lỗi để dễ dàng debug
                    Console.WriteLine("Lỗi trong BaoCaoChiPhiDAL: " + ex.Message);
                }
            }

            return dt;
        }




        public DataTable GetChiPhiThang(int month, int year)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("STT", typeof(int));
            dt.Columns.Add("LoaiChiPhi", typeof(string));
            dt.Columns.Add("SoTien", typeof(decimal));

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                // 1️⃣ Lấy tất cả hóa đơn trong tháng
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

                // 2️⃣ Lấy ChiTietHoaDon cho từng hóa đơn
                foreach (var hd in hoaDonList)
                {
                    string getChiTiet = @"SELECT MaDichVu, SoLuong, DonGia FROM ChiTietHoaDon
                                  WHERE MaHoaDon=@MaHoaDon AND SoLuong>0";
                    SqlCommand cmdCT = new SqlCommand(getChiTiet, con);
                    cmdCT.Parameters.AddWithValue("@MaHoaDon", hd.MaHoaDon);

                    using (SqlDataReader r = cmdCT.ExecuteReader())
                    {
                        while (r.Read())
                        {
                            string loai = r["MaDichVu"].ToString() switch
                            {
                                "DV1" => "Điện",
                                "DV2" => "Nước",
                                "DV3" => "Internet",
                                "DV4" => "Rác",
                                "DV5" => "Gửi xe",
                                _ => r["MaDichVu"].ToString()
                            };
                            decimal soTien = Convert.ToDecimal(r["SoLuong"]) * Convert.ToDecimal(r["DonGia"]);
                            dt.Rows.Add(stt++, loai, soTien);
                        }
                    }
                }

                // 3️⃣ Lấy phí bảo trì
                string getBaoTri = @"SELECT MoTa, ChiPhi FROM BaoTri
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
                        dt.Rows.Add(stt++, loai, soTien);
                    }
                }

                // 4️⃣ (Tuỳ chọn) thêm trễ hạn từ ThanhToan nếu muốn
            }

            return dt;
        }





    }
}
