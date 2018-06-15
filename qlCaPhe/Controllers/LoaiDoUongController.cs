using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using qlCaPhe.App_Start;
using qlCaPhe.Models;

namespace qlCaPhe.Controllers
{
    public class LoaiDoUongController : Controller
    {
        private static string idOfPage = "401";
        #region CREATE
        /// <summary>
        /// Hàm thực hiện tạo view loại đồ uống
        /// </summary>
        /// <returns></returns>
        public ActionResult ldu_TaoMoiLoaiDoUong()
        {
            if (xulyChung.duocTruyCap(idOfPage))
            {
                xulyChung.ghiNhatKyDtb(1, "Tạo mới loại đồ uống");
                return View();
            }
            return null;
        }
        /// <summary>
        /// Hàm thực hiện thêm mới loại đồ uống vào CSDL
        /// </summary>
        /// <param name="loai"></param>
        /// <param name="f"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ldu_TaoMoiLoaiDoUong(loaiSanPham loai, FormCollection f)
        {
            if (xulyChung.duocCapNhat(idOfPage, "7"))
            {
                string ndThongBao = ""; int kqLuu = 0;
                try
                {
                    qlCaPheEntities db = new qlCaPheEntities();
                    this.layDuLieuTuView(loai, f);
                    db.loaiSanPhams.Add(loai);
                    kqLuu = db.SaveChanges();
                    if (kqLuu > 0)
                    {
                        ndThongBao = createHTML.taoNoiDungThongBao("Loại đồ uống", xulyDuLieu.traVeKyTuGoc(loai.tenLoai), "ldu_TableLoaiDoUong");
                        xulyChung.ghiNhatKyDtb(2, " Loại bài viết \" " + xulyDuLieu.traVeKyTuGoc(loai.tenLoai) + " \"");
                    }
                }
                catch (Exception ex)
                {
                    ndThongBao = ex.Message;
                    this.doDuLieuLienView(loai);
                    xulyFile.ghiLoi("Class LoaiDoUongController - Function: ldu_TaoMoiLoaiDoUongPost", ex.Message);
                }
                ViewBag.ThongBao = createHTML.taoThongBaoLuu(ndThongBao);
            }
            return View();
        }
        #endregion
        #region READ
        /// <summary>
        /// Hàm thực hiện tạo giao diện danh mục loại đồ uống
        /// </summary>
        /// <returns></returns>
        public ActionResult ldu_TableLoaiDoUong(int? page)
        {
            int trangHienHanh = (page ?? 1);
            if (xulyChung.duocTruyCap(idOfPage))
            {
                string htmlTable = "";
                try
                {
                    qlCaPheEntities db = new qlCaPheEntities();
                    int soPhanTu = db.loaiSanPhams.Count();
                    ViewBag.PhanTrang = createHTML.taoPhanTrang(soPhanTu, createHTML.pageSize, trangHienHanh, "/LoaiDoUong/ldu_TableLoaiDoUong"); //------cấu hình phân trang
                    foreach (loaiSanPham loai in db.loaiSanPhams.OrderBy(c => c.tenLoai).Skip((trangHienHanh - 1) * createHTML.pageSize).Take(createHTML.pageSize).ToList())
                    {
                        htmlTable += "<tr role=\"row\" class=\"odd\">";
                        htmlTable += "      <td>" + xulyDuLieu.traVeKyTuGoc(loai.tenLoai) + "</td>";
                        htmlTable += "      <td>" + xulyDuLieu.traVeKyTuGoc(loai.dienGiai) + "</td>";
                        htmlTable += "      <td>" + xulyDuLieu.traVeKyTuGoc(loai.ghiChu) + "</td>";
                        htmlTable += "      <td>";
                        htmlTable += "          <div class=\"btn-group\">";
                        htmlTable += "              <button type=\"button\" class=\"btn btn-primary dropdown-toggle\" data-toggle=\"dropdown\" aria-expanded=\"true\">";
                        htmlTable += "                  Chức năng <span class=\"caret\"></span>";
                        htmlTable += "              </button>";
                        htmlTable += "              <ul class=\"dropdown-menu\" role=\"menu\">";
                        htmlTable += createTableData.taoNutChinhSua("/LoaiDoUong/ldu_ChinhSuaLoaiDoUong", loai.maLoai.ToString());
                        htmlTable += createTableData.taoNutXoaBo(loai.maLoai.ToString());
                        htmlTable += "              </ul>";
                        htmlTable += "          </div>";
                        htmlTable += "      </td>";
                        htmlTable += "</tr>";
                    }
                }
                catch (Exception ex)
                {
                    xulyFile.ghiLoi("Class LoaiDoUongController - Function: ldu_TableLoaiDoUong", ex.Message);
                    return RedirectToAction("ServerError", "Home");
                }
                ViewBag.TableData = htmlTable;
                ViewBag.ScriptAjax = createScriptAjax.scriptAjaxXoaDoiTuong("LoaiDoUong/xoaLoaiDoUong?maLoai=");
                ViewBag.ModalXoa = createHTML.taoCanhBaoXoa("Loại đồ uống");
                xulyChung.ghiNhatKyDtb(1, "Danh mục loại đồ uống");
            }
            return View();
        }
        #endregion
        #region UPDATE
        /// <summary>
        /// Hàm thực hiện tạo giao diện chỉnh sửa loại đồ uống
        /// </summary>
        /// <param name="maLoai"></param>
        /// <returns></returns>
        public ActionResult ldu_ChinhSuaLoaiDoUong()
        {
            if (xulyChung.duocCapNhat(idOfPage, "7"))
            {
                try
                {
                    string param = xulyChung.nhanThamSoTrongSession();
                    if (param.Length > 0)
                    {
                        int maLoai = xulyDuLieu.doiChuoiSangInteger(param);
                        loaiSanPham loai = new qlCaPheEntities().loaiSanPhams.SingleOrDefault(l => l.maLoai == maLoai);
                        if (loai != null)
                        {
                            this.doDuLieuLienView(loai);
                            xulyChung.ghiNhatKyDtb(1, "Chỉnh sửa loại đồ uống\" " + xulyDuLieu.traVeKyTuGoc(loai.tenLoai) + " \"");
                        }
                        else
                            throw new Exception("Loại sản phẩm có mã " + maLoai.ToString() + " không tồn tại để cập nhật");
                    }
                    else throw new Exception("không nhận được tham số");
                }
                catch (Exception ex)
                {
                    xulyFile.ghiLoi("Class: LoaiDoUongController - Function: ldu_ChinhSuaLoaiDoUongGet", ex.Message);
                    return RedirectToAction("PageNotFound", "Home");
                }
            }
            return View();
        }
        /// <summary>
        /// Hàm thực hiện cập nhật thông tin loại đồ uống trong CSDL
        /// </summary>
        /// <param name="maLoai"></param>
        /// <param name="f"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ldu_ChinhSuaLoaiDoUong(FormCollection f)
        {
            if (xulyChung.duocCapNhat(idOfPage, "7"))
            {
                string ndThongBao = ""; int kqLuu = 0;
                loaiSanPham loaiSua = new loaiSanPham();
                try
                {
                    int maLoai = xulyDuLieu.doiChuoiSangInteger(f["txtMaLoai"]);
                    qlCaPheEntities db = new qlCaPheEntities();
                    loaiSua = db.loaiSanPhams.SingleOrDefault(l => l.maLoai == maLoai);
                    if (loaiSua != null)
                    {
                        this.layDuLieuTuView(loaiSua, f);
                        db.Entry(loaiSua).State = System.Data.Entity.EntityState.Modified;
                        kqLuu = db.SaveChanges();
                        if (kqLuu > 0)
                        {
                            xulyChung.ghiNhatKyDtb(4, " Loại bài viết \" " + xulyDuLieu.traVeKyTuGoc(loaiSua.tenLoai) + " \"");
                            return RedirectToAction("ldu_TableLoaiDoUong");
                        }
                    }
                }
                catch (Exception ex)
                {
                    xulyFile.ghiLoi("Class: LoaiDoUongController - Function: ldu_ChinhSuaLoaiDoUongGet", ex.Message);
                    ndThongBao = ex.Message;
                    this.doDuLieuLienView(loaiSua);
                }
                ViewBag.ThongBao = createHTML.taoThongBaoLuu(ndThongBao);
            }
            return View();
        }
        #endregion
        #region DELETE
        /// <summary>
        /// Hàm thực hiện xóa 1 loại sản phẩm khỏi CSDL
        /// </summary>
        /// <param name="maLoai"></param>
        public void xoaLoaiDoUong(int maLoai)
        {
            try
            {
                int kqLuu = 0;
                qlCaPheEntities db = new qlCaPheEntities();
                loaiSanPham loaiXoa = db.loaiSanPhams.SingleOrDefault(l => l.maLoai == maLoai);
                if (loaiXoa != null)
                {
                    db.loaiSanPhams.Remove(loaiXoa);
                    kqLuu = db.SaveChanges();
                    if (kqLuu > 0)
                        xulyChung.ghiNhatKyDtb(3, "Loại sản phẩm \"" + xulyDuLieu.traVeKyTuGoc(loaiXoa.tenLoai) + " \"");
                }
                else
                    throw new Exception("Loại sản phẩm có mã " + maLoai.ToString() + " không tồn tại trong hệ thống");
            }
            catch (Exception ex)
            {
                xulyFile.ghiLoi("Class: LoaiDoUongController - Function: xoaLoaiDoUong", ex.Message);
            }
        }
        #endregion
        #region ORTHERS
        /// <summary>
        /// Hàm thực hiện lấy dữ liệu từ giao diện để gán vào các thuộc tính cho loaiSanPham
        /// </summary>
        /// <param name="loai"></param>
        /// <param name="f"></param>
        private void layDuLieuTuView(loaiSanPham loai, FormCollection f)
        {
            string loi = "";
            loai.tenLoai = xulyDuLieu.xulyKyTuHTML(f["txtTenLoai"]);
            if (loai.tenLoai.Length <= 0)
                loi += "Vui lòng nhập tên loại sản phẩm <br/>";
            loai.dienGiai = xulyDuLieu.xulyKyTuHTML(f["txtDienGiai"]);
            loai.ghiChu = xulyDuLieu.xulyKyTuHTML(f["txtGhiChu"]);
        }
        /// <summary>
        /// Hàm thực hiện đổ dữ liệu của loại sản phẩm lên  giao diện
        /// </summary>
        /// <param name="loai"></param>
        private void doDuLieuLienView(loaiSanPham loai)
        {
            ViewBag.txtMaLoai = loai.maLoai.ToString();
            ViewBag.txtTenLoai = xulyDuLieu.traVeKyTuGoc(loai.tenLoai);
            ViewBag.txtDienGiai = xulyDuLieu.traVeKyTuGoc(loai.dienGiai);
            ViewBag.txtGhiChu = xulyDuLieu.traVeKyTuGoc(loai.ghiChu);
        }
        #endregion
    }
}