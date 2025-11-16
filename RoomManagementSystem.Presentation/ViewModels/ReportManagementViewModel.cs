using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows.Controls; // Cần cho TextBlock

namespace RoomManagementSystem.Presentation.ViewModels
{
    public partial class ReportManagementViewModel : ObservableObject
    {
        // ✅ Thuộc tính để giữ View/ViewModel hiện tại đang được hiển thị
        
        [ObservableProperty]
        private object _currentReportViewModel;

        public ReportManagementViewModel()
        {
            // Hiển thị tab Doanh thu làm mặc định khi khởi động
            ChangeTab("Doanh thu");
        }

        // ✅ Command để chuyển đổi giữa các tab
        [RelayCommand]
        private void ChangeTab(string tabName)
        {
            switch (tabName)
            {
                case "Doanh thu":
                    // Tạo một instance của ViewModel con và gán cho View hiện tại
                    CurrentReportViewModel = new ReportMonthlyRevenueViewModel();
                    break;
                case "Chi phí":
                    CurrentReportViewModel = new ReportMonthlyExpenseViewModel();
                    break;
                case "Lợi nhuận":
                    CurrentReportViewModel = new ReportMonthlyProfitViewModel();
                    break;
                case "Danh sách phòng":
                    CurrentReportViewModel = new ReportRoomListViewModel();
                    break;
                case "Công nợ":
                    CurrentReportViewModel = new ReportDebtListViewModel();
                    break;
                default:
                    CurrentReportViewModel = null;
                    break;
            }
        }
    }
}