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


        // ✅ THÊM THUỘC TÍNH NÀY
        // Dùng để binding với CheckBox "Chọn tất cả"
        [ObservableProperty]
        private bool _isAllSelected;

        

        // --- BẮT ĐẦU PHẦN ĐIỀU KHIỂN POPUP ---

        [ObservableProperty]
        private bool _isAddPopupVisible;
        [ObservableProperty]
        private bool _isDeleteConfirmationVisible;

        [ObservableProperty]
        private bool _isEditPopupVisible;

        // Dùng để lưu trữ thông tin của người thuê đang được sửa
        [ObservableProperty]
        private NguoiThue _editingTenant;

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

        // ✅ THÊM COMMAND NÀY
        [RelayCommand]
        private void ToggleSelectAll()
        {
            foreach (var tenant in DanhSachNguoiThue)
            {
                tenant.IsSelected = IsAllSelected;
            }
        }


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

            

            IsAddPopupVisible = true;
        }

        [RelayCommand]
        private void SaveNewTenant()
        {
            try
            {
                var nguoiThueMoi = new NguoiThue
                {
                    

                    HoTen = NewTenantName,
                    Email = NewTenantEmail,
                    Sdt = NewTenantPhone,

                    // Lấy số giấy tờ từ TextBox
                    SoGiayTo = NewTenantIdCard,

                   
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


        // ✅ THÊM CÁC COMMAND MỚI CHO LOGIC XÓA

        // Thay thế Command trống cũ bằng Command có logic này
        [RelayCommand]
        private void OpenDeleteTenantDialog()
        {
            // Tìm những người thuê đang được chọn
            var selectedTenants = DanhSachNguoiThue.Where(t => t.IsSelected).ToList();

            if (!selectedTenants.Any())
            {
                MessageBox.Show("Vui lòng chọn ít nhất một người thuê để xóa.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

           
            IsDeleteConfirmationVisible = true;
        }

        [RelayCommand]
        private void ConfirmDelete()
        {
            try
            {
                var selectedTenants = DanhSachNguoiThue.Where(t => t.IsSelected).ToList();
                int successCount = 0;

                foreach (var tenant in selectedTenants)
                {
                    if (_nguoiThueService.XoaNguoiThue(tenant.MaNguoiThue))
                    {
                        successCount++;
                    }
                }
                MessageBox.Show($"Đã xóa thành công {successCount} người thuê.", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi xóa: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsDeleteConfirmationVisible = false;
                LoadData();
            }
        }

        [RelayCommand]
        private void CancelDelete()
        {
            IsDeleteConfirmationVisible = false;
        }




        // --- ✅ LOGIC MỚI CHO VIỆC SỬA ---

        [RelayCommand]
        private void OpenEditTenantDialog()
        {
            // 1. Lấy danh sách TẤT CẢ những người thuê đang được chọn
            var selectedTenants = DanhSachNguoiThue.Where(t => t.IsSelected).ToList();

            // 2. Kiểm tra số lượng người được chọn
            if (selectedTenants.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn một người thuê để sửa!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (selectedTenants.Count > 1)
            {
                MessageBox.Show("Chỉ có thể chỉnh sửa thông tin của một người thuê tại một thời điểm.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // 3. Nếu chỉ có MỘT người được chọn, tiếp tục
            var tenantToEdit = selectedTenants.First();

            // Tạo một BẢN SAO của người thuê để chỉnh sửa.
            // Điều này rất quan trọng: nếu người dùng nhấn "Hủy", dữ liệu gốc trong danh sách sẽ không bị thay đổi.
            EditingTenant = new NguoiThue
            {
                MaNguoiThue = tenantToEdit.MaNguoiThue,
                HoTen = tenantToEdit.HoTen,
                Email = tenantToEdit.Email,
                Sdt = tenantToEdit.Sdt,
                SoGiayTo = tenantToEdit.SoGiayTo,
                NgayBatDauThue = tenantToEdit.NgayBatDauThue,
                NgayTao = tenantToEdit.NgayTao,
            };

            // 4. Hiển thị popup Sửa
            IsEditPopupVisible = true;
        }

        [RelayCommand]
        private void SaveEdit()
        {
            if (EditingTenant == null) return;

            try
            {
                // Gọi service để cập nhật thông tin
                if (_nguoiThueService.CapNhatNguoiThue(EditingTenant))
                {
                    MessageBox.Show("Cập nhật thành công!");
                    LoadData(); // Tải lại danh sách
                    IsEditPopupVisible = false; // Đóng popup
                }
                else
                {
                    MessageBox.Show("Cập nhật thất bại!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi cập nhật: {ex.Message}");
            }
        }

        [RelayCommand]
        private void CancelEdit()
        {
            IsEditPopupVisible = false;
        }

        
    }
}