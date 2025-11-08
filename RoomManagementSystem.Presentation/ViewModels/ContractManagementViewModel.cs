using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RoomManagementSystem.BusinessLayer;
using RoomManagementSystem.DataLayer;
using RoomManagementSystem.Presentation.ViewModels; // ✅ THÊM using này để nhận diện RoomItemViewModel
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

// Thêm các using cần thiết cho việc tải dữ liệu sau này
// using RoomManagementSystem.DataLayer;
// using RoomManagementSystem.BusinessLayer;
// using System.Collections.ObjectModel;

namespace RoomManagementSystem.Presentation.ViewModels
{
    public partial class ContractManagementViewModel : ViewModelBase
    {
        // --- Properties cho việc hiển thị ---
        [ObservableProperty]
        private bool _isAddContractPopupVisible;

        // --- Dữ liệu cho các ComboBox và Form ---
        [ObservableProperty]
        private ObservableCollection<NguoiThue> _tenantList;

        [ObservableProperty]
        private ObservableCollection<Phong> _availableRoomList;

        // Các thuộc tính để binding với các control trên AddContractView
        // ... (Sẽ được thêm vào sau khi cần)

        // Các Service từ Business Layer
        private readonly QuanLyNguoiThue _tenantService;
        private readonly QL_TaiSan_Phong _roomService;

        public ContractManagementViewModel()
        {
            _tenantService = new QuanLyNguoiThue();
            _roomService = new QL_TaiSan_Phong();

            _isAddContractPopupVisible = false;

            // Khởi tạo các danh sách
            _tenantList = new ObservableCollection<NguoiThue>();
            _availableRoomList = new ObservableCollection<Phong>();

            // Tải dữ liệu ban đầu
            LoadInitialData();
        }

        private void LoadInitialData()
        {
            // Tải danh sách hợp đồng (chưa làm)

            // Tải dữ liệu cho các ComboBox trên popup
            LoadTenants();
            LoadAvailableRooms();
        }

        // Tải danh sách người thuê chưa có hợp đồng
        private void LoadTenants()
        {
            // Giả sử bạn có phương thức để lấy người thuê chưa có HĐ
            // var tenants = _tenantService.GetTenantsWithoutContract(); 
            // TenantList.Clear();
            // foreach(var tenant in tenants) TenantList.Add(tenant);
        }

        // Tải danh sách phòng trống
        private void LoadAvailableRooms()
        {
            // Giả sử bạn có phương thức để lấy các phòng trống
            // var rooms = _roomService.GetAvailableRooms();
            // AvailableRoomList.Clear();
            // foreach(var room in rooms) AvailableRoomList.Add(room);
        }

        // --- Commands ---

        [RelayCommand]
        private void OpenAddContractPopup()
        {
            // Reset các trường dữ liệu trên popup trước khi hiển thị
            // Ví dụ: SelectedTenant = null; SelectedRoom = null; Deposit = 0; ...
            IsAddContractPopupVisible = true;
        }

        [RelayCommand]
        private void CancelAddContract()
        {
            IsAddContractPopupVisible = false;
        }

        [RelayCommand]
        private void CreateContract()
        {
            // Logic xử lý việc tạo hợp đồng mới sẽ ở đây
            // 1. Kiểm tra dữ liệu đầu vào (validate)
            // 2. Tạo đối tượng HopDong mới
            // 3. Gọi service để lưu vào database
            // 4. Hiển thị thông báo

            // Đóng popup sau khi hoàn tất
            IsAddContractPopupVisible = false;
        }
    }
}