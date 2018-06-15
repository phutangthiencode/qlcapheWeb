using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Services;
using qlCaPhe.App_Start;
using qlCaPhe.Models.Entities;
using qlCaPhe.Models.Business;
using qlCaPhe.Models;
using System.Xml.Serialization;
using qlCaPhe.Models.Entities.Services;

/// <summary>
/// Summary description for bNhanVien
/// </summary>
[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]

[SerializableAttribute]
// To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
// [System.Web.Script.Services.ScriptService]
public class bNhanVien : System.Web.Services.WebService
{
    [WebMethod]
    /// <summary>
    /// Hàm tạo service đăng nhập vào hệ thống
    /// </summary>
    /// <param name="user">Tên tài khoản</param>
    /// <param name="pass">Mật khẩu (Đã mã hóa)</param>
    /// <returns>1: Thành công, 2: Thất bại</returns>
    public int Login(string user, string pass)
    {
        int kq = 0;
        try
        {
            kq = bProfile.kiemTraDangNhap(user, pass);
        }
        catch (Exception ex)
        {
            xulyFile.ghiLoi("Class: bNhanVien - Function: Login", ex.Message);
        }
        return kq;
    }
    /// <summary>
    /// Hàm lấy thông tin tài khoản thông qua Webservices
    /// </summary>
    /// <param name="tenDangNhap">Tên tài khoản cần lấy thông tin</param>
    /// <returns>Object chứa thông tin tài khoản</returns>
    [WebMethod]
    public svTaiKhoan getInForTaiKhoan(string tenDangNhap)
    {
        svTaiKhoan kq = new svTaiKhoan();
        try
        {
            taiKhoan temp = new qlCaPheEntities().taiKhoans.SingleOrDefault(s => s.tenDangNhap.Equals(tenDangNhap));
            if (temp != null)
            {
                kq.tenDangNhap = temp.tenDangNhap;
                kq.maTV = temp.maTV;
                kq.maNhomTK = temp.maNhomTK;
                kq.trangThai = temp.trangThai;
                kq.ghiChu = temp.ghiChu;
                kq.quyenHan = temp.nhomTaiKhoan.quyenHan;
            }
        }
        catch (Exception ex)
        {
            xulyFile.ghiLoi("Class: bNhanVien - Function: getInForTaiKhoan", ex.Message);
        }
        return kq;
    }

    /// <summary>
    /// Hàm lấy thông tin thành viên đã đăng nhập
    /// </summary>
    /// <param name="maTV">Mã thành viên</param>
    /// <returns>Thông tin thành viên</returns>
    [WebMethod]
    public svThanhVien getInForThanhVien(int maTV)
    {
        svThanhVien kq = new svThanhVien();
        try
        {
            thanhVien temp = new qlCaPheEntities().thanhViens.SingleOrDefault(s => s.maTV == maTV);
            if(temp!=null)
            {
                kq.diaChi = temp.diaChi;
                kq.Email = temp.Email;
                kq.Facebook = temp.Facebook;
                kq.ghiChu = temp.ghiChu;
                kq.gioiTinh = temp.gioiTinh;
                kq.hinhDD = temp.hinhDD;
                kq.hoTV = temp.hoTV;
                kq.maTV = temp.maTV;
                kq.ngayCap = temp.ngayCap;
                kq.ngaySinh = temp.ngaySinh;
                kq.ngayThamGia = temp.ngayThamGia;
                kq.noiCap = temp.noiCap;
                kq.noiSinh = temp.noiSinh;
                kq.soCMND = temp.soCMND;
                kq.soDT = temp.soDT;
                kq.tenTV = temp.tenTV;
            }
        }
        catch (Exception ex)
        {
            xulyFile.ghiLoi("Class: bNhanVien - Function: getInFoThanhVien", ex.Message);
        }
        return kq;
    }
    /// <summary>
    /// Hàm lấy danh sách thông báo của 1 tài khoản nhân viên được yêu cầu qua Webservice
    /// </summary>
    /// <param name="tenDangNhap">Tên tài khoản của nhân viên cần lấy thông báo</param>
    /// <returns>List object thông báo</returns>
    [WebMethod]
    public List<svThongBao> getListNotificationsOfUser(string tenDangNhap)
    {
        List<svThongBao> kq = new List<svThongBao>();
        try
        {
            qlCaPheEntities db = new qlCaPheEntities();
            //------Lấy danh sách tất cả thông báo chưa xem của tài khoản
            foreach (thongBao item in db.thongBaos.Where(t => t.taiKhoan == tenDangNhap && t.daXem == false).ToList())
            {
                svThongBao itemKQ = new svThongBao();
                itemKQ.daXem = (bool) item.daXem;
                itemKQ.ghiChu = item.ghiChu;
                itemKQ.maThongBao = item.maThongBao;
                itemKQ.ndThongBao = item.ndThongBao;
                itemKQ.ngayTao = (DateTime) item.ngayTao;
                itemKQ.taiKhoan = item.taiKhoan;
                kq.Add(itemKQ);

                //-----Chuyển trạng thái đã xem
                item.daXem = true;
                db.Entry(item).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

            }
        }
        catch (Exception ex)
        {
            xulyFile.ghiLoi("Class: bNhanVien - Function: getListNotificationsOfUser", ex.Message);
        }
        return kq;
    }

}
