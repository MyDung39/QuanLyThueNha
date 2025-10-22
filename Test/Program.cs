using RoomManagementSystem.BusinessLayer;
using RoomManagementSystem.DataLayer;
using System;
using System.Collections.Generic;
using System.Text;

namespace RoomManagementApp
{
    class Program
    {
        // Khởi tạo lớp Business Layer (BLL)
        static QL_TaiSan_Phong ql = new QL_TaiSan_Phong();

        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.InputEncoding = System.Text.Encoding.UTF8;

            while (true)
            {
                Console.Clear();
                Console.WriteLine("== QUẢN LÝ NHÀ VÀ PHÒNG ==");
                Console.WriteLine("1. Quản lý Nhà");
                Console.WriteLine("2. Quản lý Phòng");
                Console.WriteLine("0. Thoát");
                Console.Write("Chọn chức năng: ");
                string choice = Console.ReadLine() ?? "";

                switch (choice)
                {
                    case "1":
                        MenuNha();
                        break;
                    case "2":
                        MenuPhong();
                        break;
                    case "0":
                        Console.WriteLine("Đã thoát chương trình.");
                        return;
                    default:
                        Console.WriteLine("Lựa chọn không hợp lệ. Nhấn phím bất kỳ để thử lại.");
                        Console.ReadKey();
                        break;
                }
            }
        }
        static void MenuNha()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("== QUẢN LÝ NHÀ ==");
                Console.WriteLine("1. Đăng ký nhà mới");
                Console.WriteLine("2. Cập nhật thông tin nhà");
                Console.WriteLine("3. Xem danh sách nhà");
                Console.WriteLine("0. Quay lại menu chính");
                Console.Write("Chọn chức năng: ");
                string choice = Console.ReadLine() ?? "";

                switch (choice)
                {
                    case "1":
                        ThemNhaMoi();
                        break;
                    case "2":
                        CapNhatNha();
                        break;
                    case "3":
                        XemDanhSachNha();
                        break;
                    case "0":
                        return;
                    default:
                        Console.WriteLine("Lựa chọn không hợp lệ. Nhấn phím bất kỳ để thử lại.");
                        Console.ReadKey();
                        break;
                }
            }
        }
        static void ThemNhaMoi()
        {
            try
            {
                Console.WriteLine("-- ĐĂNG KÝ NHÀ MỚI --");
                Console.Write("Nhập Mã nhà (ví dụ: NHA01): ");
                string maNha = Console.ReadLine() ?? "";
                Console.Write("Nhập Địa chỉ: ");
                string diaChi = Console.ReadLine() ?? "";
                int tongSoPhong = ReadInt("Nhập Tổng số phòng: ");
                int tongSoPhongHienTai = ReadInt("Nhập Tổng số phòng hiện tại (đã có): ");
                Console.Write("Nhập Ghi chú (nếu có): ");
                string ghiChu = Console.ReadLine() ?? "";

                bool result = ql.DangKyThongTinNha(maNha, diaChi, tongSoPhong, tongSoPhongHienTai, ghiChu);

                if (result)
                {
                    Console.WriteLine("=> Đăng ký nhà mới thành công!");
                }
                else
                {
                    Console.WriteLine("=> Đăng ký nhà mới thất bại.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"=> LỖI: {ex.Message}");
            }
            Console.WriteLine("Nhấn phím bất kỳ để tiếp tục...");
            Console.ReadKey();
        }
        static void CapNhatNha()
        {
            try
            {
                Console.WriteLine("-- CẬP NHẬT THÔNG TIN NHÀ --");
                Console.Write("Nhập Mã nhà cần cập nhật: ");
                string maNha = Console.ReadLine() ?? "";

                Console.Write("Nhập Địa chỉ MỚI: ");
                string diaChi = Console.ReadLine() ?? "";
                int tongSoPhong = ReadInt("Nhập Tổng số phòng MỚI: ");
                int tongSoPhongHienTai = ReadInt("Nhập Tổng số phòng hiện tại MỚI: ");
                Console.Write("Nhập Ghi chú MỚI (nếu có): ");
                string ghiChu = Console.ReadLine() ?? "";

                bool result = ql.UpdateNha(maNha, diaChi, tongSoPhong, tongSoPhongHienTai, ghiChu);

                if (result)
                {
                    Console.WriteLine("=> Cập nhật nhà thành công!");
                }
                else
                {
                    Console.WriteLine("=> Cập nhật nhà thất bại (Không tìm thấy Mã nhà hoặc không có gì thay đổi).");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"=> LỖI: {ex.Message}");
            }
            Console.WriteLine("Nhấn phím bất kỳ để tiếp tục...");
            Console.ReadKey();
        }
        static void XemDanhSachNha()
        {
            try
            {
                Console.WriteLine("-- DANH SÁCH NHÀ --");
                List<Nha> dsNha = ql.DanhSachNha();

                if (dsNha == null || dsNha.Count == 0)
                {
                    Console.WriteLine("Không có phòng nào trong hệ thống.");
                }
                else
                {
                    foreach (var n in dsNha)
                    {
                        Console.WriteLine("---------------------------------");
                        Console.WriteLine($"Mã nhà: \t{n.MaNha}");
                        Console.WriteLine($"Mã người dùng: \t{n.MaNguoiDung}");
                        Console.WriteLine($"Địa chỉ: \t{n.DiaChi}");
                        Console.WriteLine($"Tổng số phòng: \t{n.TongSoPhong}");
                        Console.WriteLine($"Phòng hiện tại: \t{n.TongSoPhongHienTai}");
                        Console.WriteLine($"Ghi chú: \t{n.GhiChu}");
                        Console.WriteLine($"Ngày tạo: \t{n.NgayTao:dd/MM/yyyy}");
                    }
                    Console.WriteLine("---------------------------------");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"=> LỖI: {ex.Message}");
            }
            Console.WriteLine("Nhấn phím bất kỳ để tiếp tục...");
            Console.ReadKey();
        }
        static void MenuPhong()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("== QUẢN LÝ PHÒNG ==");
                Console.WriteLine("1. Xem danh sách phòng");
                Console.WriteLine("2. Thêm phòng mới");
                Console.WriteLine("3. Cập nhật thông tin phòng");
                Console.WriteLine("0. Quay lại menu chính");
                Console.Write("Chọn chức năng: ");
                string choice = Console.ReadLine() ?? "";

                switch (choice)
                {
                    case "1":
                        XemDanhSachPhong();
                        break;
                    case "2":
                        ThemPhongMoi();
                        break;
                    case "3":
                        CapNhatPhong();
                        break;
                    case "0":
                        return;
                    default:
                        Console.WriteLine("Lựa chọn không hợp lệ. Nhấn phím bất kỳ để thử lại.");
                        Console.ReadKey();
                        break;
                }
            }
        }
        static void XemDanhSachPhong()
        {
            try
            {
                Console.WriteLine("-- DANH SÁCH PHÒNG --");
                List<Phong> dsPhong = ql.DanhSachPhong();

                if (dsPhong == null || dsPhong.Count == 0)
                {
                    Console.WriteLine("Không có phòng nào trong hệ thống.");
                }
                else
                {
                    foreach (var p in dsPhong)
                    {
                        Console.WriteLine("---------------------------------");
                        Console.WriteLine($"Mã phòng: \t{p.MaPhong}");
                        Console.WriteLine($"Thuộc nhà: \t{p.MaNha}");
                        Console.WriteLine($"Loại phòng: \t{p.LoaiPhong}");
                        Console.WriteLine($"Trạng thái: \t{p.TrangThai}");
                        Console.WriteLine($"Diện tích: \t{p.DienTich} m2");
                        Console.WriteLine($"Giá thuê: \t{p.GiaThue:N0} VND");
                        Console.WriteLine($"Số người HT: \t{p.SoNguoiHienTai}");
                    }
                    Console.WriteLine("---------------------------------");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"=> LỖI: {ex.Message}");
            }
            Console.WriteLine("Nhấn phím bất kỳ để tiếp tục...");
            Console.ReadKey();
        }
        static void ThemPhongMoi()
        {
            try
            {
                Console.WriteLine("-- THÊM PHÒNG MỚI --");
                Phong p = new Phong();

                Console.Write("Nhập Mã phòng (ví dụ: PHONG001): ");
                p.MaPhong = Console.ReadLine() ?? "";
                Console.Write("Nhập Mã nhà (phải tồn tại trong CSDL, ví dụ: NHA001): ");
                p.MaNha = Console.ReadLine() ?? "";
                Console.Write("Nhập Loại phòng (Phòng trống, Phòng có đồ cơ bản): ");
                p.LoaiPhong = Console.ReadLine() ?? "";
                p.DienTich = ReadFloat("Nhập Diện tích (m2): ");
                p.GiaThue = ReadFloat("Nhập Giá thuê (VND): ");
                Console.Write("Nhập Trạng thái (ví dụ: Trống, Đang thuê, Dự kiến, Bảo trì): ");
                p.TrangThai = Console.ReadLine() ?? "";
                Console.Write("Nhập Ghi chú (nếu có): ");
                p.GhiChu = Console.ReadLine() ?? "";

                bool result = ql.ThemPhong(p);

                if (result)
                {
                    Console.WriteLine("=> Thêm phòng mới thành công!");
                }
                else
                {
                    Console.WriteLine("=> Thêm phòng mới thất bại.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"=> LỖI: {ex.Message} (Kiểm tra lại Mã nhà có tồn tại không?)");
            }
            Console.WriteLine("Nhấn phím bất kỳ để tiếp tục...");
            Console.ReadKey();
        }
        static void CapNhatPhong()
        {
            try
            {
                Console.WriteLine("-- CẬP NHẬT THÔNG TIN PHÒNG --");
                Phong p = new Phong();

                Console.Write("Nhập Mã phòng cần cập nhật: ");
                p.MaPhong = Console.ReadLine() ?? "";
                Console.Write("Nhập Loại phòng MỚI: ");
                p.LoaiPhong = Console.ReadLine() ?? "";
                p.DienTich = ReadFloat("Nhập Diện tích MỚI (m2): ");
                p.GiaThue = ReadFloat("Nhập Giá thuê MỚI (VND): ");
                Console.Write("Nhập Trạng thái MỚI: ");
                p.TrangThai = Console.ReadLine() ?? "";
                Console.Write("Nhập Ghi chú MỚI (nếu có): ");
                p.GhiChu = Console.ReadLine() ?? "";

                bool result = ql.CapNhatPhong(p);

                if (result)
                {
                    Console.WriteLine("=> Cập nhật phòng thành công!");
                }
                else
                {
                    Console.WriteLine("=> Cập nhật phòng thất bại (Không tìm thấy Mã phòng).");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"=> LỖI: {ex.Message}");
            }
            Console.WriteLine("Nhấn phím bất kỳ để tiếp tục...");
            Console.ReadKey();
        }
        // Hàm hỗ trợ đọc số nguyên (int) an toàn
        static int ReadInt(string prompt)
        {
            int value;
            while (true)
            {
                Console.Write(prompt);
                if (int.TryParse(Console.ReadLine(), out value))
                {
                    return value;
                }
                else
                {
                    Console.WriteLine("Giá trị không hợp lệ. Vui lòng nhập lại một số nguyên.");
                }
            }
        }
        // Hàm hỗ trợ đọc số thực (float) an toàn
        static float ReadFloat(string prompt)
        {
            float value;
            while (true)
            {
                Console.Write(prompt);
                if (float.TryParse(Console.ReadLine(), out value))
                {
                    return value;
                }
                else
                {
                    Console.WriteLine("Giá trị không hợp lệ. Vui lòng nhập lại một số thực.");
                }
            }
        }

    }
}
