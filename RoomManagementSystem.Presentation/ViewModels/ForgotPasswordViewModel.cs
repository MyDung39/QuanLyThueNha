using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RoomManagementSystem.BusinessLayer;
using RoomManagementSystem.Presentation.Views.Windows;
using System.Linq;
using System.Windows;

namespace RoomManagementSystem.Presentation.ViewModels
{
    public partial class ForgotPasswordViewModel : ViewModelBase
    {
        private readonly DangNhap _dangNhap = new DangNhap();

        [ObservableProperty]
        private string email = string.Empty;

        [RelayCommand]
        private void SendEmail()
        {
            if (string.IsNullOrWhiteSpace(Email))
            {
                MessageBox.Show("Vui lòng nhập email!");
                return;
            }

            bool exists = _dangNhap.checkMail(Email);
            if (exists)
            {
                // 1. Tạo và LẤY mã OTP
                string generatedOtp = _dangNhap.OTP();

                // 2. GỌI HÀM gửi email với mã OTP vừa tạo
                bool sentSuccessfully = _dangNhap.SendOTP(Email, generatedOtp);

                if (sentSuccessfully)
                {
                    // 3. Nếu gửi thành công, mở cửa sổ OTP

                    // ===========================================
                    // BẮT ĐẦU SỬA LỖI
                    // ===========================================

                    // a. Tạo OtpVerificationViewModel và truyền dịch vụ DangNhap vào
                    var otpViewModel = new OtpVerificationViewModel(_dangNhap);

                    // b. Tạo cửa sổ
                    var otpWindow = new OtpVerificationWindow();

                    // c. GÁN DATACONTEXT (Đây là bước bạn bị thiếu)
                    otpWindow.DataContext = otpViewModel;

                    // d. Hiển thị cửa sổ
                    otpWindow.Show();

                    // ===========================================
                    // KẾT THÚC SỬA LỖI
                    // ===========================================

                    // Đóng ForgotPasswordWindow hiện tại
                    Application.Current.Windows
                        .OfType<Window>()
                        .FirstOrDefault(w => w.DataContext == this)?
                        .Close();
                }
                else
                {
                    // 4. Thông báo lỗi nếu không gửi được mail
                    MessageBox.Show("Có lỗi xảy ra khi gửi email xác thực. Vui lòng thử lại!");
                }
            }
            else
            {
                MessageBox.Show("Email không tồn tại!");
            }
        }

        [RelayCommand]
        private void BackToLogin()
        {
            var loginWindow = new LoginWindow();
            loginWindow.Show();

            Application.Current.Windows
                .OfType<Window>()
                .FirstOrDefault(w => w.DataContext == this)?
                .Close();
        }
    }
}
