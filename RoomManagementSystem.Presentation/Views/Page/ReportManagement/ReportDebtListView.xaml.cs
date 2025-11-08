using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Linq;
using System.Globalization;
using Microsoft.Win32;
using System.IO;

namespace RoomManagementSystem.Presentation.Views.Page.ReportManagement
{
    public partial class ReportDebtListView : UserControl
    {
        private readonly ObservableCollection<DebtItem> _items = new ObservableCollection<DebtItem>();

        public ReportDebtListView()
        {
            InitializeComponent();
            reportDataGrid.ItemsSource = _items;
            LoadSampleData();
            UpdateTotal();
        }

        private void LoadSampleData()
        {
            _items.Clear();
            _items.Add(new DebtItem { STT = 1, NguoiThue = "Trần Thanh Nhã", Phong = "APPLE-001", SoTienNo = "3,000,000" });
            _items.Add(new DebtItem { STT = 2, NguoiThue = "Lê Công Bảo", Phong = "BANANA-002", SoTienNo = "2,000,000" });
        }

        private void UpdateTotal()
        {
            // Remove previous summary if exists
            var summaries = _items.Where(i => i.IsSummary).ToList();
            foreach (var s in summaries) _items.Remove(s);

            decimal sum = 0m;
            foreach (var v in _items.Select(i => i.SoTienNo))
            {
                if (string.IsNullOrWhiteSpace(v)) continue;
                var digits = new string(v.Where(char.IsDigit).ToArray());
                if (decimal.TryParse(digits, out var num)) sum += num;
            }

            _items.Add(new DebtItem
            {
                STT = _items.Count + 1,
                NguoiThue = "Tổng",
                Phong = string.Empty,
                SoTienNo = string.Format(CultureInfo.GetCultureInfo("vi-VN"), "{0:n0} đ", sum),
                IsSummary = true
            });
        }

        private void DownloadButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var sfd = new SaveFileDialog
            {
                Title = "Tải xuống báo cáo",
                Filter = "CSV file (*.csv)|*.csv|All files (*.*)|*.*",
                FileName = "cong_no.csv"
            };
            if (sfd.ShowDialog() == true)
            {
                using (var sw = new StreamWriter(sfd.FileName, false, System.Text.Encoding.UTF8))
                {
                    sw.WriteLine("STT,NGƯỜI THUÊ,PHÒNG,SỐ TIỀN NỢ");
                    foreach (var it in _items)
                    {
                        var line = string.Join(",",
                            EscapeCsv(it.STT.ToString()),
                            EscapeCsv(it.NguoiThue),
                            EscapeCsv(it.Phong),
                            EscapeCsv(it.SoTienNo));
                        sw.WriteLine(line);
                    }
                }
            }
        }

        private static string EscapeCsv(string input)
        {
            if (input == null) return string.Empty;
            if (input.Contains(",") || input.Contains("\"") || input.Contains("\n"))
            {
                return "\"" + input.Replace("\"", "\"\"") + "\"";
            }
            return input;
        }
    }

    public class DebtItem
    {
        public int STT { get; set; }
        public string NguoiThue { get; set; }
        public string Phong { get; set; }
        public string SoTienNo { get; set; }
        public bool IsSummary { get; set; }
    }
}
