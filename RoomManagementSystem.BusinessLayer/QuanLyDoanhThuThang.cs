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

        /*
        public decimal TinhTongDoanhThu(int thang, int nam)
        {
            DataTable dtBaoCao = LayBaoCaoThang(thang, nam);
            decimal tong = 0;

            foreach (DataRow row in dtBaoCao.Rows)
            {
                if (decimal.TryParse(row["DoanhThu"].ToString(), out decimal doanhThu))
                    tong += doanhThu;
            }

            return tong;
        }
        */


        public decimal TinhTongDoanhThu(int thang, int nam)
        {
            // Ủy quyền việc tính tổng cho tầng DAL để có hiệu suất tốt nhất
            return dt.TinhTongDoanhThuThang(thang, nam);
        }

        //Xuat Excel
        public void ExportToExcel(DataTable dt, string filePath, string sheetName = "Sheet1")
        {
            using (var workbook = new XLWorkbook())
            {
                var ws = workbook.Worksheets.Add(sheetName);
                ws.Cell(1, 1).InsertTable(dt);
                ws.Columns().AdjustToContents();

                // Định dạng cột Tiền thuê nếu có
                if (dt.Columns.Contains("Tiền thuê"))
                {
                    // Lấy cột theo tên và định dạng tiền tệ
                    var colIndex = dt.Columns["Tiền thuê"].Ordinal + 1;
                    ws.Column(colIndex).Style.NumberFormat.Format = "#,##0 \"VNĐ\"";
                }

                // ...
                workbook.SaveAs(filePath);
            }
        }
    }
}
