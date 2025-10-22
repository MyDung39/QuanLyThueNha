using RoomManagementSystem.BusinessLayer;
using RoomManagementSystem.DataLayer;
using System.Data;

namespace RoomManagementSystem.ConsoleTest
{
    public class Program
    {
        // Khởi tạo lớp Business Layer
        private static QL_BaoTri qlBaoTri = new QL_BaoTri();

        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.InputEncoding = System.Text.Encoding.UTF8;

            bool running = true;
            while (running)
            {
                Console.WriteLine("== QUẢN LÝ BẢO TRÌ ==");
                Console.WriteLine("1. Thêm mới yêu cầu bảo trì");
                Console.WriteLine("2. Cập nhật trạng thái xử lý");
                Console.WriteLine("3. Cập nhật chi phí");
                Console.WriteLine("4. Hiển thị tất cả yêu cầu");
                Console.WriteLine("5. Tìm yêu cầu theo Mã");
                Console.WriteLine("6. Xem báo cáo chi phí theo tháng");
                Console.WriteLine("0. Thoát");
                Console.Write("Chọn chức năng: ");

                string? choice = Console.ReadLine();

                try
                {
                    switch (choice)
                    {
                        case "1":
                            TestThemMoi();
                            break;
                        case "2":
                            TestCapNhatTrangThai();
                            break;
                        case "3":
                            TestCapNhatChiPhi();
                            break;
                        case "4":
                            TestHienThiTatCa();
                            break;
                        case "5":
                            TestLayTheoId();
                            break;
                        case "6":
                            TestBaoCao();
                            break;
                        case "0":
                            running = false;
                            break;
                        default:
                            Console.WriteLine("Lựa chọn không hợp lệ. Vui lòng chọn lại.");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    // In lỗi nghiệp vụ hoặc lỗi kỹ thuật ra màn hình
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"\nLỖI: {ex.Message}");
                    Console.ResetColor();
                }

                if (running)
                {
                    Console.WriteLine("\nNhấn phím bất kỳ để tiếp tục...");
                    Console.ReadKey();
                    Console.Clear(); // Xóa màn hình cho lần lặp mới
                }
            }

            Console.WriteLine("Đã thoát chương trình.");
        }

        // 1. Kiểm thử Thêm mới
        private static void TestThemMoi()
        {
            Console.WriteLine("\n--- 1. Thêm Mới Yêu Cầu ---");
            BaoTri bt = new BaoTri();

            Console.Write("Nhập Mã Bảo Trì (ví dụ: BT001): ");
            bt.MaBaoTri = Console.ReadLine();

            Console.Write("Nhập Mã Phòng (ví dụ: PHONG001): ");
            bt.MaPhong = Console.ReadLine();

            Console.Write("Nhập Mã Người Thuê (ví dụ: NT001, bỏ trống nếu là chủ nhà): ");
            bt.MaNguoiThue = Console.ReadLine();
            if (string.IsNullOrEmpty(bt.MaNguoiThue))
            {
                bt.MaNguoiThue = null;
            }

            Console.Write("Nguồn yêu cầu (Chủ nhà, Người thuê, Kiểm tra định kỳ): ");
            bt.NguonYeuCau = Console.ReadLine();

            Console.Write("Mô tả sự cố (ví dụ: Hỏng vòi nước): ");
            bt.MoTa = Console.ReadLine();

            bt.NgayYeuCau = DateTime.Now; // Lớp BL sẽ kiểm tra logic này
            // TrangThaiXuLy và ChiPhi sẽ được BL tự gán mặc định

            qlBaoTri.Insert(bt);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("=> Thêm mới yêu cầu bảo trì thành công!");
            Console.ResetColor();
        }

        // 2. Kiểm thử Cập nhật trạng thái
        private static void TestCapNhatTrangThai()
        {
            Console.WriteLine("\n--- 2. Cập Nhật Trạng Thái ---");
            Console.Write("Nhập Mã Bảo Trì cần cập nhật: ");
            string maBaoTri = Console.ReadLine() ?? "";

            Console.Write("Nhập Trạng Thái Mới (Chưa xử lý, Đang xử lý, Hoàn tất): ");
            string trangThai = Console.ReadLine() ?? "";

            qlBaoTri.UpdateTrangThai(maBaoTri, trangThai);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("=> Cập nhật trạng thái thành công!");
            Console.ResetColor();
        }

        // 3. Kiểm thử Cập nhật chi phí
        private static void TestCapNhatChiPhi()
        {
            Console.WriteLine("\n--- 3. Cập Nhật Chi Phí ---");
            Console.Write("Nhập Mã Bảo Trì cần cập nhật chi phí: ");
            string maBaoTri = Console.ReadLine() ?? "";

            Console.Write("Nhập Chi Phí Mới (ví dụ: 150000): ");
            decimal chiPhi;
            while (!decimal.TryParse(Console.ReadLine(), out chiPhi))
            {
                Console.Write("Chi phí không hợp lệ. Vui lòng nhập lại: ");
            }

            qlBaoTri.UpdateChiPhi(maBaoTri, chiPhi);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("=> Cập nhật chi phí thành công!");
            Console.ResetColor();
        }

        // 4. Kiểm thử Hiển thị tất cả
        private static void TestHienThiTatCa()
        {
            Console.WriteLine("\n--- 4. Danh Sách Tất Cả Yêu Cầu ---");
            var list = qlBaoTri.GetAll();

            if (list == null || list.Count == 0)
            {
                Console.WriteLine("Không có yêu cầu bảo trì nào.");
                return;
            }

            foreach (var bt in list)
            {
                PrintBaoTri(bt);
            }
        }

        // 5. Kiểm thử Lấy theo ID
        private static void TestLayTheoId()
        {
            Console.WriteLine("\n--- 5. Tìm Yêu Cầu Theo Mã ---");
            Console.Write("Nhập Mã Bảo Trì cần tìm: ");
            string maBaoTri = Console.ReadLine() ?? "";

            var bt = qlBaoTri.GetById(maBaoTri);

            if (bt == null)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"Không tìm thấy yêu cầu có mã: {maBaoTri}");
                Console.ResetColor();
            }
            else
            {
                PrintBaoTri(bt);
            }
        }

        // 6. Kiểm thử Báo cáo
        private static void TestBaoCao()
        {
            Console.WriteLine("\n--- 6. Báo Cáo Chi Phí (Đã Hoàn Tất) ---");
            Console.Write("Nhập Tháng (1-12): ");
            int thang;
            while (!int.TryParse(Console.ReadLine(), out thang))
            {
                Console.Write("Tháng không hợp lệ. Vui lòng nhập lại: ");
            }

            Console.Write("Nhập Năm (ví dụ: 2025): ");
            int nam;
            while (!int.TryParse(Console.ReadLine(), out nam))
            {
                Console.Write("Năm không hợp lệ. Vui lòng nhập lại: ");
            }

            DataTable dt = qlBaoTri.GetBaoCaoChiPhiThang(thang, nam);
            PrintDataTable(dt, $"Báo Cáo Chi Phí Bảo Trì Hoàn Tất - {thang}/{nam}");
        }


        // Hàm trợ giúp: In thông tin chi tiết
        private static void PrintBaoTri(BaoTri bt)
        {
            Console.WriteLine("----------------------------------------");
            Console.WriteLine($"Mã Bảo Trì:     {bt.MaBaoTri}");
            Console.WriteLine($"Mã Phòng:        {bt.MaPhong}");
            Console.WriteLine($"Người Yêu Cầu:   {bt.MaNguoiThue ?? "(Chủ nhà)"}");
            Console.WriteLine($"Nguồn:           {bt.NguonYeuCau}");
            Console.WriteLine($"Mô Tả:           {bt.MoTa}");
            Console.WriteLine($"Ngày Yêu Cầu:    {bt.NgayYeuCau.ToString("dd/MM/yyyy")}");
            Console.WriteLine($"Trạng Thái:      {bt.TrangThaiXuLy}");
            string ngayHTFormatted = bt.NgayHoanThanh.HasValue
                ? bt.NgayHoanThanh.Value.ToString("dd/MM/yyyy")
                : "Chưa hoàn thành";
            Console.WriteLine($"Ngày Hoàn Thành: {ngayHTFormatted}");
            Console.WriteLine($"Chi Phí:         {bt.ChiPhi:N0} VND");
            Console.WriteLine("----------------------------------------");
        }

        // Hàm trợ giúp: In DataTable
        private static void PrintDataTable(DataTable dt, string title)
        {
            Console.WriteLine($"\n--- {title} ---");
            if (dt.Rows.Count == 0)
            {
                Console.WriteLine("Không có dữ liệu.");
                return;
            }

            // In tiêu đề cột
            foreach (DataColumn col in dt.Columns)
            {
                Console.Write($"{col.ColumnName,-20} | ");
            }
            Console.WriteLine("\n" + new string('-', dt.Columns.Count * 23));

            // In dữ liệu hàng
            foreach (DataRow row in dt.Rows)
            {
                foreach (var item in row.ItemArray)
                {
                    Console.Write($"{item,-20} | ");
                }
                Console.WriteLine();
            }
        }
    }
}