using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using qlCaPhe.Models;
using qlCaPhe.App_Start;
using System.IO;

namespace qlCaPhe.Controllers
{
    public class BaiVietController : Controller
    {
        private string idOfPage = "1001";
        #region CREATE
        /// <summary>
        /// Hàm tạo view tạo mới bài viết
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult bv_TaoMoiBaiViet()
        {
            if (xulyChung.duocTruyCap(idOfPage))
                ViewBag.HinhDD = "/images/image-gallery/1.jpg";
            return View();
        }
        /// <summary>
        /// Hàm thêm mới một bài viết vào CSDL
        /// </summary>
        /// <param name="bv"></param>
        /// <param name="f"></param>
        /// <param name="fileUpload"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult bv_TaoMoiBaiViet(baiViet bv, FormCollection f, HttpPostedFileBase fileUpload)
        {
            string ndThongBao = "";
            if (xulyChung.duocCapNhat(idOfPage, "7"))
            {
                try
                {
                    qlCaPheEntities db = new qlCaPheEntities();
                    this.layDuLieuTuView(bv, f, fileUpload);
                    bv.ngayTao = DateTime.Now;
                    bv.nguoiTao = ((taiKhoan)Session["login"]).tenDangNhap;
                    bv.hienTrangChu = false;
                    db.baiViets.Add(bv);
                    db.SaveChanges();
                    ndThongBao = createHTML.taoNoiDungThongBao("Bài viết", xulyDuLieu.traVeKyTuGoc(bv.tenBaiViet), "bv_TableBaiVietChoDuyet");
                    ViewBag.HinhDD = "/images/image-gallery/1.jpg";
                }
                catch (Exception ex)
                {
                    ndThongBao = ex.Message;
                    xulyFile.ghiLoi("Class: BaiVietController - Function: bv_TaoMoiBaiViet_Post", ex.Message);
                    doDuLieuLenView(bv);
                }
                ViewBag.ThongBao = createHTML.taoThongBaoLuu(ndThongBao);
            }
            return View();
        }
        #endregion
        #region READ
        /// <summary>
        /// Hàm tạo view danh sách bài viết đã duyệt
        /// </summary>
        /// <returns></returns>
        public ActionResult bv_TableBaiVietDaDuyet()
        {
            if (xulyChung.duocTruyCap(idOfPage))
                this.createTableBaiViet(1);
            return View();
        }
        /// <summary>
        /// Hàm tạo view danh mục bài viết chờ kiểm duyệt
        /// </summary>
        /// <returns></returns>
        public ActionResult bv_TableBaiVietChoDuyet()
        {
            if (xulyChung.duocTruyCap(idOfPage))
                this.createTableBaiViet(0);
            return View();
        }

        /// <summary>
        /// Hàm tạo view danh mục bài viết bị gở bỏ
        /// </summary>
        /// <returns></returns>
        public ActionResult bv_TableBaiVietBiGoBo()
        {

            if (xulyChung.duocTruyCap(idOfPage))
                try
                {
                    this.createTableBaiViet(2);
                }
                catch (Exception ex)
                {
                    xulyFile.ghiLoi("Class: BaiVietController - Function: bv_TableBaiVietBiGoBo", ex.Message);
                    return RedirectToAction("ServerError", "Home");
                }
            return View();
        }
        /// <summary>
        /// Hàm thực hiện tạo table bài viết dựa theo trạng thái
        /// </summary>
        /// <param name="trangThai"></param>
        private void createTableBaiViet(int trangThai)
        {
            string htmlTable = "";
            try
            {
                foreach (baiViet bv in new qlCaPheEntities().baiViets.ToList().Where(b => b.trangThai == trangThai))
                {
                    htmlTable += "<tr role=\"row\" class=\"odd\">";
                    htmlTable += "      <td>";
                    htmlTable += "          <a href=\"#\"  maBV=\"" + bv.maBaiViet.ToString() + "\" class=\"goiY\">";
                    htmlTable += xulyDuLieu.traVeKyTuGoc(bv.tenBaiViet);
                    htmlTable += "              <span class=\"noiDungGoiY-right\">Click để xem hình</span>";
                    htmlTable += "          </a>";
                    htmlTable += "      </td>";
                    htmlTable += "      <td>" + xulyDuLieu.traVeKyTuGoc(bv.loaiTin) + "</td>";
                    htmlTable += "      <td>" + bv.ngayTao.ToString() + "</td>";
                    htmlTable += "      <td>" + bv.ngayDang.ToString() + "</td>";
                    htmlTable += "      <td>" + xulyDuLieu.traVeKyTuGoc(bv.taiKhoan.thanhVien.hoTV) + " " + xulyDuLieu.traVeKyTuGoc(bv.taiKhoan.thanhVien.tenTV) + "</td>";
                    htmlTable += "      <td>";
                    htmlTable += "          <div class=\"btn-group\">";
                    htmlTable += "              <button type=\"button\" class=\"btn btn-primary dropdown-toggle\" data-toggle=\"dropdown\" aria-expanded=\"true\">";
                    htmlTable += "                  Chức năng <span class=\"caret\"></span>";
                    htmlTable += "              </button>";
                    htmlTable += "              <ul class=\"dropdown-menu\" role=\"menu\">";
                    htmlTable += createTableData.taoNutChinhSua("/BaiViet/bv_ChinhSuaBaiViet", bv.maBaiViet.ToString());
                    if (trangThai == 0 || trangThai == 2) //---Nếu trạng thái Đã tạo hoặc đã gở thì được phép Duyệt = 1
                        htmlTable += createTableData.taoNutCapNhat("/BaiViet/capNhatTrangThai", bv.maBaiViet.ToString() + "&1", "col-orange", "spellcheck", "Đăng bài");
                    else if (trangThai == 1) //---Nếu trạng thái đã duyệt thì cập nhật thành GỞ BỎ = 2
                        htmlTable += createTableData.taoNutCapNhat("/BaiViet/capNhatTrangThai", bv.maBaiViet.ToString() + "&2", "col-orange", "spellcheck", "Đăng bài");
                    htmlTable += createTableData.taoNutXoaBo(bv.maBaiViet.ToString());
                    htmlTable += "              </ul>";
                    htmlTable += "          </div>";
                    htmlTable += "      </td>";
                    htmlTable += "</tr>";
                }
            }
            catch (Exception ex)
            {
                xulyFile.ghiLoi("Class: BaiVietController - Function: createTableBaiViet", ex.Message);
                Response.Redirect(xulyChung.layTenMien() + "/Home/ServerError");
            }
            ViewBag.TableData = htmlTable;
            ViewBag.ScriptAjax = createScriptAjax.scriptAjaxXoaDoiTuong("BaiViet/xoaBaiViet?maBV=");
            ViewBag.ModalXoa = createHTML.taoCanhBaoXoa("Bài viết");
            //-------Tạo script ajax cho việc click vào tên bài viết để xem nội dung bài viết
            ViewBag.ScriptAjaxXemChiTiet = createScriptAjax.scriptAjaxXemChiTietKhiClick("goiY", "maBV", "BaiViet/AjaxXemChiTietBaiViet?maBV=", "vungChiTiet", "modalChiTiet");
            //-----Tạo modal dạng lớn để chứa nội dung bài viết
            ViewBag.ModalChiTiet = createHTML.taoModalChiTiet("modalChiTiet", "vungChiTiet", 3);
        }
        /// <summary>
        /// Hàm thực hiện lấy thông tin bài viết khi người dùng click vào tiêu để để xem nội dung chi tiết
        /// </summary>
        /// <param name="maBV"></param>
        /// <returns></returns>
        public string AjaxXemChiTietBaiViet(int maBV)
        {
            string htmlDetails = "";
            if (xulyChung.duocTruyCap(idOfPage))
                try
                {
                    baiViet bv = new qlCaPheEntities().baiViets.SingleOrDefault(b => b.maBaiViet == maBV);
                    if (bv != null)
                    {
                        htmlDetails += "<div class=\"modal-header\">";
                        htmlDetails += "      <button type=\"button\" class=\"close\" data-dismiss=\"modal\">×</button>";
                        htmlDetails += "    <h3 class=\"modal-title\" id=\"largeModalLabel\">NỘI DUNG BÀI VIẾT</h3>";
                        htmlDetails += "</div>";
                        htmlDetails += "<div class=\"modal-body\">";
                        htmlDetails += "    <div class=\"row\">";
                        htmlDetails += "        <div class=\"col-md-12 col-lg-12\">";
                        htmlDetails += "            <div class=\"card\">";
                        htmlDetails += "                <div class=\"header bg-cyan\">";
                        htmlDetails += "                    <h2>";
                        htmlDetails += "                        <b>" + xulyDuLieu.traVeKyTuGoc(bv.tenBaiViet) + "</b>";
                        htmlDetails += "                    </h2>";
                        htmlDetails += "                </div>";
                        htmlDetails += "                <div class=\"body table-responsive\">";
                        htmlDetails += "                <!--Nội dung-->";
                        htmlDetails += xulyDuLieu.traVeKyTuGoc(bv.noiDungBaiViet);
                        htmlDetails += "                </div>";
                        htmlDetails += "            </div>";
                        htmlDetails += "        </div>";
                        htmlDetails += "</div>";
                        htmlDetails += "<div class=\"modal-footer\">";
                        htmlDetails += "    <button type=\"button\" class=\"btn btn-default waves-effect\"";
                        htmlDetails += "        data-dismiss=\"modal\">";
                        htmlDetails += "        <i class=\"material-icons\">exit_to_app</i>Đóng lại";
                        htmlDetails += "    </button>";
                        htmlDetails += "</div>";
                        htmlDetails += "</div>";
                    }
                }
                catch (Exception ex)
                {
                    xulyFile.ghiLoi("Class: BaiVietController - Function: AjaxXemChiTietBaiViet", ex.Message);
                }
            return htmlDetails;
        }
        #endregion
        #region UPDATE
        /// <summary>
        /// Hàm tạo giao diện chỉnh sửa bài viết
        /// </summary>
        /// <param name="maBV"></param>
        /// <returns></returns>
        public ActionResult bv_ChinhSuaBaiViet()
        {
            if (xulyChung.duocCapNhat(idOfPage, "7"))
                try
                {
                    string param = xulyChung.nhanThamSoTrongSession(); //-----param=maBV
                    if (param.Length > 0)
                    {
                        int maBV = xulyDuLieu.doiChuoiSangInteger(param);
                        baiViet bvSua = new qlCaPheEntities().baiViets.SingleOrDefault(b => b.maBaiViet == maBV);
                        if (bvSua != null)
                            this.doDuLieuLenView(bvSua);
                        else
                            throw new Exception("Bài viết cần chỉnh sửa không tồn tại");
                    }
                    else throw new Exception("không nhận được tham số");

                }
                catch (Exception ex)
                {
                    xulyFile.ghiLoi("Class: BaiVietController - Function: bv_ChinhSuaBaiViet", ex.Message);
                }
            return View();
        }
        /// <summary>
        /// Hàm thực hiện cập nhật bài viết trong CSDL
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult bv_ChinhSuaBaiViet(FormCollection f, HttpPostedFileBase fileUpload)
        {
            string ndThongBao = "";
            if (xulyChung.duocCapNhat(idOfPage, "7"))
            {
                baiViet bvSua = new baiViet();
                qlCaPheEntities db = new qlCaPheEntities();
                try
                {
                    int maBV = Convert.ToInt32(f["txtMaBV"]);
                    bvSua = db.baiViets.SingleOrDefault(b => b.maBaiViet == maBV);
                    if (bvSua != null)
                    {
                        this.doDuLieuLenView(bvSua);
                        this.layDuLieuTuView(bvSua, f, fileUpload);
                        db.Entry(bvSua).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                        return RedirectToAction("bv_TableBaiVietChoDuyet");
                    }
                }
                catch (Exception ex)
                {
                    ndThongBao = ex.Message;
                    xulyFile.ghiLoi("Class: BaiVietController - Function: bv_ChinhSuaBaiViet", ex.Message);
                    this.doDuLieuLenView(bvSua);
                }
                ViewBag.ThongBao = createHTML.taoThongBaoLuu(ndThongBao);
            }
            return View();
        }
        /// <summary>
        /// Hàm thực hiện cập nhật trạng thái của bài viết
        /// </summary>
        public void capNhatTrangThai()
        {
            if (xulyChung.duocCapNhat(idOfPage, "7"))
                try
                {
                    string param = xulyChung.nhanThamSoTrongSession(); //----param = maBV&trangThai
                    if (param.Length > 0)
                    {
                        int maBV = xulyDuLieu.doiChuoiSangInteger(param.Split('&')[0]);
                        int trangThai = xulyDuLieu.doiChuoiSangInteger(param.Split('&')[1]);
                        if (trangThai == 1 || trangThai == 2) //Chỉ chấp nhận tham số trạng thái cập nhật là 1 hoặc 2
                        {
                            qlCaPheEntities db = new qlCaPheEntities();
                            baiViet bvSua = db.baiViets.SingleOrDefault(b => b.maBaiViet == maBV);
                            if (bvSua != null)
                            {
                                int trangThaiCu = (int)bvSua.trangThai; //Lưu lại trạng thái cũ để chuyển đến danh sách tương ứng
                                if (trangThai == 1) //Nếu là 1 thì cập nhật thêm ngày đăng
                                    bvSua.ngayDang = DateTime.Now;
                                bvSua.trangThai = trangThai;
                                db.Entry(bvSua).State = System.Data.Entity.EntityState.Modified;
                                db.SaveChanges();
                                if (trangThaiCu == 0)//Chuyển đến danh sách bài viết chờ duyệt
                                    Response.Redirect(xulyChung.layTenMien() + "/BaiViet/bv_TableBaiVietChoDuyet");
                                else if (trangThaiCu == 1)//Chuyển đến danh sách bài viết đã duyệt.
                                    Response.Redirect(xulyChung.layTenMien() + "/BaiViet/bv_TableBaiVietDaDuyet");
                                else if (trangThaiCu == 2) //Chuyển đến danh sách bài viết bị gở
                                    Response.Redirect(xulyChung.layTenMien() + "/BaiViet/bv_TableBaiVietBiGoBo");
                            }
                            else
                                throw new Exception("Bài viết cần cập nhật không tồn tại");
                        }
                        else
                            throw new Exception("Trạng thái cần cập nhật không chính xác");
                    }
                    else throw new Exception("không nhận được tham số");

                }
                catch (Exception ex)
                {
                    xulyFile.ghiLoi("Class: BaiVietController - Function: capNhatTrangThai", ex.Message);
                    Response.Redirect(xulyChung.layTenMien() + "/Home/ServerError");
                }
        }
        #endregion
        #region DELETE
        /// <summary>
        /// hàm thực hiện xóa 1 bài viết khỏi CSDL
        /// </summary>
        /// <param name="maBV"></param>
        public void xoaBaiViet(int maBV)
        {
            try
            {
                qlCaPheEntities db = new qlCaPheEntities();
                baiViet bv = db.baiViets.SingleOrDefault(b => b.maBaiViet == maBV);
                if (bv != null)
                {
                    db.baiViets.Remove(bv);
                    db.SaveChanges();
                }
                else
                    throw new Exception("Bài viết có mã " + maBV.ToString() + " không tồn tại để xóa");
            }
            catch (Exception ex)
            {
                xulyFile.ghiLoi("Class: BaiVietController - Function: xoaBaiViet", ex.Message);
                Response.Redirect(xulyChung.layTenMien() + "/Home/PageNotFound");
            }
        }
        #endregion
        #region ORTHERS
        /// <summary>
        /// hàm thực hiện lấy dữ liệu từ giao diện gán cho các thuộc tính của baiViet
        /// </summary>
        /// <param name="bv"></param>
        /// <param name="f"></param>
        /// <param name="fileUpLoad"></param>
        private void layDuLieuTuView(baiViet bv, FormCollection f, HttpPostedFileBase fileUpLoad)
        {
            string loi = "";
            bv.tenBaiViet = xulyDuLieu.xulyKyTuHTML(f["txtTenBaiViet"]);
            if (bv.tenBaiViet.Length <= 0)
                loi += "Vui lòng nhập tiêu đề bài viết<br/>";
            bv.noiDungTomTat = xulyDuLieu.xulyKyTuHTML(f["txtTomTat"]);
            if (bv.noiDungTomTat.Length <= 0)
                loi += "Vui lòng nhập nội dung tóm tắt cho bài viết<br/>";
            bv.loaiTin = xulyDuLieu.xulyKyTuHTML(f["txtLoaiTin"]);
            if (bv.loaiTin.Length <= 0)
                loi += "Vui lòng nhập loại bài viết cho bài viết<br/>";
            bv.ghiChu = xulyDuLieu.xulyKyTuHTML(f["txtGhiChu"]);
            bv.noiDungBaiViet = xulyDuLieu.xulyKyTuHTML(f["txtNoiDungBV"]);
            if (bv.noiDungBaiViet.Length <= 0)
                loi += "Vui lòng nhập nội dung cho bài viết<br/>";
            bv.trangThai = 0; bv.ngayDang = null;
            if (fileUpLoad != null)//Nếu đã chọn hình ảnh
            {
                var fileName = Path.GetFileName(fileUpLoad.FileName);
                //Đường dẫn vào thư mục tạm trên host   
                string folder = Server.MapPath("~/pages/temp/baiViet");
                //Đường dẫn tới file hình trong thư mục tạm
                var path = Path.Combine(folder, fileName);
                //Lưu hình vào thư mục tạm chờ convert
                fileUpLoad.SaveAs(path);
                bv.hinhDD = xulyDuLieu.chuyenDoiHinhSangByteArray(path);
                //--Xóa tập hình trong thư mục tạm
                xulyFile.donDepTM(folder);
            }
            if (loi.Length > 0)
                throw new Exception(loi);
        }
        /// <summary>
        /// Hàm thực hiện đổ dữ liệu có trong baiViet lên giao diện
        /// </summary>
        /// <param name="bv"></param>
        private void doDuLieuLenView(baiViet bv)
        {
            ViewBag.txtMaBV = bv.maBaiViet.ToString();
            ViewBag.txtTenBaiViet = xulyDuLieu.traVeKyTuGoc(bv.tenBaiViet);
            ViewBag.txtTomTat = xulyDuLieu.traVeKyTuGoc(bv.noiDungTomTat);
            ViewBag.txtLoaiTin = xulyDuLieu.traVeKyTuGoc(bv.loaiTin);
            ViewBag.txtGhiChu = xulyDuLieu.traVeKyTuGoc(bv.ghiChu);
            ViewBag.txtNoiDungBV = xulyDuLieu.traVeKyTuGoc(bv.noiDungBaiViet);
            if (bv.hinhDD != null)
                ViewBag.HinhDD = string.Format("data:image/png;base64, {0}", Convert.ToBase64String(bv.hinhDD));
        }
        #endregion
    }
}