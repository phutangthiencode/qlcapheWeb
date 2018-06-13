using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using qlCaPhe.App_Start;
using qlCaPhe.Models;

namespace qlCaPhe.Controllers
{
    public class LoaiNguyenLieuController : Controller
    {
        private string idOfPage = "701";
        #region CREATE
        /// <summary>
        /// Hàm tạo giao diện tạo mới loại nguyên liệu
        /// </summary>
        /// <returns></returns>
        public ActionResult lnl_TaoMoiLoaiNguyenLieu()
        {
            if (xulyChung.duocTruyCap(idOfPage))
            {
                xulyChung.ghiNhatKyDtb(1, "Tạo mới loại nguyên liệu");
                return View();
            }
            return null;
        }
        /// <summary>
        /// Hàm thực hiện thêm mới một loại nguyên liệu vào CSDL
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult lnl_TaoMoiLoaiNguyenLieu(loaiNguyenLieu loai, FormCollection f)
        {
            if (xulyChung.duocCapNhat(idOfPage, "7"))
            {
                string ndThongBao = ""; int kqLuu = 0;
                try
                {
                    qlCaPheEntities db = new qlCaPheEntities();
                    this.layDuLieuTuView(loai, f);
                    this.kiemTraTenLoaiTrung(loai.tenLoai, db);
                    db.loaiNguyenLieux.Add(loai);
                    kqLuu=db.SaveChanges();
                    if (kqLuu > 0)
                    {
                        ndThongBao = createHTML.taoNoiDungThongBao("Loại nguyên liệu", xulyDuLieu.traVeKyTuGoc(loai.tenLoai), "lnl_TableLoaiNguyenLieu");
                        xulyChung.ghiNhatKyDtb(2, ndThongBao);
                    }
                }
                catch (Exception ex)
                {
                    ndThongBao = ex.Message;
                    xulyFile.ghiLoi("Class LoaiNguyenLieuController - Function: lnl_TaoMoiLoaiNguyenLieu_Post", ex.Message);
                    this.doDuLieuLenView(loai);
                }
                ViewBag.ThongBao = createHTML.taoThongBaoLuu(ndThongBao);
            }
            return View();
        }
        #endregion
        #region READ

        /// <summary>
        /// Hàm thực hiện tạo giao diện loại nguyên liệu
        /// </summary>
        /// <returns></returns>
        public ActionResult lnl_TableLoaiNguyenLieu(int ?page)
        {
            int trangHienHanh = (page ?? 1);
            if (xulyChung.duocTruyCap(idOfPage))
            {
                string htmlTable = "";
                try
                {
                    qlCaPheEntities db = new qlCaPheEntities();
                    int soPhanTu = db.loaiNguyenLieux.Count();
                    ViewBag.PhanTrang = createHTML.taoPhanTrang(soPhanTu, trangHienHanh, "/LoaiNguyenLieu/lnl_TableLoaiNguyenLieu"); //------cấu hình phân trang
                    foreach (loaiNguyenLieu loai in db.loaiNguyenLieux.OrderBy(l => l.tenLoai).Skip((trangHienHanh - 1) * createHTML.pageSize).Take(createHTML.pageSize).ToList())
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
                        htmlTable += createTableData.taoNutChinhSua("/LoaiNguyenLieu/lnl_ChinhSuaLoaiNguyenLieu", loai.maLoai.ToString());
                        htmlTable += createTableData.taoNutXoaBo(loai.maLoai.ToString());
                        htmlTable += "              </ul>";
                        htmlTable += "          </div>";
                        htmlTable += "      </td>";
                        htmlTable += "</tr>";
                    }
                    ViewBag.ScriptAjax = createScriptAjax.scriptAjaxXoaDoiTuong("LoaiNguyenLieu/xoaLoaiNguyenLieu?maLoai=");
                    ViewBag.ModalXoa = createHTML.taoCanhBaoXoa("Loại nguyên liệu");
                    ViewBag.TableData = htmlTable;
                    xulyChung.ghiNhatKyDtb(1, "Danh mục loại nguyên liệu");
                }
                catch (Exception ex)
                {
                    xulyFile.ghiLoi("Class LoaiNguyenLieuController - Function: lnl_TableLoaiNguyenLieu", ex.Message);
                    return RedirectToAction("ServerError", "Home");
                }
            }
            return View();
        }
        #endregion
        #region UPDATE
        /// <summary>
        /// Hàm thực hiện tạo giao diện chỉnh sửa thông tin loại nguyên liệu
        /// </summary>
        /// <param name="maLoai"></param>
        /// <returns></returns>
        public ActionResult lnl_ChinhSuaLoaiNguyenLieu()
        {
            if (xulyChung.duocCapNhat(idOfPage, "7"))
            {
                try
                {
                    string param = xulyChung.nhanThamSoTrongSession();
                    if (param.Length > 0)
                    {
                        int maLoai = xulyDuLieu.doiChuoiSangInteger(param);
                        loaiNguyenLieu loaiSua = new qlCaPheEntities().loaiNguyenLieux.SingleOrDefault(l => l.maLoai == maLoai);
                        if (loaiSua != null)
                        {
                            this.doDuLieuLenView(loaiSua);
                            xulyChung.ghiNhatKyDtb(1, "Chỉnh sửa loại nguyên liệu\" " + xulyDuLieu.traVeKyTuGoc(loaiSua.tenLoai) + " \"");
                        }
                        else
                            throw new Exception("Loại nguyên liệu có mã " + maLoai.ToString() + " không tồn tại để cập nhật");
                    }
                    else throw new Exception("không nhận được tham số");
                }
                catch (Exception ex)
                {
                    xulyFile.ghiLoi("Class LoaiNguyenLieuController - Function: lnl_ChinhSuaLoaiNguyenLieu_Get", ex.Message);
                    return RedirectToAction("PageNotFound", "Home");
                }
            }
            return View();
        }

        /// <summary>
        /// Hàm thực hiện chỉnh sửa thông tin loại nguyên liệu trong csdl
        /// </summary>
        /// <param name="maLoai"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult lnl_ChinhSuaLoaiNguyenLieu(FormCollection f)
        {
            if (xulyChung.duocCapNhat(idOfPage, "7"))
            {
                string ndThongBao = ""; int kqLuu = 0;
                loaiNguyenLieu loaiSua = new loaiNguyenLieu();
                try
                {
                    int maLoai = xulyDuLieu.doiChuoiSangInteger(f["txtMaLoai"]);
                    qlCaPheEntities db = new qlCaPheEntities();
                    loaiSua = db.loaiNguyenLieux.SingleOrDefault(l => l.maLoai == maLoai);
                    if (loaiSua != null)
                    {
                        this.layDuLieuTuView(loaiSua, f);
                        db.Entry(loaiSua).State = System.Data.Entity.EntityState.Modified;
                        kqLuu=db.SaveChanges();
                        if (kqLuu > 0)
                        {
                            xulyChung.ghiNhatKyDtb(4, "Loại nguyên liệu \" " + xulyDuLieu.traVeKyTuGoc(loaiSua.tenLoai) + " \"");
                            return RedirectToAction("lnl_TableLoaiNguyenLieu");
                        }
                    }
                }
                catch (Exception ex)
                {
                    ndThongBao = ex.Message;
                    this.doDuLieuLenView(loaiSua);
                    xulyFile.ghiLoi("Class LoaiNguyenLieuController - Function: lnl_ChinhSuaLoaiNguyenLieu_Post", ex.Message);
                }
                ViewBag.ThongBao = createHTML.taoThongBaoLuu(ndThongBao);
            }
            return View();
        }
        #endregion
        #region DELETE
        /// <summary>
        /// Hàm thực hiện xóa bỏ 1 loại nguyên liệu khỏi csdl
        /// </summary>
        /// <param name="maLoai"></param>
        public void xoaLoaiNguyenLieu(int maLoai)
        {
            try
            {
                qlCaPheEntities db = new qlCaPheEntities(); int kqLuu = 0;
                loaiNguyenLieu loaiXoa = db.loaiNguyenLieux.Single(l => l.maLoai == maLoai);
                if (loaiXoa != null)
                {
                    db.loaiNguyenLieux.Remove(loaiXoa);
                    kqLuu=db.SaveChanges();
                    if(kqLuu>0)
                        xulyChung.ghiNhatKyDtb(3, "Loại nguyên liệu\"" + xulyDuLieu.traVeKyTuGoc(loaiXoa.tenLoai) + " \"");
                }
                else
                    throw new Exception("Loại nguyên liệu có mã " + maLoai.ToString() + " không tồn tại để xóa");
            }
            catch (Exception ex)
            {
                xulyFile.ghiLoi("Class: LoaiNguyenLieuController - Function: xoaLoaiNguyenLieu", ex.Message);
                Response.Redirect(xulyChung.layTenMien() + "/Home/PageNotFound");
            }
        }
        #endregion
        #region ORTHERS
        /// <summary>
        /// Hàm thực hiện lấy dữ liệu từ giao diện và gán vào các thuộc tính của LoainguyenLieu
        /// </summary>
        /// <param name="loai"></param>
        /// <param name="f"></param>
        private void layDuLieuTuView(loaiNguyenLieu loai, FormCollection f)
        {
            string loi = "";
            loai.tenLoai = xulyDuLieu.xulyKyTuHTML(f["txtTenLoai"]);
            if (loai.tenLoai.Length <= 0)
                loi += "Vui lòng nhập tên loại<br/>";
            loai.dienGiai = xulyDuLieu.xulyKyTuHTML(f["txtDienGiai"]);
            if (loai.dienGiai.Length <= 0)
                loi += "Vui lòng nhập diễn giải cho loại";
            loai.ghiChu = xulyDuLieu.xulyKyTuHTML(f["txtGhiChu"]);
            if (loi.Length > 0)
                throw new Exception(loi);
        }
        private void kiemTraTenLoaiTrung(string tenLoai, qlCaPheEntities db)
        {
            try
            {
                loaiNguyenLieu loaiTrung = db.loaiNguyenLieux.SingleOrDefault(l => l.tenLoai.Equals(tenLoai));
                if (loaiTrung != null)
                    throw new Exception("Loại <b>" + tenLoai + "</b> đã tồn tại, vui lòng nhập tên loại khác");
            }
            catch (Exception ex)
            {
                xulyFile.ghiLoi("Class LoaiNguyenLieuController - Function: kiemTraTenLoaiTrung", ex.Message);
                throw new Exception(ex.Message);
            }
        }
        /// <summary>
        /// Hàm thực hiện đổ dữ liệu của loaiNguyenLieu lên giao diện
        /// </summary>
        /// <param name="loai"></param>
        private void doDuLieuLenView(loaiNguyenLieu loai)
        {
            ViewBag.txtMaLoai = loai.maLoai.ToString();
            ViewBag.txtTenLoai = xulyDuLieu.traVeKyTuGoc(loai.tenLoai);
            ViewBag.txtDienGiai = xulyDuLieu.traVeKyTuGoc(loai.dienGiai);
            ViewBag.txtGhiChu = xulyDuLieu.traVeKyTuGoc(loai.ghiChu);
        }

        #endregion
    }
}