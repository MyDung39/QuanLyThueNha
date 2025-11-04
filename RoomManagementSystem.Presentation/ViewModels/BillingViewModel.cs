using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RoomManagementSystem.BusinessLayer;
using RoomManagementSystem.DataLayer;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace RoomManagementSystem.Presentation.ViewModels
{
    public partial class BillingViewModel : ViewModelBase
    {
        private readonly QL_TaiSan_Phong _taiSanPhongService;
        private readonly XuatBienLai _xuatBienLaiService;

        [ObservableProperty] private ObservableCollection<NhaPhongViewModel> _danhSachNha;
        [ObservableProperty] private ObservableCollection<BienLai> _chiTietHoaDon;
        [ObservableProperty] private Phong _selectedPhong;

        // THUỘC TÍNH MỚI: Lưu thông tin chung của hóa đơn đang hiển thị
        [ObservableProperty]
        private BienLai _selectedBillInfo;

        public BillingViewModel(QL_TaiSan_Phong taiSanPhongService, XuatBienLai xuatBienLaiService)
        {
            _taiSanPhongService = taiSanPhongService;
            _xuatBienLaiService = xuatBienLaiService;
            _danhSachNha = new ObservableCollection<NhaPhongViewModel>();
            _chiTietHoaDon = new ObservableCollection<BienLai>();
            LoadSideBarData();
        }

        [RelayCommand]
        private void SelectPhong(Phong selectedPhong)
        {
            ChiTietHoaDon.Clear();
            SelectedBillInfo = null;

            if (selectedPhong != null)
            {
                try
                {
                    var hoaDonData = _xuatBienLaiService.GetBienLai(selectedPhong.MaPhong);
                    if (hoaDonData.Any())
                    {
                        // Gán thông tin chung từ dòng đầu tiên
                        SelectedBillInfo = hoaDonData.First();

                        // Nạp chi tiết vào DataGrid
                        foreach (var item in hoaDonData)
                        {
                            ChiTietHoaDon.Add(item);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi khi lấy dữ liệu hóa đơn: {ex.Message}");
                }
            }
        }

        private void LoadSideBarData()
        {
            var dsNha = _taiSanPhongService.DanhSachNha();
            var dsPhong = _taiSanPhongService.DanhSachPhong();
            var phongTheoNhaLookup = dsPhong.ToLookup(p => p.MaNha);
            var list = dsNha.Select(nha => new NhaPhongViewModel
            {
                MaNha = nha.MaNha,
                DiaChi = nha.DiaChi,
                DanhSachPhong = new ObservableCollection<Phong>(phongTheoNhaLookup[nha.MaNha])
            });
            DanhSachNha = new ObservableCollection<NhaPhongViewModel>(list);
        }
    }

    public partial class NhaPhongViewModel : ViewModelBase
    {
        [ObservableProperty] private string _maNha;
        [ObservableProperty] private string _tenNha;
        [ObservableProperty] private string _diaChi;
        [ObservableProperty] private ObservableCollection<Phong> _danhSachPhong;
    }
}