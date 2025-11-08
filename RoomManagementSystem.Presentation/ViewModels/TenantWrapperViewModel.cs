using CommunityToolkit.Mvvm.ComponentModel;
using RoomManagementSystem.DataLayer;

namespace RoomManagementSystem.Presentation.ViewModels
{
    public partial class TenantWrapperViewModel : ObservableObject
    {
        public NguoiThue Tenant { get; }

        [ObservableProperty]
        private bool _isSelected;

        public TenantWrapperViewModel(NguoiThue tenant)
        {
            Tenant = tenant;
        }

        // Tạo các thuộc tính "proxy" để dễ dàng binding trong XAML
        public string HoTen => Tenant.HoTen;
        public string Sdt => Tenant.Sdt;
        public string Email => Tenant.Email;
        public DateTime? NgayBatDauThue => Tenant.NgayBatDauThue;
        public DateTime? NgayDonRa => Tenant.NgayDonRa;
        public string TrangThaiThue => Tenant.TrangThaiThue;
    }
}