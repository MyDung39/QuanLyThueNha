using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoomManagementSystem.DataLayer
{
    public class BaoTri
    {
        public string? MaBaoTri { get; set; }
        public string? MaPhong { get; set; }
        public string? MaNguoiThue { get; set; }
        public string? TenNguoiThue { get; set; }
        public string? NguonYeuCau { get; set; }
        public string? MoTa { get; set; }
        public string? TrangThaiXuLy { get; set; }
        public DateTime NgayYeuCau { get; set; }
        public DateTime? NgayHoanThanh { get; set; }
        public decimal ChiPhi { get; set; }
        public DateTime NgayTao { get; set; }
        public DateTime NgayCapNhat { get; set; }
    }
}
