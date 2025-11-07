using ClosedXML.Excel;
using RoomManagementSystem.DataLayer;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace RoomManagementSystem.BusinessLayer
{
    public class BaoCaoLoiNhuan
    {
        private QuanLyDoanhThuThang qlDoanhThuThang = new QuanLyDoanhThuThang();
        private BaoCaoChiPhiBLL baoCaoChiPhiBLL = new BaoCaoChiPhiBLL();

        // 🔹 Tính lợi nhuận 1 tháng
        public decimal TinhLoiNhuanThang(int thang, int nam)
        {
            string thoiKy = $"{thang:D2}/{nam}";
            DataTable dtDoanhThu = qlDoanhThuThang.LayBaoCaoThang(thang, nam);
            DataTable dtChiPhi = baoCaoChiPhiBLL.GetChiPhiThang(thoiKy);

            decimal doanhThu = TinhTong(dtDoanhThu, "TongTien");
            decimal chiPhi = TinhTong(dtChiPhi, "TongChiPhi");

            return doanhThu - chiPhi; // Gross profit
        }

        // 🔹 Tính tổng theo cột
        private decimal TinhTong(DataTable dt, string column)
        {
            decimal sum = 0;
            if (dt == null || !dt.Columns.Contains(column)) return 0;
            foreach (DataRow row in dt.Rows)
                if (decimal.TryParse(row[column]?.ToString(), out decimal v))
                    sum += v;
            return sum;
        }

        // 🔹 Lấy xu hướng lợi nhuận 12 tháng
        public List<(string ThoiKy, decimal LoiNhuan)> LayXuHuongLoiNhuan(int nam)
        {
            List<(string, decimal)> data = new();
            for (int thang = 1; thang <= 12; thang++)
                data.Add(($"{thang:D2}/{nam}", TinhLoiNhuanThang(thang, nam)));
            return data;
        }

        // 🔹 So sánh tháng trước (%)
        public decimal SoSanhThangTruoc(int thang, int nam)
        {
            decimal hienTai = TinhLoiNhuanThang(thang, nam);
            int thangTruoc = thang - 1, namTruoc = nam;
            if (thangTruoc == 0) { thangTruoc = 12; namTruoc--; }

            decimal truoc = TinhLoiNhuanThang(thangTruoc, namTruoc);
            if (truoc == 0) return 0;
            return ((hienTai - truoc) / truoc) * 100;
        }

        // 🔹 Dự báo lợi nhuận tháng tới (trung bình 3 tháng gần nhất)
        public decimal DuBaoThangToi(int thang, int nam)
        {
            List<decimal> lst = new();
            for (int i = 2; i >= 0; i--)
            {
                int t = thang - i, y = nam;
                if (t <= 0) { t += 12; y--; }
                lst.Add(TinhLoiNhuanThang(t, y));
            }
            return lst.Average();
        }

        // 🔹 Xuất Excel
        public void ExportBaoCaoExcel(int nam, string filePath)
        {
            var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("Báo cáo lợi nhuận");

            // ====== Tiêu đề ======
            ws.Cell("A1").Value = "BÁO CÁO LỢI NHUẬN NĂM " + nam;
            ws.Range("A1:E1").Merge().Style
                .Font.SetBold().Font.SetFontSize(16)
                .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

            // ====== Header bảng ======
            ws.Cell("A3").Value = "Tháng";
            ws.Cell("B3").Value = "Doanh thu (VNĐ)";
            ws.Cell("C3").Value = "Chi phí (VNĐ)";
            ws.Cell("D3").Value = "Lợi nhuận (VNĐ)";
            ws.Cell("E3").Value = "So với tháng trước (%)";
            ws.Range("A3:E3").Style
                .Font.SetBold()
                .Fill.SetBackgroundColor(XLColor.LightGray)
                .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

            // ====== Ghi dữ liệu ======
            int row = 4;
            for (int thang = 1; thang <= 12; thang++)
            {
                string tk = $"{thang:D2}/{nam}";
                decimal doanhThu = TinhTong(qlDoanhThuThang.LayBaoCaoThang(thang, nam), "TongTien");
                decimal chiPhi = TinhTong(baoCaoChiPhiBLL.GetChiPhiThang(tk), "TongChiPhi");
                decimal loiNhuan = doanhThu - chiPhi;
                decimal ss = SoSanhThangTruoc(thang, nam);

                ws.Cell(row, 1).Value = thang;
                ws.Cell(row, 2).Value = doanhThu;
                ws.Cell(row, 3).Value = chiPhi;
                ws.Cell(row, 4).Value = loiNhuan;
                ws.Cell(row, 5).Value = ss;

                row++;
            }

            // Format tiền tệ
            ws.Range($"B4:D{row - 1}").Style.NumberFormat.Format = "#,##0";
            ws.Range($"E4:E{row - 1}").Style.NumberFormat.Format = "0.00\\%";
            ws.Columns().AdjustToContents();

            // ====== Dự báo tháng tới ======
            ws.Cell(row + 1, 3).Value = "Dự báo tháng tới:";
            ws.Cell(row + 1, 4).Value = DuBaoThangToi(12, nam);
            ws.Cell(row + 1, 4).Style.Font.SetBold().Font.SetFontColor(XLColor.Blue);

            // ====== Lưu file ======
            wb.SaveAs(filePath);
        }
    }
}
