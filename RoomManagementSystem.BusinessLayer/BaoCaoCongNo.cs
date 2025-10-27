using ClosedXML.Excel;
using DocumentFormat.OpenXml.Bibliography;
using RoomManagementSystem.DataLayer;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoomManagementSystem.BusinessLayer
{
    public class BaoCaoCongNo
    {
        CongNo cn = new CongNo();
        public DataTable LayBaoCaoCongNo()
        {
            DataTable dt = cn.GetDanhSachCongNo();

            dt.Columns.Add("SoThangNo", typeof(int));
            foreach (DataRow row in dt.Rows)
            {
                DateTime han = Convert.ToDateTime(row["NgayHanThanhToan"]);
                int thangNo = ((DateTime.Now.Year - han.Year) * 12) + DateTime.Now.Month - han.Month;
                row["SoThangNo"] = Math.Max(thangNo, 0);
            }
            return dt;
        }
        public DataTable LayLichSuThanhToan(string maKhach)
        {
            return cn.GetLichSuThanhToan(maKhach);
        }

        public decimal TongCongNoHeThong(DataTable dt)
        {
            return dt.AsEnumerable().Sum(r => r.Field<decimal>("SoTienConLai"));
        }
        public bool ExportCongNoToExcel(DataTable dt, string filePath)
        {
            try
            {
                using (var wb = new XLWorkbook())
                {
                    var ws = wb.Worksheets.Add("Báo cáo Công nợ");

                    ws.Cell(1, 1).InsertTable(dt);

                    var header = ws.Row(1);
                    header.Style.Font.Bold = true;
                    header.Style.Fill.BackgroundColor = XLColor.LightSteelBlue;
                    header.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                    // Format tiền + ngày
                    ws.Column(dt.Columns["SoTienConLai"].Ordinal + 1)
                        .Style.NumberFormat.Format = "#,##0 VNĐ";

                    ws.Column(dt.Columns["NgayHanThanhToan"].Ordinal + 1)
                        .Style.DateFormat.Format = "dd/MM/yyyy";

                    // Tô đỏ nếu nợ > 2 tháng
                    int soThangNoCol = dt.Columns["SoThangNo"].Ordinal + 1;
                    foreach (var row in ws.RangeUsed().RowsUsed().Skip(1))
                    {
                        int soThangNo = row.Cell(soThangNoCol).GetValue<int>();
                        if (soThangNo >= 2)
                            row.Style.Fill.BackgroundColor = XLColor.LightPink;
                    }

                    ws.RangeUsed().Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    ws.RangeUsed().Style.Border.InsideBorder = XLBorderStyleValues.Thin;

                    ws.Columns().AdjustToContents();

                    wb.SaveAs(filePath);
                }

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi xuất Excel: " + ex.Message);
            }
        }
    }
}
