using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;

namespace RoomManagementSystem.DataLayer
{
    // Lớp BienLai này đã được cập nhật để chứa tất cả các thông tin cần thiết
    public class BienLai
    {
        public string MaHoaDon { get; set; }
        public decimal TongTien { get; set; }
        public string MaPhong { get; set; }
        public string TenNha { get; set; }
        public string DanhSachNguoiThue { get; set; }
        public int SoNguoiHienTai { get; set; }
        public string TenDichVu { get; set; }
        public string DVT { get; set; }
        public decimal SoLuong { get; set; }
        public decimal DonGia { get; set; }
        public decimal ThanhTien { get; set; }
        public string DanhSachSDT { get; set; }
        public DateTime NgayThanhToan { get; set; }
        public string DiaChi { get; set; }
        public string TenTaiKhoan { get; set; }

        // BỔ SUNG 3 THUỘC TÍNH NÀY VÀO
        public DateTime? NgayLapHoaDon { get; set; }
        public DateTime? NgayGuiThongBao { get; set; }
        public DateTime? NgayHanThanhToan { get; set; }

        //add 2 properties
        public decimal SoTienDaThanhToan { get; set; }
        public decimal ConLai { get; set; }
    }

    public class BienLaiDAL
    {
        string connect = DbConfig.ConnectionString;

        public List<BienLai> GetBienLaiTheoMaPhong(string maPhong)
        {
            List<BienLai> lst = new List<BienLai>();
            using (SqlConnection con = new SqlConnection(connect))
            {
                // ✅ ĐÂY LÀ CÂU TRUY VẤN ĐÃ CHẠY THÀNH CÔNG TRONG SSMS
                string query = @"
    DECLARE @MaHoaDon VARCHAR(20);
    SELECT TOP 1 @MaHoaDon = MaHoaDon 
    FROM HoaDon 
    WHERE MaPhong = @MaPhong 
    ORDER BY NgayTao DESC;

    IF @MaHoaDon IS NOT NULL
    BEGIN
        SELECT 
            hd.MaHoaDon,
            hd.NgayTao AS NgayLapHoaDon,
            tbp.NgayGui AS NgayGuiThongBao,
            tt.NgayHanThanhToan,
            tt.SoTienDaThanhToan,
            tt.SoTienConLai,

            p.MaPhong,
            nha.DiaChi AS DiaChi,
            ngd.TenTaiKhoan AS TenChuNha,

            (SELECT STRING_AGG(nt.HoTen, ', ')
             FROM HopDong_NguoiThue h_nt
             JOIN NguoiThue nt ON h_nt.MaNguoiThue = nt.MaNguoiThue
             JOIN HopDong h2 ON h_nt.MaHopDong = h2.MaHopDong
             WHERE h2.MaPhong = p.MaPhong AND h_nt.TrangThaiThue = N'Đang ở'
            ) AS DanhSachNguoiThue,

            (SELECT STRING_AGG(nt.SoDienThoai, ', ')
             FROM HopDong_NguoiThue h_nt
             JOIN NguoiThue nt ON h_nt.MaNguoiThue = nt.MaNguoiThue
             JOIN HopDong h2 ON h_nt.MaHopDong = h2.MaHopDong
             WHERE h2.MaPhong = p.MaPhong AND h_nt.TrangThaiThue = N'Đang ở'
            ) AS DanhSachSDT,

            p.SoNguoiHienTai,

            (SELECT SUM(cthd_sum.ThanhTien) 
             FROM ChiTietHoaDon cthd_sum 
             WHERE cthd_sum.MaHoaDon = hd.MaHoaDon) AS TongTien,

            dv.TenDichVu,
            cthd.DVT,
            cthd.SoLuong,
            cthd.DonGia,
            cthd.ThanhTien

        FROM HoaDon hd
        LEFT JOIN Phong p ON hd.MaPhong = p.MaPhong
        LEFT JOIN Nha nha ON p.MaNha = nha.MaNha
        LEFT JOIN NguoiDung ngd ON nha.MaNguoiDung = ngd.MaNguoiDung      -- ⭐ Thêm JOIN lấy chủ nhà
        LEFT JOIN HopDong h ON p.MaPhong = h.MaPhong AND h.TrangThai = N'Hiệu lực'
        LEFT JOIN ChiTietHoaDon cthd ON hd.MaHoaDon = cthd.MaHoaDon
        LEFT JOIN DichVu dv ON cthd.MaDichVu = dv.MaDichVu
        LEFT JOIN ThanhToan tt ON hd.MaHoaDon = tt.MaHoaDon
        LEFT JOIN ThongBaoPhi tbp ON tt.MaThongBaoPhi = tbp.MaThongBaoPhi
        WHERE hd.MaHoaDon = @MaHoaDon;
    END";


                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@MaPhong", maPhong);

                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    // Phần đọc dữ liệu này đã được cập nhật để khớp với câu SELECT
                    lst.Add(new BienLai()
                    {
                        MaHoaDon = dr["MaHoaDon"]?.ToString(),
                        NgayLapHoaDon = dr["NgayLapHoaDon"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(dr["NgayLapHoaDon"]),
                        NgayGuiThongBao = dr["NgayGuiThongBao"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(dr["NgayGuiThongBao"]),
                        NgayHanThanhToan = dr["NgayHanThanhToan"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(dr["NgayHanThanhToan"]),
                        TongTien = dr["TongTien"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["TongTien"]),
                        SoTienDaThanhToan = dr["SoTienDaThanhToan"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["SoTienDaThanhToan"]),
                        ConLai = dr["SoTienConLai"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["SoTienConLai"]),

                        MaPhong = dr["MaPhong"]?.ToString(),
                    //    TenNha = dr["TenNha"]?.ToString(),      // <- Thêm TenNha
                        DiaChi = dr["DiaChi"]?.ToString(),
                        DanhSachNguoiThue = dr["DanhSachNguoiThue"]?.ToString(),
                        SoNguoiHienTai = dr["SoNguoiHienTai"] == DBNull.Value ? 0 : Convert.ToInt32(dr["SoNguoiHienTai"]),
                        DanhSachSDT = dr["DanhSachSDT"]?.ToString(),
                        TenTaiKhoan = dr["TenChuNha"]?.ToString(),
                     //   NgayThanhToan = dr["NgayThanhToan"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(dr["NgayThanhToan"]),

                        TenDichVu = dr["TenDichVu"]?.ToString(),
                        DVT = dr["DVT"]?.ToString(),
                        SoLuong = dr["SoLuong"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["SoLuong"]),
                        DonGia = dr["DonGia"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["DonGia"]),
                        ThanhTien = dr["ThanhTien"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["ThanhTien"])
                    });

                }
            }
            return lst;
        }
    }
}