using System;
using RoomManagementSystem.BusinessLayer;
using RoomManagementSystem.DataLayer;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace RoomManagementSystem.App
{
    class Program
    {
        // Khởi tạo đối tượng QL_HopDong để dùng chung cho các chức năng
        static QL_HopDong qlHD = new QL_HopDong();

        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.WriteLine("QUẢN LÝ HỢP ĐỒNG");
            Console.WriteLine("=====================================");
            while (true)
            {
                // Hiển thị Menu
                Console.WriteLine("\n--- MENU CHỨC NĂNG ---");
                Console.WriteLine("1. Xem danh sách hợp đồng");
                Console.WriteLine("2. Thêm hợp đồng mới");
                Console.WriteLine("3. Cập nhật thông tin hợp đồng");
                Console.WriteLine("4. Xóa hợp đồng");
                Console.WriteLine("5. Kiểm tra & Tạo thông báo (cho hợp đồng sắp hết hạn)");
                Console.WriteLine("6. Xuất hợp đồng ra file PDF (Tải về)");
                Console.WriteLine("0. Thoát chương trình");
                Console.ResetColor();
                Console.Write("Vui lòng chọn chức năng: ");

                string? luaChon = Console.ReadLine();

                // Xóa màn hình để giao diện sạch hơn
                Console.Clear();

                switch (luaChon)
                {
                    case "1":
                        TestXemDanhSach();
                        break;
                    case "2":
                        TestThemHopDong();
                        break;
                    case "3":
                        TestCapNhatHopDong();
                        break;
                    case "4":
                        TestXoaHopDong();
                        break;
                    case "5":
                        TestTaoThongBao();
                        break;
                    case "6":
                        TestXuatPDF();
                        break;
                    case "0":
                        Console.WriteLine("Cảm ơn bạn đã sử dụng. Nhấn Enter để thoát.");
                        Console.ReadLine();
                        return;
                    default:
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("Lựa chọn không hợp lệ. Vui lòng chọn lại.");
                        Console.ResetColor();
                        break;
                }

                Console.WriteLine("\nNhấn Enter để quay lại menu...");
                Console.ReadLine();
                Console.Clear(); // Xóa màn hình trước khi lặp lại menu
            }
        }


        static void TestXemDanhSach()
        {
            Console.WriteLine("\n--- 1. Danh Sách Hợp Đồng ---");
            try
            {
                List<HopDong> listHD = qlHD.DanhSachHopDong();
                if (listHD.Count == 0)
                {
                    Console.WriteLine("Không có hợp đồng nào trong hệ thống.");
                    return;
                }

                foreach (var hd in listHD)
                {
                    Console.WriteLine($"- Mã HĐ: {hd.MaHopDong} | Phòng: {hd.MaPhong} | Người Thuê: {hd.MaNguoiThue} | Trạng Thái: {hd.TrangThai} | Kết Thúc: {hd.NgayKetThuc:dd/MM/yyyy}");
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"LỖI KHI TẢI DANH SÁCH: {ex.Message}");
                Console.ResetColor();
            }
        }

        static void TestThemHopDong()
        {
            Console.WriteLine("\n--- 2. Thêm Hợp Đồng Mới ---");
            try
            {
                HopDong hdMoi = new HopDong();

                // Mã hợp đồng sẽ được tạo tự động bởi QL_HopDong và HopDongDAL

                Console.Write("Nhập Mã Phòng (ví dụ: PHONG003): ");
                hdMoi.MaPhong = Console.ReadLine();

                Console.Write("Nhập Số Giấy Tờ (CCCD/CMND) của Người Thuê: ");
                string soGiayTo = Console.ReadLine() ?? "";

                // Gọi BLL để tìm MaNguoiThue
                string? maNguoiThue = qlHD.TimMaNguoiThueBangSoGiayTo(soGiayTo);

                if (string.IsNullOrEmpty(maNguoiThue))
                {
                    // Nếu không tìm thấy, báo lỗi và dừng lại
                    throw new Exception($"Không tìm thấy người thuê nào có Số Giấy Tờ là '{soGiayTo}'.");
                }

                // Nếu tìm thấy, gán vào đối tượng
                Console.WriteLine($"Đã tìm thấy Mã Người Thuê tương ứng: {maNguoiThue}");
                hdMoi.MaNguoiThue = maNguoiThue;


                Console.Write("Nhập Tiền Cọc (ví dụ: 2000000): ");
                hdMoi.TienCoc = decimal.Parse(Console.ReadLine() ?? "0");

                Console.Write("Nhập Ngày Bắt Đầu (định dạng yyyy-MM-dd, ví dụ: 2025-05-01): ");
                hdMoi.NgayBatDau = DateTime.Parse(Console.ReadLine() ?? DateTime.Now.ToString("yyyy-MM-dd"));

                Console.Write("Nhập Thời Hạn (số tháng, ví dụ: 12): ");
                hdMoi.ThoiHan = int.Parse(Console.ReadLine() ?? "12");

                hdMoi.TrangThai = "Hiệu lực"; // Mặc định khi mới tạo

                // Hàm ThemHopDong bây giờ sẽ nhận được MaNguoiThue
                // và tự động kiểm tra VaiTro (Chủ hợp đồng) bên trong
                bool kq = qlHD.ThemHopDong(hdMoi);

                if (kq)
                {
                    // hdMoi.MaHopDong đã được cập nhật bởi BLL
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"Thêm thành công hợp đồng! Mã hợp đồng mới là: {hdMoi.MaHopDong}");
                    Console.ResetColor();
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("Thêm thất bại (không có lỗi ngoại lệ).");
                    Console.ResetColor();
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"LỖI KHI THÊM: {ex.Message}");
                Console.ResetColor();
            }
        }


        static void TestCapNhatHopDong()
        {
            Console.WriteLine("\n--- 3. Cập Nhật Hợp Đồng ---");
            try
            {
                Console.Write("Nhập Mã Hợp Đồng cần cập nhật (ví dụ: HD001): ");
                string maHD = Console.ReadLine() ?? "";

                // Lấy thông tin hợp đồng hiện tại
                HopDong? hdCanSua = qlHD.DanhSachHopDong().Find(h => h.MaHopDong == maHD);

                if (hdCanSua == null)
                {
                    Console.WriteLine("Không tìm thấy hợp đồng với mã này.");
                    return;
                }

                Console.WriteLine($"Tìm thấy hợp đồng: {hdCanSua.MaHopDong} | Phòng: {hdCanSua.MaPhong}");
                Console.WriteLine($"Trạng thái hiện tại: {hdCanSua.TrangThai}");
                Console.Write("Nhập Trạng Thái mới (bỏ trống để giữ nguyên): ");
                string trangThaiMoi = Console.ReadLine() ?? "";
                if (!string.IsNullOrEmpty(trangThaiMoi))
                {
                    hdCanSua.TrangThai = trangThaiMoi;
                }

                Console.WriteLine($"Thời hạn hiện tại: {hdCanSua.ThoiHan} tháng");
                Console.Write("Nhập Thời Hạn mới (bỏ trống để giữ nguyên): ");
                string thoiHanMoiStr = Console.ReadLine() ?? "";
                if (!string.IsNullOrEmpty(thoiHanMoiStr))
                {
                    hdCanSua.ThoiHan = int.Parse(thoiHanMoiStr);
                }

                bool kq = qlHD.CapNhatHopDong(hdCanSua);
                Console.WriteLine(kq ? "Cập nhật thành công!" : "Cập nhật thất bại.");
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"LỖI KHI CẬP NHẬT: {ex.Message}");
                Console.ResetColor();
            }
        }

        static void TestXoaHopDong()
        {
            Console.WriteLine("\n--- 4. Xóa Hợp Đồng ---");
            try
            {
                Console.Write("Nhập Mã Hợp Đồng cần xóa (ví dụ: HD003): ");
                string maHD = Console.ReadLine() ?? "";

                if (string.IsNullOrEmpty(maHD))
                {
                    Console.WriteLine("Mã hợp đồng không được để trống.");
                    return;
                }

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write($"Bạn có chắc chắn muốn xóa {maHD}? (Y/N): ");
                Console.ResetColor();
                string xacNhan = Console.ReadLine() ?? "";

                if (xacNhan.Equals("Y", StringComparison.OrdinalIgnoreCase))
                {
                    bool kq = qlHD.XoaHopDong(maHD);
                    Console.WriteLine(kq ? "Xóa thành công!" : "Xóa thất bại (Có thể do ràng buộc khóa ngoại, ví dụ: hợp đồng đã có thông báo hạn).");
                }
                else
                {
                    Console.WriteLine("Đã hủy thao tác xóa.");
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"LỖI KHI XÓA: {ex.Message}");
                Console.ResetColor();
            }
        }

        static void TestTaoThongBao()
        {
            Console.WriteLine("\n--- 5. Kiểm Tra Thông Báo Hết Hạn ---");
            try
            {
                Console.Write("Nhập Mã Hợp Đồng cần kiểm tra (ví dụ: HD001): ");
                string maHD = Console.ReadLine() ?? "";

                if (string.IsNullOrEmpty(maHD))
                {
                    Console.WriteLine("Mã hợp đồng không được để trống.");
                    return;
                }

                Console.WriteLine($"Kiểm tra {maHD} với ngày hệ thống là {DateTime.Now:dd/MM/yyyy}...");

                bool kq = qlHD.TaoThongBaoHetHan(maHD);

                if (kq)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Thành công! Đã tạo thông báo mới (vì hợp đồng sắp hết hạn trong 30 ngày tới).");
                    Console.ResetColor();
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("Không cần tạo thông báo (hợp đồng còn hạn > 30 ngày hoặc đã hết hạn).");
                    Console.ResetColor();
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"LỖI KHI KIỂM TRA: {ex.Message}");
                Console.ResetColor();
            }
        }

        static void TestXuatPDF()
        {
            Console.WriteLine("\n--- 6. Xuất Hợp Đồng PDF (Tải về Desktop) ---");
            try
            {
                Console.Write("Nhập Mã Hợp Đồng cần xuất file (ví dụ: HD001 hoặc HD002): ");
                string maHopDongCanXuat = Console.ReadLine() ?? "";

                if (string.IsNullOrEmpty(maHopDongCanXuat))
                {
                    Console.WriteLine("Mã hợp đồng không được để trống.");
                    return;
                }

                // Tạo đường dẫn lưu file trên Desktop của người dùng
                string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                string outputFileName = $"HopDong_{maHopDongCanXuat}_Output.pdf";
                string outputPath = Path.Combine(desktopPath, outputFileName);

                Console.WriteLine("Đang xử lý xuất file, vui lòng đợi...");
                bool kqXuatFile = qlHD.XuatHopDongRaPdf(maHopDongCanXuat, outputPath);

                if (kqXuatFile)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"Thành công! File đã được lưu tại: {outputPath}");
                    Console.ResetColor();

                    // Tự động mở thư mục Desktop
                    try
                    {
                        Process.Start("explorer.exe", desktopPath);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Không thể tự động mở thư mục: {ex.Message}");
                    }
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("Thất bại! Không thể xuất file.");
                    Console.WriteLine("Nguyên nhân có thể là:");
                    Console.WriteLine($"1. Không tìm thấy hợp đồng '{maHopDongCanXuat}'.");
                    Console.WriteLine("2. File template 'mau-hop-dong-thue-nha-o.docx' không tồn tại trong folder 'Templates'.");
                    Console.ResetColor();
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"LỖI KHI XUẤT FILE: {ex.Message}");
                Console.ResetColor();
            }
        }
    }
}