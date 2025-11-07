using System;
using System.Data;
using RoomManagementSystem.BusinessLayer;

namespace RoomManagementSystem.Test
{
    internal class TestBaoCaoCongNo
    {
        static void Main(string[] args)
        {
            BaoCaoCongNo bc = new BaoCaoCongNo();

            // ==== 1️⃣ LẤY DANH SÁCH CÔNG NỢ ====
            Console.WriteLine("=== DANH SÁCH CÔNG NỢ ===");
            DataTable dtCongNo = bc.LayBaoCaoCongNo();
            foreach (DataRow row in dtCongNo.Rows)
            {
                Console.WriteLine($"{row["HoTen"],-20} | {row["MaPhong"],-10} | Nợ còn lại: {row["SoTienConLai"],10} | Tháng nợ: {row["SoThangNo"],2}");
            }

            // ==== 2️⃣ LẤY LỊCH SỬ THANH TOÁN CỦA 1 KHÁCH ====
            if (dtCongNo.Rows.Count > 0)
            {
                string maNguoiThue = dtCongNo.Rows[0]["MaNguoiThue"].ToString();
                Console.WriteLine($"\n=== LỊCH SỬ THANH TOÁN CỦA KHÁCH: {maNguoiThue} ===");
                DataTable dtLichSu = bc.LayLichSuThanhToan(maNguoiThue);

                foreach (DataRow row in dtLichSu.Rows)
                {
                    Console.WriteLine($"{row["MaThanhToan"],-10} | Ngày: {row["NgayTao"],-15} | Đã trả: {row["SoTienDaThanhToan"],10} | Còn lại: {row["SoTienConLai"],10} | Trạng thái: {row["TrangThai"]}");
                }
            }

            // ==== 3️⃣ TÍNH TỔNG CÔNG NỢ ====
            decimal tongNo = bc.TongCongNoHeThong(dtCongNo);
            Console.WriteLine($"\n>>> TỔNG CÔNG NỢ TOÀN HỆ THỐNG: {tongNo:N0} VNĐ\n");

            // ==== 4️⃣ XUẤT EXCEL ====
            try
            {
                string filePath = @"D:\BaoCaoCongNo.xlsx";
                if (bc.ExportCongNoToExcel(dtCongNo, filePath))
                {
                    Console.WriteLine($"✅ Đã xuất báo cáo công nợ ra file: {filePath}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Lỗi xuất Excel: " + ex.Message);
            }

            Console.WriteLine("\nHoàn tất kiểm thử.");
            Console.ReadLine();
        }
    }
}
