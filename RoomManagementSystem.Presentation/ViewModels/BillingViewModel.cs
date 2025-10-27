using CommunityToolkit.Mvvm.ComponentModel;
using RoomManagementSystem.BusinessLayer;
using RoomManagementSystem.DataLayer;
using System.Collections.ObjectModel;
using System.Linq;

namespace RoomManagementSystem.Presentation.ViewModels
{
    public partial class BillingViewModel : ObservableObject
    {
        private readonly QL_TaiSan_Phong _taiSanPhongService;

        [ObservableProperty]
        private ObservableCollection<NhaPhongViewModel> danhSachNha;

        /*    public BillingViewModel()
            {
                _taiSanPhongService = new QL_TaiSan_Phong();
                LoadData();
            }
        */

        public BillingViewModel(QL_TaiSan_Phong taiSanPhongService)
        {
            _taiSanPhongService = taiSanPhongService; // Chỉ gán, không "new"
            LoadData();
        }

        /*
        private void LoadData()
        {
            var dsNha = _taiSanPhongService.DanhSachNha();
            var dsPhong = _taiSanPhongService.DanhSachPhong();

            var list = dsNha.Select(nha => new NhaPhongViewModel
            {
                MaNha = nha.MaNha,
                TenNha = nha.DiaChi,
                DanhSachPhong = new ObservableCollection<Phong>(
                    dsPhong.Where(p => p.MaNha == nha.MaNha)
                )
            });

            DanhSachNha = new ObservableCollection<NhaPhongViewModel>(list);
        }
        */

        private void LoadData()
        {
            var dsNha = _taiSanPhongService.DanhSachNha();
            var dsPhong = _taiSanPhongService.DanhSachPhong();

            // === TỐI ƯU HÓA ===
            // 1. Nhóm tất cả phòng theo MaNha (chỉ chạy 1 lần - O(N))
            var phongTheoNhaLookup = dsPhong.ToLookup(p => p.MaNha);

            // 2. Lặp qua danh sách nhà (O(M))
            var list = dsNha.Select(nha => new NhaPhongViewModel
            {
                MaNha = nha.MaNha,
                TenNha = nha.DiaChi,
                // Lấy danh sách phòng từ Lookup (gần như tức thời - O(1))
                DanhSachPhong = new ObservableCollection<Phong>(phongTheoNhaLookup[nha.MaNha])
            });

            DanhSachNha = new ObservableCollection<NhaPhongViewModel>(list);
        }

    }

    public class NhaPhongViewModel
    {
        public string MaNha { get; set; }
        public string TenNha { get; set; }
        public ObservableCollection<Phong> DanhSachPhong { get; set; }
    }
}
