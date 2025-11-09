using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RoomManagementSystem.BusinessLayer;
using RoomManagementSystem.DataLayer;
using RoomManagementSystem.Presentation.ViewModels; // ✅ THÊM using này để nhận diện RoomItemViewModel
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace RoomManagementSystem.Presentation.ViewModels
{
    public partial class HouseManagementViewModel : ViewModelBase
    {
        private readonly QL_TaiSan_Phong _service;

        // Các thuộc tính cho việc quản lý Nhà
        [ObservableProperty]
        private ObservableCollection<Nha> _danhSachNha;

        private Nha _selectedNha;
        public Nha SelectedNha
        {
            get => _selectedNha;
            set
            {
                if (SetProperty(ref _selectedNha, value))
                {
                    OnSelectedNhaChanged(value);
                }
            }
        }

        // ✅ THAY ĐỔI: Danh sách phòng bây giờ là một tập hợp các RoomItemViewModel
        [ObservableProperty]
        private ObservableCollection<RoomItemViewModel> _danhSachPhongHienThi;

        // ✅ THÊM: Thuộc tính để binding với Checkbox "Chọn Tất Cả" trên giao diện
        [ObservableProperty]
        private bool _isAllSelected;

        // --- Các thuộc tính điều khiển Popup (Giữ nguyên) ---
        [ObservableProperty] private bool _isAddHousePopupVisible;
        [ObservableProperty] private string _newHouseAddress;
        [ObservableProperty] private string _newHouseNotes;
        [ObservableProperty] private bool _isEditHousePopupVisible;
        [ObservableProperty] private string _editingHouseAddress;
        [ObservableProperty] private string _editingHouseNotes;
        [ObservableProperty] private bool _isDeleteHousePopupVisible;
        [ObservableProperty] private bool _isAddRoomPopupVisible;
        [ObservableProperty] private string _newRoomNumber;
        [ObservableProperty] private decimal _newRoomArea;
        [ObservableProperty] private decimal _newRoomCost;
        [ObservableProperty] private string _newRoomNotes;
        [ObservableProperty] private ObservableCollection<string> _loaiPhongOptions;
        [ObservableProperty] private string _newRoomLoaiPhong;
        [ObservableProperty] private bool _isEditRoomPopupVisible;
        [ObservableProperty] private string _editingRoomNumber;
        [ObservableProperty] private decimal _editingRoomArea;
        [ObservableProperty] private decimal _editingRoomCost;
        [ObservableProperty] private string _editingRoomNotes;

        // ✅ THAY ĐỔI: Thuộc tính này sẽ điều khiển popup xác nhận xóa phòng
        [ObservableProperty]
        private bool _isDeleteRoomPopupVisible;

        // ✅ THÊM: Một thuộc tính để lưu trữ phòng đang được sửa
        private RoomItemViewModel _roomBeingEdited;

        // --- Constructor ---
        public HouseManagementViewModel()
        {
            _service = new QL_TaiSan_Phong();
            _danhSachNha = new ObservableCollection<Nha>();

            // ✅ THAY ĐỔI: Khởi tạo đúng kiểu danh sách
            _danhSachPhongHienThi = new ObservableCollection<RoomItemViewModel>();

            _loaiPhongOptions = new ObservableCollection<string> { "Phòng trống", "Phòng có đồ cơ bản" };
            _newRoomLoaiPhong = _loaiPhongOptions.FirstOrDefault();

            LoadHouseData();
        }

        // --- Logic tải dữ liệu ---
        private void LoadHouseData()
        {
            try
            {
                var houses = _service.DanhSachNha();
                DanhSachNha.Clear();
                foreach (var house in houses)
                {
                    DanhSachNha.Add(house);
                }
                if (DanhSachNha.Any())
                {
                    SelectedNha = DanhSachNha.First();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải dữ liệu nhà: {ex.Message}");
            }
        }

        // ✅ THAY ĐỔI: Cập nhật hàm này để làm việc với RoomItemViewModel
        private void OnSelectedNhaChanged(Nha value)
        {
            DanhSachPhongHienThi.Clear();
            if (value != null)
            {
                try
                {
                    var roomsInHouse = _service.DanhSachPhong(value.MaNha);
                    foreach (var room in roomsInHouse)
                    {
                        // Thay vì thêm `Phong`, ta tạo một `RoomItemViewModel` để bọc nó lại rồi mới thêm vào danh sách
                        DanhSachPhongHienThi.Add(new RoomItemViewModel(room));
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi tải danh sách phòng: {ex.Message}");
                }
            }
            // Cập nhật lại trạng thái của checkbox "Chọn tất cả" mỗi khi tải lại danh sách
            UpdateSelectionState();
        }

        // --- Logic xử lý Checkbox ---

        // ✅ THÊM: Logic này được CommunityToolkit.Mvvm tự động gọi khi thuộc tính IsAllSelected thay đổi (do người dùng tick vào)
        partial void OnIsAllSelectedChanged(bool value)
        {
            // Duyệt qua tất cả các phòng và gán trạng thái IsSelected của chúng bằng với giá trị mới của checkbox cha
            foreach (var item in DanhSachPhongHienThi)
            {
                item.IsSelected = value;
            }
        }

        // ✅ THÊM: Command này được gọi mỗi khi một checkbox của phòng riêng lẻ được click
        [RelayCommand]
        private void UpdateSelectionState()
        {
            // Kiểm tra xem: có phòng nào trong danh sách KHÔNG và tất cả chúng đều đang được chọn
            var allSelected = DanhSachPhongHienThi.Any() && DanhSachPhongHienThi.All(item => item.IsSelected);

            // Cập nhật thuộc tính IsAllSelected một cách "thầm lặng" để không kích hoạt lại vòng lặp vô hạn từ OnIsAllSelectedChanged
            SetProperty(ref _isAllSelected, allSelected, nameof(IsAllSelected));
        }

        // --- Logic Xóa Phòng (Một, Nhiều, Tất Cả) ---

        // ✅ THAY ĐỔI: Nút xóa chính giờ sẽ kiểm tra và mở popup
        [RelayCommand]
        private void DeleteRoom()
        {
            // Kiểm tra xem có bất kỳ phòng nào đang được chọn không
            var anySelected = DanhSachPhongHienThi.Any(item => item.IsSelected);
            if (!anySelected)
            {
                MessageBox.Show("Vui lòng chọn ít nhất một phòng để xóa.", "Chưa chọn phòng", MessageBoxButton.OK, MessageBoxImage.Information);
                return; // Không làm gì nếu chưa chọn
            }

            // Nếu có ít nhất một phòng được chọn, mở popup xác nhận
            IsDeleteRoomPopupVisible = true;
        }

        // ✅ THÊM: Command cho nút "Xác nhận" trên popup xóa
        [RelayCommand]
        private void ConfirmDeleteRoom()
        {
            // Dùng LINQ để lấy danh sách tất cả các phòng đã được chọn
            var selectedItems = DanhSachPhongHienThi.Where(item => item.IsSelected).ToList();

            try
            {
                int successCount = 0;
                foreach (var item in selectedItems)
                {
                    // Gọi hàm xóa với MaPhong từ đối tượng Phong gốc bên trong wrapper
                    if (_service.XoaPhong(item.Phong.MaPhong))
                    {
                        successCount++;
                    }
                }

                MessageBox.Show($"Đã xóa thành công {successCount}/{selectedItems.Count} phòng.", "Hoàn tất", MessageBoxButton.OK, MessageBoxImage.Information);

                // Tải lại danh sách phòng của nhà hiện tại để cập nhật giao diện
                OnSelectedNhaChanged(SelectedNha);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Đã xảy ra lỗi trong quá trình xóa: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                // Luôn đóng popup sau khi hoàn tất
                IsDeleteRoomPopupVisible = false;
            }
        }

        // ✅ THÊM: Command cho nút "Hủy" hoặc "Đóng" trên popup xóa
        [RelayCommand]
        private void CancelDeleteRoom()
        {
            IsDeleteRoomPopupVisible = false;
        }

        // --- Các Command khác (Giữ nguyên logic của bạn) ---

        [RelayCommand]
        private void AddHouse()
        {
            NewHouseAddress = string.Empty;
            NewHouseNotes = string.Empty;
            IsAddHousePopupVisible = true;
        }

        [RelayCommand]
        private void SaveNewHouse()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(NewHouseAddress))
                {
                    MessageBox.Show("Vui lòng nhập địa chỉ nhà.");
                    return;
                }

                // Giả định hàm DangKyThongTinNha trong service của bạn nhận vào (string DiaChi, string GhiChu)
                if (_service.DangKyThongTinNha(NewHouseAddress, NewHouseNotes))
                {
                    MessageBox.Show("Thêm nhà thành công!");
                    LoadHouseData(); // Tải lại danh sách nhà
                    IsAddHousePopupVisible = false; // Đóng popup
                }
                else
                {
                    MessageBox.Show("Thêm nhà thất bại!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}");
            }
        }

        [RelayCommand]
        private void CancelAddHouse()
        {
            IsAddHousePopupVisible = false;
        }


        [RelayCommand]
        private void EditHouse()
        {
            // 1. Kiểm tra xem đã chọn nhà nào chưa
            if (SelectedNha == null)
            {
                MessageBox.Show("Vui lòng chọn một nhà để chỉnh sửa.");
                return;
            }

            // 2. Tải dữ liệu của nhà đã chọn vào các thuộc tính
            EditingHouseAddress = SelectedNha.DiaChi;
            EditingHouseNotes = SelectedNha.GhiChu;

            // 3. Hiển thị popup
            IsEditHousePopupVisible = true;
        }

        [RelayCommand]
        private void SaveEditHouse()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(EditingHouseAddress))
                {
                    MessageBox.Show("Vui lòng nhập địa chỉ nhà.");
                    return;
                }

                // Cập nhật thông tin cho đối tượng SelectedNha
                SelectedNha.DiaChi = EditingHouseAddress;
                SelectedNha.GhiChu = EditingHouseNotes;

                // Giả định bạn có hàm Cập nhật trong service
                // (Bạn cần tự tạo hàm này trong BusinessLayer/DataLayer)
                if (_service.UpdateNha(SelectedNha.MaNha, SelectedNha.DiaChi, SelectedNha.GhiChu))
                {
                    MessageBox.Show("Cập nhật nhà thành công!");
                    LoadHouseData(); // Tải lại danh sách nhà
                    IsEditHousePopupVisible = false; // Đóng popup
                }
                else
                {
                    MessageBox.Show("Cập nhật nhà thất bại!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}");
            }
        }


        [RelayCommand]
        private void CancelEditHouse()
        {
            IsEditHousePopupVisible = false;
        }


        [RelayCommand]
        private void DeleteHouse()
        {
            if (SelectedNha == null)
            {
                MessageBox.Show("Vui lòng chọn một nhà để xóa.");
                return;
            }
            // Mở popup xác nhận xóa
            IsDeleteHousePopupVisible = true;
        }


        [RelayCommand]
        private void ConfirmDeleteHouse()
        {
            if (SelectedNha == null) return; // Kiểm tra an toàn

            try
            {
                // Chúng ta sẽ cần tạo hàm 'XoaNha' này ở BusinessLayer
                if (_service.XoaNha(SelectedNha.MaNha))
                {
                    MessageBox.Show("Xóa nhà thành công!");
                    LoadHouseData(); // Tải lại danh sách nhà
                    IsDeleteHousePopupVisible = false; // Đóng popup
                }
                else
                {
                    MessageBox.Show("Xóa nhà thất bại! Hãy xóa hết phòng của nhà");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi xóa nhà: {ex.Message}");
            }
        }


        [RelayCommand]
        private void CancelDeleteHouse()
        {
            IsDeleteHousePopupVisible = false;
        }


        [RelayCommand]
        private void AddRoom()
        {
            // Kiểm tra xem đã chọn nhà nào chưa
            if (SelectedNha == null)
            {
                MessageBox.Show("Vui lòng chọn một nhà trước khi thêm phòng.");
                return;
            }

            // Reset các trường
            NewRoomNumber = string.Empty;
            NewRoomArea = 0;
            NewRoomCost = 0;
            NewRoomNotes = string.Empty;
            NewRoomLoaiPhong = LoaiPhongOptions.FirstOrDefault(); // ✅ ĐẶT LẠI GIÁ TRỊ MẶC ĐỊNH

            // Hiển thị popup
            IsAddRoomPopupVisible = true;
        }


        [RelayCommand]
        private void SaveNewRoom()
        {
            try
            {
                if (SelectedNha == null) return; // Kiểm tra an toàn

                if (string.IsNullOrWhiteSpace(NewRoomNumber))
                {
                    MessageBox.Show("Vui lòng nhập số phòng.");
                    return;
                }

                // Tạo đối tượng Phong mới
                Phong newRoom = new Phong
                {
                    MaNha = SelectedNha.MaNha,
                    MaPhong = NewRoomNumber, // Mã phòng này sẽ được gán tự động ở BLL
                    DienTich = NewRoomArea,  // ✅ ĐÃ SỬA: Ép kiểu sang float
                    GiaThue = NewRoomCost,   // ✅ ĐÃ SỬA: Ép kiểu sang float
                    GhiChu = NewRoomNotes,
                    TrangThai = "Trống", // Mặc định là trống
                    LoaiPhong = NewRoomLoaiPhong // ✅ SỬA LỖI: Lấy giá trị từ ComboBox
                };

                // Gọi hàm ThemPhong từ BusinessLayer (bạn đã có)
                if (_service.ThemPhong(newRoom))
                {
                    MessageBox.Show("Thêm phòng thành công!");
                    OnSelectedNhaChanged(SelectedNha); // Tải lại danh sách phòng
                    IsAddRoomPopupVisible = false; // Đóng popup
                }
                else
                {
                    MessageBox.Show("Thêm phòng thất bại!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}");
            }
        }


        [RelayCommand]
        private void CancelAddRoom()
        {
            IsAddRoomPopupVisible = false;
        }


        [RelayCommand]
        // ✅ SỬA LẠI: Logic cho nút Sửa Phòng
        
        private void EditRoom()
        {
            var selectedRooms = DanhSachPhongHienThi.Where(r => r.IsSelected).ToList();

            if (selectedRooms.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn một phòng để chỉnh sửa.", "Chưa chọn phòng", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (selectedRooms.Count > 1)
            {
                MessageBox.Show("Chỉ có thể chỉnh sửa một phòng mỗi lần.", "Chọn quá nhiều", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            // Lấy phòng duy nhất đã được chọn
            _roomBeingEdited = selectedRooms.First();
            var phongToEdit = _roomBeingEdited.Phong;

            // Tải dữ liệu của phòng đó vào các thuộc tính để binding với popup
            EditingRoomNumber = phongToEdit.MaPhong;
            EditingRoomArea = (decimal)phongToEdit.DienTich;
            EditingRoomCost = (decimal)phongToEdit.GiaThue;
            EditingRoomNotes = phongToEdit.GhiChu;

            IsEditRoomPopupVisible = true;
        }



        // ✅ SỬA LẠI: Logic cho nút Lưu sau khi Sửa Phòng
        [RelayCommand]
        private void SaveEditRoom()
        {
            if (_roomBeingEdited == null) return;

            try
            {
                var phongToUpdate = _roomBeingEdited.Phong;

                if (string.IsNullOrWhiteSpace(EditingRoomNumber))
                {
                    MessageBox.Show("Vui lòng nhập số phòng.");
                    return;
                }

                // Cập nhật thông tin vào đối tượng Phong gốc
                // Mã phòng (khóa chính) thường không nên thay đổi
                // phongToUpdate.MaPhong = EditingRoomNumber;
                phongToUpdate.DienTich = EditingRoomArea;
                phongToUpdate.GiaThue = EditingRoomCost;
                phongToUpdate.GhiChu = EditingRoomNotes;

                if (_service.CapNhatPhong(phongToUpdate))
                {
                    MessageBox.Show("Cập nhật phòng thành công!");
                    OnSelectedNhaChanged(SelectedNha); // Tải lại danh sách
                    IsEditRoomPopupVisible = false; // Đóng popup
                }
                else
                {
                    MessageBox.Show("Cập nhật phòng thất bại!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}");
            }
            finally
            {
                _roomBeingEdited = null; // Reset lại phòng đang sửa
            }
        }

        [RelayCommand]
        private void CancelEditRoom() { IsEditRoomPopupVisible = false; _roomBeingEdited = null; }
    }
}