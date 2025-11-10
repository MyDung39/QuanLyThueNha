using System;

namespace RoomManagementSystem.DataLayer
{
    public class ThongBaoHan
    {
        public string MaThongBao { get; set; }
        public string MaHopDong { get; set; }
        public string NoiDung { get; set; }
        public DateTime NgayThongBao { get; set; }
        public string TrangThai { get; set; }
        public DateTime NgayTao { get; set; }
    }
}