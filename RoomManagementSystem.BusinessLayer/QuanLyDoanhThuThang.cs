using ClosedXML.Excel;
using RoomManagementSystem.DataLayer;
using System;
using System.Data;

namespace RoomManagementSystem.BusinessLayer
{
    public class QuanLyDoanhThuThang
    {
        DoanhThuThang dt = new DoanhThuThang();

        // Lấy báo cáo doanh thu theo tháng
        public DataTable LayBaoCaoThang(int thang, int nam)
        {
            return dt.BaoCaoDoanhThuThang(thang, nam);
        }

        // Xuất Excel đẹp
        public void ExportToExcel(DataTable dt, string filePath, string sheetName = "DoanhThu")
        {
            using (var workbook = new XLWorkbook())
            {
                var ws = workbook.Worksheets.Add(sheetName);

                // ===== Tiêu đề =====
                var titleRange = ws.Range("A1:H1").Merge();
                titleRange.Value = $"BÁO CÁO DOANH THU THÁNG {DateTime.Now:MM/yyyy}";
                titleRange.Style.Font.SetBold();
                titleRange.Style.Font.FontSize = 16;
                titleRange.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                // ===== Bảng dữ liệu =====
                ws.Cell(3, 1).InsertTable(dt, "BáoCáo");

                // ===== Style Header =====
                var headerRange = ws.Range(3, 1, 3, dt.Columns.Count);
                headerRange.Style.Font.SetBold();
                headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;
                headerRange.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                // ===== Căn giữa các cột mã =====
                ws.Columns("A:B").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                // ===== Format tiền =====
                foreach (var colName in new[] { "GiaThue", "TongDoanhThuThucTe", "TongDaThanhToan", "TongNoConLai" })
                {
                    if (dt.Columns.Contains(colName))
                        ws.Column(dt.Columns[colName].Ordinal + 1)
                          .Style.NumberFormat.Format = "#,##0 \"₫\"";
                }

                // ===== Border & autofit =====
                ws.RangeUsed().Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                ws.RangeUsed().Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                ws.Columns().AdjustToContents();

                workbook.SaveAs(filePath);
            }
        }
    }
}
