using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RoomManagementSystem.BusinessLayer;
using RoomManagementSystem.DataLayer;
using System;
using System.Collections.Generic; // Cần cho List<T>
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace RoomManagementSystem.Presentation.ViewModels
{
    public partial class HouseManagementViewModel : ViewModelBase
    {
        private readonly QL_TaiSan_Phong _service;

        [ObservableProperty]
        private ObservableCollection<Nha> _danhSachNha;

        [ObservableProperty]
        private ObservableCollection<Phong> _danhSachPhongHienThi;

        // ✅ VIẾT LẠI THUỘC TÍNH NÀY MỘT CÁCH THỦ CÔNG ĐỂ ĐẢM BẢO HOẠT ĐỘNG
        private Nha _selectedNha;
        public Nha SelectedNha
        {
            get => _selectedNha;
            set
            {
                if (SetProperty(ref _selectedNha, value))
                {
                    OnSelectedNhaChanged(value); // Gọi logic cập nhật thủ công
                }
            }
        }

        private List<Phong> _allRooms = new List<Phong>();


        // --- ✅ BẮT ĐẦU PHẦN ĐIỀU KHIỂN POPUP THÊM NHÀ ---

        [ObservableProperty]
        private bool _isAddHousePopupVisible;

        // Các thuộc tính để binding với các ô TextBox trong popup
        [ObservableProperty]
        private string _newHouseAddress;

        [ObservableProperty]
        private string _newHouseNotes;

        // --- ✅ KẾT THÚC PHẦN ĐIỀU KHIỂN POPUP THÊM NHÀ ---


        // --- ✅ BẮT ĐẦU PHẦN ĐIỀU KHIỂN POPUP SỬA NHÀ ---

        [ObservableProperty]
        private bool _isEditHousePopupVisible;

        // Các thuộc tính để binding với các ô TextBox trong popup "Sửa"
        [ObservableProperty]
        private string _editingHouseAddress;

        [ObservableProperty]
        private string _editingHouseNotes;

        // --- ✅ KẾT THÚC PHẦN ĐIỀU KHIỂN POPUP SỬA NHÀ ---


        [ObservableProperty]
        private bool _isDeleteHousePopupVisible;


        // --- ✅ BẮT ĐẦU PHẦN ĐIỀU KHIỂN POPUP THÊM PHÒNG ---
        [ObservableProperty]
        private bool _isAddRoomPopupVisible;

        // Các thuộc tính để binding với các ô TextBox trong popup "Thêm Phòng"
        [ObservableProperty]
        private string _newRoomNumber;

        [ObservableProperty]
        private decimal _newRoomArea;

        [ObservableProperty]
        private decimal _newRoomCost;

        [ObservableProperty]
        private string _newRoomNotes;
        // --- ✅ KẾT THÚC PHẦN ĐIỀU KHIỂN POPUP THÊM PHÒNG ---


        [ObservableProperty]
        private Phong _selectedPhong;

        // --- ✅ BẮT ĐẦU PHẦN ĐIỀU KHIỂN POPUP SỬA PHÒNG ---
        [ObservableProperty]
        private bool _isEditRoomPopupVisible;

        // Các thuộc tính để binding với các ô TextBox trong popup "Sửa Phòng"
        [ObservableProperty]
        private string _editingRoomNumber;

        [ObservableProperty]
        private decimal _editingRoomArea;

        [ObservableProperty]
        private decimal _editingRoomCost;

        [ObservableProperty]
        private string _editingRoomNotes;
        // --- ✅ KẾT THÚC PHẦN ĐIỀU KHIỂN POPUP SỬA PHÒNG ---

        public HouseManagementViewModel()
        {
            _service = new QL_TaiSan_Phong();
            _danhSachNha = new ObservableCollection<Nha>();
            _danhSachPhongHienThi = new ObservableCollection<Phong>();
            LoadHouseData();
        }

        // Đổi tên hàm, bây giờ chỉ tải danh sách nhà
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

        private void OnSelectedNhaChanged(Nha value)
        {
            DanhSachPhongHienThi.Clear();

            if (value != null)
            {
                try
                {
                    // ✅ GỌI ĐÚNG PHƯƠNG THỨC VỚI THAM SỐ LÀ MaNha CỦA NHÀ ĐƯỢC CHỌN
                    var roomsInHouse = _service.DanhSachPhong(value.MaNha);
                    foreach (var room in roomsInHouse)
                    {
                        DanhSachPhongHienThi.Add(room);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi tải danh sách phòng: {ex.Message}");
                }
            }
        }

        // CÁC COMMAND CHO CÁC NÚT BẤM (tạm thời hiển thị MessageBox)
        // THAY THẾ LỆNH GIẢ 'AddHouse' BẰNG LỆNH CÓ LOGIC NÀY
        [RelayCommand]
        private void AddHouse()
        {
            NewHouseAddress = string.Empty;
            NewHouseNotes = string.Empty;
            IsAddHousePopupVisible = true;
        }

        // ✅ THÊM CÁC COMMAND MỚI
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
                    MessageBox.Show("Xóa nhà thất bại!");
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
                    DienTich = (float)NewRoomArea,  // ✅ ĐÃ SỬA: Ép kiểu sang float
                    GiaThue = (float)NewRoomCost,   // ✅ ĐÃ SỬA: Ép kiểu sang float
                    GhiChu = NewRoomNotes,
                    TrangThai = "Trống" // Mặc định là trống
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
        private void EditRoom()
        {
            if (SelectedPhong == null)
            {
                MessageBox.Show("Vui lòng chọn một phòng để chỉnh sửa.");
                return;
            }

            // Tải dữ liệu từ phòng đã chọn vào các thuộc tính "Editing"
            EditingRoomNumber = SelectedPhong.MaPhong;
            EditingRoomArea = (decimal)SelectedPhong.DienTich; // Ép kiểu ngược lại
            EditingRoomCost = (decimal)SelectedPhong.GiaThue;   // Ép kiểu ngược lại
            EditingRoomNotes = SelectedPhong.GhiChu;

            IsEditRoomPopupVisible = true;
        }

        [RelayCommand]
        private void SaveEditRoom()
        {
            try
            {
                if (SelectedPhong == null) return; // Kiểm tra an toàn

                if (string.IsNullOrWhiteSpace(EditingRoomNumber))
                {
                    MessageBox.Show("Vui lòng nhập số phòng.");
                    return;
                }

                // Cập nhật thông tin cho đối tượng SelectedPhong
                // (Mã phòng là khóa chính, thường không nên cho sửa)
                // SelectedPhong.MaPhong = EditingRoomNumber; 
                SelectedPhong.DienTich = (float)EditingRoomArea;
                SelectedPhong.GiaThue = (float)EditingRoomCost;
                SelectedPhong.GhiChu = EditingRoomNotes;
                // Lưu ý: LoaiPhong và TrangThai sẽ được giữ nguyên

                // Gọi hàm CapNhatPhong từ BusinessLayer (bạn đã có)
                if (_service.CapNhatPhong(SelectedPhong))
                {
                    MessageBox.Show("Cập nhật phòng thành công!");
                    OnSelectedNhaChanged(SelectedNha); // Tải lại danh sách phòng
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
        }

        [RelayCommand]
        private void CancelEditRoom()
        {
            IsEditRoomPopupVisible = false;
        }
        [RelayCommand] private void DeleteRoom() { MessageBox.Show("Delete Room Clicked!"); }
    }
}