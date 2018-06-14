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
        //-------Khởi tạo một số biến ban đầu
        private static string tenDangNhap = "-1";
        private static DateTime startDate = new DateTime(1900,1,1), endDate = DateTime.Now;

        /// <summary>
        /// Hàm tạo giao diện danh mục nhật ký truy cập của thành viên
        /// </summary>
        /// <returns></returns>
        public ActionResult nky_NhatKyTruyCap(int? page)
        {
            if (xulyChung.duocTruyCap(idOfPage))
            {
                try
                {
                    int trangHienHanh = (page ?? 1);
                    qlCaPheEntities db = new qlCaPheEntities();
                    this.taoCbbTaiKhoan(db);
                    //------Hiện ngày hiện tại lên textbox ngày
                    ViewBag.StartDate = xulyDuLieu.doiNgaySangStringHienLenView(new DateTime(1900, 1, 1));
                    ViewBag.EndDate = xulyDuLieu.doiNgaySangStringHienLenView(DateTime.Now);
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
        /// <param name="list">Danh sách nhật ký cần tạo dữ liệu</param>
        /// <returns>Chuỗi html tạo dữ liệu cho bảng</returns>
        private string taoDuLieuBangNhatKy(List<nhatKy> list)
        {
            string kq = "";
            foreach (nhatKy nk in list)
            {
                kq += "<tr role=\"row\" class=\"odd\">";
                kq += "    <td><b class=\"col-blue\">" + xulyDuLieu.traVeKyTuGoc(nk.tenDangNhap + " -- " + nk.taiKhoan.thanhVien.hoTV + " " + nk.taiKhoan.thanhVien.tenTV) + "</b></td>";
                kq += "    <td>" + nk.thoiDiem.ToString() + "</td>";
                kq += "    <td>" + nk.IP + "</td>";
                kq += "    <td>" + nk.trinhDuyet + "</td>";
                kq += "    <td>" + nk.OS + "</td>";
                kq += "    <td><b>" + xulyDuLieu.traVeKyTuGoc(nk.chucNang) + "</b></td>";
                kq += "</tr>";
            }
            return kq;
        }

        /// <summary>
        /// hàm thực hiện xử lý lấy danh sách nhật ký được liệt kê
        /// </summary>
        /// <param name="param">Tham số cần lấy nhật ký. <para/>Có dạng: tenDangNhap|startDate|endDate|yeuCauThayDoi </param>
        /// <param name="page">Trang hiện hành đang đứng</param>
        /// <returns>Chuổi html tạo dữ liệu cho bảng nhật ký và danh sách số trang đã chia</returns>
        public string AjaxLietKeNhatKy(string param, int? page)
        {
            string kq = "";
            try
            {
                int trangHienHanh = (page ?? 1);
                //-------Kiểm tra có chứa tham số yêu cầu thay đổi liệt kê
                if (param.Split('|').Count() == 4)
                    if (param.Split('|')[3].Equals("1")) //-------Có yêu cầu thay đổi liệt kê danh sách
                    {
                        //------Xử lý và thiết lập lại tham số
                        tenDangNhap = param.Split('|')[0];
                        startDate = DateTime.Parse(param.Split('|')[1]);
                        endDate = DateTime.Parse(param.Split('|')[2]).AddDays(1); //----Tăng thêm 1 ngày để liệt kê từ startDate đến endDate chính xác
                    }
                kq += this.layDanhSachLietKe(trangHienHanh, new qlCaPheEntities());
            }
            catch (Exception ex)
            {
                xulyFile.ghiLoi("Class: NhatKyController - Function: AjaxLietKeNhatKy", ex.Message);
            }
            return kq;
        }
        /// <summary>
        /// Hàm thực hiện 
        /// </summary>
        /// <param name="trangHienHanh">Trang hiện hành đang đứng để lấy danh sách</param>
        /// <param name="db"></param>
        /// <returns>Chuỗi html tạo bảng dữ liệu nhật ký và các trang đã phân chia</returns>
        private string layDanhSachLietKe(int trangHienHanh, qlCaPheEntities db)
        {
            string kq = ""; int soPhanTu = 0;
            List<nhatKy> list = new List<nhatKy>();
            if (tenDangNhap.Equals("-1"))
            {
                //-------Lấy tất cả dữ liệu theo thời điểm
                list = db.nhatKies.Where(n => n.thoiDiem >= startDate && n.thoiDiem <= endDate).OrderByDescending(n => n.thoiDiem).Skip((trangHienHanh - 1) * createHTML.pageSize).Take(createHTML.pageSize).ToList();                
                soPhanTu = db.nhatKies.Where(n => n.thoiDiem >= startDate && n.thoiDiem <= endDate).Count();            
            }
            else
            {
                //------Liệt kê nhật ký theo tài khoản
                list = db.nhatKies.Where(n => n.tenDangNhap.Equals(tenDangNhap) && n.thoiDiem >= startDate && n.thoiDiem <= endDate).OrderByDescending(n => n.thoiDiem).Skip((trangHienHanh - 1) * createHTML.pageSize).Take(createHTML.pageSize).ToList();
                soPhanTu = db.nhatKies.Where(n => n.tenDangNhap.Equals(tenDangNhap) && n.thoiDiem >= startDate && n.thoiDiem <= endDate).Count();
            }
            kq += this.taoDuLieuBangNhatKy(list);
            kq += "&&&";
            kq += createHTML.taoPhanTrang(soPhanTu, trangHienHanh, "/NhatKy/nky_NhatKyTruyCap"); //------cấu hình phân trang
            return kq;
        }

    }
}