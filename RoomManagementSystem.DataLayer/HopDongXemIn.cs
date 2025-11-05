using System;
using System.Collections.Generic;

namespace RoomManagementSystem.DataLayer
{
    public class ThanhVienCungPhong
    {
        public string? HoTen { get; set; }
        public string? Cccd { get; set; }
    }
    public class HopDongXemIn
    {
        // Thông tin bảng NguoiThue
        public string? TenNguoiThue { get; set; }
        public string? CccdNguoiThue { get; set; }
        public DateTime NgayDonVao { get; set; }

        // Thông tin bảng Nha - Phong
        public string? DiaChiNha { get; set; }
        public int TongSoPhong { get; set; }
        public decimal GiaThue { get; set; }
        public decimal DienTich { get; set; }

        // Thông tin bảng HopDong
        public string? MaHopDong { get; set; }
        public string? MaPhong { get; set; }
        public DateTime NgayBatDau { get; set; }
        public DateTime NgayKetThuc { get; set; }
        public decimal TienCoc { get; set; }
        public string? FileDinhKem { get; set; }
        public int ThoiHan { get; set; }

        // Danh sách người ở cùng (cho Phụ lục)
        public List<ThanhVienCungPhong> ThanhVien { get; set; } = new List<ThanhVienCungPhong>();
    }
}