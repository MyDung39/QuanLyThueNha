using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using RoomManagementSystem.BusinessLayer;
using ScottPlot.TickGenerators.Financial;
using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Windows;

namespace RoomManagementSystem.Presentation.ViewModels
{

    public class ProfitItemViewModel
    {
        public int STT { get; set; }
        public string MaPhong { get; set; }
        public decimal DoanhThu { get; set; }
        public decimal ChiPhi { get; set; }

        // Tự tính lợi nhuận
        public decimal LoiNhuan => DoanhThu - ChiPhi;

        // Đánh dấu dòng tổng
        public bool IsSummary { get; set; }
    }


    public partial class ReportMonthlyProfitViewModel : ObservableObject
    {
        private readonly LoiNhuanBL _profitService;

        [ObservableProperty]
        private ObservableCollection<ProfitItemViewModel> _profitData = new();

        [ObservableProperty]
        private List<int> _months;

        [ObservableProperty]
        private List<int> _years;

        [ObservableProperty]
        private int _selectedMonth;

        [ObservableProperty]
        private int _selectedYear;

        private string _currentPeriodDisplay; // dùng để hiển thị MM/yyyy và đặt tên file Excel

        public ReportMonthlyProfitViewModel()
        {
            _profitService = new LoiNhuanBL();

            // Tạo danh sách tháng/năm
            Months = Enumerable.Range(1, 12).ToList();
            Years = Enumerable.Range(DateTime.Now.Year - 5, 10).ToList();

            var previousMonthDate = DateTime.Now.AddMonths(-1);

            SelectedMonth = previousMonthDate.Month;
            SelectedYear = previousMonthDate.Year;

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

                _currentPeriodDisplay = $"{month:00}/{year}";

                DataTable dt = _profitService.GetLoiNhuanThang(_currentPeriodDisplay);

                var items = new ObservableCollection<ProfitItemViewModel>();
                decimal totalDoanhThu = 0;
                decimal totalChiPhi = 0;
                int stt = 1;

                if (dt != null)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        var item = new ProfitItemViewModel
                        {
                            STT = stt++,
                            MaPhong = row["MaPhong"].ToString(),
                            DoanhThu = row["DoanhThu"] != DBNull.Value ? Convert.ToDecimal(row["DoanhThu"]) : 0,
                            ChiPhi = row["ChiPhi"] != DBNull.Value ? Convert.ToDecimal(row["ChiPhi"]) : 0
                        };
                        totalDoanhThu += item.DoanhThu;
                        totalChiPhi += item.ChiPhi;
                        items.Add(item);
                    }
                }

                // Thêm dòng tổng
                items.Add(new ProfitItemViewModel
                {
                    STT = 0,
                    MaPhong = "TỔNG",
                    DoanhThu = totalDoanhThu,
                    ChiPhi = totalChiPhi,
                    IsSummary = true
                });

                ProfitData = items;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải báo cáo lợi nhuận: {ex.Message}");
            }
        }

        [RelayCommand]
        private void Download()
        {
            if (ProfitData == null || ProfitData.Count == 0)
            {
                MessageBox.Show("Không có dữ liệu để xuất file.", "Thông báo");
                return;
            }

            try
            {
                var saveFileDialog = new SaveFileDialog
                {
                    Filter = "Excel Workbook|*.xlsx",
                    Title = "Lưu báo cáo lợi nhuận",
                    FileName = $"BaoCaoLoiNhuan_{_currentPeriodDisplay.Replace('/', '_')}.xlsx"
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    if (_profitService.XuatExcel(saveFileDialog.FileName, _currentPeriodDisplay))
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
