using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using RoomManagementSystem.BusinessLayer;
using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Windows;

namespace RoomManagementSystem.Presentation.ViewModels
{
    public class ExpenseItem
    {
        public int STT { get; set; }
        public string DonViChi { get; set; }
        public decimal SoTien { get; set; }
    }

    public partial class ReportMonthlyExpenseViewModel : ObservableObject
    {
        private readonly BaoCaoChiPhiBLL _expenseService;

        [ObservableProperty]
        private ObservableCollection<ExpenseItem> _expenseData = new();

        [ObservableProperty]
        private List<int> _months;

        [ObservableProperty]
        private List<int> _years;

        [ObservableProperty]
        private int _selectedMonth;

        [ObservableProperty]
        private int _selectedYear;

        private string _currentPeriodDisplay; // để hiển thị MM/yyyy và đặt tên file

        // Trong file: ReportMonthlyExpenseViewModel.cs

        // Trong file: ReportMonthlyExpenseViewModel.cs
        public ReportMonthlyExpenseViewModel()
        {
            _expenseService = new BaoCaoChiPhiBLL();

            Months = Enumerable.Range(1, 12).ToList();
            Years = Enumerable.Range(DateTime.Now.Year - 5, 10).ToList();

            var previousMonthDate = DateTime.Now.AddMonths(-1);

            // GÁN TRƯỚC
            SelectedMonth = previousMonthDate.Month;
            SelectedYear = previousMonthDate.Year;

            // SAU ĐÓ MỚI LOAD
            LoadData(SelectedMonth, SelectedYear);
        }

        partial void OnSelectedMonthChanged(int value)
        {
            LoadData(SelectedMonth, SelectedYear);
        }

        partial void OnSelectedYearChanged(int value)
        {
            LoadData(SelectedMonth, SelectedYear);
        }

        private void LoadData(int month, int year)
        {
            try
            {
                if (month < 1 || month > 12) return;

                // ❌ Bỏ kiểm tra năm — KHÔNG CẦN THIẾT
                // if (!Years.Contains(year)) return;

                _currentPeriodDisplay = $"{month:00}/{year}";

                DataTable dt = _expenseService.GetChiPhiThang(month, year);

                var items = new ObservableCollection<ExpenseItem>();
                decimal total = 0;

                if (dt != null)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        var item = new ExpenseItem
                        {
                            STT = row.Table.Columns.Contains("STT") && row["STT"] != DBNull.Value ? Convert.ToInt32(row["STT"]) : 0,
                            DonViChi = row.Table.Columns.Contains("LoaiChiPhi") ? row["LoaiChiPhi"].ToString() : string.Empty,
                            SoTien = row.Table.Columns.Contains("SoTien") && row["SoTien"] != DBNull.Value ? Convert.ToDecimal(row["SoTien"]) : 0
                        };
                        total += item.SoTien;
                        items.Add(item);
                    }
                }

                items.Add(new ExpenseItem { DonViChi = "Tổng", SoTien = total });
                ExpenseData = items;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải báo cáo chi phí: {ex.Message}");
            }
        }

        [RelayCommand]
        private void Download()
        {
            if (ExpenseData == null || ExpenseData.Count == 0)
            {
                MessageBox.Show("Không có dữ liệu để xuất file.", "Thông báo");
                return;
            }

            try
            {
                var saveFileDialog = new SaveFileDialog
                {
                    Filter = "Excel Workbook|*.xlsx",
                    Title = "Lưu báo cáo chi phí",
                    FileName = $"BaoCaoChiPhi_{_currentPeriodDisplay.Replace('/', '_')}.xlsx"
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    if (_expenseService.ExportExcel(SelectedMonth, SelectedYear, saveFileDialog.FileName))
                        MessageBox.Show($"Đã xuất file thành công!\nĐường dẫn: {saveFileDialog.FileName}", "Thành công");
                    else
                        MessageBox.Show("Xuất file thất bại, không có dữ liệu.", "Lỗi");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi xuất file Excel: {ex.Message}");
            }
        }
    }
}
