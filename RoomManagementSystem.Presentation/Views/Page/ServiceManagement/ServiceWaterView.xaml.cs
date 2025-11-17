using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using RoomManagementSystem.BusinessLayer;
using RoomManagementSystem.Presentation.ViewModels;

namespace RoomManagementSystem.Presentation.Views.Page.ServiceManagement
{
    public partial class ServiceWaterView : UserControl
    {
        public ServiceWaterView()
        {
            InitializeComponent();
            this.Loaded += (s, e) =>
            {
                if (OldIndexTextBox != null) OldIndexTextBox.TextChanged += OnInputsChanged;
                if (NewIndexTextBox != null) NewIndexTextBox.TextChanged += OnInputsChanged;
                if (UnitPriceTextBox != null) UnitPriceTextBox.TextChanged += OnInputsChanged;
                if (ThoiKyTextBox != null && string.IsNullOrWhiteSpace(ThoiKyTextBox.Text))
                {
                    ThoiKyTextBox.Text = DateTime.Now.ToString("MM/yyyy");
                }
                UpdateSummary();
            };
        }

        private void OnInputsChanged(object sender, TextChangedEventArgs e) => UpdateSummary();

        private void UpdateSummary()
        {
            if (OldIndexTextBox == null || NewIndexTextBox == null || UnitPriceTextBox == null ||
                ConsumptionText == null || TotalText == null)
                return;

            if (!TryParseDouble(OldIndexTextBox.Text, out double oldIdx)) oldIdx = 0;
            if (!TryParseDouble(NewIndexTextBox.Text, out double newIdx)) newIdx = oldIdx;
            if (!TryParseDouble(UnitPriceTextBox.Text, out double unitPrice)) unitPrice = 0;

            double consumption = Math.Max(0, newIdx - oldIdx);
            double total = consumption * unitPrice;

            ConsumptionText.Text = $"{consumption:0} m³";
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

                if (!TryParseDouble(OldIndexTextBox.Text, out double oldIdx) || oldIdx < 0)
                {
                    MessageBox.Show("Chỉ số nước cũ không hợp lệ!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                if (!TryParseDouble(NewIndexTextBox.Text, out double newIdx) || newIdx < 0)
                {
                    MessageBox.Show("Chỉ số nước mới không hợp lệ!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                if (!TryParseDouble(UnitPriceTextBox.Text, out double unitPrice) || unitPrice <= 0)
                {
                    MessageBox.Show("Đơn giá không hợp lệ!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                if (newIdx < oldIdx)
                {
                    MessageBox.Show("Chỉ số nước mới phải >= chỉ số cũ!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Tính toán
                decimal tieuThu = Convert.ToDecimal(newIdx - oldIdx);
                decimal gia = Convert.ToDecimal(unitPrice);

                // Gọi hàm chuyên biệt cho Nước
                var mgr = new ServiceManager();
                mgr.SaveWater(maPhong, thoiKy, tieuThu, gia);

                MessageBox.Show($"Đã lưu tiền nước cho phòng {maPhong} kỳ {thoiKy}.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi lưu nước: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}