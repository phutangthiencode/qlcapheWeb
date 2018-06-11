using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using qlCaPhe.Models;
using qlCaPhe.App_Start;

namespace qlCaPhe.Controllers
{
    public class DanhGiaController : Controller
    {
        private static string idOfPageMucTieu = "904";
        #region NHÓM HÀM CHO BẢNG MỤC TIÊU ĐÁNH GIÁ
        #region CREATE
        /// <summary>
        /// Hàm tạo giao diện tạo mới mục tiêu đánh giá
        /// </summary>
        /// <returns></returns>
        public ActionResult mtdg_TaoMoiMucTieuDanhGia()
        {
            if (xulyChung.duocTruyCap(idOfPageMucTieu))
                return View();
            return null;
        }
        /// <summary>
        /// Hàm thêm mới 1 mục tiêu đánh giá vào csdl
        /// </summary>
        /// <param name="mucTieu"></param>
        /// <param name="f"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult mtdg_TaoMoiMucTieuDanhGia(mucTieuDanhGia mucTieu, FormCollection f)
        {
            if (xulyChung.duocCapNhat(idOfPageMucTieu, "7"))
            {
                string ndThongBao = "";
                try
                {
                    this.layDuLieuTuViewMucTieuDanhGia(mucTieu, f);
                    qlCaPheEntities db = new qlCaPheEntities();
                    mucTieu.trangThai = true;
                    db.mucTieuDanhGias.Add(mucTieu);
                    db.SaveChanges();
                    ndThongBao = createHTML.taoNoiDungThongBao("Mục tiêu", xulyDuLieu.traVeKyTuGoc(mucTieu.tenMucTieu), "mtdg_TableMucTieuDanhGia");
                }
                catch (Exception ex)
                {
                    ndThongBao = ex.Message;
                    xulyFile.ghiLoi("Class DanhGiaController - Function: mtdg_TaoMoiMucTieuDanhGia_Post", ex.Message);
                    this.doDuLieuLenViewMucTieuDanhGia(mucTieu);
                }
                ViewBag.ThongBao = createHTML.taoThongBaoLuu(ndThongBao);
            }
            return View();
        }
        #endregion
        #region READ

        /// <summary>
        /// Hàm định tuyến vào danh sách mục tiêu CÒN SỬ DỤNG
        /// </summary>
        /// <returns>Chuyển đến danh sách</returns>
        public ActionResult RouteTableMucTieuDanhGia()
        {
            xulyChung.addSessionUrlAction("mtdg_TableMucTieuDanhGia", "1");
            return RedirectToAction("mtdg_TableMucTieuDanhGia");
        }
        /// <summary>
        /// Hàm định tuyến vào danh sách mục tiêu KHÔNG SỬ DỤNG
        /// </summary>
        /// <returns></returns>
        public ActionResult RouteTableMucTieuDanhGiaBiHuy()
        {
            xulyChung.addSessionUrlAction("mtdg_TableMucTieuDanhGia", "2");
            return RedirectToAction("mtdg_TableMucTieuDanhGia");
        }

        /// <summary>
        /// Hàm tạo view danh sách mục tiêu còn phù hợp
        /// </summary>
        /// <returns></returns>
        public ActionResult mtdg_TableMucTieuDanhGia(int? page)
        {
            if (xulyChung.duocTruyCap(idOfPageMucTieu))
            {
                try
                {
                    int trangHienHanh = (page ?? 1);
                    string urlAction = (string)Session["urlAction"];
                    string request = urlAction.Split('|')[1]; //-------request có dạng: request=trangThai
                    request = request.Replace("request=", ""); //-------request có dạng trangThai
                    bool trangThai = request.Equals("1") ? true : false;
                    this.thietLapThongSoChung(trangThai);
                    this.createTableMucTieuDanhGia(trangThai, trangHienHanh);
                }
                catch (Exception ex)
                {
                    xulyFile.ghiLoi("Class DanhGiaController - Function: mtdg_TableMucTieuDanhGia", ex.Message);
                    return RedirectToAction("PageNotFound", "Home");
                }
            }
            return View();
        }
        /// <summary>
        /// Hàm thực hiện tạo giao diên table mục tiêu theo trạng thái
        /// </summary>
        /// <param name="trangThai"></param>
        /// <param name="trangHienHanh">Tham số trang hiện hành cần duyệt trong danh sách</param>
        private void createTableMucTieuDanhGia(bool trangThai, int trangHienHanh)
        {
            string htmlTable = "";
            try
            {
                qlCaPheEntities db = new qlCaPheEntities();
                int soPhanTu = db.mucTieuDanhGias.Where(s => s.trangThai == trangThai).Count();
                ViewBag.PhanTrang = createHTML.taoPhanTrang(soPhanTu, trangHienHanh, "/DanhGia/mtdg_TableMucTieuDanhGia"); //------cấu hình phân trang
                foreach (mucTieuDanhGia mucTieu in db.mucTieuDanhGias.ToList().Where(m => m.trangThai == trangThai).OrderBy(s => s.tenMucTieu).Skip((trangHienHanh - 1) * createHTML.pageSize).Take(createHTML.pageSize))
                {
                    htmlTable += "<tr role=\"row\" class=\"odd\">";
                    htmlTable += "       <td>" + xulyDuLieu.traVeKyTuGoc(mucTieu.tenMucTieu) + "</td>";
                    htmlTable += "       <td>" + xulyDuLieu.traVeKyTuGoc(mucTieu.dienGiai) + "</td>";
                    htmlTable += "       <td>" + xulyDuLieu.traVeKyTuGoc(mucTieu.ghiChu) + "</td>";
                    htmlTable += "       <td>";
                    htmlTable += this.taoNutChucNangDanhSach(mucTieu);
                    htmlTable += "       </td>";
                    htmlTable += "</tr>";
                }
            }
            catch (Exception ex)
            {
                xulyFile.ghiLoi("Class DanhGiaController - Function: createTableMucTieuDanhGia", ex.Message);
                Response.Redirect(xulyChung.layTenMien() + "/Home/ServerError");
            }
            ViewBag.TableData = htmlTable;
        }
        /// <summary>
        /// Hàm tạo nút chức năng trên danh mục
        /// </summary>
        /// <param name="mucTieu">Object mục tiêu để tạo nút chức năng tương ứng</param>
        /// <returns>Chuỗi html cho button Group chức năng</returns>
        private string taoNutChucNangDanhSach(mucTieuDanhGia mucTieu)
        {
            string htmlTable = "";
            htmlTable += "           <div class=\"btn-group\">";
            htmlTable += "               <button type=\"button\" class=\"btn btn-primary dropdown-toggle\" data-toggle=\"dropdown\" aria-expanded=\"true\">";
            htmlTable += "                   Chức năng <span class=\"caret\"></span>";
            htmlTable += "               </button>";
            htmlTable += "               <ul class=\"dropdown-menu\" role=\"menu\">";
            htmlTable += createTableData.taoNutChinhSua("/DanhGia/mtdg_ChinhSuaMucTieuDanhGia", mucTieu.maMucTieu.ToString());
            if (mucTieu.trangThai == true)
                htmlTable += "          <li><a task=\"" + xulyChung.taoUrlCoTruyenThamSo("/DanhGia/capNhatTrangThaiMucTieu", mucTieu.maMucTieu.ToString()) + "\" class=\"guiRequest col-orange\"><i class=\"material-icons\">done</i>Không phù hợp</a></li>";
            else
                htmlTable += "          <li><a task=\"" + xulyChung.taoUrlCoTruyenThamSo("/DanhGia/capNhatTrangThaiMucTieu", mucTieu.maMucTieu.ToString()) + "\" class=\"guiRequest col-orange\"><i class=\"material-icons\">done</i>Phù hợp</a></li>";
            htmlTable += createTableData.taoNutXoaBo(mucTieu.maMucTieu.ToString());
            htmlTable += "               </ul>";
            htmlTable += "           </div>";
            return htmlTable;
        }
        #endregion
        #region UPDATE
        /// <summary>
        /// Hàm thực hiện cập nhật trạng thái của mục tiêu
        /// Trạng thái được cập nhật sẽ ngược với trạng thái hiện tại
        /// </summary>
        public ActionResult capNhatTrangThaiMucTieu()
        {
            if (xulyChung.duocCapNhat(idOfPageMucTieu, "7"))
            {
                try
                {
                    string urlAction = (string)Session["urlAction"];
                    string request = urlAction.Split('|')[1]; //-------request có dạng: request=maDanhGia
                    request = request.Replace("request=", ""); //-------request có dạng maDanhGia
                    int maMucTieu = xulyDuLieu.doiChuoiSangInteger(request);
                    qlCaPheEntities db = new qlCaPheEntities();
                    mucTieuDanhGia mucTieuSua = db.mucTieuDanhGias.SingleOrDefault(m => m.maMucTieu == maMucTieu);
                    if (mucTieuSua != null)
                    {
                        bool trangThaiCu = (bool)mucTieuSua.trangThai; //Lưu lại trạng thái cũ để chuyển đến trang tương ứng
                        mucTieuSua.trangThai = !trangThaiCu;
                        db.Entry(mucTieuSua).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                        if (trangThaiCu)//Chuyển đến danh sách mục tiêu hợp lệ
                            Response.Redirect(xulyChung.layTenMien() + "/DanhGia/RouteTableMucTieuDanhGia");
                        else
                            Response.Redirect(xulyChung.layTenMien() + "/DanhGia/RouteTableMucTieuDanhGiaBiHuy");
                    }
                    else
                        throw new Exception("Mục tiêu đánh giá có mã " + maMucTieu.ToString() + " cần cập nhật không tồn tại");
                }
                catch (Exception ex)
                {
                    xulyFile.ghiLoi("Class: DanhGiaController - Function: capNhatTrangThaiMucTieu", ex.Message);
                    Response.Redirect(xulyChung.layTenMien() + "/Home/ServerError");
                }
            }
            return RedirectToAction("RouteTableMucTieuDanhGia");
        }
        /// <summary>
        /// Hàm thực hiện tạo giao diện chỉnh sửa mục tiêu đánh giá
        /// </summary>
        /// <returns></returns>
        public ActionResult mtdg_ChinhSuaMucTieuDanhGia()
        {
            if (xulyChung.duocTruyCap(idOfPageMucTieu))
                try
                {
                    string param = xulyChung.nhanThamSoTrongSession();
                    int maMucTieu = xulyDuLieu.doiChuoiSangInteger(param);
                    mucTieuDanhGia mucTieuSua = new qlCaPheEntities().mucTieuDanhGias.SingleOrDefault(m => m.maMucTieu == maMucTieu);
                    if (mucTieuSua != null)
                        this.doDuLieuLenViewMucTieuDanhGia(mucTieuSua);
                    else
                        throw new Exception("Mục tiêu đánh giá có mã " + maMucTieu.ToString() + " không tồn tại để cập nhật");
                }
                catch (Exception ex)
                {
                    xulyFile.ghiLoi("Class: DanhGiaController - Function: mtdg_ChinhSuaMucTieuDanhGia_Get", ex.Message);
                    return RedirectToAction("PageNotFound", "Home");
                }
            return View();
        }
        /// <summary>
        /// Hàm thực hiện chỉnh sửa 1 mục tiêu trong CSDL
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult mtdg_ChinhSuaMucTieuDanhGia(FormCollection f)
        {
            if (xulyChung.duocCapNhat(idOfPageMucTieu, "7"))
            {
                qlCaPheEntities db = new qlCaPheEntities();
                mucTieuDanhGia mucTieuSua = new mucTieuDanhGia();
                try
                {
                    int maMucTieu = Convert.ToInt32(f["txtMaMucTieu"]);
                    mucTieuSua = db.mucTieuDanhGias.SingleOrDefault(m => m.maMucTieu == maMucTieu);
                    if (mucTieuSua != null)
                    {
                        this.layDuLieuTuViewMucTieuDanhGia(mucTieuSua, f);
                        mucTieuSua.trangThai = true;
                        db.Entry(mucTieuSua).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                        return RedirectToAction("RouteTableMucTieuDanhGia");
                    }
                }
                catch (Exception ex)
                {
                    ViewBag.ThongBao = createHTML.taoThongBaoLuu(ex.Message);
                    xulyFile.ghiLoi("Class: DanhGiaController - Function: mtdg_ChinhSuaMucTieuDanhGia_Post", ex.Message);
                    ////-----Hiện lại dữ liệu trên giao diện
                    this.doDuLieuLenViewMucTieuDanhGia(mucTieuSua);
                }
            }
            return View();
        }
        #endregion
        #region DELETE
        /// <summary>
        /// Hàm thực hiện xóa 1 mục tiêu khỏi CSDL
        /// </summary>
        /// <param name="maMucTieu">Mã mục tiêu cần xóa</param>
        public void xoaMucTieuDanhGia(int maMucTieu)
        {
            if (xulyChung.duocCapNhat(idOfPageMucTieu, "7"))
                try
                {
                    qlCaPheEntities db = new qlCaPheEntities();
                    mucTieuDanhGia mucTieuXoa = db.mucTieuDanhGias.SingleOrDefault(m => m.maMucTieu == maMucTieu);
                    if (mucTieuXoa != null)
                    {
                        bool trangThai = (bool)mucTieuXoa.trangThai;//Lưu lại trạng thái để chuyển đến trang danh sách nhà cung cấp trước đó
                        db.mucTieuDanhGias.Remove(mucTieuXoa);
                        db.SaveChanges();
                        if (trangThai)//Chuyển đến danh sách mục tiêu hợp lệ
                            Response.Redirect(xulyChung.layTenMien() + "/DanhGia/RouteTableMucTieuDanhGia");
                        else
                            Response.Redirect(xulyChung.layTenMien() + "/DanhGia/RouteTableMucTieuDanhGiaBiHuy");
                    }
                    else
                        throw new Exception("Mục tiêu đánh giá có mã " + maMucTieu.ToString() + " không tồn tại để xóa");
                }
                catch (Exception ex)
                {
                    xulyFile.ghiLoi("Class: DanhGiaController - Function: xoaMucTieuDanhGia", ex.Message);
                    Response.Redirect(xulyChung.layTenMien() + "/Home/ServerError");
                }
        }
        #endregion
        #region ORTHERS
        /// <summary>
        /// Hàm nhận dữ liệu từ view mục tiêu đánh giá gán vào các thuộc tình của bảng mucTieuDanhGia
        /// </summary>
        /// <param name="mucTieu"></param>
        /// <param name="f"></param>
        private void layDuLieuTuViewMucTieuDanhGia(mucTieuDanhGia mucTieu, FormCollection f)
        {
            string loi = "";
            mucTieu.tenMucTieu = xulyDuLieu.xulyKyTuHTML(f["txtTenMucTieu"]);
            if (mucTieu.tenMucTieu.Length <= 0)
                loi += "Vui lòng nhập tên mục tiêu cần đánh giá <br/>";
            mucTieu.dienGiai = xulyDuLieu.xulyKyTuHTML(f["txtDienGiai"]);
            if (mucTieu.dienGiai.Length <= 0)
                loi += "Vui lòng nhập thông tin diễn giải cho mục tiêu <br/>";
            mucTieu.ghiChu = xulyDuLieu.xulyKyTuHTML(f["txtGhiChu"]);
            if (loi.Length > 0)
                throw new Exception(loi);
        }
        /// <summary>
        /// Hàm thực hiện đồ dữ liệu của 1 mục tiêu lên giao diện
        /// </summary>
        /// <param name="mucTieu"></param>
        private void doDuLieuLenViewMucTieuDanhGia(mucTieuDanhGia mucTieu)
        {
            ViewBag.txtMaMucTieu = mucTieu.maMucTieu.ToString();
            ViewBag.txtTenMucTieu = xulyDuLieu.traVeKyTuGoc(mucTieu.tenMucTieu);
            ViewBag.txtDienGiai = xulyDuLieu.traVeKyTuGoc(mucTieu.dienGiai);
            ViewBag.txtGhiChu = xulyDuLieu.traVeKyTuGoc(mucTieu.ghiChu);
        }
        /// <summary>
        /// Hàm thực hiện thiết lập giao diện dựa vào trạng thái
        /// </summary>
        /// <param name="trangThai">Trạng thái cho trang cần cấu hình</param>
        private void thietLapThongSoChung(bool trangThai)
        {
            //------Gán css active class ul tab danh sách
            if (trangThai)
            {
                ViewBag.Style1 = "active"; ViewBag.Style2 = "";
            }
            else
            {
                ViewBag.Style2 = "active"; ViewBag.Style1 = "";
            }
            ViewBag.ScriptAjax = createScriptAjax.scriptAjaxXoaDoiTuong("DanhGia/xoaMucTieuDanhGia?maMucTieu=");
            ViewBag.ModalXoa = createHTML.taoCanhBaoXoa("Mục tiêu đánh giá");
        }
        #endregion
        #endregion
        #region Nhóm hàm cho bảng đánh giá nhân viên
        public ActionResult DanhGiaNhanVien()
        {
            return View();
        }

        public ActionResult DanhMucDanhGia()
        {
            return View();
        }
        #endregion
    }
}