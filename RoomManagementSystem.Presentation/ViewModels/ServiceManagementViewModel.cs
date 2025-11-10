using CommunityToolkit.Mvvm.ComponentModel;
using RoomManagementSystem.BusinessLayer;
using RoomManagementSystem.DataLayer;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Input;
using System.Linq;

namespace RoomManagementSystem.Presentation.ViewModels
{
    public partial class ServiceManagementViewModel : ViewModelBase
    {
        private readonly QL_TaiSan_Phong _service = new QL_TaiSan_Phong();

        // Danh sách nhà và phòng
        public ObservableCollection<Nha> DanhSachNha { get; } = new ObservableCollection<Nha>();
        public ObservableCollection<Phong> DanhSachPhong { get; } = new ObservableCollection<Phong>();

        // Danh sách nhóm Nhà - Phòng để hiển thị theo UI cũ
        public ObservableCollection<HouseRooms> DanhSachNhaPhong { get; } = new ObservableCollection<HouseRooms>();

        [ObservableProperty]
        private Nha _selectedNha;

        partial void OnSelectedNhaChanged(Nha value)
        {
            LoadRoomsForHouse(value);
        }

        [ObservableProperty]
        private Phong _selectedPhong;

        public ServiceManagementViewModel()
        {
            LoadHouses();
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

        // Chọn phòng từ danh sách bên trái (giữ UI cũ nhưng có tương tác)
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
    }
}

namespace RoomManagementSystem.Presentation.ViewModels
{
    public class HouseRooms
    {
        public Nha House { get; set; }
        public ObservableCollection<Phong> Rooms { get; set; }
    }
}
