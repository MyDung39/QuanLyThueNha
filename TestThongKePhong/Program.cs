using System;
using System.Data;
using RoomManagementSystem.BusinessLayer;

namespace RoomManagementSystem.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                // 1. Tạo instance ThongKeTinhTrangPhong
                ThongKeTinhTrangPhong tk = new ThongKeTinhTrangPhong();

                // 2. Hiển thị dữ liệu từng loại phòng ra console
                Console.WriteLine("=== Phòng trống ===");
                PrintDataTable(tk.GetPhongTrong());

                Console.WriteLine("\n=== Phòng đang thuê ===");
                PrintDataTable(tk.GetPhongDangThue());

                Console.WriteLine("\n=== Phòng sắp trống ===");
                PrintDataTable(tk.GetPhongSapTrong());

                Console.WriteLine("\n=== Phòng bảo trì ===");
                PrintDataTable(tk.GetPhongBaoTri());

                Console.WriteLine("\n=== Tỷ lệ lấp đầy ===");
                var tyLe = tk.GetTyLeLapDay();
                Console.WriteLine($"{tyLe} %");

                // 3. Xuất báo cáo Excel
                string testDir = Path.Combine(
                    Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.Parent.FullName,
                    "Test"
                );

                if (!Directory.Exists(testDir))
                {
                    Directory.CreateDirectory(testDir);
                }

                string filePath = Path.Combine(testDir, "ThongKePhong.xlsx");
                tk.XuatBaoCaoExcel(filePath);

                Console.WriteLine($"\nXuất báo cáo Excel thành công: {filePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi: " + ex.Message);
            }

            Console.WriteLine("\nNhấn phím bất kỳ để thoát...");
            Console.ReadKey();
        }

        // Hàm tiện ích in DataTable ra console
        static void PrintDataTable(DataTable table)
        {
            if (table.Rows.Count == 0)
            {
                Console.WriteLine("Không có dữ liệu.");
                return;
            }

            // In header
            foreach (DataColumn col in table.Columns)
            {
                Console.Write($"{col.ColumnName}\t");
            }
            Console.WriteLine();

            // In dữ liệu
            foreach (DataRow row in table.Rows)
            {
                foreach (var item in row.ItemArray)
                {
                    Console.Write($"{item}\t");
                }
                Console.WriteLine();
            }
        }
    }
}

