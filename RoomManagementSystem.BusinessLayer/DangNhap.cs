using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RoomManagementSystem.DataLayer;

namespace RoomManagementSystem.BusinessLayer
{
    public class DangNhap
    {
        NguoiDungAccess user=new NguoiDungAccess();

        //Kiem tra dang nhap bang email
        public Boolean Login(string email,string password)
        {
            return user.checkDangNhap(email, password);
        }

        //Kiem tra dang nhap bang OTP

        //Kiem tra so dien thoai nguoi dung
        public Boolean checkSDT(string sdt)
        {
            return user.sodienthoai(sdt);
        }

        //Tao OTP khi xac thuc sdt
        public string otp;
        public string OTP(string sdt)
        {
            if (user.sodienthoai(sdt))
            {
                Random rand = new Random();
                int num = rand.Next(1000, 10000);
                otp = num.ToString();
                return otp;
            }
            return "Fail";
        }

        //Xac thuc OTP
        public Boolean verifyOTP(string inputOTP)
        {
            return otp != null && otp == inputOTP;
        }

        //Cap nhat Password nguoi dung (khi da dang nhap bang otp hoac mat khau)
        public Boolean UpdatePassword(string newPassword)
        {
            return user.UpdatePassword(newPassword);
        }

    }
}
