using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RoomManagementSystem.BusinessLayer;
using RoomManagementSystem.DataLayer;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace RoomManagementSystem.Presentation.ViewModels
{
    public partial class MaintenanceManagementViewModel : ViewModelBase
    {
        private readonly QL_BaoTri _maintenanceService;
        private readonly QL_TaiSan_Phong _roomService;
        private readonly QuanLyNguoiThue _tenantService;
        private readonly QL_HopDong _contractService;

        // Danh sách gốc chứa TẤT CẢ các item từ CSDL, là nguồn dữ liệu chính
        private readonly List<MaintenanceItemViewModel> _allItems;

        // --- Properties cho Giao diện chính ---
        [ObservableProperty] private ObservableCollection<MaintenanceItemViewModel> _maintenanceList;
        [ObservableProperty] private string _searchKeyword;
        [ObservableProperty] private string _selectedSortOption = "Mới nhất";
        [ObservableProperty] private int _currentPage = 1;
        [ObservableProperty] private int _totalPages = 1;
        [ObservableProperty] private bool _isAllSelected;
        private readonly int _pageSize = 8;
        public string PaginationInfoText => $"Trang {CurrentPage} / {TotalPages}";

        // --- Properties cho Popup ---
        [ObservableProperty] private bool _isAddPopupVisible;
        [ObservableProperty] private bool _isDeletePopupVisible;
        [ObservableProperty] private bool _isEditPopupVisible;
        [ObservableProperty] private ObservableCollection<Phong> _roomList;
        [ObservableProperty] private ObservableCollection<NguoiThue> _tenantList;

        // --- Properties cho Popup Thêm ---
        [ObservableProperty] private string _newDescription;
        [ObservableProperty] private DateTime? _newRequestDate;

        // --- Properties cho Popup Sửa ---
        [ObservableProperty] private MaintenanceItemViewModel _selectedItemForEdit;
        [ObservableProperty] private string _editDescription;
        [ObservableProperty] private DateTime? _editRequestDate;
        [ObservableProperty] private DateTime? _editCompletionDate;
        [ObservableProperty] private decimal _editCost;
        [ObservableProperty] private string _editStatus;
        public List<string> StatusOptions { get; } = new List<string> { "Chưa xử lý", "Đang xử lý", "Hoàn tất" };


        private Phong _selectedRoom;
        public Phong SelectedRoom { get => _selectedRoom; set { if (SetProperty(ref _selectedRoom, value)) OnRoomSelectionChanged(value); } }
        private NguoiThue _selectedTenant;
        public NguoiThue SelectedTenant { get => _selectedTenant; set { if (SetProperty(ref _selectedTenant, value)) OnTenantSelectionChanged(value); } }
        private bool _isUpdatingSelection = false;

        private List<Phong> _allRoomsForPopup = new();
        private List<NguoiThue> _allTenantsForPopup = new();

        public MaintenanceManagementViewModel()
        {
            _maintenanceService = new QL_BaoTri();
            _roomService = new QL_TaiSan_Phong();
            _tenantService = new QuanLyNguoiThue();
            _contractService = new QL_HopDong();

            _allItems = new List<MaintenanceItemViewModel>();
            _maintenanceList = new ObservableCollection<MaintenanceItemViewModel>();
            _roomList = new ObservableCollection<Phong>();
            _tenantList = new ObservableCollection<NguoiThue>();

            LoadData();
        }

        private void LoadData()
        {
            try
            {
                var maintenanceData = _maintenanceService.GetAll();
                var oldSelections = _allItems.Where(i => i.IsSelected).Select(i => i.OriginalData.MaBaoTri).ToHashSet();
                _allItems.Clear();
                foreach (var item in maintenanceData)
                {
                    var vmItem = new MaintenanceItemViewModel(item);
                    if (oldSelections.Contains(item.MaBaoTri))
                    {
                        vmItem.IsSelected = true;
                    }
                    vmItem.SelectionChanged = UpdateSelectAllState;
                    _allItems.Add(vmItem);
                }
                RefreshDisplayList();
            }
            catch (Exception ex) { MessageBox.Show($"Lỗi khi tải dữ liệu bảo trì: {ex.Message}"); }
        }

        private void RefreshDisplayList()
        {
            IEnumerable<MaintenanceItemViewModel> filteredItems = _allItems;
            if (!string.IsNullOrWhiteSpace(SearchKeyword))
            {
                filteredItems = _allItems.Where(item =>
                    (item.Phong?.Contains(SearchKeyword, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (item.NguoiThue?.Contains(SearchKeyword, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (item.MoTa?.Contains(SearchKeyword, StringComparison.OrdinalIgnoreCase) ?? false));
            }

            var sortedItems = SelectedSortOption switch
            {
                "Phòng" => filteredItems.OrderBy(x => x.Phong),
                "Ngày hoàn thành" => filteredItems.OrderByDescending(x => x.NgayHoanThanh),
                _ => filteredItems.OrderByDescending(x => x.NgayYeuCau),
            };

            TotalPages = Math.Max(1, (int)Math.Ceiling(sortedItems.Count() / (double)_pageSize));
            CurrentPage = Math.Clamp(CurrentPage, 1, TotalPages);
            OnPropertyChanged(nameof(PaginationInfoText));

            var pagedItems = sortedItems.Skip((CurrentPage - 1) * _pageSize).Take(_pageSize);

            MaintenanceList.Clear();
            foreach (var item in pagedItems)
            {
                MaintenanceList.Add(item);
            }

            UpdateSelectAllState();
        }

        private void UpdateSelectAllState()
        {
            var allInPageSelected = MaintenanceList.Any() && MaintenanceList.All(i => i.IsSelected);
            SetProperty(ref _isAllSelected, allInPageSelected, nameof(IsAllSelected));
        }

        partial void OnIsAllSelectedChanged(bool value)
        {
            foreach (var item in MaintenanceList)
            {
                item.IsSelected = value;
            }
        }

        partial void OnSearchKeywordChanged(string value) { CurrentPage = 1; RefreshDisplayList(); }

        private void LoadDataForPopup()
        {
            try
            {
                _allRoomsForPopup = _roomService.GetAllRooms();
                _allTenantsForPopup = _tenantService.getAll();
                RoomList.Clear();
                foreach (var room in _allRoomsForPopup) RoomList.Add(room);
                TenantList.Clear();
                foreach (var tenant in _allTenantsForPopup) TenantList.Add(tenant);
            }
            catch (Exception ex) { MessageBox.Show($"Lỗi khi tải dữ liệu cho popup: {ex.Message}"); }
        }

        private void OnRoomSelectionChanged(Phong selectedRoom)
        {
            if (_isUpdatingSelection) return;
            _isUpdatingSelection = true;
            SetProperty(ref _selectedTenant, null, nameof(SelectedTenant));
            if (selectedRoom != null)
            {
                var tenantsInRoom = _tenantService.GetTenantsByRoom(selectedRoom.MaPhong);
                TenantList.Clear();
                foreach (var tenant in tenantsInRoom) { TenantList.Add(tenant); }
                SelectedTenant = TenantList.FirstOrDefault();
            }
            else { foreach (var tenant in _allTenantsForPopup) { TenantList.Add(tenant); } }
            _isUpdatingSelection = false;
        }

        private void OnTenantSelectionChanged(NguoiThue selectedTenant)
        {
            if (_isUpdatingSelection) return;
            _isUpdatingSelection = true;
            if (selectedTenant != null)
            {
                var contract = _contractService.GetActiveContractByTenant(selectedTenant.MaNguoiThue);
                if (contract != null)
                {
                    var roomToSelect = _allRoomsForPopup.FirstOrDefault(r => r.MaPhong == contract.MaPhong);
                    if (!Equals(_selectedRoom, roomToSelect))
                    {
                        SetProperty(ref _selectedRoom, roomToSelect, nameof(SelectedRoom));
                    }
                }
            }
            else { foreach (var room in _allRoomsForPopup) { RoomList.Add(room); } }
            _isUpdatingSelection = false;
        }

        [RelayCommand]
        private void Sort(string sortBy) { SelectedSortOption = sortBy; CurrentPage = 1; RefreshDisplayList(); }

        [RelayCommand]
        private void ChangePage(string direction)
        {
            if (direction == "Next" && CurrentPage < TotalPages) CurrentPage++;
            else if (direction == "Prev" && CurrentPage > 1) CurrentPage--;
            RefreshDisplayList();
        }

        // ===================================
        // ===== CHỨC NĂNG THÊM (ADD) =====
        // ===================================

        [RelayCommand]
        private void OpenAddPopup()
        {
            LoadDataForPopup();
            SelectedRoom = null;
            SelectedTenant = null;
            NewDescription = string.Empty;
            NewRequestDate = DateTime.Today;
            IsAddPopupVisible = true;
        }

        [RelayCommand] private void CancelAdd() => IsAddPopupVisible = false;

        [RelayCommand]
        private void ConfirmAdd()
        {
            if (SelectedRoom == null || string.IsNullOrWhiteSpace(NewDescription) || NewRequestDate == null)
            {
                MessageBox.Show("Vui lòng chọn phòng, nhập mô tả và ngày yêu cầu."); return;
            }
            try
            {
                var newMaintenance = new BaoTri
                {
                    MaPhong = SelectedRoom.MaPhong,
                    MaNguoiThue = SelectedTenant?.MaNguoiThue,
                    MoTa = NewDescription,
                    NgayYeuCau = NewRequestDate.Value,
                    TrangThaiXuLy = "Chưa xử lý", // Trạng thái mặc định
                    ChiPhi = 0
                };
                _maintenanceService.Insert(newMaintenance);
                MessageBox.Show("Thêm yêu cầu bảo trì thành công!");
                IsAddPopupVisible = false;
                LoadData();
            }
            catch (Exception ex) { MessageBox.Show($"Lỗi khi thêm yêu cầu: {ex.Message}"); }
        }

        // ===================================
        // ===== CHỨC NĂNG SỬA (EDIT) =====
        // ===================================

        [RelayCommand]
        private void OpenEditPopup()
        {
            var selectedItems = _allItems.Where(item => item.IsSelected).ToList();
            if (selectedItems.Count != 1)
            {
                MessageBox.Show("Vui lòng chọn đúng một yêu cầu để chỉnh sửa.", "Chọn lại");
                return;
            }

            SelectedItemForEdit = selectedItems.First();

            EditDescription = SelectedItemForEdit.MoTa;
            EditRequestDate = SelectedItemForEdit.NgayYeuCau;
            EditCompletionDate = SelectedItemForEdit.NgayHoanThanh;
            EditCost = SelectedItemForEdit.ChiPhi;
            EditStatus = SelectedItemForEdit.TrangThaiXuLy;

            IsEditPopupVisible = true;
        }

        [RelayCommand]
        private void CancelEdit() => IsEditPopupVisible = false;

        [RelayCommand]
        private void ConfirmEdit()
        {
            if (SelectedItemForEdit == null || string.IsNullOrWhiteSpace(EditDescription) || EditRequestDate == null)
            {
                MessageBox.Show("Mô tả và ngày yêu cầu không được để trống."); return;
            }
            try
            {
                var originalData = SelectedItemForEdit.OriginalData;
                originalData.MoTa = EditDescription;
                originalData.NgayYeuCau = EditRequestDate.Value;
                originalData.NgayHoanThanh = EditCompletionDate;
                originalData.ChiPhi = EditCost;
                originalData.TrangThaiXuLy = EditStatus;

               // _maintenanceService.Update(originalData);
                MessageBox.Show("Cập nhật thông tin thành công!");

                IsEditPopupVisible = false;
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi cập nhật: {ex.Message}");
            }
        }

        // ===================================
        // ===== CHỨC NĂNG XÓA (DELETE) =====
        // ===================================

        [RelayCommand]
        private void OpenDeletePopup()
        {
            // <<== ĐẶT BREAKPOINT Ở ĐÂY
            var selectedItems = _allItems.Where(item => item.IsSelected).ToList();

            if (!selectedItems.Any()) // Hoặc if (selectedItems.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn ít nhất một yêu cầu để xóa.", "Chưa chọn mục");
                return;
            }
            IsDeletePopupVisible = true;
        }

        [RelayCommand] private void CancelDelete() => IsDeletePopupVisible = false;

        [RelayCommand]
        private void ConfirmDelete()
        {
            try
            {
                var idsToDelete = _allItems.Where(i => i.IsSelected).Select(i => i.OriginalData.MaBaoTri).ToList();
                if (!idsToDelete.Any())
                {
                    MessageBox.Show("Không có mục nào được chọn để xóa."); IsDeletePopupVisible = false; return;
                }
                if (_maintenanceService.XoaYeuCau(idsToDelete))
                {
                    MessageBox.Show($"Đã xóa thành công {idsToDelete.Count} yêu cầu.");
                }
                else { MessageBox.Show("Không có yêu cầu nào được xóa."); }
            }
            catch (Exception ex) { MessageBox.Show($"Lỗi khi xóa: {ex.Message}"); }
            finally
            {
                IsDeletePopupVisible = false;
                LoadData();
            }
        }
    }
}