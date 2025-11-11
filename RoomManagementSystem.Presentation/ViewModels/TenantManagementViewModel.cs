using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RoomManagementSystem.BusinessLayer;
using RoomManagementSystem.DataLayer;
using RoomManagementSystem.Presentation.ViewModels; // Đảm bảo có using này
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace RoomManagementSystem.Presentation.ViewModels
{
    public partial class TenantManagementViewModel : ViewModelBase
    {
        private readonly QuanLyNguoiThue _nguoiThueService;
        private readonly QL_TaiSan_Phong _taiSanPhongService;

        // ✅ ĐÃ SỬA: Kiểu của danh sách là TenantWrapperViewModel
        [ObservableProperty]
        private ObservableCollection<TenantWrapperViewModel> _danhSachNguoiThue;

        [ObservableProperty]
        private bool _isAllSelected;

        // --- Các thuộc tính điều khiển Popup ---
        [ObservableProperty] private bool _isAddPopupVisible;
        [ObservableProperty] private bool _isDeleteConfirmationVisible;
        [ObservableProperty] private bool _isEditPopupVisible;
        [ObservableProperty] private NguoiThue _editingTenant; // Dùng để binding với popup sửa
        [ObservableProperty] private string _newTenantName = "";
        [ObservableProperty] private string _newTenantEmail = "";
        [ObservableProperty] private string _newTenantPhone = "";
        [ObservableProperty] private string _newTenantIdCard = "";
        [ObservableProperty] private DateTime? _newTenantStartDate = DateTime.Now;
        [ObservableProperty] private ObservableCollection<Phong> _availableRooms;
        [ObservableProperty] private Phong _selectedRoomForNewTenant;

        // --- Constructor ---
        public TenantManagementViewModel()
        {
            _nguoiThueService = new QuanLyNguoiThue();
            _taiSanPhongService = new QL_TaiSan_Phong();
            _danhSachNguoiThue = new ObservableCollection<TenantWrapperViewModel>();
            _availableRooms = new ObservableCollection<Phong>();
            LoadData();
        }

        // --- Logic Tải Dữ Liệu ---
        private void LoadData()
        {
            try
            {
                var danhSach = _nguoiThueService.getAll();
                DanhSachNguoiThue.Clear();
                foreach (var nguoiThue in danhSach)
                {
                    // ✅ SỬA LỖI: Bọc đối tượng NguoiThue trong TenantWrapperViewModel trước khi thêm
                    DanhSachNguoiThue.Add(new TenantWrapperViewModel(nguoiThue));
                }
                UpdateSelectionState(); // Cập nhật lại trạng thái checkbox "chọn tất cả"
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Không thể tải danh sách người thuê: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // --- Logic Xử Lý Checkbox ---
        partial void OnIsAllSelectedChanged(bool value)
        {
            foreach (var tenantVM in DanhSachNguoiThue)
            {
                tenantVM.IsSelected = value;
            }
        }

        [RelayCommand]
        private void TenantSelectionChanged()
        {
            UpdateSelectionState();
        }

        private void UpdateSelectionState()
        {
            if (DanhSachNguoiThue.Any())
            {
                var allSelected = DanhSachNguoiThue.All(t => t.IsSelected);
                SetProperty(ref _isAllSelected, allSelected, nameof(IsAllSelected));
            }
            else
            {
                SetProperty(ref _isAllSelected, false, nameof(IsAllSelected));
            }
        }

        // --- Logic Thêm Người Thuê ---
        [RelayCommand]
        private void OpenAddPopup()
        {
            // Reset dữ liệu cũ
            NewTenantName = "";
            NewTenantEmail = "";
            NewTenantPhone = "";
            NewTenantIdCard = "";
            NewTenantStartDate = DateTime.Now;
            SelectedRoomForNewTenant = null;
            IsAddPopupVisible = true;
        }

        [RelayCommand]
        private void SaveNewTenant()
        {
            try
            {
                // ✅ VALIDATION: Kiểm tra các trường bắt buộc
                if (string.IsNullOrWhiteSpace(NewTenantName))
                {
                    MessageBox.Show("Vui lòng nhập tên người thuê!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(NewTenantIdCard))
                {
                    MessageBox.Show("Vui lòng nhập số giấy tờ (CCCD/CMND)!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // ✅ VALIDATION: Kiểm tra số CCCD phải có đúng 12 số
                if (!string.IsNullOrWhiteSpace(NewTenantIdCard))
                {
                    if (!System.Text.RegularExpressions.Regex.IsMatch(NewTenantIdCard, @"^\d{12}$"))
                    {
                        MessageBox.Show("Số CCCD phải có đúng 12 chữ số!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                }

                // ✅ VALIDATION: Kiểm tra số điện thoại phải có đúng 10 số
                if (!string.IsNullOrWhiteSpace(NewTenantPhone))
                {
                    if (!System.Text.RegularExpressions.Regex.IsMatch(NewTenantPhone, @"^\d{10}$"))
                    {
                        MessageBox.Show("Số điện thoại phải có đúng 10 chữ số!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                }

                // ✅ VALIDATION: Kiểm tra định dạng email
                if (!string.IsNullOrWhiteSpace(NewTenantEmail))
                {
                    if (!System.Text.RegularExpressions.Regex.IsMatch(NewTenantEmail, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                    {
                        MessageBox.Show("Email không đúng định dạng! Vui lòng nhập email có @ và dấu chấm.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                }

                var nguoiThueMoi = new NguoiThue
                {
                    HoTen = NewTenantName,
                    Email = NewTenantEmail,
                    Sdt = NewTenantPhone,
                    SoGiayTo = NewTenantIdCard,
                };

                if (_nguoiThueService.ThemNguoiThue(nguoiThueMoi))
                {
                    MessageBox.Show("Thêm người thuê thành công!");
                    LoadData();
                    IsAddPopupVisible = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        [RelayCommand]
        private void CancelAdd() { IsAddPopupVisible = false; }

        // --- Logic Sửa Người Thuê ---
        [RelayCommand]
        private void OpenEditTenantDialog()
        {
            var selectedTenants = DanhSachNguoiThue.Where(t => t.IsSelected).ToList();

            if (selectedTenants.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn một người thuê để sửa!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            if (selectedTenants.Count > 1)
            {
                MessageBox.Show("Chỉ có thể chỉnh sửa một người thuê tại một thời điểm.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var tenantWrapperToEdit = selectedTenants.First();
            // ✅ SỬA LỖI: Lấy đối tượng NguoiThue gốc từ thuộc tính .Tenant
            var tenantToEdit = tenantWrapperToEdit.Tenant;

            // Tạo một BẢN SAO để chỉnh sửa, tránh thay đổi dữ liệu gốc nếu người dùng nhấn Hủy
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

            IsEditPopupVisible = true;
        }

        [RelayCommand]
        private void SaveEdit()
        {
            if (EditingTenant == null) return;
            try
            {
                // ✅ VALIDATION: Kiểm tra các trường bắt buộc
                if (string.IsNullOrWhiteSpace(EditingTenant.HoTen))
                {
                    MessageBox.Show("Vui lòng nhập tên người thuê!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(EditingTenant.SoGiayTo))
                {
                    MessageBox.Show("Vui lòng nhập số giấy tờ (CCCD/CMND)!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // ✅ VALIDATION: Kiểm tra số CCCD phải có đúng 12 số
                if (!string.IsNullOrWhiteSpace(EditingTenant.SoGiayTo))
                {
                    if (!System.Text.RegularExpressions.Regex.IsMatch(EditingTenant.SoGiayTo, @"^\d{12}$"))
                    {
                        MessageBox.Show("Số CCCD phải có đúng 12 chữ số!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                }

                // ✅ VALIDATION: Kiểm tra số điện thoại phải có đúng 10 số
                if (!string.IsNullOrWhiteSpace(EditingTenant.Sdt))
                {
                    if (!System.Text.RegularExpressions.Regex.IsMatch(EditingTenant.Sdt, @"^\d{10}$"))
                    {
                        MessageBox.Show("Số điện thoại phải có đúng 10 chữ số!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                }

                // ✅ VALIDATION: Kiểm tra định dạng email
                if (!string.IsNullOrWhiteSpace(EditingTenant.Email))
                {
                    if (!System.Text.RegularExpressions.Regex.IsMatch(EditingTenant.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                    {
                        MessageBox.Show("Email không đúng định dạng! Vui lòng nhập email có @ và dấu chấm.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                }

                if (_nguoiThueService.CapNhatNguoiThue(EditingTenant))
                {
                    MessageBox.Show("Cập nhật thành công!");
                    LoadData();
                    IsEditPopupVisible = false;
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
        private void CancelEdit() { IsEditPopupVisible = false; }

        // --- Logic Xóa Người Thuê ---
        [RelayCommand]
        private void OpenDeleteTenantDialog()
        {
            if (!DanhSachNguoiThue.Any(t => t.IsSelected))
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
                var selectedTenantsVMs = DanhSachNguoiThue.Where(t => t.IsSelected).ToList();
                int successCount = 0;
                foreach (var tenantVM in selectedTenantsVMs)
                {
                    if (_nguoiThueService.XoaNguoiThue(tenantVM.Tenant.MaNguoiThue))
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
        private void CancelDelete() { IsDeleteConfirmationVisible = false; }
    }
}