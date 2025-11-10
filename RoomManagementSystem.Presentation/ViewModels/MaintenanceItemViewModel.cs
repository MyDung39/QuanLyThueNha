using CommunityToolkit.Mvvm.ComponentModel;
using RoomManagementSystem.DataLayer;
using System;

namespace RoomManagementSystem.Presentation.ViewModels
{
    public partial class MaintenanceItemViewModel : ObservableObject
    {
        public BaoTri OriginalData { get; set; }

        [ObservableProperty]
        private bool _isSelected;

        public Action SelectionChanged { get; set; }

        public MaintenanceItemViewModel(BaoTri baoTri)
        {
            OriginalData = baoTri;
        }

        public string Phong => OriginalData.MaPhong;
        public string NguoiThue => OriginalData.TenNguoiThue;
        public string MoTa => OriginalData.MoTa;
        public DateTime NgayYeuCau => OriginalData.NgayYeuCau;
        public DateTime? NgayHoanThanh => OriginalData.NgayHoanThanh;
        public string TrangThaiXuLy => OriginalData.TrangThaiXuLy;
        public decimal ChiPhi => OriginalData.ChiPhi;

        partial void OnIsSelectedChanged(bool value)
        {
            SelectionChanged?.Invoke();
        }

        public void UpdateFromEntity(BaoTri newData)
        {
            OriginalData = newData;
            OnPropertyChanged(nameof(Phong));
            OnPropertyChanged(nameof(NguoiThue));
            OnPropertyChanged(nameof(MoTa));
            OnPropertyChanged(nameof(NgayYeuCau));
            OnPropertyChanged(nameof(NgayHoanThanh));
            OnPropertyChanged(nameof(TrangThaiXuLy));
            OnPropertyChanged(nameof(ChiPhi));
        }
    }
}