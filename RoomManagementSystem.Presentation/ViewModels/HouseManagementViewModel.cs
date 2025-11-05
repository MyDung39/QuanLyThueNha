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
    public partial class HouseManagementViewModel : ViewModelBase
    {
        private readonly QL_TaiSan_Phong _service;

        // DANH SÁCH NHÀ (cho cột trái)
        [ObservableProperty]
        private ObservableCollection<Nha> _danhSachNha;

        // DANH SÁCH PHÒNG (cho cột phải)
        [ObservableProperty]
        private ObservableCollection<Phong> _danhSachPhongHienThi;

        // LƯU TRỮ NHÀ ĐANG ĐƯỢC CHỌN
        [ObservableProperty]
        private Nha _selectedNha;
        
        // Lưu trữ toàn bộ danh sách phòng để lọc
        private List<Phong> _allRooms = new List<Phong>();

        public HouseManagementViewModel()
        {
            _service = new QL_TaiSan_Phong();
            _danhSachNha = new ObservableCollection<Nha>();
            _danhSachPhongHienThi = new ObservableCollection<Phong>();
            
            LoadData();
        }

        private void LoadData()
        {
            try
            {
                var houses = _service.DanhSachNha();
                _allRooms = _service.DanhSachPhong();

                DanhSachNha.Clear();
                foreach(var house in houses)
                {
                    DanhSachNha.Add(house);
                }

                // Tự động chọn nhà đầu tiên trong danh sách
                if (DanhSachNha.Any())
                {
                    SelectedNha = DanhSachNha.First();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải dữ liệu: {ex.Message}");
            }
        }

        // HÀM ĐƯỢC GỌI KHI 'SelectedNha' THAY ĐỔI
        // (Bộ tạo mã sẽ tự động tạo partial method này)
        partial void OnSelectedNhaChanged(Nha value)
        {
            if (value == null)
            {
                DanhSachPhongHienThi.Clear();
                return;
            }

            // Lọc và hiển thị các phòng thuộc về nhà được chọn
            var roomsInHouse = _allRooms.Where(r => r.MaNha == value.MaNha);
            
            DanhSachPhongHienThi.Clear();
            foreach(var room in roomsInHouse)
            {
                DanhSachPhongHienThi.Add(room);
            }
        }

        // CÁC COMMAND CHO CÁC NÚT BẤM (sẽ làm sau)
        [RelayCommand] private void AddHouse() { /* Logic mở popup thêm nhà */ }
        [RelayCommand] private void EditHouse() { /* Logic mở popup sửa nhà */ }
        [RelayCommand] private void DeleteHouse() { /* Logic mở popup xóa nhà */ }

        [RelayCommand] private void AddRoom() { /* Logic mở popup thêm phòng */ }
        [RelayCommand] private void EditRoom() { /* Logic mở popup sửa phòng */ }
        [RelayCommand] private void DeleteRoom() { /* Logic mở popup xóa phòng */ }
    }
}