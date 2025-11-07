using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RoomManagementSystem.BusinessLayer; // Cần cái này để biết 'DangNhap'
using RoomManagementSystem.Presentation.Views.Windows; // Cần cho LoginWindow, ResetPasswordWindow
using System.Linq;
using System.Windows;

namespace RoomManagementSystem.Presentation.ViewModels
{
    public partial class OtpVerificationViewModel : ViewModelBase
    {
        // 1. Biến để lưu dịch vụ DangNhap
        // Dịch vụ này được truyền từ ForgotPasswordViewModel
        private readonly DangNhap _dangNhapService;

        // 2. Thuộc tính (Property) để Binding với TextBox nhập OTP
        [ObservableProperty]
        private string _enteredOtp = string.Empty;

        // 3. Constructor (Hàm khởi tạo)
        // BẮT BUỘC phải có constructor này để nhận dịch vụ DangNhap
        public OtpVerificationViewModel(DangNhap dangNhapService)
        {
            _dangNhapService = dangNhapService;
        }

        /// <summary>
        /// Lệnh này được gọi khi người dùng nhấn nút "Xác nhận"
        /// </summary>
        [RelayCommand]
        private void VerifyOtp()
        {
            if (string.IsNullOrWhiteSpace(EnteredOtp))
            {
                MessageBox.Show("Vui lòng nhập mã OTP!");
                return;
            }

            // 4. GỌI HÀM CHECKOTP (ĐÂY LÀ PHẦN BẠN CẦN)
            // (Đảm bảo DangNhap.cs của bạn đã sửa CheckOTP chỉ còn 1 tham số)
            bool isOtpCorrect = _dangNhapService.CheckOTP(EnteredOtp);

            if (isOtpCorrect)
            {
                // 5. NẾU ĐÚNG: Mở cửa sổ Đặt lại Mật khẩu (Bước 3)
                // Nó sẽ dùng code ResetPasswordViewModel mà bạn vừa gửi
                var resetVm = new ResetPasswordViewModel(_dangNhapService);
                var resetWindow = new ResetPasswordWindow
                {
                    DataContext = resetVm
                };
                resetWindow.Show();

                // Đóng cửa sổ OTP hiện tại
                CloseCurrentWindow();
            }
            else
            {
                // 6. NẾU SAI: Báo lỗi
                MessageBox.Show("Mã OTP không chính xác!");
            }
        }

        /// <summary>
        /// Lệnh này được gọi khi người dùng nhấn nút "Quay lại"
        /// </summary>
        [RelayCommand]
        private void BackToLogin()
        {
            var loginWindow = new LoginWindow();
            loginWindow.Show();
            CloseCurrentWindow();
        }

        /// <summary>
        /// Hàm hỗ trợ để đóng cửa sổ hiện tại
        /// </summary>
        private void CloseCurrentWindow()
        {
            Application.Current.Windows
                .OfType<Window>()
                .FirstOrDefault(w => w.DataContext == this)?
                .Close();
        }
    }
}