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
        // Text bindings for validation before parsing
        [ObservableProperty] private string _newRoomAreaText;
        [ObservableProperty] private string _newRoomCostText;
        [ObservableProperty] private string _newRoomNotes;
        [ObservableProperty] private ObservableCollection<string> _loaiPhongOptions;
        [ObservableProperty] private string _newRoomLoaiPhong;
        [ObservableProperty] private bool _isEditRoomPopupVisible;
        [ObservableProperty] private string _editingRoomNumber;
        [ObservableProperty] private decimal _editingRoomArea;
        [ObservableProperty] private decimal _editingRoomCost;
        // Text bindings for edit validation
        [ObservableProperty] private string _editingRoomAreaText;
        [ObservableProperty] private string _editingRoomCostText;
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

            // ✅ Reset TẤT CẢ các trường để tránh lưu giá trị cũ
            NewRoomNumber = string.Empty;
            NewRoomArea = 0;
            NewRoomCost = 0;
            NewRoomAreaText = string.Empty;  // ← Quan trọng: Reset text binding
            NewRoomCostText = string.Empty;  // ← Quan trọng: Reset text binding
            NewRoomNotes = string.Empty;
            NewRoomLoaiPhong = LoaiPhongOptions.FirstOrDefault();

            // Hiển thị popup
            IsAddRoomPopupVisible = true;
        }


        [RelayCommand]
        private void SaveNewRoom()
        {
            try
            {
                if (SelectedNha == null) return; // Kiểm tra an toàn

                // ✅ VALIDATION: Kiểm tra số phòng
                if (string.IsNullOrWhiteSpace(NewRoomNumber))
                {
                    MessageBox.Show("❌ Lỗi: Vui lòng nhập số phòng!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // ✅ VALIDATION: Kiểm tra diện tích - TUYỆT ĐỐI KHÔNG CHO QUA
                if (string.IsNullOrWhiteSpace(NewRoomAreaText))
                {
                    MessageBox.Show("❌ Lỗi: Vui lòng nhập diện tích phòng!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // 🚫 KIỂM TRA TUYỆT ĐỐI: Không cho phép bất kỳ ký tự nào khác ngoài số và dấu thập phân
                string cleanNewAreaText = NewRoomAreaText.Trim();
                if (string.IsNullOrEmpty(cleanNewAreaText))
                {
                    MessageBox.Show("❌ Lỗi: Diện tích không được để trống!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Kiểm tra từng ký tự trong chuỗi diện tích
                bool hasInvalidNewAreaChar = false;
                foreach (char c in cleanNewAreaText)
                {
                    if (!char.IsDigit(c) && c != '.' && c != ',')
                    {
                        hasInvalidNewAreaChar = true;
                        break;
                    }
                }

                if (hasInvalidNewAreaChar)
                {
                    MessageBox.Show($"❌ CHẶN: Diện tích '{cleanNewAreaText}' chứa ký tự không hợp lệ!\n\n🚫 Phát hiện ký tự chữ cái hoặc ký tự đặc biệt\n✅ Chỉ được nhập: số (0-9), dấu chấm (.) hoặc dấu phẩy (,)\n\n📝 Ví dụ hợp lệ: 25, 25.5, 25,5", "ĐỊNH DẠNG SAI", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Kiểm tra regex bổ sung
                if (!System.Text.RegularExpressions.Regex.IsMatch(cleanNewAreaText, @"^[0-9]+([\.\,][0-9]{0,2})?$"))
                {
                    MessageBox.Show($"❌ Lỗi: Diện tích '{cleanNewAreaText}' không đúng định dạng!\n\n✅ Chỉ được nhập số (ví dụ: 25 hoặc 25.5)", "Định dạng không hợp lệ", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                decimal parsedNewArea;
                if (!decimal.TryParse(cleanNewAreaText.Replace(',', '.'), System.Globalization.NumberStyles.Number, System.Globalization.CultureInfo.InvariantCulture, out parsedNewArea) || parsedNewArea <= 0)
                {
                    MessageBox.Show($"❌ Lỗi: Diện tích '{cleanNewAreaText}' phải là số lớn hơn 0!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // ✅ VALIDATION: Kiểm tra chi phí theo tháng
                decimal parsedNewCost = 0m;
                if (!string.IsNullOrWhiteSpace(NewRoomCostText))
                {
                    string cleanNewCostText = NewRoomCostText.Trim();
                    
                    // Kiểm tra từng ký tự trong chuỗi chi phí
                    bool hasInvalidNewCostChar = false;
                    foreach (char c in cleanNewCostText)
                    {
                        if (!char.IsDigit(c) && c != '.' && c != ',')
                        {
                            hasInvalidNewCostChar = true;
                            break;
                        }
                    }

                    if (hasInvalidNewCostChar)
                    {
                        MessageBox.Show($"❌ CHẶN: Chi phí '{cleanNewCostText}' chứa ký tự không hợp lệ!\n\n🚫 Phát hiện ký tự chữ cái hoặc ký tự đặc biệt\n✅ Chỉ được nhập: số (0-9), dấu chấm (.) hoặc dấu phẩy (,)", "ĐỊNH DẠNG SAI", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    // Kiểm tra regex
                    if (!System.Text.RegularExpressions.Regex.IsMatch(cleanNewCostText, @"^[0-9]+([\.\,][0-9]{0,2})?$"))
                    {
                        MessageBox.Show($"❌ Lỗi: Chi phí '{cleanNewCostText}' không đúng định dạng!\n\n✅ Chỉ được nhập số (ví dụ: 5000000 hoặc 5000000.50)", "Định dạng không hợp lệ", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    if (!decimal.TryParse(cleanNewCostText.Replace(',', '.'), System.Globalization.NumberStyles.Number, System.Globalization.CultureInfo.InvariantCulture, out parsedNewCost) || parsedNewCost < 0)
                    {
                        MessageBox.Show($"❌ Lỗi: Chi phí '{cleanNewCostText}' phải là số không âm!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                }

                // Tạo đối tượng Phong mới
                Phong newRoom = new Phong
                {
                    MaNha = SelectedNha.MaNha,
                    MaPhong = NewRoomNumber, // Mã phòng này sẽ được gán tự động ở BLL
                    DienTich = parsedNewArea,
                    GiaThue = parsedNewCost,
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

            // ✅ Tải dữ liệu của phòng đó vào các thuộc tính để binding với popup
            EditingRoomNumber = phongToEdit.MaPhong ?? string.Empty;
            EditingRoomArea = (decimal)phongToEdit.DienTich;
            EditingRoomCost = (decimal)phongToEdit.GiaThue;
            // ← Quan trọng: Đảm bảo text binding được cập nhật đúng
            EditingRoomAreaText = phongToEdit.DienTich.ToString(System.Globalization.CultureInfo.InvariantCulture);
            EditingRoomCostText = phongToEdit.GiaThue.ToString(System.Globalization.CultureInfo.InvariantCulture);
            EditingRoomNotes = phongToEdit.GhiChu ?? string.Empty;

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

                // 🚨 KIỂM TRA TUYỆT ĐỐI - KHÔNG CHO QUA BẤT KỲ TRƯỜNG HỢP NÀO
                
                // 1. Kiểm tra diện tích
                if (string.IsNullOrWhiteSpace(EditingRoomAreaText))
                {
                    MessageBox.Show("❌ Lỗi: Bạn chưa nhập diện tích!\n\n📝 Vui lòng nhập diện tích hợp lệ (ví dụ: 25 hoặc 25.5)", "Thiếu thông tin", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // 2. Kiểm tra từng ký tự trong diện tích
                string areaText = EditingRoomAreaText.Trim();
                bool hasInvalidAreaChar = false;
                char invalidAreaChar = ' ';
                
                foreach (char c in areaText)
                {
                    if (!char.IsDigit(c) && c != '.' && c != ',')
                    {
                        hasInvalidAreaChar = true;
                        invalidAreaChar = c;
                        break;
                    }
                }
                
                if (hasInvalidAreaChar)
                {
                    MessageBox.Show($"❌ Lỗi: Diện tích chứa ký tự không hợp lệ!\n\n" +
                        $"🚫 Ký tự sai: '{invalidAreaChar}'\n" +
                        $"📝 Bạn đã nhập: '{areaText}'\n\n" +
                        $"✅ Chỉ được nhập:\n" +
                        $"   • Số (0-9)\n" +
                        $"   • Dấu chấm (.)\n" +
                        $"   • Dấu phẩy (,)\n\n" +
                        $"📝 Ví dụ đúng: 25, 30.5, 42,75", 
                        "Nhập sai định dạng", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                
                // 3. Kiểm tra chi phí (nếu có nhập)
                if (!string.IsNullOrWhiteSpace(EditingRoomCostText))
                {
                    string costText = EditingRoomCostText.Trim();
                    bool hasInvalidCostChar = false;
                    char invalidCostChar = ' ';
                    
                    foreach (char c in costText)
                    {
                        if (!char.IsDigit(c) && c != '.' && c != ',')
                        {
                            hasInvalidCostChar = true;
                            invalidCostChar = c;
                            break;
                        }
                    }
                    
                    if (hasInvalidCostChar)
                    {
                        MessageBox.Show($"❌ Lỗi: Chi phí chứa ký tự không hợp lệ!\n\n" +
                            $"🚫 Ký tự sai: '{invalidCostChar}'\n" +
                            $"📝 Bạn đã nhập: '{costText}'\n\n" +
                            $"✅ Chỉ được nhập số (ví dụ: 5000000 hoặc 2500000.50)", 
                            "Nhập sai định dạng", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }
                
                // 4. Kiểm tra parse được thành số không
                decimal parsedArea;
                if (!decimal.TryParse(areaText.Replace(',', '.'), System.Globalization.NumberStyles.Number, System.Globalization.CultureInfo.InvariantCulture, out parsedArea))
                {
                    MessageBox.Show($"❌ Lỗi: Diện tích không phải là số hợp lệ!\n\n" +
                        $"📝 Bạn đã nhập: '{areaText}'\n" +
                        $"✅ Ví dụ đúng: 25, 30.5, 42,75", 
                        "Không phải số", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                
                if (parsedArea <= 0)
                {
                    MessageBox.Show($"❌ Lỗi: Diện tích phải lớn hơn 0!\n\n" +
                        $"📝 Bạn đã nhập: {parsedArea}\n" +
                        $"✅ Vui lòng nhập số dương (ví dụ: 25, 30.5)", 
                        "Số không hợp lệ", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // 🔍 DEBUG: Log tất cả giá trị để debug
                System.Diagnostics.Debug.WriteLine($"=== SAVE EDIT ROOM DEBUG ===");
                System.Diagnostics.Debug.WriteLine($"EditingRoomNumber: '{EditingRoomNumber}'");
                System.Diagnostics.Debug.WriteLine($"EditingRoomAreaText: '{EditingRoomAreaText}'");
                System.Diagnostics.Debug.WriteLine($"EditingRoomCostText: '{EditingRoomCostText}'");
                System.Diagnostics.Debug.WriteLine($"EditingRoomNotes: '{EditingRoomNotes}'");

                // VALIDATION: Kiểm tra số phòng
                if (string.IsNullOrWhiteSpace(EditingRoomNumber))
                {
                    MessageBox.Show(" Lỗi: Vui lòng nhập số phòng!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // VALIDATION: Kiểm tra diện tích - TUYỆT ĐỐI KHÔNG CHO QUA
                if (string.IsNullOrWhiteSpace(EditingRoomAreaText))
                {
                    MessageBox.Show(" Lỗi: Vui lòng nhập diện tích phòng!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // KIỂM TRA TUYỆT ĐỐI: Không cho phép bất kỳ ký tự nào khác ngoài số và dấu thập phân
                string cleanAreaText = EditingRoomAreaText.Trim();
                if (string.IsNullOrEmpty(cleanAreaText))
                {
                    MessageBox.Show(" Lỗi: Diện tích không được để trống!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Kiểm tra từng ký tự trong chuỗi diện tích
                bool hasInvalidChar = false;
                foreach (char c in cleanAreaText)
                {
                    if (!char.IsDigit(c) && c != '.' && c != ',')
                    {
                        hasInvalidChar = true;
                        break;
                    }
                }

                if (hasInvalidChar)
                {
                    MessageBox.Show($" CHẶN: Diện tích '{cleanAreaText}' chứa ký tự không hợp lệ!\n\n Phát hiện ký tự chữ cái hoặc ký tự đặc biệt\n Chỉ được nhập: số (0-9), dấu chấm (.) hoặc dấu phẩy (,)\n\n Ví dụ hợp lệ: 25, 25.5, 25,5", "ĐỊNH DẠNG SAI", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Kiểm tra regex bổ sung
                if (!System.Text.RegularExpressions.Regex.IsMatch(cleanAreaText, @"^[0-9]+([\.\,][0-9]{0,2})?$"))
                {
                    MessageBox.Show($" Lỗi: Diện tích '{cleanAreaText}' không đúng định dạng!\n\n Chỉ được nhập số (ví dụ: 25 hoặc 25.5)", "Định dạng không hợp lệ", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                decimal parsedEditArea;
                if (!decimal.TryParse(cleanAreaText.Replace(',', '.'), System.Globalization.NumberStyles.Number, System.Globalization.CultureInfo.InvariantCulture, out parsedEditArea) || parsedEditArea <= 0)
                {
                    MessageBox.Show($" Lỗi: Diện tích '{cleanAreaText}' phải là số lớn hơn 0!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // VALIDATION: Kiểm tra chi phí theo tháng
                decimal parsedEditCost = 0m;
                if (!string.IsNullOrWhiteSpace(EditingRoomCostText))
                {
                    string cleanCostText = EditingRoomCostText.Trim();
                    
                    // Kiểm tra từng ký tự trong chuỗi chi phí
                    bool hasInvalidCostChar = false;
                    foreach (char c in cleanCostText)
                    {
                        if (!char.IsDigit(c) && c != '.' && c != ',')
                        {
                            hasInvalidCostChar = true;
                            break;
                        }
                    }

                    if (hasInvalidCostChar)
                    {
                        MessageBox.Show($" CHẶN: Chi phí '{cleanCostText}' chứa ký tự không hợp lệ!\n\n Phát hiện ký tự chữ cái hoặc ký tự đặc biệt\n Chỉ được nhập: số (0-9), dấu chấm (.) hoặc dấu phẩy (,)", "ĐỊNH DẠNG SAI", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    // Kiểm tra regex
                    if (!System.Text.RegularExpressions.Regex.IsMatch(cleanCostText, @"^[0-9]+([\.\,][0-9]{0,2})?$"))
                    {
                        MessageBox.Show($" Lỗi: Chi phí '{cleanCostText}' không đúng định dạng!\n\n Chỉ được nhập số (ví dụ: 5000000 hoặc 5000000.50)", "Định dạng không hợp lệ", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    if (!decimal.TryParse(cleanCostText.Replace(',', '.'), System.Globalization.NumberStyles.Number, System.Globalization.CultureInfo.InvariantCulture, out parsedEditCost) || parsedEditCost < 0)
                    {
                        MessageBox.Show($" Lỗi: Chi phí '{cleanCostText}' phải là số không âm!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                }

                // Cập nhật thông tin vào đối tượng Phong gốc
                // Mã phòng (khóa chính) thường không nên thay đổi
                // phongToUpdate.MaPhong = EditingRoomNumber;
                phongToUpdate.DienTich = parsedEditArea;
                phongToUpdate.GiaThue = parsedEditCost;
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