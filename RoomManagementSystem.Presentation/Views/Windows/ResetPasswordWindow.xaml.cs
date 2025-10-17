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
        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            var passwordBox = sender as PasswordBox;
            if (passwordBox == null) return;

            // Tìm TextBlock placeholder tương ứng bên trong ControlTemplate
            var placeholder = passwordBox.Template.FindName("PlaceholderText", passwordBox) as TextBlock;
            if (placeholder != null)
            {
                // Nếu ô Password có nội dung, ẩn chữ mờ đi. Nếu trống, hiện ra.
                placeholder.Visibility = string.IsNullOrEmpty(passwordBox.Password) ? Visibility.Visible : Visibility.Collapsed;
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