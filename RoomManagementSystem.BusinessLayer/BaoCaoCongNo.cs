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
        // Trong file RoomManagementSystem.BusinessLayer.BaoCaoCongNo.cs

        public DataTable LayBaoCaoCongNo()
        {
            DataTable dt = cn.GetDanhSachCongNo();

            dt.Columns.Add("SoThangNo", typeof(int));
            foreach (DataRow row in dt.Rows)
            {
                // Kiểm tra nếu NgayHanThanhToan không phải là NULL
                if (row["NgayHanThanhToan"] != DBNull.Value && row["NgayHanThanhToan"] != null)
                {
                    DateTime han = Convert.ToDateTime(row["NgayHanThanhToan"]);
                    int thangNo = ((DateTime.Now.Year - han.Year) * 12) + DateTime.Now.Month - han.Month;
                    row["SoThangNo"] = Math.Max(thangNo, 0);
                }
                else
                {
                    // Nếu ngày hạn là NULL thì coi như chưa quá hạn (0 tháng)
                    row["SoThangNo"] = 0;
                }
            }
            return dt;
        }

        // Đồng thời, sửa hàm tính tổng để tránh lỗi nếu SoTienConLai bị NULL
        public decimal TongCongNoHeThong(DataTable dt)
        {
            decimal tong = 0;
            foreach (DataRow row in dt.Rows)
            {
                if (row["SoTienConLai"] != DBNull.Value)
                {
                    tong += Convert.ToDecimal(row["SoTienConLai"]);
                }
            }
            return tong;
        }
        public DataTable LayLichSuThanhToan(string maKhach)
        {
            return cn.GetLichSuThanhToan(maKhach);
        }
    }
}
