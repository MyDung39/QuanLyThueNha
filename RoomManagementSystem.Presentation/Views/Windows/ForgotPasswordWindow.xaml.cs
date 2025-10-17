using System.Windows;
using System.Windows.Input;

namespace RoomManagementSystem.Presentation.Views.Windows
{
    public partial class ForgotPasswordWindow : Window
    {
        public ForgotPasswordWindow()
        {
            InitializeComponent();
            this.Loaded += (s, e) => EmailInput.Focus(); // Tự động focus vào ô email
        }

        // --- Logic sự kiện ---
        private void BackToLogin_Click(object sender, RoutedEventArgs e)
        {
            // Logic để mở lại cửa sổ đăng nhập
            var loginWindow = new LoginWindow();
            loginWindow.Show();
            this.Close(); // Đóng cửa sổ hiện tại
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