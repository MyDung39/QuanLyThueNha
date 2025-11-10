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
            // New services
            if (MaintenanceCostTextBox != null) MaintenanceCostTextBox.TextChanged += OnInputsChanged;
            if (ParkingCostTextBox != null) ParkingCostTextBox.TextChanged += OnInputsChanged;
            if (LateFeeCostTextBox != null) LateFeeCostTextBox.TextChanged += OnInputsChanged;
            this.Loaded += (s, e) =>
            {
                // Tự điền thời kỳ tháng hiện tại nếu trống
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

            // Internet
            if (InternetYesRadio != null && InternetYesRadio.IsChecked == true)
            {
                total += Math.Max(0, TryParseDoubleOrZero(InternetCostTextBox.Text));
            }

            // Cleaning
            if (CleaningYesRadio != null && CleaningYesRadio.IsChecked == true)
            {
                total += Math.Max(0, TryParseDoubleOrZero(CleaningCostTextBox.Text));
            }

            // Trash
            if (TrashYesRadio != null && TrashYesRadio.IsChecked == true)
            {
                total += Math.Max(0, TryParseDoubleOrZero(TrashCostTextBox.Text));
            }

            // Maintenance
            if (MaintenanceYesRadio != null && MaintenanceYesRadio.IsChecked == true)
            {
                total += Math.Max(0, TryParseDoubleOrZero(MaintenanceCostTextBox.Text));
            }

            // Parking
            if (ParkingYesRadio != null && ParkingYesRadio.IsChecked == true)
            {
                total += Math.Max(0, TryParseDoubleOrZero(ParkingCostTextBox.Text));
            }

            // Late fee
            if (LateFeeYesRadio != null && LateFeeYesRadio.IsChecked == true)
            {
                total += Math.Max(0, TryParseDoubleOrZero(LateFeeCostTextBox.Text));
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

        private void OnSelectionChanged(object sender, RoutedEventArgs e)
        {
            UpdateSummary();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Lấy đầu vào từ ViewModel để đảm bảo đúng phòng được chọn
                var vm = this.DataContext as ServiceManagementViewModel;
                string maPhong = vm?.SelectedPhong?.MaPhong?.Trim();
                string thoiKy = ThoiKyTextBox?.Text?.Trim(); // MM/yyyy

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

                decimal? internet = InternetYesRadio.IsChecked == true ? TryParseDecimalNullable(InternetCostTextBox.Text) : null;
                decimal? rac = TrashYesRadio.IsChecked == true ? TryParseDecimalNullable(TrashCostTextBox.Text) : null;
                decimal? baoTri = MaintenanceYesRadio.IsChecked == true ? TryParseDecimalNullable(MaintenanceCostTextBox.Text) : null;
                decimal? treHan = LateFeeYesRadio.IsChecked == true ? TryParseDecimalNullable(LateFeeCostTextBox.Text) : null;
                decimal? guiXe = ParkingYesRadio.IsChecked == true ? TryParseDecimalNullable(ParkingCostTextBox.Text) : null;

                var mgr = new ServiceManager();
                mgr.SaveServiceCosts(maPhong, thoiKy, null, null, internet, rac, guiXe, baoTri, treHan);

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
