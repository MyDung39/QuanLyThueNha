using System;
using RoomManagementSystem.BusinessLayer;
using RoomManagementSystem.DataLayer;
using System.Collections.Generic;

namespace RoomManagementSystem.App
{
    class Program
    {
        static void Main(string[] args)
        {
            QuanLyNguoiThue ql = new QuanLyNguoiThue();
            List<NguoiThue> ds;

            // Test thêm người thuê
            //NguoiThue nt1 = new NguoiThue
            //{
            //    MaNguoiThue = "NT004",
            //    MaPhong = "PHONG003",
            //    HoTen = "Nguyen Van A",
            //    Sdt = "0901234567",
            //    Email = "vana@example.com",
            //    SoGiayTo = "123456789",
            //    NgayBatDauThue = DateTime.Now,
            //    TrangThaiThue = "Đang ở",
            //    NgayDonVao = DateTime.Now,
            //    NgayDonRa = new DateTime(2026, 9, 20),
            //    NgayTao = DateTime.Now
            //};

            //bool kqThem = ql.ThemNguoiThue(nt1);
            //ds = ql.getAll(); 
            //foreach (var nt in ds)
            //{
            //    Console.WriteLine($"{nt.MaNguoiThue} - {nt.HoTen} - {nt.MaPhong} - {nt.TrangThaiThue}");
            //}
            //Console.WriteLine(kqThem ? "T" : "F");

            // Test cập nhật thông tin
            //nt1.TrangThaiThue = "Đã trả phòng";
            //nt1.NgayDonRa = DateTime.Now;
            //bool kqCapNhat = ql.CapNhatNguoiThue(nt1);
            //ds = ql.getAll(); 
            //foreach (var nt in ds)
            //{
            //    Console.WriteLine($"{nt.MaNguoiThue} - {nt.HoTen} - {nt.MaPhong} - {nt.TrangThaiThue}");
            //}
            //Console.WriteLine(kqCapNhat ? "T" : "F");

            // Test lấy tất cả người thuê
            Console.WriteLine("\n=== All_List ===");
            ds = ql.getAll();
            foreach (var nt in ds)
            {
                foreach (var prop in typeof(NguoiThue).GetProperties())
                {
                    Console.WriteLine($"{prop.Name}: {prop.GetValue(nt)}");
                }
                Console.WriteLine("---------------------------");
            }

            // Test lấy người thuê theo phòng
            Console.WriteLine("\n=== ListByPhong ===");
            List<NguoiThue> dsTheoPhong = ql.getByMaPhong(new NguoiThue { MaPhong = "PHONG001" });
            foreach (var nt in dsTheoPhong)
            {
                foreach (var prop in typeof(NguoiThue).GetProperties())
                {
                    Console.WriteLine($"{prop.Name}: {prop.GetValue(nt)}");
                }
                Console.WriteLine("---------------------------");
            }
        }
    }
}