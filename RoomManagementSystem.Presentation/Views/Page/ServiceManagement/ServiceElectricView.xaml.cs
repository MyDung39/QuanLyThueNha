using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using RoomManagementSystem.BusinessLayer;
using RoomManagementSystem.Presentation.ViewModels;

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
                // Nếu chưa nhập thời kỳ, tự điền tháng hiện tại MM/yyyy
                if (ThoiKyTextBox != null && string.IsNullOrWhiteSpace(ThoiKyTextBox.Text))
                {
                    ThoiKyTextBox.Text = DateTime.Now.ToString("MM/yyyy");
                }
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
            try
            {
                var vm = this.DataContext as ServiceManagementViewModel;
                string maPhong = vm?.SelectedPhong?.MaPhong?.Trim();
                string thoiKy = ThoiKyTextBox?.Text?.Trim();

                if (string.IsNullOrWhiteSpace(maPhong))
                {
                    MessageBox.Show("Vui lòng chọn hoặc nhập Mã phòng.");
                    return;
                }
                if (string.IsNullOrWhiteSpace(thoiKy))
                {
                    MessageBox.Show("Vui lòng nhập Thời kỳ (MM/yyyy).");
                    return;
                }

                // Validate chỉ số điện cũ
                if (!TryParseDouble(OldIndexTextBox.Text, out double oldIdx) || oldIdx < 0)
                {
                    MessageBox.Show("Chỉ số điện cũ phải là số không âm!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                // Validate chỉ số điện mới
                if (!TryParseDouble(NewIndexTextBox.Text, out double newIdx) || newIdx < 0)
                {
                    MessageBox.Show("Chỉ số điện mới phải là số không âm!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                // Validate đơn giá
                if (!TryParseDouble(UnitPriceTextBox.Text, out double unitPrice) || unitPrice <= 0)
                {
                    MessageBox.Show("Đơn giá phải là số lớn hơn 0!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                // Validate logic: chỉ số mới >= chỉ số cũ
                if (newIdx < oldIdx)
                {
                    MessageBox.Show("Chỉ số điện mới phải lớn hơn hoặc bằng chỉ số cũ!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                double consumption = newIdx - oldIdx;
                double total = consumption * unitPrice;

                var mgr = new ServiceManager();
                mgr.SaveServiceCosts(
                    maPhong,
                    thoiKy,
                    dien: Convert.ToDecimal(total),
                    nuoc: null,
                    internet: null,
                    rac: null,
                    guiXe: null,
                    baoTri: null,
                    treHan: null);

                MessageBox.Show($"Đã lưu tiền điện cho phòng {maPhong} kỳ {thoiKy}.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi lưu điện: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void OldIndexTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
