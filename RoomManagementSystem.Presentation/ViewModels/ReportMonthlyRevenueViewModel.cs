using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32; // Cần cho SaveFileDialog
using RoomManagementSystem.BusinessLayer;
using System;
using System.Data;
using System.Windows;
using System.Collections.Generic;
using System.Linq;

namespace RoomManagementSystem.Presentation.ViewModels
{
    public partial class ReportMonthlyRevenueViewModel : ObservableObject
    {
        private readonly QuanLyDoanhThuThang _revenueService;

        [ObservableProperty]
        private DataView _revenueData;

        [ObservableProperty] private List<int> _months;
        [ObservableProperty] private List<int> _years;
        [ObservableProperty] private int _selectedMonth;
        [ObservableProperty] private int _selectedYear;

        private string _currentPeriodDisplay; // MM/yyyy dùng để đặt tên file

        public ReportMonthlyRevenueViewModel()
        {
            _revenueService = new QuanLyDoanhThuThang();

            Months = Enumerable.Range(1, 12).ToList();
            Years = Enumerable.Range(DateTime.Now.Year - 5, 10).ToList();

            var previousMonthDate = DateTime.Now.AddMonths(-1);
            SelectedMonth = previousMonthDate.Month;
            SelectedYear = previousMonthDate.Year;

            _currentPeriodDisplay = $"{SelectedMonth:00}/{SelectedYear}";
            LoadData(SelectedMonth, SelectedYear);
        }

        private void LoadData(int month, int year)
        {
            try
            {
                _currentPeriodDisplay = $"{month:00}/{year}";

                DataTable dt = _revenueService.LayBaoCaoThang(month, year);
                RevenueData = dt.DefaultView;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải báo cáo doanh thu: {ex.Message}");
            }
        }

        partial void OnSelectedMonthChanged(int value)
        {
            _currentPeriodDisplay = $"{SelectedMonth:00}/{SelectedYear}";
            LoadData(SelectedMonth, SelectedYear);
        }

        partial void OnSelectedYearChanged(int value)
        {
            _currentPeriodDisplay = $"{SelectedMonth:00}/{SelectedYear}";
            LoadData(SelectedMonth, SelectedYear);
        }

        [RelayCommand]
        private void Download()
        {
            if (RevenueData == null || RevenueData.ToTable().Rows.Count == 0)
            {
                MessageBox.Show("Không có dữ liệu để xuất file.", "Thông báo");
                return;
            }

            try
            {
                var saveFileDialog = new SaveFileDialog
                {
                    Filter = "Excel Workbook|*.xlsx",
                    Title = "Lưu báo cáo doanh thu",
                    FileName = $"BaoCaoDoanhThu_{_currentPeriodDisplay.Replace('/', '_')}.xlsx"
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    DataTable dtToExport = RevenueData.ToTable();
                    _revenueService.ExportToExcel(dtToExport, saveFileDialog.FileName, "DoanhThuThang");
                    MessageBox.Show($"Đã xuất file thành công!\nĐường dẫn: {saveFileDialog.FileName}", "Thành công");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi xuất file Excel: {ex.Message}");
            }
        }
    }
}
