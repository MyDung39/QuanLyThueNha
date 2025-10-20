using System;
using RoomManagementSystem.BusinessLayer;

namespace RoomManagementSystem.App
{
    class Program
    {
        static void Main(string[] args)
        {
            DangNhap dn = new DangNhap();

            Console.WriteLine("=== TEST DANG NHAP ===");

            // 1. Test đăng nhập bằng email + password
            Console.WriteLine("\n>> Test login with Email + Password");
            string email = "admin@gmail.com";   // thay bằng email có trong DB
            string password = "admin";          // thay bằng password thật trong DB
            bool loginResult = dn.Login(email, password);
            Console.WriteLine(loginResult ? "Đăng nhập thành công" : "Đăng nhập thất bại");

            // 2. Test OTP bằng SĐT
            Console.WriteLine("\n>> Test OTP");
            string sdt = "0908083890";  // thay bằng số điện thoại trong DB
            string otp = dn.OTP(sdt);
            if (otp != "Fail")
            {
                Console.WriteLine($"OTP đã tạo: {otp}");
                Console.Write("Nhập OTP vừa nhận: ");
                string inputOTP = Console.ReadLine();
                bool otpCheck = dn.verifyOTP(inputOTP);
                Console.WriteLine(otpCheck ? "OTP hợp lệ" : "OTP sai");
            }
            else
            {
                Console.WriteLine("SĐT không tồn tại trong hệ thống!");
            }

            // 3. Test cập nhật password
            Console.WriteLine("\n>> Test Update Password");
            Console.Write("Nhập mật khẩu mới: ");
            string newPassword = Console.ReadLine();
            bool updateResult = dn.UpdatePassword(newPassword);
            Console.WriteLine(updateResult ? "Cập nhật mật khẩu thành công" : "Cập nhật mật khẩu thất bại");

            Console.WriteLine("\nEnd test.");
            Console.ReadKey();
        }
    }
}

