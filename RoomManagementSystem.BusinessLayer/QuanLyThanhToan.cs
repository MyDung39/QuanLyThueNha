using DocumentFormat.OpenXml.Bibliography;
using RoomManagementSystem.DataLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace RoomManagementSystem.BusinessLayer
{
    public class QuanLyThanhToan
    {
        public ThanhToanDAL dal = new ThanhToanDAL();
        public bool ThemThanhToan(ThanhToan thanhToan)
        {
            // Có thể thêm kiểm tra nghiệp vụ ở đây
            if (thanhToan == null)
                throw new ArgumentNullException("thanhToan", "Dữ liệu thanh toán không được rỗng.");

            if (thanhToan.TongCongNo < 0)
                throw new ArgumentException("Tổng công nợ không hợp lệ.");

            return dal.ThemThanhToan(thanhToan);
        }
        public bool CapNhatThanhToan(string maThanhToan, decimal soTien, string phuongThuc, string ghiChu)
        {
            if (string.IsNullOrEmpty(maThanhToan))
                throw new ArgumentException("Mã thanh toán không được để trống.");

            if (soTien <= 0)
                throw new ArgumentException("Số tiền thanh toán phải lớn hơn 0.");

            return dal.CapNhatThanhToan(maThanhToan, soTien, phuongThuc, ghiChu);
        }
        public DataTable LayDanhSachThanhToan()
        {
            return dal.GetDanhSachThanhToan();
        }

    }
}
