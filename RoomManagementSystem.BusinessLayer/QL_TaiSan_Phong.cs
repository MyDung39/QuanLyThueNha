using System;
using System.Collections.Generic;
using System.Linq;
using RoomManagementSystem.DataLayer;

namespace RoomManagementSystem.BusinessLayer
{
    public class QL_TaiSan_Phong
    {
        // Khởi tạo tất cả các lớp DAL cần thiết ở đây.
        private readonly NhaAccess nha = new NhaAccess();
        private readonly PhongDAL p = new PhongDAL();
        // ✅ 1. KHỞI TẠO BAOTRIDAL
        private readonly BaoTriDAL _baoTriDAL = new BaoTriDAL();
        private readonly HopDongDAL _hopDongDAL = new HopDongDAL();

        // --- QUẢN LÝ NHÀ (Giữ nguyên) ---

        public bool DangKyThongTinNha(string DiaChi, string GhiChu)
        {
            string newMaNha = nha.AutoMaNha();
            return nha.registerHouse(newMaNha, DiaChi, GhiChu);
        }

        public bool UpdateNha(string MaNha, string DiaChi, string GhiChu)
        {
            return nha.updateHouse(MaNha, DiaChi, GhiChu);
        }

        public bool XoaNha(string MaNha)
        {
            int soLuongPhong = p.GetRoomCountByHouse(MaNha);
            if (soLuongPhong > 0)
            {
                return false;
            }
            return nha.DeleteHouse(MaNha);
        }

        public List<Nha> DanhSachNha()
        {
            return nha.getAllHouse();
        }


        // --- QUẢN LÝ PHÒNG ---

        public bool ThemPhong(Phong a)
        {
            string newMaPhong = p.AutoMaPhong();
            a.MaPhong = newMaPhong;
            return p.InsertPhong(a);
        }

        // ✅ 2. SỬA LẠI HOÀN TOÀN PHƯƠNG THỨC NÀY ĐỂ THÊM CẢ 2 LOGIC
        /// <summary>
        /// Lấy danh sách phòng của một nhà và tổng hợp thông tin ngày bảo trì, ngày có sẵn.
        /// </summary>
        public List<Phong> DanhSachPhong(string manha)
        {
            // --- BƯỚC 1: LẤY TẤT CẢ DỮ LIỆU GỐC ---
            List<Phong> danhSachPhong = p.GetAllPhong(manha);

            if (danhSachPhong == null || !danhSachPhong.Any())
            {
                return new List<Phong>();
            }

            // Lấy dữ liệu từ các bảng liên quan trong MỘT lần gọi để tối ưu
            List<BaoTri> allMaintenanceRecords = _baoTriDAL.GetAll();
            List<HopDong> allContracts = _hopDongDAL.GetAllHopDong();

            // --- BƯỚC 2: TỐI ƯU HÓA TRUY XUẤT ---
            // Chuyển danh sách hợp đồng thành Dictionary để tra cứu nhanh theo MaPhong
            // Chỉ lấy các hợp đồng đang 'Hiệu lực'
            var activeContractsDict = allContracts
                .Where(c => c.TrangThai == "Hiệu lực")
                .GroupBy(c => c.MaPhong) // Nhóm theo mã phòng
                .ToDictionary(g => g.Key, g => g.OrderByDescending(c => c.NgayBatDau).First()); // Lấy hợp đồng mới nhất cho mỗi phòng

            // --- BƯỚC 3: XỬ LÝ VÀ LÀM GIÀU DỮ LIỆU ---
            foreach (var phong in danhSachPhong)
            {
                // --- Logic cho "Ngày bảo trì" (giữ nguyên) ---
                var latestPendingMaintenance = allMaintenanceRecords
                    .Where(bt => bt.MaPhong == phong.MaPhong &&
                                 (bt.TrangThaiXuLy == "Chưa xử lý" || bt.TrangThaiXuLy == "Đang xử lý"))
                    .OrderByDescending(bt => bt.NgayYeuCau)
                    .FirstOrDefault();

                phong.NgayBaoTriHienTai = latestPendingMaintenance?.NgayYeuCau;

                // --- Logic cho "Ngày có sẵn" ---
                // Chỉ xử lý nếu phòng đang ở trạng thái 'Đang thuê'
                if (phong.TrangThai == "Đang thuê")
                {
                    // Tra cứu trong Dictionary đã tạo
                    if (activeContractsDict.TryGetValue(phong.MaPhong, out HopDong contract))
                    {
                        // Nếu tìm thấy hợp đồng hiệu lực, ngày có sẵn là ngày kết thúc của hợp đồng đó
                        phong.NgayCoSan = contract.NgayKetThuc;
                    }
                    else
                    {
                        // Trường hợp lạ: Phòng "Đang thuê" nhưng không tìm thấy HĐ hiệu lực? -> Để trống
                        phong.NgayCoSan = null;
                    }
                }
                else if (phong.TrangThai == "Trống")
                {
                    // Nếu phòng trống, ngày có sẵn chính là ngày hôm nay.
                    phong.NgayCoSan = DateTime.Today;
                }
                else
                {
                    // Nếu phòng không ở trạng thái 'Đang thuê', nó đã có sẵn -> Để trống
                    phong.NgayCoSan = null;
                }
            }

            // --- BƯỚC 4: TRẢ VỀ KẾT QUẢ ---
            return danhSachPhong;
        }

        public bool CapNhatPhong(Phong a)
        {
            if (string.IsNullOrEmpty(a.MaPhong))
            {
                throw new Exception("Mã phòng không được để trống");
            }
            return p.UpdatePhong(a);
        }

        public bool XoaPhong(string MaPhong)
        {
            return p.DeletePhong(MaPhong);
        }



        public List<Phong> GetAvailableRooms()
        {
            // Giả sử 'p' là biến PhongDAL của bạn
            return p.GetAvailableRooms(); // Bạn cần tạo phương thức này trong PhongDAL
        }




    }
}