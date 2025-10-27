using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoomManagementSystem.DataLayer
{
    public class ThanhToan
    {
        public string? MaThanhToan { get; set; }
        public string? MaPhong { get; set; }
        public string? MaHoaDon { get; set; }
        public string? MaHopDong { get; set; }
        public string? MaThongBaoPhi { get; set; }
        public decimal TongCongNo { get; set; }
        public DateTime? NgayHanThanhToan { get; set; }
        public string? PhuongThucThanhToan { get; set; }
        public string? TrangThai { get; set; }
        public DateTime NgayTao { get; set; }
        public DateTime NgayCapNhat { get; set; }
        public decimal SoTienDaThanhToan { get; set; }
        public decimal SoTienConLai => TongCongNo - SoTienDaThanhToan;
    }

}
