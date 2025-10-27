using RoomManagementSystem.BusinessLayer;
using RoomManagementSystem.DataLayer;
using System;
using System.Collections.Generic;

namespace TestExportBienLai
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            try
            {
                Console.Write("Nhập mã thanh toán (ví dụ: TT001): ");
                string maThanhToan = Console.ReadLine();

                XuatBienLai blBus = new XuatBienLai();
                List<BienLai> list = blBus.GetBienLai(maThanhToan);

                if (list == null || list.Count == 0)
                {
                    Console.WriteLine("❌ Không có dữ liệu biên lai!");
                    return;
                }

                string filePath = $"BienLai_{maThanhToan}.xlsx";
                blBus.XuatBienLaiExcel(list, filePath);

                Console.WriteLine("✅ Xuất biên lai thành công! Kiểm tra file trong bin/debug/net8 của thư mục test hiện tại");
                Console.WriteLine($"📁 File: {filePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Lỗi: " + ex.Message);
            }

            Console.WriteLine("Ấn phím bất kỳ để kết thúc...");
            Console.ReadKey();
        }
    }
}
