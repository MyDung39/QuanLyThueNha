using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RoomManagementSystem.BusinessLayer;
using System.Windows;
namespace RoomManagementSystem.Presentation.ViewModels
{
    public partial class BillingViewModel : ViewModelBase
    {
        [RelayCommand]
        private void NavigateHome()
        {
            // Ví dụ tạm
            MessageBox.Show("Đi tới trang chính!");
        }

        [RelayCommand]
        private void ShowRooms()
        {
            MessageBox.Show("Hiển thị danh sách phòng!");
        }

        [RelayCommand]
        private void ShowBilling()
        {
            MessageBox.Show("Trang hóa đơn!");
        }

        [RelayCommand]
        private void ShowReports()
        {
            MessageBox.Show("Hiển thị báo cáo!");
        }

        [RelayCommand]
        private void Settings()
        {
            MessageBox.Show("Cài đặt hệ thống!");
        }

        [RelayCommand]
        private void Logout()
        {
            MessageBox.Show("Đăng xuất!");
        }
    }
}