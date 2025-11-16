using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using ClosedXML.Excel;

namespace RoomManagementSystem.BusinessLayer
{
    public class LoiNhuanBL
    {
        private readonly DataLayer.LoiNhuan loiNhuanDAL = new DataLayer.LoiNhuan();
        public DataTable GetLoiNhuanThang(string thoiKy)
        {
            return loiNhuanDAL.GetLoiNhuanThang(thoiKy);
        }
        public bool XuatExcel(string filePath, string thoiKy)
        {
            try
            {
                DataTable dt = loiNhuanDAL.GetLoiNhuanThang(thoiKy);

                if (dt == null || dt.Rows.Count == 0)
                    return false;

                using (var wb = new XLWorkbook())
                {
                    var ws = wb.Worksheets.Add("BaoCaoLoiNhuan");

                    // Tiêu đề
                    ws.Cell(1, 1).Value = "BÁO CÁO LỢI NHUẬN THÁNG";
                    ws.Cell(2, 1).Value = $"Thời kỳ: {thoiKy}";

                    ws.Range("A1:E1").Merge().Style.Font.SetBold().Font.FontSize = 16;
                    ws.Range("A2:E2").Merge().Style.Font.SetBold();

                    // Ghi DataTable vào Excel
                    ws.Cell(4, 1).InsertTable(dt);

                    // Format tiền
                    int rowCount = dt.Rows.Count;
                    ws.Range(5, 2, 4 + rowCount, dt.Columns.Count)
                      .Style.NumberFormat.Format = "#,##0";

                    ws.Columns().AdjustToContents();

                    wb.SaveAs(filePath);
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Xuất Excel lỗi: " + ex.Message);
                return false;
            }
        }
    }
}
