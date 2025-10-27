using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RoomManagementSystem.BusinessLayer; // Service
using RoomManagementSystem.Presentation.ViewModels; // Các VM con
using System.Windows;

namespace RoomManagementSystem.Presentation.ViewModels.Windows
{
    public partial class TestWindowViewModel : ViewModelBase
    {
        // 1. Thuộc tính chứa ViewModel hiện tại
        [ObservableProperty]
        private object _currentViewModel;

        // 2. Khai báo các service
        private readonly QL_TaiSan_Phong _taiSanPhongService;
        // (Thêm các service khác nếu UserManagementViewModel cần)

        public TestWindowViewModel()
        {
            // 3. Khởi tạo các service
            _taiSanPhongService = new QL_TaiSan_Phong();

            // 4. Khởi tạo ViewModel mặc định
            CurrentViewModel = new UserManagementViewModel();
        }

        // 5. Command điều hướng (tự động tạo 'NavigateCommand')
        [RelayCommand]
        private void Navigate(string pageKey)
        {
            if (string.IsNullOrEmpty(pageKey)) return;

            switch (pageKey)
            {
                case "Tenant":
                    CurrentViewModel = new UserManagementViewModel();
                    break;
                case "Billing":
                    // Truyền service vào BillingViewModel
                    CurrentViewModel = new BillingViewModel(_taiSanPhongService);
                    break;
                case "Contract":
                    // CurrentViewModel = new ContractViewModel();
                    MessageBox.Show("Trang Hợp đồng (chưa tạo)");
                    break;
                case "Overview":
                    MessageBox.Show("Trang Tổng quan (chưa tạo)");
                    break;
                case "Asset":
                    MessageBox.Show("Trang Tài sản (chưa tạo)");
                    break;
            }
        }
    }
}