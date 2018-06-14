using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using qlCaPhe.App_Start;
using qlCaPhe.Models;

namespace qlCaPhe.Controllers
{
    public class NhatKyController : Controller
    {
        private static string idOfPage = "1103";
        /// <summary>
        /// Hàm tạo giao diện danh mục nhật ký truy cập của thành viên
        /// </summary>
        /// <returns></returns>
        public ActionResult nky_NhatKyTruyCap(int ?page)
        {
            if (xulyChung.duocTruyCap(idOfPage))
            {
                try
                {
                    int trangHienHanh = (page ?? 1);
                    qlCaPheEntities db = new qlCaPheEntities();
                    this.taoCbbTaiKhoan(db);
                    //-------Cấu hình phân trang cho danh sách tất cả nhật ký
                    int soPhanTu = db.nhatKies.Count();
                    ViewBag.PhanTrang = createHTML.taoPhanTrang(soPhanTu, trangHienHanh, "/NhatKy/nky_NhatKyTruyCap"); //------cấu hình phân trang
                    //-------Load tất cả nhật ký đã phân trang
                    this.taoDuLieuBangNhatKy(db.nhatKies.OrderBy(n => n.thoiDiem).Skip((trangHienHanh - 1) * createHTML.pageSize).Take(createHTML.pageSize).ToList());
                }
                catch (Exception ex)
                {
                    xulyFile.ghiLoi("Class: NhatKyController - Function: nky_NhatKyTruyCap", ex.Message);
                    return RedirectToAction("PageNotFound", "Home");
                }
            }
            return View();

        }

        /// <summary>
        /// Hàm tạo dữ liệu cho combobox tài khoản trên giao diện
        /// </summary>
        /// <param name="db"></param>
        private void taoCbbTaiKhoan(qlCaPheEntities db)
        {
            try
            {
                string cbb = "";
                //------Lặp qua tất cả các tài khoản có trên hệ thống
                foreach (taiKhoan tk in db.taiKhoans.ToList())
                    cbb += "<option value=\"" + xulyDuLieu.traVeKyTuGoc(tk.tenDangNhap) + "\">" + xulyDuLieu.traVeKyTuGoc(tk.tenDangNhap) + " -- " + xulyDuLieu.traVeKyTuGoc(tk.thanhVien.hoTV + " " + tk.thanhVien.tenTV) + "</option>";
                ViewBag.cbbTaiKhoan = cbb;
            }
            catch (Exception ex)
            {
                xulyFile.ghiLoi("Class: NhatKyController - Function: taoCbbTaiKhoan", ex.Message);
            }
        }

        /// <summary>
        /// Hàm thực hiện tạo dữ liệu cho bảng nhật ký
        /// </summary>
        /// <param name="db"></param>
        private void taoDuLieuBangNhatKy(List<nhatKy> list)
        {
            string kq = "";
            foreach (nhatKy nk in list)
            {
                kq+="<tr role=\"row\" class=\"odd\">";
                kq+="    <td><b class=\"col-blue\">"+xulyDuLieu.traVeKyTuGoc(nk.tenDangNhap + " -- " + nk.taiKhoan.thanhVien.hoTV + " " + nk.taiKhoan.thanhVien.tenTV)+"</b></td>";
                kq+="    <td>"+nk.thoiDiem.ToString()+"</td>";
                kq+="    <td>"+nk.IP+"</td>";
                kq+="    <td>"+nk.trinhDuyet+"</td>";
                kq+="    <td>"+nk.OS+"</td>";
                kq+="    <td><b>"+xulyDuLieu.traVeKyTuGoc(nk.chucNang)+"</b></td>";
                kq+="</tr>";
            }
            ViewBag.TableData = kq;
        }
    }
}