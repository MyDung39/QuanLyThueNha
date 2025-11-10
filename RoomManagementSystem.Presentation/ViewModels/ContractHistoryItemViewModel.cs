using CommunityToolkit.Mvvm.ComponentModel;
using System;

namespace RoomManagementSystem.Presentation.ViewModels
{
    public partial class ContractHistoryItemViewModel : ObservableObject
    {
        [ObservableProperty] private int _stt;
        [ObservableProperty] private DateTime _ngaySua;
        [ObservableProperty] private string _nguoiThucHien;
        [ObservableProperty] private string _hanhDong;
        [ObservableProperty] private string _noiDung;
    }
}