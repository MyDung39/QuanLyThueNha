using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RoomManagementSystem.BusinessLayer;
using RoomManagementSystem.DataLayer;
using Spire.Doc;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Documents;

namespace RoomManagementSystem.Presentation.ViewModels
{
    public partial class ContractManagementViewModel : ViewModelBase
    {
        // --- Properties cho Popup Thêm ---
        [ObservableProperty] private bool _isAddContractPopupVisible;
        [ObservableProperty] private ObservableCollection<NguoiThue> _tenantList;
        [ObservableProperty] private ObservableCollection<Phong> _availableRoomList;
        [ObservableProperty] private NguoiThue _selectedNewTenant;
        [ObservableProperty] private Phong _selectedNewRoom;
        [ObservableProperty] private decimal _newContractDeposit;
        [ObservableProperty] private string _newContractNotes;
        [ObservableProperty] private DateTime? _newContractStartDate;
        [ObservableProperty] private int _newContractDuration;

        // --- Properties cho Danh sách chính ---
        private readonly ObservableCollection<ContractItemViewModel> _allContracts; // Danh sách gốc để lọc
        [ObservableProperty] private ObservableCollection<ContractItemViewModel> _contractList; // Danh sách binding với UI
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
            _tenantList = new ObservableCollection<NguoiThue>();
            _availableRoomList = new ObservableCollection<Phong>();
            _contractHistory = new ObservableCollection<ContractHistoryItemViewModel>();
            _notificationList = new ObservableCollection<NotificationItemViewModel>();

            LoadContracts();
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
                // Lấy thông tin hợp đồng gốc để điền các trường còn thiếu
                var originalContract = _contractService.LayChiTietHopDong(maHopDong);

                foreach (var history in historyList)
                {
                    notifications.Add(new NotificationItemViewModel
                    {
                        LoaiThongBao = history.HanhDong,
                        NguoiLienQuan = history.TenNguoiThayDoi,
                        NgayThongBao = history.NgayThayDoi,
                        NoiDung = history.NoiDungThayDoi,

                        // ✅ ĐIỀN DỮ LIỆU BỔ SUNG (Lấy từ hợp đồng gốc)
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

                        // ✅ ĐIỀN DỮ LIỆU BỔ SUNG
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


        private void LoadDataForPopup()
        {
            try
            {
                TenantList.Clear();
                var tenants = _tenantService.GetTenantsWithoutActiveContract();
                foreach (var tenant in tenants) { TenantList.Add(tenant); }

                AvailableRoomList.Clear();
                var rooms = _roomService.GetAvailableRooms();
                foreach (var room in rooms) { AvailableRoomList.Add(room); }
            }
            catch (Exception ex) { MessageBox.Show($"Lỗi khi tải dữ liệu cho popup: {ex.Message}"); }
        }

        private void LoadContractDocument(string maHopDong)
        {
            var docToDisplay = new FlowDocument();
            try
            {
                string filePath = _contractService.GetContractFilePath(maHopDong);
                if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
                {
                    docToDisplay.Blocks.Add(new Paragraph(new Run($"Lỗi: Không tìm thấy file hợp đồng tại đường dẫn:\n{filePath}")));
                    ContractDocument = docToDisplay;
                    return;
                }
                Document document = new Document();
                document.LoadFromFile(filePath);
                using (var ms = new MemoryStream())
                {
                    document.SaveToStream(ms, FileFormat.Rtf);
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

        // ✅ SỬA LẠI: Tải lịch sử theo MaHopDong
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
        }

        [RelayCommand]
        private void ChangeTab(string tabName) => SelectedTab = tabName;

        [RelayCommand]
        private void OpenAddContractPopup()
        {
            LoadDataForPopup();
            NewContractDeposit = 0;
            NewContractNotes = string.Empty;
            NewContractStartDate = DateTime.Today;
            NewContractDuration = 12;
            IsAddContractPopupVisible = true;
        }

        [RelayCommand]
        private void CancelAddContract() => IsAddContractPopupVisible = false;

        [RelayCommand]
        private void CreateContract()
        {
            if (SelectedNewTenant == null || SelectedNewRoom == null || NewContractStartDate == null)
            {
                MessageBox.Show("Vui lòng điền đầy đủ thông tin."); return;
            }
            // Validate deposit
            if (NewContractDeposit <= 0)
            {
                MessageBox.Show("Tiền cọc phải là số lớn hơn 0 và chỉ được nhập số!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            try
            {
                HopDong newContract = new HopDong
                {
                    MaPhong = SelectedNewRoom.MaPhong,
                    ChuNha = "ND001",
                    TienCoc = NewContractDeposit,
                    NgayBatDau = NewContractStartDate.Value,
                    ThoiHan = NewContractDuration,
                    TrangThai = "Hiệu lực",
                    GhiChu = NewContractNotes
                };
                string newContractId = _contractService.TaoHopDong(newContract, SelectedNewTenant.MaNguoiThue);
                MessageBox.Show($"Tạo hợp đồng {newContractId} thành công!");
                IsAddContractPopupVisible = false;
                LoadContracts();
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
        private void CancelDeleteContract() => IsDeleteConfirmationVisible = false;
        [RelayCommand]
        private void SaveContractChanges()
        {
            if (EditingContract == null) return;

            // Validate deposit
            if (EditingContract.TienCoc <= 0)
            {
                MessageBox.Show("Tiền cọc phải là số lớn hơn 0 và chỉ được nhập số!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string currentContractId = EditingContract.MaHopDong;

            try
            {
                string currentUser = "ND001";
                if (_contractService.CapNhatHopDong(EditingContract, currentUser))
                {
                    MessageBox.Show("Cập nhật hợp đồng thành công!");
                    LoadContracts(currentContractId);
                }
                else { MessageBox.Show("Cập nhật hợp đồng thất bại."); }
            }
            catch (Exception ex) { MessageBox.Show($"Lỗi khi cập nhật hợp đồng: {ex.Message}"); }
        }

        [RelayCommand] private void DownloadContract() => MessageBox.Show("Chức năng tải xuống đang được phát triển.");
        [RelayCommand] private void SendContract() => MessageBox.Show("Chức năng gửi đang được phát triển.");
    }
}