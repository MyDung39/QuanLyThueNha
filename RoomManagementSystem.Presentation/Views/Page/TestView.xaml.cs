using RoomManagementSystem.Presentation.Views.Page.TenantManagement;
using RoomManagementSystem.Presentation.Views.Page.ServiceManagement;
using RoomManagementSystem.Presentation.Views;
using System.Windows;
using System.Windows.Controls;

namespace RoomManagementSystem.Presentation.Views.Page
{
    public partial class TestView : UserControl
    {
        public TestView()
        {
            InitializeComponent();
            // Default content
            Loaded += (s, e) =>
            {
                // Load default Tenants view
                MainContent.Children.Clear();
                MainContent.Children.Add(new TenantManagementView());

                // Hook sidebars navigation
                FullSidebar.NavigationRequested += Sidebar_NavigationRequested;
                SmallSidebar.NavigationRequested += Sidebar_NavigationRequested;

                // Set active highlight
                SetActiveMenu("tenants");

                // Additionally, hook inner RadioButtons to be extra-safe
                HookInnerSidebarButtons();
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

        private void HookInnerSidebarButtons()
        {
            void TryHook(object ctrl, string key)
            {
                if (ctrl is RadioButton rb)
                {
                    rb.Checked += (_, __) => { NavigateTo(key); SetActiveMenu(key); };
                    rb.PreviewMouseLeftButtonUp += (_, __) => { NavigateTo(key); SetActiveMenu(key); };
                }
            }

            // Names defined in the sidebars XAML
            TryHook(FullSidebar.FindName("DashboardItem"), "dashboard");
            TryHook(FullSidebar.FindName("TenantsItem"), "tenants");
            TryHook(FullSidebar.FindName("ServicesItem"), "services");

            TryHook(SmallSidebar.FindName("DashboardItem"), "dashboard");
            TryHook(SmallSidebar.FindName("TenantsItem"), "tenants");
            TryHook(SmallSidebar.FindName("ServicesItem"), "services");
        }
    }
}

