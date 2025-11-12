using CommunityToolkit.Mvvm.ComponentModel;
using RoomManagementSystem.DataLayer;
using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using Microsoft.Win32;
using RoomManagementSystem.BusinessLayer;
using CommunityToolkit.Mvvm.Input;


// ----- LỚP: TenantSelectItemViewModel -----
namespace RoomManagementSystem.Presentation.ViewModels
{
    // LỚP HỖ TRỢ CHO YÊU CẦU 2 (CHỌN NHIỀU NGƯỜI THUÊ)
    public partial class TenantSelectItemViewModel : ObservableObject
    {
        public NguoiThue Tenant { get; }

        [ObservableProperty]
        private bool _isSelected;

        [ObservableProperty]
        private bool _isEnabled = true;

        public string HoTen => Tenant.HoTen;
        public string MaNguoiThue => Tenant.MaNguoiThue;
        public TenantSelectItemViewModel(NguoiThue tenant)
        {
            Tenant = tenant;
            _isSelected = false;
        }
    }

    // ----- LỚP: ContractManagementViewModel (Đã cập nhật) -----
    public partial class ContractManagementViewModel : ViewModelBase
    {
        // --- Properties cho Popup Thêm ---
        [ObservableProperty] private bool _isAddContractPopupVisible;
        // Thay đổi danh sách người thuê
        [ObservableProperty] private ObservableCollection<TenantSelectItemViewModel> _tenantList;
        [ObservableProperty] private ObservableCollection<Phong> _availableRoomList;
        [ObservableProperty] private Phong _selectedNewRoom;
        [ObservableProperty] private decimal _newContractDeposit;
        [ObservableProperty] private string _newContractNotes;
        [ObservableProperty] private DateTime? _newContractStartDate;
        [ObservableProperty] private int _newContractDuration;

        [ObservableProperty] // <-- THÊM MỚI (Cho Yêu cầu 2)
        private ObservableCollection<TenantSelectItemViewModel> _editingTenantList;

        // --- Properties cho Danh sách chính ---
        private readonly ObservableCollection<ContractItemViewModel> _allContracts;
        [ObservableProperty] private ObservableCollection<ContractItemViewModel> _contractList;
        [ObservableProperty] private ContractItemViewModel _selectedContract;
        [ObservableProperty] private string _searchKeyword;
        // --- Properties cho Popup Xóa ---
        [ObservableProperty] private bool _isDeleteConfirmationVisible;
        // --- Properties cho các Tab ---
        [ObservableProperty] private string _selectedTab = "Xem";
        [ObservableProperty] private FlowDocument _contractDocument;
        [ObservableProperty] private ContractDetailViewModel _detailedContractInfo;
        [ObservableProperty] private ObservableCollection<ContractHistoryItemViewModel> _contractHistory;
        [ObservableProperty] private HopDong _editingContract;
        [ObservableProperty] private ObservableCollection<NotificationItemViewModel> _notificationList;
        // --- Services ---
        private readonly QuanLyNguoiThue _tenantService;
        private readonly QL_TaiSan_Phong _roomService;
        private readonly QL_HopDong _contractService;

        public ContractManagementViewModel()
        {
            _tenantService = new QuanLyNguoiThue();
            _roomService = new QL_TaiSan_Phong();
            _contractService = new QL_HopDong();

            _allContracts = new ObservableCollection<ContractItemViewModel>();
            _contractList = new ObservableCollection<ContractItemViewModel>();
            // Khởi tạo danh sách mới
            _tenantList = new ObservableCollection<TenantSelectItemViewModel>();
            _editingTenantList = new ObservableCollection<TenantSelectItemViewModel>();
            _availableRoomList = new ObservableCollection<Phong>();
            _contractHistory = new ObservableCollection<ContractHistoryItemViewModel>();
            _notificationList = new ObservableCollection<NotificationItemViewModel>();

            LoadContracts();
            // Kích hoạt kiểm tra hợp đồng hết hạn ở luồng nền
            Task.Run(() =>
            {
                try
                {
                    _contractService.KiemTraVaTaoThongBaoHetHan();
                }
                catch (Exception)
                {
                    /* Tùy chọn: Ghi log lỗi nếu tác vụ nền thất bại */
                }
            });
            // KẾT THÚC THÊM MỚI
        }

        /// <summary>
        /// Tự động cập nhật tiền cọc khi người dùng chọn một phòng mới.
        /// </summary>
        partial void OnSelectedNewRoomChanged(Phong value)
        {
            if (value != null)
            {
                // Lấy giá thuê của phòng đã chọn và gán vào Tiền cọc
                NewContractDeposit = value.GiaThue;
            }
            else
            {
                NewContractDeposit = 0;
            }
        }


        private void LoadContracts(string selectContractId = null)
        {
            try
            {
                var contractsWithTenants = _contractService.GetContractsWithTenantNames();
                _allContracts.Clear();
                Application.Current.Dispatcher.Invoke(() =>
                {
                    ContractList.Clear();
                    foreach (var entry in contractsWithTenants)
                    {
                        var contractVM = new ContractItemViewModel(entry.Key) { TenantName = entry.Value };
                        _allContracts.Add(contractVM);
                        ContractList.Add(contractVM);
                    }
                });
                ContractItemViewModel itemToSelect = null;
                if (selectContractId != null)
                {
                    itemToSelect = ContractList.FirstOrDefault(c => c.OriginalContract.MaHopDong == selectContractId);
                }

                SelectedContract = itemToSelect ?? ContractList.FirstOrDefault();
            }
            catch (Exception ex) { MessageBox.Show($"Lỗi khi tải danh sách hợp đồng: {ex.Message}"); }
        }

        partial void OnSelectedContractChanged(ContractItemViewModel value)
        {
            if (value != null)
            {
                string maHopDong = value.OriginalContract.MaHopDong;
                LoadContractDocument(maHopDong);
                LoadDetailedContractInfo(maHopDong);
                LoadContractHistory(maHopDong);
                //LoadNotifications(maHopDong);
                LoadDataForEdit(value.OriginalContract);
                LoadNotifications(value.OriginalContract.MaHopDong);
            }
            else
            {
                ContractDocument = null;
                DetailedContractInfo = null;
                ContractHistory?.Clear();
                NotificationList?.Clear();
                EditingContract = null;
            }
        }

        private void LoadNotifications(string maHopDong)
        {
            try
            {
                NotificationList.Clear();
                var notifications = new List<NotificationItemViewModel>();

                // 1. Lấy thông báo từ Lịch sử thay đổi
                var historyList = _contractService.GetContractHistory(maHopDong);
                var originalContract = _contractService.LayChiTietHopDong(maHopDong);
                foreach (var history in historyList)
                {
                    notifications.Add(new NotificationItemViewModel
                    {
                        LoaiThongBao = history.HanhDong,
                        NguoiLienQuan = history.TenNguoiThayDoi,
                        NgayThongBao = history.NgayThayDoi,
                        NoiDung = history.NoiDungThayDoi,
                        Phong = originalContract?.MaPhong,
                        BatDau = originalContract?.NgayBatDau,
                        KetThuc = originalContract?.NgayKetThuc,
                        TienCoc = $"{originalContract?.TienCoc:N0} VND"
                    });
                }

                // 2. Lấy thông báo từ Bảng thông báo hạn
                var expiryNotifications = _contractService.GetNotificationsByContractId(maHopDong);
                foreach (var expiryNotif in expiryNotifications)
                {
                    notifications.Add(new NotificationItemViewModel
                    {
                        LoaiThongBao = "Đến hạn kết thúc",
                        NguoiLienQuan = originalContract?.TenNguoiThue ?? "Hệ thống",
                        NgayThongBao = expiryNotif.NgayThongBao,
                        NoiDung = expiryNotif.NoiDung,
                        Phong = originalContract?.MaPhong,
                        BatDau = originalContract?.NgayBatDau,
                        KetThuc = originalContract?.NgayKetThuc,
                        TienCoc = $"{originalContract?.TienCoc:N0} VND"
                    });
                }

                // 3. Sắp xếp và thêm vào Collection
                int stt = 1;
                foreach (var notif in notifications.OrderByDescending(n => n.NgayThongBao))
                {
                    notif.Stt = stt++;
                    NotificationList.Add(notif);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải lịch sử thông báo: {ex.Message}");
            }
        }

        // Cập nhật cách tải danh sách người thuê
        private void LoadDataForPopup()
        {
            try
            {
                TenantList.Clear();
                var tenants = _tenantService.GetTenantsWithoutActiveContract();
                foreach (var tenant in tenants)
                {
                    // Thêm vào danh sách mới
                    TenantList.Add(new TenantSelectItemViewModel(tenant));
                }

                AvailableRoomList.Clear();
                var rooms = _roomService.GetAvailableRooms();
                foreach (var room in rooms) { AvailableRoomList.Add(room); }
            }
            catch (Exception ex) { MessageBox.Show($"Lỗi khi tải dữ liệu cho popup: {ex.Message}"); }
        }


        // Cập nhật logic tải văn bản hợp đồng
        private void LoadContractDocument(string maHopDong)
        {
            var docToDisplay = new FlowDocument();
            try
            {
                // 1. Gọi BLL để lấy MemoryStream của file RTF ĐÃ ĐƯỢC TRỘN DỮ LIỆU
                using (var ms = _contractService.GetMergedContractDocument(maHopDong))
                {
                    if (ms == null || ms.Length == 0)
                    {
                        throw new Exception("Không thể tạo file hợp đồng từ dữ liệu.");
                    }

                    // 2. Tải MemoryStream RTF vào FlowDocument
                    ms.Position = 0;
                    var textRange = new TextRange(docToDisplay.ContentStart, docToDisplay.ContentEnd);
                    textRange.Load(ms, DataFormats.Rtf);
                }

                ContractDocument = docToDisplay;
            }
            catch (Exception ex)
            {
                docToDisplay.Blocks.Add(new Paragraph(new Run($"Đã xảy ra lỗi khi tải file hợp đồng: {ex.Message}")));
                ContractDocument = docToDisplay;
            }
        }


        private void LoadDetailedContractInfo(string maHopDong)
        {
            try
            {
                var contractData = _contractService.LayChiTietHopDong(maHopDong);
                if (contractData != null)
                {
                    DetailedContractInfo = new ContractDetailViewModel(contractData);
                }
            }
            catch (Exception ex) { MessageBox.Show($"Lỗi khi tải thông tin chi tiết: {ex.Message}"); }
        }

        private void LoadContractHistory(string maHopDong)
        {
            try
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    ContractHistory.Clear();
                    var historyList = _contractService.GetContractHistory(maHopDong);
                    int stt = 1;
                    foreach (var history in historyList)
                    {
                        ContractHistory.Add(new ContractHistoryItemViewModel
                        {
                            Stt = stt++,
                            NgaySua = history.NgayThayDoi,
                            NguoiThucHien = history.TenNguoiThayDoi,
                            HanhDong = history.HanhDong,
                            NoiDung = history.NoiDungThayDoi
                        });
                    }
                });
            }
            catch (Exception ex) { MessageBox.Show($"Lỗi khi tải lịch sử hợp đồng: {ex.Message}"); }
        }


        private void LoadDataForEdit(HopDong selected)
        {
            // 1. Sao chép các thuộc tính của hợp đồng đang chỉnh sửa
            EditingContract = new HopDong
            {
                MaHopDong = selected.MaHopDong,
                MaPhong = selected.MaPhong,
                TienCoc = selected.TienCoc,
                GhiChu = selected.GhiChu,
                NgayBatDau = selected.NgayBatDau,
                ThoiHan = selected.ThoiHan,
                ChuNha = selected.ChuNha,
                TrangThai = selected.TrangThai
            };

            // Tải danh sách người thuê cho popup chỉnh sửa
            try
            {
                EditingTenantList.Clear();
                var allTenantsDict = _tenantService.getAll().ToDictionary(t => t.MaNguoiThue);

                // 2. Lấy những người thuê HIỆN TẠI của hợp đồng này
                var currentTenantDetails = _contractService.GetAllHopDongNguoiThue()
                    .Where(d => d.MaHopDong == selected.MaHopDong);

                foreach (var detail in currentTenantDetails)
                {
                    if (allTenantsDict.TryGetValue(detail.MaNguoiThue, out var tenant))
                    {
                        var vm = new TenantSelectItemViewModel(tenant)
                        {
                            IsSelected = true,
                            IsEnabled = false // Không cho phép bỏ chọn người thuê hiện tại
                        };
                        EditingTenantList.Add(vm);
                    }
                }

                // 3. Lấy những người thuê CHƯA CÓ HỢP ĐỒNG
                var availableTenants = _tenantService.GetTenantsWithoutActiveContract();
                foreach (var tenant in availableTenants)
                {
                    var vm = new TenantSelectItemViewModel(tenant)
                    {
                        IsSelected = false,
                        IsEnabled = true // Cho phép chọn thêm
                    };
                    EditingTenantList.Add(vm);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải danh sách người thuê: {ex.Message}");
            }
            // KẾT THÚC THÊM MỚI
        }

        [RelayCommand]
        private void ChangeTab(string tabName) => SelectedTab = tabName;
        [RelayCommand]
        private void OpenAddContractPopup()
        {
            LoadDataForPopup();
            // NewContractDeposit = 0; // <-- XÓA BỎ, sẽ được set tự động
            SelectedNewRoom = null;
            // Đảm bảo chọn phòng để kích hoạt OnSelectedNewRoomChanged
            NewContractNotes = string.Empty;
            NewContractStartDate = DateTime.Today;
            NewContractDuration = 12;
            IsAddContractPopupVisible = true;
        }

        [RelayCommand]
        private void CancelAddContract() => IsAddContractPopupVisible = false;
        // Cập nhật logic tạo hợp đồng
        [RelayCommand]
        private void CreateContract()
        {
            // 1. Lấy danh sách người thuê được chọn
            var selectedTenants = TenantList
                .Where(t => t.IsSelected)
                .Select(t => t.Tenant)
                .ToList();
            // 2. Validate
            if (selectedTenants.Count == 0 || SelectedNewRoom == null || NewContractStartDate == null)
            {
                MessageBox.Show("Vui lòng chọn ít nhất một người thuê, một phòng và ngày bắt đầu.");
                return;
            }
            if (NewContractDeposit <= 0)
            {
                MessageBox.Show("Tiền cọc phải là số lớn hơn 0 và chỉ được nhập số!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                // 3. Chuẩn bị dữ liệu
                string maNguoiThueChuHopDong = selectedTenants.First().MaNguoiThue;
                List<string> maNguoiThueOCung = selectedTenants.Skip(1).Select(t => t.MaNguoiThue).ToList();

                HopDong newContract = new HopDong
                {
                    MaPhong = SelectedNewRoom.MaPhong,
                    ChuNha = "ND001", // Giả định
                    TienCoc = NewContractDeposit,
                    NgayBatDau = NewContractStartDate.Value,
                    ThoiHan = NewContractDuration,
                    TrangThai = "Hiệu lực",
                    GhiChu = NewContractNotes
                };

                // 4. Gọi BLL với chữ ký mới
                string newContractId = _contractService.TaoHopDong(newContract, maNguoiThueChuHopDong, maNguoiThueOCung);
                MessageBox.Show($"Tạo hợp đồng {newContractId} thành công!\n" +
                                $"Chủ hợp đồng: {selectedTenants.First().HoTen}\n" +
                                $"{maNguoiThueOCung.Count} người ở cùng.");
                IsAddContractPopupVisible = false;
                LoadContracts(newContractId); // Tải lại và chọn hợp đồng vừa tạo
            }
            catch (Exception ex) { MessageBox.Show($"Lỗi khi tạo hợp đồng: {ex.Message}"); }
        }


        [RelayCommand]
        private void OpenDeleteConfirmation()
        {
            if (SelectedContract == null) { MessageBox.Show("Vui lòng chọn hợp đồng để xóa."); return; }
            IsDeleteConfirmationVisible = true;
        }

        [RelayCommand]
        private void ConfirmDeleteContract()
        {
            if (SelectedContract == null) return;
            try
            {
                if (_contractService.XoaHopDong(SelectedContract.OriginalContract.MaHopDong))
                {
                    MessageBox.Show("Xóa hợp đồng thành công!");
                    LoadContracts();
                }
                else { MessageBox.Show("Xóa hợp đồng thất bại."); }
            }
            catch (Exception ex) { MessageBox.Show($"Lỗi khi xóa hợp đồng: {ex.Message}"); }
            finally { IsDeleteConfirmationVisible = false; }
        }

        [RelayCommand]
        private void CancelDeleteContract() => IsDeleteConfirmationVisible = false; // <-- SỬA LỖI LOGIC

        [RelayCommand]
        private void SaveContractChanges()
        {
            if (EditingContract == null) return;
            if (EditingContract.TienCoc <= 0)
            {
                MessageBox.Show("Tiền cọc phải là số lớn hơn 0 và chỉ được nhập số!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string currentContractId = EditingContract.MaHopDong;

            // Lấy danh sách người thuê mới
            var newlyAddedTenantIds = EditingTenantList
                .Where(t => t.IsEnabled && t.IsSelected) // Chỉ lấy những người mới được thêm (IsEnabled) và được chọn
                .Select(t => t.Tenant.MaNguoiThue)
                .ToList();
            // KẾT THÚC THÊM MỚI

            try
            {
                string currentUser = "ND001";
                if (_contractService.CapNhatHopDong(EditingContract, currentUser, newlyAddedTenantIds)) //
                {
                    MessageBox.Show("Cập nhật hợp đồng thành công!");
                    LoadContracts(currentContractId);
                }
                else { MessageBox.Show("Cập nhật hợp đồng thất bại."); }
            }
            catch (Exception ex) { MessageBox.Show($"Lỗi khi cập nhật hợp đồng: {ex.Message}"); }
        }

        [RelayCommand]
        private void DownloadContract()
        {
            if (SelectedContract == null)
            {
                MessageBox.Show("Vui lòng chọn hợp đồng cần tải xuống.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            try
            {
                string maHopDong = SelectedContract.OriginalContract.MaHopDong;
                string defaultFileName = $"HopDong_{maHopDong}.pdf";

                SaveFileDialog saveDialog = new SaveFileDialog
                {
                    Title = "Chọn nơi lưu hợp đồng (PDF)",
                    FileName = defaultFileName,
                    Filter = "PDF Documents (*.pdf)|*.pdf", // Chỉ cho phép lưu PDF
                    DefaultExt = ".pdf"
                };
                bool? result = saveDialog.ShowDialog();
                if (result == true)
                {
                    // 1. Gọi BLL để lấy MemoryStream của file PDF
                    using (var ms = _contractService.GetMergedContractDocumentAsPdf(maHopDong))
                    {
                        if (ms == null || ms.Length == 0)
                        {
                            throw new Exception("Không thể tạo file PDF từ dữ liệu hợp đồng.");
                        }

                        // 2. Lưu MemoryStream xuống file
                        File.WriteAllBytes(saveDialog.FileName, ms.ToArray());
                    }

                    MessageBox.Show($"Đã tải xuống file PDF thành công:\n{saveDialog.FileName}", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải hợp đồng PDF: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            // KẾT THÚC SỬA ĐỔI
        }

        [RelayCommand] private void SendContract() => MessageBox.Show("Chức năng gửi đang được phát triển.");
    }
}