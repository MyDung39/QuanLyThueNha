using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using RoomManagementSystem.DataLayer;

namespace RoomManagementSystem.BusinessLayer
{
    public class QL_BaoTri
    {
        BaoTriDAL dal = new BaoTriDAL();
        public List<string> ValidTrangThai { get; } = new List<string> { "Chưa xử lý", "Đang xử lý", "Hoàn tất" };

        // Lấy tất cả yêu cầu bảo trì
        public List<BaoTri> GetAll()
        {
            // Thay đổi ở DAL và Model đã tự động thêm TenNguoiThue vào đây
            return dal.GetAll();
        }

        // Lấy 1 yêu cầu theo Mã
        public BaoTri? GetById(string maBaoTri)
        {
            // Thay đổi ở DAL và Model đã tự động thêm TenNguoiThue vào đây
            return dal.GetById(maBaoTri);
        }

        // Thêm mới yêu cầu bảo trì
        public void Insert(BaoTri baoTri)
        {
            // Kiểm tra nghiệp vụ
            if (string.IsNullOrEmpty(baoTri.MaPhong))
                throw new Exception("Mã phòng không được để trống!");

            if (string.IsNullOrEmpty(baoTri.MoTa))
                throw new Exception("Mô tả sự cố không được để trống!");

            if (baoTri.NgayYeuCau > DateTime.Now)
                throw new Exception("Ngày yêu cầu không hợp lệ!");

            // Mặc định trạng thái khi tạo mới
            baoTri.TrangThaiXuLy = "Chưa xử lý";
            baoTri.ChiPhi = 0; // Chi phí ban đầu là 0

            string newMaBT = dal.AutoMaBT();
            baoTri.MaBaoTri = newMaBT;

            dal.Insert(baoTri);
        }

        // Cập nhật trạng thái xử lý 
        public void UpdateTrangThai(string maBaoTri, string trangThai)
        {
            if (string.IsNullOrEmpty(maBaoTri))
                throw new Exception("Mã bảo trì không được để trống!");

            if (string.IsNullOrEmpty(trangThai) || !ValidTrangThai.Contains(trangThai))
                throw new Exception($"Trạng thái xử lý không hợp lệ! Phải là một trong: {string.Join(", ", ValidTrangThai)}");

            BaoTri? existing = dal.GetById(maBaoTri);
            if (existing == null)
                throw new Exception("Không tìm thấy yêu cầu bảo trì!");

            DateTime? ngayHoanThanh = existing.NgayHoanThanh;

            // Nếu chuyển sang "Hoàn tất" và chưa có ngày hoàn thành, set ngày là hôm nay
            if (trangThai == "Hoàn tất" && !ngayHoanThanh.HasValue)
            {
                ngayHoanThanh = DateTime.Now;
            }
            // Nếu chuyển từ "Hoàn tất" về trạng thái khác, xóa ngày hoàn thành
            else if (trangThai != "Hoàn tất")
            {
                ngayHoanThanh = null;
            }

            dal.UpdateTrangThai(maBaoTri, trangThai, ngayHoanThanh);
        }

        // Cập nhật chi phí
        public void UpdateChiPhi(string maBaoTri, decimal chiPhi)
        {
            if (string.IsNullOrEmpty(maBaoTri))
                throw new Exception("Mã bảo trì không được để trống!");

            if (chiPhi < 0)
                throw new Exception("Chi phí không được là số âm!");

            BaoTri? existing = dal.GetById(maBaoTri);
            if (existing == null)
                throw new Exception("Không tìm thấy yêu cầu bảo trì!");

            // Cập nhật chi phí
            dal.UpdateChiPhi(maBaoTri, chiPhi);
        }

        // Lấy báo cáo chi phí 
        public DataTable GetBaoCaoChiPhiThang(int thang, int nam)
        {
            if (thang < 1 || thang > 12)
                throw new Exception("Tháng không hợp lệ!");
            if (nam < 2000 || nam > DateTime.Now.Year + 5)
                throw new Exception("Năm không hợp lệ!");

            return dal.GetBaoCaoChiPhiThang(thang, nam);
        }

        public Dictionary<string, string> GetNguoiThueDangOByPhong(string maPhong)
        {
            if (string.IsNullOrEmpty(maPhong))
            {
                throw new Exception("Mã phòng không được để trống!");
            }

            // Gọi xuống DAL
            DataTable dt = dal.GetNguoiThueByPhong(maPhong);

            // Chuyển đổi DataTable thành Dictionary để UI dễ sử dụng
            Dictionary<string, string> tenants = new Dictionary<string, string>();
            foreach (DataRow row in dt.Rows)
            {
                tenants[row["MaNguoiThue"].ToString()] = row["HoTen"].ToString();
            }

            return tenants;
        }
    }
}