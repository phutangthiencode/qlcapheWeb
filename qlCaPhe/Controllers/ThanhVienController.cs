using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using qlCaPhe.App_Start;
using qlCaPhe.Models;
using System.IO;
using System.Data.Entity;
namespace qlCaPhe.Controllers
{
    public class ThanhVienController : Controller
    {
        private static string pathTempHinhCu = null; //Biến lưu trữ đường dẫn đến tập tin hình ảnh cũ trước khi cập nhật
        private static string idOfPage = "203";

        #region CREATE
        /// <summary>
        /// Hàm tạo view tạo mới thành viên
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult tv_TaoMoiThanhVien()
        {
            if (xulyChung.duocTruyCap(idOfPage))
            {
                this.resetDuLieuTrenView();
            }
            return View();
        }
        /// <summary>
        /// Hàm thêm mới thành viên vào CSDL
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult tv_TaoMoiThanhVien(FormCollection f, HttpPostedFileBase fileUpload)
        {
            if (xulyChung.duocCapNhat(idOfPage, "7"))
            {
                string ndThongBao = "";
                thanhVien tv = new thanhVien();
                try
                {
                    qlCaPheEntities db = new qlCaPheEntities();
                    layDuLieuTuView(tv, f, fileUpload);
                    db.thanhViens.Add(tv);
                    db.SaveChanges();
                    ndThongBao = createHTML.taoNoiDungThongBao("Thành viên", xulyDuLieu.traVeKyTuGoc(tv.hoTV + " " + tv.tenTV), "tv_TableThanhVien");
                    this.resetDuLieuTrenView();
                }
                catch (Exception ex)
                {
                    ndThongBao = ex.Message;
                    xulyFile.ghiLoi("Class: ThanhVienController - Function: tv_TaoMoiThanhVien", ex.Message);
                    this.doDuLieuLenView(tv);
                }
                ViewBag.ThongBao = createHTML.taoThongBaoLuu(ndThongBao);
            }
            return View();
        }
        #endregion
        #region READ

        /// <summary>
        /// Hàm thực hiện tạo giao diện danh sách thành viên
        /// </summary>
        /// <returns></returns>
        public ActionResult tv_TableThanhVien(int ?page)
        {
            int trangHienHanh = (page ?? 1);
            if (xulyChung.duocTruyCap(idOfPage))
            {
                try
                {
                    qlCaPheEntities db = new qlCaPheEntities();
                    int soPhanTu = db.thanhViens.Count();
                    ViewBag.PhanTrang = createHTML.taoPhanTrang(soPhanTu, trangHienHanh, "/ThanhVien/tv_TableThanhVien"); //------cấu hình phân trang
                    var thanhVienList = db.thanhViens.OrderBy(t=>t.tenTV).Skip((trangHienHanh - 1) * createHTML.pageSize).Take(createHTML.pageSize).ToList();
                    string htmlTable = "";
                    foreach (thanhVien tv in thanhVienList)
                    {
                        htmlTable += "<tr role=\"row\" class=\"odd\">";
                        htmlTable += "<td><b>" + tv.hoTV + " " + tv.tenTV + "</b></td>";
                        htmlTable += "<td>" + (tv.gioiTinh == true ? "Nam" : "Nữ") + "</td>";
                        htmlTable += "<td>" + string.Format("{0:dd/MM/yyyy}", tv.ngaySinh) + "</td>";
                        htmlTable += "<td>" + tv.diaChi + "</td>";
                        htmlTable += "<td>" + tv.soDT + "</td>";
                        htmlTable += "<td>" + string.Format("{0:dd/MM/yyyy}", tv.ngayThamGia) + "</td>";
                        htmlTable += "<td>";
                        htmlTable += "      <div class=\"btn-group\">";
                        htmlTable += "          <button type=\"button\" class=\"btn btn-primary dropdown-toggle\" data-toggle=\"dropdown\">";
                        htmlTable += "              Chức năng <span class=\"caret\"></span>";
                        htmlTable += "          </button>";
                        htmlTable += "          <ul class=\"dropdown-menu\" role=\"menu\">";
                        htmlTable += createTableData.taoNutChinhSua("/ThanhVien/tv_ChinhSuaThanhVien", tv.maTV.ToString());
                        htmlTable += createTableData.taoNutXoaBo(tv.maTV.ToString());
                        htmlTable += "          </ul>";
                        htmlTable += "      </div>";
                        htmlTable += "</td>";
                        htmlTable += "</tr>";
                    }
                    ViewBag.TableData = htmlTable;
                    ViewBag.ScriptAjax = createScriptAjax.scriptAjaxXoaDoiTuong("ThanhVien/xoaThanhVien?maTV=");
                    ViewBag.ModalXoa = createHTML.taoCanhBaoXoa("Thành viên");
                }
                catch (Exception ex)
                {
                    xulyFile.ghiLoi("Class: ThanhVienController - Function: tv_TaoMoiThanhVien", ex.Message);
                    return RedirectToAction("ServerError", "Home");
                }
            }
            return View();
        }
        #endregion
        #region UPDATE
        /// <summary>
        /// Hàm tạo giao diện chỉnh sửa thành viên
        /// </summary>
        /// <param name="maTV"></param>
        /// <returns></returns>
        public ActionResult tv_ChinhSuaThanhVien()
        {
            if (xulyChung.duocCapNhat(idOfPage, "7"))
            {
                try
                {
                    string urlAction = (string)Session["urlAction"]; //urlAction có dạng: page=tv_ChinhSuaThanhVien|request=maTV
                    //-----Xử lý request trong session
                    urlAction = urlAction.Split('|')[1];  //urlAction có dạng: request=maTV
                    urlAction = urlAction.Replace("request=", ""); //urlAction có dạng: maTV
                    if (urlAction.Length > 0)
                    {
                        int maTV = xulyDuLieu.doiChuoiSangInteger(urlAction);
                        thanhVien thanhVienSua = new qlCaPheEntities().thanhViens.SingleOrDefault(tv => tv.maTV == maTV);
                        if (thanhVienSua != null)
                        {
                            this.resetDuLieuTrenView();
                            this.doDuLieuLenView(thanhVienSua);
                        }
                        else
                            throw new Exception("Thành viên cần cập nhật không tồn tại");
                    }
                    else throw new Exception("KHÔNG nhận được tham số");

                }
                catch (Exception ex)
                {
                    xulyFile.ghiLoi("Class: ThanhVienController - Function: tv_ChinhSuaThanhVien", ex.Message);
                    return RedirectToAction("PageNotFound", "Home");
                }
            }
            return View();
        }
        /// <summary>
        /// Hàm thực hiện chỉnh sửa 1 thành viên trong CSDL
        /// </summary>
        /// <param name="f"></param>
        /// <param name="fileUpload"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult tv_ChinhSuaThanhVien(FormCollection f, HttpPostedFileBase fileUpload)
        {
            if (xulyChung.duocCapNhat(idOfPage, "7"))
            {
                thanhVien thanhVienSua = new thanhVien();
                try
                {
                    int maTV = Int32.Parse(f["txtMaTV"]);
                    qlCaPheEntities db = new qlCaPheEntities();
                    thanhVienSua = db.thanhViens.SingleOrDefault(tv => tv.maTV == maTV);
                    if (thanhVienSua != null)
                    {
                        this.layDuLieuTuView(thanhVienSua, f, fileUpload);
                        db.Entry(thanhVienSua).State = EntityState.Modified;
                        db.SaveChanges();
                        this.resetDuLieuTrenView();
                        return RedirectToAction("tv_TableThanhVien");
                    }
                }
                catch (Exception ex)
                {
                    ViewBag.ThongBao = createHTML.taoThongBaoLuu(ex.Message);
                    xulyFile.ghiLoi("Class: ThanhVienController - Function: tv_TaoMoiThanhVien", ex.Message);
                    this.doDuLieuLenView(thanhVienSua);
                }
            }
            return View();
        }
        #endregion
        #region DELETE
        /// <summary>
        /// Hàm thực hiện xóa 1 thành viên khỏi CSDL
        /// </summary>
        /// <param name="maTV">Mã thành viên cần xóa</param>
        public void xoaThanhVien(int maTV)
        {
            try
            {
                qlCaPheEntities db = new qlCaPheEntities();
                var thanhVienXoa = db.thanhViens.First(tv => tv.maTV == maTV);
                db.thanhViens.Remove(thanhVienXoa);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                xulyFile.ghiLoi("Class: ThanhVienController - Function: xoaThanhVien", ex.Message);
                RedirectToAction("ServerError", "Home");
            }
        }
        #endregion
        #region ORTHERS
        /// <summary>
        /// Hàm thực hiện lấy dữ liệu từ giao diện
        /// </summary>
        /// <param name="tv"></param>
        /// <param name="f"></param>
        /// <param name="fileUpload"></param>
        private void layDuLieuTuView(thanhVien tv, FormCollection f, HttpPostedFileBase fileUpload)
        {
            string loi = "";
            tv.hoTV = xulyDuLieu.xulyKyTuHTML(f["txtHoTV"]);
            if (tv.hoTV.Length <= 0)
                loi += "Vui lòng nhập họ và tên đệm cho thành viên <br/>";
            tv.tenTV = xulyDuLieu.xulyKyTuHTML(f["txtTenTV"]);
            if (tv.tenTV.Length <= 0)
                loi += "Vui lòng nhập tên thành viên <br/>";
            var valGender = f["Gender"];
            if (valGender.Equals("1"))
                tv.gioiTinh = true;
            else
                tv.gioiTinh = false;
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
            tv.ngayThamGia = DateTime.Now;
            tv.ghiChu = xulyDuLieu.xulyKyTuHTML(f["txtGhiChu"]);
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
            else if (pathTempHinhCu != null)
                tv.hinhDD = xulyDuLieu.chuyenDoiHinhSangByteArray(pathTempHinhCu);
            if (loi.Length > 0)
                throw new Exception(loi);
        }
        /// <summary>
        /// Hàm thực hiện đổ dữ liệu của thành viên lên giao diện
        /// </summary>
        /// <param name="tv"></param>
        private void doDuLieuLenView(thanhVien tv)
        {
            ViewBag.txtMaTV = tv.maTV.ToString();
            ViewBag.txtHoTV = xulyDuLieu.traVeKyTuGoc(tv.hoTV);
            ViewBag.txtTenTV = xulyDuLieu.traVeKyTuGoc(tv.tenTV);
            if (tv.gioiTinh == true) ViewBag.rdbNam = "checked";
            else ViewBag.rdbNu = "checked";
            ViewBag.txtNgaySinh = string.Format("{0:yyyy-MM-dd}", tv.ngaySinh);
            ViewBag.txtNoiSinh = xulyDuLieu.traVeKyTuGoc(tv.noiSinh);
            ViewBag.txtDiaChi = xulyDuLieu.traVeKyTuGoc(tv.diaChi);
            ViewBag.txtSDT = xulyDuLieu.traVeKyTuGoc(tv.soDT);
            ViewBag.txtEmail = xulyDuLieu.traVeKyTuGoc(tv.Email);
            ViewBag.txtFacebook = xulyDuLieu.traVeKyTuGoc(tv.Facebook);
            ViewBag.txtCMND = xulyDuLieu.traVeKyTuGoc(tv.soCMND);
            ViewBag.txtNgayCap = string.Format("{0:yyyy-MM-dd}", tv.ngayCap);
            ViewBag.txtNoiCap = xulyDuLieu.traVeKyTuGoc(tv.noiCap);
            ViewBag.txtGhiChu = xulyDuLieu.traVeKyTuGoc(tv.ghiChu);
            if (tv.hinhDD != null)
            {
                ViewBag.HinhDD = string.Format("data:image/png;base64, {0}", Convert.ToBase64String(tv.hinhDD));
                //Lưu lại hình ảnh để sửa
                xulyDuLieu.chuyenByteArrayThanhHinhAndSave(tv.hinhDD, Server.MapPath("~/pages/temp/thanhVien/hinhAnhCu"));
                //Lưu lại đường dẫn hình ảnh cũ
                pathTempHinhCu = Server.MapPath("~/pages/temp/thanhVien/hinhAnhCu.png");
            }
        }
        /// <summary>
        /// Hàm thực hiện lấy thông tin: Hình ảnh, họ tên, số điện thoại của thành viên và hiện lên giao diện tạo tài khoản
        /// Thực hiện Ajax và trả về chuỗi html
        /// </summary>
        /// <param name="maTV">Mã thành viên cần lấy thông tin</param>
        /// <returns>Chuỗi html kết quả để hiển thị.</returns>
        public string getInfoThanhVienForCreateTaiKhoan(int maTV)
        {
            string html = "";
            try
            {
                if (maTV > 0)
                {
                    var thanhVien = new qlCaPheEntities().thanhViens.First(n => n.maTV == maTV);
                    if (thanhVien != null)
                        html=xulyChung.DrawInforThanhVien(thanhVien);
                }
            }
            catch (Exception ex)
            {
                xulyFile.ghiLoi("Class: ThanhVienController - Function: getInfoThanhVienForCreateTaiKhoan", ex.Message);
            }
            return html;
        }
        /// <summary>
        /// hàm ajax lấy thông tin thành viên theo tên đăng nhập
        /// </summary>
        /// <param name="param">Tên đăng nhập của tài khoản thành viên cần lấy</param>
        /// <returns></returns>
        public string AjaxGetInforThanhVienByTaiKhoan(string param)
        {
            string html = "";
            try
            {
                string tenDangNhap = xulyDuLieu.xulyKyTuHTML(param);
                if(!param.Equals(""))
                {
                    taiKhoan tk = new qlCaPheEntities().taiKhoans.SingleOrDefault(t => t.tenDangNhap.Equals(tenDangNhap));
                    if (tk != null)
                        html = xulyChung.DrawInforThanhVienWithTaiKhoan(tk);
                }
            }
            catch (Exception ex)
            {
                xulyFile.ghiLoi("Class: ThanhVienController - Function: AjaxGetInforThanhVienByTaiKhoan", ex.Message);
            }
            return html;
        }

        /// <summary>
        /// Hàm thực hiện reset lại dữ liệu trên giao diện
        /// </summary>
        private void resetDuLieuTrenView()
        {
            ViewBag.rdbNam = "checked";
            ViewBag.hinhDD = "/images/gallery-upload-256.png";
            pathTempHinhCu =null;
        }
        #endregion
    }
}