using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoomManagementSystem.DataLayer
{
    public class Nha
    {
        public string MaNha { get; set; }
        public string MaNguoiDung { get; set; }
        public string DiaChi { get; set; }
        public int TongSoPhong { get; set; }
        public int TongSoPhongHienTai { get; set; }
        public string GhiChu { get; set; }
        public DateTime NgayTao { get; set; }
        public DateTime NgayCapNhat { get; set; }

    }
}
