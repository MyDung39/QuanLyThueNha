using RoomManagementSystem.DataLayer;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClosedXML.Excel;

namespace RoomManagementSystem.BusinessLayer
{
    public class QlThanhToan
    {
        private ThanhToanDAL dal = new ThanhToanDAL();

        // Lấy danh sách tất cả thanh toán
        public List<ThanhToan> GetAll()
        {
            return dal.GetAll();
        }

        // Ghi nhận thanh toán
        public void Insert(ThanhToan thanhToan)
        {
            // Có thể thêm kiểm tra nghiệp vụ ở đây
            if (string.IsNullOrEmpty(thanhToan.MaPhong))
                throw new Exception("Mã phòng không được để trống!");

            dal.Insert(thanhToan);
        }

        // Cập nhật trạng thái
        public void UpdateTrangThai(string maThanhToan, string trangThai)
        {
            dal.UpdateTrangThai(maThanhToan, trangThai);
        }

        // Báo cáo theo tháng
        public DataTable GetBaoCaoThang(int thang, int nam)
        {
            return dal.GetBaoCaoThang(thang, nam);
        }

        public DataTable BienLai(string mtt)
        {
            return dal.BienLai(mtt);
        }
        public void ExportBienLaiExcel(string path, DataTable dtBienLai)
        {
            using (var workbook = new XLWorkbook())
            {
                var ws = workbook.Worksheets.Add("BienLai");

                // Tiêu đề
                ws.Cell("A1").Value = "BIÊN LAI THANH TOÁN";
                ws.Cell("A1").Style.Font.Bold = true;
                ws.Cell("A1").Style.Font.FontSize = 16;
                ws.Range("A1:E1").Merge().Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                // Ghi DataTable
                ws.Cell(3, 1).InsertTable(dtBienLai, "BienLai", true);

                ws.Columns().AdjustToContents();
                workbook.SaveAs(path);
            }
        }
    }
}
