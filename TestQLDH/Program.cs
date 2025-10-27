using System;
using RoomManagementSystem.BusinessLayer;
using RoomManagementSystem.DataLayer;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using GrapeCity.Documents.Word;
using GrapeCity.Documents.Word.Layout;
using Spire.Doc;
using Spire.Doc.Documents;
using System.Diagnostics;
using System.IO;

namespace RoomManagementSystem.App
{
    class Program
    {
        static void Main(string[] args)
        {
            QL_HopDong qlHD = new QL_HopDong();
            Console.WriteLine("\n=== Quan Ly Hop Dong ===");

            //// 1. Thêm hợp đồng
            //Console.WriteLine("\n1. Them hop dong...");
            //HopDong hd1 = new HopDong()
            //{
            //    MaHopDong = "HD003", // Đã được tạo và có sẵn
            //    MaPhong = "PHONG002", // Giả định phòng tồn tại từ test trước
            //    MaNguoiThue = "NT003", // Giả định người thuê tồn tại
            //    ChuNha = "ND001",
            //    TienCoc = 2000000,
            //    NgayBatDau = new DateTime(2025, 10, 1),
            //    NgayKetThuc = new DateTime(2025, 11, 1), // Sắp hết hạn (trong 30 ngày từ 12/10/2025)
            //    TrangThai = "Hiệu lực",
            //    GhiChu = "Hợp đồng mới"
            //};
            //bool kqHD = qlHD.ThemHopDong(hd1);
            //Console.WriteLine(kqHD ? "Success" : "Fail");

            // 2. Xem danh sách hợp đồng
            Console.WriteLine("\n2. List hop dong:");
            List<HopDong> listHD = qlHD.DanhSachHopDong();
            foreach (var hd in listHD)
            {
                Console.WriteLine($"{hd.MaHopDong} - Phòng: {hd.MaPhong} - Người thuê: {hd.MaNguoiThue} - Trạng thái: {hd.TrangThai} - Kết thúc: {hd.NgayKetThuc.ToShortDateString()}");
            }

            //// 3. Cập nhật hợp đồng (thay đổi trạng thái, ngày kết thúc)
            //Console.WriteLine("\n3. Update hop dong...");
            //hd1.TrangThai = "Hết hạn";
            //bool kqUpdateHD = qlHD.CapNhatHopDong(hd1);
            //Console.WriteLine(kqUpdateHD ? "Success" : "Fail");

            //// Xem lại danh sách sau update
            //listHD = qlHD.DanhSachHopDong();
            //foreach (var hd in listHD)
            //{
            //    Console.WriteLine($"{hd.MaHopDong} - Phòng: {hd.MaPhong} - Người thuê: {hd.MaNguoiThue} - Trạng thái: {hd.TrangThai} - Kết thúc: {hd.NgayKetThuc.ToShortDateString()}");
            //}

            // 4. Tạo thông báo hết hạn (nếu sắp hết)
            Console.WriteLine("\n4. Tao thong bao het han...");
            bool kqThongBao = qlHD.TaoThongBaoHetHan("HD002");
            Console.WriteLine(kqThongBao ? "Thong bao da tao" : "Khong can thong bao (chua sap het han)");

            //// 5. Xóa hợp đồng (comment nếu không muốn xóa thực tế)
            //Console.WriteLine("\n5. Xoa hop dong...");
            //bool kqDelete = qlHD.XoaHopDong("HD003");
            //Console.WriteLine(kqDelete ? "Success" : "Fail");


            // 6. Xuất hợp đồng ra file PDF
            // Đường dẫn template file ở folder ...\TestQLDH\bin\Debug\net8.0\Templates\mau-hop-dong-thue-nha-o.docx
            Console.WriteLine("\n3. Xuat hop dong ra file PDF...");

            // Tạo đường dẫn đầu ra động, lưu file trên Desktop của người dùng
            string maHopDongCanXuat = "HD002";
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string outputFileName = $"HopDong_{maHopDongCanXuat}_Output.pdf";
            string outputPath = Path.Combine(desktopPath, outputFileName);

            bool kqXuatFile = qlHD.XuatHopDongRaPdf(maHopDongCanXuat, outputPath);

            if (kqXuatFile)
            {
                Console.WriteLine($"Thanh cong! File da duoc luu tai: {outputPath}");

                try
                {
                    // Sử dụng "explorer.exe" để đảm bảo hoạt động nhất quán trên Windows
                    Process.Start("explorer.exe", desktopPath);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Khong the mo thu muc: {ex.Message}");
                }
            }

            else
            {
                Console.WriteLine("That bai! Khong the xuat file hop dong.");
            }

            Console.WriteLine("\n=== End Test Hop Dong, Check Database ===");
            Console.ReadLine();
        }
    }
}