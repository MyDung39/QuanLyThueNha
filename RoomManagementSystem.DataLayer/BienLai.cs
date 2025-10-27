using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoomManagementSystem.DataLayer
{
    public class BienLai
    {
        public string HoTen { get; set; }
        public string Sdt { get; set; }
        public string MaPhong { get; set; }
        public string SoNguoiHienTai { get; set; }
        public string TenDichVu { get; set; }
        public decimal SoLuong { get; set; }
        public string DVT { get; set; }
        public decimal DonGia { get; set; }
        public decimal ThanhTien { get; set; }
    }
    public class BienLaiDAL
    {
        string connect = "Data Source=DESKTOP-4JTJGR2\\SQLEXPRESS;Initial Catalog=QLTN;Integrated Security=True;Encrypt=True;Trust Server Certificate=True";

        public List<BienLai> GetBienLai(string maThanhToan)
        {
            List<BienLai> lst = new List<BienLai>();
            using (SqlConnection con = new SqlConnection(connect))
            {
                string query = @"
                    SELECT 
                        nt.HoTen,
                        nt.SoDienThoai AS Sdt,
                        p.MaPhong,
                        p.SoNguoiHienTai,
                        dv.TenDichVu,
                        cthd.SoLuong,
                        dv.DVT,
                        cthd.DonGia,
                        cthd.ThanhTien
                    FROM ThanhToan tt
                    JOIN HoaDon hd ON tt.MaHoaDon = hd.MaHoaDon
                    JOIN ChiTietHoaDon cthd ON hd.MaHoaDon = cthd.MaHoaDon
                    JOIN DichVu dv ON cthd.MaDichVu = dv.MaDichVu
                    JOIN Phong p ON tt.MaPhong = p.MaPhong
                    JOIN NguoiThue nt ON p.MaPhong = nt.MaPhong
                    WHERE tt.MaThanhToan = @MaThanhToan";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@MaThanhToan", maThanhToan);

                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    lst.Add(new BienLai()
                    {
                        HoTen = dr["HoTen"].ToString(),
                        Sdt = dr["Sdt"].ToString(),
                        MaPhong = dr["MaPhong"].ToString(),
                        SoNguoiHienTai = dr["SoNguoiHienTai"].ToString(),
                        TenDichVu = dr["TenDichVu"].ToString(),
                        SoLuong = Convert.ToDecimal(dr["SoLuong"]),
                        DVT = dr["DVT"].ToString(),
                        DonGia = Convert.ToDecimal(dr["DonGia"]),
                        ThanhTien = Convert.ToDecimal(dr["ThanhTien"])
                    });
                }
            }
            return lst;
        }
    }
}
