using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows;
using RoomManagementSystem.BusinessLayer;
using System.Windows.Controls; // Cần để dùng PasswordBox

namespace RoomManagementSystem.Presentation.ViewModels
{
    public partial class LoginViewModel : ViewModelBase
    {
        // Framework sẽ tự động tạo property "Email" từ field "_email"
        // và tự gọi OnPropertyChanged() khi giá trị thay đổi.
        [ObservableProperty]
        private string _email = string.Empty;


        private readonly DangNhap _dangNhap;
        



        public LoginViewModel()
        {
            _dangNhap = new DangNhap();
            
        }



        [RelayCommand]
        private void ForgotPassword()
        {
            // Mở cửa sổ quên mật khẩu
            var forgotPasswordWindow = new RoomManagementSystem.Presentation.Views.Windows.ForgotPasswordWindow();
            forgotPasswordWindow.Show();

            // (Tùy chọn) đóng cửa sổ đăng nhập hiện tại
            Application.Current.Windows[0]?.Close();
        }




        // Framework sẽ tự động tạo một ICommand tên là "LoginCommand"
        // từ phương thức Login() này.
        [RelayCommand]
        private void Login(PasswordBox passwordBox)
        {
            string email = Email;
            string password = passwordBox.Password;

            // ================================================================
            // ✅ BƯỚC 1: THÊM VALIDATION (KIỂM TRA ĐẦU VÀO)
            // ================================================================
            if (string.IsNullOrEmpty(email))
            {
                MessageBox.Show("Vui lòng nhập địa chỉ email.", "Thông báo lỗi");
                return; // Dừng hàm, không làm gì cả
            }

            if (string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Vui lòng nhập mật khẩu.", "Thông báo lỗi");
                return; // Dừng hàm, không làm gì cả
            }
            // ================================================================
            // ✅ KẾT THÚC VALIDATION
            // ================================================================

            // Code bên dưới này CHỈ CHẠY khi cả email và mật khẩu đều đã được nhập
            if (_dangNhap.Login(Email, password))
            {
                // Hiển thị thông báo thành công
                //MessageBox.Show("Đăng nhập thành công!", "Thông báo");

                // Mở MainWindow
                //var mainWindow = new RoomManagementSystem.Presentation.Views.Windows.MainWindow();
                //mainWindow.Show();

                var testWindow = new RoomManagementSystem.Presentation.Views.Windows.TestWindow();
                testWindow.Show();

                // Đóng LoginWindow hiện tại
                CloseCurrentWindow();
            }
            else
            {
                MessageBox.Show("Sai email hoặc mật khẩu!", "Lỗi");
            }
        }

        private void CloseCurrentWindow()
        {
            var window = Application.Current.Windows
                         .OfType<Window>()
                         .FirstOrDefault(w => w.DataContext == this);
            window?.Close();
        }

    }
}