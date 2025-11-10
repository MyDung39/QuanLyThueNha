using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoomManagementSystem.DataLayer
{
    public class LichSuHopDong
    {
        public int MaLichSu { get; set; }
        public string MaHopDong { get; set; }
        public DateTime NgayThayDoi { get; set; }
        public string MaNguoiThayDoi { get; set; }
        public string HanhDong { get; set; }
        public string NoiDungThayDoi { get; set; }

        // Thuộc tính phụ để JOIN lấy tên người dùng từ bảng NguoiDung
        public string TenNguoiThayDoi { get; set; }
    }
}