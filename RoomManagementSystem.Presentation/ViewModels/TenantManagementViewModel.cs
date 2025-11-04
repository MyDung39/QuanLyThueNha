using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RoomManagementSystem.BusinessLayer;
using RoomManagementSystem.DataLayer;
using System;
using System.Collections.ObjectModel;
using System.Linq; // Cần cho .Where()
using System.Windows;

namespace RoomManagementSystem.Presentation.ViewModels
{
    public partial class TenantManagementViewModel : ViewModelBase
    {
        private readonly QuanLyNguoiThue _nguoiThueService;
        private readonly QL_TaiSan_Phong _taiSanPhongService; // ✅ Thêm service để lấy danh sách phòng

        [ObservableProperty]
        private ObservableCollection<NguoiThue> _danhSachNguoiThue;

        // --- BẮT ĐẦU PHẦN ĐIỀU KHIỂN POPUP ---

        [ObservableProperty]
        private bool _isAddPopupVisible;

        // CÁC THUỘC TÍNH DỮ LIỆU CHO POPUP
        [ObservableProperty] private string _newTenantName = "";
        [ObservableProperty] private string _newTenantEmail = "";
        [ObservableProperty] private string _newTenantPhone = "";
        [ObservableProperty] private string _newTenantIdCard = ""; // ✅ Thêm thuộc tính cho SoGiayTo
        [ObservableProperty] private DateTime? _newTenantStartDate = DateTime.Now;

        // CÁC THUỘC TÍNH ĐỂ HIỂN THỊ DANH SÁCH PHÒNG TRỐNG
        [ObservableProperty] private ObservableCollection<Phong> _availableRooms;
        [ObservableProperty] private Phong _selectedRoomForNewTenant; // ✅ Thêm thuộc tính cho MaPhong

        // --- KẾT THÚC PHẦN ĐIỀU KHIỂN POPUP ---

        public TenantManagementViewModel()
        {
            _nguoiThueService = new QuanLyNguoiThue();
            _taiSanPhongService = new QL_TaiSan_Phong(); // ✅ Khởi tạo service
            _danhSachNguoiThue = new ObservableCollection<NguoiThue>();
            _availableRooms = new ObservableCollection<Phong>();
            LoadData();
        }

        private void LoadData()
        {
            var danhSach = _nguoiThueService.getAll();
            DanhSachNguoiThue.Clear();
            foreach (var nguoiThue in danhSach)
            {
                DanhSachNguoiThue.Add(nguoiThue);
            }
        }

        // PHƯƠNG THỨC MỚI ĐỂ TẢI DANH SÁCH PHÒNG TRỐNG
        private void LoadAvailableRooms()
        {
            try
            {
                var allRooms = _taiSanPhongService.DanhSachPhong();
                // Lọc ra các phòng có trạng thái là 'Trống'
                var freeRooms = allRooms.Where(p => p.TrangThai == "Trống");
                AvailableRooms.Clear();
                foreach (var room in freeRooms)
                {
                    AvailableRooms.Add(room);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Không thể tải danh sách phòng: {ex.Message}");
            }
        }

        // --- CÁC LỆNH (COMMANDS) ---

        [RelayCommand]
        private void OpenAddPopup()
        {
            // Reset dữ liệu cũ
            NewTenantName = "";
            NewTenantEmail = "";
            NewTenantPhone = "";
            NewTenantIdCard = ""; // Reset
            NewTenantStartDate = DateTime.Now;
            SelectedRoomForNewTenant = null; // Reset

            // Tải danh sách các phòng còn trống để người dùng chọn
            LoadAvailableRooms();

            IsAddPopupVisible = true;
        }

        [RelayCommand]
        private void SaveNewTenant()
        {
            try
            {
                var nguoiThueMoi = new NguoiThue
                {
                    // Lấy mã phòng từ phòng được chọn trong ComboBox
                    MaPhong = SelectedRoomForNewTenant?.MaPhong,

                    HoTen = NewTenantName,
                    Email = NewTenantEmail,
                    Sdt = NewTenantPhone,

                    // Lấy số giấy tờ từ TextBox
                    SoGiayTo = NewTenantIdCard,

                    NgayDonVao = NewTenantStartDate, // Giả sử ngày dọn vào là ngày bắt đầu
                    NgayBatDauThue = NewTenantStartDate ?? DateTime.Now,
                };

                // Gọi đến lớp Business Layer đã được cập nhật
                if (_nguoiThueService.ThemNguoiThue(nguoiThueMoi))
                {
                    MessageBox.Show("Thêm người thuê thành công!");
                    LoadData(); // Tải lại danh sách người thuê
                    IsAddPopupVisible = false; // Đóng popup
                }
                // (Không cần else, vì nếu thất bại, lớp Business sẽ ném ra exception)
            }
            catch (Exception ex)
            {
                // Hiển thị các lỗi nghiệp vụ từ lớp Business
                MessageBox.Show($"Lỗi: {ex.Message}", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        [RelayCommand]
        private void CancelAdd()
        {
            IsAddPopupVisible = false;
        }

        // Các lệnh giả
        [RelayCommand] private void OpenEditTenantDialog() { }
        [RelayCommand] private void OpenDeleteTenantDialog() { }
    }
}