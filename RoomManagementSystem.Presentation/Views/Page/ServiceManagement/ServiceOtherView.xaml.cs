using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using RoomManagementSystem.BusinessLayer;
using RoomManagementSystem.Presentation.ViewModels;

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
            if (ParkingCostTextBox != null) ParkingCostTextBox.TextChanged += OnInputsChanged;
            if (LateFeeCostTextBox != null) LateFeeCostTextBox.TextChanged += OnInputsChanged;

            this.Loaded += (s, e) =>
            {
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
            double total = 0;
            if (InternetYesRadio != null && InternetYesRadio.IsChecked == true)
                total += Math.Max(0, TryParseDoubleOrZero(InternetCostTextBox.Text));
            if (CleaningYesRadio != null && CleaningYesRadio.IsChecked == true)
                total += Math.Max(0, TryParseDoubleOrZero(CleaningCostTextBox.Text));
            if (TrashYesRadio != null && TrashYesRadio.IsChecked == true)
                total += Math.Max(0, TryParseDoubleOrZero(TrashCostTextBox.Text));
            if (ParkingYesRadio != null && ParkingYesRadio.IsChecked == true)
                total += Math.Max(0, TryParseDoubleOrZero(ParkingCostTextBox.Text));
            if (LateFeeYesRadio != null && LateFeeYesRadio.IsChecked == true)
                total += Math.Max(0, TryParseDoubleOrZero(LateFeeCostTextBox.Text));

            if (this.DataContext is ServiceManagementViewModel vm)
            {
                total += (double)vm.MaintenanceFeePreview;
            }

            if (TotalText != null)
            {
                TotalText.Text = string.Format(CultureInfo.GetCultureInfo("vi-VN"), "{0:n0} VND", total);
            }
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

        private void OnSelectionChanged(object sender, RoutedEventArgs e) => UpdateSummary();

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var vm = this.DataContext as ServiceManagementViewModel;
                string maPhong = vm?.SelectedPhong?.MaPhong?.Trim();
                string thoiKy = ThoiKyTextBox?.Text?.Trim();

                if (string.IsNullOrWhiteSpace(maPhong))
                {
                    MessageBox.Show("Vui lòng chọn phòng trước khi lưu.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                if (string.IsNullOrWhiteSpace(thoiKy))
                {
                    MessageBox.Show("Vui lòng nhập Thời kỳ (MM/yyyy).", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Lấy dữ liệu tiền (nullable)
                decimal? internetVal = InternetYesRadio.IsChecked == true ? TryParseDecimalNullable(InternetCostTextBox.Text) : null;
                decimal? racVal = TrashYesRadio.IsChecked == true ? TryParseDecimalNullable(TrashCostTextBox.Text) : null;
                decimal? treHanVal = LateFeeYesRadio.IsChecked == true ? TryParseDecimalNullable(LateFeeCostTextBox.Text) : null;
                decimal? guiXeVal = ParkingYesRadio.IsChecked == true ? TryParseDecimalNullable(ParkingCostTextBox.Text) : null;
                // Lấy tiền máy giặt
                decimal? mayGiatVal = CleaningYesRadio.IsChecked == true ? TryParseDecimalNullable(CleaningCostTextBox.Text) : null;

                // Lấy dữ liệu bảo trì
                var mgr = new ServiceManager();
                mgr.SaveOtherServices(maPhong, thoiKy, internetVal, racVal, guiXeVal, mayGiatVal, treHanVal);                
                // Refresh lại ViewModel để cập nhật hiển thị nếu cần
                (this.DataContext as ServiceManagementViewModel)?.LoadMaintenanceInfo();

                MessageBox.Show($"Đã lưu dịch vụ cho phòng {maPhong} kỳ {thoiKy}.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi lưu dịch vụ: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private static decimal? TryParseDecimalNullable(string? s)
        {
            if (string.IsNullOrWhiteSpace(s)) return null;
            if (decimal.TryParse(s, NumberStyles.Number, CultureInfo.InvariantCulture, out var v)) return v;
            if (decimal.TryParse(s, NumberStyles.Number, CultureInfo.GetCultureInfo("vi-VN"), out v)) return v;
            return null;
        }
    }
}