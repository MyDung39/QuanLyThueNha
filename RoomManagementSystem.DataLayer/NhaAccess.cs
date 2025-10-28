using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoomManagementSystem.DataLayer
{
    public class NhaAccess
    {
        string connect = "Data Source=LAPTOP-5FKFDEEM;Initial Catalog=QLTN;Integrated Security=True;Encrypt=True;Trust Server Certificate=True";
        //Them thong tin can nha
        public Boolean registerHouse(string MaNha, string DiaChi, int SoPhong, int TongSoPhongHienTai, string GhiChu)
        {
            //Them thong tin vao Table Nha trong csdl
            using (SqlConnection c = new SqlConnection(connect))
            {
                c.Open();
                string a = "SELECT MaNguoiDung FROM NguoiDung";
                SqlCommand b = new SqlCommand(a, c);
                var MaNguoiDung = b.ExecuteScalar();

                string qr = @"INSERT INTO Nha (MaNguoiDung,MaNha, DiaChi, TongSoPhong, TongSoPhongHienTai, GhiChu, NgayTao, NgayCapNhat) 
                          VALUES (@MaNguoiDung,@MaNha, @DiaChi, @TongSoPhong, @TongSoPhongHienTai, @GhiChu, GETDATE(), GETDATE())";
                SqlCommand q = new SqlCommand(qr, c);
                q.Parameters.AddWithValue("@MaNguoiDung", MaNguoiDung.ToString());
                q.Parameters.AddWithValue("@MaNha", MaNha);
                q.Parameters.AddWithValue("@DiaChi", DiaChi);
                q.Parameters.AddWithValue("@TongSoPhong", SoPhong);
                q.Parameters.AddWithValue("@TongSoPhongHienTai", TongSoPhongHienTai);
                q.Parameters.AddWithValue("@GhiChu", GhiChu);
                
                int result = q.ExecuteNonQuery();
                return result > 0; // Trả về true nếu thêm thành công 
            }
        }
        //Cap nhat thong tin can nha
        public Boolean updateHouse(string MaNha, string DiaChi, int SoPhong, int TongSoPhongHienTai, string GhiChu)
        {
            using (SqlConnection c = new SqlConnection(connect))
            {
                c.Open();
                string a = "SELECT MaNguoiDung FROM NguoiDung";
                SqlCommand b = new SqlCommand(a, c);
                var MaNguoiDung = b.ExecuteScalar();

                string qr = @"UPDATE Nha SET MaNguoiDung = @MaNguoiDung,DiaChi = @DiaChi,TongSoPhong = @TongSoPhong,TongSoPhongHienTai = @TongSoPhongHienTai,GhiChu = @GhiChu,NgayCapNhat = GETDATE()  WHERE MaNha = @MaNha";
                SqlCommand q = new SqlCommand(qr, c);
                q.Parameters.AddWithValue("@MaNguoiDung", MaNguoiDung.ToString());
                q.Parameters.AddWithValue("@MaNha", MaNha);
                q.Parameters.AddWithValue("@DiaChi", DiaChi);
                q.Parameters.AddWithValue("@TongSoPhong", SoPhong);
                q.Parameters.AddWithValue("@TongSoPhongHienTai", TongSoPhongHienTai);
                q.Parameters.AddWithValue("@GhiChu", GhiChu);
                int result = q.ExecuteNonQuery();
                return result > 0; // Trả về true nếu  thành công 
            }
        }
    }
}
