using RoomManagementSystem.Presentation.Views.Page.TenantManagement;
using RoomManagementSystem.Presentation.Views.Page.ServiceManagement;
using RoomManagementSystem.Presentation.Views;
using System;
using System.Windows;

namespace RoomManagementSystem.Presentation.Views.Windows
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            // Sử dụng Grid thay vì Frame để tránh lỗi navigation
            MainContent.Children.Add(new TenantManagementView());

            // Đăng ký sự kiện điều hướng từ sidebar
            FullSidebar.Loaded += (_, __) =>
            {
                FullSidebar.NavigationRequested += Sidebar_NavigationRequested;
                SetActiveMenu("tenants");
            };
            SmallSidebar.Loaded += (_, __) =>
            {
                SmallSidebar.NavigationRequested += Sidebar_NavigationRequested;
            };
        }

        private void Sidebar_NavigationRequested(object? sender, string key)
        {
            NavigateTo(key);
            SetActiveMenu(key);
        }

        private void NavigateTo(string key)
        {
            MainContent.Children.Clear();
            switch (key)
            {
                case "dashboard":
                    MainContent.Children.Add(new DashboardView());
                    break;
                case "services":
                    MainContent.Children.Add(new ServiceManagementView());
                    break;
                case "tenants":
                default:
                    MainContent.Children.Add(new TenantManagementView());
                    break;
            }
        }

        private void SetActiveMenu(string key)
        {
            FullSidebar.SetActive(key);
            SmallSidebar.SetActive(key);
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