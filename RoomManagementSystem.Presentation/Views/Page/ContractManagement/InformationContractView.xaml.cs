using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace RoomManagementSystem.Presentation.Views.Page.ContractManagement
{
    public partial class InformationContractView : UserControl
    {
        private readonly ObservableCollection<HistoryItem> _historyItems = new ObservableCollection<HistoryItem>();

        public InformationContractView()
        {
            InitializeComponent();
            historyDataGrid.ItemsSource = _historyItems;
            LoadSampleData();
        }

        private void LoadSampleData()
        {
            _historyItems.Clear();
            _historyItems.Add(new HistoryItem
            {
                STT = 1,
                NgaySua = "03/05/2020",
                NguoiThue = "Lê Công Bảo",
                Phong = "APPLE-001",
                BatDau = "03/05/2020",
                KetThuc = "02/06/2020",
                TienCoc = "2,000,000 VND"
            });
            _historyItems.Add(new HistoryItem
            {
                STT = 2,
                NgaySua = "10/05/2020",
                NguoiThue = "Lê Công Bảo",
                Phong = "APPLE-001",
                BatDau = "03/05/2020",
                KetThuc = "03/07/2020",
                TienCoc = "2,000,000 VND"
            });
            _historyItems.Add(new HistoryItem
            {
                STT = 3,
                NgaySua = "20/05/2020",
                NguoiThue = "Lê Công Bảo",
                Phong = "APPLE-001",
                BatDau = "03/05/2020",
                KetThuc = "03/08/2020",
                TienCoc = "2,500,000 VND"
            });
        }
    }

    public class HistoryItem
    {
        public int STT { get; set; }
        public string NgaySua { get; set; }
        public string NguoiThue { get; set; }
        public string Phong { get; set; }
        public string BatDau { get; set; }
        public string KetThuc { get; set; }
        public string TienCoc { get; set; }
    }
}
