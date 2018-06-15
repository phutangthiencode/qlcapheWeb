using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using qlCaPhe.App_Start;
using qlCaPhe.Models;

namespace qlCaPhe.Controllers
{
    public class KhoHangController : Controller
    {
        private static string idOfPage = "303";
        #region CREATE
        /// <summary>
        /// Hàm thực hiện tạo giao diện thêm mới kho hàng
        /// </summary>
        /// <returns></returns>
        public ActionResult kh_TaoMoiKhoHang()
        {
            if (xulyChung.duocTruyCap(idOfPage))
            {
                xulyChung.ghiNhatKyDtb(1, "Tạo mới kho hàng");
                return View();
            }
            return null;
        }
        /// <summary>
        /// Hàm thực hiện thêm mới 1 kho hàng vào CSDL
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult kh_TaoMoiKhoHang(khoHang kh, FormCollection f)
        {
            if (xulyChung.duocCapNhat(idOfPage, "7"))
            {
                string ndThongBao = ""; int kqLuu = 0;
                try
                {
                    qlCaPheEntities db = new qlCaPheEntities();
                    this.layDuLieuTuView(kh, f);
                    db.khoHangs.Add(kh);
                    kqLuu = db.SaveChanges();
                    if (kqLuu > 0)
                    {
                        ndThongBao = createHTML.taoNoiDungThongBao("Kho hàng", xulyDuLieu.traVeKyTuGoc(kh.tenKhoHang), "kh_TableKhoHang");
                        xulyChung.ghiNhatKyDtb(2, "Kho hàng\" " + xulyDuLieu.traVeKyTuGoc(kh.tenKhoHang) + " \"");
                    }
                }
                catch (Exception ex)
                {
                    ndThongBao = ex.Message;
                    xulyFile.ghiLoi("Class: KhoHangController - Function: kh_TaoMoiKhoHang_Post", ex.Message);
                    this.doDuLieuLenGiaoDien(kh);
                }
                ViewBag.ThongBao = createHTML.taoThongBaoLuu(ndThongBao);
            }
            return View();
        }

        #endregion
        #region READ
        /// <summary>
        /// Hàm thực hiện tạo giao diện danh sách kho hàng
        /// </summary>
        /// <returns></returns>
        public ActionResult kh_TableKhoHang()
        {
            if (xulyChung.duocTruyCap(idOfPage))
            {
                string htmlTable = "";
                try
                {
                    foreach (khoHang kh in new qlCaPheEntities().khoHangs.ToList())
                    {
                        htmlTable += "<tr role=\"row\" class=\"odd\">";
                        htmlTable += "      <td>" + xulyDuLieu.traVeKyTuGoc(kh.tenKhoHang) + "</td>";
                        htmlTable += "      <td>" + kh.dienTich.ToString() + " m<sup>2</sup></td>";
                        htmlTable += "      <td>" + xulyDuLieu.traVeKyTuGoc(kh.sdt) + "</td>";
                        htmlTable += "      <td>";
                        htmlTable += "          <a href=\"/KhoHang/kh_HienBanDoKhoHang?maKhoHang=" + kh.maKhoHang.ToString() + "\" target=\"_blank\" class=\"goiY\">";
                        htmlTable += xulyDuLieu.traVeKyTuGoc(kh.diaChi);
                        htmlTable += "              <span class=\"noiDungGoiY-bottom\">Click để xem chi tiết</span>";
                        htmlTable += "          </a>";
                        htmlTable += "      </td>";
                        htmlTable += "      <td>" + (kh.trangThai ? "Còn sử dụng" : "Không sử dụng") + "</td>";
                        htmlTable += "      <td>";
                        htmlTable += "          <div class=\"btn-group\">";
                        htmlTable += "              <button type=\"button\" class=\"btn btn-primary dropdown-toggle\" data-toggle=\"dropdown\">";
                        htmlTable += "                  Chức năng <span class=\"caret\"></span>";
                        htmlTable += "              </button>";
                        htmlTable += "              <ul class=\"dropdown-menu\" role=\"menu\">";
                        htmlTable += createTableData.taoNutChinhSua("/KhoHang/kh_ChinhSuaKhoHang", kh.maKhoHang.ToString());
                        htmlTable += createTableData.taoNutCapNhat("/KhoHang/capNhatTrangThai", kh.maKhoHang.ToString(), "col-orange", "clear", "Tạm ngưng/sử dụng");
                        htmlTable += createTableData.taoNutXoaBo(kh.maKhoHang.ToString());
                        htmlTable += "              </ul>";
                        htmlTable += "          </div>";
                        htmlTable += "      </td>";
                        htmlTable += "</tr>";
                        xulyChung.ghiNhatKyDtb(1, "Danh mục kho hàng");
                    }
                }
                catch (Exception ex)
                {
                    xulyFile.ghiLoi("Class: KhoHangController - Function: kh_TableKhoHang", ex.Message);
                    return RedirectToAction("ServerError", "Home");
                }
                ViewBag.TableData = htmlTable;
                ViewBag.ScriptAjax = createScriptAjax.scriptAjaxXoaDoiTuong("KhoHang/xoaKhoHang?maKhoHang=");
                ViewBag.ModalXoa = createHTML.taoCanhBaoXoa("Kho hàng");
            }
            return View();
        }

        /// <summary>
        /// Hàm thực hiện tạo view vị trí kho hàng trên bản đồ
        /// </summary>
        /// <param name="maKhoHang"></param>
        /// <returns></returns>
        public ActionResult kh_HienBanDoKhoHang(int maKhoHang)
        {
            if (xulyChung.duocTruyCap(idOfPage))
            {
                try
                {
                    //-----Lấy thông tin kho hàng cần xem vị trí
                    khoHang khLay = new qlCaPheEntities().khoHangs.SingleOrDefault(kh => kh.maKhoHang == maKhoHang);
                    //-----Gán dữ liệu vị trí kho hàng cho bản đồ
                    string data = "\"Id\": 1, \"tenKhoHang\": \"" + xulyDuLieu.traVeKyTuGoc(khLay.tenKhoHang) + "\", \"diaChi\": \"" + xulyDuLieu.traVeKyTuGoc(khLay.diaChi) + "\", \"sdt\": \"" + xulyDuLieu.traVeKyTuGoc(khLay.sdt) +
                        "\", \"GeoLong\": \"" + xulyDuLieu.traVeKyTuGoc(khLay.kinhDo) + "\", \"GeoLat\": \"" + xulyDuLieu.traVeKyTuGoc(khLay.viDo) + "\"";
                    //-----Thêm div chi tiết vị trí khi nhấn vào marker trên bản đồ
                    string content = "content: \"<div class='infoDiv'><h2>\" + item.tenKhoHang + \"</h2>\" + \"<div><h4>Địa chỉ: \" + item.diaChi + \"</h4><h4>Điện thoại: \" + item.sdt + \"</h4></div></div>\"\n";
                    //-----Nhúng script chứa API key bản đồ
                    ViewBag.ScriptAPI = createScriptAjax.taoScriptGoogleMapAPIKey();
                    //-----Nhúng script bản đồ vào giao diện
                    ViewBag.ScriptMap = createScriptAjax.taoScriptNhungBanDo(xulyDuLieu.traVeKyTuGoc(khLay.kinhDo), xulyDuLieu.traVeKyTuGoc(khLay.viDo), data, content);
                    //-----Hiện thông tin kho hàng lên title
                    ViewBag.TenKhoHang = xulyDuLieu.traVeKyTuGoc(khLay.tenKhoHang);
                    xulyChung.ghiNhatKyDtb(1, "Địa điểm kho hàng trên bản đồ");
                }
                catch (Exception ex)
                {
                    xulyFile.ghiLoi("Class: KhoHangController - Function: kh_HienBanDoKhoHang", ex.Message);
                    return RedirectToAction("PageNotFound", "Home");
                }
            }
            return View();
        }
        #endregion
        #region UPDATE

        /// <summary>
        /// Hàm thực hiện cập nhật trạng thái của kho hàng
        /// Trạng thái mới sẽ cập nhật ngược với trạng thái hiện tại
        /// </summary>
        /// <param name="maKhoHang"></param>
        /// <returns></returns>
        public void capNhatTrangThai()
        {
            if (xulyChung.duocCapNhat(idOfPage, "7"))
                try
                {
                    int kqLuu = 0;
                    string param = xulyChung.nhanThamSoTrongSession();
                    if (param.Length > 0)
                    {
                        int maKhoHang = xulyDuLieu.doiChuoiSangInteger(param);
                        qlCaPheEntities db = new qlCaPheEntities();
                        khoHang khoHangSua = db.khoHangs.SingleOrDefault(kh => kh.maKhoHang == maKhoHang);
                        if (khoHangSua != null)
                        {
                            khoHangSua.trangThai = !khoHangSua.trangThai;
                            db.Entry(khoHangSua).State = System.Data.Entity.EntityState.Modified;
                            kqLuu = db.SaveChanges();
                            if (kqLuu > 0)
                            {
                                xulyChung.ghiNhatKyDtb(4, "Cập nhật trạng thái kho hàng\" " + xulyDuLieu.traVeKyTuGoc(khoHangSua.tenKhoHang) + " \"");
                                Response.Redirect(xulyChung.layTenMien() + "/KhoHang/kh_TableKhoHang");
                            }
                        }
                        else
                            throw new Exception("Kho hàng có mã " + maKhoHang.ToString() + " không tồn tại trong hệ thống để cập nhật");
                    }
                    else throw new Exception("không nhận được tham số");
                }
                catch (Exception ex)
                {
                    xulyFile.ghiLoi("Class: KhoHangController - Function: capNhatTrangThai", ex.Message);
                    Response.Redirect(xulyChung.layTenMien() + "/Home/ServerError");
                }
        }
        /// <summary>
        /// Hàm thực hiện tạo giao diện chỉnh sửa kho hàng
        /// </summary>
        /// <param name="maKhoHang"></param>
        /// <returns></returns>
        public ActionResult kh_ChinhSuaKhoHang()
        {
            if (xulyChung.duocCapNhat(idOfPage, "7"))
            {
                try
                {
                    string param = xulyChung.nhanThamSoTrongSession();
                    if (param.Length > 0)
                    {
                        int maKhoHang = xulyDuLieu.doiChuoiSangInteger(param);
                        khoHang khoHangSua = new qlCaPheEntities().khoHangs.SingleOrDefault(kh => kh.maKhoHang == maKhoHang);
                        if (khoHangSua != null)
                        {
                            this.doDuLieuLenGiaoDien(khoHangSua);
                            xulyChung.ghiNhatKyDtb(1, "Chỉnh sửa kho hàng\" " + xulyDuLieu.traVeKyTuGoc(khoHangSua.tenKhoHang) + " \"");
                        }
                        else
                            throw new Exception("Kho hàng có mã " + maKhoHang.ToString() + " không tồn tại để cập nhật");
                    }
                    else
                        throw new Exception("không nhận được tham số");
                }
                catch (Exception ex)
                {
                    xulyFile.ghiLoi("Class: KhoHangController - Function: kh_ChinhSuaKhoHangGet", ex.Message);
                    return RedirectToAction("PageNotFound", "Home");
                }
            }
            return View();
        }
        /// <summary>
        /// hàm thực hiện cập nhật lại thông tin kho hàng trong csdl
        /// </summary>
        /// <param name="maKhoHang"></param>
        /// <param name="f"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult kh_ChinhSuaKhoHang(FormCollection f)
        {
            if (xulyChung.duocCapNhat(idOfPage, "7"))
            {
                khoHang khoHangSua = new khoHang();
                try
                {
                    int kqLuu = 0;
                    qlCaPheEntities db = new qlCaPheEntities();
                    int maKhoHang = xulyDuLieu.doiChuoiSangInteger(f["txtMaKH"]);
                    khoHangSua = db.khoHangs.SingleOrDefault(kh => kh.maKhoHang == maKhoHang);
                    if (khoHangSua != null)
                    {
                        this.layDuLieuTuView(khoHangSua, f);
                        db.Entry(khoHangSua).State = System.Data.Entity.EntityState.Modified;
                        kqLuu = db.SaveChanges();
                        if (kqLuu > 0)
                        {
                            xulyChung.ghiNhatKyDtb(4, "Kho hàng\" " + xulyDuLieu.traVeKyTuGoc(khoHangSua.tenKhoHang) + " \"");
                            return RedirectToAction("kh_TableKhoHang");
                        }
                    }
                }
                catch (Exception ex)
                {
                    ViewBag.ThongBao = createHTML.taoThongBaoLuu(ex.Message);
                    this.doDuLieuLenGiaoDien(khoHangSua);
                    xulyFile.ghiLoi("Class: KhoHangController - Function: kh_ChinhSuaKhoHangGet", ex.Message);
                }
            }
            return View();
        }
        #endregion
        #region DELETE
        /// <summary>
        /// Hàm thực hiện xóa một kho hàng trong csdl
        /// </summary>
        /// <param name="maKhoHang"></param>
        /// <returns></returns>
        public void xoaKhoHang(int maKhoHang)
        {
            try
            {
                int kqLuu = 0;
                qlCaPheEntities db = new qlCaPheEntities();
                khoHang khoHangXoa = db.khoHangs.SingleOrDefault(kh => kh.maKhoHang == maKhoHang);
                if (khoHangXoa != null)
                {
                    db.khoHangs.Remove(khoHangXoa);
                    kqLuu = db.SaveChanges();
                    if (kqLuu > 0)
                        xulyChung.ghiNhatKyDtb(3, "Kho hàng \"" + xulyDuLieu.traVeKyTuGoc(khoHangXoa.tenKhoHang) + " \"");
                }
                else
                    throw new Exception("kho hàng có mã " + maKhoHang.ToString() + " không tồn tại để xóa bỏ");
            }
            catch (Exception ex)
            {
                xulyFile.ghiLoi("Class: KhoHangController - Function: xoaKhoHang", ex.Message);
            }
        }
        #endregion
        #region ORTHERS
        /// <summary>
        /// Hàm thực hiện lấy dữ liệu từ giao diện và gán cho đối tượng KhoHang
        /// </summary>
        /// <param name="kh"></param>
        /// <param name="f"></param>
        private void layDuLieuTuView(khoHang kh, FormCollection f)
        {
            string loi = "";
            kh.tenKhoHang = xulyDuLieu.xulyKyTuHTML(f["txtTenKhoHang"]);
            if (kh.tenKhoHang.Length <= 0)
                loi += "Vui lòng nhập tên kho hàng <br/>";
            kh.sdt = xulyDuLieu.xulyKyTuHTML(f["txtSDT"]);
            kh.diaChi = xulyDuLieu.xulyKyTuHTML(f["txtDiaChi"]);
            if (kh.diaChi.Length <= 0)
                loi += "Vui lòng nhập địa chỉ của kho hàng <br/>";
            kh.kinhDo = xulyDuLieu.xulyKyTuHTML(f["txtKinhDo"]);
            kh.viDo = xulyDuLieu.xulyKyTuHTML(f["txtViDo"]);
            kh.dienTich = Convert.ToInt32(f["txtDienTich"]);
            if (kh.dienTich <= 0)
                loi += "Diện tích kho hàng phải lớn hơn 0 <br/>";
            kh.ghiChu = xulyDuLieu.xulyKyTuHTML(f["txtGhiChu"]);
            kh.trangThai = true;
            if (loi.Length > 0)
                throw new Exception(loi);
        }
        /// <summary>
        /// Hàm thực hiện đổ dữ liệu của kho hàng lên giao diện
        /// </summary>
        /// <param name="kh"></param>
        private void doDuLieuLenGiaoDien(khoHang kh)
        {
            ViewBag.txtMaKH = kh.maKhoHang.ToString();
            ViewBag.txtTenKhoHang = xulyDuLieu.traVeKyTuGoc(kh.tenKhoHang);
            ViewBag.txtSDT = xulyDuLieu.traVeKyTuGoc(kh.sdt);
            ViewBag.txtDiaChi = xulyDuLieu.traVeKyTuGoc(kh.diaChi);
            ViewBag.txtKinhDo = xulyDuLieu.traVeKyTuGoc(kh.kinhDo);
            ViewBag.txtViDo = xulyDuLieu.traVeKyTuGoc(kh.viDo);
            ViewBag.txtDienTich = kh.dienTich.ToString();
            ViewBag.txtGhiChu = xulyDuLieu.traVeKyTuGoc(kh.ghiChu);
        }
        #endregion
    }
}