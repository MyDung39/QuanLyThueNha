using RoomManagementSystem.BusinessLayer;
using RoomManagementSystem.DataLayer;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace RoomManagementApp
{
    class Program
    {
        // Khởi tạo các lớp Business Layer (BLL)
        static QuanLyNguoiThue ql_NguoiThue = new QuanLyNguoiThue();

        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.InputEncoding = System.Text.Encoding.UTF8;

            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== QUẢN LÝ NGƯỜI THUÊ ===");
                Console.WriteLine("1. Thêm người thuê mới");
                Console.WriteLine("2. Cập nhật thông tin người thuê");
                Console.WriteLine("3. Xem danh sách tất cả người thuê");
                Console.WriteLine("4. Tìm người thuê theo Mã phòng");
                Console.WriteLine("0. Thoát");
                Console.Write("Chọn chức năng: ");
                string choice = Console.ReadLine() ?? "";

                switch (choice)
                {
                    case "1":
                        ThemNguoiThueMoi();
                        break;
                    case "2":
                        CapNhatNguoiThue();
                        break;
                    case "3":
                        XemTatCaNguoiThue();
                        break;
                    case "4":
                        TimNguoiThueTheoPhong();
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

        static void ThemNguoiThueMoi()
        {
            try
            {
                Console.WriteLine("-- THÊM NGƯỜI THUÊ MỚI --");
                NguoiThue nt = new NguoiThue();

                Console.Write("Nhập Mã phòng (ví dụ: PHONG001): ");
                nt.MaPhong = Console.ReadLine() ?? "";
                Console.Write("Nhập Họ và tên: ");
                nt.HoTen = Console.ReadLine() ?? "";
                Console.Write("Nhập Số điện thoại: ");
                nt.Sdt = Console.ReadLine() ?? "";
                Console.Write("Nhập Email: ");
                nt.Email = Console.ReadLine() ?? "";
                Console.Write("Nhập Số giấy tờ (CCCD/CMND): ");
                nt.SoGiayTo = Console.ReadLine() ?? "";
                nt.NgayBatDauThue = ReadDate("Nhập Ngày bắt đầu thuê (dd/MM/yyyy): ");
                Console.Write("Nhập Trạng thái thuê (Đang ở, Đã trả phòng, Lịch hẹn trả Bỏ trống để mặc định 'Đang ở'): ");
                nt.TrangThaiThue = Console.ReadLine() ?? "";
                nt.NgayDonVao = ReadDateNullable("Nhập Ngày dọn vào (dd/MM/yyyy - Bỏ trống nếu chưa dọn vào): ");

                // VaiTro sẽ được BLL gán tự động, không cần nhập ở đây
                // NgayDonRa và NgayTao sẽ được xử lý ở BLL/DAL

                bool result = ql_NguoiThue.ThemNguoiThue(nt);

                if (result)
                {
                    Console.WriteLine("=> Thêm người thuê mới thành công!");
                }
                else
                {
                    Console.WriteLine("=> Thêm người thuê mới thất bại.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"=> LỖI: {ex.Message}");
            }
            Console.WriteLine("Nhấn phím bất kỳ để tiếp tục...");
            Console.ReadKey();
        }

        static void CapNhatNguoiThue()
        {
            try
            {
                Console.WriteLine("-- CẬP NHẬT THÔNG TIN NGƯỜI THUÊ --");
                NguoiThue nt = new NguoiThue();

                Console.Write("Nhập Mã người thuê cần cập nhật (ví dụ: NT001): ");
                nt.MaNguoiThue = Console.ReadLine() ?? "";
                nt.NgayTao = ReadDate("Nhập lại Ngày tạo (dd/MM/yyyy - bắt buộc): ");

                Console.WriteLine("--- NHẬP THÔNG TIN MỚI ---");
                Console.Write("Nhập Mã phòng MỚI: ");
                nt.MaPhong = Console.ReadLine() ?? "";
                Console.Write("Nhập Họ và tên MỚI: ");
                nt.HoTen = Console.ReadLine() ?? "";
                Console.Write("Nhập Số điện thoại MỚI: ");
                nt.Sdt = Console.ReadLine() ?? "";
                Console.Write("Nhập Email MỚI: ");
                nt.Email = Console.ReadLine() ?? "";
                Console.Write("Nhập Số giấy tờ MỚI: ");
                nt.SoGiayTo = Console.ReadLine() ?? "";
                Console.Write("Nhập Vai trò MỚI (Chủ hợp đồng / Người ở cùng): ");
                nt.VaiTro = Console.ReadLine() ?? "";
                nt.NgayBatDauThue = ReadDate("Nhập Ngày bắt đầu thuê MỚI (dd/MM/yyyy): ");
                Console.Write("Nhập Trạng thái thuê MỚI (Đang ở, Đã trả phòng,...): ");
                nt.TrangThaiThue = Console.ReadLine() ?? "";
                nt.NgayDonVao = ReadDateNullable("Nhập Ngày dọn vào MỚI (dd/MM/yyyy): ");
                nt.NgayDonRa = ReadDateNullable("Nhập Ngày dọn ra (dd/MM/yyyy - Bỏ trống nếu chưa dọn ra): ");


                bool result = ql_NguoiThue.CapNhatNguoiThue(nt);

                if (result)
                {
                    Console.WriteLine("=> Cập nhật thông tin người thuê thành công!");
                }
                else
                {
                    Console.WriteLine("=> Cập nhật thất bại (Kiểm tra lại Mã người thuê).");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"=> LỖI: {ex.Message}");
            }
            Console.WriteLine("Nhấn phím bất kỳ để tiếp tục...");
            Console.ReadKey();
        }

        static void XemTatCaNguoiThue()
        {
            try
            {
                Console.WriteLine("-- DANH SÁCH TẤT CẢ NGƯỜI THUÊ --");
                List<NguoiThue> ds = ql_NguoiThue.getAll();

                if (ds == null || ds.Count == 0)
                {
                    Console.WriteLine("Không có người thuê nào trong hệ thống.");
                }
                else
                {
                    foreach (var nt in ds)
                    {
                        Console.WriteLine("---------------------------------");
                        Console.WriteLine($"Mã NT: \t\t{nt.MaNguoiThue}");
                        Console.WriteLine($"Mã phòng: \t{nt.MaPhong}");
                        Console.WriteLine($"Họ tên: \t{nt.HoTen}");
                        Console.WriteLine($"SĐT: \t\t{nt.Sdt}");
                        Console.WriteLine($"Email: \t\t{nt.Email}");
                        Console.WriteLine($"Số giấy tờ: \t{nt.SoGiayTo}");
                        Console.WriteLine($"Vai trò: \t{nt.VaiTro}");
                        Console.WriteLine($"Trạng thái: \t{nt.TrangThaiThue}");
                        Console.WriteLine($"Ngày BĐ thuê: \t{nt.NgayBatDauThue:dd/MM/yyyy}");
                        Console.WriteLine($"Ngày dọn vào: \t{nt.NgayDonVao?.ToString("dd/MM/yyyy") ?? "(Chưa dọn vào)"}");
                        Console.WriteLine($"Ngày dọn ra: \t{nt.NgayDonRa?.ToString("dd/MM/yyyy") ?? "(Chưa dọn ra)"}");
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

        static void TimNguoiThueTheoPhong()
        {
            try
            {
                Console.Write("Nhập Mã phòng cần tìm (ví dụ: PH001): ");
                string maPhong = Console.ReadLine() ?? "";

                Console.WriteLine($"-- DANH SÁCH NGƯỜI THUÊ CỦA PHÒNG {maPhong} --");
                List<NguoiThue> ds = ql_NguoiThue.getByMaPhong(maPhong);

                if (ds == null || ds.Count == 0)
                {
                    Console.WriteLine($"Không tìm thấy người thuê nào thuộc phòng {maPhong}.");
                }
                else
                {
                    foreach (var nt in ds)
                    {
                        Console.WriteLine("---------------------------------");
                        Console.WriteLine($"Mã NT: \t\t{nt.MaNguoiThue}");
                        Console.WriteLine($"Họ tên: \t{nt.HoTen}");
                        Console.WriteLine($"SĐT: \t\t{nt.Sdt}");
                        Console.WriteLine($"Email: \t\t{nt.Email}");
                        Console.WriteLine($"Vai trò: \t{nt.VaiTro}");
                        Console.WriteLine($"Trạng thái: \t{nt.TrangThaiThue}");
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

        // Hàm hỗ trợ đọc ngày tháng (bắt buộc)
        static DateTime ReadDate(string prompt)
        {
            DateTime value;
            while (true)
            {
                Console.Write(prompt);
                if (DateTime.TryParseExact(Console.ReadLine(), "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out value))
                {
                    return value;
                }
                else
                {
                    Console.WriteLine("Định dạng ngày không hợp lệ. Vui lòng nhập lại theo (dd/MM/yyyy).");
                }
            }
        }

        // Hàm hỗ trợ đọc ngày tháng (có thể bỏ trống - nullable)
        static DateTime? ReadDateNullable(string prompt)
        {
            DateTime value;
            while (true)
            {
                Console.Write(prompt);
                string input = Console.ReadLine() ?? "";
                if (string.IsNullOrEmpty(input))
                {
                    return null; // Trả về null nếu bỏ trống
                }
                if (DateTime.TryParseExact(input, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out value))
                {
                    return value;
                }
                else
                {
                    Console.WriteLine("Định dạng ngày không hợp lệ. Vui lòng nhập lại theo (dd/MM/yyyy) hoặc bỏ trống.");
                }
            }
        }
    }
}