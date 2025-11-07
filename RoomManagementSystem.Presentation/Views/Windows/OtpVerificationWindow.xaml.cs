using RoomManagementSystem.Presentation.ViewModels; // <-- THÊM DÒNG NÀY
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

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
            StartCountdown(60);
            OtpBox1.Focus();
        }

        // --- Logic đếm ngược (Giữ nguyên code của bạn) ---
        private void StartCountdown(int seconds)
        {
            _countdownSeconds = seconds;
            ResendCodeButton.IsEnabled = false;
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
                ResendCodeButton.IsEnabled = true;
                ResendCodeButton.Opacity = 0.7;
                CountdownText.Visibility = Visibility.Collapsed;
            }
        }

        private void ResendCodeButton_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Lý tưởng nhất, việc này nên gọi một Command trong ViewModel
            MessageBox.Show("Đang gửi lại mã xác minh...");
            StartCountdown(60);
        }

        // --- Logic xử lý các ô OTP (ĐÃ SỬA) ---
        private void OtpBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var currentTextBox = sender as TextBox;
            if (currentTextBox == null) return;

            // --- PHẦN 1: CẬP NHẬT VIEWMODEL (ĐÃ THÊM) ---
            UpdateViewModelWithCombinedOtp();

            // --- PHẦN 2: TỰ ĐỘNG CHUYỂN FOCUS (Code của bạn) ---
            if (currentTextBox.Text.Length != 1)
                return;

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

            // Xử lý phím Backspace (Code của bạn)
            if (e.Key == Key.Back && currentTextBox.Text.Length == 0)
            {
                if (currentTextBox == OtpBox4)
                    OtpBox3.Focus();
                else if (currentTextBox == OtpBox3)
                    OtpBox2.Focus();
                else if (currentTextBox == OtpBox2)
                    OtpBox1.Focus();
            }

            // --- CẬP NHẬT VIEWMODEL SAU KHI XÓA (ĐÃ THÊM) ---
            // Dùng Dispatcher để chạy sau khi phím Backspace đã thực sự xóa ký tự
            Dispatcher.BeginInvoke(new Action(UpdateViewModelWithCombinedOtp), DispatcherPriority.ContextIdle);
        }

        // --- HÀM "CẦU NỐI" MỚI ---
        private void UpdateViewModelWithCombinedOtp()
        {
            // 1. Lấy ViewModel từ DataContext
            var viewModel = this.DataContext as OtpVerificationViewModel;
            if (viewModel == null) return;

            // 2. Gộp giá trị từ 4 ô và gán vào ViewModel
            viewModel.EnteredOtp = OtpBox1.Text + OtpBox2.Text + OtpBox3.Text + OtpBox4.Text;
        }


        // --- Logic điều khiển cửa sổ (Giữ nguyên code của bạn) ---
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed) // Sửa lại điều kiện
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