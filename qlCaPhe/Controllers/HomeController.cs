using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using qlCaPhe.Models.Business;
using qlCaPhe.Models;
using qlCaPhe.App_Start;
using System.IO;
using System.Xml.Linq;
using System.Xml;
using qlCaPhe.Models.Entities;
using qlCaPhe.Models.Business;
using System.Collections;


namespace qlCaPhe.Controllers
{
    public class HomeController : Controller
    {
        private static string pathTempHinhCu = null; //Biến lưu trữ đường dẫn đến tập tin hình ảnh cũ trước khi cập nhật
        /// <summary>
        /// Hàm tạo giao diện login vào hệ thống dành cho member
        /// </summary>
        /// <returns></returns>
        public ActionResult Login()
        {
            try
            {
                taiKhoan tkLogin = (taiKhoan)Session["login"];
                if (tkLogin.tenDangNhap != null)//------Đã đăng nhập
                    Response.Redirect(tkLogin.nhomTaiKhoan.trangMacDinh);
            }
            catch (Exception ex)
            {
                xulyFile.ghiLoi("Class: HomeController - Function: Login", ex.Message);
            }
            return View();
        }
        /// <summary>
        /// Hàm thực hiện đăng nhập vào hệ thống
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Login(FormCollection f)
        {
            string tenDangNhap = f["txtTenDangNhap"]; string matKhau = f["txtMatKhau"];
            if (bProfile.kiemTraDangNhap(tenDangNhap, matKhau) == 1)//-----Đăng nhập thành công
            {
                taiKhoan tkLogin = new qlCaPheEntities().taiKhoans.SingleOrDefault(t => t.tenDangNhap == tenDangNhap);
                Session["login"] = tkLogin;
                if (!tkLogin.nhomTaiKhoan.trangMacDinh.Equals(""))
                    return Redirect(xulyDuLieu.traVeKyTuGoc(tkLogin.nhomTaiKhoan.trangMacDinh));
                return RedirectToAction("Index", "Tools_VungLamViec");
            }
            //-------Thất bại về trang chủ
            return RedirectToAction("Index", "PublicPage");
        }

        /// <summary>
        /// Hàm xử lý logout khỏi hệ thống
        /// </summary>
        /// <returns></returns>
        public ActionResult Logout()
        {
            Session.Clear();
            Session.RemoveAll();
            Session.Abandon();
            return RedirectToAction("Login", "Home");
        }
        /// <summary>
        /// Hàm tạo view trang profile của user đã đăng nhập
        /// </summary>
        /// <returns></returns>
        public ActionResult Profile()
        {
            taiKhoan tkLogin = (taiKhoan)Session["login"];
            if (tkLogin.tenDangNhap == null) //------Nếu không nhận được tên đăng nhập. Chưa login
                return RedirectToAction("Login");
            //Lưu lại hình ảnh để sửa
            xulyDuLieu.chuyenByteArrayThanhHinhAndSave(tkLogin.thanhVien.hinhDD, Server.MapPath("~/pages/temp/thanhVien/hinhAnhCu"));
            //Lưu lại đường dẫn hình ảnh cũ
            pathTempHinhCu = Server.MapPath("~/pages/temp/thanhVien/hinhAnhCu.png");
            return View(tkLogin);
        }


        public ActionResult Index()
        {
            return View();
        }

        #region CRUD Account
        /// <summary>
        /// Hàm thực hiện cập nhật lại mật khẩu tài khoản thành viên
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult capNhatMatKhau(FormCollection f)
        {
            string thongBao = ""; int kqLuu = 0;
            taiKhoan tk = new taiKhoan();
            try
            {
                tk = (taiKhoan)Session["login"];
                if (tk.tenDangNhap != null)
                {
                    qlCaPheEntities db = new qlCaPheEntities();
                    tk = db.taiKhoans.SingleOrDefault(t => t.tenDangNhap == tk.tenDangNhap);
                    this.layDuLieuTuViewTaiKhoan(tk, f);
                    db.Entry(tk).State = System.Data.Entity.EntityState.Modified;
                    kqLuu = db.SaveChanges();
                    if (kqLuu > 0)
                        thongBao = "Cập nhật thông tin tài khoản thành công";
                }
                else
                    return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                xulyFile.ghiLoi("Class: HomeController - Function: capNhatMatKhau", ex.Message);
                thongBao = ex.Message;
            }
            ViewBag.ThongBao = createHTML.taoThongBaoLuu(thongBao);
            return View("Profile", tk);
        }
        /// <summary>
        /// Hàm lấy dữ liệu từ view dành cho các thuộc tính của object tài khoản
        /// </summary>
        /// <param name="tk"></param>
        private void layDuLieuTuViewTaiKhoan(taiKhoan tk, FormCollection f)
        {
            string loi = "";
            string matKhauCu = f["txtMatKhauCu"], matKhauMoi = f["txtMatKhauMoi"].Trim(), xacNhan = f["txtXacNhan"].Trim();
            if (matKhauMoi.Length < 8)
                throw new Exception("Vui lòng nhập mật khẩu có ít nhất 8 ký tự");
            if (matKhauMoi.Equals(xacNhan) == false)
                throw new Exception("Mật khẩu mới không trùng khớp");
            //------Xử lý mã hóa mật khẩu
            matKhauCu = xulyMaHoa.Encrypt(matKhauCu);
            matKhauMoi = xulyMaHoa.Encrypt(matKhauMoi);
            //------Kiểm tra, nếu mật khẩu cũ sai thì báo lỗi
            if (!matKhauCu.Equals(tk.matKhau))
                throw new Exception("Mật khẩu cũ không chính xác");
            //-----------------Hợp lệ để cập nhật
            tk.matKhau = matKhauMoi;
            tk.ghiChu = xulyDuLieu.xulyKyTuHTML(f["txtGhiChuTaiKhoan"]);
            if (loi.Length > 0)
                throw new Exception(loi);
        }
        #endregion
        #region CRUD PROFILE
        /// <summary>
        /// Hàm cập nhật thông tin thành viên tại trang profile
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult capNhatProfile(FormCollection f, HttpPostedFileBase fileUpload)
        {
            string thongBao = ""; int kqLuu = 0;
            taiKhoan tkLogin = new taiKhoan();
            try
            {
                tkLogin = (taiKhoan)Session["login"];
                if (tkLogin.tenDangNhap != null)
                {
                    qlCaPheEntities db = new qlCaPheEntities();
                    thanhVien tvSua = db.taiKhoans.SingleOrDefault(t => t.tenDangNhap == tkLogin.tenDangNhap).thanhVien;
                    this.layDuLieuTuViewThanhVien(tvSua, f, fileUpload);
                    db.Entry(tvSua).State = System.Data.Entity.EntityState.Modified;
                    kqLuu = db.SaveChanges();
                    if (kqLuu > 0)
                    {
                        thongBao = "Lưu thông tin thành công";
                        tkLogin.thanhVien = tvSua;
                    }
                }
                else
                    return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                xulyFile.ghiLoi("Class: HomeController - Function: capNhatProfile", ex.Message);
                thongBao = ex.Message;
            }
            ViewBag.ThongBao = createHTML.taoThongBaoLuu(thongBao);
            return View("Profile", tkLogin);
        }



        /// <summary>
        /// Hàm thực hiện lấy dữ liệu từ giao diện
        /// </summary>
        /// <param name="tv"></param>
        /// <param name="f"></param>
        /// <param name="fileUpload"></param>
        private void layDuLieuTuViewThanhVien(thanhVien tv, FormCollection f, HttpPostedFileBase fileUpload)
        {
            string loi = "";
            tv.hoTV = xulyDuLieu.xulyKyTuHTML(f["txtHoTV"]);
            if (tv.hoTV.Length <= 0)
                loi += "Vui lòng nhập họ và tên đệm cho thành viên <br/>";
            tv.tenTV = xulyDuLieu.xulyKyTuHTML(f["txtTenTV"]);
            if (tv.tenTV.Length <= 0)
                loi += "Vui lòng nhập tên thành viên <br/>";
            var valGender = f["Gender"];
            tv.gioiTinh = true ? valGender.Equals("1") : false;
            tv.ngaySinh = DateTime.Parse(f["txtNgaySinh"]);
            tv.noiSinh = xulyDuLieu.xulyKyTuHTML(f["txtNoiSinh"]);
            tv.diaChi = xulyDuLieu.xulyKyTuHTML(f["txtDiaChi"]);
            if (tv.diaChi.Length <= 0)
                loi += "Vui lòng nhập địa chỉ liên lạc của thành viên <br/>";
            tv.soDT = xulyDuLieu.xulyKyTuHTML(f["txtSDT"]);
            if (tv.soDT.Length <= 0)
                loi += "Vui lòng nhập số điện thoại của thành viên <br/>";
            tv.Email = xulyDuLieu.xulyKyTuHTML(f["txtEmail"]);
            tv.Facebook = xulyDuLieu.xulyKyTuHTML(f["txtFacebook"]);
            tv.soCMND = xulyDuLieu.xulyKyTuHTML(f["txtCMND"]);
            if (tv.soCMND.Length <= 0)
                loi += "Vui lòng nhập số CMND của thành viên <br/>";
            if (!f["txtNgayCap"].Equals(""))
                tv.ngayCap = DateTime.Parse(f["txtNgayCap"]);
            tv.noiCap = xulyDuLieu.xulyKyTuHTML(f["txtNoiCap"]);
            tv.ghiChu = xulyDuLieu.xulyKyTuHTML(f["txtGhiChuThanhVien"]);
            if (fileUpload != null) //Nếu có hình ảnh 
            {
                var fileName = Path.GetFileName(fileUpload.FileName);
                //Đường dẫn vào thư mục tạm trên host   
                string folder = Server.MapPath("~/pages/temp/thanhVien");
                //Đường dẫn tới file hình trong thư mục tạm
                var path = Path.Combine(folder, fileName);
                //Lưu hình vào thư mục tạm chờ convert
                fileUpload.SaveAs(path);
                tv.hinhDD = xulyDuLieu.chuyenDoiHinhSangByteArray(path);
                //--Xóa tập hình trong thư mục tạm
                xulyFile.donDepTM(folder);
            }
            if (loi.Length > 0)
                throw new Exception(loi);
        }
        #endregion

        #region VÙNG NAVBAR MENU TOOLS

        /// <summary>
        /// Hàm hiện thông tin thành viên đã đăng nhập lên vùng slidebar công cụ
        /// </summary>
        /// <returns></returns>
        public ActionResult PartProfileNavTools()
        {
            taiKhoan tkLogin = new taiKhoan();
            try
            {
                tkLogin = (taiKhoan)Session["login"];
                if (tkLogin.tenDangNhap != null)
                    return PartialView(tkLogin);
            }
            catch (Exception ex)
            {
                xulyFile.ghiLoi("Class: HomeController - Function: PartProfileNavTools", ex.Message);
                return RedirectToAction("Login");
            }
            return RedirectToAction("Login");
        }
        /// <summary>
        /// Hàm tạo danh mục menu trên trang private
        /// </summary>
        /// <returns></returns>
        public ActionResult PartMenuTools()
        {
            //string quyenHan = "1|-2|201&6:202&7:203&3-3|301&1:302&2:303&5";
            taiKhoan tkLogin = (taiKhoan)Session["login"];
            try
            {
                if (tkLogin.tenDangNhap != null)
                {
                    string quyenHan = xulyDuLieu.traVeKyTuGoc(tkLogin.nhomTaiKhoan.quyenHan);
                    List<string> listCum = new List<string>(); //-----Mảng chứa các menu cha
                    string[] cum = quyenHan.Split('-');
                    foreach (string i in cum)
                        listCum.Add(i);
                    return PartialView(new bMenuTools().readMenuToolsWithPermission(listCum));
                }

            }
            catch (Exception ex)
            {
                xulyFile.ghiLoi("Class: HomeController - Function: PartMenuTools", ex.Message);
            }
            return Redirect(xulyChung.layTenMien() + "/Login");
        }

        #endregion

        #region TRANG BÁO LỖI
        public ActionResult PageNotFound()
        {
            Response.StatusCode = 404;
            taiKhoan tkLogin = (taiKhoan)Session["login"];
            if (tkLogin.tenDangNhap != null)//------Đã đăng nhập
                ViewBag.VeTrangChu = tkLogin.nhomTaiKhoan.trangMacDinh;
            else
                return RedirectToAction("Login");
            return View();
        }
        public ActionResult ServerError()
        {
            return View();
        }

        /// <summary>
        /// Hàm tạo giao diện trang ngăn chặn truy cập khi không có quyền hạn truy cập
        /// </summary>
        /// <returns></returns>
        public ActionResult h_AccessDenied()
        {
            try
            {
                taiKhoan tkLogin = (taiKhoan)Session["login"];//Lấy tài khoản đang login để lấy trang mặc định của tài khoản này
                if(tkLogin!=null)
                    if(tkLogin.tenDangNhap!=null)
                        return View(tkLogin);
            }
            catch (Exception ex)
            {
                xulyFile.ghiLoi("Class: HomeController - Function: h_AccessDenied", ex.Message);
            }
            return RedirectToAction("Login");

        }

        #endregion

        #region HÀM HIỂN THỊ THÔNG BÁO
        /// <summary>
        /// Hàm tạo partialview thông báo trên thanh title
        /// </summary>
        /// <returns></returns>
        public ActionResult Notifications()
        {
            try
            {
                taiKhoan tkLogin = new taiKhoan();
                tkLogin = (taiKhoan)Session["login"];
                List<thongBao> notifys  = new List<thongBao>();
                notifys = new qlCaPheEntities().thongBaos.Where(t => t.taiKhoan == tkLogin.tenDangNhap && t.daXem==false).ToList();
                return PartialView(notifys);
            }
            catch (Exception ex)
            {
                xulyFile.ghiLoi("Class: HomeController - Function: Notifications", ex.Message);
                return RedirectToAction("Login");
            }
        }
        /// <summary>
        /// Hàm xử lý Ajax CẬP NHẬT TRẠNG THÁI thông báo khi người dùng click vào nút xóa tất cả thông báo trên vùng thông báo
        /// </summary>
        public void AjaxXoaTatCaThongBao()
        {
            try
            {
                taiKhoan tkLogin = (taiKhoan)Session["login"];
                qlCaPheEntities db = new qlCaPheEntities();
                foreach (thongBao itemXoa in db.thongBaos.Where(t => t.taiKhoan == tkLogin.tenDangNhap).ToList())
                {
                    itemXoa.daXem = true;
                    db.Entry(itemXoa).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                xulyFile.ghiLoi("Class: HomeController - Function: xoaThongBao", ex.Message);
            }
        }
        #endregion

    }
}