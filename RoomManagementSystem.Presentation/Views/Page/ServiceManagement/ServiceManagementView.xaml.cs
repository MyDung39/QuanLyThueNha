using System.Windows.Controls;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

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

    // Local converter to avoid XAML namespace resolution issues
    public class SelectedRoomToFontWeightConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || values.Length < 2)
                return FontWeights.Medium;

            var currentMaPhong = values[0]?.ToString();
            var selectedMaPhong = values[1]?.ToString();
            if (!string.IsNullOrEmpty(currentMaPhong) && !string.IsNullOrEmpty(selectedMaPhong) &&
                string.Equals(currentMaPhong, selectedMaPhong, StringComparison.OrdinalIgnoreCase))
            {
                return FontWeights.Bold;
            }
            return FontWeights.Medium;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
