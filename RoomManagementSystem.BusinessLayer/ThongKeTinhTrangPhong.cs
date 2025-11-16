using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RoomManagementSystem.DataLayer;
using System.Data;
using ClosedXML.Excel;

namespace RoomManagementSystem.BusinessLayer
{
    public class ThongKeTinhTrangPhong
    {
        BaoCaoTinhTrangPhong dal = new BaoCaoTinhTrangPhong();
        // Lấy phòng trống
        public DataTable GetPhongTrong()
        {
            return dal.GetPhongTrong();
        }

        // Lấy phòng đang thuê
        public DataTable GetPhongDangThue()
        {
            return dal.GetPhongDangThue();
        }

        // Lấy phòng sắp trống
        public DataTable GetPhongSapTrong()
        {
            return dal.GetPhongSapTrong();
        }

        // Lấy phòng bảo trì
        public DataTable GetPhongBaoTri()
        {
            return dal.GetPhongBaoTri();
        }

        // Lấy tỷ lệ lấp đầy
        public object GetTyLeLapDay()
        {
            return dal.GetTyLeLapDay();
        }

        public void AddSheet(XLWorkbook workbook, string sheetName, DataTable table)
        {
            var ws = workbook.Worksheets.Add(sheetName);

            // Thêm header
            ws.Cell(1, 1).InsertTable(table, sheetName, true);

            ws.Columns().AdjustToContents(); 

        
        }

        // Xuất báo cáo Excel
        public void XuatBaoCaoExcel(string filePath)
        {
            using (var workbook = new XLWorkbook())
            {
                // Thêm từng sheet
                AddSheet(workbook, "PhongTrong", GetPhongTrong());
                AddSheet(workbook, "PhongDangThue", GetPhongDangThue());
                AddSheet(workbook, "PhongSapTrong", GetPhongSapTrong());
                AddSheet(workbook, "PhongBaoTri", GetPhongBaoTri());


                // Tạo sheet "Tất cả"
                DataTable dtTatCa = new DataTable();
                dtTatCa.Columns.Add("STT", typeof(int));
                dtTatCa.Columns.Add("Phòng", typeof(string));
                dtTatCa.Columns.Add("Tình trạng", typeof(string));
                dtTatCa.Columns.Add("Số tiền", typeof(decimal));

                int stt = 1;

                void AddRowsFromTable(DataTable dt, string tinhTrang)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        decimal soTien = dt.Columns.Contains("GiaThue") && row["GiaThue"] != DBNull.Value
                            ? Convert.ToDecimal(row["GiaThue"])
                            : 0;

                        dtTatCa.Rows.Add(stt++, row["MaPhong"].ToString(), tinhTrang, soTien);
                    }
                }

                AddRowsFromTable(GetPhongTrong(), "Trống");
                AddRowsFromTable(GetPhongDangThue(), "Đang thuê");
                AddRowsFromTable(GetPhongSapTrong(), "Dự kiến");
                AddRowsFromTable(GetPhongBaoTri(), "Bảo trì");

                AddSheet(workbook, "TatCaPhong", dtTatCa);


                // Sheet tỷ lệ lấp đầy
                DataTable dtTyLe = new DataTable();
                dtTyLe.Columns.Add("TyLeLapDay (%)", typeof(decimal));
                dtTyLe.Rows.Add(GetTyLeLapDay());
                AddSheet(workbook, "TyLeLapDay", dtTyLe);

                // Lưu file
                workbook.SaveAs(filePath);
            }
        }

    }
}
