using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RoomManagementSystem.DataLayer;

namespace RoomManagementSystem.BusinessLayer
{
    public class QuanLyNguoiThue
    {
        NguoiThueDAL nt=new NguoiThueDAL();
        // Nhập thông tin nguoi thue
        public bool ThemNguoiThue(NguoiThue a)
        {
            return nt.ThemNguoiThue(a); 
        }

        //Cập nhật tình trạng thuê (đang ở/đã trả phòng/lịch hẹn trả)
        public bool CapNhatNguoiThue(NguoiThue a)
        {
            return nt.CapNhatNguoiThue(a) ;
        }

        //Tra ve danh sach nguoi thue
        public List<NguoiThue> getAll()
        {
            return nt.getAllNguoiThue();
        }

        //Tra ve danh sach nguoi thue theo phong

        public List<NguoiThue> getByMaPhong(NguoiThue a)
        {
            return nt.getNguoiThueByPhong(a) ;
        }
    }
}
