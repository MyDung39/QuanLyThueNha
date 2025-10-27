using System;
using RoomManagementSystem.BusinessLayer;

namespace RoomManagementSystem.Test
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            DangNhap dn = new DangNhap();

            Console.WriteLine("====== TEST CHỨC NĂNG ĐĂNG NHẬP & OTP ======\n");

            // 1️⃣ Nhập email để kiểm tra
            Console.Write("Nhập Email người dùng: ");
            string email = Console.ReadLine();

            if (!dn.checkMail(email))
            {
                Console.WriteLine("❌ Email không tồn tại trong hệ thống!");
                return;
            }
            else
            {
                Console.WriteLine("✅ Email hợp lệ trong hệ thống.");
            }

            // 2️⃣ Chọn cách đăng nhập
            Console.WriteLine("\nChọn cách đăng nhập:");
            Console.WriteLine("1. Đăng nhập bằng mật khẩu");
            Console.WriteLine("2. Đăng nhập bằng OTP");
            Console.Write("Lựa chọn của bạn (1/2): ");
            string choice = Console.ReadLine();

            bool loginSuccess = false;

            if (choice == "1")
            {
                // Đăng nhập bằng mật khẩu
                Console.Write("Nhập mật khẩu: ");
                string password = Console.ReadLine();

                if (dn.Login(email, password))
                {
                    Console.WriteLine("✅ Đăng nhập thành công!");
                    loginSuccess = true;
                }
                else
                {
                    Console.WriteLine("❌ Sai email hoặc mật khẩu!");
                }
            }
            else if (choice == "2")
            {
                // Đăng nhập bằng OTP
                string otp = dn.OTP();
                Console.WriteLine($"\n🔐 OTP tạo ra: {otp} (chỉ để test, thực tế sẽ gửi email)");

                // Gửi OTP qua Gmail
                Console.WriteLine("Đang gửi OTP đến email...");
                if (dn.SendOTP(email, otp))
                {
                    Console.WriteLine("✅ Gửi OTP thành công! Hãy kiểm tra hộp thư đến.");
                }
                else
                {
                    Console.WriteLine("❌ Không gửi được OTP! Kiểm tra lại cấu hình Gmail hoặc quyền truy cập ứng dụng.");
                    return;
                }

                Console.Write("\nNhập OTP bạn nhận được: ");
                string nhapOtp = Console.ReadLine();

                if (dn.CheckOTP(email, nhapOtp))
                {
                    Console.WriteLine("✅ Xác thực OTP thành công!");
                    loginSuccess = true;
                }
                else
                {
                    Console.WriteLine("❌ Mã OTP không đúng hoặc đã hết hạn!");
                }
            }

            // 3️⃣ Nếu đăng nhập thành công → cho phép đổi mật khẩu
            if (loginSuccess)
            {
                Console.Write("\nBạn có muốn đổi mật khẩu mới không? (y/n): ");
                string doi = Console.ReadLine();
                if (doi.ToLower() == "y")
                {
                    Console.Write("Nhập mật khẩu mới: ");
                    string newPass = Console.ReadLine();

                    if (dn.UpdatePassword(newPass))
                        Console.WriteLine("✅ Đổi mật khẩu thành công!");
                    else
                        Console.WriteLine("❌ Đổi mật khẩu thất bại!");
                }
            }

            Console.WriteLine("\n=== Kết thúc chương trình test ===");
            Console.ReadKey();
        }
    }
}