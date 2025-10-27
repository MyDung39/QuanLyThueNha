using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RoomManagementSystem.BusinessLayer;
using RoomManagementSystem.DataLayer;

namespace Test
{
    class TestQLTaiSanPhong
    {
        static void Main(string[] args)
        {
            QL_TaiSan_Phong ql = new QL_TaiSan_Phong();
            Console.WriteLine("=== Quan Ly Nha va Phong===");

            // 1. Đăng ký thông tin nhà
            //Console.WriteLine("\n1.Dang ky thong tin nha...");
            //bool kqNha = ql.DangKyThongTinNha("NHA002", "123 Nguyễn Văn Cừ", 10, 0, "Nhà mới xây");
            //Console.WriteLine(kqNha ? "Success" : "Fail");

            // 2. Cập nhật thông tin nhà
            //Console.WriteLine("\n2. update thong tin nha...");
            //bool kqUpdateNha = ql.UpdateNha("NHA002", "456 Lý Thường Kiệt", 12, 0, "Đã sửa lại");
            //Console.WriteLine(kqUpdateNha ? "Success" : "Fail");

            // 3. Thêm phòng
            Console.WriteLine("\n3. Them phong...");
            Phong p1 = new Phong()
            {
                MaPhong = "PHONG005",
                MaNha = "NHA001",
                LoaiPhong = "Phòng đơn",
                DienTich = 25.5f,
                GiaThue = 3500000,
                TrangThai = "Trống",
                GhiChu = "Phòng mới"
            };
            bool kqPhong = ql.ThemPhong(p1);
            Console.WriteLine(kqPhong ? "Success_Result:" : "Fail");
            List<Phong> listPhong = ql.DanhSachPhong();
            foreach (var phong in listPhong)
            {
                Console.WriteLine($"{phong.MaPhong} - {phong.LoaiPhong} - {phong.GiaThue} - {phong.TrangThai}");
            }

            // 4. Xem danh sách phòng
            Console.WriteLine("\n4. List phong:");
            listPhong = ql.DanhSachPhong();
             foreach (var phong in listPhong)
            {
                Console.WriteLine($"{phong.MaPhong} - {phong.LoaiPhong} - {phong.GiaThue} - {phong.TrangThai}");
            }

            // 5. Cập nhật phòng (status,price)
            Console.WriteLine("\n5. Update room...");
            p1.TrangThai = "Đang thuê";
            p1.GiaThue = 400000;
            bool kqUpdatePhong = ql.CapNhatPhong(p1);
            Console.WriteLine(kqUpdatePhong ? "Success_Result: " : "Fail");
            listPhong = ql.DanhSachPhong();
            foreach (var phong in listPhong)
            {
                Console.WriteLine($"{phong.MaPhong} - {phong.LoaiPhong} - {phong.GiaThue} - {phong.TrangThai}");
            }
            Console.WriteLine("\n=== End Test, Check Database ===");
            Console.ReadLine();
        }
    }
}

