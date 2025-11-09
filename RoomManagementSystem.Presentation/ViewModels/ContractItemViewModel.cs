using CommunityToolkit.Mvvm.ComponentModel;

namespace RoomManagementSystem.Presentation.ViewModels
{
    // Lớp này đại diện cho một mục trong danh sách hợp đồng hiển thị trên UI
    public partial class ContractItemViewModel : ObservableObject
    {
        // Lưu trữ đối tượng HopDong gốc để có thể truy cập các thông tin khác nếu cần
        public DataLayer.HopDong OriginalContract { get; }

        [ObservableProperty]
        private string _contractName; // Ví dụ: "Hợp đồng 1"

        [ObservableProperty]
        private string _tenantName; // Tên người thuê

        public ContractItemViewModel(DataLayer.HopDong contract)
        {
            OriginalContract = contract;
            // Xử lý tên hợp đồng (có thể là mã hợp đồng)
            _contractName = contract.MaHopDong;

            // Tên người thuê sẽ được điền từ Business Layer sau
            _tenantName = "Đang tải...";
        }
    }
}