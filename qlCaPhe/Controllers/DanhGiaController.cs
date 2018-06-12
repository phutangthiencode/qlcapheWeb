using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using qlCaPhe.Models;
using qlCaPhe.App_Start;
using qlCaPhe.App_Start.Cart;

namespace qlCaPhe.Controllers
{
    public class DanhGiaController : Controller
    {
        private static string idOfPageMucTieu = "903";
        private static string idOfPageDanhGia= "904";

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
        #region CREATE
        /// <summary>
        /// Hàm tạo giao diện đánh giá nhân viên
        /// </summary>
        /// <returns></returns>
        public ActionResult dg_TaoMoiDanhGia()
        {
            if (xulyChung.duocTruyCap(idOfPageDanhGia))
            {
                try
                {
                    this.resetSession();
                    qlCaPheEntities db = new qlCaPheEntities();
                    this.taoDuLieuComboboxThanhVienDanhGia(db, 0);
                    taoDanhSachMucTieuDanhGia(db);
                    this.taoScript();
                    return View();
                }
                catch (Exception ex)
                {
                    xulyFile.ghiLoi("Class: DanhGiaController - Function: dg_TaoMoiDanhGia", ex.Message);
                }
            }
            return null;
        }
        /// <summary>
        /// Hàm lưu 1 đánh giá của 1 nhân viên vào CSDL
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult dg_TaoMoiDanhGia(FormCollection f)
        {
            if (xulyChung.duocCapNhat(idOfPageDanhGia, "7"))
            {
                string ndThongBao = ""; int kqLuu = 0;
                danhGiaNhanVien danhGiaAdd = new danhGiaNhanVien();
                try
                {
                    qlCaPheEntities db = new qlCaPheEntities();
                    this.layDuLieuTuViewDanhGia(danhGiaAdd, f, db);
                    //-----Thêm mới đánh giá vào CSDL
                    db.danhGiaNhanViens.Add(danhGiaAdd);
                    kqLuu = db.SaveChanges();
                    if (kqLuu > 0)
                    {
                        this.themChiTietDanhGiaVaoDatabase(db, danhGiaAdd);
                        ndThongBao = createHTML.taoNoiDungThongBao("Đánh giá nhân viên", xulyDuLieu.traVeKyTuGoc(danhGiaAdd.thanhVien.hoTV) + " " + xulyDuLieu.traVeKyTuGoc(danhGiaAdd.thanhVien.tenTV), "dg_TableDanhGiaNhanVien");
                        this.resetSession();
                    }
                }
                catch (Exception ex)
                {
                    ndThongBao = ex.Message;
                    xulyFile.ghiLoi("Class: DanhGiaController - Function: dg_TaoMoiDanhGia_Post", ex.Message);
                    this.doDuLieuLenViewDanhGia(danhGiaAdd);
                }
                ViewBag.ThongBao = createHTML.taoThongBaoLuu(ndThongBao);
            }
            return View();
        }
        /// <summary>
        /// Hàm thực hiện thêm chi tiết đánh giá trong CSDL
        /// </summary>
        /// <param name="db"></param>
        /// <param name="danhGia"></param>
        private void themChiTietDanhGiaVaoDatabase(qlCaPheEntities db, danhGiaNhanVien danhGia)
        {
            try
            {
                cartDanhGia cartRate = (cartDanhGia)Session["daDanhGia"];
                foreach (ctDanhGia ct in cartRate.Info.Values)
                {
                    ctDanhGia ctAdd = new ctDanhGia();
                    ctAdd.diemSo = ct.diemSo;
                    ctAdd.dienGiai = ct.dienGiai;
                    ctAdd.maDanhGia = danhGia.maDanhGia;
                    ctAdd.maMucTieu = ct.maMucTieu;
                    db.ctDanhGias.Add(ctAdd);
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                xulyFile.ghiLoi("Class: DanhGiaController - Function: themChiTietDanhGiaVaoDatabase", ex.Message);
            }
        }
        #endregion
        /// <summary>
        /// Hàm tạo giao diện danh mục các đánh giá nhân viên
        /// </summary>
        /// <returns></returns>
        public ActionResult dg_TableDanhGiaNhanVien(int ?page)
        {
            if (xulyChung.duocTruyCap(idOfPageDanhGia))
            {
                try
                {
                    string htmlTable = ""; int trangHienHanh = (page ?? 1);
                    qlCaPheEntities db = new qlCaPheEntities();
                    int soPhanTu = db.danhGiaNhanViens.Count();
                    ViewBag.PhanTrang = createHTML.taoPhanTrang(soPhanTu, trangHienHanh, "/DanhGia/dg_TableDanhGiaNhanVien"); //------cấu hình phân trang
                    foreach (danhGiaNhanVien danhGia in db.danhGiaNhanViens.ToList().OrderByDescending(s => s.ngayDanhGia).Skip((trangHienHanh - 1) * createHTML.pageSize).Take(createHTML.pageSize))
                    {
                        htmlTable+="<tr role=\"row\" class=\"odd\">";
                        htmlTable+="    <td><b class=\"col-blue\">"+xulyDuLieu.traVeKyTuGoc(danhGia.thanhVien.hoTV) + " " + xulyDuLieu.traVeKyTuGoc(danhGia.thanhVien.tenTV)+"</b></td>";
                        htmlTable+="    <td>"+danhGia.ngayDanhGia.ToString()+"</td>";
                        htmlTable+="    <td>"+danhGia.ctDanhGias.Sum(t=>t.diemSo).ToString()+"</td>";
                        htmlTable+="    <td>"+xulyDuLieu.traVeKyTuGoc(danhGia.taiKhoan.thanhVien.hoTV) + " " + xulyDuLieu.traVeKyTuGoc(danhGia.taiKhoan.thanhVien.tenTV)+"</td>";
                        htmlTable += "    <td>" + xulyDuLieu.traVeKyTuGoc(danhGia.ghiChu)+ "</td>";
                        htmlTable += "  <td>";
                        htmlTable += "           <div class=\"btn-group\">";
                        htmlTable += "               <button type=\"button\" class=\"btn btn-primary dropdown-toggle\" data-toggle=\"dropdown\" aria-expanded=\"true\">";
                        htmlTable += "                   Chức năng <span class=\"caret\"></span>";
                        htmlTable += "               </button>";
                        htmlTable += "               <ul class=\"dropdown-menu\" role=\"menu\">";
                        htmlTable += "<li><a madg=\""+danhGia.maDanhGia.ToString()+"\" class=\"js-btnXemChiTiet col-green waves-effect waves-block\"><i class=\"material-icons\">search</i>Xem chi tiết</a></li>";
                        htmlTable += createTableData.taoNutChinhSua("/DanhGia/dg_ChinhSuaDanhGia", danhGia.maDanhGia.ToString());
                        htmlTable += createTableData.taoNutXoaBo(danhGia.maDanhGia.ToString());
                        htmlTable += "               </ul>";
                        htmlTable += "           </div>";
                        htmlTable += "  </td>";
                        htmlTable += "</tr>";
                    }
                    ViewBag.ScriptAjaxXemChiTiet = createScriptAjax.scriptAjaxXemChiTietKhiClick("js-btnXemChiTiet", "madg", "DanhGia/AjaxXemChiTietDanhGia?maDG=", "vungChiTiet", "modalChiTiet");
                    ViewBag.ModalChiTiet = createHTML.taoModalChiTiet("modalChiTiet", "vungChiTiet", 3);
                    ViewBag.ScriptAjax = createScriptAjax.scriptAjaxXoaDoiTuong("DanhGia/xoaMotDanhGia=");
                    ViewBag.ModalXoa = createHTML.taoCanhBaoXoa("Đánh giá");
                    ViewBag.TableData = htmlTable;
                    return View();
                }
                catch (Exception ex)
                {
                    xulyFile.ghiLoi("Class DanhGiaController - Function: dg_TableDanhGiaNhanVien", ex.Message);
                    return RedirectToAction("PageNotFound", "Home");
                }
            }
            return null;
        }
        /// <summary>
        /// Hàm xử lý tạo modal xem chi tiết nội dung đã đánh giá của 1 nhân viên
        /// </summary>
        /// <param name="maDG">mã đánh giá cần xem</param>
        /// <returns>Nội dung hiển thị lên modal</returns>
        public string AjaxXemChiTietDanhGia(int maDG)
        {
            string kq = "";
            if (xulyChung.duocTruyCap(idOfPageDanhGia))
            {
                try
                {
                    danhGiaNhanVien danhGia = new qlCaPheEntities().danhGiaNhanViens.SingleOrDefault(t => t.maDanhGia == maDG);
                    if (danhGia != null)
                    {
                        kq += "<div class=\"modal-header\">";
                        kq += "      <button type=\"button\" class=\"close\" data-dismiss=\"modal\">×</button>";
                        kq += "    <h3 class=\"modal-title\" id=\"largeModalLabel\">XEM CHI TIẾT ĐÁNH GIÁ CỦA \"" + xulyDuLieu.traVeKyTuGoc(danhGia.thanhVien.hoTV) + " " + xulyDuLieu.traVeKyTuGoc(danhGia.thanhVien.tenTV) + "\" </h3>";
                        kq += "</div>";
                        kq += "<div class=\"modal-body\">";
                        kq += "    <div class=\"row\">";
                        kq += "        <div class=\"col-md-12 col-lg-12\">";
                        kq += "            <div class=\"card\">";
                        kq += "                <div class=\"header bg-cyan\">";
                        kq += "                    <h2>Danh mục các mục tiêu đã đánh giá</h2>";
                        kq += "                </div>";
                        kq += "                <div class=\"body table-responsive\">";
                        kq += this.taoBangChiTietDanhGia(danhGia);
                        kq += "                </div>";
                        kq += "            </div>";
                        kq += "        </div>";
                        kq += "</div>";
                        kq += "<div class=\"modal-footer\">";
                        kq += "    <div class=\"col-md-8 pull-right\">          ";
                        kq += "        <a class=\"btn btn-default waves-effect\"  data-dismiss=\"modal\"><i class=\"material-icons\">close</i>Đóng lại</a>";
                        kq += "    </div>";
                        kq += "</div>";
                    }
                }
                catch (Exception ex)
                {
                    xulyFile.ghiLoi("Class DanhGiaController - Function: AjaxXemChiTietDanhGia", ex.Message);
                }
            }
            return kq;
        }
        /// <summary>
        /// Hàm tạo nội dung cho bảng chi tiết đánh giá có trên modal
        /// </summary>
        /// <param name="danhGia"></param>
        /// <returns>Chuỗi html dành cho bảng</returns>
        private string taoBangChiTietDanhGia(danhGiaNhanVien danhGia)
        {
            string kq = "";
            try
            {
                kq+="<table class=\"table table-hover\">";
                kq+="    <thead>";
                kq+="        <tr>";
                kq+="            <th width=\"20%\">Mục tiêu</th>";
                kq+="            <th width=\"20%\">Diễn giải mục tiêu</th>";
                kq+="            <th width=\"15%\">Điểm số</th>";
                kq+="            <th width=\"35%\">Diễn giải điểm số</th>";
                kq+="        </tr>";
                kq+="    </thead>";
                kq+="    <tbody>";
                foreach (ctDanhGia ct in danhGia.ctDanhGias)
                {
                    kq += "        <tr>";
                    kq += "            <td>"+xulyDuLieu.traVeKyTuGoc(ct.mucTieuDanhGia.tenMucTieu)+"</td>";
                    kq += "            <td>"+xulyDuLieu.traVeKyTuGoc(ct.mucTieuDanhGia.dienGiai)+"</td>";
                    kq += "            <td>"+ct.diemSo.ToString()+"</td>";
                    kq += "            <td>"+xulyDuLieu.traVeKyTuGoc(ct.dienGiai)+"</td>";
                    kq += "        </tr>";
                }
                kq+="    </tbody>";
                kq += "</table>";
            }
            catch (Exception ex)
            {
                xulyFile.ghiLoi("Class DanhGiaController - Function: taoBangChiTietDanhGia", ex.Message);
            }
            return kq;
        }


        #region ORTHERS
        /// <summary>
        /// Hàm thực hiện load tất cả thành viên lên combobox để đánh giá
        /// </summary>
        /// <param name="db"></param>
        private void taoDuLieuComboboxThanhVienDanhGia(qlCaPheEntities db, int maTV)
        {
            //Hiển thị dropdownList lựa chọn thành viên
            string htmlCbbThanhVien = "";
            foreach (thanhVien tv in db.thanhViens.ToList())
            {
                htmlCbbThanhVien += "<option ";
                if (tv.maTV == maTV) //--------Tích chọn thành viên nếu trùng mã
                    htmlCbbThanhVien += " selected ";
                htmlCbbThanhVien+="class=\"chonTV\" value=\"" + tv.maTV.ToString() + "\" maLay=\"" + tv.maTV.ToString() + "\">" + xulyDuLieu.traVeKyTuGoc(tv.hoTV) + " " + xulyDuLieu.traVeKyTuGoc(tv.tenTV) + "</option>";
            }
            ViewBag.cbbThanhVien = htmlCbbThanhVien;
        }
        /// <summary>
        /// Hàm thực hiện reset lại tất cả các giỏ
        /// </summary>
        private void resetSession()
        {
            Session.Remove("chuaDanhGia"); Session.Remove("daDanhGia");
            Session.Add("chuaDanhGia", new cartMucTieu()); Session.Add("daDanhGia", new cartDanhGia()); 
        }
        /// <summary>
        /// Hàm thực hiện tạo mục tiêu đánh giá và lưu vào giỏ Trước đánh giá
        /// </summary>
        /// <param name="db"></param>
        private void taoDanhSachMucTieuDanhGia(qlCaPheEntities db)
        {
            cartMucTieu cartChuaDanhGia = (cartMucTieu)Session["chuaDanhGia"];
            foreach (mucTieuDanhGia mucTieu in db.mucTieuDanhGias.Where(t => t.trangThai == true))
                cartChuaDanhGia.addCart(mucTieu);
        }
        /// <summary>
        /// Hàm lấy dữ liệu từ giao diện Đánh giá nhân viên và gán vào các thuộc tính của object DanhGiaNhanVien
        /// </summary>
        /// <param name="danhGiaAdd"></param>
        /// <param name="f"></param>
        private void layDuLieuTuViewDanhGia(danhGiaNhanVien danhGiaAdd, FormCollection f, qlCaPheEntities db)
        {
            string loi = "";
            danhGiaAdd.maTV = xulyDuLieu.doiChuoiSangInteger(f["cbbThanhVien"]);
            if (danhGiaAdd.maTV <= 0)
                loi += "Vui lòng chọn thành viên cần đánh giá<br/>";
            danhGiaAdd.ngayDanhGia = DateTime.Now;
            danhGiaAdd.taiKhoanDanhGia = ((taiKhoan)Session["login"]).tenDangNhap;
            danhGiaAdd.ghiChu = xulyDuLieu.xulyKyTuHTML(f["txtGhiChu"]);
            cartDanhGia cartRate = (cartDanhGia)Session["daDanhGia"];
            if (cartRate.Info.Count == 0)
                loi += "<i class=\"col-red\">*Chưa đánh giá bất kỳ mục tiêu nào</i>";
            danhGiaAdd.thanhVien = db.thanhViens.SingleOrDefault(t => t.maTV == danhGiaAdd.maTV);
            if (loi.Length > 0)
                throw new Exception(loi);
        }
        /// <summary>
        /// Hàm thực hiện đổ dữ liệu lên giao diện
        /// </summary>
        /// <param name="danhGia"></param>
        /// <param name="db"></param>
        private void doDuLieuLenViewDanhGia(danhGiaNhanVien danhGia)
        {
            try
            {
                this.taoScript();
                qlCaPheEntities db = new qlCaPheEntities();
                this.taoDuLieuComboboxThanhVienDanhGia(db, danhGia.maTV);
                ViewBag.txtGhiChu = xulyDuLieu.traVeKyTuGoc(danhGia.ghiChu);
                if (danhGia.thanhVien.hinhDD != null)
                    ViewBag.VungThongTinThanhVien = new ThanhVienController().getInfoThanhVienForCreateTaiKhoan(danhGia.maTV);
            }
            catch (Exception ex)
            {
                xulyFile.ghiLoi("Class: DanhGiaController - Function: doDuLieuLenViewDanhGia", ex.Message);
            }
        }
        /// <summary>
        /// Hàm nhúng các script ajax lên giao diện
        /// Và nhúng kèm thông tin tài khoản người đánh giá lên giao diện
        /// </summary>
        private void taoScript()
        {
            //-----Hiện thông tin thành viên thực hiện đánh giá
            taiKhoan tkLogin = (taiKhoan)Session["login"];
            ViewBag.TaiKhoanDanhGia = xulyDuLieu.traVeKyTuGoc(tkLogin.thanhVien.hoTV) + " " + xulyDuLieu.traVeKyTuGoc(tkLogin.thanhVien.tenTV) + " - " + xulyDuLieu.traVeKyTuGoc(tkLogin.tenDangNhap);
            //Nhúng script ajax lấy thông tin thành viên
            ViewBag.ScriptAjax = createScriptAjax.scriptGetInfoComboboxClick("cbbThanhVien", "ThanhVien/getInfoThanhVienForCreateTaiKhoan?maTV=", "vungThongTinThanhVien");
            ViewBag.ToastThongBao = createHTML.taoToastThongBao("Vui lòng nhập đầy đủ thông tin đánh giá", "bg-blue-grey");
        }

        #endregion

        #region AJAX
        /// <summary>
        /// Hàm ajax lấy danh sách các mục tiêu cần đánh giá và hiện lên giao diện
        /// Các mục tiêu đánh giá được lấy từ giỏ
        /// </summary>
        /// <returns>Chuỗi html tạo các dòng dữ liệu mục tiêu</returns>
        public string AjaxLayDanhSachMucTieu()
        {
            string kq = "";
            try
            {
                //--------Lặp qua các mục tiêu còn lại trong giỏ truocDanhGia
                cartMucTieu cartMucTieu = (cartMucTieu)Session["chuaDanhGia"];
                foreach (mucTieuDanhGia mucTieu in cartMucTieu.Info.Values)
                {
                    kq+="<tr>";
                    kq += "  <td><b class=\"col-blue\">" + xulyDuLieu.traVeKyTuGoc(mucTieu.tenMucTieu) + "</b></td>";
                    kq += "  <td>" + xulyDuLieu.traVeKyTuGoc(mucTieu.dienGiai) + "</td>";
                    kq+="    <td class=\"focused\">";
                    kq+="        <input type=\"number\" id=\"txtDiemSo"+mucTieu.maMucTieu.ToString()+"\" class=\"form-control\" placeholder=\"Nhập Điểm số đánh giá...\">";
                    kq+="    </td>";
                    kq+="    <td>";
                    kq+="        <input type=\"text\"  id=\"txtDienGiai"+mucTieu.maMucTieu.ToString()+"\" class=\"form-control\" placeholder=\"Nhập diễn giải điểm số đã cho....\">";
                    kq+="    </td>";
                    kq+="    <td>";
                    kq+="        <button type=\"button\" mamt=\""+mucTieu.maMucTieu.ToString()+"\" class=\"js-btnDanhGia btn btn-primary waves-effect\">Đánh giá</button>";
                    kq+="    </td>";
                    kq += "</tr>";
                }

            }
            catch (Exception ex)
            {
                xulyFile.ghiLoi("Class: DanhGiaController - Function: AjaxLayDanhSachMucTieu", ex.Message);
            }
            return kq;
        }
        /// <summary>
        /// Hàm Ajax thực hiện xử lý thêm 1 đánh giá mới cho nhân viên
        /// </summary>
        /// <param name="param">Chuỗi tham số chứa mục tiêu và thông tin đã đánh giá <para/> param = maMucTieu|diemSo|dienGiai</param>
        /// <returns>Chuỗi html tạo dữ liệu cho các bảng. Có dạng <para/> DuLieuMucTieuDanhGia|DuLieuDaDanhGia</returns>
        public string AjaxDanhGiaNhanVien(string param)
        {
            try
            {
                cartMucTieu cartTarget = (cartMucTieu)Session["chuaDanhGia"];
                cartDanhGia cartRate = (cartDanhGia)Session["daDanhGia"];
                if (param.Split('|').Count() == 3) //------Kiểm tra tham số
                {//------Xử lý tham số
                    int maMucTieu = xulyDuLieu.doiChuoiSangInteger(param.Split('|')[0]);
                    int diemSo = xulyDuLieu.doiChuoiSangInteger(param.Split('|')[1]);
                    string dienGiai = xulyDuLieu.xulyKyTuHTML(param.Split('|')[2]);
                    //------Lấy mục tiêu đánh giá
                    mucTieuDanhGia mucTieuSelect = cartTarget.getInfo(maMucTieu);
                    if (mucTieuSelect != null)
                    {
                        //-----Thêm mới mục tiêu vào cart Đánh giá
                        ctDanhGia ctAdd = new ctDanhGia();
                        ctAdd.maMucTieu = maMucTieu; ctAdd.diemSo = diemSo; ctAdd.dienGiai = dienGiai; ctAdd.mucTieuDanhGia = mucTieuSelect;
                        cartRate.addCart(ctAdd); Session["daDanhGia"] = cartRate;

                        //----Xóa mục tiêu đã đánh giá khỏi danh sách chưa đánh giá
                        cartTarget.removeItem(maMucTieu);
                        Session["chuaDanhGia"] = cartTarget;
                    }
                }

            }
            catch (Exception ex)
            {
                xulyFile.ghiLoi("Class: DanhGiaController - Function: AjaxLayDanhSachMucTieu", ex.Message);
            }
            return AjaxLayDanhSachMucTieu() + "|" + taoBangMucTieuDaDanhGia();
        }
        /// <summary>
        /// Hàm tạo dữ liệu cho bảng mục tiêu đã đánh giá
        /// Dự liệu có trên SEssion daDanhGia
        /// </summary>
        /// <returns>Chuỗi html tạo dữ liệu cho bảng</returns>
        public string taoBangMucTieuDaDanhGia()
        {
            string kq = "";
            try
            {
                cartDanhGia cartRate = (cartDanhGia)Session["daDanhGia"];
                foreach (ctDanhGia ct in cartRate.Info.Values)
                {
                    kq+="<tr>";
                    kq+="    <td><b class=\"col-blue\">"+xulyDuLieu.traVeKyTuGoc(ct.mucTieuDanhGia.tenMucTieu)+"</b></td>";
                    kq+="    <td>"+xulyDuLieu.traVeKyTuGoc(ct.mucTieuDanhGia.dienGiai)+"</td>";
                    kq+="    <td><b>"+ct.diemSo.ToString()+"</b></td>";
                    kq+="    <td>"+xulyDuLieu.traVeKyTuGoc(ct.dienGiai)+"</td>";
                    kq+="    <td><button type=\"button\" mamt=\""+ct.maMucTieu.ToString()+"\" class=\"js-btnDanhGiaLai btn btn-danger waves-effect\">Đánh giá lại</button></td>";
                    kq += "</tr>";
                }

            }
            catch (Exception ex)
            {
                xulyFile.ghiLoi("Class: DanhGiaController - Function: AjaxLayDanhSachMucTieu", ex.Message);
            }
            return kq;
        }
        /// <summary>
        /// Hàm xử lý sự kiện xóa bỏ 1 đánh giá đã đánh giá khỏi giỏ daDanhGia
        /// </summary>
        /// <param name="param">Tham số chứa mã mục tiêu cần loại bỏ</param>
        /// <returns>Chuỗi html tạo dữ liệu cho các bảng. Có dạng <para/> DuLieuMucTieuDanhGia|DuLieuDaDanhGia</returns>
        public string AjaxDanhGiaLai(string param)
        {
            try
            {
                cartMucTieu cartTarget = (cartMucTieu)Session["chuaDanhGia"];
                cartDanhGia cartRate = (cartDanhGia)Session["daDanhGia"];
                if (param.Count()>0) //------Kiểm tra tham số
                {//------Xử lý tham số
                    int maMucTieu = xulyDuLieu.doiChuoiSangInteger(param.Split('|')[0]);
                    ctDanhGia ctSelected = cartRate.getInfo(maMucTieu);
                    if (ctSelected != null)
                    {
                        //-----Thêm mới mục tiêu vào cart chưa Đánh giá
                        mucTieuDanhGia mucTieuAdd = new mucTieuDanhGia();
                        mucTieuAdd.dienGiai = ctSelected.mucTieuDanhGia.dienGiai; mucTieuAdd.ghiChu = ctSelected.mucTieuDanhGia.ghiChu;
                        mucTieuAdd.maMucTieu = ctSelected.mucTieuDanhGia.maMucTieu; mucTieuAdd.tenMucTieu = ctSelected.mucTieuDanhGia.tenMucTieu;
                        mucTieuAdd.trangThai = ctSelected.mucTieuDanhGia.trangThai;
                        cartTarget.addCart(mucTieuAdd); Session["chuaDanhGia"] = cartTarget;

                        //----Xóa mục tiêu đã đánh giá khỏi danh sách đã đánh giá
                        cartRate.removeItem(maMucTieu);
                        Session["daDanhGia"] = cartRate;
                    }
                }

            }
            catch (Exception ex)
            {
                xulyFile.ghiLoi("Class: DanhGiaController - Function: AjaxDanhGiaLai", ex.Message);
            }
            return AjaxLayDanhSachMucTieu() + "|" + taoBangMucTieuDaDanhGia();
        }
        /// <summary>
        /// Hàm thực hiện xóa tất cả các mục tiêu đã đánh giá trong giỏ
        /// </summary>
        /// <returns>Chuỗi html tạo danh sách các mục tiêu cần đánh giá</returns>
        public string AjaxXoaTatCaDanhGia()
        {
            try
            {
                this.resetSession();
                this.taoDanhSachMucTieuDanhGia(new qlCaPheEntities());            
            }
            catch (Exception ex)
            {
                xulyFile.ghiLoi("Class: DanhGiaController - Function: AjaxXoaTatCaDanhGia", ex.Message);
            }
            return AjaxLayDanhSachMucTieu() + "|"; //---thêm ký tự để xác định dữ liệu đổ lên bảng
        }
        #endregion
        #endregion
    }
}