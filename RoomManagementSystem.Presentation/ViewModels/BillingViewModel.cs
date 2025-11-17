using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using RoomManagementSystem.BusinessLayer;
using RoomManagementSystem.DataLayer;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace RoomManagementSystem.Presentation.ViewModels
{
    // Lớp helper để chứa dữ liệu cho cột Tóm Tắt
    public class MucTomTat
    {
        public string TieuDe { get; set; }
        public string MaSo { get; set; }
        public DateTime? Ngay { get; set; }
        public decimal SoTien { get; set; }
    }

    // Lớp helper cho danh sách phòng
    public partial class NhaPhongViewModel : ViewModelBase
    {
        [ObservableProperty] private string _maNha;
        [ObservableProperty] private string _diaChi;
        [ObservableProperty] private ObservableCollection<Phong> _danhSachPhong;
    }

    public partial class BillingViewModel : ViewModelBase
    {
        // === KHAI BÁO CÁC SERVICE ===
        private readonly QL_TaiSan_Phong _taiSanPhongService;
        private readonly XuatBienLai _xuatBienLaiService;
        private readonly ThanhToanDAL _thanhToanService;
        private readonly BaoTriDAL _baoTriService;

        // === CÁC THUỘC TÍNH BINDING ===
        [ObservableProperty] private ObservableCollection<NhaPhongViewModel> _danhSachNha;
        [ObservableProperty] private ObservableCollection<BienLai> _chiTietHoaDon;
        [ObservableProperty] private BienLai _selectedBillInfo;
        [ObservableProperty] private ObservableCollection<MucTomTat> _danhSachTomTat;
        [ObservableProperty] private decimal _tongTienCanTra;

        private Phong _selectedPhong;
        public Phong SelectedPhong
        {
            get => _selectedPhong;
            set { if (SetProperty(ref _selectedPhong, value)) { OnSelectedPhongChanged(value); } }
        }

        public BillingViewModel()
        {
            _taiSanPhongService = new QL_TaiSan_Phong();
            _xuatBienLaiService = new XuatBienLai();
            _thanhToanService = new ThanhToanDAL();
            _baoTriService = new BaoTriDAL();

            _danhSachNha = new ObservableCollection<NhaPhongViewModel>();
            _chiTietHoaDon = new ObservableCollection<BienLai>();
            _danhSachTomTat = new ObservableCollection<MucTomTat>();

            _chiTietHoaDon.CollectionChanged += (s, e) =>
            {
                OnPropertyChanged(nameof(TongTienCot2));
            };

            LoadSideBarData();
        }

        public decimal TongTienCot2 => ChiTietHoaDon.Sum(x => x.ThanhTien);

        private void OnSelectedPhongChanged(Phong phong)
        {
            ChiTietHoaDon.Clear();
            DanhSachTomTat.Clear();
            SelectedBillInfo = null;
            TongTienCanTra = 0;

            if (phong == null) return;

            try
            {
                // Lấy dữ liệu hóa đơn (đã được lưu từ các màn hình ServiceElectricView/ServiceWaterView...)
                var hoaDonData = _xuatBienLaiService.GetBienLai(phong.MaPhong);
                var thanhToanHienTai = _thanhToanService.GetThanhToanHienTaiByPhong(phong.MaPhong);
                var phiBaoTri = ChiTietHoaDon.FirstOrDefault(x => x.TenDichVu.StartsWith("Bảo trì"));

                // 1. Thêm tiền thuê phòng (luôn có)
                ChiTietHoaDon.Add(new BienLai
                {
                    TenDichVu = "Tiền thuê phòng",
                    DVT = "Tháng",
                    SoLuong = 1,
                    DonGia = phong.GiaThue,
                    ThanhTien = phong.GiaThue
                });
                DanhSachTomTat.Add(new MucTomTat
                {
                    TieuDe = "Tiền thuê phòng",
                    SoTien = phong.GiaThue
                });

                // 2. Thêm các dịch vụ từ hóa đơn (Điện, Nước, Internet...)
                if (hoaDonData.Any())
                {
                    SelectedBillInfo = hoaDonData.First();
                    foreach (var item in hoaDonData)
                    {
                        ChiTietHoaDon.Add(item);
                        DanhSachTomTat.Add(new MucTomTat
                        {
                            TieuDe = item.TenDichVu,
                            SoTien = item.ThanhTien,
                            Ngay = item.NgayLapHoaDon
                        });
                    }
                }

                // 3. Tính công nợ cũ
                decimal tienCongNoConLai = Math.Max(0, (thanhToanHienTai?.TongCongNo ?? 0) - (thanhToanHienTai?.SoTienDaThanhToan ?? 0));
                if (tienCongNoConLai > 0)
                {
                    ChiTietHoaDon.Add(new BienLai
                    {
                        TenDichVu = "Công nợ cũ",
                        ThanhTien = tienCongNoConLai
                    });
                    DanhSachTomTat.Add(new MucTomTat
                    {
                        TieuDe = "Công nợ cũ",
                        SoTien = tienCongNoConLai,
                        Ngay = thanhToanHienTai?.NgayTao
                    });
                }

                // 4. Đã thanh toán
                decimal tienDaThanhToan = thanhToanHienTai?.SoTienDaThanhToan ?? 0;
                if (tienDaThanhToan > 0)
                {
                    DanhSachTomTat.Add(new MucTomTat
                    {
                        TieuDe = "Đã thanh toán",
                        SoTien = tienDaThanhToan,
                        Ngay = thanhToanHienTai?.NgayCapNhat
                    });
                }

                // 5. Tổng cộng
                decimal tong = ChiTietHoaDon.Sum(x => x.ThanhTien) + tienCongNoConLai;
                DanhSachTomTat.Add(new MucTomTat
                {
                    TieuDe = "Tổng cộng",
                    SoTien = tong
                });

                TongTienCanTra = tong;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi lấy dữ liệu hóa đơn: {ex.Message}");
            }
        }

        private void LoadSideBarData()
        {
            try
            {
                var dsNha = _taiSanPhongService.DanhSachNha();
                DanhSachNha.Clear();
                foreach (var nha in dsNha)
                {
                    var phongCuaNha = _taiSanPhongService.DanhSachPhong(nha.MaNha);
                    var nhaVM = new NhaPhongViewModel
                    {
                        MaNha = nha.MaNha,
                        DiaChi = nha.DiaChi,
                        DanhSachPhong = new ObservableCollection<Phong>(phongCuaNha)
                    };
                    DanhSachNha.Add(nhaVM);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải danh sách nhà: {ex.Message}");
            }
        }

        [RelayCommand]
        private void XuatExcel()
        {
            XuatHoaDon();
        }

        public void XuatHoaDon()
        {
            if (ChiTietHoaDon == null || !ChiTietHoaDon.Any())
            {
                MessageBox.Show("Không có dữ liệu để xuất.");
                return;
            }

            SaveFileDialog saveFile = new SaveFileDialog
            {
                Filter = "Excel files (*.xlsx)|*.xlsx",
                FileName = $"BienLai_{SelectedBillInfo?.MaHoaDon ?? "ChuaCo"}.xlsx"
            };

            if (saveFile.ShowDialog() == true)
            {
                try
                {
                    _xuatBienLaiService.XuatBienLaiExcel(SelectedBillInfo, ChiTietHoaDon.ToList(), saveFile.FileName);
                    MessageBox.Show("Xuất Excel thành công!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi khi xuất Excel: {ex.Message}");
                }
            }
        }
    }
}