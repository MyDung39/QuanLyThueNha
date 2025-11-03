using DocumentFormat.OpenXml.Spreadsheet;
using RoomManagementSystem.DataLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace RoomManagementSystem.BusinessLayer
{
    public class DangNhap
    {

        NguoiDungAccess user = new NguoiDungAccess();

        //Kiem tra dang nhap bang email
        public Boolean Login(string email, string password)

        {
            return user.checkDangNhap(email, password);
        }

        //Kiem tra dang nhap bang OTP
        string otp;

        private string _emailDangXuLy;


        //Tao OTP
        public string OTP()
        {
            Random rnd = new Random();

            otp = rnd.Next(1000, 9999).ToString();

            return otp;
        }
        //Kiem tra Mail nguoi dung
        public Boolean checkMail(string Mail)
        {

            bool exists = user.Mail(Mail);
            if (exists)
            {
                // Nếu mail tồn tại, lưu lại để biết chúng ta đang xử lý cho ai
                _emailDangXuLy = Mail;
            }
            return exists;
        }



        public bool SendOTP(string toEmail, string otp)
        {
            try
            {
                MailMessage mail = new MailMessage();
                mail.To.Add(toEmail);
                mail.From = new MailAddress("pentanix79@gmail.com");
                mail.Subject = "Mã OTP xác thực đăng nhập";
                mail.Body = $"Mã OTP của bạn là: {otp}";

                using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587))
                {

                    smtp.Credentials = new NetworkCredential("pentanix79@gmail.com", "cjrm zdds dacn bgtn");

                    smtp.EnableSsl = true;
                    smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                    smtp.Send(mail);
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Lỗi gửi mail: " + ex.ToString());
                return false;
            }
        }
        //Kiem tra Dang nhap bang otp

        public bool CheckOTP( string otp)
        {
            return this.otp == otp;
        }
        //Cap nhat Password nguoi dung (khi da dang nhap bang otp hoac mat khau)
        public bool UpdatePassword(string newPassword)
        {
            // Xóa bỏ kiểm tra "if" đi, vì bạn không dùng _emailDangXuLy

            // Cứ thế gọi DataLayer, vì bạn nói DataLayer tự biết

            return user.UpdatePassword(newPassword);
        }

    }
}
