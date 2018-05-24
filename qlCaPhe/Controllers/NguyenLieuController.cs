using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using qlCaPhe.App_Start;
using qlCaPhe.Models;

namespace qlCaPhe.Controllers
{
    public class NguyenLieuController : Controller
    {
        private string idOfPage = "702";
        #region CREATE
        /// <summary>
        /// Hàm tạo giao diện tạo mới nguyên liệu
        /// </summary>
        /// <returns></returns>
        public ActionResult nl_TaoMoiNguyenLieu()
        {
            if (xulyChung.duocCapNhat(idOfPage, "7"))
            {
                this.resetDuLieu();
                try
                {
                    taoDuLieuChoCbb(new qlCaPheEntities());
                }
                catch (Exception ex)
                {
                    xulyFile.ghiLoi("Class: NguyenLieuController - Function: nl_TaoMoiNguyenLieu_Get", ex.Message);
                }
            }
            return View();
        }
        /// <summary>
        /// hàm thêm mới 1 nguyên liệu vào CSDL
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult nl_TaoMoiNguyenLieu(nguyenLieu nl, FormCollection f)
        {

            if (xulyChung.duocCapNhat(idOfPage, "7"))
            {
                string ndThongBao = "";
                qlCaPheEntities db = new qlCaPheEntities();
                try
                {
                    this.layDuLieuTuView(nl, f);
                    nl.trangThai = true;
                    db.nguyenLieux.Add(nl);
                    db.SaveChanges();
                    ndThongBao = createHTML.taoNoiDungThongBao("Nguyên liệu", xulyDuLieu.traVeKyTuGoc(nl.tenNguyenLieu), "nl_TableNguyenLieu");
                    this.resetDuLieu();
                }
                catch (Exception ex)
                {
                    ndThongBao = ex.Message;
                    xulyFile.ghiLoi("Class: NguyenLieuController - Function: nl_TaoMoiNguyenLieu_Post", ex.Message);
                    ////-----Hiện lại dữ liệu trên giao diện
                    this.resetDuLieu();
                    this.doDuLieuLenView(nl, db);
                }
                this.taoDuLieuChoCbb(db);
                ViewBag.ThongBao = createHTML.taoThongBaoLuu(ndThongBao);
            }
            return View();
        }
        #endregion
        #region READ
        /// <summary>
        /// hàm tạo giao diện danh sách nguyên liệu đang sử dụng
        /// Trạng thái = true
        /// </summary>
        /// <returns></returns>
        public ActionResult nl_TableNguyenLieu()
        {
            if (xulyChung.duocTruyCap(idOfPage))
            {
                this.createTableNguyenLieu(true);
                return View();
            } return null;
        }
        /// <summary>
        /// hàm tạo giao diện danh sách nguyên liệu ngưng sử dụng
        /// Trạng thái = true
        /// </summary>
        /// <returns></returns>
        public ActionResult nl_TableNguyenLieuTamNgung()
        {
            if (xulyChung.duocTruyCap(idOfPage))
            {
                this.createTableNguyenLieu(false);
                return View();
            }
            return null;
        }
        /// <summary>
        /// Hàm tạo bảng nguyên liệu theo trạng thái
        /// </summary>
        /// <param name="trangThai">Trạng thái nguyên liệu cần lấy. True: Đang sử dụng, False: Tạm ngưng</param>
        private void createTableNguyenLieu(bool trangThai)
        {
            string htmlTable = "";
            try
            {
                qlCaPheEntities db = new qlCaPheEntities();
                foreach (nguyenLieu nl in db.nguyenLieux.ToList().Where(n => n.trangThai == trangThai).OrderBy(n => n.tenNguyenLieu))
                {
                    htmlTable += "<tr role=\"row\" class=\"odd\">";
                    htmlTable += "      <td>";
                    htmlTable += "          <a href=\"#\"  maHinh=\"" + nl.maNguyenLieu.ToString() + "\" class=\"goiY\">";
                    htmlTable += xulyDuLieu.traVeKyTuGoc(nl.tenNguyenLieu);
                    htmlTable += "              <span class=\"noiDungGoiY-right\">Click để xem hình</span>";
                    htmlTable += "          </a>";
                    htmlTable += "      </td>";
                    htmlTable += "      <td>" + xulyDuLieu.traVeKyTuGoc(nl.loaiNguyenLieu.tenLoai) + "</td>";
                    htmlTable += "      <td>" + xulyDuLieu.xulyCatChuoi(xulyDuLieu.traVeKyTuGoc(nl.moTa), 100) + "</td>";
                    htmlTable += "      <td>" + nl.tongSoLuong.ToString() + " " + xulyDuLieu.traVeKyTuGoc(nl.donViHienThi) + "</td>";
                    htmlTable += "      <td>" + nl.thoiHanSuDung.ToString() + " ngày</td>";
                    htmlTable += "      <td>";
                    htmlTable += "          <div class=\"btn-group\">";
                    htmlTable += "              <button type=\"button\" class=\"btn btn-primary dropdown-toggle\" data-toggle=\"dropdown\">";
                    htmlTable += "                  Chức năng <span class=\"caret\"></span>";
                    htmlTable += "              </button>";
                    htmlTable += "              <ul class=\"dropdown-menu\" role=\"menu\">";
                    htmlTable += createTableData.taoNutChinhSua("/NguyenLieu/nl_ChinhSuaNguyenLieu", nl.maNguyenLieu.ToString());
                    if (trangThai == true)//--Kiểm tra trạng thái đề hiện chức năng tương ứng. Trạng thái đang sử dụng có chức năng cập nhật lại là ngưng sử dụng
                        htmlTable += createTableData.taoNutCapNhat("/NguyenLieu/capNhatTrangThai", nl.maNguyenLieu.ToString(), "col-orange", "clear", "Ngưng sử dụng");
                    else
                        htmlTable += createTableData.taoNutCapNhat("/NguyenLieu/capNhatTrangThai", nl.maNguyenLieu.ToString(), "col-orange", "refresh", "Sử dụng lại");
                    htmlTable += createTableData.taoNutXoaBo(nl.maNguyenLieu.ToString());
                    htmlTable += "              </ul>";
                    htmlTable += "          </div>";
                    htmlTable += "      </td>";
                    htmlTable += "</tr>";
                }
            }
            catch (Exception ex)
            {
                xulyFile.ghiLoi("Class: NguyenLieuController - Function: createTableNguyenLieu", ex.Message);
                Response.Redirect(xulyChung.layTenMien() + "/Home/ServerError");
            }
            ViewBag.TableData = htmlTable;
            ViewBag.ScriptAjax = createScriptAjax.scriptAjaxXoaDoiTuong("NguyenLieu/xoaNguyenLieu?maNguyenLieu=");
            ViewBag.ModalXoa = createHTML.taoCanhBaoXoa("Nguyên liệu");
            //----Nhúng script ajax hiển thị hình khi người dùng click vào tên sản phẩm
            ViewBag.ScriptAjaxXemHinh = createScriptAjax.scriptAjaxXemChiTietKhiClick("goiY", "maHinh", "NguyenLieu/hienHinhNguyenLieu?maNguyenLieu=", "vungChiTiet", "modalChiTiet");
            //----Nhúng các tag html cho modal chi tiết
            ViewBag.ModalChiTiet = createHTML.taoModalChiTiet("modalChiTiet", "vungChiTiet", 1);
        }
        /// <summary>
        /// Hàm thực hiện hiển thị hình ảnh của đồ uống trên modal khi người dùng click vào tên đồ uống
        /// </summary>
        /// <param name="maDoUong"></param>
        /// <returns></returns>
        public string hienHinhNguyenLieu(int maNguyenLieu)
        {
            string kq = "";
            nguyenLieu nl = new qlCaPheEntities().nguyenLieux.SingleOrDefault(n => n.maNguyenLieu == maNguyenLieu);
            if (nl != null)
            {
                kq += "<div class=\"modal-header\">";
                kq += "      <button type=\"button\" class=\"close\" data-dismiss=\"modal\">×</button>";
                kq += "     <h4 class=\"modal-title\" id=\"defaultModalLabel\">" + xulyDuLieu.traVeKyTuGoc(nl.tenNguyenLieu) + "</h4>";
                kq += "</div>";
                kq += "<div class=\"modal-body\">";
                if (nl.hinhAnh != null)
                    kq += "     <img src=\"" + xulyDuLieu.chuyenByteHinhThanhSrcImage(nl.hinhAnh) + "\" style=\"max-width:100%; height:auto; width:500px;\" />";
                kq += "</div>";
                kq += "<div class=\"modal-footer\">";
                kq += "     <button type=\"button\" class=\"btn btn-link waves-effect\" data-dismiss=\"modal\">Đóng</button>";
                kq += "</div>";
            }
            return kq;
        }
        /// <summary>
        /// hàm thực hiện ajax để lấy danh sách nguyên liệu hiện lên modal tìm nguyên liệu
        /// </summary>
        /// <param name="tenNL"></param>
        /// <returns></returns>
        public string layDanhSachNguyenLieuTimKiem(string tenNL)
        {
            string kq = "";
            try
            {
                List<nguyenLieu> listNguyenLieu = new qlCaPheEntities().nguyenLieux.Where(n => n.tenNguyenLieu.StartsWith(tenNL) && n.trangThai==true).ToList();
                if (listNguyenLieu.Count() > 0)
                    foreach (nguyenLieu nl in listNguyenLieu)
                    {
                        kq += "<div class=\"media itemNguyenLieu\" manl=\"" + nl.maNguyenLieu.ToString() + "\" data-dismiss=\"modal\">";
                        kq += "     <div class=\"media-left\">";
                        kq += "          <img src=\"" + xulyDuLieu.chuyenByteHinhThanhSrcImage(nl.hinhAnh) + "\" width=\"64\" height=\"64\" />";
                        kq += "     </div>";
                        kq += "     <div class=\"media-body\">";
                        kq += "             <h4 class=\"media-heading\">" + nl.tenNguyenLieu + "</h4>";
                        kq += xulyDuLieu.traVeKyTuGoc(nl.moTa);
                        kq += "     </div>";
                        kq += "</div>";
                    }
                else
                    kq += " <h4 class=\"media-heading  font-bold col-pink\">Không tìm thấy nguyên liệu trên</h4>";
            }
            catch (Exception ex)
            {
                kq += ex.Message;
                xulyFile.ghiLoi("Class CongThucController - Function: layDanhSachNguyenLieuTimKiem", ex.Message);
            }
            return kq;
        }
        /// <summary>
        /// Hàm thực hiện lấy thông tin nguyên liệu khi đã click chọn trên modal
        /// </summary>
        /// <param name="maNL"></param>
        /// <returns></returns>
        public string layNguyenLieuModal(int maNL)
        {
            string kq = "";
            try
            {
                qlCaPheEntities db = new qlCaPheEntities();
                nguyenLieu nl = db.nguyenLieux.SingleOrDefault(n => n.maNguyenLieu == maNL);
                if (nl != null)
                {
                    //-----Lấy chi tiết
                    kq += "<img id=\"hinhNguyenLieu\" class='img img-responsive img-thumbnail'";
                    kq += "src=\"" + xulyDuLieu.chuyenByteHinhThanhSrcImage(nl.hinhAnh) + "\" width=\"250px\" height=\"auto\" />";
                    kq += "<br />";
                    kq += "<label id=\"lbTenNguyenLieu\" class=\"font-15 font-italic font-bold col-orange\">" + xulyDuLieu.traVeKyTuGoc(nl.tenNguyenLieu) + "</label>";
                    kq += "<input id=\"maNguyenLieuDaChon\" type=\"hidden\" value=\"" + nl.maNguyenLieu.ToString() + "\" />";
                }
            }
            catch (Exception ex)
            {
                xulyFile.ghiLoi("Class NguyenLieuController - Function: layNguyenLieuModal", ex.Message);
            }
            return kq;
        }
        #endregion
        #region UPDATE
        /// <summary>
        /// Hàm tạo giao diện chỉnh sửa nguyên liệu
        /// </summary>
        /// <param name="maNguyenLieu"></param>
        /// <returns></returns>
        public ActionResult nl_ChinhSuaNguyenLieu()
        {
            if (xulyChung.duocCapNhat(idOfPage, "7"))
            {
                try
                {
                    string param = xulyChung.nhanThamSoTrongSession();
                    if (param.Length > 0)
                    {
                        int maNguyenLieu = xulyDuLieu.doiChuoiSangInteger(param);
                        this.resetDuLieu();
                        qlCaPheEntities db = new qlCaPheEntities();
                        nguyenLieu nlSua = db.nguyenLieux.SingleOrDefault(n => n.maNguyenLieu == maNguyenLieu);
                        if (nlSua != null)
                            this.doDuLieuLenView(nlSua, db);
                        else
                            throw new Exception("Nguyên liệu có mã " + maNguyenLieu + " không tồn tại để truy cập cập nhật");
                    }
                    else throw new Exception("không nhận được tham số");

                }
                catch (Exception ex)
                {
                    xulyFile.ghiLoi("Class: NguyenLieuController - Function: nl_ChinhSuaNguyenLieu_Get", ex.Message);
                    return RedirectToAction("PageNotFound", "Home");
                }
            }
            return View();
        }

        /// <summary>
        /// Hàm thực hiện cập nhật 1 nguyên liệu trong csdl
        /// </summary>
        /// <param name="maNguyenLieu"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult nl_ChinhSuaNguyenLieu(FormCollection f)
        {
            if (xulyChung.duocCapNhat(idOfPage, "7"))
            {
                nguyenLieu nlSua = new nguyenLieu();
                qlCaPheEntities db = new qlCaPheEntities();
                try
                {
                    int maNguyenLieu = Convert.ToInt32(f["txtMaNguyenLieu"]);
                    nlSua = db.nguyenLieux.SingleOrDefault(n => n.maNguyenLieu == maNguyenLieu);
                    if (nlSua != null)
                    {
                        this.layDuLieuTuView(nlSua, f);
                        db.Entry(nlSua).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                        this.resetDuLieu();
                        //---Dựa vào trạng thái để chuyển đến trang tương ứng
                        if (nlSua.trangThai)
                            return RedirectToAction("nl_TableNguyenLieu");
                        else
                            return RedirectToAction("nl_TableNguyenLieuTamNgung");
                    }
                    else
                        throw new Exception("Nguyên liệu cần cập nhật không tồn tại");
                }
                catch (Exception ex)
                {
                    ViewBag.ThongBao = createHTML.taoThongBaoLuu(ex.Message);
                    xulyFile.ghiLoi("Class: NguyenLieuController - Function: nl_ChinhSuaNguyenLieu_Post", ex.Message);
                    this.resetDuLieu();
                    //----Hiện dữ liệu nguyên liệu lại lên view
                    this.doDuLieuLenView(nlSua, db);
                }
            }
            return View();
        }
        /// <summary>
        /// Hàm thực hiện cập nhật trạng thái của nguyên liệu.
        /// Trạng thái cập nhật sẽ ngược với trạng thái hiện có. Nếu là true thì thành false và ngược lại
        /// </summary>
        /// <param name="maNguyenLieu"></param>
        /// <returns></returns>
        public void capNhatTrangThai()
        {
            try
            {
                string param = xulyChung.nhanThamSoTrongSession();
                if (param.Length > 0)
                {
                    int maNguyenLieu = xulyDuLieu.doiChuoiSangInteger(param);
                    qlCaPheEntities db = new qlCaPheEntities();
                    nguyenLieu nlSua = db.nguyenLieux.SingleOrDefault(nl => nl.maNguyenLieu == maNguyenLieu);
                    if (nlSua != null)
                    {
                        bool trangThaiCu = nlSua.trangThai;//Lưu lại trạng thái cũ để chuyển đến trang tương ứng
                        nlSua.trangThai = !trangThaiCu;//Cập nhật trạng thái ngược lại với trạng thái cũ. Nếu true thì thành false và ngược lại
                        db.Entry(nlSua).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                        if (trangThaiCu == true)//Chuyển đến danh sách sản phẩm đang sử dụng
                            Response.Redirect(xulyChung.layTenMien() + "/NguyenLieu/nl_TableNguyenLieu");
                        else
                            Response.Redirect(xulyChung.layTenMien() + "/NguyenLieu/nl_TableNguyenLieuTamNgung");
                    }
                    else
                        throw new Exception("Nguyên liệu có mã " + maNguyenLieu + " cần cập nhật không tồn tại");
                }
                else throw new Exception("không nhận được tham số");
            }
            catch (Exception ex)
            {
                xulyFile.ghiLoi("Class: NguyenLieuController - Function: capNhatTrangThai", ex.Message);
                Response.Redirect(xulyChung.layTenMien() + "/Home/ServerError");
            }
        }
        #endregion
        #region DELETE
        /// <summary>
        /// Hàm thực hiện xóa 1 nguyên liệu khỏi CSDL
        /// </summary>
        /// <param name="maNguyenLieu"></param>
        public void xoaNguyenLieu(int maNguyenLieu)
        {
            try
            {
                qlCaPheEntities db = new qlCaPheEntities();
                nguyenLieu nlXoa = db.nguyenLieux.Single(nl => nl.maNguyenLieu == maNguyenLieu);
                if (nlXoa != null)
                {
                    db.nguyenLieux.Remove(nlXoa);
                    db.SaveChanges();
                }
                else
                    throw new Exception("Nguyên liệu có mã " + maNguyenLieu + " không tồn tại để xóa bỏ");
            }
            catch (Exception ex)
            {
                xulyFile.ghiLoi("Class: NguyenLieuController - Function: xoaLoaiNguyenLieu", ex.Message);
                Response.Redirect(xulyChung.layTenMien() + "/Home/PageNotFound");
            }
        }
        #endregion
        #region ORTHERS
        /// <summary>
        /// Hàm thực hiện tạo dữ liệu cho combobox 
        /// </summary>
        /// <param name="db"></param>
        private void taoDuLieuChoCbb(qlCaPheEntities db)
        {
            string htmlCbb = "";
            foreach (loaiNguyenLieu loai in db.loaiNguyenLieux.ToList())
                htmlCbb += "<option value=\"" + loai.maLoai.ToString() + "\">" + xulyDuLieu.traVeKyTuGoc(loai.tenLoai) + "</option>";
            ViewBag.cbbLoai = htmlCbb;
        }
        /// <summary>
        /// hàm thực hiện lấy dữ liệu từ View gán cho các thuộc tính của nguyenLieu
        /// </summary>
        /// <param name="nl"></param>
        /// <param name="f"></param>
        private void layDuLieuTuView(nguyenLieu nl, FormCollection f)
        {
            string loi = ""; //Biến ghi nhận thông báo lỗi
            nl.tenNguyenLieu = xulyDuLieu.xulyKyTuHTML(f["txtTenNguyenLieu"]);
            if (nl.tenNguyenLieu.Length <= 0)
                loi += "Vui lòng nhập tên nguyên liệu <br/>";
            nl.maLoai = xulyDuLieu.doiChuoiSangInteger(f["cbbLoai"]);
            if (nl.maLoai <= 0)
                loi += "Vui lòng chọn loại nguyên liệu <br/>";
            nl.moTa = xulyDuLieu.xulyKyTuHTML(f["txtMoTa"]);
            if (nl.moTa.Length <= 0)
                loi += "Vui lòng nhập thông tin mô tả nguyên liệu </br>";
            nl.thoiHanSuDung = xulyDuLieu.doiChuoiSangInteger(f["txtThoiHan"]);
            if (nl.thoiHanSuDung <= 0)
                loi += "Thời gian sử dụng phải lớn hơn 0 <br/>";
            nl.donViHienThi = xulyDuLieu.xulyKyTuHTML(f["txtDonViHienThi"]);
            nl.donViPhaChe = xulyDuLieu.xulyKyTuHTML(f["txtDonViPhaChe"]);
            nl.tyLeChuyenDoi = xulyDuLieu.doiChuoiSangDouble(f["txtTyLeChuyenDoi"]);
            nl.ghiChu = xulyDuLieu.xulyKyTuHTML(f["txtGhiChu"]);
            string pathHinh = f["pathHinh"];
            //------Kiểm tra xem có chọn hình không. 
            if (!pathHinh.Equals("")) //Nếu có chọn hình  thì luu hình vào csdl
            {
                pathHinh = xulyMaHoa.Decrypt(pathHinh);//Giải mã lại chuỗi đường dẫn lưu hình ảnh trên đĩa đã được mã hóa trong ajax
                nl.hinhAnh = xulyDuLieu.chuyenDoiHinhSangByteArray(pathHinh); //Lưu hình vào thuộc tính hinhAnh
            }
            else //-------Nếu không có chọn hình
                if (f["txtMaNguyenLieu"] == null) //--------Kiểm tra xem có mã sản phẩm không. Nếu không tức là thêm mới và báo lỗi
                    loi += "Vui lòng chọn hình ảnh cho nguyên liệu<br/>";
            //Nếu có lỗi thì xuất thông báo
            if (loi.Length > 0)
                throw new Exception(loi);
        }
        /// <summary>
        /// hàm thực hiện đổ dữ liệu của nguyên liệu lên giao diện
        /// <param name="nl">Đối tượng nguyên liệu cần đổ lên giao diện</param>
        /// <param name="db">Class kết nối để lấy danh sách loại nguyên liệu cho combobox</param>
        /// </summary>
        private void doDuLieuLenView(nguyenLieu nl, qlCaPheEntities db)
        {
            string htmlCbb = "";
            ViewBag.txtMaNguyenLieu = nl.maNguyenLieu.ToString();
            //------Đổ danh sách loại nguyên liệu lên Combobox
            foreach (loaiNguyenLieu loai in db.loaiNguyenLieux.ToList())
                if (loai.maLoai == nl.maLoai)
                    htmlCbb += "<option selected value=\"" + loai.maLoai.ToString() + "\">" + xulyDuLieu.traVeKyTuGoc(loai.tenLoai) + "</option>";
                else
                    htmlCbb += "<option value=\"" + loai.maLoai.ToString() + "\">" + xulyDuLieu.traVeKyTuGoc(loai.tenLoai) + "</option>";
            ViewBag.cbbLoai = htmlCbb;
            ViewBag.txtTenNguyenLieu = xulyDuLieu.traVeKyTuGoc(nl.tenNguyenLieu);
            ViewBag.txtMoTa = xulyDuLieu.traVeKyTuGoc(nl.moTa);
            ViewBag.txtThoiHan = nl.thoiHanSuDung.ToString();
            ViewBag.txtDonViHienThi = xulyDuLieu.traVeKyTuGoc(nl.donViHienThi);
            ViewBag.txtDonViPhaChe = xulyDuLieu.traVeKyTuGoc(nl.donViPhaChe);
            ViewBag.txtTyLeChuyenDoi = nl.tyLeChuyenDoi.ToString();
            ViewBag.txtGhiChu = xulyDuLieu.traVeKyTuGoc(nl.ghiChu);

            if (nl.hinhAnh != null)
                ViewBag.hinhDD = xulyDuLieu.chuyenByteHinhThanhSrcImage(nl.hinhAnh);
        }
        /// <summary>
        /// Hàm thiệt lập lại giao diện và nhúng SCript ajax
        /// </summary>
        private void resetDuLieu()
        {
            //Gán hình mặc định 
            ViewBag.hinhDD = "/images/gallery-upload-256.png";
            //Xóa tất cả các hình đã tải lên trong tập tin tạm            
            xulyFile.donDepTM(Server.MapPath("~/pages/temp/nguyenLieu"));
            //Nhúng script ajax thực hiện tải và cắt hình
            ViewBag.ScriptCropImage = createScriptAjax.scriptAjaxUpLoadAndCropImage("nguyenLieu", 268, 185, 268, 185);
            //Gán modal up và crop hình ảnh lên view
            ViewBag.modalChonHinh = createHTML.taoModalUpHinh();
        }
        #endregion
    }
}