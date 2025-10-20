using System;

namespace RoomManagementSystem.DataLayer
{
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
        public DateTime NgayBatDau { get; set; }
        public DateTime NgayKetThuc { get; set; }
        public decimal TienCoc { get; set; }
        public string? FileDinhKem { get; set; }
        public int ThoiHan { get; set; }
    }
}