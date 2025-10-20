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

            bool exists = _dangNhap.checkSDT(Email); // hoặc checkEmail nếu có
            if (exists)
            {
                _dangNhap.OTP(Email);
                var otpWindow = new OtpVerificationWindow();
                otpWindow.Show();

                // Đóng ForgotPasswordWindow hiện tại
                Application.Current.Windows
                    .OfType<Window>()
                    .FirstOrDefault(w => w.DataContext == this)?
                    .Close();
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
