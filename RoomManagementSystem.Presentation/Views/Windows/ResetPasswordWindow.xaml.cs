using RoomManagementSystem.Presentation.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace RoomManagementSystem.Presentation.Views.Windows
{
    public partial class ResetPasswordWindow : Window
    {
        public ResetPasswordWindow()
        {
            InitializeComponent();
            this.Loaded += (s, e) => NewPasswordBox.Focus(); // Tự động focus vào ô đầu tiên
        }

        // --- Logic xử lý Placeholder cho PasswordBox ---
        // --- HÀM XỬ LÝ CHÍNH ---
        // Sửa lại hàm này để làm 2 việc:
        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            var passwordBox = sender as PasswordBox;
            if (passwordBox == null) return;

            // --- 1. Logic Placeholder (Code cũ của bạn) ---
            var placeholder = passwordBox.Template.FindName("PlaceholderText", passwordBox) as TextBlock;
            if (placeholder != null)
            {
                placeholder.Visibility = string.IsNullOrEmpty(passwordBox.Password) ? Visibility.Visible : Visibility.Collapsed;
            }

            // --- 2. Logic "Cầu nối" Cập nhật ViewModel (PHẦN BỊ THIẾU) ---

            // Lấy ViewModel từ DataContext
            var viewModel = this.DataContext as ResetPasswordViewModel;
            if (viewModel == null) return; // Thoát nếu ViewModel chưa được gán

            // Cập nhật thuộc tính tương ứng trong ViewModel
            // (Dựa trên tên 'x:Name' của PasswordBox trong XAML)
            if (passwordBox.Name == "NewPasswordBox")
            {
                viewModel.NewPassword = passwordBox.Password;
            }
            else if (passwordBox.Name == "ConfirmPasswordBox")
            {
                viewModel.ConfirmPassword = passwordBox.Password;
            }
        }

        // --- Logic điều khiển cửa sổ (giữ nguyên) ---
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }


        private void MaximizeButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = (WindowState == WindowState.Maximized) ? WindowState.Normal : WindowState.Maximized;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}