using RoomManagementSystem.Presentation.Views.Page.TenantManagement;
using RoomManagementSystem.Presentation.ViewModels.Windows;
using System;
using System.Windows;

namespace RoomManagementSystem.Presentation.Views.Windows
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            // Thiết lập DataContext để Sidebar và nội dung chính hoạt động theo ViewModel
            DataContext = new TestWindowViewModel();
        }

        // === LOGIC XỬ LÝ SỰ KIỆN TỪ HEADERVIEW ===
        private void HeaderView_ToggleSidebarClick(object sender, RoutedEventArgs e)
        {
            // Kiểm tra xem sidebar lớn có đang hiển thị không
            if (FullSidebar.Visibility == Visibility.Visible)
            {
                // Nếu có, thì ẩn nó đi và hiện sidebar nhỏ
                FullSidebar.Visibility = Visibility.Collapsed;
                SmallSidebar.Visibility = Visibility.Visible;
                // Thu nhỏ độ rộng của cột chứa sidebar
                SidebarColumn.Width = new GridLength(59);
            }
            else
            {
                // Nếu không, thì làm ngược lại
                FullSidebar.Visibility = Visibility.Visible;
                SmallSidebar.Visibility = Visibility.Collapsed;
                // Mở rộng lại độ rộng của cột
                SidebarColumn.Width = new GridLength(240);
            }
        }
    }
}