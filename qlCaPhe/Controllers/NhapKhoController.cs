using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using qlCaPhe.App_Start;
using qlCaPhe.Models;
using qlCaPhe.Models.Business;

namespace qlCaPhe.Controllers
{
    public class NhapKhoController : Controller
    {
        private string idOfPage = "801";
        #region CREATE
        /// <summary>
        /// Hàm tạo giao diện nhập hàng vào kho hàng
        /// </summary>
        /// <returns></returns>
        public ActionResult nk_TaoMoiPhieuNhap()
        {
            if (xulyChung.duocTruyCap(idOfPage))
            {
                try
                {
                    qlCaPheEntities db = new qlCaPheEntities();
                    this.taoDuLieuChoCbbKhoHang(db);
                    this.taoDuLieuChoCbbNhaCungCap(db);
                    this.resetData();
                    xulyChung.ghiNhatKyDtb(1, "Tạo mới nhập kho");
                }
                catch (Exception ex)
                {
                    xulyFile.ghiLoi("Class: NhapKhoController - Function: nk_TaoMoiPhieuNhap", ex.Message);
                }
            }
            return View();
        }

        [HttpPost]
        public ActionResult nk_TaoMoiPhieuNhap(phieuNhapKho phieu, FormCollection f)
        {
            if (xulyChung.duocCapNhat(idOfPage, "7"))
            {
                string ndThongBao = ""; int kqLuu = 0;
                qlCaPheEntities db = new qlCaPheEntities();
                try
                {
                    this.layDuLieuTrenView(phieu, f);
                    db.phieuNhapKhoes.Add(phieu);
                    kqLuu = db.SaveChanges();
                    if (kqLuu > 0)
                    {
                        //----Thêm dữ liệu vào bảng chi tiết và bảng tồn kho
                        this.themctPhieuNhapKhoTrongDatabase(phieu.maPhieu, db);
                        ndThongBao = createHTML.taoNoiDungThongBao("Phiếu nhập kho", phieu.maPhieu.ToString(), "/NhapKho/nk_TablePhieuNhap");
                        this.resetData();
                        xulyChung.ghiNhatKyDtb(2, "Phiếu nhập kho có mã  \" " + phieu.maPhieu.ToString() + " \"");
                    }
                }
                catch (Exception ex)
                {
                    ndThongBao = ex.Message;
                    xulyFile.ghiLoi("Class: NhapKhoController - Function: nk_TaoMoiPhieuNhap_Post", ex.Message);
                    this.doDuLieuLenView(phieu, db);
                    this.taoDuLieuChoCbbNhaCungCap(db);
                    this.taoDuLieuChoCbbKhoHang(db);
                }
                ViewBag.ThongBao = createHTML.taoThongBaoLuu(ndThongBao);
            }
            return View();
        }
        private void themctPhieuNhapKhoTrongDatabase(int maPhieu, qlCaPheEntities db)
        {
            cartNhapKho cart = (cartNhapKho)Session["ctNhapKho"];
            if (cart.Item.Count <= 0)
                throw new Exception("Vui lòng nhập hàng vào phiếu <br/>");
            //----Lặp qua từng phần tử có trong session
            foreach (ctPhieuNhapKho ctTam in cart.getListForTable())
            {
                ctPhieuNhapKho ctAdd = new ctPhieuNhapKho();
                ctAdd.maPhieu = maPhieu;
                ctAdd.maNguyenLieu = ctTam.maNguyenLieu;
                ctAdd.maNhaCC = ctTam.maNhaCC;
                ctAdd.ghiChu = ctTam.ghiChu;
                ctAdd.donGiaNhap = ctTam.donGiaNhap;
                ctAdd.soLuongNhap = ctTam.soLuongNhap;
                db.ctPhieuNhapKhoes.Add(ctAdd);
                db.SaveChanges();
                //---------Cập nhật nguyên liệu vào tồn kho
                //  this.insertNguyenLieuVaoTonKhoDauKy(ctAdd.maNguyenLieu, ctAdd.soLuongNhap, ctAdd.donGiaNhap, db);
            }

        }
        /// <summary>
        /// Hàm thực hiện thêm nguyên liệu vào Session chứa chi tiết phiếu nhập kho
        /// </summary>
        /// <param name="duLieu">Chuỗi dữ liệu nhận từ giao diện</param>
        /// <returns></returns>
        public string AjaxThemChiTietVaTraVeBang(string duLieu)
        {
            string kq = ""; long tongTienNhap = 0;
            cartNhapKho cart = (cartNhapKho)Session["ctNhapKho"];
            try
            {
                ctPhieuNhapKho chiTietAdd = new ctPhieuNhapKho();
                this.layDuLieuTuViewChiTiet(chiTietAdd, duLieu);
                //------Thêm chi tiết vào session
                cart.addCart(chiTietAdd);
                //Cập nhật lại session
                Session["ctNhapKho"] = cart;
                tongTienNhap = cart.getTotalPrice();
            }
            catch (Exception ex)
            {
                xulyFile.ghiLoi("Class NhapKhoController - Function: AjaxThemChiTietVaTraVeBang", ex.Message);
            }
            kq += taoBangChiTietTuSession(); //------Tạo lại bảng danh sách các bước có trong session và hiện lên giao diện
            return kq + "|" + tongTienNhap.ToString(); //-------Trả về chuỗi html tạo nên bảng và chuỗi nội dung thông báo. Sau đó cắt ra và hiện lên giao diện
        }
        #endregion
        #region READ
        /// <summary>
        /// Hàm tạo giao diện danh sách phiếu nhập kho
        /// </summary>
        /// <returns></returns>
        public ActionResult nk_TablePhieuNhap(int? page)
        {
            int trangHienHanh = (page ?? 1);
            if (xulyChung.duocTruyCap(idOfPage))
            {
                string htmlTable = "";
                try
                {
                    qlCaPheEntities db = new qlCaPheEntities();
                    int soPhanTu = db.phieuNhapKhoes.Count();
                    ViewBag.PhanTrang = createHTML.taoPhanTrang(soPhanTu, createHTML.pageSize, trangHienHanh, "/NhapKho/nk_TablePhieuNhap"); //------cấu hình phân trang
                    foreach (phieuNhapKho phieu in db.phieuNhapKhoes.ToList().OrderBy(p => p.maPhieu).OrderByDescending(p => p.ngayNhap).Skip((trangHienHanh - 1) * createHTML.pageSize).Take(createHTML.pageSize))
                    {
                        htmlTable += "<tr role=\"row\" class=\"odd\">";
                        htmlTable += "      <td>";
                        htmlTable += "          <a href=\"#\" data-toggle=\"modal\" data-target=\"#modalChiTiet\" maPhieu=\"" + phieu.maPhieu.ToString() + "\" class=\"goiY\">";
                        htmlTable += phieu.maPhieu.ToString();
                        htmlTable += "              <span class=\"noiDungGoiY-right\">Click để xem chi tiết</span>";
                        htmlTable += "          </a>";
                        htmlTable += "      </td>";
                        htmlTable += "      <td>" + xulyDuLieu.traVeKyTuGoc(phieu.khoHang.tenKhoHang) + "</td>";
                        htmlTable += "      <td>" + phieu.ngayNhap.ToString() + "</td>";
                        htmlTable += "      <td>" + phieu.tongTien.ToString() + "</td>";
                        htmlTable += "      <td>" + xulyDuLieu.traVeKyTuGoc(phieu.taiKhoan.thanhVien.hoTV) + " " + xulyDuLieu.traVeKyTuGoc(phieu.taiKhoan.thanhVien.tenTV) + "</td>";
                        htmlTable += "      <td>";
                        htmlTable += "          <div class=\"btn-group\">";
                        htmlTable += "              <button type=\"button\" class=\"btn btn-primary dropdown-toggle\" data-toggle=\"dropdown\">";
                        htmlTable += "                  Chức năng <span class=\"caret\"></span>";
                        htmlTable += "              </button>";
                        htmlTable += "              <ul class=\"dropdown-menu\" role=\"menu\">";
                        htmlTable += createTableData.taoNutChinhSua("/NhapKho/nk_ChinhSuaPhieuNhap", phieu.maPhieu.ToString());
                        htmlTable += createTableData.taoNutXoaBo(phieu.maPhieu.ToString());
                        htmlTable += "              </ul>";
                        htmlTable += "          </div>";
                        htmlTable += "      </td>";
                        htmlTable += "</tr>";
                    }
                }
                catch (Exception ex)
                {
                    xulyFile.ghiLoi("Class: NhapKhoController - Function: nk_TablePhieuNhap", ex.Message);
                }
                ViewBag.TableData = htmlTable;
                ViewBag.ModalChiTiet = createHTML.taoModalChiTiet("modalChiTietPhieu", "idVungChiTiet", 3);
                ViewBag.ScriptAjaxXemChiTiet = createScriptAjax.scriptAjaxXemChiTietKhiClick("goiY", "maPhieu", "NhapKho/AjaxLayChiTietPhieu?maPhieu=", "idVungChiTiet", "modalChiTietPhieu");
                ViewBag.ScriptAjax = createScriptAjax.scriptAjaxXoaDoiTuong("NhapKho/AjaxXoaPhieuNhap?maPhieu=");
                ViewBag.ModalXoa = createHTML.taoCanhBaoXoa("Phiếu nhập hàng");
                xulyChung.ghiNhatKyDtb(1, "Danh mục phiếu nhập kho");
            }
            return View();
        }
        /// <summary>
        /// Hàm tạo dữ liệu cho bảng chi tiết nguyên liệu trên phiếu và hiện lên giao diện 
        /// </summary>
        /// <returns></returns>
        public string taoBangChiTietTuSession()
        {
            string kq = "";
            try
            {
                //---Lấy dữ liệu từ cart
                cartNhapKho cart = (cartNhapKho)Session["ctNhapKho"]; bNguyenLieu bNguyenLieu = new Models.Business.bNguyenLieu();
                kq += "<table class=\"table table-hover\">";
                kq += "     <thead>";
                kq += "         <tr>";
                //----Tạo tiêu đề cho bảng chi tiết
                kq += "             <th>Tên nguyên liệu</th><th>Nhà cung cấp</th><th>Số lượng</th><th>Đơn giá nhập</th><th>Chức năng</th>";
                kq += "         </tr>";
                kq += "     </thead>";
                kq += "     <tbody>";
                foreach (ctPhieuNhapKho ct in cart.getListForTable()) //--------Lặp qua từng phần tử có trong Session 
                {
                    kq += "         <tr>";
                    kq += "             <td>";
                    kq += "                 <img width=\"50px\" height=\"auto;\" src=\"" + xulyDuLieu.chuyenByteHinhThanhSrcImage(ct.nguyenLieu.hinhAnh) + "\">";
                    kq += xulyDuLieu.traVeKyTuGoc(ct.nguyenLieu.tenNguyenLieu);
                    kq += "             </td>";
                    kq += "             <td>" + xulyDuLieu.traVeKyTuGoc(ct.nhaCungCap.tenNhaCC) + "</td>";
                    kq += "             <td>" + bNguyenLieu.chuyenDoiDonViNhoSangLon(ct.soLuongNhap, ct.nguyenLieu).ToString() + " " + xulyDuLieu.traVeKyTuGoc(ct.nguyenLieu.donViHienThi) + "</td>";
                    kq += "             <td>" + ct.donGiaNhap.ToString() + "</td>";
                    kq += "             <td>";
                    //-----------Lấy mã nguyên liệu làm key trong list session để xác định chi tiết cần xóa
                    kq += "                 <button type=\"button\" maCt=\"" + ct.maNguyenLieu + "\" class=\"btn btn-danger xoaChiTiet\">Xoá nguyên liệu</button>";
                    kq += "             </td>";
                    kq += "         </tr>";
                }
                kq += "     </tbody>";
                kq += "</table>";
                //---Hiện thông tin tổng tiền nhập nguyên liệu
                kq += "<label class=\"font-20 col-red bold\" id=\"TongTienCart";
                kq += "\">Tổng số tiền nhập nguyên liệu: " + cart.getTotalPrice().ToString() + " VNĐ</label><br />";
            }
            catch (Exception ex)
            {
                xulyFile.ghiLoi("Class NhapKhoController - Function: taoBangChiTietTuSession", ex.Message);
            }
            return kq;
        }
        /// <summary>
        /// Hàm thực hiện tạo html bảng danh sách chi tiết phiếu và nhúng vào Modal khi người dùng click vào mã phiếu để xem
        /// </summary>
        /// <param name="maPhieu"></param>
        /// <returns></returns>
        public string AjaxLayChiTietPhieu(int maPhieu)
        {
            string kq = "";
            if (xulyChung.duocTruyCap(idOfPage))
            {
                qlCaPheEntities db = new qlCaPheEntities();
                phieuNhapKho phieuNhap = db.phieuNhapKhoes.SingleOrDefault(p => p.maPhieu == maPhieu);
                if (phieuNhap != null)
                {
                    kq += "<div class=\"modal-header\">";
                    kq += "      <button type=\"button\" class=\"close\" data-dismiss=\"modal\">×</button>";
                    kq += "    <h3 class=\"modal-title\" id=\"largeModalLabel\">CHI TIẾT PHIẾU NHẬP " + phieuNhap.maPhieu.ToString() + "</h3>";
                    kq += "</div>";
                    kq += "<div class=\"modal-body\">";
                    kq += "    <div class=\"row\">";
                    kq += "        <div class=\"col-md-12 col-lg-12\">";
                    kq += "            <div class=\"card\">";
                    kq += "                <div class=\"header bg-cyan\">";
                    kq += "                    <h2> Danh mục nguyên liệu trong phiếu nhập</h2>";
                    kq += "                </div>";
                    kq += "                <div class=\"body table-responsive\">";
                    kq += "                <!--Nội dung-->";
                    kq += "                     <table class=\"table table-hover\">";
                    kq += "                         <thead>";
                    kq += "                             <tr>";
                    kq += "                                 <th>Tên nguyên liệu</th>";
                    kq += "                                 <th>Nhà cung cấp</th>";
                    kq += "                                 <th>Số lượng nhập</th>";
                    kq += "                                 <th>Đơn giá nhập</th>";
                    kq += "                                 <th>Ghi chú</th>";
                    kq += "                             </tr>";
                    kq += "                         </thead>";
                    kq += "                         <tbody>";
                    foreach (ctPhieuNhapKho ct in phieuNhap.ctPhieuNhapKhoes.ToList())
                    {
                        kq += "                             <tr>";
                        kq += "                                 <td>";
                        kq += "                                     <img width=\"50px;\" height=\"50px;\" src=\"" + xulyDuLieu.chuyenByteHinhThanhSrcImage(ct.nguyenLieu.hinhAnh) + "\" />";
                        kq += "                                     <b>" + xulyDuLieu.traVeKyTuGoc(ct.nguyenLieu.tenNguyenLieu) + "</b>";
                        kq += "                                 </td>";
                        kq += "                                 <td>" + xulyDuLieu.traVeKyTuGoc(ct.nhaCungCap.tenNhaCC) + "</td>";
                        kq += "                                 <td>" + ((int)(ct.soLuongNhap / ct.nguyenLieu.tyLeChuyenDoi)).ToString() + " " + ct.nguyenLieu.donViHienThi + "</td>";
                        kq += "                                 <td>" + ct.donGiaNhap.ToString() + "</td>";
                        kq += "                                 <td>" + xulyDuLieu.traVeKyTuGoc(ct.ghiChu) + "</td>";
                        kq += "                             </tr>";
                    }
                    kq += "                         </tbody>";
                    kq += "                     </table>";
                    kq += "                </div>";
                    kq += "            </div>";
                    kq += "        </div>";
                    kq += "</div>";
                    kq += "<div class=\"modal-footer\">";
                    kq += "     <div class=\"col-md-9\">";
                    kq += "         <div class=\"pull-left\">";
                    kq += "             <label class=\"font-18 pull-left col-red bold\">Tổng tiền: " + phieuNhap.tongTien.ToString() + " VNĐ</label><br />";
                    kq += "             <label class=\"pull-left col-blue-grey\"><i>* Ghi chú: </i>" + xulyDuLieu.traVeKyTuGoc(phieuNhap.ghiChu) + "</label>";
                    kq += "         </div>";
                    kq += "     </div>";
                    kq += "     <div class=\"col-md-3\">";
                    kq += "         <button type=\"button\" class=\"btn btn-default waves-effect\" data-dismiss=\"modal\"><i class=\"material-icons\">exit_to_app</i>Đóng lại</button>";
                    kq += "     </div>";
                    kq += "</div>";
                    xulyChung.ghiNhatKyDtb(5, "Chi tiết phiếu nhập kho \"" + phieuNhap.maPhieu.ToString() + " \"");
                }
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
                nguyenLieu nl = new qlCaPheEntities().nguyenLieux.SingleOrDefault(n => n.maNguyenLieu == maNL);
                if (nl != null)
                {
                    //-----Lấy chi tiết
                    kq += "<img id=\"hinhNguyenLieu\" class='img img-responsive img-thumbnail'";
                    kq += "src=\"" + xulyDuLieu.chuyenByteHinhThanhSrcImage(nl.hinhAnh) + "\" width=\"250px\" height=\"auto\" />";
                    kq += "<br />";
                    kq += "<label id=\"lbTenNguyenLieu\" class=\"font-15 font-italic font-bold col-orange\">" + xulyDuLieu.traVeKyTuGoc(nl.tenNguyenLieu) + "</label>";
                    kq += "<input id=\"maNguyenLieuDaChon\" type=\"hidden\" value=\"" + nl.maNguyenLieu.ToString() + "\" />";
                    kq += "<div class=\"row clearfix\">";
                    kq += "     <div class=\"col-lg-5 col-md-5 col-sm-5 col-xs-6\">";
                    kq += "         <label for=\"txtSoLuongNhap\">Số lượng:</label>";
                    kq += "         <div class=\"form-group\">";
                    kq += "             <div class=\"form-line\">";
                    kq += "                 <input type=\"text\" id=\"txtSoLuongNhap\" class=\"form-control\" placeholder=\"Nhập số lượng nhập\">";
                    kq += "             </div>";
                    kq += "         </div>";
                    kq += "     </div>";
                    kq += "     <div class=\"col-lg-1 col-md-1 col-sm-1 col-xs-6\">";
                    kq += "         <br>";
                    kq += "         <label for=\"txtSoLuongNhap\" id=\"donViTinh\"><b>" + xulyDuLieu.traVeKyTuGoc(nl.donViHienThi) + "</b></label>";
                    kq += "     </div>";
                    kq += "     <div class=\"col-lg-6 col-md-6 col-sm-6 col-xs-6\">";
                    kq += "         <label for=\"txtDonGiaNhap\">Đơn giá:</label>";
                    kq += "         <div class=\"form-group\">";
                    kq += "             <div class=\"form-line\">";
                    kq += "                 <input type=\"text\" class=\"form-control\" id=\"txtDonGiaNhap\" placeholder=\"Nhập đơn giá nguyên liệu\">";
                    kq += "             </div>";
                    kq += "         </div>";
                    kq += "     </div>";
                    kq += "</div>";
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
        /// Hàm thực hiện tạo giao diện nhập dữ liệu vào phiếu
        /// </summary>
        /// <param name="maPhieu"></param>
        /// <returns></returns>
        public ActionResult nk_ChinhSuaPhieuNhap()
        {
            if (xulyChung.duocCapNhat(idOfPage, "7"))
            {
                try
                {
                    string param = xulyChung.nhanThamSoTrongSession();
                    if (param.Length > 0)
                    {
                        int maPhieu = xulyDuLieu.doiChuoiSangInteger(param);
                        qlCaPheEntities db = new qlCaPheEntities();
                        phieuNhapKho phieu = db.phieuNhapKhoes.SingleOrDefault(p => p.maPhieu == maPhieu);
                        if (phieu != null)
                        {
                            this.resetData();
                            this.taoDuLieuChoCbbNhaCungCap(db);
                            this.doDuLieuLenView(phieu, db);
                            //-------Gán dữ liệu có trong bảng chi tiết vào SEssion
                            cartNhapKho cart = (cartNhapKho)Session["ctNhapKho"];
                            foreach (ctPhieuNhapKho ct in db.ctPhieuNhapKhoes.ToList().Where(c => c.maPhieu == maPhieu))
                            {
                                cart.addCart(ct);
                                Session["ctNhapKho"] = cart;
                            }
                            xulyChung.ghiNhatKyDtb(1, "Chỉnh sửa phiếu nhập kho có mã \" " + phieu.maPhieu.ToString() + " \"");
                        }
                    }
                    else throw new Exception("không nhận được tham số");



                }
                catch (Exception ex)
                {
                    xulyFile.ghiLoi("Class: NhapKhoController - Function: nk_ChinhSuaPhieuNhap_Get", ex.Message);
                }
            }
            return View();
        }
        /// <summary>
        /// Hàm thực hiện cập nhật phiếu và chi tiết vào CSDL
        /// </summary>
        /// <param name="maPhieu"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult nk_ChinhSuaPhieuNhap(FormCollection f)
        {
            if (xulyChung.duocCapNhat(idOfPage, "7"))
            {
                int kqLuu = 0;
                qlCaPheEntities db = new qlCaPheEntities(); phieuNhapKho phieuSua = new phieuNhapKho();
                try
                {
                    int maPhieuSua = xulyDuLieu.doiChuoiSangInteger(f["txtMaPhieu"]);
                    phieuSua = db.phieuNhapKhoes.SingleOrDefault(p => p.maPhieu == maPhieuSua);
                    if (phieuSua != null)
                    {
                        this.layDuLieuTrenView(phieuSua, f);
                        db.Entry(phieuSua).State = System.Data.Entity.EntityState.Modified;
                        kqLuu = db.SaveChanges();
                        if (kqLuu > 0)
                        {
                            //-------Xóa tất cả dữ liệu trong chi tiết và tạo lại
                            this.xoaChiTietTrongDatabase(phieuSua.maPhieu, db);
                            //-------Thêm lại dữ liệu cho bảng chi tiết
                            this.themctPhieuNhapKhoTrongDatabase(phieuSua.maPhieu, db);
                            this.resetData();
                            xulyChung.ghiNhatKyDtb(4, "Phiếu nhập kho có mã  \" " + phieuSua.maPhieu.ToString() + " \"");
                            return RedirectToAction("nk_TablePhieuNhap");
                        }
                    }
                }
                catch (Exception ex)
                {
                    ViewBag.ThongBao = createHTML.taoThongBaoLuu(ex.Message);
                    xulyFile.ghiLoi("Class: NhapKhoController - Function: nk_ChinhSuaPhieuNhap_Post", ex.Message);
                    this.doDuLieuLenView(phieuSua, db);
                    this.taoDuLieuChoCbbNhaCungCap(db);
                }
            }
            return View();
        }
        #endregion
        #region DELETE
        /// <summary>
        /// Hàm thực thi xóa 1 chi tiết trong session
        /// </summary>
        /// <param name="maCT">Mã xác định </param>
        /// <returns>Html tạo bảng đã được cập nhật dữ liệu</returns>
        public string AjaxXoaMotChitiet(int maCT)
        {
            try
            {
                cartNhapKho cart = (cartNhapKho)Session["ctNhapKho"];
                cart.removeItem(maCT);
                //----Cập nhật lại session
                Session["ctNhapKho"] = cart;
            }
            catch (Exception ex)
            {
                xulyFile.ghiLoi("Class NhapKhoController - Function: AjaxXoaMotChitiet", ex.Message);
            }
            return this.taoBangChiTietTuSession();
        }
        /// <summary>
        /// Hàm thực hiện xóa tất cả dữ liệu trong SEssion và tạo lại session
        /// </summary>
        /// <returns></returns>
        public void AjaxXoaTatCaChiTiet()
        {
            Session.Remove("ctNhapKho"); Session.Add("ctNhapKho", new cartNhapKho());
        }
        /// <summary>
        /// hàm thực hiện xóa 1 phiếu nhập kho hàng khỏi CSDL
        /// </summary>
        /// <param name="maPhieu"></param>
        public void AjaxXoaPhieuNhap(int maPhieu)
        {
            try
            {
                int kqLuu = 0;
                qlCaPheEntities db = new qlCaPheEntities();
                phieuNhapKho phieuXoa = db.phieuNhapKhoes.SingleOrDefault(p => p.maPhieu == maPhieu);
                if (phieuXoa != null)
                {
                    this.xoaChiTietTrongDatabase(phieuXoa.maKho, db);
                    //--Xóa tất cả dữ liệu trong chi tiết trước.
                    db.phieuNhapKhoes.Remove(phieuXoa);
                    kqLuu = db.SaveChanges();
                    if (kqLuu > 0)
                        xulyChung.ghiNhatKyDtb(3, "Phiếu nhập kho có mã là \"" + phieuXoa.maPhieu.ToString() + " \"");
                }
            }
            catch (Exception ex)
            {
                xulyFile.ghiLoi("Class: NhapKhoController - Function: AjaxXoaPhieuNhap", ex.Message);
            }
        }
        /// <summary>
        /// Hàm thực hiện xóa tất cả chi tiết phiếu nhập trong CSDL
        /// </summary>
        /// <param name="maPhieu"></param>
        private void xoaChiTietTrongDatabase(int maPhieu, qlCaPheEntities db)
        {
            foreach (ctPhieuNhapKho ct in db.ctPhieuNhapKhoes.ToList().Where(c => c.maPhieu == maPhieu))
            {
                db.ctPhieuNhapKhoes.Remove(ct);
                db.SaveChanges();
            }
        }
        #endregion
        #region ORTHERS
        /// <summary>
        /// Hàm tạo dữ liệu cho combobox 
        /// </summary>
        /// <param name="db"></param>
        private void taoDuLieuChoCbbKhoHang(qlCaPheEntities db)
        {
            string htmlCbb = "";
            foreach (khoHang kh in db.khoHangs.ToList().OrderBy(s => s.tenKhoHang))
                htmlCbb += "<option value=\"" + kh.maKhoHang.ToString() + "\">" + xulyDuLieu.traVeKyTuGoc(kh.tenKhoHang) + " - " + xulyDuLieu.traVeKyTuGoc(kh.diaChi) + "</option>";
            ViewBag.cbbKhoHang = htmlCbb;
        }
        /// <summary>
        /// hàm tạo dữ liệu cho nhà cung cấp
        /// </summary>
        /// <param name="db"></param>
        private void taoDuLieuChoCbbNhaCungCap(qlCaPheEntities db)
        {
            string htmlCbb = "";
            foreach (nhaCungCap ncc in db.nhaCungCaps.ToList().OrderBy(s => s.tenNhaCC))
                htmlCbb += "<option value=\"" + ncc.maNhaCC.ToString() + "\">" + xulyDuLieu.traVeKyTuGoc(ncc.tenNhaCC) + "</option>";
            ViewBag.cbbNhaCC = htmlCbb;
        }
        private void resetData()
        {
            //--Xoá session chứa danh sách sản phẩm nhập kho 
            Session.Remove("ctNhapKho");
            //Tạo mới lại session
            Session.Add("ctNhapKho", new cartNhapKho());
        }
        /// <summary>
        /// Hàm thêm dữ liệu từ giao diện cho object ctPhieuNhapKho
        /// </summary>
        /// <param name="chiTiet"></param>
        /// <param name="duLieu">Chuỗi dữ liệu được lấy khi ajax gửi về. Dữ liệu là các giá trị trong textbox nhập vào chi tiết
        /// có dạng: maNguyenLieu|maNhaCungCap|soLuongNhap|donGiaNhap|ghiChu</param>
        private void layDuLieuTuViewChiTiet(ctPhieuNhapKho chiTiet, string duLieu)
        {
            string loi = ""; qlCaPheEntities db = new qlCaPheEntities();
            //------Thực hiện xử lý cắt chuỗi duLieu để lấy giá trị của các thuộc tính
            int maNguyenLieu = xulyDuLieu.doiChuoiSangInteger(duLieu.Split('|')[0]); int maNhaCC = xulyDuLieu.doiChuoiSangInteger(duLieu.Split('|')[1]);
            double soLuongNhap = xulyDuLieu.doiChuoiSangDouble(duLieu.Split('|')[2]); long donGiaNhap = xulyDuLieu.doiChuoiSangLong(duLieu.Split('|')[3]);
            string ghiChu = xulyDuLieu.xulyKyTuHTML(duLieu.Split('|')[4]);

            //--Gán giá trị cho các thuộc tính
            chiTiet.maNguyenLieu = maNguyenLieu;
            if (chiTiet.maNguyenLieu <= 0)
                loi += "Vui lòng chọn nguyên liệu cần nhập <br/>";
            chiTiet.maNhaCC = maNhaCC;
            if (chiTiet.maNhaCC <= 0)
                loi += "Vui lòng chọn nhà cung cấp cho nguyên liệu này <br/>";

            //------Gán các giá trị references nguyenLieu, nhaCungCap cho chi tiết
            chiTiet.nhaCungCap = db.nhaCungCaps.SingleOrDefault(s => s.maNhaCC == chiTiet.maNhaCC);
            chiTiet.nguyenLieu = db.nguyenLieux.SingleOrDefault(s => s.maNguyenLieu == chiTiet.maNguyenLieu);

            //----------Lưu số lượng nguyên liệu với đơn vị nhỏ nhất của nguyên liệu (1kg=>1000g)
            chiTiet.soLuongNhap = new bNguyenLieu().chuyenDoiDonViTuLonSangNho(soLuongNhap, chiTiet.nguyenLieu);
            if (chiTiet.soLuongNhap <= 0)
                loi += "Vui lòng nhập số lượng nguyên liệu nhập vào kho <br/>";
            chiTiet.donGiaNhap = donGiaNhap;
            if (chiTiet.donGiaNhap <= 0)
                loi += "Vui lòng nhập đơn giá nguyên liệu tại thời điểm nhập vào phiếu <br/>";
            chiTiet.ghiChu = ghiChu;
            if (loi.Length > 0)
                throw new Exception(loi);

        }
        /// <summary>
        /// Hàm thực hiện lấy dữ liệu từ giao  diện thêm vào các thuộc tính của phieuNhapKho
        /// </summary>
        /// <param name="phieu"></param>
        private void layDuLieuTrenView(phieuNhapKho phieu, FormCollection f)
        {
            string loi = "";
            phieu.maKho = xulyDuLieu.doiChuoiSangInteger(f["cbbKhoHang"]);
            if (phieu.maKho <= 0)
                loi += "Vui lòng chọn kho hàng nhận hàng <br/>";
            phieu.ngayNhap = DateTime.Now;
            phieu.tongTien = xulyDuLieu.doiChuoiSangInteger(f["txtTongTien"]);
            phieu.nguoiLapPhieu = ((taiKhoan)Session["login"]).tenDangNhap;
            phieu.ghiChu = xulyDuLieu.xulyKyTuHTML(f["txtGhiChu"]);
            if (loi.Length > 0)
                throw new Exception(loi);
        }
        /// <summary>
        /// Hàm đổ dữ liệu của phiếu nhập kho lên giao diện
        /// </summary>
        /// <param name="phieu"></param>
        /// <param name="db"></param>
        private void doDuLieuLenView(phieuNhapKho phieu, qlCaPheEntities db)
        {
            string htmlCbb = "";
            foreach (khoHang kh in db.khoHangs.ToList().OrderBy(s => s.tenKhoHang))
            {
                if (kh.maKhoHang == phieu.maKho)
                    htmlCbb += "<option selected ";// value=\""  + kh.maKhoHang.ToString() + "\">" + xulyDuLieu.traVeKyTuGoc(kh.tenKhoHang) + "</option>";
                else
                    htmlCbb += "<option ";
                htmlCbb += "value=\"" + kh.maKhoHang.ToString() + "\">" + "<b>" + xulyDuLieu.traVeKyTuGoc(kh.tenKhoHang) + "</b>" + " - " + xulyDuLieu.traVeKyTuGoc(kh.diaChi) + "</option>";
            }
            ViewBag.cbbKhoHang = htmlCbb;

            ViewBag.txtMaPhieu = phieu.maPhieu.ToString();
            ViewBag.txtNgayNhap = string.Format("{0:yyyy-MM-dd}", phieu.ngayNhap);
            ViewBag.txtTongTien = phieu.tongTien.ToString();
            ViewBag.txtNguoiLapPhieu = xulyDuLieu.traVeKyTuGoc(phieu.nguoiLapPhieu);
            ViewBag.txtGhiChu = xulyDuLieu.traVeKyTuGoc(phieu.ghiChu);
        }
        #endregion
    }
}