using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoomManagementSystem.DataLayer
{
    public class NguoiThue
    {
        public string? MaNguoiThue { get; set; }
        public string? MaPhong { get; set; }
        public string? HoTen { get; set; }
        public string? Sdt { get; set; }
        public string? Email { get; set; }
        public string? SoGiayTo { get; set; }
        public DateTime NgayBatDauThue { get; set; }
        public string? TrangThaiThue { get; set; }
        public DateTime? NgayDonVao { get; set; }
        public DateTime? NgayDonRa { get; set; }
        public string? VaiTro { get; set; }
        public DateTime NgayTao { get; set; }
        public DateTime NgayCapNhat { get; set; }
    }
}
