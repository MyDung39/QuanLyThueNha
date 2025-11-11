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

        // 2. Khai báo các service ĐÚNG
        private readonly QL_TaiSan_Phong _taiSanPhongService;
        private readonly XuatBienLai _xuatBienLaiService; // <-- Đã khai báo service đúng

        public TestWindowViewModel()
        {
            // 3. Khởi tạo các service ĐÚNG
            _taiSanPhongService = new QL_TaiSan_Phong();
            _xuatBienLaiService = new XuatBienLai(); // <-- Đã khởi tạo service đúng

            // 4. Khởi tạo ViewModel mặc định (Dashboard)
            CurrentViewModel = new DashboardViewModel();
        }

        // 5. Command điều hướng (tự động tạo 'NavigateCommand')
        [RelayCommand]
        private void Navigate(string pageKey)
        {
            if (string.IsNullOrEmpty(pageKey)) return;

            switch (pageKey)
            {


                case "HouseManagement":
                    CurrentViewModel = new HouseManagementViewModel();
                    break;

                case "Tenant":
                    CurrentViewModel = new TenantManagementViewModel();
                    break;
                case "Billing":
                    // Truyền các service ĐÚNG vào BillingViewModel
                    //CurrentViewModel = new BillingViewModel(_taiSanPhongService, _xuatBienLaiService);
                    CurrentViewModel = new BillingViewModel();
                    break;
                case "Help":
                    CurrentViewModel = new HelpViewModel();
                    break;
                case "ServiceManagement":
                    CurrentViewModel = new ServiceManagementViewModel();
                    break;
                case "ReportManagement":
                    CurrentViewModel = new ReportManagementViewModel();
                    break;
                case "Contract":
                    // CurrentViewModel = new ContractViewModel();
                    CurrentViewModel = new ContractManagementViewModel();
                    break;
                case "Calendar":
                    CurrentViewModel = new CalendarViewModel();
                    break;
                case "Overview":
                    CurrentViewModel = new DashboardViewModel();
                    break;
                case "FinancialReport":
                    MessageBox.Show("Trang Báo cáo tài chính đang phát triển và sẽ có trong phiên bản sau.");
                    break;
                case "StatusReport":
                    MessageBox.Show("Trang Báo cáo trạng thái đang phát triển và sẽ có trong phiên bản sau.");
                    break;
                case "Maintenance":
                    CurrentViewModel = new MaintenanceManagementViewModel();
                    break;
            }
        }
    }
}