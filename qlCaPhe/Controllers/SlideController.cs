using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using qlCaPhe.Models;
using qlCaPhe.App_Start;

namespace qlCaPhe.Controllers
{
    public class SlideController : Controller
    {
        private string idOfPage = "1202";
        #region CREATE
        /// <summary>
        /// Hàm thực hiện tạo giao diện thêm mới Slide
        /// </summary>
        /// <returns></returns>
        public ActionResult sl_TaoMoiSlideShow()
        {
            if (xulyChung.duocTruyCap(idOfPage))
            {
                this.resetDuLieu();
                xulyChung.ghiNhatKyDtb(1, "Tạo mới slideshow");
            }
            return View();
        }
        /// <summary>
        /// Hàm thực hiện thêm mới 1 slide vào CSDL
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult sl_TaoMoiSlideShow(slide s, FormCollection f)
        {
            if (xulyChung.duocCapNhat(idOfPage, "7"))
            {
                string ndThongBao = ""; int kqLuu = 0;
                try
                {
                    qlCaPheEntities db = new qlCaPheEntities();
                    this.layDuLieuTuView(s, f);
                    db.slides.Add(s);
                    kqLuu = db.SaveChanges();
                    if (kqLuu > 0)
                    {
                        ndThongBao = createHTML.taoNoiDungThongBao("Slideshow", s.maSlide.ToString(), "RouteSlideCamHienThi");
                        this.resetDuLieu();
                        xulyChung.ghiNhatKyDtb(2, "Chỉnh sửa slideshow\" " + s.maSlide.ToString() + " \"");
                    }
                }
                catch (Exception ex)
                {
                    ndThongBao = ex.Message;
                    xulyFile.ghiLoi("Class SlideController - Function: sl_TaoMoiSlideShow_Post", ex.Message);
                    this.resetDuLieu();
                    this.doDuLieuLenView(s);
                }
                ViewBag.ThongBao = createHTML.taoThongBaoLuu(ndThongBao);
            }
            return View();
        }
        #endregion
        #region READ

        /// <summary>
        /// hàm cấu hình chuyển đến slideshow còn hiển thị
        /// </summary>
        /// <returns>Chuyển đến Action sl_TableSlideShow</returns>
        public ActionResult RouteSlideHienThi()
        {
            xulyChung.cauHinhSession("sl_TableSlideShow", "1");
            xulyChung.ghiNhatKyDtb(1, "Danh mục slideshow được hiển thị");
            return RedirectToAction("sl_TableSlideShow");
        }
        /// <summary>
        /// hàm cấu hình chuyển đến slideshow cấm hiển thị
        /// </summary>
        /// <returns>Chuyển đến Action sl_TableSlideShow</returns>
        public ActionResult RouteSlideCamHienThi()
        {
            xulyChung.cauHinhSession("sl_TableSlideShow", "0");
            xulyChung.ghiNhatKyDtb(1, "Danh mục slideshow cấm hiển thị");
            return RedirectToAction("sl_TableSlideShow");
        }


        /// <summary>
        /// Hàm tạo giao diện danh sách slideshow, được sắp theo trạng thái
        /// </summary>
        /// <param name="page">Trang hiện hành đang đứng</param>
        /// <returns></returns>
        public ActionResult sl_TableSlideShow(int ?page)
        {
            int trangHienHanh = (page ?? 1); int pageSize = 6; //----Thiết lập 1 trang chỉ hiện 6 phần tử
            if (xulyChung.duocTruyCap(idOfPage))
            {
                string htmlTable = "";
                try
                {
                    //-------Xử lý tham số truyền vào
                    string urlAction = (string)Session["urlAction"];
                    if (urlAction.Length > 0)
                    {
                        urlAction = urlAction.Split('|')[1]; urlAction = urlAction.Replace("request=", "");
                        bool trangThai = urlAction.Equals("1") ? true : false; //-----Nếu request là 1 thì trạng thái true và ngược lại
                        thietLapThongSoTrang(trangThai);

                        qlCaPheEntities db = new qlCaPheEntities();
                        int soPhanTu = db.slides.Where(t => t.trangThai == trangThai).Count();
                        ViewBag.PhanTrang = createHTML.taoPhanTrang(soPhanTu, pageSize, trangHienHanh, "/Slide/sl_TableSlideShow");

                        foreach (slide s in new qlCaPheEntities().slides.ToList().Where(sl => sl.trangThai == trangThai).OrderBy(l=>l.thuTu).Skip((trangHienHanh - 1) * pageSize).Take(pageSize))
                            htmlTable += this.createTable(s);
                    }
                    else throw new Exception("không có tham số ");
                }
                catch (Exception ex)
                {
                    xulyFile.ghiLoi("Class SlideController - Function: sl_TableSlideShow", ex.Message);
                    return RedirectToAction("PageNotFound", "Home");
                }
                ViewBag.TableData = htmlTable;
                ViewBag.ScriptAjax = createScriptAjax.scriptAjaxXoaDoiTuong("Slide/xoaSlide?maSlide=");
                ViewBag.ModalXoa = createHTML.taoCanhBaoXoa("SlideShow");               
            }
            return View();
        }

        /// <summary>
        /// Hàm thực hiện tạo một dòng dữ liệu slideShow
        /// </summary>
        /// <param name="trangThai"></param>
        /// <returns></returns>
        private string createTable(slide s)
        {
            string htmlTable = "";
            htmlTable += "<tr role=\"row\" class=\"odd\">";
            htmlTable += "      <td>";
            htmlTable += "          <img width=\"300px\" height=\"auto;\" src=\"" + xulyDuLieu.chuyenByteHinhThanhSrcImage(s.hinhAnh) + "\">";
            htmlTable += "      </td>";
            htmlTable += "      <td>";
            htmlTable += s.thuTu.ToString();
            htmlTable += "      </td>";
            htmlTable += "      <td>" + s.ngayTao.ToString() + "</td>";
            htmlTable += "      <td>" + xulyDuLieu.traVeKyTuGoc(s.ghiChu) + "</td>";
            htmlTable += "      <td>";
            htmlTable += "          <div class=\"btn-group\">";
            htmlTable += "              <button type=\"button\" class=\"btn btn-primary dropdown-toggle\" data-toggle=\"dropdown\">";
            htmlTable += "                  Chức năng <span class=\"caret\"></span>";
            htmlTable += "              </button>";
            htmlTable += "              <ul class=\"dropdown-menu\" role=\"menu\">";
            htmlTable += createTableData.taoNutChinhSua("/Slide/sl_ChinhSuaSlideShow", s.maSlide.ToString());
            htmlTable += createTableData.taoNutCapNhat("/Slide/capNhatTrangThai", s.maSlide.ToString(), "col-orange", "refresh", "Chuyển trạng thái");
            htmlTable += createTableData.taoNutXoaBo(s.maSlide.ToString());
            htmlTable += "              </ul>";
            htmlTable += "          </div>";
            htmlTable += "      </td>";
            htmlTable += "</tr>";
            return htmlTable;
        }
        #endregion
        #region UPDATE
        /// <summary>
        /// Hàm thực hiện cập nhật trạng thái của 1 slide
        /// Trạng thái cập nhật sẽ ngược với trạng thái hiện tại
        /// </summary>
        /// <param name="maSlide">Mã Slide cần cập nhật</param>
        public void capNhatTrangThai()
        {
            if (xulyChung.duocCapNhat(idOfPage, "7"))
                try
                {
                    int kqLuu = 0;
                    string param = xulyChung.nhanThamSoTrongSession();
                    if (param.Length > 0)
                    {
                        int maSlide = xulyDuLieu.doiChuoiSangInteger(param);
                        qlCaPheEntities db = new qlCaPheEntities();
                        slide slideSua = db.slides.SingleOrDefault(s => s.maSlide == maSlide);
                        if (slideSua != null)
                        {
                            bool trangThaiCu = (bool)slideSua.trangThai; //Lưu lại trạng thái cũ để chuyển đến danh sách tương ứng
                            slideSua.trangThai = !trangThaiCu;//Cập nhật trạng thái ngược với trạng thái cũ
                            db.Entry(slideSua).State = System.Data.Entity.EntityState.Modified;
                            kqLuu = db.SaveChanges();
                            if (kqLuu > 0)
                            {
                                xulyChung.ghiNhatKyDtb(4, "Cập nhật trạng thái slideshow\" " + slideSua.maSlide.ToString() + " \"");
                                if (trangThaiCu)
                                    Response.Redirect(xulyChung.layTenMien() + "/Slide/RouteSlideHienThi");
                                else
                                    Response.Redirect(xulyChung.layTenMien() + "/Slide/RouteSlideCamHienThi");
                            }
                        }
                        else
                            throw new Exception("Slide có mã " + maSlide + " không tồn tại để cập nhật");
                    }
                    else throw new Exception("không nhận được tham số");
                }
                catch (Exception ex)
                {
                    xulyFile.ghiLoi("Class: SlideController - Function: capNhatTrangThai", ex.Message);
                    Response.Redirect(xulyChung.layTenMien() + "/Home/ServerError");
                }
        }
        /// <summary>
        /// Hàm tạo giao diện chỉnh sửa thông tin cho Slide
        /// </summary>
        /// <param name="maSlide"></param>
        /// <returns></returns>
        public ActionResult sl_ChinhSuaSlideShow()
        {
            if (xulyChung.duocCapNhat(idOfPage, "7"))
                try
                {
                    string param = xulyChung.nhanThamSoTrongSession();
                    if (param.Length > 0)
                    {
                        int maSlide = xulyDuLieu.doiChuoiSangInteger(param);
                        this.resetDuLieu();
                        slide slideSua = new qlCaPheEntities().slides.SingleOrDefault(s => s.maSlide == maSlide);
                        if (slideSua != null)
                        {
                            this.doDuLieuLenView(slideSua);
                            xulyChung.ghiNhatKyDtb(1, "Chỉnh sửa slideshow\" " + slideSua.maSlide.ToString() + " \"");
                        }
                        else
                            return RedirectToAction("PageNotFound", "Home");
                    }
                    else throw new Exception("không nhận được tham số");
                }
                catch (Exception ex)
                {
                    xulyFile.ghiLoi("Class: SlideController - Function: sl_ChinhSuaSlideShow_Get", ex.Message);
                }
            return View();
        }
        /// <summary>
        /// Hàm cập nhật 1 slideshow trong CSDL
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult sl_ChinhSuaSlideShow(FormCollection f)
        {
            if (xulyChung.duocCapNhat(idOfPage, "7"))
            {
                string ndThongBao = ""; int kqLuu = 0;
                slide slideSua = new slide();
                try
                {
                    qlCaPheEntities db = new qlCaPheEntities();
                    int maSlide = xulyDuLieu.doiChuoiSangInteger(f["txtMaSlide"]);
                    slideSua = db.slides.SingleOrDefault(s => s.maSlide == maSlide);
                    if (slideSua != null)
                    {
                        this.doDuLieuLenView(slideSua);
                        this.layDuLieuTuView(slideSua, f);
                        db.Entry(slideSua).State = System.Data.Entity.EntityState.Modified;
                        kqLuu = db.SaveChanges();
                        if (kqLuu > 0)
                        {
                            xulyChung.ghiNhatKyDtb(4, "Chỉnh sửa slideshow\" " + slideSua.maSlide.ToString() + " \"");
                            return RedirectToAction("RouteSlideCamHienThi"); //Chuyển đến trang danh sách slideshow không hiển thị khi thành công
                        }
                    }
                }
                catch (Exception ex)
                {
                    ndThongBao = ex.Message;
                    xulyFile.ghiLoi("Class: SlideController - Function: sl_ChinhSuaSlideShow_Post", ex.Message);
                    this.resetDuLieu();
                    this.doDuLieuLenView(slideSua);
                }
                ViewBag.ThongBao = createHTML.taoThongBaoLuu(ndThongBao);
            }
            return View();
        }
        #endregion
        #region DELETE
        /// <summary>
        /// Hàm thực hiện xóa 1 slide khỏi CSDL
        /// </summary>
        /// <param name="maSlide"></param>
        public void xoaSlide(int maSlide)
        {
            try
            {
                int kqLuu = 0;
                qlCaPheEntities db = new qlCaPheEntities();
                slide slideXoa = db.slides.SingleOrDefault(s => s.maSlide == maSlide);
                if (slideXoa != null)
                {
                    db.slides.Remove(slideXoa);
                    kqLuu = db.SaveChanges();
                    if (kqLuu > 0)
                        xulyChung.ghiNhatKyDtb(3, "Slide show \"" + slideXoa.maSlide.ToString() + " \"");
                }
                else
                    throw new Exception("Slide có mã " + maSlide.ToString() + " không tồn tại để xóa");
            }
            catch (Exception ex)
            {
                xulyFile.ghiLoi("Class: SlideController - Function: xoaSlide", ex.Message);
                Response.Redirect(xulyChung.layTenMien() + "/Home/ServerError");
            }
        }
        #endregion
        #region ORTHERS
        /// <summary>
        /// Hàm thực hiện lấy dữ liệu từ giao diện gán cho các thuộc tính của slide
        /// </summary>
        /// <param name="s"></param>
        /// <param name="f"></param>
        private void layDuLieuTuView(slide s, FormCollection f)
        {
            string loi = "";
            s.thuTu = xulyDuLieu.doiChuoiSangInteger(f["txtThuTu"]);
            s.ghiChu = xulyDuLieu.xulyKyTuHTML(f["txtGhiChu"]);
            string pathHinh = f["pathHinh"];
            //------Kiểm tra xem có chọn hình không. 
            if (!pathHinh.Equals("")) //Nếu có chọn hình  thì luu hình vào csdl
            {
                pathHinh = xulyMaHoa.Decrypt(pathHinh);//Giải mã lại chuỗi đường dẫn lưu hình ảnh trên đĩa đã được mã hóa trong ajax
                s.hinhAnh = xulyDuLieu.chuyenDoiHinhSangByteArray(pathHinh); //Lưu hình vào thuộc tính hinhAnh
            }
            else //-------Nếu không có chọn hình
                if (f["txtMaSlide"] == null) //--------Kiểm tra xem có mã sản phẩm không. Nếu không tức là thêm mới và báo lỗi
                    loi += "Vui lòng chọn hình ảnh cho slideshow <br/>";

            //--------Thiết lập các thuộc tính mặc định khi lưu
            s.trangThai = false;
            s.ngayTao = DateTime.Now;
            if (loi.Length > 0)
                throw new Exception(loi);
        }
        /// <summary>
        /// Hàm thực hiện hiển thị dữ liệu có trong slide lên giao diện
        /// </summary>
        /// <param name="s"></param>
        private void doDuLieuLenView(slide s)
        {
            ViewBag.txtMaSlide = s.maSlide.ToString();
            ViewBag.txtThuTu = s.thuTu.ToString();
            ViewBag.txtGhiChu = xulyDuLieu.traVeKyTuGoc(s.ghiChu);
            if (s.hinhAnh != null)
                ViewBag.hinhDD = xulyDuLieu.chuyenByteHinhThanhSrcImage(s.hinhAnh);
        }
        /// <summary>
        /// Hàm thực hiện thiết lập lại các thông số cho việc nhận hình ảnh
        /// </summary>
        private void resetDuLieu()
        {
            //Gán hình mặc định 
            ViewBag.hinhDD = "/images/gallery-upload-256.png";
            //Xóa tất cả các hình đã tải lên trong tập tin tạm            
            xulyFile.donDepTM(Server.MapPath("~/pages/temp/slideshow"));
            //Nhúng script ajax thực hiện tải và cắt hình
            ViewBag.ScriptCropImage = createScriptAjax.scriptAjaxUpLoadAndCropImage("slideshow", 388, 268, 388, 268);
            //Gán modal up và crop hình ảnh lên view
            ViewBag.modalChonHinh = createHTML.taoModalUpHinh();
        }
        /// <summary>
        /// Hàm thực hiện thiết lập thông số, nội dung cho trang dựa vào trạng thái
        /// </summary>
        /// <param name="trangThai"></param>
        private void thietLapThongSoTrang(bool trangThai)
        {
            if (trangThai) //Dành cho trang Danh mục Slideshow được hiển thị
            {
                ViewBag.TieuDeTrang = "DANH MỤC SLIDESHOW ĐƯỢC HIỂN THỊ";
                ViewBag.Style1 = "active"; //Thêm class active cho tab trang này
                ViewBag.Style2 = ""; //Reset lại class cho tab slideshow hủy
            }
            else
            {
                ViewBag.TieuDeTrang = "DANH MỤC SLIDESHOW CHƯA HIỂN THỊ";
                ViewBag.Style1 = "";  //Reset lại class cho tab slideshow hiển thị
                ViewBag.Style2 = "active"; //Thêm class active cho tab trang này 
            }
        }


        #endregion
    }
}