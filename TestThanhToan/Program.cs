using RoomManagementSystem.BusinessLayer;
using RoomManagementSystem.DataLayer;
using System;
using System.Data;

class Program
{
    static void Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        QuanLyThanhToan qlThanhToan = new QuanLyThanhToan();

        while (true)
        {
            Console.WriteLine("\n===== MENU TEST THANH TOÁN =====");
            Console.WriteLine("1. Thêm thanh toán mới");
            Console.WriteLine("2. Ghi nhận thanh toán (cập nhật công nợ)");
            Console.WriteLine("3. Xem danh sách thanh toán");
            Console.WriteLine("0. Thoát");
            Console.Write("Chọn: ");

            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    TestThemThanhToan(qlThanhToan);
                    break;

                case "2":
                    TestGhiNhanThanhToan(qlThanhToan);
                    break;

                case "3":
                    TestLayDanhSachThanhToan(qlThanhToan);
                    break;

                case "0":
                    return;

                default:
                    Console.WriteLine("❌ Lựa chọn không hợp lệ!");
                    break;
            }
        }
    }

    static void TestThemThanhToan(QuanLyThanhToan ql)
    {

        Console.Write("Nhập mã hóa đơn: ");
        string mahd = Console.ReadLine();


        Console.Write("Nhập tổng công nợ: ");
        decimal congno = decimal.Parse(Console.ReadLine());

        ThanhToan tt = new ThanhToan
        {
            MaHoaDon = mahd,
            TongCongNo = congno,
        };

        bool result = ql.ThemThanhToan(tt);

        Console.WriteLine(result ? "✅ Thêm thành công!" : "❌ Thêm thất bại!");
    }

    static void TestGhiNhanThanhToan(QuanLyThanhToan ql)
    {
        Console.Write("Nhập mã thanh toán: ");
        string ma = Console.ReadLine();

        Console.Write("Nhập số tiền thanh toán: ");
        decimal tien = decimal.Parse(Console.ReadLine());

        Console.Write("Nhập phương thức (Tiền mặt/Chuyển khoản): ");
        string phuongthuc = Console.ReadLine();

        Console.Write("Ghi chú: ");
        string ghichu = Console.ReadLine();

        bool result = ql.CapNhatThanhToan(ma, tien, phuongthuc, ghichu);

        Console.WriteLine(result ? "✅ Cập nhật thanh toán thành công!" : "❌ Cập nhật thất bại!");
    }

    static void TestLayDanhSachThanhToan(QuanLyThanhToan ql)
    {
        DataTable dt = ql.LayDanhSachThanhToan();
        Console.WriteLine("\n📋 DANH SÁCH THANH TOÁN:");

        if (dt.Rows.Count == 0)
        {
            Console.WriteLine("❌ Không có dữ liệu!");
            return;
        }

        foreach (DataRow row in dt.Rows)
        {
            Console.WriteLine($"Mã: {row["MaThanhToan"]} | Mã HĐ: {row["MaHoaDon"]} | Công nợ: {row["TongCongNo"]}");
        }
    }
}

