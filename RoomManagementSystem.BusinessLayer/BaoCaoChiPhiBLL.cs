using ClosedXML.Excel;
using RoomManagementSystem.DataLayer;
using System;
using System.Data;
using System.IO;

namespace RoomManagementSystem.BusinessLayer
{
    public class BaoCaoChiPhiBLL
    {
        private BaoCaoChiPhiDAL dal = new BaoCaoChiPhiDAL();
        public DataTable GetChiPhiThang(string thoiKy)
        {
            return dal.GetChiPhiThang(thoiKy);  
        }
        public bool ExportExcel(string thoiKy, string filePath)
        {
            DataTable dt = dal.GetChiPhiThang(thoiKy);

            if (dt == null || dt.Rows.Count == 0)
                return false;

            using (var wb = new XLWorkbook())
            {
                var ws = wb.Worksheets.Add("Báo cáo chi phí");

                // ✅ Tiêu đề
                ws.Cell("A1").Value = "BÁO CÁO CHI PHÍ ";
                ws.Range("A1:F1").Merge().Style
                    .Font.SetBold().Font.SetFontSize(16)
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                ws.Cell("A2").Value = $"Thời kỳ: {thoiKy}";
                ws.Range("A2:F2").Merge().Style
                    .Font.SetBold()
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);

                // ✅ Đưa dữ liệu DataTable vào từ dòng 4
                ws.Cell(4, 1).InsertTable(dt, true);

                // ✅ Format header
                var headerRange = ws.Range(4, 1, 4, dt.Columns.Count);
                headerRange.Style
                    .Font.SetBold()
                    .Fill.SetBackgroundColor(XLColor.LightBlue)
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                    .Border.SetInsideBorder(XLBorderStyleValues.Thin)
                    .Border.SetOutsideBorder(XLBorderStyleValues.Thick);

                // ✅ Format data rows
                var dataRange = ws.Range(5, 1, dt.Rows.Count + 4, dt.Columns.Count);
                dataRange.Style
                    .NumberFormat.SetNumberFormatId(4)   // ##,### kiểu tiền
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right)
                    .Border.SetInsideBorder(XLBorderStyleValues.Thin)
                    .Border.SetOutsideBorder(XLBorderStyleValues.Thin);

                // ✅ AutoFit tất cả cột
                ws.Columns().AdjustToContents();

                // ✅ Freeze header để kéo xuống vẫn thấy tiêu đề
                ws.SheetView.FreezeRows(4);

                wb.SaveAs(filePath);
            }

            return true;
        }
    }
}
