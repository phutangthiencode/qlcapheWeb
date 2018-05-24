using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using qlCaPhe.Models;
using qlCaPhe.App_Start;

namespace qlCaPhe.Controllers
{
    public class KhuVucController : Controller
    {
        private static string idOfPage = "301";
        #region CREATE
        /// <summary>
        /// Hàm tạo view tạo mới khu vực
        /// </summary>
        /// <returns></returns>
        public ActionResult kv_TaoMoiKhuVuc()
        {
            if(xulyChung.duocTruyCap(idOfPage))
                return View();
            return null;
        }
        /// <summary>
        /// Hàm thực hiện thêm mới khu vực vào CSDL
        /// </summary>
        /// <param name="f"></param>
        /// <param name="kv"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult kv_TaoMoiKhuVuc(FormCollection f, khuVuc kv)
        {
            if (xulyChung.duocCapNhat(idOfPage, "7"))
            {
                string ndThongBao = "";
                try
                {
                    qlCaPheEntities db = new qlCaPheEntities();
                    this.layDuLieuTuGiaoDien(f, kv);
                    db.khuVucs.Add(kv);
                    db.SaveChanges();
                    ndThongBao = createHTML.taoNoiDungThongBao("Khu vực", xulyDuLieu.traVeKyTuGoc(kv.tenKhuVuc), "kv_TableKhuVuc");
                }
                catch (Exception ex)
                {
                    //-----Hiện lại dữ liệu trên giao diện sau khi thông báo lỗi
                    this.doDuLieuLenGiaoDien(kv);
                    //----Tạo nội dung thông báo lỗi
                    ndThongBao = ex.Message;
                    xulyFile.ghiLoi("Class: KhuVucController - Function: kv_TaoMoiKhuVuc_Post", ex.Message);
                }
                ViewBag.ThongBao = createHTML.taoThongBaoLuu(ndThongBao);
            }
            return View();
        }
        #endregion
        #region READ
        /// <summary>
        /// Hàm tạo giao diện danh sách khu vực
        /// </summary>
        /// <returns></returns>
        public ActionResult kv_TableKhuVuc()
        {
            if (xulyChung.duocTruyCap(idOfPage))
            {
                try
                {
                    string htmlTable = "";
                    foreach (khuVuc kv in new qlCaPheEntities().khuVucs.ToList().OrderBy(k => k.tenKhuVuc))
                    {
                        htmlTable += "<tr role=\"row\" class=\"odd\">";
                        htmlTable += "      <td>";
                        htmlTable += xulyDuLieu.traVeKyTuGoc(kv.tenKhuVuc);
                        htmlTable += "      </td>";
                        htmlTable += "      <td>" + xulyDuLieu.traVeKyTuGoc(kv.dienGiai) + "</td>";
                        htmlTable += "      <td>" + kv.dienTich.ToString() + " m<sup>2</sup></td>";
                        htmlTable += "      <td>" + kv.tongSucChua.ToString() + " khách</td>";
                        htmlTable += "      <td>";
                        htmlTable += "          <div class=\"btn-group\">";
                        htmlTable += "              <button type=\"button\" class=\"btn btn-primary dropdown-toggle\" data-toggle=\"dropdown\">";
                        htmlTable += "                  Chức năng <span class=\"caret\"></span>";
                        htmlTable += "              </button>";
                        htmlTable += "              <ul class=\"dropdown-menu\" role=\"menu\">";
                        htmlTable += createTableData.taoNutChinhSua("/KhuVuc/kv_ChinhSuaKhuVuc", kv.maKhuVuc.ToString());
                        htmlTable += createTableData.taoNutXoaBo(kv.maKhuVuc.ToString());
                        htmlTable += "              </ul>";
                        htmlTable += "          </div>";
                        htmlTable += "      </td>";
                        htmlTable += "</tr>";
                    }
                    ViewBag.TableData = htmlTable;
                    ViewBag.ScriptAjax = createScriptAjax.scriptAjaxXoaDoiTuong("KhuVuc/xoaKhuVuc?maKV=");
                    ViewBag.ModalXoa = createHTML.taoCanhBaoXoa("Khu vực");

                }
                catch (Exception ex)
                {
                    xulyFile.ghiLoi("Class: KhuVucController - Function: kv_TableKhuVuc", ex.Message);
                    return RedirectToAction("ServerError", "Home");
                }
            }
            return View();
        }
        #endregion
        #region UPDATE
        /// <summary>
        /// Hàm tạo view chỉnh sửa khu vực
        /// </summary>
        /// <param name="maKV"></param>
        /// <returns></returns>
        public ActionResult kv_ChinhSuaKhuVuc()
        {
            if (xulyChung.duocCapNhat(idOfPage, "7"))
            {
                try
                {
                    string urlAction = (string)Session["urlAction"]; //urlAction có dạng: page=tv_ChinhSuaThanhVien|request=maTV
                    if (urlAction.Length > 0)
                    {
                        //-----Xử lý request trong session
                        urlAction = urlAction.Split('|')[1];  //urlAction có dạng: request=maTV
                        urlAction = urlAction.Replace("request=", ""); //urlAction có dạng: maTV
                        int maKV = xulyDuLieu.doiChuoiSangInteger(urlAction);
                        khuVuc khuVucSua = new qlCaPheEntities().khuVucs.SingleOrDefault(kv => kv.maKhuVuc == maKV);
                        if (khuVucSua != null)
                            this.doDuLieuLenGiaoDien(khuVucSua);
                        else
                            throw new Exception("Khu vực có mã '" + maKV.ToString() + "' không tồn tại để chỉnh sửa");
                    }
                    else throw new Exception("không nhận được tham số");
                }
                catch (Exception ex)
                {
                    xulyFile.ghiLoi("Class: KhuVucController - Function: kv_ChinhSuaKhuVuc", ex.Message);
                    return RedirectToAction("ServerError", "Home");
                }
            }
            return View();
        }
        /// <summary>
        /// Hàm thực hiện chỉnh sửa khu vực trong CSDL
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult kv_ChinhSuaKhuVuc(FormCollection f)
        {
            if (xulyChung.duocCapNhat(idOfPage, "7"))
            {
                khuVuc khuVucSua = new khuVuc();
                try
                {
                    qlCaPheEntities db = new qlCaPheEntities();
                    int maKV = xulyDuLieu.doiChuoiSangInteger(f["txtMaKV"]);
                    khuVucSua = db.khuVucs.Single(kv => kv.maKhuVuc == maKV);
                    if (khuVucSua != null)
                    {
                        this.layDuLieuTuGiaoDien(f, khuVucSua);
                        db.Entry(khuVucSua).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                        return RedirectToAction("kv_TableKhuVuc");
                    }
                    else
                        throw new Exception("Khu vực có mã '" + maKV.ToString() + "' không tồn tại để chỉnh sửa");
                }
                catch (Exception ex)
                {
                    ViewBag.ThongBao = createHTML.taoThongBaoLuu(ex.Message);
                    xulyFile.ghiLoi("Class: KhuVucController - Function: kv_ChinhSuaKhuVuc", ex.Message);
                    this.doDuLieuLenGiaoDien(khuVucSua);
                }
            }
            return View();
        }
        #endregion
        #region DELETE
        /// <summary>
        /// Hàm thực hiện xóa 1 khu vực khỏi csdl
        /// </summary>
        /// <param name="tenTK">Tên tài khoản cần xóa</param>
        public void xoaKhuVuc(int maKV)
        {
            try
            {
                qlCaPheEntities db = new qlCaPheEntities();
                var khuVucXoa = db.khuVucs.Single(kv => kv.maKhuVuc == maKV);
                if (khuVucXoa != null)
                {
                    db.khuVucs.Remove(khuVucXoa);
                    db.SaveChanges();
                }
                else
                    throw new Exception("Khu vực có mã '" + maKV.ToString() + "' không tồn tại để xóa");
            }
            catch (Exception ex)
            {
                xulyFile.ghiLoi("Class: KhuVucController - Function: xoaKhuVuc", ex.Message);
                RedirectToAction("ServerError", "Home");
            }
        }
        #endregion
        #region ORTHERS
        /// <summary>
        /// hàm thực hiện lấy dữ liệu từ giao diện và gán cho các thuộc tính của KhuVuc
        /// </summary>
        /// <param name="f"></param>
        /// <param name="kv"></param>
        private void layDuLieuTuGiaoDien(FormCollection f, khuVuc kv)
        {
            string loi = "";
            kv.tenKhuVuc = xulyDuLieu.xulyKyTuHTML(f["txtTenKV"]);
            if (kv.tenKhuVuc.Length <= 0)
                loi += "Vui lòng nhập tên khu vực <br/>";
            kv.dienGiai = xulyDuLieu.xulyKyTuHTML(f["txtDienGiai"]);
            if (kv.dienGiai.Length <= 0)
                loi += "Vui lòng nhập thông tin diễn giải cho khu vực <br/>";
            kv.tongSucChua = xulyDuLieu.doiChuoiSangInteger(f["txtTongSucChua"]);
            if (kv.tongSucChua <= 0)
                loi += "Tổng sức chứa của khu vực phải lớn hơn 0 </br>";
            kv.dienTich = xulyDuLieu.doiChuoiSangInteger(f["txtDienTich"]);
            if (kv.dienTich <= 0)
                loi += "Diện tích của khu vực phải lớn hơn 0 <br/>";
            kv.ghiChu = xulyDuLieu.xulyKyTuHTML(f["txtGhiChu"]);
            if (loi.Length > 0)
                throw new Exception(loi);
        }
        /// <summary>
        /// Hàm thực hiện đổ dữ liệu lên giao diện khu vực
        /// </summary>
        /// <param name="maKV"></param>
        private void doDuLieuLenGiaoDien(khuVuc kv)
        {
            //-----Hiện lại dữ liệu trên giao diện 
            ViewBag.txtMaKV = kv.maKhuVuc.ToString();
            ViewBag.txtTenKV = xulyDuLieu.traVeKyTuGoc(kv.tenKhuVuc);
            ViewBag.txtDienGiai = xulyDuLieu.traVeKyTuGoc(kv.dienGiai);
            ViewBag.txtDienTich = xulyDuLieu.traVeKyTuGoc(kv.dienTich.ToString());
            ViewBag.txtTongSucChua = xulyDuLieu.traVeKyTuGoc(kv.tongSucChua.ToString());
            ViewBag.txtGhiChu = xulyDuLieu.traVeKyTuGoc(kv.ghiChu);
        }
        #endregion
    }
}