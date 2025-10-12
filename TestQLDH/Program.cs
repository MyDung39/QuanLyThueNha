using System;
using RoomManagementSystem.BusinessLayer;
using RoomManagementSystem.DataLayer;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;

namespace RoomManagementSystem.App
{
    class Program
    {
        static void Main(string[] args)
        {
            QL_HopDong qlHD = new QL_HopDong();
            Console.WriteLine("\n=== Quan Ly Hop Dong ===");

            // 1. Thêm hợp đồng
            Console.WriteLine("\n1. Them hop dong...");
            HopDong hd1 = new HopDong()
            {
                MaHopDong = "HD003", // Đã được tạo và có sẵn
                MaPhong = "PHONG002", // Giả định phòng tồn tại từ test trước
                MaNguoiThue = "NT003", // Giả định người thuê tồn tại
                ChuNha = "ND001",
                TienCoc = 2000000f,
                NgayBatDau = new DateTime(2025, 10, 1),
                NgayKetThuc = new DateTime(2025, 11, 1), // Sắp hết hạn (trong 30 ngày từ 12/10/2025)
                FileDinhKem = "hopdong_hd003.pdf",
                TrangThai = "Hiệu lực",
                GhiChu = "Hợp đồng mới"
            };
            bool kqHD = qlHD.ThemHopDong(hd1);
            Console.WriteLine(kqHD ? "Success" : "Fail");

            // 2. Xem danh sách hợp đồng
            Console.WriteLine("\n2. List hop dong:");
            List<HopDong> listHD = qlHD.DanhSachHopDong();
            foreach (var hd in listHD)
            {
                Console.WriteLine($"{hd.MaHopDong} - Phòng: {hd.MaPhong} - Người thuê: {hd.MaNguoiThue} - Trạng thái: {hd.TrangThai} - Kết thúc: {hd.NgayKetThuc.ToShortDateString()}");
            }

            // 3. Cập nhật hợp đồng (thay đổi trạng thái, ngày kết thúc)
            Console.WriteLine("\n3. Update hop dong...");
            hd1.TrangThai = "Hết hạn";
            hd1.NgayKetThuc = new DateTime(2025, 12, 1);
            bool kqUpdateHD = qlHD.CapNhatHopDong(hd1);
            Console.WriteLine(kqUpdateHD ? "Success" : "Fail");

            // Xem lại danh sách sau update
            listHD = qlHD.DanhSachHopDong();
            foreach (var hd in listHD)
            {
                Console.WriteLine($"{hd.MaHopDong} - Phòng: {hd.MaPhong} - Người thuê: {hd.MaNguoiThue} - Trạng thái: {hd.TrangThai} - Kết thúc: {hd.NgayKetThuc.ToShortDateString()}");
            }

            // 4. Tạo thông báo hết hạn (nếu sắp hết)
            Console.WriteLine("\n4. Tao thong bao het han...");
            bool kqThongBao = qlHD.TaoThongBaoHetHan("HD003");
            Console.WriteLine(kqThongBao ? "Thong bao da tao" : "Khong can thong bao (chua sap het han)");

            // 5. Xóa hợp đồng (comment nếu không muốn xóa thực tế)
            // Console.WriteLine("\n5. Xoa hop dong...");
            // bool kqDelete = qlHD.XoaHopDong("HD001");
            // Console.WriteLine(kqDelete ? "Success" : "Fail");

            Console.WriteLine("\n=== End Test Hop Dong, Check Database ===");
            Console.ReadLine();
        }
    }
}