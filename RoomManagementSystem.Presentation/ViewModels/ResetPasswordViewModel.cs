using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RoomManagementSystem.BusinessLayer;
using RoomManagementSystem.Presentation.Views.Windows;
using System.Linq;
using System.Windows;

namespace RoomManagementSystem.Presentation.ViewModels
{
    public partial class ResetPasswordViewModel : ViewModelBase
    {
        private readonly DangNhap _dangNhapService;

        // 1. Tạo 2 thuộc tính cho 2 ô mật khẩu
        // Chúng ta sẽ cập nhật 2 thuộc tính này từ code-behind
        [ObservableProperty]
        private string _newPassword = string.Empty;

        [ObservableProperty]
        private string _confirmPassword = string.Empty;

        public ResetPasswordViewModel(DangNhap dangNhapService)
        {
            _dangNhapService = dangNhapService;
        }

        [RelayCommand]
        private void ResetPassword()
        {
            // 2. LOGIC XÁC THỰC (nằm trong ViewModel)

            // Kiểm tra rỗng (dùng thuộc tính đã được cập nhật)
            if (string.IsNullOrWhiteSpace(NewPassword) || string.IsNullOrWhiteSpace(ConfirmPassword))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ mật khẩu.");
                return;
            }

            // Kiểm tra trùng khớp
            if (NewPassword != ConfirmPassword)
            {
                MessageBox.Show("Mật khẩu xác nhận không khớp!");
                return;
            }

            // 3. Nếu mọi thứ OK, gọi BusinessLayer
            bool success = _dangNhapService.UpdatePassword(NewPassword);

            if (success)
            {
                MessageBox.Show("Đổi mật khẩu thành công! Vui lòng đăng nhập lại.");

                var loginWindow = new LoginWindow();
                loginWindow.Show();
                CloseCurrentWindow();
            }
            else
            {
                MessageBox.Show("Có lỗi xảy ra, không thể cập nhật mật khẩu.");
            }
        }

        private void CloseCurrentWindow()
        {
            Application.Current.Windows
                .OfType<Window>()
                .FirstOrDefault(w => w.DataContext == this)?
                .Close();
        }
    }
}