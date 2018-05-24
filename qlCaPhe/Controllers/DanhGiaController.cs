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
        #region NHÓM HÀM CHO BẢNG MỤC TIÊU ĐÁNH GIÁ
        #region CREATE
        /// <summary>
        /// Hàm tạo giao diện tạo mới mục tiêu đánh giá
        /// </summary>
        /// <returns></returns>
        public ActionResult mtdg_TaoMoiMucTieuDanhGia()
        {
            return View();
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
            return View();
        }
        #endregion
        #region READ
        /// <summary>
        /// Hàm tạo view danh sách mục tiêu còn phù hợp
        /// </summary>
        /// <returns></returns>
        public ActionResult mtdg_TableMucTieuDanhGia()
        {
            this.createTableMucTieuDanhGia(true);
            return View();
        }
        /// <summary>
        /// Hàm thực hiện tạo view danh sách mục tiêu không còn phù hợp - Bị hủy
        /// </summary>
        /// <returns></returns>
        public ActionResult mtdg_TableMucTieuDanhGiaHuy()
        {
            this.createTableMucTieuDanhGia(false);
            return View();
        }
        /// <summary>
        /// Hàm thực hiện tạo giao diên table mục tiêu theo trạng thái
        /// </summary>
        /// <param name="trangThai"></param>
        private void createTableMucTieuDanhGia(bool trangThai)
        {
            string htmlTable = "";
            try
            {
                foreach (mucTieuDanhGia mucTieu in new qlCaPheEntities().mucTieuDanhGias.ToList().Where(m => m.trangThai == trangThai))
                {
                    htmlTable+= "<tr role=\"row\" class=\"odd\">";
                    htmlTable+= "       <td>"+xulyDuLieu.traVeKyTuGoc(mucTieu.tenMucTieu)+"</td>";
                    htmlTable += "       <td>" + xulyDuLieu.traVeKyTuGoc(mucTieu.dienGiai) + "</td>";
                    htmlTable += "       <td>" + xulyDuLieu.traVeKyTuGoc(mucTieu.ghiChu) + "</td>";
                    htmlTable+= "       <td>";
                    htmlTable+= "           <div class=\"btn-group\">";
                    htmlTable+= "               <button type=\"button\" class=\"btn btn-primary dropdown-toggle\" data-toggle=\"dropdown\" aria-expanded=\"true\">";
                    htmlTable+= "                   Chức năng <span class=\"caret\"></span>";
                    htmlTable+= "               </button>";
                    htmlTable+= "               <ul class=\"dropdown-menu\" role=\"menu\">";
                    htmlTable+= "                   <li><a href=\"/DanhGia/mtdg_ChinhSuaMucTieuDanhGia?maMucTieu="+mucTieu.maMucTieu.ToString()+"\" class=\"col-blue waves-effect waves-block\"><i class=\"material-icons\">mode_edit</i>Chỉnh sửa</a></li>";
                    if(mucTieu.trangThai==true)
                        htmlTable+= "                   <li><a href=\"/DanhGia/capNhatTrangThaiMucTieu?maMucTieu="+mucTieu.maMucTieu.ToString()+"\" class=\"col-orange waves-effect waves-block\"><i class=\"material-icons\">clear</i>Không phù hợp</a></li>";
                    else
                        htmlTable += "                   <li><a href=\"/DanhGia/capNhatTrangThaiMucTieu?maMucTieu="+mucTieu.maMucTieu.ToString()+"\" class=\"col-orange waves-effect waves-block\"><i class=\"material-icons\">clear</i>Phù hợp</a></li>";
                    htmlTable += "                   <li class=\"xoa\" maXoa=\"" + mucTieu.maMucTieu.ToString() + "\"><a href=\"#\" class=\"col-red waves-effect waves-block\"><i class=\"material-icons\">delete</i>Xoá bỏ</a></li>";
                    htmlTable+= "               </ul>";
                    htmlTable+= "           </div>";
                    htmlTable+= "       </td>";
                    htmlTable+= "</tr>";
                }
            }
            catch (Exception ex)
            {
                xulyFile.ghiLoi("Class DanhGiaController - Function: createTableMucTieuDanhGia", ex.Message);
                Response.Redirect(xulyChung.layTenMien() + "/Home/ServerError");
            }
            ViewBag.TableData = htmlTable;
            ViewBag.ScriptAjax = createScriptAjax.scriptAjaxXoaDoiTuong("DanhGia/xoaMucTieuDanhGia?maMucTieu=");
            ViewBag.ModalXoa = createHTML.taoCanhBaoXoa("Mục tiêu đánh giá");
        }
        #endregion
        #region UPDATE        
        /// <summary>
        /// Hàm thực hiện cập nhật trạng thái của mục tiêu
        /// Trạng thái được cập nhật sẽ ngược với trạng thái hiện tại
        /// </summary>
        /// <param name="maMucTieu"></param>
        public void capNhatTrangThaiMucTieu(int maMucTieu)
        {
            try
            {
                qlCaPheEntities db = new qlCaPheEntities();
                mucTieuDanhGia mucTieuSua = db.mucTieuDanhGias.SingleOrDefault(m => m.maMucTieu == maMucTieu);
                if (mucTieuSua != null)
                {
                    bool trangThaiCu = (bool)mucTieuSua.trangThai; //Lưu lại trạng thái cũ để chuyển đến trang tương ứng
                    mucTieuSua.trangThai = !trangThaiCu;
                    db.Entry(mucTieuSua).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    if (trangThaiCu)//Chuyển đến danh sách mục tiêu hợp lệ
                        Response.Redirect(xulyChung.layTenMien() + "/DanhGia/mtdg_TableMucTieuDanhGia");
                    else
                        Response.Redirect(xulyChung.layTenMien() + "/DanhGia/mtdg_TableMucTieuDanhGiaHuy");
                }
                else
                    throw new Exception("Mục tiêu đánh giá có mã " +maMucTieu.ToString() + " cần cập nhật không tồn tại");
            }
            catch (Exception ex)
            {
                xulyFile.ghiLoi("Class: DanhGiaController - Function: capNhatTrangThaiMucTieu", ex.Message);
                Response.Redirect(xulyChung.layTenMien() + "/Home/ServerError");
            }
        }
        /// <summary>
        /// Hàm thực hiện tạo giao diện chỉnh sửa mục tiêu đánh giá
        /// </summary>
        /// <param name="maMucTieu"></param>
        /// <returns></returns>
        public ActionResult mtdg_ChinhSuaMucTieuDanhGia(int maMucTieu)
        {
            try
            {
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
                    return RedirectToAction("mtdg_TableMucTieuDanhGia");
                }
            }
            catch (Exception ex)
            {
                ViewBag.ThongBao = createHTML.taoThongBaoLuu(ex.Message);
                xulyFile.ghiLoi("Class: DanhGiaController - Function: mtdg_ChinhSuaMucTieuDanhGia_Post", ex.Message);
                ////-----Hiện lại dữ liệu trên giao diện
                this.doDuLieuLenViewMucTieuDanhGia(mucTieuSua);
            }
            return View();
        }
        #endregion
        #region DELETE
        /// <summary>
        /// Hàm thực hiện xóa 1 mục tiêu khỏi CSDL
        /// </summary>
        /// <param name="maMucTieu"></param>
        public void xoaMucTieuDanhGia(int maMucTieu)
        {
            try
            {
                qlCaPheEntities db = new qlCaPheEntities();
                mucTieuDanhGia mucTieuXoa = db.mucTieuDanhGias.SingleOrDefault(m => m.maMucTieu == maMucTieu);
                if (mucTieuXoa != null)
                {
                    bool trangThai = (bool) mucTieuXoa.trangThai;//Lưu lại trạng thái để chuyển đến trang danh sách nhà cung cấp trước đó
                    db.mucTieuDanhGias.Remove(mucTieuXoa);
                    db.SaveChanges();
                    if (trangThai)//Chuyển đến danh sách mục tiêu hợp lệ
                        Response.Redirect(xulyChung.layTenMien() + "/DanhGia/mtdg_TableMucTieuDanhGia");
                    else
                        Response.Redirect(xulyChung.layTenMien() + "/DanhGia/mtdg_TableMucTieuDanhGiaHuy");
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