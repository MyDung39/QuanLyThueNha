using CommunityToolkit.Mvvm.ComponentModel;
using RoomManagementSystem.BusinessLayer;
using RoomManagementSystem.DataLayer;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace RoomManagementSystem.Presentation.ViewModels
{
    // Lớp helper để chứa dữ liệu cho cột Tóm Tắt (bên phải)
    public class MucTomTat
    {
        public string TieuDe { get; set; }
        public string MaSo { get; set; }
        public DateTime? Ngay { get; set; }
        public decimal SoTien { get; set; }
    }

    // Lớp helper cho danh sách phòng (bên trái)
    // (Đã có sẵn trong file của bạn)
    public partial class NhaPhongViewModel : ViewModelBase
    {
        [ObservableProperty] private string _maNha;
        [ObservableProperty] private string _diaChi;
        [ObservableProperty] private ObservableCollection<Phong> _danhSachPhong;
    }


    /// <summary>
    /// ViewModel chính cho trang BillingView
    /// </summary>
    public partial class BillingViewModel : ViewModelBase
    {
        // === KHAI BÁO CÁC SERVICE (BLL/DAL) ===
        private readonly QL_TaiSan_Phong _taiSanPhongService;
        private readonly XuatBienLai _xuatBienLaiService;
        private readonly ThanhToanDAL _thanhToanService; // ✅ ĐÃ THÊM
        private readonly BaoTriDAL _baoTriService; // ✅ ĐÃ THÊM BAOTRIDAL

        // === CÁC THUỘC TÍNH BINDING VỚI VIEW ===

        // Cột 1: Danh sách Nhà/Phòng
        [ObservableProperty]
        private ObservableCollection<NhaPhongViewModel> _danhSachNha;

        // Cột 2 (DataGrid): Chi tiết các dịch vụ (Điện, Nước...)
        [ObservableProperty]
        private ObservableCollection<BienLai> _chiTietHoaDon;

        // Cột 2 (Thông tin chung): Dữ liệu chính của hóa đơn
        [ObservableProperty]
        private BienLai _selectedBillInfo;

        // Cột 3: Danh sách tóm tắt (Hóa đơn, Công nợ, Phụ thu)
        [ObservableProperty]
        private ObservableCollection<MucTomTat> _danhSachTomTat; // ✅ ĐÃ THÊM

        // Cột 3 (Box màu hồng): Tổng tiền cuối cùng
        [ObservableProperty]
        private decimal _tongTienCanTra; // ✅ ĐÃ THÊM

        // Thuộc tính để nhận phòng được chọn từ ListBox
        private Phong _selectedPhong;
        public Phong SelectedPhong
        {
            get => _selectedPhong;
            set { if (SetProperty(ref _selectedPhong, value)) { OnSelectedPhongChanged(value); } }
        }

        // === HÀM KHỞI TẠO (CONSTRUCTOR) ===
        // ✅ SỬA LẠI: Dùng hàm khởi tạo rỗng để XAML có thể gọi
        public BillingViewModel()
        {
            // Tự khởi tạo các service
            _taiSanPhongService = new QL_TaiSan_Phong();
            _xuatBienLaiService = new XuatBienLai();
            _thanhToanService = new ThanhToanDAL(); // ✅ ĐÃ THÊM
            _baoTriService = new BaoTriDAL(); // ✅ KHỞI TẠO BAOTRIDAL

            // Khởi tạo các Collection
            _danhSachNha = new ObservableCollection<NhaPhongViewModel>();
            _chiTietHoaDon = new ObservableCollection<BienLai>();
            _danhSachTomTat = new ObservableCollection<MucTomTat>(); // ✅ ĐÃ THÊM

            // Tải dữ liệu ban đầu cho cột 1
            LoadSideBarData();
        }

        // === LOGIC TẢI DỮ LIỆU ===

        /// <summary>
        /// Được gọi khi người dùng bấm chọn một phòng từ cột 1
        /// </summary>
        /// 
        /*
        private void OnSelectedPhongChanged(Phong value)
        {
            // 1. Xóa hết dữ liệu cũ
            ChiTietHoaDon.Clear();
            DanhSachTomTat.Clear(); // ✅ ĐÃ THÊM
            SelectedBillInfo = null;
            TongTienCanTra = 0; // ✅ ĐÃ THÊM

            if (value == null) return;

            try
            {
                // 2. Tải dữ liệu cột 2 (Chi tiết hóa đơn & DataGrid)
                var hoaDonData = _xuatBienLaiService.GetBienLai(value.MaPhong);
                if (hoaDonData.Any())
                {
                    SelectedBillInfo = hoaDonData.First();
                    foreach (var item in hoaDonData)
                    {
                        ChiTietHoaDon.Add(item);
                    }
                }
                else
                {
                    // Nếu phòng này không có hóa đơn, dừng lại
                    return;
                }

                // ✅ BẮT ĐẦU PHẦN CODE BỊ LỖI (ĐÃ DI CHUYỂN VÀO ĐÂY)

                // 3. Tải dữ liệu cột 3 (Tóm tắt)
                // (Dữ liệu này lấy từ ThanhToanDAL)
                ThanhToan congNoChinh = _thanhToanService.GetThanhToanHienTaiByPhong(value.MaPhong);

                decimal tienHoaDonChinh = SelectedBillInfo?.TongTien ?? 0;

                // === PHẦN NÀY BẠN CẦN SERVICE RIÊNG ĐỂ TẢI ===
                // Hiện tại đang gán cứng dữ liệu demo dựa trên giao diện của bạn
                decimal tienCongNoCu = 700000;
                decimal tienPhuThu = 300000;
                // Bạn cần tạo hàm (ví dụ: _thanhToanService.GetCongNoCu(...)) để lấy số liệu thật
                // ------------------------------------------------

                // Thêm "Hóa đơn chính" vào danh sách tóm tắt
                DanhSachTomTat.Add(new MucTomTat
                {
                    TieuDe = "Hóa đơn Số: ",
                    MaSo = SelectedBillInfo.MaHoaDon,
                    Ngay = SelectedBillInfo.NgayLapHoaDon,
                    SoTien = tienHoaDonChinh
                });

                // Thêm "Công nợ" (placeholder)
                DanhSachTomTat.Add(new MucTomTat
                {
                    TieuDe = "Công nợ",
                    Ngay = new DateTime(2025, 5, 3), // (Ngày giả định)
                    SoTien = tienCongNoCu
                });

                // Thêm "Phụ thu" (placeholder)
                DanhSachTomTat.Add(new MucTomTat
                {
                    TieuDe = "Phụ thu",
                    Ngay = new DateTime(2025, 5, 1), // (Ngày giả định)
                    SoTien = tienPhuThu
                });


                // 4. Tính "Số tiền cần trả" (dựa trên logic hardcode của bạn)
                TongTienCanTra = tienHoaDonChinh - tienCongNoCu - tienPhuThu;

                // ✅ KẾT THÚC PHẦN CODE BỊ LỖI
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi lấy dữ liệu hóa đơn: {ex.Message}");
            }
        }
        */



        private void OnSelectedPhongChanged(Phong phong)
        {
            // 1. Xóa hết dữ liệu cũ
            ChiTietHoaDon.Clear();
            DanhSachTomTat.Clear();
            SelectedBillInfo = null;
            TongTienCanTra = 0;

            if (phong == null) return;

            try
            {
                // 2. Tải dữ liệu cột 2 (Chi tiết hóa đơn & DataGrid)
                var hoaDonData = _xuatBienLaiService.GetBienLai(phong.MaPhong);
                if (hoaDonData.Any())
                {
                    SelectedBillInfo = hoaDonData.First();
                    foreach (var item in hoaDonData)
                    {
                        ChiTietHoaDon.Add(item);
                    }
                }
                else
                {
                    // Nếu phòng này không có hóa đơn, dừng lại
                    return;
                }

                // 3. Tải dữ liệu cột 3 (Tóm tắt)

                // Lấy Hóa đơn chính (ví dụ: 505,000)
                decimal tienHoaDonChinh = SelectedBillInfo?.TongTien ?? 0;

                // Lấy thông tin Công nợ & Đã thanh toán từ ThanhToanDAL
                ThanhToan thanhToanHienTai = _thanhToanService.GetThanhToanHienTaiByPhong(phong.MaPhong);

                // Gán giá trị từ CSDL, nếu không tìm thấy thì bằng 0
                decimal tienCongNoCu = thanhToanHienTai?.TongCongNo ?? 0;
                decimal tienDaThanhToan = thanhToanHienTai?.SoTienDaThanhToan ?? 0;

                // Lấy Phụ thu từ BaoTriDAL (dùng hàm mới thêm ở Bước 1)
                // (Trong ảnh demo của bạn là 300,000)
                decimal tienPhuThu = _baoTriService.GetTongChiPhiBaoTriByPhong(phong.MaPhong);


                // === Xây dựng danh sách Tóm Tắt (Cột 3) ===

                // Thêm "Hóa đơn chính"
                DanhSachTomTat.Add(new MucTomTat
                {
                    TieuDe = "Hóa đơn Số: ",
                    MaSo = SelectedBillInfo.MaHoaDon,
                    Ngay = SelectedBillInfo.NgayLapHoaDon,
                    SoTien = tienHoaDonChinh
                });

                // Thêm "Công nợ" (nếu có)
                if (tienCongNoCu > 0)
                {
                    DanhSachTomTat.Add(new MucTomTat
                    {
                        TieuDe = "Công nợ",
                        Ngay = thanhToanHienTai?.NgayTao, // Lấy ngày của bản ghi công nợ
                        SoTien = tienCongNoCu
                    });
                }

                // Thêm "Phụ thu" (nếu có)
                if (tienPhuThu > 0)
                {
                    DanhSachTomTat.Add(new MucTomTat
                    {
                        TieuDe = "Phụ thu",
                        // (Bạn có thể lấy ngày gần nhất từ bảng BaoTri nếu muốn)
                        SoTien = tienPhuThu
                    });
                }

                if (tienDaThanhToan > 0)
                {
                    DanhSachTomTat.Add(new MucTomTat
                    {
                        TieuDe = "Đã thanh toán",
                        // (Bạn có thể lấy ngày thanh toán gần nhất từ thanhToanHienTai)
                        Ngay = thanhToanHienTai?.NgayCapNhat,
                        SoTien = tienDaThanhToan
                    });
                }

                // 4. Tính "Số tiền cần trả" (Logic đã sửa)
                // (Tổng phải trả) = Hóa đơn + Công nợ + Phụ thu
                decimal tongCacKhoanPhi = tienHoaDonChinh + tienCongNoCu + tienPhuThu;

                // (Tiền cuối cùng) = (Tổng phải trả) - (Đã thanh toán)
                TongTienCanTra = tongCacKhoanPhi - tienDaThanhToan;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi lấy dữ liệu hóa đơn: {ex.Message}");
            }
        }


        /// <summary>
        /// Tải danh sách Nhà và các Phòng con cho cột 1 (Sidebar)
        /// (Code này của bạn đã đúng)
        /// </summary>
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
    }
}