using System.Windows.Controls;

namespace RoomManagementSystem.Presentation.Views.Page.ServiceManagement
{
    public partial class ServiceManagementView : UserControl
    {
        private ServiceElectricView _electricView;
        private ServiceWaterView _waterView;
        private ServiceOtherView _otherView;

        public ServiceManagementView()
        {
            InitializeComponent();
            this.Loaded += (s, e) =>
            {
                _electricView ??= new ServiceElectricView();
                tabContentControl.Content = _electricView;
            };
        }

        private void ElectricTab_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            if (tabContentControl is null) return;
            _electricView ??= new ServiceElectricView();
            tabContentControl.Content = _electricView;
        }

        private void WaterTab_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            if (tabContentControl is null) return;
            _waterView ??= new ServiceWaterView();
            tabContentControl.Content = _waterView;
        }

        private void OtherTab_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            if (tabContentControl is null) return;
            _otherView ??= new ServiceOtherView();
            tabContentControl.Content = _otherView;
        }
    }
}
