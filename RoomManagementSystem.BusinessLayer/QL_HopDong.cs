using RoomManagementSystem.DataLayer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Globalization;
using GrapeCity.Documents.Word;
using Spire.Doc;
using Spire.Doc.Documents;


namespace RoomManagementSystem.BusinessLayer
{
    public class QL_HopDong
    {
        HopDongDAL hdDAL = new HopDongDAL();

        // Thêm hợp đồng
        public bool ThemHopDong(HopDong hd)
        {
            if (string.IsNullOrEmpty(hd.MaHopDong) || string.IsNullOrEmpty(hd.MaPhong) || string.IsNullOrEmpty(hd.MaNguoiThue))
            {
                throw new Exception("Mã hợp đồng, phòng hoặc người thuê không được để trống");
            }

            // [Mới] Áp dụng điều kiện: Chỉ 'Chủ hợp đồng' mới được tạo hợp đồng
            if (!hdDAL.IsChuHopDong(hd.MaNguoiThue))
            {
                throw new Exception("Thêm hợp đồng thất bại: Người thuê không phải là 'Chủ hợp đồng'.");
            }

            return hdDAL.InsertHopDong(hd);
        }

        // Tra ve danh sach hop dong hien co
        public List<HopDong> DanhSachHopDong()
        {
            return hdDAL.GetAllHopDong();
        }

        // Cap nhat hop dong (trang thai, ngay ket thuc,...)
        public bool CapNhatHopDong(HopDong hd)
        {
            if (string.IsNullOrEmpty(hd.MaHopDong))
            {
                throw new Exception("Mã hợp đồng không được để trống");
            }
            return hdDAL.UpdateHopDong(hd);
        }

        // Xoa hop dong
        public bool XoaHopDong(string maHopDong)
        {
            if (string.IsNullOrEmpty(maHopDong))
            {
                throw new Exception("Mã hợp đồng không được để trống");
            }
            return hdDAL.DeleteHopDong(maHopDong);
        }

        // Xuat hop dong ra file PDF
        public bool XuatHopDongRaPdf(string maHopDong, string outputPath)
        {
            // 1. Lấy dữ liệu chi tiết của hợp đồng
            HopDongXemIn? data = hdDAL.GetInHD(maHopDong);
            if (data == null)
            {
                Console.WriteLine("Lỗi: Không tìm thấy thông tin hợp đồng để xuất file.");
                return false;
            }

            // Lấy tên tệp mẫu từ dữ liệu và xây dựng đường dẫn động
            if (string.IsNullOrEmpty(data.FileDinhKem))
            {
                Console.WriteLine("Lỗi: Hợp đồng này không có tệp mẫu được định nghĩa trong CSDL.");
                return false;
            }

            string templateFileName = data.FileDinhKem;
            // Lấy đường dẫn thư mục gốc của ứng dụng (ví dụ: .../bin/Debug/netX.X)
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            // Kết hợp để tạo đường dẫn đầy đủ đến tệp mẫu trong thư mục Templates
            string templatePath = Path.Combine(baseDirectory, "Templates", templateFileName);


            if (!File.Exists(templatePath))
            {
                Console.WriteLine($"Lỗi: File mẫu không tồn tại tại đường dẫn: {templatePath}");
                return false;
            }

            try
            {
                // 2. Tải file Word mẫu bằng Spire.Doc
                Document document = new Document();
                document.LoadFromFile(templatePath);

                // 3. Tạo một Dictionary chứa các placeholder và giá trị tương ứng
                var culture = new CultureInfo("vi-VN");
                Dictionary<string, string> replacements = new Dictionary<string, string>
                {
                    { "{{NgayBatDau}}", $"{data.NgayBatDau.Day:00}" },
                    { "{{ThangBatDau}}", $"{data.NgayBatDau.Month:00}" },
                    { "{{NamBatDau}}", data.NgayBatDau.Year.ToString() },
                    { "{{TenBenB}}", data.TenNguoiThue ?? "Không có dữ liệu" },
                    { "{{CccdBenB}}", data.CccdNguoiThue ?? "Không có dữ liệu" },
                    { "{{TongSoPhong}}", data.TongSoPhong.ToString() },
                    { "{{DiaChi}}", data.DiaChiNha ?? "Không có dữ liệu" },
                    { "{{NgayDonVao}}", $"{data.NgayDonVao.Day:00}" },
                    { "{{ThangDonVao}}", $"{data.NgayDonVao.Month:00}" },
                    { "{{NamDonVao}}", data.NgayDonVao.Year.ToString() },
                    { "{{TienCoc}}", data.TienCoc.ToString("N0", culture) },
                    { "{{GiaThue}}", data.GiaThue.ToString("N0", culture) },
                    { "{{DienTich}}", data.DienTich.ToString("N2", culture) },
                    { "{{ThoiHan}}", $"{data.ThoiHan} tháng" }
                };

                // Thay thế tất cả các placeholder trong văn bản
                foreach (var item in replacements)
                {
                    document.Replace(item.Key, item.Value, false, true);
                }

                // 4. Lưu tài liệu ra file PDF (Spire.Doc có phương thức riêng cho việc này)
                document.SaveToFile(outputPath, FileFormat.PDF);

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Đã xảy ra lỗi khi xuất file PDF: {ex.Message}");
                return false;
            }
        }

        // Tao thong bao het han neu sap het han (logic: < 30 ngay tu ngay hien tai)
        public bool TaoThongBaoHetHan(string maHopDong)
        {
            HopDong? hd = hdDAL.GetHopDongById(maHopDong);
            if (hd == null)
            {
                throw new Exception("Hợp đồng không tồn tại");
            }

            DateTime ngayHienTai = DateTime.Now;
            // Kiểm tra xem hợp đồng có ngày kết thúc không và có sắp hết hạn không
            if (hd.NgayKetThuc != default(DateTime) && (hd.NgayKetThuc - ngayHienTai).TotalDays < 30 && (hd.NgayKetThuc - ngayHienTai).TotalDays > 0)
            {
                string maThongBao = "TB" + Guid.NewGuid().ToString().Substring(0, 8);
                string noiDung = $"Hợp đồng {maHopDong} sắp hết hạn vào {hd.NgayKetThuc:dd/MM/yyyy}";
                DateTime ngayThongBao = ngayHienTai;
                string trangThai = "Chưa thông báo";

                return hdDAL.InsertThongBaoHan(maThongBao, maHopDong, noiDung, ngayThongBao, trangThai);
            }
            return false;
        }
    }
}