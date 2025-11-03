using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input; // Cần cho RelayCommand
using RoomManagementSystem.BusinessLayer;
using RoomManagementSystem.DataLayer;
using System.Collections.ObjectModel;
using System.Linq;

namespace RoomManagementSystem.Presentation.ViewModels
{
    public partial class BillingViewModel : ViewModelBase
    {
        private readonly QL_TaiSan_Phong _taiSanPhongService;
        private readonly XuatBienLai _xuatBienLaiService;

        [ObservableProperty]
        private ObservableCollection<NhaPhongViewModel> _danhSachNha;

        [ObservableProperty]
        private ObservableCollection<BienLai> _chiTietHoaDon;

        // Thuộc tính này bây giờ chỉ để lưu trữ, không cần logic phức tạp
        [ObservableProperty]
        private Phong _selectedPhong;

        public BillingViewModel(QL_TaiSan_Phong taiSanPhongService, XuatBienLai xuatBienLaiService)
        {
            _taiSanPhongService = taiSanPhongService;
            _xuatBienLaiService = xuatBienLaiService;
            _danhSachNha = new ObservableCollection<NhaPhongViewModel>();
            _chiTietHoaDon = new ObservableCollection<BienLai>();
            LoadSideBarData();
        }

        // ✅ TẠO MỘT COMMAND MỚI ĐỂ XỬ LÝ VIỆC CHỌN PHÒNG
        [RelayCommand]
        private void SelectPhong(Phong selectedPhong)
        {
            ChiTietHoaDon.Clear();

            if (selectedPhong != null)
            {
                try
                {
                    var chiTiet = _xuatBienLaiService.GetBienLai(selectedPhong.MaPhong);
                    foreach (var item in chiTiet)
                    {
                        ChiTietHoaDon.Add(item);
                    }
                }
                catch (System.Exception ex)
                {
                    System.Windows.MessageBox.Show($"Lỗi khi lấy dữ liệu hóa đơn: {ex.Message}");
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
                TenNha = nha.DiaChi,
                DanhSachPhong = new ObservableCollection<Phong>(phongTheoNhaLookup[nha.MaNha])
            });
            DanhSachNha = new ObservableCollection<NhaPhongViewModel>(list);
        }
    }

    public partial class NhaPhongViewModel : ViewModelBase
    {
        [ObservableProperty] private string _maNha;
        [ObservableProperty] private string _tenNha;
        [ObservableProperty] private ObservableCollection<Phong> _danhSachPhong;
    }
}