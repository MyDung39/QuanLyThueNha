using CommunityToolkit.Mvvm.ComponentModel;
using RoomManagementSystem.DataLayer;
using System;

namespace RoomManagementSystem.Presentation.ViewModels
{
    // Lớp này kết hợp thông tin từ NguoiThue và HopDong_NguoiThue
    public partial class TenantInfoViewModel : ObservableObject
    {
        // Thông tin từ bảng NguoiThue
        [ObservableProperty] private string _hoTen;
        [ObservableProperty] private string _soDienThoai;
        [ObservableProperty] private string _email;

        // Thông tin từ bảng HopDong_NguoiThue
        [ObservableProperty] private DateTime? _ngayBatDauThue;
        [ObservableProperty] private DateTime? _ngayDonRa; // Ngày kết thúc thuê
        [ObservableProperty] private string _trangThaiThue;

        // Thuộc tính để chọn trong DataGrid
        [ObservableProperty] private bool _isSelected;
    }
}