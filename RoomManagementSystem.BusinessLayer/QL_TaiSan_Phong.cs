using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RoomManagementSystem.DataLayer;

namespace RoomManagementSystem.BusinessLayer
{
    public class QL_TaiSan_Phong
    {
        NhaAccess nha=new NhaAccess();
        //Dang ki thong tin nha
        public Boolean DangKyThongTinNha(string MaNha, string DiaChi, int SoPhong, int TongSoPhongHienTai, string GhiChu)
        {
            return nha.registerHouse(MaNha, DiaChi, SoPhong, TongSoPhongHienTai, GhiChu);
        }
        //Cap nhat lai thong tin nha
        public Boolean UpdateNha(string MaNha, string DiaChi, int SoPhong, int TongSoPhongHienTai, string GhiChu)
        {
            return nha.updateHouse(MaNha,DiaChi,SoPhong, TongSoPhongHienTai,GhiChu);
        }


        //Tra ve danh sach nha hien co
        public List<Nha> DanhSachNha()
        {
            return nha.getAllHouse();
        }


        //Them thong tin phong 
        PhongDAL p =new PhongDAL();
        
        public Boolean ThemPhong(Phong a)
        {
            return p.InsertPhong(a);
        }
        
        //Tra ve danh sach phong hien co
        public List<Phong> DanhSachPhong()
        {
            return p.GetAllPhong();
        }

        //Cap nhat gia, trang thai,...
        public Boolean CapNhatPhong(Phong a)
        {
            if (string.IsNullOrEmpty(a.MaPhong))
            {
                throw new Exception("Mã phòng không được để trống");
            }
            return p.UpdatePhong(a);
        }
    }
}
