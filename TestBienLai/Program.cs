using RoomManagementSystem.BusinessLayer;
using System;
using System.Data;
using System.IO;

namespace ThanhToanExcel
{
    class Program
    {
        static void Main(string[] args)
        {
            QlThanhToan qltt = new QlThanhToan();

            try
            {
                // Lấy dữ liệu biên lai từ DAL thông qua BLL
                DataTable dtBienLai = qltt.BienLai("001"); // ví dụ MTT001 là mã thanh toán

                if (dtBienLai == null || dtBienLai.Rows.Count == 0)
                {
                    Console.WriteLine("Không có dữ liệu biên lai!");
                    return;
                }

                // Đường dẫn lưu file trong thư mục Test
                string testDir = Path.Combine(
                    Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.Parent.FullName,
                    "Test"
                );

                if (!Directory.Exists(testDir))
                {
                    Directory.CreateDirectory(testDir);
                }

                string filePath = Path.Combine(testDir, "BienLai_MTT001.xlsx");

                // Xuất Excel
                qltt.ExportBienLaiExcel(filePath, dtBienLai);

                Console.WriteLine("Xuất biên lai thành công: " + filePath);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi: " + ex.Message);
            }
        }
    }
}

