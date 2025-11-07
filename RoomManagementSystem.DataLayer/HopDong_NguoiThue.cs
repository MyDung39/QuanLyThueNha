using System;

namespace RoomManagementSystem.DataLayer
{
    public class HopDong_NguoiThue
    {
        public string? MaHopDong { get; set; }
        public string? MaNguoiThue { get; set; }
        public string? VaiTro { get; set; }
        public string? TrangThaiThue { get; set; }
        public DateTime? NgayDonVao { get; set; }
        public DateTime? NgayDonRa { get; set; }
        public DateTime? NgayBatDauThue { get; set; }
    }
}