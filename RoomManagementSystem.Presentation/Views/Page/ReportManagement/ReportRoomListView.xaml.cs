using RoomManagementSystem.Presentation.ViewModels;
using System.Windows.Controls;

// ✅ THÊM LẠI NAMESPACE CHO ĐÚNG VỚI KHAI BÁO x:Class TRONG XAML
namespace RoomManagementSystem.Presentation.Views.Page.ReportManagement
{
    public partial class ReportRoomListView : UserControl
    {
        public ReportRoomListView()
        {
            InitializeComponent();
            // Bây giờ InitializeComponent() sẽ được tìm thấy
            DataContext = new ReportRoomListViewModel();
        }
    }
}