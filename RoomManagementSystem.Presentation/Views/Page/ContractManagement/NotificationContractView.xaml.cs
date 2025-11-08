using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace RoomManagementSystem.Presentation.Views.Page.ContractManagement
{
    public partial class NotificationContractView : UserControl
    {
        private readonly ObservableCollection<NotificationItem> _items = new ObservableCollection<NotificationItem>();

        public NotificationContractView()
        {
            InitializeComponent();
            notificationDataGrid.ItemsSource = _items;
            LoadSampleData();
        }

        private void LoadSampleData()
        {
            _items.Clear();
            _items.Add(new NotificationItem { STT = 1, LoaiThongBao = "Đến hạn kết thúc hợp đồng", NguoiThue = "Mỹ Dung", Phong = "BANANA-001", BatDau = "03/03/2020", KetThuc = "02/04/2020", TienCoc = "2,000,000" });
            _items.Add(new NotificationItem { STT = 2, LoaiThongBao = "Chỉnh sửa hợp đồng", NguoiThue = "Lê Công Bảo", Phong = "BANANA-001", BatDau = "03/04/2020", KetThuc = "02/05/2020", TienCoc = "2,000,000" });
            _items.Add(new NotificationItem { STT = 3, LoaiThongBao = "Lập hợp đồng", NguoiThue = "Lê Công Bảo", Phong = "APPLE-001", BatDau = "03/05/2020", KetThuc = "02/06/2020", TienCoc = "2,000,000" });
        }
    }

    public class NotificationItem
    {
        public int STT { get; set; }
        public string LoaiThongBao { get; set; }
        public string NguoiThue { get; set; }
        public string Phong { get; set; }
        public string BatDau { get; set; }
        public string KetThuc { get; set; }
        public string TienCoc { get; set; }
    }
}
