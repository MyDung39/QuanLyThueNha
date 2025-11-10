using CommunityToolkit.Mvvm.ComponentModel;
using System;

namespace RoomManagementSystem.Presentation.ViewModels
{
    public partial class NotificationItemViewModel : ObservableObject
    {
        [ObservableProperty] private int _stt;
        [ObservableProperty] private string _loaiThongBao;
        [ObservableProperty] private string _nguoiLienQuan;
        [ObservableProperty] private DateTime _ngayThongBao;
        [ObservableProperty] private string _noiDung;

        // ✅ THÊM CÁC THUỘC TÍNH NÀY
        [ObservableProperty] private string _phong;
        [ObservableProperty] private DateTime? _batDau;
        [ObservableProperty] private DateTime? _ketThuc;
        [ObservableProperty] private string _tienCoc;
    }
}