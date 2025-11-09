using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RoomManagementSystem.BusinessLayer;
using RoomManagementSystem.DataLayer;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using Spire.Doc; // Thư viện để làm việc với file Word

namespace RoomManagementSystem.Presentation.ViewModels
{
    public partial class ContractManagementViewModel : ViewModelBase
    {
        [ObservableProperty] private bool _isAddContractPopupVisible;
        [ObservableProperty] private ObservableCollection<NguoiThue> _tenantList;
        [ObservableProperty] private ObservableCollection<Phong> _availableRoomList;
        [ObservableProperty] private NguoiThue _selectedNewTenant;
        [ObservableProperty] private Phong _selectedNewRoom;
        [ObservableProperty] private decimal _newContractDeposit;
        [ObservableProperty] private string _newContractNotes;
        [ObservableProperty] private DateTime? _newContractStartDate = DateTime.Today;
        [ObservableProperty] private int _newContractDuration = 12;


        [ObservableProperty] private ObservableCollection<ContractItemViewModel> _allContracts;
        [ObservableProperty] private ObservableCollection<ContractItemViewModel> _contractList;
        [ObservableProperty] private ContractItemViewModel _selectedContract;
        [ObservableProperty] private string _searchKeyword;

        // ✅ THÊM: Cờ điều khiển popup xác nhận xóa
        [ObservableProperty] private bool _isDeleteConfirmationVisible;

        // ✅ THÊM: Property để chứa nội dung file Word đã được chuyển đổi
        [ObservableProperty]
        private FlowDocument _contractDocument;


        [ObservableProperty]
        private string _selectedTab = "Xem"; // Mặc định là tab "Xem"


        private readonly QuanLyNguoiThue _tenantService;
        private readonly QL_TaiSan_Phong _roomService;
        private readonly QL_HopDong _contractService; // Đã đổi lại thành QL_HopDong

        public ContractManagementViewModel()
        {
            _tenantService = new QuanLyNguoiThue();
            _roomService = new QL_TaiSan_Phong();
            _contractService = new QL_HopDong(); // Đã đổi lại thành QL_HopDong

            _isAddContractPopupVisible = false;
            _tenantList = new ObservableCollection<NguoiThue>();
            _availableRoomList = new ObservableCollection<Phong>();

            _allContracts = new ObservableCollection<ContractItemViewModel>();
            _contractList = new ObservableCollection<ContractItemViewModel>();

            LoadContracts();
        }


        // ✅ PHƯƠNG THỨC TẢI DANH SÁCH HỢP ĐỒNG
        private void LoadContracts()
        {
            try
            {
                var contractsWithTenants = _contractService.GetContractsWithTenantNames();
                
                _allContracts.Clear();
                _contractList.Clear();

                foreach (var entry in contractsWithTenants)
                {
                    var contract = entry.Key;
                    var tenantName = entry.Value;
                    
                    var contractVM = new ContractItemViewModel(contract)
                    {
                        // Ghi đè lại tên người thuê đã được tải
                        TenantName = tenantName
                    };
                    
                    _allContracts.Add(contractVM);
                    _contractList.Add(contractVM);
                }

                // Tự động chọn mục đầu tiên nếu có
                if (_contractList.Any())
                {
                    SelectedContract = _contractList.First();
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải danh sách hợp đồng: {ex.Message}");
            }
        }
        
        // ✅ LOGIC TÌM KIẾM
        partial void OnSearchKeywordChanged(string value)
        {
            FilterContracts(value);
        }

        private void FilterContracts(string searchText)
        {
            searchText = searchText?.Trim().ToLower() ?? "";
            
            var filtered = string.IsNullOrEmpty(searchText)
                ? _allContracts.ToList()
                : _allContracts.Where(c => 
                    c.TenantName.ToLower().Contains(searchText) || 
                    c.ContractName.ToLower().Contains(searchText)
                ).ToList();
            
            ContractList.Clear();
            foreach (var contract in filtered)
            {
                ContractList.Add(contract);
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
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải dữ liệu cho popup: {ex.Message}");
            }
        }

        [RelayCommand]
        private void OpenAddContractPopup()
        {
            LoadDataForPopup();

            SelectedNewTenant = null;
            SelectedNewRoom = null;
            NewContractDeposit = 0;
            NewContractNotes = string.Empty;
            NewContractStartDate = DateTime.Today;
            NewContractDuration = 12;

            IsAddContractPopupVisible = true;
        }

        [RelayCommand]
        private void CancelAddContract() { IsAddContractPopupVisible = false; }

        [RelayCommand]
        private void CreateContract()
        {
            if (SelectedNewTenant == null) { MessageBox.Show("Vui lòng chọn người thuê."); return; }
            if (SelectedNewRoom == null) { MessageBox.Show("Vui lòng chọn phòng."); return; }
            if (NewContractStartDate == null) { MessageBox.Show("Vui lòng chọn ngày bắt đầu."); return; }

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

                if (!string.IsNullOrEmpty(newContractId))
                {
                    MessageBox.Show($"Tạo hợp đồng {newContractId} thành công!");
                    IsAddContractPopupVisible = false;
                    // Tải lại danh sách hợp đồng
                    LoadContracts();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tạo hợp đồng: {ex.Message}");
            }
        }




        // ✅ SỬA LẠI: Đổi tên Command cho rõ ràng
        [RelayCommand]
        private void OpenDeleteConfirmation()
        {
            if (SelectedContract == null)
            {
                MessageBox.Show("Vui lòng chọn một hợp đồng để xóa.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            IsDeleteConfirmationVisible = true;
        }

        // ✅ THÊM: Command cho nút "Xóa" trên popup
        [RelayCommand]
        private void ConfirmDeleteContract()
        {
            if (SelectedContract == null) return;

            try
            {
                // Gọi xuống Business Layer để xóa
                bool success = _contractService.XoaHopDong(SelectedContract.OriginalContract.MaHopDong);

                if (success)
                {
                    MessageBox.Show("Xóa hợp đồng thành công!");
                    // Tải lại toàn bộ danh sách để cập nhật giao diện
                    LoadContracts();
                }
                else
                {
                    MessageBox.Show("Xóa hợp đồng thất bại.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi xóa hợp đồng: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                // Luôn đóng popup sau khi xử lý xong
                IsDeleteConfirmationVisible = false;
            }
        }

        // ✅ THÊM: Command cho nút "Hủy" trên popup
        [RelayCommand]
        private void CancelDeleteContract()
        {
            IsDeleteConfirmationVisible = false;
        }


        // ✅ LOGIC QUAN TRỌNG: Được gọi mỗi khi người dùng chọn một hợp đồng khác
        partial void OnSelectedContractChanged(ContractItemViewModel value)
        {
            if (value != null)
            {
                // Khi một hợp đồng được chọn, gọi hàm để tải nội dung của nó
                LoadContractDocument(value.OriginalContract.MaHopDong);
            }
            else
            {
                // Nếu không có hợp đồng nào được chọn, xóa nội dung hiển thị
                ContractDocument = null;
            }
        }

        // ✅ PHƯƠNG THỨC MỚI: Tải và chuyển đổi file Word bằng Spire.Doc
        private void LoadContractDocument(string maHopDong)
        {
            var docToDisplay = new FlowDocument();
            try
            {
                // Giả sử bạn đã có phương thức này trong BLL để lấy đường dẫn file
                string filePath = _contractService.GetContractFilePath(maHopDong);

                if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
                {
                    docToDisplay.Blocks.Add(new Paragraph(new Run($"Lỗi: Không tìm thấy file hợp đồng tại đường dẫn:\n{filePath}")));
                    ContractDocument = docToDisplay;
                    return;
                }

                // 1. Tải file .docx bằng Spire.Doc
                Document document = new Document();
                document.LoadFromFile(filePath);

                // 2. Chuyển đổi sang định dạng RTF làm trung gian
                using (var ms = new MemoryStream())
                {
                    document.SaveToStream(ms, FileFormat.Rtf);
                    ms.Position = 0; // Đưa con trỏ về đầu stream để đọc

                    // 3. Đọc dữ liệu RTF vào FlowDocument
                    var textRange = new TextRange(docToDisplay.ContentStart, docToDisplay.ContentEnd);
                    textRange.Load(ms, DataFormats.Rtf);
                }

                // 4. Gán FlowDocument đã có nội dung vào thuộc tính để UI cập nhật
                ContractDocument = docToDisplay;
            }
            catch (Exception ex)
            {
                docToDisplay.Blocks.Add(new Paragraph(new Run($"Đã xảy ra lỗi khi tải file: {ex.Message}")));
                ContractDocument = docToDisplay;
            }
        }

        // ✅ THÊM: Các command cho nút Tải xuống và Gửi
        [RelayCommand]
        private void DownloadContract()
        {
            MessageBox.Show("Chức năng tải xuống đang được phát triển.");
        }

        [RelayCommand]
        private void SendContract()
        {
            MessageBox.Show("Chức năng gửi đang được phát triển.");
        }


        [RelayCommand]
        private void ChangeTab(string tabName)
        {
            SelectedTab = tabName;

            // Tùy chọn: Bạn có thể thêm logic ở đây
            // Ví dụ: khi chuyển sang tab "Thông tin", tải dữ liệu chi tiết
            if (tabName == "Thông tin" && SelectedContract != null)
            {
                // LoadDetailedContractInfo(SelectedContract.OriginalContract.MaHopDong);
            }
        }


    }
}