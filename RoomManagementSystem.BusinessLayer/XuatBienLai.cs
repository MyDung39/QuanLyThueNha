using ClosedXML.Excel;
using RoomManagementSystem.DataLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoomManagementSystem.BusinessLayer
{
    public class XuatBienLai

    {
        BienLaiDAL bienLai = new BienLaiDAL();
        public List<BienLai> GetBienLai(string MTT)
        {
            return bienLai.GetBienLaiByPhong(MTT);
        }
        public void XuatBienLaiExcel(List<BienLai> bienLais, string filePath)
        {
            if (bienLais == null || bienLais.Count == 0) return;

            using (var wb = new XLWorkbook())
            {
                var ws = wb.Worksheets.Add("BienLai");
                int row = 1;

                var first = bienLais[0];

                ws.Style.Font.SetFontName("Times New Roman");

                // ======= THÔNG TIN KHÁCH HÀNG =======
                ws.Cell(row, 1).Value = "Kính gửi:";
                row++;

                ws.Cell(row, 1).Value = "[Khách hàng]";
                ws.Cell(row, 2).Value = first.HoTen;
                ws.Cell(row, 5).Value = "Phòng:";
                ws.Cell(row, 6).Value = first.MaPhong;
                row++;

                ws.Cell(row, 1).Value = "[Điện thoại]";
                ws.Cell(row, 2).Value = first.Sdt;
                ws.Cell(row, 5).Value = "Số người lưu trú";
                ws.Cell(row, 6).Value = first.SoNguoiHienTai;
                row += 2;

                // ======= HEADER BẢNG DỊCH VỤ =======
                ws.Cell(row, 1).Value = "STT";
                ws.Cell(row, 2).Value = "Nội dung chi tiết";
                ws.Cell(row, 3).Value = "Đơn vị";
                ws.Cell(row, 4).Value = "Đơn giá";
                ws.Cell(row, 5).Value = "Số lượng";
                ws.Cell(row, 6).Value = "Thành tiền";

                ws.Range(row, 1, row, 6).Style
                    .Font.SetBold()
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                    .Fill.SetBackgroundColor(XLColor.FromHtml("#C8D3DC"));

                row++;

                // ======= DỮ LIỆU =======
                int stt = 1;
                foreach (var item in bienLais)
                {
                    ws.Cell(row, 1).Value = stt++;
                    ws.Cell(row, 2).Value = item.TenDichVu;
                    ws.Cell(row, 3).Value = item.DVT;
                    ws.Cell(row, 4).Value = item.DonGia;
                    ws.Cell(row, 5).Value = item.SoLuong;
                    ws.Cell(row, 6).Value = item.ThanhTien;
                    row++;
                }

                // ======= TỔNG CỘNG =======
                ws.Cell(row, 1).Value = "TỔNG CỘNG";
                ws.Range(row, 1, row, 5).Merge();
                ws.Cell(row, 1).Style.Font.SetBold();
                ws.Cell(row, 6).FormulaA1 = $"=SUM(F{row - stt + 1}:F{row - 1})";
                ws.Cell(row, 6).Style.Font.SetBold();

                // ======= FORMAT =======
                ws.Columns().AdjustToContents();
                ws.Range("A1:F" + row).Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                                               .Border.SetInsideBorder(XLBorderStyleValues.Thin);

                wb.SaveAs(filePath);
            }
        }
    }
}
