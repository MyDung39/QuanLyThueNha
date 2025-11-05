using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RoomManagementSystem.BusinessLayer;
using RoomManagementSystem.DataLayer;
using System;
using System.Windows;

namespace RoomManagementSystem.Presentation.ViewModels
{
    public partial class AddTenantViewModel : ViewModelBase
    {
        private readonly QuanLyNguoiThue _nguoiThueService;

        [ObservableProperty] private string _hoTen;
        [ObservableProperty] private string _diaChiEmail;
        [ObservableProperty] private string _soDienThoai;
        [ObservableProperty] private DateTime? _ngayBatDauThue = DateTime.Now;

        public bool IsSaved { get; private set; } = false;

        // Dòng này rất quan trọng, nó là tín hiệu để báo cho ViewModel cha
        public event EventHandler? RequestClose;

        public AddTenantViewModel()
        {
            _nguoiThueService = new QuanLyNguoiThue();
        }

        [RelayCommand]
        private void AddTenant()
        {
            // ... (Logic thêm người thuê của bạn đã đúng)
            var nguoiThueMoi = new NguoiThue { /* ... */ };
            bool success = _nguoiThueService.ThemNguoiThue(nguoiThueMoi);
            if (success)
            {
                IsSaved = true;
                // Phát tín hiệu yêu cầu đóng
                RequestClose?.Invoke(this, EventArgs.Empty);
            }
            // ...
        }

        // Lệnh này được gọi bởi nút "X"
        [RelayCommand]
        private void Cancel()
        {
            IsSaved = false; // Người dùng đã hủy
            // Phát tín hiệu yêu cầu đóng
            RequestClose?.Invoke(this, EventArgs.Empty);
        }
    }
}