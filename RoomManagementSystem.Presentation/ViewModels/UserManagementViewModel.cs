using CommunityToolkit.Mvvm.ComponentModel;
using RoomManagementSystem.BusinessLayer;
using RoomManagementSystem.DataLayer;
using System.Collections.ObjectModel;
using System.Linq;

// ĐẢM BẢO NAMESPACE NÀY KHỚP VỚI XAML
namespace RoomManagementSystem.Presentation.ViewModels
{
    // Kế thừa từ ViewModelBase (lớp ObservableObject của bạn)
    public partial class UserManagementViewModel : ViewModelBase
    {
        // Thêm logic cho User Management ở đây
        // Ví dụ:
        [ObservableProperty]
        private string _searchText;

        public UserManagementViewModel()
        {
            SearchText = "Đây là User Management View";
        }
    }
}