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
        //public float DienTich { get; set; }
        public decimal DienTich { get; set; }
        //public float GiaThue { get; set; }
        public decimal GiaThue { get; set; }
        public string TrangThai { get; set; }
        public int SoNguoiHienTai { get; set; }
        public string GhiChu { get; set; }
        public DateTime NgayTao { get; set; }
        public DateTime NgayCapNhat { get; set; }

        // ✅ THÊM THUỘC TÍNH NÀY:
        // Dùng để lưu ngày bảo trì đang chờ xử lý.
        // Tên "HienTai" để phân biệt với các lần bảo trì trong quá khứ.
        public DateTime? NgayBaoTriHienTai { get; set; }

        public DateTime? NgayCoSan { get; set; }
    }
}
