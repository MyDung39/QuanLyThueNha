using System;
using System.Data;
using System.IO;
using RoomManagementSystem.BusinessLayer;

namespace TestBaoCaoCongNo
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            try
            {
                Console.WriteLine("=== XUẤT BÁO CÁO CÔNG NỢ ===");

                BaoCaoCongNo bll = new BaoCaoCongNo();

                // Lấy dữ liệu công nợ
                DataTable dt = bll.LayBaoCaoCongNo();

                if (dt.Rows.Count == 0)
                {
                    Console.WriteLine("❌ Không có dữ liệu công nợ!");
                    return;
                }

                // Chọn tên file xuất
                string fileName = $"BaoCaoCongNo_{DateTime.Now:yyyyMMdd_HHmm}.xlsx";
                string filePath = Path.Combine(Directory.GetCurrentDirectory(), fileName);

                // Xuất Excel
                bool result = bll.ExportCongNoToExcel(dt, filePath);

                if (result)
                {
                    Console.WriteLine("\n✅ Xuất báo cáo thành công!");
                    Console.WriteLine($"📂 File Excel: {filePath}");
                }
                else
                {
                    Console.WriteLine("❌ Xuất báo cáo thất bại!");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("\n⚠️ Lỗi: " + ex.Message);
            }

            Console.WriteLine("\nNhấn phím bất kỳ để thoát...");
            Console.ReadKey();
        }
    }
}
