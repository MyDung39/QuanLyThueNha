using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoomManagementSystem.DataLayer
{
    public class HopDong
    {
        public string? MaHopDong { get; set; }
        public string? MaPhong { get; set; }
        public string? MaNguoiThue { get; set; }
        public string? ChuNha { get; set; }
        public decimal TienCoc { get; set; }
        public DateTime NgayBatDau { get; set; }
        public int ThoiHan { get; set; }
        public DateTime NgayKetThuc { get; set; }
        public string? FileDinhKem { get; set; }
        public string? TrangThai { get; set; }
        public string? GhiChu { get; set; }
        public DateTime NgayTao { get; set; }
        public DateTime NgayCapNhat { get; set; }
    }
}
