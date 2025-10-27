using RoomManagementSystem.BusinessLayer;
using System;
using System.IO;

namespace TestBaoCaoChiPhi
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            try
            {
                Console.Write("Nhập thời kỳ (MM/yyyy) cần xuất báo cáo: ");
                string thoiKy = Console.ReadLine();

                BaoCaoChiPhiBLL bcBLL = new BaoCaoChiPhiBLL();

                string fileName = $"BaoCaoChiPhi_{thoiKy.Replace("/", "_")}.xlsx";
                string filePath = Path.Combine(Directory.GetCurrentDirectory(), fileName);

                bool result = bcBLL.ExportExcel(thoiKy, filePath);

                if (result)
                {
                    Console.WriteLine("\n✅ Xuất báo cáo thành công!");
                    Console.WriteLine($"📁 File Excel được tạo tại: {filePath}");
                }
                else
                {
                    Console.WriteLine("\n❌ Không có dữ liệu để xuất báo cáo!");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("\n❌ Đã xảy ra lỗi: " + ex.Message);
            }

            Console.WriteLine("\nNhấn phím bất kỳ để thoát...");
            Console.ReadKey();
        }
    }
}
