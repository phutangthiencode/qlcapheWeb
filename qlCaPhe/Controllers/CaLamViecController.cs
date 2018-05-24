using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using qlCaPhe.App_Start;
using qlCaPhe.Models;

namespace qlCaPhe.Controllers
{
    public class CaLamViecController : Controller
    {
        private string idOfPage = "901";
        #region CREATE
        /// <summary>
        /// Hàm tạo giao diện thêm mới ca làm việc
        /// </summary>
        /// <returns></returns>
        public ActionResult ca_TaoMoiCaLamViec()
        {
            if (xulyChung.duocTruyCap(idOfPage))
                this.taoDuLieuChoCbb();
            return View();
        }
        /// <summary>
        /// Hàm thêm mới ca làm việc vào CSDL
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ca_TaoMoiCaLamViec(caLamViec ca, FormCollection f)
        {
            if (xulyChung.duocCapNhat(idOfPage, "7"))
            {
                string ndThongBao = "";
                try
                {
                    this.taoDuLieuChoCbb();
                    this.layDuLieuTuView(ca, f);
                    qlCaPheEntities db = new qlCaPheEntities();
                    db.caLamViecs.Add(ca);
                    db.SaveChanges();
                    ndThongBao = createHTML.taoNoiDungThongBao("Ca làm việc", xulyDuLieu.traVeKyTuGoc(ca.tenCa), "ca_TableCaLamViec");
                }
                catch (Exception ex)
                {
                    ndThongBao = ex.Message;
                    xulyFile.ghiLoi("Class CaLamViecController - Function:ca_TaoMoiCaLamViec_Post", ex.Message);
                    this.doDuLieuLenView(ca);
                }
                ViewBag.ThongBao = createHTML.taoThongBaoLuu(ndThongBao);
            }
            return View();
        }
        #endregion
        #region READ
        /// <summary>
        /// hàm tạo bảng ca làm việc
        /// </summary>
        /// <returns></returns>
        public ActionResult ca_TableCaLamViec()
        {
            if (xulyChung.duocCapNhat(idOfPage, "7"))
            {
                string htmlTable = "";
                try
                {
                    foreach (caLamViec ca in new qlCaPheEntities().caLamViecs.ToList().OrderBy(c => c.tenCa))
                    {
                        htmlTable += "<tr role=\"row\" class=\"odd\">";
                        htmlTable += "      <td>" + xulyDuLieu.traVeKyTuGoc(ca.tenCa) + "</td>";
                        htmlTable += "      <td>";
                        if (ca.buoi == 1) htmlTable += "Sáng";
                        else if (ca.buoi == 2) htmlTable += "Chiều";
                        else if (ca.buoi == 3) htmlTable += "Tối";
                        htmlTable += "    </td>";
                        htmlTable += "      <td>" + string.Format("{0:hh:mm}", ca.batDau.ToString()) + " - " + string.Format("{0:hh:mm}", ca.ketThuc.ToString()) + "</td>";
                        htmlTable += "      <td>" + xulyDuLieu.traVeKyTuGoc(ca.ghiChu) + "</td>";
                        htmlTable += "      <td>";
                        htmlTable += "          <div class=\"btn-group\">";
                        htmlTable += "              <button type=\"button\" class=\"btn btn-primary dropdown-toggle\" data-toggle=\"dropdown\">";
                        htmlTable += "                  Chức năng <span class=\"caret\"></span>";
                        htmlTable += "              </button>";
                        htmlTable += "              <ul class=\"dropdown-menu\" role=\"menu\">";
                        htmlTable += createTableData.taoNutChinhSua("/CaLamViec/ca_ChinhSuaCaLamViec", ca.maCa.ToString());
                        htmlTable += createTableData.taoNutXoaBo(ca.maCa.ToString());
                        htmlTable += "              </ul>";
                        htmlTable += "          </div>";
                        htmlTable += "      </td>";
                        htmlTable += "</tr>";
                    }
                }
                catch (Exception ex)
                {
                    xulyFile.ghiLoi("Class CaLamViecController - Function:ca_TableCaLamViec", ex.Message);
                    return RedirectToAction("ServerError", "Home");
                }
                ViewBag.TableData = htmlTable;
                ViewBag.ScriptAjax = createScriptAjax.scriptAjaxXoaDoiTuong("CaLamViec/xoaCaLamViec?maCa=");
                ViewBag.ModalXoa = createHTML.taoCanhBaoXoa("Ca làm việc");
            }
            return View();
        }
        #endregion
        #region UPDATE
        /// <summary>
        /// Hàm tạo giao diện chỉnh sửa ca làm việc
        /// </summary>
        /// <param name="maCa"></param>
        /// <returns></returns>
        public ActionResult ca_ChinhSuaCaLamViec()
        {
            if (xulyChung.duocCapNhat(idOfPage, "7"))
                try
                {
                    string param = xulyChung.nhanThamSoTrongSession();
                    if (param.Length > 0)
                    {
                        int maCa = xulyDuLieu.doiChuoiSangInteger(param);
                        caLamViec caSua = new qlCaPheEntities().caLamViecs.SingleOrDefault(c => c.maCa == maCa);
                        if (caSua != null)
                            this.doDuLieuLenView(caSua);
                        else
                            return RedirectToAction("PageNotFound", "Home");
                    }
                    else
                        throw new Exception("không nhận được tham số");
                }
                catch (Exception ex)
                {
                    xulyFile.ghiLoi("Class CaLamViecController - Function: ca_ChinhSuaCaLamViec_Get", ex.Message);
                }
            return View();
        }
        /// <summary>
        /// hàm thực hiện chỉnh sửa thông tin ca làm việc trong CSDL
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ca_ChinhSuaCaLamViec(FormCollection f)
        {
            if (xulyChung.duocCapNhat(idOfPage, "7"))
            {
                caLamViec caSua = new caLamViec();
                qlCaPheEntities db = new qlCaPheEntities();
                try
                {
                    int maCa = Convert.ToInt32(f["txtMaCa"]);
                    caSua = db.caLamViecs.SingleOrDefault(c => c.maCa == maCa);
                    if (caSua != null)
                    {
                        this.layDuLieuTuView(caSua, f);
                        db.Entry(caSua).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                        return RedirectToAction("ca_TableCaLamViec");
                    }
                }
                catch (Exception ex)
                {
                    ViewBag.ThognBao = createHTML.taoThongBaoLuu(ex.Message);
                    xulyFile.ghiLoi("Class CaLamViecController - Function: ca_ChinhSuaCaLamViec_Post", ex.Message);
                    this.doDuLieuLenView(caSua);
                }
            }
            return View();
        }
        #endregion
        #region DELETE
        /// <summary>
        /// Hàm thực hiện xóa 1 ca làm việc khỏi CSDL
        /// </summary>
        /// <param name="maCa"></param>
        public void xoaCaLamViec(int maCa)
        {
            try
            {
                qlCaPheEntities db = new qlCaPheEntities();
                caLamViec caXoa = db.caLamViecs.SingleOrDefault(c => c.maCa == maCa);
                if (caXoa != null)
                {
                    db.caLamViecs.Remove(caXoa);
                    db.SaveChanges();
                }
                else
                    throw new Exception("Ca làm việc có mã " + maCa.ToString() + " không tồn tại để xóa");
            }
            catch (Exception ex)
            {
                xulyFile.ghiLoi("Class CaLamViecController - Function:xoaCaLamViec", ex.Message);
                Response.Redirect(xulyChung.layTenMien() + "/Home/ServerError");
            }
        }
        #endregion
        #region ORTHERS
        /// <summary>
        /// Hàm lấy dữ liệu từ giao diện gán cho các thuộc tính của caLamViec
        /// </summary>
        /// <param name="ca"></param>
        /// <param name="f"></param>
        private void layDuLieuTuView(caLamViec ca, FormCollection f)
        {
            string loi = "";
            ca.buoi = Convert.ToInt32(f["cbbBuoi"]);
            if (ca.buoi <= 0)
                loi += "Vui lòng chọn buổi cho ca <br/>";
            ca.tenCa = xulyDuLieu.xulyKyTuHTML(f["txtTenCa"]);
            if (ca.tenCa.Length <= 0)
                loi += "Vui lòng nhập tên ca làm việc <br/>";
            ca.ghiChu = xulyDuLieu.xulyKyTuHTML(f["txtGhiChu"]);
            string tgBatDau = f["txtBatDau"];
            if (tgBatDau.Length > 0)
                ca.batDau = TimeSpan.Parse(tgBatDau);
            else
                loi += "Vui lòng nhập thời gian bắt đầu cho ca <br/>";
            string tgKetThuc = f["txtKetThuc"];
            if (tgKetThuc.Length > 0)
                ca.ketThuc = TimeSpan.Parse(tgKetThuc);
            else
                loi += "Vui lòng nhập thời gian kết thúc cho ca <br/>";
            if (loi.Length > 0)
                throw new Exception(loi);
        }
        /// <summary>
        /// Hàm thực hiện đổ dữ liệu từ các thuộc tính của caLamViec lên giao diện
        /// </summary>
        /// <param name="ca"></param>
        private void doDuLieuLenView(caLamViec ca)
        {
            string cbbHtml = "";
            cbbHtml += " <option ";
            //-------Gán thuộc tính để chọn buổi
            if (ca.buoi == 1) cbbHtml += " selected ";
            cbbHtml += " value=\"1\">Sáng</option>";
            cbbHtml += "<option ";
            if (ca.buoi == 2) cbbHtml += " selected";
            cbbHtml += " value=\"2\">Chiều</option>";
            cbbHtml += "<option ";
            if (ca.buoi == 3) cbbHtml += " selected";
            cbbHtml += " value=\"3\">Tối</option>";
            ViewBag.cbbBuoi = cbbHtml;
            ViewBag.txtMaCa = ca.maCa.ToString();
            ViewBag.txtTenCa = xulyDuLieu.traVeKyTuGoc(ca.tenCa);
            ViewBag.txtGhiChu = xulyDuLieu.traVeKyTuGoc(ca.ghiChu);
            ViewBag.txtBatDau = string.Format("{0:hh:mm}", ca.batDau.ToString());
            ViewBag.txtKetThuc = string.Format("{0:hh:mm}", ca.ketThuc.ToString());
        }
        /// <summary>
        /// Hàm thực hiện tạo buổi
        /// </summary>
        private void taoDuLieuChoCbb()
        {
            string cbbHtml = "";
            cbbHtml += "<option value=\"1\">Sáng</option>";
            cbbHtml += "<option value=\"2\">Chiều</option>";
            cbbHtml += "<option value=\"3\">Tối</option>";
            ViewBag.cbbBuoi = cbbHtml;
        }
        #endregion
    }
}