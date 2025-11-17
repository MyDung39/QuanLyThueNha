using System;
using RoomManagementSystem.DataLayer;

namespace RoomManagementSystem.BusinessLayer
{
    public class ServiceManager
    {
        private readonly PhongDAL phongDal = new PhongDAL();
        private readonly HoaDonDAL hoaDonDal = new HoaDonDAL();
        private readonly ChiTietHoaDonDAL chiTietDal = new ChiTietHoaDonDAL();
        private readonly ThanhToanDAL thanhToanDal = new ThanhToanDAL();
        private readonly BaoTriDAL baoTriDal = new BaoTriDAL();

        // Khai báo mã dịch vụ
        private const string DV_DIEN = "DV1";
        private const string DV_NUOC = "DV2";
        private const string DV_INTERNET = "DV3";
        private const string DV_RAC = "DV4";
        private const string DV_GUI_XE = "DV5";
        private const string DV_TRE_HAN = "DV6";
        private const string DV_MAY_GIAT = "DV7";
        private const string DV_BAO_TRI = "DV8";

        // 1. Hàm dành riêng cho View Điện
        public void SaveElectric(string maPhong, string thoiKy, decimal tieuThu, decimal donGia)
        {
            if (string.IsNullOrWhiteSpace(maPhong) || string.IsNullOrWhiteSpace(thoiKy)) return;

            string maHoaDon = hoaDonDal.GetOrCreateByPhongThoiKy(maPhong, thoiKy);

            // Lưu vào chi tiết hóa đơn: Số lượng = Tiêu thụ (kWh), Đơn giá = Giá nhập
            if (tieuThu >= 0 && donGia > 0)
            {
                chiTietDal.UpsertByMaHoaDonMaDichVu(maHoaDon, DV_DIEN, tieuThu, "kWh", donGia);
            }
        }

        // 2. Hàm dành riêng cho View Nước
        public void SaveWater(string maPhong, string thoiKy, decimal tieuThu, decimal donGia)
        {
            if (string.IsNullOrWhiteSpace(maPhong) || string.IsNullOrWhiteSpace(thoiKy)) return;

            string maHoaDon = hoaDonDal.GetOrCreateByPhongThoiKy(maPhong, thoiKy);

            // Lưu vào chi tiết hóa đơn: Số lượng = Tiêu thụ (m3), Đơn giá = Giá nhập
            if (tieuThu >= 0 && donGia > 0)
            {
                chiTietDal.UpsertByMaHoaDonMaDichVu(maHoaDon, DV_NUOC, tieuThu, "m³", donGia);
            }
        }

        // 3. Hàm dành cho View Dịch vụ khác
        public void SaveOtherServices(string maPhong, string thoiKy,
            decimal? internet, decimal? rac, decimal? guiXe, decimal? mayGiat, decimal? treHan)
        {
            if (string.IsNullOrWhiteSpace(maPhong) || string.IsNullOrWhiteSpace(thoiKy)) return;

            string maHoaDon = hoaDonDal.GetOrCreateByPhongThoiKy(maPhong, thoiKy);

            // 1. Internet
            if (internet.HasValue && internet.Value > 0)
                chiTietDal.UpsertByMaHoaDonMaDichVu(maHoaDon, DV_INTERNET, 1, "tháng", internet.Value);

            // 2. Rác
            if (rac.HasValue && rac.Value > 0)
                chiTietDal.UpsertByMaHoaDonMaDichVu(maHoaDon, DV_RAC, 1, "phòng/tháng", rac.Value);

            // 3. Gửi xe
            if (guiXe.HasValue && guiXe.Value > 0)
                chiTietDal.UpsertByMaHoaDonMaDichVu(maHoaDon, DV_GUI_XE, 1, "tháng", guiXe.Value);

            // 4. Máy giặt: Tính 35k/người/phòng
            if (mayGiat.HasValue && mayGiat.Value > 0)
            {
                // Gọi hàm tính số người từ PhongDAL
                int soNguoi = phongDal.GetSoNguoiHienTai(maPhong);
                if (soNguoi > 0)
                {
                    decimal donGiaMayGiat = 35000; // Cố định 35k
                    chiTietDal.UpsertByMaHoaDonMaDichVu(maHoaDon, DV_MAY_GIAT, soNguoi, "người/phòng", donGiaMayGiat);
                }
            }

            // 5. Phí trễ hạn
            if (treHan.HasValue && treHan.Value > 0)
            {
                // Lưu vào bảng ChiTietHoaDon để Query lấy lên được (hiển thị dòng "Phí trễ hạn")
                chiTietDal.UpsertByMaHoaDonMaDichVu(maHoaDon, DV_TRE_HAN, 1, "lần/tháng", treHan.Value);

                // Đồng thời cập nhật vào bảng ThanhToan để tính tổng công nợ
                thanhToanDal.CapNhatCongNoTheoHoaDon(maHoaDon, treHan.Value);
            }

            // 6. Phí bảo trì
            if (DateTime.TryParseExact(thoiKy, "MM/yyyy", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out DateTime dtKy))
            {
                // Lấy tổng phí bảo trì trong tháng đó
                var thongTinBaoTri = baoTriDal.GetBaoTriInfoByMonth(maPhong, dtKy.Month, dtKy.Year);

                if (thongTinBaoTri.TongChiPhi > 0)
                {
                    // Lưu vào bảng ChiTietHoaDon. 
                    // Số lượng = 1, Đơn giá = Tổng chi phí.
                    // Lưu ý: Mô tả chi tiết (sửa cái gì) sẽ được xử lý khi Hiển thị/Xuất biên lai
                    chiTietDal.UpsertByMaHoaDonMaDichVu(maHoaDon, DV_BAO_TRI, 1, "lần", thongTinBaoTri.TongChiPhi);
                }
            }
        }
    }
}