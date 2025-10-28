using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoomManagementSystem.DataLayer
{
    public class Phong
    {
        public string MaPhong { get; set; }
        public string MaNha { get; set; }
        public string LoaiPhong { get; set; }
        public float DienTich { get; set; }
        public float GiaThue { get; set; }
        public string TrangThai { get; set; }
        public int SoNguoiHienTai { get; set; }
        public string GhiChu { get; set; }
        public DateTime NgayTao { get; set; }
        public DateTime NgayCapNhat { get; set; }
    }
}
