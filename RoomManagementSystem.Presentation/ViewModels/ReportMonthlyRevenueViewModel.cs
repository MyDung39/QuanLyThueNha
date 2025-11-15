using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32; // Cần cho SaveFileDialog
using RoomManagementSystem.BusinessLayer;
using System;
using System.Data;
using System.Windows;

namespace RoomManagementSystem.Presentation.ViewModels
{
    public partial class ReportMonthlyRevenueViewModel : ObservableObject
    {
        private readonly QuanLyDoanhThuThang _revenueService;

        // Thuộc tính để binding với DataGrid. Dùng DataView là tốt nhất cho WPF khi làm việc với DataTable.
        [ObservableProperty]
        private DataView _revenueData;




        [ObservableProperty] private List<int> _months;
        [ObservableProperty] private List<int> _years;
        [ObservableProperty] private int _selectedMonth;
        [ObservableProperty] private int _selectedYear;

        public ReportMonthlyRevenueViewModel()
        {
            _revenueService = new QuanLyDoanhThuThang();

            //LoadData();

            Months = Enumerable.Range(1, 12).ToList();
            Years = Enumerable.Range(DateTime.Now.Year - 5, 10).ToList();

            // Mặc định chọn tháng trước
            var previousMonthDate = DateTime.Now.AddMonths(-1);
            SelectedMonth = previousMonthDate.Month;
            SelectedYear = previousMonthDate.Year;
            LoadData(SelectedMonth, SelectedYear);
        }

        private void LoadData(int month, int year)
        {
            try
            {
                // Chỉ cần lấy DataTable và gán trực tiếp, không cần tính tổng
                DataTable dt = _revenueService.LayBaoCaoThang(month, year);
                RevenueData = dt.DefaultView;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải báo cáo doanh thu: {ex.Message}");
            }
        }



        // Xử lý khi người dùng chọn tháng mới
        partial void OnSelectedMonthChanged(int value)
        {
            LoadData(SelectedMonth, SelectedYear);
        }

        // Xử lý khi người dùng chọn năm mới
        partial void OnSelectedYearChanged(int value)
        {
            LoadData(SelectedMonth, SelectedYear);
        }


        // Command cho nút "Tải xuống"
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
                    FileName = $"BaoCaoDoanhThu_{DateTime.Now:yyyy_MM}.xlsx"
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    DataTable dtToExport = RevenueData.ToTable();
                    // Bỏ dòng tổng trước khi xuất nếu bạn muốn
                    if (dtToExport.Rows.Count > 0)
                    {
                        // dtToExport.Rows.RemoveAt(dtToExport.Rows.Count - 1);
                    }
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