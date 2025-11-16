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
    public partial class ReportDebtListViewModel : ObservableObject
    {
        private readonly BaoCaoCongNo _debtService;

        // -------------------------
        // MODEL CON (GỘP TRONG FILE)
        // -------------------------
        public class ReportDebtItemViewModel
        {
            public int STT { get; set; }
            public string NguoiThue { get; set; }
            public string Phong { get; set; }
            public decimal SoTienNo { get; set; }
        }

        // -------------------------
        // THUỘC TÍNH BINDING
        // -------------------------
        [ObservableProperty]
        private ObservableCollection<ReportDebtItemViewModel> _debtData = new();

        public ReportDebtListViewModel()
        {
            _debtService = new BaoCaoCongNo();
            LoadData();
        }

        // -------------------------
        // LOAD DỮ LIỆU CÔNG NỢ
        // -------------------------
        [RelayCommand]
        private void LoadData()
        {
            try
            {
                DataTable dt = _debtService.LayBaoCaoCongNo();

                var list = new ObservableCollection<ReportDebtItemViewModel>();
                int stt = 1;

                foreach (DataRow row in dt.Rows)
                {
                    list.Add(new ReportDebtItemViewModel
                    {
                        STT = stt++,
                        NguoiThue = row["HoTen"].ToString(),
                        Phong = row["MaPhong"].ToString(),
                        SoTienNo = Convert.ToDecimal(row["SoTienConLai"])
                    });
                }

                DebtData = list;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải dữ liệu công nợ: {ex.Message}");
            }
        }

        // -------------------------
        // CLICK NÚT XUẤT EXCEL
        // -------------------------
        [RelayCommand]
        private void Download()
        {
            if (DebtData == null || DebtData.Count == 0)
            {
                MessageBox.Show("Không có dữ liệu để xuất.", "Thông báo");
                return;
            }

            var saveDialog = new SaveFileDialog
            {
                Filter = "Excel Workbook (*.xlsx)|*.xlsx",
                Title = "Lưu báo cáo công nợ",
                FileName = $"BaoCaoCongNo_{DateTime.Now:yyyy_MM}.xlsx"
            };

            if (saveDialog.ShowDialog() == true)
            {
                try
                {
                    ExportExcel(saveDialog.FileName);
                    MessageBox.Show("Xuất file thành công!", "Thành công");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi khi xuất file Excel: {ex.Message}");
                }
            }
        }

        // -------------------------
        // TẠO FILE EXCEL
        // -------------------------
        private void ExportExcel(string filePath)
        {
            using (var workbook = new ClosedXML.Excel.XLWorkbook())
            {
                var dt = new DataTable();

                dt.Columns.Add("STT", typeof(int));
                dt.Columns.Add("Người thuê", typeof(string));
                dt.Columns.Add("Phòng", typeof(string));
                dt.Columns.Add("Số tiền nợ", typeof(decimal));

                foreach (var item in DebtData)
                {
                    dt.Rows.Add(item.STT, item.NguoiThue, item.Phong, item.SoTienNo);
                }

                var ws = workbook.Worksheets.Add("CongNo");
                ws.Cell(1, 1).InsertTable(dt, "CongNo", true);
                ws.Columns().AdjustToContents();

                workbook.SaveAs(filePath);
            }
        }
    }
}
