using CommunityToolkit.Mvvm.ComponentModel;
using RoomManagementSystem.BusinessLayer;
using RoomManagementSystem.DataLayer;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Input;
using System.Linq;
using BLL;
using System.Threading.Tasks;
using System;
using System.Data;

namespace RoomManagementSystem.Presentation.ViewModels
{
    public partial class ServiceManagementViewModel : ViewModelBase
    {
        private readonly QL_TaiSan_Phong _service = new QL_TaiSan_Phong();
        // Thêm BLL
        private readonly GoogleSheetBL _googleSheetBL = new GoogleSheetBL();

        // Danh sách nhà và phòng
        public ObservableCollection<Nha> DanhSachNha
        {
            get;
        } = new ObservableCollection<Nha>();
        public ObservableCollection<Phong> DanhSachPhong { get; } = new ObservableCollection<Phong>();
        public ObservableCollection<HouseRooms> DanhSachNhaPhong
        {
            get;
        } = new ObservableCollection<HouseRooms>();

        [ObservableProperty]
        private Nha _selectedNha;
        partial void OnSelectedNhaChanged(Nha value)
        {
            LoadRoomsForHouse(value);
        }

        [ObservableProperty]
        private Phong _selectedPhong;

        [ObservableProperty]
        private string _thoiKy;

        [ObservableProperty]
        private string _oldElectricIndex;

        [ObservableProperty]
        private string _newElectricIndex;

        [ObservableProperty]
        private string _unitPriceElectric = "4000";

        [ObservableProperty]
        private string _oldWaterIndex; 

        [ObservableProperty]
        private string _newWaterIndex;

        [ObservableProperty]
        private string _unitPriceWater = "20000"; 

        [ObservableProperty]
        private string _googleSheetStatus;

        public ServiceManagementViewModel()
        {
            LoadHouses();
            // Tự động gán thời kỳ là tháng/năm hiện tại
            _thoiKy = DateTime.Now.ToString("MM/yyyy");
        }

        private void LoadHouses()
        {
            DanhSachNha.Clear();
            var houses = _service.DanhSachNha() ?? new System.Collections.Generic.List<Nha>();
            foreach (var h in houses)
                DanhSachNha.Add(h);
            if (DanhSachNha.Count > 0)
                SelectedNha = DanhSachNha[0];
            // Xây dựng danh sách nhóm Nhà-Phòng cho giao diện
            DanhSachNhaPhong.Clear();
            foreach (var h in houses)
            {
                var rooms = _service.DanhSachPhong(h.MaNha) ?? new System.Collections.Generic.List<Phong>();
                DanhSachNhaPhong.Add(new HouseRooms
                {
                    House = h,
                    Rooms = new ObservableCollection<Phong>(rooms)
                });
            }
        }

        private void LoadRoomsForHouse(Nha nha)
        {
            DanhSachPhong.Clear();
            if (nha == null) return;
            var rooms = _service.DanhSachPhong(nha.MaNha) ?? new System.Collections.Generic.List<Phong>();
            foreach (var r in rooms)
                DanhSachPhong.Add(r);
            SelectedPhong = DanhSachPhong.Count > 0 ? DanhSachPhong[0] : null;
        }

        // Chọn phòng từ danh sách bên trái
        [RelayCommand]
        private void SelectRoom(Phong room)
        {
            if (room == null) return;
            // Tìm nhà tương ứng
            var nha = DanhSachNha.FirstOrDefault(n => n.MaNha == room.MaNha);
            if (nha != null && SelectedNha != nha)
            {
                // Chỉ set SelectedNha nếu khác nhà hiện tại, để tránh trigger OnSelectedNhaChanged không cần thiết
                SelectedNha = nha;
            }
            // Luôn set SelectedPhong cuối cùng để đảm bảo phòng được chọn đúng
            SelectedPhong = room;
        }

        [RelayCommand]
        private async Task FetchDataFromGoogleSheet()
        {
            if (SelectedPhong == null || string.IsNullOrWhiteSpace(ThoiKy))
            {
                GoogleSheetStatus = "Vui lòng chọn phòng và nhập thời kỳ.";
                return;
            }

            GoogleSheetStatus = "Đang tải dữ liệu từ Google Sheet...";
            DataTable dt = null;
            try
            {
                dt = await Task.Run(() => _googleSheetBL.GetData("Sheet1"));
            }
            catch (Exception ex)
            {
                GoogleSheetStatus = $"Lỗi khi tải: {ex.Message}";
                return;
            }

            if (dt == null || dt.Rows.Count == 0)
            {
                GoogleSheetStatus = "Không tìm thấy dữ liệu trên Sheet.";
                return;
            }

            const string colMaPhong = "Số phòng";
            const string colThoiKy = "Thời kỳ";
            const string colChiSoDienMoi = "Chỉ số điện kỳ này";
            const string colChiSoNuocMoi = "Chỉ số nước kỳ này";
            const string colChiSoDienCu = "Chỉ số điện kỳ trước";
            const string colChiSoNuocCu = "Chỉ số nước kỳ trước";

            if (!dt.Columns.Contains(colMaPhong) || !dt.Columns.Contains(colThoiKy) ||
                !dt.Columns.Contains(colChiSoDienMoi) || !dt.Columns.Contains(colChiSoNuocMoi) ||
                !dt.Columns.Contains(colChiSoDienCu) || !dt.Columns.Contains(colChiSoNuocCu))
            {
                GoogleSheetStatus = "Sheet thiếu các cột cần thiết (cũ hoặc mới).";
                return;
            }

            string currentMaPhong = SelectedPhong.MaPhong;
            string currentThoiKy = ThoiKy;

            // Lặp ngược để lấy bản ghi mới nhất
            for (int i = dt.Rows.Count - 1; i >= 0; i--)
            {
                var row = dt.Rows[i];
                if (row[colMaPhong]?.ToString().Trim() == currentMaPhong &&
                    row[colThoiKy]?.ToString().Trim() == currentThoiKy)
                {
                    NewElectricIndex = row[colChiSoDienMoi]?.ToString().Trim();
                    NewWaterIndex = row[colChiSoNuocMoi]?.ToString().Trim();
                    OldElectricIndex = row[colChiSoDienCu]?.ToString().Trim(); 
                    OldWaterIndex = row[colChiSoNuocCu]?.ToString().Trim(); 

                    GoogleSheetStatus = "Đã tìm thấy và cập nhật dữ liệu (cũ và mới)!";
                    return; // Thoát sau khi tìm thấy
                }
            }

            // Nếu vòng lặp kết thúc mà không tìm thấy
            GoogleSheetStatus = "Không tìm thấy dữ liệu khớp cho phòng/thời kỳ này.";
            NewElectricIndex = "0";
            NewWaterIndex = "0";
            OldElectricIndex = "0";
            OldWaterIndex = "0";
        }
    }
}

namespace RoomManagementSystem.Presentation.ViewModels
{
    public class HouseRooms
    {
        public Nha House
        {
            get;
            set;
        }
        public ObservableCollection<Phong> Rooms { get; set; }
    }
}