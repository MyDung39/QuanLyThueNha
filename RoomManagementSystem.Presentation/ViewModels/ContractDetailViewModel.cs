using CommunityToolkit.Mvvm.ComponentModel;
using RoomManagementSystem.DataLayer;
using System;

namespace RoomManagementSystem.Presentation.ViewModels
{
    public partial class ContractDetailViewModel : ObservableObject
    {
        [ObservableProperty] private DateTime? _ngayLapHopDong;
        [ObservableProperty] private DateTime? _ngayBatDau;
        [ObservableProperty] private string _thoiHan;
        [ObservableProperty] private DateTime? _ngayKetThuc;
        [ObservableProperty] private string _tenNguoiThue;
        [ObservableProperty] private string _diaChiThue;
        [ObservableProperty] private string _thongTinLienHe;
        [ObservableProperty] private string _maHopDong;
        [ObservableProperty] private string _tienCoc;
        [ObservableProperty] private string _ghiChu;

        public ContractDetailViewModel(HopDongXemIn contractData)
        {
            if (contractData == null) return;
            NgayLapHopDong = contractData.NgayTao;
            NgayBatDau = contractData.NgayBatDau;
            ThoiHan = $"{contractData.ThoiHan} Tháng";
            NgayKetThuc = contractData.NgayKetThuc;
            TenNguoiThue = contractData.TenNguoiThue;
            DiaChiThue = $"{contractData.DiaChiNha} - Phòng {contractData.MaPhong}";
            ThongTinLienHe = $"{contractData.SdtNguoiThue} | {contractData.EmailNguoiThue}";
            MaHopDong = contractData.MaHopDong;
            TienCoc = $"{contractData.TienCoc:N0} VND";
            GhiChu = contractData.GhiChuHopDong;
        }
    }
}