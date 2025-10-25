using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows;
using System.Windows.Controls; // Cần để dùng PasswordBox

namespace RoomManagementSystem.Presentation.ViewModels
{
    public partial class LoginViewModel : ViewModelBase
    {
        // Framework sẽ tự động tạo property "Email" từ field "_email"
        // và tự gọi OnPropertyChanged() khi giá trị thay đổi.
        [ObservableProperty]
        private string _email = string.Empty;

<<<<<<< HEAD
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



=======
>>>>>>> origin/an_xuyen
        // Framework sẽ tự động tạo một ICommand tên là "LoginCommand"
        // từ phương thức Login() này.
        [RelayCommand]
        private void Login(PasswordBox passwordBox)
        {
<<<<<<< HEAD

            string email = Email;
=======
>>>>>>> origin/an_xuyen
            // Lấy mật khẩu từ PasswordBox được truyền vào
            string password = passwordBox.Password;



            // ===== LOGIC ĐĂNG NHẬP Ở ĐÂY =====
            // Trong tương lai, bạn sẽ gọi một service từ BusinessLayer
            // Ví dụ: var user = _authService.Login(Email, password);

            // Hiện tại, chúng ta chỉ hiển thị một thông báo để kiểm tra
<<<<<<< HEAD
            //MessageBox.Show($"Email: {Email}\nPassword: {password}\n\nAttempting to log in...");

            if (_dangNhap.Login(Email, password))
            {
                // Hiển thị thông báo thành công
                //MessageBox.Show("Đăng nhập thành công!", "Thông báo");

                // Mở MainWindow
                var mainWindow = new RoomManagementSystem.Presentation.Views.Windows.MainWindow();
                mainWindow.Show();

                // Đóng LoginWindow hiện tại
                CloseCurrentWindow();
            }
            else
            {
                MessageBox.Show("Sai email hoặc mật khẩu!", "Lỗi");
            }
=======
            MessageBox.Show($"Email: {Email}\nPassword: {password}\n\nAttempting to log in...");
>>>>>>> origin/an_xuyen
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