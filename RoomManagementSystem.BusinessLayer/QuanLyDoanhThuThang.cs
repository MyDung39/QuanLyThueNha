using ClosedXML.Excel;
using RoomManagementSystem.DataLayer;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoomManagementSystem.BusinessLayer
{
    public class QuanLyDoanhThuThang
    {
        DoanhThuThang dt = new DoanhThuThang();

        // Lấy báo cáo tháng 
        public DataTable LayBaoCaoThang(int thang, int nam)
        {
            DataTable dtBaoCao = dt.BaoCaoDoanhThuThang(thang, nam);
            return dtBaoCao;
        }

        //Xuat Excel
        public void ExportToExcel(DataTable dt, string filePath, string sheetName = "Sheet1")
        {
            using (var workbook = new XLWorkbook())
            {
                var ws = workbook.Worksheets.Add(sheetName);
                ws.Cell(1, 1).InsertTable(dt); // tự động ghi header + dữ liệu
                ws.Columns().AdjustToContents();

                // Format cột tiền
                if (dt.Columns.Contains("GiaThue"))
                    ws.Column("C").Style.NumberFormat.Format = "#,##0.00";

                workbook.SaveAs(filePath);
            }
        }
    }
}
