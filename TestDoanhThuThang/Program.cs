using System;
using System.Data;
using System.IO;
using RoomManagementSystem.BusinessLayer;

namespace RoomManagementSystem.Test
{
    class TestExportExcel
    {
        static void Main(string[] args)
        {
            try
            {
                QuanLyDoanhThuThang qlDT = new QuanLyDoanhThuThang();

                string thang = "10";
                string nam = "2025";

                DataTable dtBaoCao = qlDT.LayBaoCaoThang(thang+"/"+nam);

                if (dtBaoCao.Rows.Count == 0)
                {
                    Console.WriteLine("Không có dữ liệu cho tháng/năm này.");
                    return;
                }

                // Thư mục lưu file
                string testDir = Path.Combine(
                    Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.Parent.FullName,
                    "Test"
                );

                if (!Directory.Exists(testDir))
                    Directory.CreateDirectory(testDir);

                string filePath = Path.Combine(testDir, $"BaoCaoDoanhThu_{thang}_{nam}.xlsx");

                // Xuất Excel
                qlDT.ExportToExcel(dtBaoCao, filePath);

                Console.WriteLine("Xuất báo cáo Excel thành công: " + filePath);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi: " + ex.Message);
            }

            Console.WriteLine("\nNhấn Enter để kết thúc...");
            Console.ReadLine();
        }
    }
}

