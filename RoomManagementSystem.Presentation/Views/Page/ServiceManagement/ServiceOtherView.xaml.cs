using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;

namespace RoomManagementSystem.Presentation.Views.Page.ServiceManagement
{
    public partial class ServiceOtherView : UserControl
    {
        public ServiceOtherView()
        {
            InitializeComponent();
            InternetCostTextBox.TextChanged += OnInputsChanged;
            CleaningCostTextBox.TextChanged += OnInputsChanged;
            TrashCostTextBox.TextChanged += OnInputsChanged;
            UpdateSummary();
        }

        private void OnInputsChanged(object sender, TextChangedEventArgs e) => UpdateSummary();

        private void UpdateSummary()
        {
            double internet = TryParseDoubleOrZero(InternetCostTextBox.Text);
            double cleaning = TryParseDoubleOrZero(CleaningCostTextBox.Text);
            double trash = TryParseDoubleOrZero(TrashCostTextBox.Text);
            double total = Math.Max(0, internet) + Math.Max(0, cleaning) + Math.Max(0, trash);
            TotalText.Text = string.Format(CultureInfo.GetCultureInfo("vi-VN"), "{0:n0} VND", total);
        }

        private static bool TryParseDouble(string? s, out double value)
        {
            return double.TryParse(s, NumberStyles.Number, CultureInfo.InvariantCulture, out value)
                || double.TryParse(s, NumberStyles.Number, CultureInfo.GetCultureInfo("vi-VN"), out value);
        }

        private static double TryParseDoubleOrZero(string? s)
        {
            return TryParseDouble(s, out var v) ? v : 0;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Đã lưu dịch vụ khác thành công.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
