using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;

namespace RoomManagementSystem.Presentation.Views.Page.ServiceManagement
{
    public partial class ServiceElectricView : UserControl
    {
        public ServiceElectricView()
        {
            InitializeComponent();
            this.Loaded += (s, e) =>
            {
                if (OldIndexTextBox != null) OldIndexTextBox.TextChanged += OnInputsChanged;
                if (NewIndexTextBox != null) NewIndexTextBox.TextChanged += OnInputsChanged;
                if (UnitPriceTextBox != null) UnitPriceTextBox.TextChanged += OnInputsChanged;
                UpdateSummary();
            };
        }

        private void OnInputsChanged(object sender, TextChangedEventArgs e)
        {
            UpdateSummary();
        }

        private void UpdateSummary()
        {
            if (OldIndexTextBox == null || NewIndexTextBox == null || UnitPriceTextBox == null ||
                ConsumptionText == null || TotalText == null)
                return;
            if (!TryParseDouble(OldIndexTextBox.Text, out double oldIdx))
                oldIdx = 0;
            if (!TryParseDouble(NewIndexTextBox.Text, out double newIdx))
                newIdx = oldIdx; // avoid negative when empty
            if (!TryParseDouble(UnitPriceTextBox.Text, out double unitPrice))
                unitPrice = 0;

            double consumption = Math.Max(0, newIdx - oldIdx);
            double total = consumption * unitPrice;

            ConsumptionText.Text = $"{consumption:0} kWh";
            TotalText.Text = string.Format(CultureInfo.GetCultureInfo("vi-VN"), "{0:n0} VND", total);
        }

        private static bool TryParseDouble(string? s, out double value)
        {
            return double.TryParse(s, NumberStyles.Number, CultureInfo.InvariantCulture, out value)
                || double.TryParse(s, NumberStyles.Number, CultureInfo.GetCultureInfo("vi-VN"), out value);
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Đã lưu chỉ số điện và tính tiền thành công.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void OldIndexTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
