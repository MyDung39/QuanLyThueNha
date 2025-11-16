using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using RoomManagementSystem.BusinessLayer;
using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Windows;

namespace RoomManagementSystem.Presentation.ViewModels
{
    public class ReportRoomItemViewModel : ObservableObject
    {
        public int STT { get; set; }
        public string Phong { get; set; }
        public string TinhTrang { get; set; }
        public decimal SoTien { get; set; }
        public bool IsSummary { get; set; } = false;
    }

    public partial class ReportRoomListViewModel : ObservableObject
    {
        private readonly ThongKeTinhTrangPhong _roomService;

        [ObservableProperty]
        private ObservableCollection<ReportRoomItemViewModel> _roomData = new();

        [ObservableProperty]
        private string _selectedStatus;

        [ObservableProperty]
        private ObservableCollection<string> _statuses;

        public ReportRoomListViewModel()
        {
            _roomService = new ThongKeTinhTrangPhong();

            Statuses = new ObservableCollection<string>
            {
                "Tất cả",
                "Trống",
                "Đang thuê",
                "Dự kiến",
                "Bảo trì"
            };
            SelectedStatus = "Tất cả";

            LoadData();
        }

        partial void OnSelectedStatusChanged(string value)
        {
            LoadData();
        }

        [RelayCommand]
        private void LoadData()
        {
            try
            {
                var list = new ObservableCollection<ReportRoomItemViewModel>();

                if (SelectedStatus == "Trống" || SelectedStatus == "Tất cả")
                    list = MergeDataTable(list, _roomService.GetPhongTrong(), "Trống");

                if (SelectedStatus == "Đang thuê" || SelectedStatus == "Tất cả")
                    list = MergeDataTable(list, _roomService.GetPhongDangThue(), "Đang thuê");

                if (SelectedStatus == "Dự kiến" || SelectedStatus == "Tất cả")
                    list = MergeDataTable(list, _roomService.GetPhongSapTrong(), "Dự kiến");

                if (SelectedStatus == "Bảo trì" || SelectedStatus == "Tất cả")
                    list = MergeDataTable(list, _roomService.GetPhongBaoTri(), "Bảo trì");

                RoomData = list;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải dữ liệu phòng: {ex.Message}");
            }
        }

        private ObservableCollection<ReportRoomItemViewModel> MergeDataTable(ObservableCollection<ReportRoomItemViewModel> list, DataTable dt, string status)
        {
            int stt = list.Count + 1;
            foreach (DataRow row in dt.Rows)
            {
                list.Add(new ReportRoomItemViewModel
                {
                    STT = stt++,
                    Phong = row["MaPhong"].ToString(),
                    TinhTrang = status,
                    SoTien = row.Table.Columns.Contains("GiaThue") && row["GiaThue"] != DBNull.Value
                        ? Convert.ToDecimal(row["GiaThue"])
                        : 0
                });
            }
            return list;
        }

        [RelayCommand]
        private void Download()
        {
            if (RoomData == null || RoomData.Count == 0)
            {
                MessageBox.Show("Không có dữ liệu để xuất file.", "Thông báo");
                return;
            }

            try
            {
                var saveFileDialog = new SaveFileDialog
                {
                    Filter = "Excel Workbook|*.xlsx",
                    Title = "Lưu báo cáo danh sách phòng",
                    FileName = $"BaoCaoDanhSachPhong_{DateTime.Now:yyyy_MM}.xlsx"
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    _roomService.XuatBaoCaoExcel(saveFileDialog.FileName);
                    MessageBox.Show($"Đã xuất file thành công!\nĐường dẫn: {saveFileDialog.FileName}", "Thành công");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi xuất file Excel: {ex.Message}");
            }
        }
    }
}
