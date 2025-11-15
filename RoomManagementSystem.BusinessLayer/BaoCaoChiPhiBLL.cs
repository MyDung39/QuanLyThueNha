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

        // Giữ nguyên phương thức cũ
        public DataTable GetChiPhiThang(string thoiKy)
        {
            return dal.GetChiPhiThang(thoiKy);
        }

        // ✅ Thêm overload nhận month, year
        public DataTable GetChiPhiThang(int month, int year)
        {
            return dal.GetChiPhiThang(month, year);
        }

        public bool ExportExcel(string thoiKy, string filePath)
        {
            DataTable dt = dal.GetChiPhiThang(thoiKy);
            if (dt == null || dt.Rows.Count == 0) return false;

            using (var wb = new ClosedXML.Excel.XLWorkbook())
            {
                var ws = wb.Worksheets.Add("Báo cáo chi phí");
                ws.Cell("A1").Value = "BÁO CÁO CHI PHÍ ";
                ws.Range("A1:F1").Merge().Style.Font.SetBold().Font.SetFontSize(16)
                    .Alignment.SetHorizontal(ClosedXML.Excel.XLAlignmentHorizontalValues.Center);
                ws.Cell("A2").Value = $"Thời kỳ: {thoiKy}";
                ws.Range("A2:F2").Merge().Style.Font.SetBold()
                    .Alignment.SetHorizontal(ClosedXML.Excel.XLAlignmentHorizontalValues.Left);
                ws.Cell(4, 1).InsertTable(dt, true);
                ws.Columns().AdjustToContents();
                ws.SheetView.FreezeRows(4);
                wb.SaveAs(filePath);
            }

            return true;
        }

        // ✅ Thêm overload ExportExcel nhận month, year
        public bool ExportExcel(int month, int year, string filePath)
        {
            DataTable dt = dal.GetChiPhiThang(month, year);
            if (dt == null || dt.Rows.Count == 0) return false;

            string period = $"{month:00}/{year}";

            using (var wb = new ClosedXML.Excel.XLWorkbook())
            {
                var ws = wb.Worksheets.Add("Báo cáo chi phí");
                ws.Cell("A1").Value = "BÁO CÁO CHI PHÍ ";
                ws.Range("A1:F1").Merge().Style.Font.SetBold().Font.SetFontSize(16)
                    .Alignment.SetHorizontal(ClosedXML.Excel.XLAlignmentHorizontalValues.Center);
                ws.Cell("A2").Value = $"Thời kỳ: {period}";
                ws.Range("A2:F2").Merge().Style.Font.SetBold()
                    .Alignment.SetHorizontal(ClosedXML.Excel.XLAlignmentHorizontalValues.Left);
                ws.Cell(4, 1).InsertTable(dt, true);
                ws.Columns().AdjustToContents();
                ws.SheetView.FreezeRows(4);
                wb.SaveAs(filePath);
            }

            return true;
        }
    }

}
