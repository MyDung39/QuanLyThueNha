using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading; // Cần cho DispatcherTimer

namespace RoomManagementSystem.Presentation.Views.Windows
{
    public partial class OtpVerificationWindow : Window
    {
        private DispatcherTimer _countdownTimer;
        private int _countdownSeconds;

        public OtpVerificationWindow()
        {
            InitializeComponent();
            this.Loaded += OtpVerificationWindow_Loaded;
        }

        private void OtpVerificationWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Bắt đầu đếm ngược ngay khi cửa sổ được tải
            StartCountdown(60);
            OtpBox1.Focus(); // Tự động focus vào ô đầu tiên
        }

        // --- Logic đếm ngược ---
        private void StartCountdown(int seconds)
        {
            _countdownSeconds = seconds;
            ResendCodeButton.IsEnabled = false; // Vô hiệu hóa nút gửi lại
            ResendCodeButton.Opacity = 0.4;
            CountdownText.Visibility = Visibility.Visible;

            _countdownTimer = new DispatcherTimer();
            _countdownTimer.Interval = TimeSpan.FromSeconds(1);
            _countdownTimer.Tick += CountdownTimer_Tick;
            _countdownTimer.Start();

            CountdownTimerRun.Text = _countdownSeconds.ToString();
        }

        private void CountdownTimer_Tick(object sender, EventArgs e)
        {
            _countdownSeconds--;
            CountdownTimerRun.Text = _countdownSeconds.ToString();

            if (_countdownSeconds <= 0)
            {
                _countdownTimer.Stop();
                ResendCodeButton.IsEnabled = true; // Kích hoạt lại nút
                ResendCodeButton.Opacity = 0.7;
                CountdownText.Visibility = Visibility.Collapsed; // Ẩn thông báo
            }
        }

        private void ResendCodeButton_Click(object sender, RoutedEventArgs e)
        {
            // Logic gửi lại mã ở đây...
            MessageBox.Show("Đang gửi lại mã xác minh...");
            StartCountdown(60); // Bắt đầu đếm ngược lại
        }

        // --- Logic xử lý các ô OTP ---
        private void OtpBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var currentTextBox = sender as TextBox;
            if (currentTextBox == null || currentTextBox.Text.Length != 1)
                return;

            // Tự động chuyển đến ô tiếp theo
            if (currentTextBox == OtpBox1)
                OtpBox2.Focus();
            else if (currentTextBox == OtpBox2)
                OtpBox3.Focus();
            else if (currentTextBox == OtpBox3)
                OtpBox4.Focus();
        }

        private void OtpBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            var currentTextBox = sender as TextBox;
            if (currentTextBox == null) return;

            // Xử lý phím Backspace để quay lại ô trước đó
            if (e.Key == Key.Back && currentTextBox.Text.Length == 0)
            {
                if (currentTextBox == OtpBox4)
                    OtpBox3.Focus();
                else if (currentTextBox == OtpBox3)
                    OtpBox2.Focus();
                else if (currentTextBox == OtpBox2)
                    OtpBox1.Focus();
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