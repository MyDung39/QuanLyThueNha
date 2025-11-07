using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;

namespace RoomManagementSystem.DataLayer
{
    public class BienLai
    {
        public string HoTenChuNha { get; set; }
        public string MaPhong { get; set; }
        public string DiaChi { get; set; }
        public string DanhSachKhach { get; set; }
        public string DanhSachSDT { get; set; }
        public string SoNguoiHienTai { get; set; }
        public string TenDichVu { get; set; }
        public decimal SoLuong { get; set; }
        public string DVT { get; set; }
        public decimal DonGia { get; set; }
        public decimal ThanhTien { get; set; }
        public DateTime NgayThanhToan { get; set; }
    }

    public class BienLaiDAL
    {
        string connect = "Data Source=DESKTOP-4JTJGR2\\SQLEXPRESS;Initial Catalog=QLTN;Integrated Security=True;Encrypt=True;Trust Server Certificate=True";

        public List<BienLai> GetBienLaiByPhong(string maPhong)
        {
            List<BienLai> lst = new List<BienLai>();
            using (SqlConnection con = new SqlConnection(connect))
            {
                // ✅ ĐÃ XÓA CÁC CỘT KHÔNG TỒN TẠI
                string query = @"
    SELECT 
        nd.TenTaiKhoan AS HoTenChuNha,
        p.MaPhong,
        n.DiaChi,

        (SELECT STRING_AGG(nt.HoTen, ', ') 
         FROM HopDong_NguoiThue hdnt
         JOIN NguoiThue nt ON hdnt.MaNguoiThue = nt.MaNguoiThue
         WHERE hdnt.MaHopDong = hop.MaHopDong) AS DanhSachKhach,

        (SELECT STRING_AGG(nt.SoDienThoai, ', ') 
         FROM HopDong_NguoiThue hdnt
         JOIN NguoiThue nt ON hdnt.MaNguoiThue = nt.MaNguoiThue
         WHERE hdnt.MaHopDong = hop.MaHopDong) AS DanhSachSDT,

        p.SoNguoiHienTai,
        dv.TenDichVu,
        cthd.SoLuong,
        dv.DVT,
        cthd.DonGia,
        cthd.ThanhTien,
        tt.NgayTao AS NgayThanhToan
    FROM HoaDon hd
    JOIN ThanhToan tt ON tt.MaHoaDon = hd.MaHoaDon
    JOIN ChiTietHoaDon cthd ON hd.MaHoaDon = cthd.MaHoaDon
    JOIN DichVu dv ON cthd.MaDichVu = dv.MaDichVu
    JOIN Phong p ON hd.MaPhong = p.MaPhong
    JOIN HopDong hop ON hop.MaPhong = p.MaPhong
    JOIN Nha n ON p.MaNha = n.MaNha
    JOIN NguoiDung nd ON n.MaNguoiDung = nd.MaNguoiDung
    WHERE p.MaPhong = @MaPhong";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@MaPhong", maPhong);

                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    lst.Add(new BienLai()
                    {
                        HoTenChuNha = dr["HoTenChuNha"]?.ToString(),
                        MaPhong = dr["MaPhong"]?.ToString(),
                        DiaChi = dr["DiaChi"]?.ToString(),
                        DanhSachKhach = dr["DanhSachKhach"]?.ToString(),
                        DanhSachSDT = dr["DanhSachSDT"]?.ToString(),
                        SoNguoiHienTai = dr["SoNguoiHienTai"]?.ToString(),
                        TenDichVu = dr["TenDichVu"]?.ToString(),
                        SoLuong = dr["SoLuong"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["SoLuong"]),
                        DVT = dr["DVT"]?.ToString(),
                        DonGia = dr["DonGia"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["DonGia"]),
                        ThanhTien = dr["ThanhTien"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["ThanhTien"]),
                        NgayThanhToan = dr["NgayThanhToan"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(dr["NgayThanhToan"])
                    });
                }
            }
            return lst;
        }
    }
}