using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RoomManagementSystem.BusinessLayer;
using RoomManagementSystem.DataLayer;
using System;
using System.Collections.ObjectModel;
using System.Windows;

namespace RoomManagementSystem.Presentation.ViewModels
{
    public partial class TenantManagementViewModel : ViewModelBase
    {
        private readonly QuanLyNguoiThue _nguoiThueService;

        [ObservableProperty]
        private ObservableCollection<NguoiThue> _danhSachNguoiThue;

        // --- BẮT ĐẦU PHẦN ĐIỀU KHIỂN POPUP ---

        // 1. Một thuộc tính duy nhất để BẬT/TẮT popup
        [ObservableProperty]
        private bool _isAddPopupVisible;

        // 2. Các thuộc tính để lưu dữ liệu từ các ô TextBox trong popup
        [ObservableProperty]
        private string _newTenantName = "";

        [ObservableProperty]
        private string _newTenantEmail = "";

        [ObservableProperty]
        private string _newTenantPhone = "";

        [ObservableProperty]
        private DateTime? _newTenantStartDate = DateTime.Now;

        // --- KẾT THÚC PHẦN ĐIỀU KHIỂN POPUP ---

        public TenantManagementViewModel()
        {
            _nguoiThueService = new QuanLyNguoiThue();
            _danhSachNguoiThue = new ObservableCollection<NguoiThue>();
            LoadData();
        }

        private void LoadData()
        {
            var danhSach = _nguoiThueService.getAll();
            DanhSachNguoiThue.Clear();
            foreach (var nguoiThue in danhSach)
            {
                DanhSachNguoiThue.Add(nguoiThue);
            }
        }

        // --- CÁC LỆNH (COMMANDS) ---

        // Lệnh được gọi bởi nút "+"
        [RelayCommand]
        private void OpenAddPopup()
        {
            // Reset dữ liệu cũ
            NewTenantName = "";
            NewTenantEmail = "";
            NewTenantPhone = "";
            NewTenantStartDate = DateTime.Now;

            // Bật popup
            IsAddPopupVisible = true;
        }

        // Lệnh được gọi bởi nút "Thêm" trong popup
        [RelayCommand]
        private void SaveNewTenant()
        {
            // (Bạn có thể thêm kiểm tra dữ liệu ở đây)
            try
            {
                var nguoiThueMoi = new NguoiThue
                {
                    HoTen = NewTenantName,
                    Email = NewTenantEmail,
                    Sdt = NewTenantPhone,
                    NgayBatDauThue = NewTenantStartDate ?? DateTime.Now,
                    TrangThaiThue = "Đang thuê",
                    NgayTao = DateTime.Now,
                    NgayCapNhat = DateTime.Now
                };

                if (_nguoiThueService.ThemNguoiThue(nguoiThueMoi))
                {
                    MessageBox.Show("Thêm thành công!");
                    LoadData(); // Tải lại danh sách
                    IsAddPopupVisible = false; // Đóng popup
                }
                else
                {
                    MessageBox.Show("Thêm thất bại!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}");
            }
        }

        // Lệnh được gọi bởi nút "X" trong popup
        [RelayCommand]
        private void CancelAdd()
        {
            // Chỉ cần đóng popup
            IsAddPopupVisible = false;
        }
    }
}