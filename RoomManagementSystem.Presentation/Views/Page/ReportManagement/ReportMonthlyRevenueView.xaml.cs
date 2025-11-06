using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Linq;
using System.Globalization;
using Microsoft.Win32;
using System.IO;

namespace RoomManagementSystem.Presentation.Views.Page.ReportManagement
{
    public partial class ReportMonthlyRevenueView : UserControl
    {
        private readonly ObservableCollection<RevenueItem> _items = new ObservableCollection<RevenueItem>();

        public ReportMonthlyRevenueView()
        {
            InitializeComponent();
            reportDataGrid.ItemsSource = _items;
            LoadSampleData();
            UpdateTotal();
        }

        private void LoadSampleData()
        {
            _items.Clear();
            _items.Add(new RevenueItem { STT = 1, DonViThu = "Phòng cho thuê đã thanh toán", SoTien = "2,000,000" });
            _items.Add(new RevenueItem { STT = 2, DonViThu = "Công nợ từ người thuê", SoTien = "2,000,000" });
            _items.Add(new RevenueItem { STT = 3, DonViThu = "Chi phí tháng", SoTien = "2,000,000" });
        }

        private void UpdateTotal()
        {
            // Remove previous summary if exists
            var summaries = _items.Where(i => i.IsSummary).ToList();
            foreach (var s in summaries) _items.Remove(s);

            decimal sum = 0m;
            foreach (var v in _items.Select(i => i.SoTien))
            {
                if (string.IsNullOrWhiteSpace(v)) continue;
                var digits = new string(v.Where(char.IsDigit).ToArray());
                if (decimal.TryParse(digits, out var num)) sum += num;
            }

            _items.Add(new RevenueItem
            {
                STT = _items.Count + 1,
                DonViThu = "Tổng",
                SoTien = string.Format(CultureInfo.GetCultureInfo("vi-VN"), "{0:n0} đ", sum),
                IsSummary = true
            });
        }

        private void DownloadButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var sfd = new SaveFileDialog
            {
                Title = "Tải xuống báo cáo",
                Filter = "CSV file (*.csv)|*.csv|All files (*.*)|*.*",
                FileName = "doanh_thu_thang.csv"
            };
            if (sfd.ShowDialog() == true)
            {
                using (var sw = new StreamWriter(sfd.FileName, false, System.Text.Encoding.UTF8))
                {
                    sw.WriteLine("STT,ĐƠN VỊ THU,SỐ TIỀN");
                    foreach (var it in _items)
                    {
                        var line = string.Join(",",
                            EscapeCsv(it.STT.ToString()),
                            EscapeCsv(it.DonViThu),
                            EscapeCsv(it.SoTien));
                        sw.WriteLine(line);
                    }
                }
            }
        }

        private static string EscapeCsv(string input)
        {
            if (input == null) return string.Empty;
            if (input.Contains("\",") || input.Contains("\"") || input.Contains("\n"))
            {
                return "\"" + input.Replace("\"", "\"\"") + "\"";
            }
            return input;
        }
    }

    public class RevenueItem
    {
        public int STT { get; set; }
        public string DonViThu { get; set; }
        public string SoTien { get; set; }
        public bool IsSummary { get; set; }
    }
}
