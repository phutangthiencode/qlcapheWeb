using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using qlCaPhe.App_Start;
using qlCaPhe.Models;

namespace qlCaPhe.Controllers
{
    public class PhanHoiController : Controller
    {
        private static string idOfPage = "1002";
        #region READ
        /// <summary>
        /// Hàm tạo giao diện table chờ Phản hồi
        /// </summary>
        /// <returns></returns>
        public ActionResult ph_TableChoPhanHoi(int? page)
        {
            if (xulyChung.duocTruyCap(idOfPage))
            {
                try
                {
                    qlCaPheEntities db = new qlCaPheEntities();
                    int trangHienHanh = (page ?? 1);
                    //------Lấy danh sách phản hồi chưa xem
                    int soPhanTu = db.Feedbacks.ToList().Where(f => f.trangThai == 0).Count();
                    //------cấu hình phân trang
                    ViewBag.PhanTrang = createHTML.taoPhanTrang(soPhanTu, createHTML.pageSize, trangHienHanh, "/PhanHoi/ph_TableChoPhanHoi");
                    //-----Nhúng script để xủ lý 1 số chức năng
                    this.nhungScript();
                    xulyChung.ghiNhatKyDtb(1, "Danh mục phản hồi từ khách hàng đang chờ trả lời");
                }
                catch (Exception ex)
                {
                    xulyFile.ghiLoi("Class: PhanHoiController - Function: ph_TableChoPhanHoi", ex.Message);
                    return RedirectToAction("ServerError", "Home");
                }
            }
            return View();
        }

        /// <summary>
        /// Hàm tạo giao diện table đã Phản hồi
        /// </summary>
        /// <returns></returns>
        public ActionResult ph_TableDaPhanHoi(int? page)
        {
            if (xulyChung.duocTruyCap(idOfPage))
            {
                try
                {
                    qlCaPheEntities db = new qlCaPheEntities();
                    int trangHienHanh = (page ?? 1);
                    //------Lấy danh sách phản hồi chưa xem
                    int soPhanTu = db.Feedbacks.ToList().Where(f => f.trangThai == 1).Count();
                    //------cấu hình phân trang
                    ViewBag.PhanTrang = createHTML.taoPhanTrang(soPhanTu, createHTML.pageSize, trangHienHanh, "/PhanHoi/ph_TableChoPhanHoi");
                    //-----Nhúng script để xủ lý 1 số chức năng
                    this.nhungScript();
                    xulyChung.ghiNhatKyDtb(1, "Danh mục phản hồi từ khách hàng đã trả lời");
                }
                catch (Exception ex)
                {
                    xulyFile.ghiLoi("Class: PhanHoiController - Function: ph_TableChoPhanHoi", ex.Message);
                    return RedirectToAction("ServerError", "Home");
                }
            }
            return View();
        }

        /// <summary>
        /// Hàm tạo bảng danh sách phản hồi và gán lên giao diện
        /// </summary>
        /// <param name="page">Trang hiện hành</param>
        /// <param name="trangThai">Trạng thái phản hồi cần lấy</param>
        /// <returns>PartialView vùng danh sách</returns>
        public ActionResult ph_PartTable(int? page, int? trangThai)
        {
            List<Feedback> listPhanHoi = new List<Feedback>();
            if (xulyChung.duocTruyCap(idOfPage))
            {
                try
                {
                    qlCaPheEntities db = new qlCaPheEntities();
                    int trangHienHanh = (page ?? 1);
                    listPhanHoi= db.Feedbacks.Where(f=>f.trangThai==trangThai).OrderBy(t => t.ngayDang).Skip((trangHienHanh - 1) * createHTML.pageSize).Take(createHTML.pageSize).ToList();
                }
                catch (Exception ex)
                {
                    xulyFile.ghiLoi("Class: PhanHoiController - Function: ph_TableChoPhanHoi", ex.Message);
                    return RedirectToAction("ServerError", "Home");
                }
            }
            return PartialView(listPhanHoi);
        }

        /// <summary>
        /// Hàm được ajax gọi đến để lấy thông tin chi tiết một phản hồi
        /// </summary>
        /// <param name="maFB">Mã feedback cần lấy</param>
        /// <returns>Chuỗi html tạo giao diện chứa nội dung phản hồi hiện lên model chi tiết</returns>
        public string AjaxXemChiTietFeedBack(int maFB)
        {
            string htmlDetails = "";
            if (xulyChung.duocTruyCap(idOfPage))
                try
                {
                    Feedback phanHoi = new qlCaPheEntities().Feedbacks.SingleOrDefault(f => f.maFB == maFB);
                    if (phanHoi != null)
                    {
                        htmlDetails += "<div class=\"modal-header\">";
                        htmlDetails += "      <button type=\"button\" class=\"close\" data-dismiss=\"modal\">×</button>";
                        htmlDetails += "    <h3 class=\"modal-title\" id=\"largeModalLabel\">NỘI DUNG CHI TIẾT PHẢN HỒI CỦA KHÁCH HÀNG \"" + xulyDuLieu.traVeKyTuGoc(phanHoi.tenNguoiGui) + "\"</h3>";
                        htmlDetails += "</div>";
                        htmlDetails += "<div class=\"modal-body\">";
                        htmlDetails += "    <div class=\"row\">";
                        htmlDetails += "        <div class=\"col-md-12 col-lg-12\">";
                        htmlDetails += "            <div class=\"card\">";
                        htmlDetails += "                <div class=\"header bg-cyan\">";
                        htmlDetails += "                    <h2>";
                        htmlDetails += "                        <b>" + xulyDuLieu.traVeKyTuGoc(phanHoi.tieuDe) + "</b>";
                        htmlDetails += "                    </h2>";
                        htmlDetails += "                </div>";
                        htmlDetails += "                <div class=\"body table-responsive\">";
                        htmlDetails += xulyDuLieu.traVeKyTuGoc(phanHoi.noiDung);
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
                    xulyFile.ghiLoi("Class: PhanHoiController - Function: AjaxXemChiTietFeedBack", ex.Message);
                }
            return htmlDetails;
        }
        #endregion
        #region UPDATE
        /// <summary>
        /// Hàm thực hiện cập nhật trạng thái Đã trả lời của 1 phản hồi.
        /// </summary>
        /// <returns>Trả về giao diện phàn hồi chờ trả lời</returns>
        public ActionResult capNhatTrangThai()
        {
            if (xulyChung.duocCapNhat(idOfPage, "7"))
            {
                try
                {
                    int kqLuu = 0;
                    string urlAction = xulyChung.nhanThamSoTrongSession();
                    if (urlAction.Length > 0)
                    {
                        int maFB = xulyDuLieu.doiChuoiSangInteger(urlAction);
                        qlCaPheEntities db = new qlCaPheEntities();
                        Feedback fbSua = db.Feedbacks.SingleOrDefault(f => f.maFB == maFB);
                        if (fbSua != null)
                        {
                            fbSua.trangThai = 1; //------Cập nhật trạng thái feedback đã phản hồi
                            db.Entry(fbSua).State = System.Data.Entity.EntityState.Modified;
                            kqLuu=   db.SaveChanges();
                            if (kqLuu > 0)
                                xulyChung.ghiNhatKyDtb(4, "Cập nhật trạng thái của phản hồi\" " + xulyDuLieu.traVeKyTuGoc(fbSua.tieuDe) + " \"");
                        }
                    }
                }
                catch (Exception ex)
                {
                    xulyFile.ghiLoi("Class: PhanHoiController - Function: capNhatTrangThai", ex.Message);
                }
            }
            return RedirectToAction("ph_TableChoPhanHoi");
        }
        #endregion
        #region DELETE
        /// <summary>
        /// Hàm thực hiện xóa 1 phản hồi từ người dùng
        /// </summary>
        /// <param name="maFB">Mã phản hồi cần xóa</param>
        public void xoaPhanHoi(int maFB)
        {
            if (xulyChung.duocCapNhat(idOfPage, "7"))
            {
                try
                {
                    int kqLuu = 0;
                    qlCaPheEntities db = new qlCaPheEntities();
                    Feedback fbXoa = db.Feedbacks.SingleOrDefault(f => f.maFB == maFB);
                    if (fbXoa != null)
                    {
                        db.Feedbacks.Remove(fbXoa);
                        kqLuu = db.SaveChanges();
                        if(kqLuu>0)
                            xulyChung.ghiNhatKyDtb(3, "Phản hồi \"" + xulyDuLieu.traVeKyTuGoc(fbXoa.tieuDe) + " \"");
                    }
                }
                catch (Exception ex)
                {
                    xulyFile.ghiLoi("Class: PhanHoiController - Function: capNhatTrangThai", ex.Message);
                }
            }
        }
        #endregion
        /// <summary>
        /// Hàm nhúng các đoạn script, html cần thiết lên trang
        /// Script xóa phản hồi, Scrip Ajax xem chi tiết phản hồi
        /// Html Model xóa phản hồi, Modal chứa nội dung chi tiết của phản hồi
        /// </summary>
        private void nhungScript()
        {
            //------Nhúng script xóa feedback
            ViewBag.ScriptAjaxXoaFeedBack = createScriptAjax.scriptAjaxXoaDoiTuong("PhanHoi/xoaPhanHoi?maFB=");
            ViewBag.ModalXoa = createHTML.taoCanhBaoXoa("Phản hồi từ khách hàng");
            //---Script hiện modal chi tiết công thức
            ViewBag.ScriptAjaxXemChiTiet = createScriptAjax.scriptAjaxXemChiTietKhiClick("js-xem-chitiet", "maFB", "PhanHoi/AjaxXemChiTietFeedBack?maFB=", "vungChiTiet", "modalChiTiet");
            //-----Tạo modal dạng lớn để chứa chi tiết nội dung của feedback
            ViewBag.ModalChiTietFeedBack = createHTML.taoModalChiTiet("modalChiTiet", "vungChiTiet", 3);
        }

	}
}