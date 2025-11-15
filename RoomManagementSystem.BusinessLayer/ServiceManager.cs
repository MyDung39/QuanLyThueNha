using System;
using RoomManagementSystem.DataLayer;

namespace RoomManagementSystem.BusinessLayer
{
    public class ServiceManager
    {
        private readonly HoaDonDAL hoaDonDal = new HoaDonDAL();
        private readonly ChiTietHoaDonDAL chiTietDal = new ChiTietHoaDonDAL();
        private readonly ThanhToanDAL thanhToanDal = new ThanhToanDAL();
        private readonly BaoTriDAL baoTriDal = new BaoTriDAL();



        private readonly ChiSoDienDAL chiSoDienDal = new ChiSoDienDAL();
        private readonly ChiSoNuocDAL chiSoNuocDal = new ChiSoNuocDAL();


        // Mã dịch vụ chuẩn theo hệ thống hiện tại
        private const string DV_DIEN = "DV1";
        private const string DV_NUOC = "DV2";
        private const string DV_INTERNET = "DV3";
        private const string DV_RAC = "DV4";
        private const string DV_GUI_XE = "DV5";


        /*
        public void SaveServiceCosts(
            string maPhong,
            string thoiKy, // MM/yyyy
            decimal? dien,
            decimal? nuoc,
            decimal? internet,
            decimal? rac,
            decimal? guiXe,
            decimal? baoTri,
            decimal? treHan)
        {
            if (string.IsNullOrWhiteSpace(maPhong)) throw new ArgumentException("MaPhong không được trống");
            if (string.IsNullOrWhiteSpace(thoiKy)) throw new ArgumentException("Thời kỳ không được trống");

            string maHoaDon = hoaDonDal.GetOrCreateByPhongThoiKy(maPhong, thoiKy);

            // Upsert chi tiết hóa đơn cho từng dịch vụ DV1..DV4
            
          //  if (dien.HasValue && dien.Value > 0)
           //     chiTietDal.UpsertByMaHoaDonMaDichVu(maHoaDon, DV_DIEN, 1, "tháng", dien.Value);
       //     if (nuoc.HasValue && nuoc.Value > 0)
          //      chiTietDal.UpsertByMaHoaDonMaDichVu(maHoaDon, DV_NUOC, 1, "tháng", nuoc.Value);
            
            if (dien.HasValue && dien.Value > 0)
            {
                var chiSo = chiSoDienDal.GetByMaPhongThoiKy(maPhong, thoiKy);
                if (chiSo != null && chiSo.ChiSoThangTruoc.HasValue && chiSo.ChiSoThangNay.HasValue)
                {
                    var thanhTien = (chiSo.ChiSoThangNay.Value - chiSo.ChiSoThangTruoc.Value) * dien.Value;
                    chiTietDal.UpsertByMaHoaDonMaDichVu(maHoaDon, DV_DIEN, 1, "tháng", thanhTien);
                }
            }

            if (nuoc.HasValue && nuoc.Value > 0)
            {
                var chiSo = chiSoNuocDal.GetByMaPhongThoiKy(maPhong, thoiKy);
                if (chiSo != null && chiSo.ChiSoThangTruoc.HasValue && chiSo.ChiSoThangNay.HasValue)
                {
                    var thanhTien = (chiSo.ChiSoThangNay.Value - chiSo.ChiSoThangTruoc.Value) * nuoc.Value;
                    chiTietDal.UpsertByMaHoaDonMaDichVu(maHoaDon, DV_NUOC, 1, "tháng", thanhTien);
                }
            }

            if (internet.HasValue && internet.Value > 0)
                chiTietDal.UpsertByMaHoaDonMaDichVu(maHoaDon, DV_INTERNET, 1, "tháng", internet.Value);
            if (rac.HasValue && rac.Value > 0)
                chiTietDal.UpsertByMaHoaDonMaDichVu(maHoaDon, DV_RAC, 1, "tháng", rac.Value);
            if (guiXe.HasValue && guiXe.Value > 0)
                chiTietDal.UpsertByMaHoaDonMaDichVu(maHoaDon, DV_GUI_XE, 1, "xe/tháng", guiXe.Value);

            // Bảo trì: ghi nhận vào bảng BaoTri để khớp báo cáo hiện có
            if (baoTri.HasValue && baoTri.Value > 0)
            {
                var thoiKyDate = ParseThoiKyToDate(thoiKy);
                baoTriDal.InsertChiPhiBaoTri(maPhong, thoiKyDate, baoTri.Value);
            }

            // Trễ hạn: cộng vào Tổng công nợ của ThanhToan theo MaHoaDon
            if (treHan.HasValue && treHan.Value > 0)
            {
                thanhToanDal.CapNhatCongNoTheoHoaDon(maHoaDon, treHan.Value);
            }
        }
        */


        public void SaveServiceCosts(string maPhong, string thoiKy,
    decimal? dien, decimal? nuoc, decimal? internet, decimal? rac,
    decimal? guiXe, decimal? baoTri, decimal? treHan)
        {
            if (string.IsNullOrWhiteSpace(maPhong)) throw new ArgumentException("MaPhong không được trống");
            if (string.IsNullOrWhiteSpace(thoiKy)) throw new ArgumentException("Thời kỳ không được trống");

            string maHoaDon = hoaDonDal.GetOrCreateByPhongThoiKy(maPhong, thoiKy);

            // ----- Điện -----
            var chiSoDien = dien.HasValue && dien.Value > 0 ? new ChiSoDien
            {
                MaDichVu = maPhong,
                ChiSoThangNay = dien.Value
            } : chiSoDienDal.GetByMaPhongThoiKy(maPhong, thoiKy);

            if (chiSoDien != null && chiSoDien.ChiSoThangTruoc.HasValue && chiSoDien.ChiSoThangNay.HasValue)
            {
                decimal thanhTienDien = (chiSoDien.ChiSoThangNay.Value - chiSoDien.ChiSoThangTruoc.Value) * (chiSoDien.DonGia ?? 0);
                chiTietDal.UpsertByMaHoaDonMaDichVu(maHoaDon, DV_DIEN, 1, "tháng", thanhTienDien);
            }

            // ----- Nước -----
            var chiSoNuoc = nuoc.HasValue && nuoc.Value > 0 ? new ChiSoNuoc
            {
                MaDichVu = maPhong,
                ChiSoThangNay = nuoc.Value
            } : chiSoNuocDal.GetByMaPhongThoiKy(maPhong, thoiKy);

            if (chiSoNuoc != null && chiSoNuoc.ChiSoThangTruoc.HasValue && chiSoNuoc.ChiSoThangNay.HasValue)
            {
                decimal thanhTienNuoc = (chiSoNuoc.ChiSoThangNay.Value - chiSoNuoc.ChiSoThangTruoc.Value) * 10000; // Giả sử 1 đơn giá nước cố định 10k
                chiTietDal.UpsertByMaHoaDonMaDichVu(maHoaDon, DV_NUOC, 1, "m³", thanhTienNuoc);
            }

            // ----- Internet -----
            if (internet.HasValue && internet.Value > 0)
                chiTietDal.UpsertByMaHoaDonMaDichVu(maHoaDon, DV_INTERNET, 1, "tháng", internet.Value);

            // ----- Rác -----
            if (rac.HasValue && rac.Value > 0)
                chiTietDal.UpsertByMaHoaDonMaDichVu(maHoaDon, DV_RAC, 1, "tháng", rac.Value);

            // ----- Gửi xe -----
            if (guiXe.HasValue && guiXe.Value > 0)
                chiTietDal.UpsertByMaHoaDonMaDichVu(maHoaDon, DV_GUI_XE, 1, "xe/tháng", guiXe.Value);

            // ----- Bảo trì -----
            if (baoTri.HasValue && baoTri.Value > 0)
            {
                var thoiKyDate = ParseThoiKyToDate(thoiKy);
                baoTriDal.InsertChiPhiBaoTri(maPhong, thoiKyDate, baoTri.Value);
            }

            // ----- Trễ hạn -----
            if (treHan.HasValue && treHan.Value > 0)
            {
                thanhToanDal.CapNhatCongNoTheoHoaDon(maHoaDon, treHan.Value);
            }
        }






        private static DateTime ParseThoiKyToDate(string thoiKy)
        {
            // Kỳ dạng MM/yyyy -> chọn ngày 01
            if (DateTime.TryParseExact("01/" + thoiKy, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out var d))
                return d;
            // fallback: now
            return new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
        }
    }
}
