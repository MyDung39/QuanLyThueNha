using System;
using System.Windows;
using System.Windows.Controls;

namespace RoomManagementSystem.Presentation.Views.Shared
{
    /// <summary>
    /// Interaction logic for SidebarSmallView.xaml
    /// </summary>
    public partial class SidebarSmallView : UserControl
    {
        public SidebarSmallView()
        {
            InitializeComponent();
        }

        public event EventHandler<string> NavigationRequested;

        private void RaiseNav(string key)
        {
            NavigationRequested?.Invoke(this, key);
        }

        private void OnDashboardChecked(object sender, RoutedEventArgs e) => RaiseNav("dashboard");
        private void OnTenantsChecked(object sender, RoutedEventArgs e) => RaiseNav("tenants");
        private void OnServicesChecked(object sender, RoutedEventArgs e) => RaiseNav("services");

        public void SetActive(string key)
        {
            switch (key)
            {
                case "dashboard":
                    if (FindName("DashboardItem") is RadioButton rbDash) rbDash.IsChecked = true;
                    break;
                case "tenants":
                    if (FindName("TenantsItem") is RadioButton rbTenants) rbTenants.IsChecked = true;
                    break;
                case "services":
                    if (FindName("ServicesItem") is RadioButton rbServices) rbServices.IsChecked = true;
                    break;
            }
        }
    }
}