using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RoomManagementSystem.BusinessLayer;
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

        private readonly DangNhap _dangNhap;
        



        public LoginViewModel()
        {
            _dangNhap = new DangNhap();
            
        }

        // Framework sẽ tự động tạo một ICommand tên là "LoginCommand"
        // từ phương thức Login() này.
        [RelayCommand]
        private void Login(PasswordBox passwordBox)
        {
           

            // Lấy mật khẩu từ PasswordBox được truyền vào
            string password = passwordBox.Password;

            // ===== LOGIC ĐĂNG NHẬP Ở ĐÂY =====
            // Trong tương lai, bạn sẽ gọi một service từ BusinessLayer
            // Ví dụ: var user = _authService.Login(Email, password);

            // Hiện tại, chúng ta chỉ hiển thị một thông báo để kiểm tra
            //MessageBox.Show($"Email: {Email}\nPassword: {password}\n\nAttempting to log in...");

            if (_dangNhap.Login(Email, password))
            {
                MessageBox.Show("Đăng nhập thành công!", "Thông báo");

                
                
               

                // đóng cửa sổ đăng nhập
                Application.Current.Windows[0]?.Close();
            }
            else
            {
                MessageBox.Show("Sai email hoặc mật khẩu!", "Lỗi");
            }
        }
    }
}