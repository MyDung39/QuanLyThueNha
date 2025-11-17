//using DocumentFormat.OpenXml.Bibliography;
//using System;
//using System.Collections.Generic;
//using System.Data;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace RoomManagementSystem.BusinessLayer
//{
//    public class DashBoardValue
//    {
//        public decimal TongDoanhThuThang(int month, int year)
//        {
//            QuanLyDoanhThuThang qldt = new QuanLyDoanhThuThang();
//            return qldt.TinhTongDoanhThu(month, year);
//        }
//        public decimal TinhPhanTramDoanhThuTang(int thang, int nam)
//        {
//            QuanLyDoanhThuThang dt = new QuanLyDoanhThuThang();
//            // Xác định tháng trước
//            int thangTruoc = thang - 1;
//            int namTruoc = nam;

//            if (thangTruoc == 0)
//            {
//                thangTruoc = 12;
//                namTruoc -= 1;
//            }

//            // Lấy doanh thu 2 tháng
//            decimal doanhThuHienTai = dt.TinhTongDoanhThu(thang, nam);
//            decimal doanhThuTruoc = dt.TinhTongDoanhThu(thangTruoc, namTruoc);

//            // Nếu tháng trước = 0 thì tránh chia cho 0
//            if (doanhThuTruoc == 0)
//                return 0;

//            // Công thức tính %
//            decimal phanTram = ((doanhThuHienTai - doanhThuTruoc) / doanhThuTruoc) * 100;
//            return Math.Round(phanTram, 2);
//        }
//        public DataTable ChiTietDoanhThu(int month, int year)
//        {
//            QuanLyDoanhThuThang qldt = new QuanLyDoanhThuThang();
//            return qldt.LayBaoCaoThang(month, year);
//        }
//        public DataTable ThongKePhongThueTrong()
//        {
//            ThongKeTinhTrangPhong qlp = new ThongKeTinhTrangPhong();
//            return qlp.GetPhongTrong();
//        }
//        public DataTable ThongKePhongDangThue()
//        {
//            ThongKeTinhTrangPhong qlp = new ThongKeTinhTrangPhong();
//            return qlp.GetPhongDangThue();
//        }
//        public DataTable ThongKePhongSapTrong()
//        {
//            ThongKeTinhTrangPhong qlp = new ThongKeTinhTrangPhong();
//            return qlp.GetPhongSapTrong();
//        }
//        public (decimal phongTrong, decimal phongDangThue, decimal phongSapTrong) TinhPhanTramTinhTrangPhong()
//        {
//            ThongKeTinhTrangPhong qlp = new ThongKeTinhTrangPhong();

//            DataTable dtTrong = qlp.GetPhongTrong();
//            DataTable dtDangThue = qlp.GetPhongDangThue();
//            DataTable dtSapTrong = qlp.GetPhongSapTrong();

//            int soPhongTrong = dtTrong.Rows.Count;
//            int soPhongDangThue = dtDangThue.Rows.Count;
//            int soPhongSapTrong = dtSapTrong.Rows.Count;

//            int tongPhong = soPhongTrong + soPhongDangThue + soPhongSapTrong;

//            if (tongPhong == 0)
//                return (0, 0, 0);

//            decimal ptTrong = Math.Round((decimal)soPhongTrong / tongPhong * 100, 2);
//            decimal ptDangThue = Math.Round((decimal)soPhongDangThue / tongPhong * 100, 2);
//            decimal ptSapTrong = Math.Round((decimal)soPhongSapTrong / tongPhong * 100, 2);

//            return (ptTrong, ptDangThue, ptSapTrong);
//        }
//        public DataTable ChiTietPhiThang(int month, int year)
//        {
//            BaoCaoChiPhiBLL bccp = new BaoCaoChiPhiBLL();
//            return bccp.GetChiPhiThang(month, year);
//        }
//        public decimal TongChiPhiDien(int month, int year)
//        {
//            BaoCaoChiPhiBLL bc = new BaoCaoChiPhiBLL();
//            DataTable dt = bc.GetChiPhiThang(month, year);
//            decimal sum = 0;

//            for (int i = 0; i < dt.Rows.Count; i++)
//            {
//                if (dt.Rows[i]["Chi phí điện"] != DBNull.Value)
//                {
//                    sum += Convert.ToDecimal(dt.Rows[i]["Chi phí điện"]);
//                }
//            }
//            return sum;
//        }
//        public decimal TongChiPhiNuoc(int month, int year)
//        {
//            BaoCaoChiPhiBLL bc = new BaoCaoChiPhiBLL();
//            DataTable dt = bc.GetChiPhiThang(month, year);
//            decimal sum = 0;

//            for (int i = 0; i < dt.Rows.Count; i++)
//            {
//                if (dt.Rows[i]["Chi phí nước"] != DBNull.Value)
//                {
//                    sum += Convert.ToDecimal(dt.Rows[i]["Chi phí nước"]);
//                }
//            }
//            return sum;
//        }
//        public decimal TongChiBaoTri(int month, int year)
//        {
//            BaoCaoChiPhiBLL bc = new BaoCaoChiPhiBLL();
//            DataTable dt = bc.GetChiPhiThang(month, year);
//            decimal sum = 0;

//            for (int i = 0; i < dt.Rows.Count; i++)
//            {
//                if (dt.Rows[i]["Chi phí bảo trì"] != DBNull.Value)
//                {
//                    sum += Convert.ToDecimal(dt.Rows[i]["Chi phí bảo trì"]);
//                }
//            }
//            return sum;
//        }
//        public DataTable ChiTietCongNo()
//        {
//            BaoCaoCongNo bc = new BaoCaoCongNo();
//            return bc.LayBaoCaoCongNo();
//        }
//        public decimal TinhLoiNhuan(int t, int n)
//        {
//            // 1. Lấy tổng doanh thu tháng
//            QuanLyDoanhThuThang qldt = new QuanLyDoanhThuThang();
//            decimal doanhThu = qldt.TinhTongDoanhThu(t, n);

//            // 2. Lấy tổng chi phí tháng (điện + nước + bảo trì)
//            string thoiKy = $"{t:D2}/{n}"; // Định dạng "mm/yyyy"
//            BaoCaoChiPhiBLL bc = new BaoCaoChiPhiBLL();
//            DataTable dtChiPhi = bc.GetChiPhiThang(thoiKy);

//            decimal tongChiPhi = 0;
//            foreach (DataRow row in dtChiPhi.Rows)
//            {
//                if (row["Chi phí điện"] != DBNull.Value)
//                    tongChiPhi += Convert.ToDecimal(row["Chi phí điện"]);
//                if (row["Chi phí nước"] != DBNull.Value)
//                    tongChiPhi += Convert.ToDecimal(row["Chi phí nước"]);
//                if (row["Chi phí bảo trì"] != DBNull.Value)
//                    tongChiPhi += Convert.ToDecimal(row["Chi phí bảo trì"]);
//                if (row["Chi phí Internet"] != DBNull.Value)
//                    tongChiPhi += Convert.ToDecimal(row["Chi phí Internet"]);
//                if (row["Chi phí rác"] != DBNull.Value)
//                    tongChiPhi += Convert.ToDecimal(row["Chi phí rác"]);

//            }


//            // 3. Tính lợi nhuận
//            decimal loiNhuan = doanhThu - tongChiPhi;

//            return Math.Round(loiNhuan, 2);
//        }
//        public decimal TinhPhanTramLoiNhuanTang(int thang, int nam)
//        {
//            // Tạo đối tượng DashBoardValue để gọi lại TinhLoiNhuan()
//            DashBoardValue db = new DashBoardValue();

//            // Xác định tháng trước
//            int thangTruoc = thang - 1;
//            int namTruoc = nam;
//            if (thangTruoc == 0)
//            {
//                thangTruoc = 12;
//                namTruoc -= 1;
//            }

//            // Lấy lợi nhuận tháng hiện tại và tháng trước
//            decimal loiNhuanHienTai = db.TinhLoiNhuan(thang, nam);
//            decimal loiNhuanTruoc = db.TinhLoiNhuan(thangTruoc, namTruoc);

//            // Tránh chia cho 0
//            if (loiNhuanTruoc == 0)
//                return 0;

//            // Tính phần trăm thay đổi
//            decimal phanTram = ((loiNhuanHienTai - loiNhuanTruoc) / loiNhuanTruoc) * 100;

//            return Math.Round(phanTram, 2);
//        }
//    }
//}
