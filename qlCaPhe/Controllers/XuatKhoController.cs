using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using qlCaPhe.App_Start;
using qlCaPhe.Models;
using qlCaPhe.App_Start.Cart;
using qlCaPhe.Models.Business;

namespace qlCaPhe.Controllers
{
    public class XuatKhoController : Controller
    {
        private string idOfPage = "802";
        #region DATABASE, GENERAL
        #region CREATE
        /// <summary>
        /// Hàm thực hiện tạo giao diện xuất kho
        /// </summary>
        /// <returns></returns>
        public ActionResult xk_TaoMoiPhieuXuat()
        {
            if (xulyChung.duocTruyCap(idOfPage))
                try
                {
                    this.taoDuLieuChoCbbKhoHang(new qlCaPheEntities());
                    this.resetSession();
                }
                catch (Exception ex)
                {
                    xulyFile.ghiLoi("Class: XuatKhoController - Function: xk_TaoMoiPhieuXuat", ex.Message);
                }
            return View();
        }
        /// <summary>
        /// Hàm thêm mới phiếu xuất kho vào database
        /// </summary>
        /// <param name="f"></param>
        /// <param name="phieu"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult xk_TaoMoiPhieuXuat(FormCollection f, phieuXuatKho phieu)
        {
            if (xulyChung.duocCapNhat(idOfPage, "7"))
            {
                string ndThongBao = "";
                qlCaPheEntities db = new qlCaPheEntities();
                try
                {
                    this.layDuLieuTrenView(phieu, f);
                    db.phieuXuatKhoes.Add(phieu);
                    db.SaveChanges();
                    //----------Thêm chi tiết phiếu xuất kho
                    this.insertCtXuatKhoAndUpdateTonKho(phieu.maPhieu, db);
                    ndThongBao = createHTML.taoNoiDungThongBao("Phiếu xuất kho", phieu.maPhieu.ToString(), "xk_TableXuatKho");
                    this.taoDuLieuChoCbbKhoHang(db);
                    this.resetSession();
                }
                catch (Exception ex)
                {
                    ndThongBao = ex.Message;
                    xulyFile.ghiLoi("Class: XuatKhoController - Function: xk_TaoMoiPhieuXuat_Post", ex.Message);
                    this.doDuLieuLenView(phieu, db);
                }
                ViewBag.ThongBao = createHTML.taoThongBaoLuu(ndThongBao);
            }
            return View();
        }
        /// <summary>
        /// Hàm thêm dữ liệu vào bảng ctPhieuXuatKho và cập nhật lại số lượng tồn kho
        /// </summary>
        /// <param name="maPhieu"></param>
        /// <param name="db"></param>
        private void insertCtXuatKhoAndUpdateTonKho(int maPhieu, qlCaPheEntities db)
        {
            cartXuatKho cart = (cartXuatKho)Session["ctXuatKho"];
            if (cart.Item.Count <= 0)
                throw new Exception("Vui lòng nhập hàng vào phiếu <br/>");
            //----Lặp qua từng phần tử có trong session
            foreach (ctPhieuXuatKho ctTam in cart.getList())
            {
                ctPhieuXuatKho ctAdd = new ctPhieuXuatKho();
                ctAdd.maPhieu = maPhieu;
                ctAdd.maNguyenLieu = ctTam.maNguyenLieu;
                ctAdd.ghiChu = ctTam.ghiChu;
                ctAdd.donGiaXuat = ctTam.donGiaXuat;
                ctAdd.soLuongXuat = ctTam.soLuongXuat;
                //ctAdd.nguyenLieu = ctTam.nguyen0Lieu;
                db.ctPhieuXuatKhoes.Add(ctAdd);
                db.SaveChanges();
            }
        }

        #endregion
        #region READ
        /// <summary>
        /// Hàm tạo giao diện và gán danh sách phiếu xuất kho
        /// </summary>
        /// <returns></returns>
        public ActionResult xk_TableXuatKho()
        {
            if (xulyChung.duocTruyCap(idOfPage))
            {
                string htmlTable = "";
                try
                {
                    foreach (phieuXuatKho phieu in new qlCaPheEntities().phieuXuatKhoes.ToList().OrderBy(p => p.maPhieu).OrderByDescending(p=>p.ngayXuat))
                    {
                        htmlTable += "<tr role=\"row\" class=\"odd\">";
                        htmlTable += "      <td>";
                        htmlTable += "          <a href=\"#\" data-toggle=\"modal\" data-target=\"#modalChiTiet\" maPhieu=\"" + phieu.maPhieu.ToString() + "\" class=\"goiY\">";
                        htmlTable += phieu.maPhieu.ToString();
                        htmlTable += "              <span class=\"noiDungGoiY-right\">Click để xem chi tiết</span>";
                        htmlTable += "          </a>";
                        htmlTable += "      </td>";
                        htmlTable += "      <td>" + xulyDuLieu.traVeKyTuGoc(phieu.khoHang.tenKhoHang) + "</td>";
                        htmlTable += "      <td>" + phieu.ngayXuat.ToString() + "</td>";
                        htmlTable += "      <td>" + phieu.tongTien.ToString() + "</td>";
                        htmlTable += "      <td>" + xulyDuLieu.traVeKyTuGoc(phieu.taiKhoan.thanhVien.hoTV) + " " + xulyDuLieu.traVeKyTuGoc(phieu.taiKhoan.thanhVien.tenTV) + "</td>";
                        htmlTable += "      <td>";
                        htmlTable += "          <div class=\"btn-group\">";
                        htmlTable += "              <button type=\"button\" class=\"btn btn-primary dropdown-toggle\" data-toggle=\"dropdown\">";
                        htmlTable += "                  Chức năng <span class=\"caret\"></span>";
                        htmlTable += "              </button>";
                        htmlTable += "              <ul class=\"dropdown-menu\" role=\"menu\">";
                        htmlTable += createTableData.taoNutChinhSua("/XuatKho/xk_ChinhSuaPhieuXuat", phieu.maPhieu.ToString());
                        htmlTable += createTableData.taoNutXoaBo(phieu.maPhieu.ToString());
                        htmlTable += "              </ul>";
                        htmlTable += "          </div>";
                        htmlTable += "      </td>";
                        htmlTable += "</tr>";
                    }
                }
                catch (Exception ex)
                {
                    xulyFile.ghiLoi("Class: XuatKhoController - Function: xk_TableXuatKho", ex.Message);
                }
                ViewBag.TableData = htmlTable;
                ViewBag.ModalChiTiet = createHTML.taoModalChiTiet("modalChiTietPhieu", "idVungChiTiet", 3);
                ViewBag.ScriptAjaxXemChiTiet = createScriptAjax.scriptAjaxXemChiTietKhiClick("goiY", "maPhieu", "XuatKho/AjaxLayChiTietPhieu?maPhieu=", "idVungChiTiet", "modalChiTietPhieu");
                ViewBag.ScriptAjax = createScriptAjax.scriptAjaxXoaDoiTuong("XuatKho/AjaxXoaPhieuXuat?maPhieu=");
                ViewBag.ModalXoa = createHTML.taoCanhBaoXoa("Phiếu xuất kho");
            }
            return View();
        }
        /// <summary>
        /// Hàm tạo html bảng danh sách chi tiết phiếu và nhúng vào Modal khi người dùng click vào mã phiếu để xem
        /// </summary>
        /// <param name="maPhieu"></param>
        /// <returns>html modal chi tiết</returns>
        public string AjaxLayChiTietPhieu(int maPhieu)
        {
            string kq = "";
            if (xulyChung.duocTruyCap(idOfPage))
            {
                qlCaPheEntities db = new qlCaPheEntities();
                phieuXuatKho phieu = db.phieuXuatKhoes.SingleOrDefault(p => p.maPhieu == maPhieu);
                if (phieu != null)
                {
                    kq += "<div class=\"modal-header\">";
                    kq += "      <button type=\"button\" class=\"close\" data-dismiss=\"modal\">×</button>";
                    kq += "    <h3 class=\"modal-title\" id=\"largeModalLabel\">CHI TIẾT PHIẾU XUẤT " + phieu.maPhieu.ToString() + "</h3>";
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
                    kq += "                                 <th>Số lượng xuất</th>";
                    kq += "                                 <th>Đơn giá xuất</th>";
                    kq += "                                 <th>Ghi chú</th>";
                    kq += "                             </tr>";
                    kq += "                         </thead>";
                    kq += "                         <tbody>";
                    foreach (ctPhieuXuatKho ct in phieu.ctPhieuXuatKhoes.ToList())
                    {
                        kq += "                             <tr>";
                        kq += "                                 <td>";
                        kq += "                                     <img width=\"50px;\" height=\"50px;\" src=\"" + xulyDuLieu.chuyenByteHinhThanhSrcImage(ct.nguyenLieu.hinhAnh) + "\" />";
                        kq += "                                     <b>" + xulyDuLieu.traVeKyTuGoc(ct.nguyenLieu.tenNguyenLieu) + "</b>";
                        kq += "                                 </td>";
                        kq += "                                 <td>" + ct.soLuongXuat.ToString() + " " + ct.nguyenLieu.donViHienThi + "</td>";
                        kq += "                                 <td>" + ct.donGiaXuat.ToString() + "</td>";
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
                    kq += "             <label class=\"font-18 pull-left col-red bold\">Tổng tiền: " + phieu.tongTien.ToString() + " VNĐ</label><br />";
                    kq += "             <label class=\"pull-left col-blue-grey\"><i>* Ghi chú: </i>" + xulyDuLieu.traVeKyTuGoc(phieu.ghiChu) + "</label>";
                    kq += "         </div>";
                    kq += "     </div>";
                    kq += "     <div class=\"col-md-3\">";
                    kq += "         <button type=\"button\" class=\"btn btn-default waves-effect\" data-dismiss=\"modal\"><i class=\"material-icons\">exit_to_app</i>Đóng lại</button>";
                    kq += "     </div>";
                    kq += "</div>";
                }
            }
            return kq;
        }

        /// <summary>
        /// Hàm tìm kiếm nguyên liệu hiện tại có trong kho <para/> chỉ lấy những nguyên liệu có số lượng tồn >0
        /// </summary>
        /// <param name="tenNL">Tên nguyên liệu cần tìm</param>
        /// <returns>Danh sách item nguyên liệu hiện lên modal</returns>
        public string AjaxTimKiemNguyenLieuTonKho(string tenNL)
        {
            string kq = "";
            if (xulyChung.duocTruyCap(idOfPage))
            {
                kq = " <h4 class=\"media-heading  font-bold col-pink\">Không tìm thấy nguyên liệu hoặc nguyên liệu chưa được nhập</h4>";
                try
                {
                    qlCaPheEntities db = new qlCaPheEntities(); bTonKho bTonKho = new Models.Business.bTonKho();
                    List<nguyenLieu> listNguyenLieu = db.nguyenLieux.Where(n => n.tenNguyenLieu.StartsWith(tenNL)).ToList();
                    if (listNguyenLieu.Count() > 0)
                    {
                        kq = ""; //--------Reset lại dữ liệu trên modal
                        foreach (nguyenLieu nl in listNguyenLieu)
                        {
                            double soLuongTon = bTonKho.laySoLuongNguyenLieuTonThucTeTrongKho(nl.maNguyenLieu, db);
                            if (soLuongTon > 0)
                            {
                                kq += "<div class=\"media itemNguyenLieu\" manl=\"" + nl.maNguyenLieu.ToString() + "\" data-dismiss=\"modal\">";
                                kq += "     <div class=\"media-left\">";
                                kq += "          <img src=\"" + xulyDuLieu.chuyenByteHinhThanhSrcImage(nl.hinhAnh) + "\" width=\"64\" height=\"64\" />";
                                kq += "     </div>";
                                kq += "     <div class=\"media-body\">";
                                double soLuongChuyenDoi = (double)(soLuongTon / nl.tyLeChuyenDoi); //-------Tính số lượng tồn nguyên liệu với đơn vị lớn nhất (VD: kg, lit)
                                kq += "             <h4 class=\"media-heading\">" + nl.tenNguyenLieu + " <span class=\"col-red\">" + soLuongChuyenDoi.ToString() + " (" + nl.donViHienThi + ")</span></h4>";
                                kq += xulyDuLieu.traVeKyTuGoc(nl.moTa);
                                kq += "     </div>";
                                kq += "</div>";
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    kq += ex.Message;
                    xulyFile.ghiLoi("Class CongThucController - Function: AjaxTimKiemNguyenLieuTonKho", ex.Message);
                }
            }
            return kq;
        }
        #endregion
        #region DELETE
        /// <summary>
        /// hàm xóa 1 phiếu xuất kho hàng khỏi CSDL
        /// </summary>
        /// <param name="maPhieu"></param>
        public void AjaxXoaPhieuXuat(int maPhieu)
        {
            try
            {
                qlCaPheEntities db = new qlCaPheEntities();
                phieuXuatKho phieuXoa = db.phieuXuatKhoes.SingleOrDefault(p => p.maPhieu == maPhieu);
                if (phieuXoa != null)
                {
                    this.xoaChiTietTrongDatabase(phieuXoa.maKho, db);
                    //--Xóa tất cả dữ liệu trong chi tiết trước.
                    db.phieuXuatKhoes.Remove(phieuXoa);
                    db.SaveChanges();
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
            foreach (ctPhieuXuatKho ct in db.ctPhieuXuatKhoes.ToList().Where(c => c.maPhieu == maPhieu))
            {
                db.ctPhieuXuatKhoes.Remove(ct);
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
        /// Hàm thiết lập lại giá trị cho session
        /// </summary>
        private void resetSession()
        {
            //--Xoá session chứa danh sách sản phẩm nhập kho 
            Session.Remove("ctXuatKho");
            //Tạo mới lại session
            Session.Add("ctXuatKho", new cartXuatKho());
        }

        /// <summary>
        /// Hàm thực hiện lấy dữ liệu từ giao  diện thêm vào các thuộc tính của phieuXuatKho
        /// </summary>
        /// <param name="phieu"></param>
        private void layDuLieuTrenView(phieuXuatKho phieu, FormCollection f)
        {
            string loi = "";
            phieu.maKho = xulyDuLieu.doiChuoiSangInteger(f["cbbKhoHang"]);
            if (phieu.maKho <= 0)
                loi += "Vui lòng chọn kho hàng nhận hàng <br/>";
            phieu.ngayXuat = DateTime.Now;
            phieu.tongTien = xulyDuLieu.doiChuoiSangInteger(f["txtTongTien"]);
            phieu.nguoiLapPhieu = ((taiKhoan)Session["login"]).tenDangNhap;
            phieu.ghiChu = xulyDuLieu.xulyKyTuHTML(f["txtGhiChu"]);
            if (loi.Length > 0)
                throw new Exception(loi);
        }

        /// <summary>
        /// Hàm đổ dữ liệu của phiếu xuất kho lên giao diện
        /// </summary>
        /// <param name="phieu"></param>
        /// <param name="db"></param>
        private void doDuLieuLenView(phieuXuatKho phieu, qlCaPheEntities db)
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
            ViewBag.txtNgayNhap = string.Format("{0:yyyy-MM-dd}", phieu.ngayXuat);
            ViewBag.txtTongTien = phieu.tongTien.ToString();
            ViewBag.txtNguoiLapPhieu = xulyDuLieu.traVeKyTuGoc(phieu.nguoiLapPhieu);
            ViewBag.txtGhiChu = xulyDuLieu.traVeKyTuGoc(phieu.ghiChu);
        }
        #endregion
        #endregion

        #region CART
        #region CREATE
        /// <summary>
        /// Hàm thực hiện thêm nguyên liệu vào Session chứa chi tiết phiếu nhập kho
        /// </summary>
        /// <param name="duLieu">Chuỗi dữ liệu nhận từ giao diện</param>
        /// <returns></returns>
        public string AjaxThemChiTietVaTraVeBang(string duLieu)
        {
            string kq = ""; double tongTienNhap = 0;
            if (xulyChung.duocCapNhat(idOfPage, "7"))
            {
                cartXuatKho cart = (cartXuatKho)Session["ctXuatKho"];
                try
                {
                    ctPhieuXuatKho chiTietAdd = new ctPhieuXuatKho();
                    this.layDuLieuTuViewChiTiet(chiTietAdd, duLieu);
                    //------Thêm chi tiết vào session
                    cart.addCart(chiTietAdd);
                    //Cập nhật lại session
                    Session["ctXuatKho"] = cart;
                    tongTienNhap = cart.getTotalPrice();
                }
                catch (Exception ex)
                {
                    xulyFile.ghiLoi("Class XuatKhoController - Function: themChiTietVaTraVeBang", ex.Message);
                }
                kq += taoBangChiTietTuSession(); //------Tạo lại bảng danh sách các bước có trong session và hiện lên giao diện
            }
            return kq + "|" + tongTienNhap.ToString(); //-------Trả về chuỗi html tạo nên bảng và chuỗi nội dung thông báo. Sau đó cắt ra và hiện lên giao diện
        }
        #endregion
        #region READ

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
                cartXuatKho cart = (cartXuatKho)Session["ctXuatKho"]; bNguyenLieu bNguyenLieu = new Models.Business.bNguyenLieu();
                kq += "<table class=\"table table-hover\">";
                kq += "     <thead>";
                kq += "         <tr>";
                //----Tạo tiêu đề cho bảng chi tiết
                kq += "             <th>Tên nguyên liệu</th><th>Số lượng</th><th>Đơn giá nhập</th><th>Chức năng</th>";
                kq += "         </tr>";
                kq += "     </thead>";
                kq += "     <tbody>";
                foreach (ctPhieuXuatKho ct in cart.Item.Values) //--------Lặp qua từng phần tử có trong Session 
                {
                    kq += "         <tr>";
                    kq += "             <td>";
                    kq += "                 <img width=\"50px\" height=\"auto;\" src=\"" + xulyDuLieu.chuyenByteHinhThanhSrcImage(ct.nguyenLieu.hinhAnh) + "\">";
                    kq += xulyDuLieu.traVeKyTuGoc(ct.nguyenLieu.tenNguyenLieu);
                    kq += "             </td>";
                    kq += "             <td>" + bNguyenLieu.chuyenDoiDonViNhoSangLon(ct.soLuongXuat, ct.nguyenLieu).ToString() + " " + xulyDuLieu.traVeKyTuGoc(ct.nguyenLieu.donViHienThi) + "</td>";
                    kq += "             <td>" + ct.donGiaXuat.ToString() + "</td>";
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
                kq += "\">Tổng số tiền xuất nguyên liệu: " + cart.getTotalPrice().ToString() + " VNĐ</label><br />";
            }
            catch (Exception ex)
            {
                xulyFile.ghiLoi("Class XuatKhoController - Function: taoBangChiTietTuSession", ex.Message);
            }
            return kq;
        }
        #endregion
        #region DELETE
        /// <summary>
        /// Hàm thực hiện xóa tất cả dữ liệu trong SEssion và tạo lại session
        /// </summary>
        /// <returns></returns>
        public void AjaxXoaTatCaChiTiet()
        {
            if (xulyChung.duocCapNhat(idOfPage, "7"))
            {
                Session.Remove("ctXuatKho"); Session.Add("ctXuatKho", new cartXuatKho());
            }
        }
        /// <summary>
        /// Hàm thực thi xóa 1 chi tiết trong session
        /// </summary>
        /// <param name="maCT">Mã xác định </param>
        /// <returns>Html tạo bảng đã được cập nhật dữ liệu</returns>
        public string AjaxXoaMotChitiet(int maCT)
        {
            if (xulyChung.duocCapNhat(idOfPage, "7"))
            {
                try
                {
                    cartXuatKho cart = (cartXuatKho)Session["ctXuatKho"];
                    cart.removeItem(maCT);
                    //----Cập nhật lại session
                    Session["ctXuatKho"] = cart;
                }
                catch (Exception ex)
                {
                    xulyFile.ghiLoi("Class XuatKhoController - Function: AjaxXoaMotChitiet", ex.Message);
                }
                return this.taoBangChiTietTuSession();
            }
            return "";
        }
        #endregion
        #region ORTHERS
        /// <summary>
        /// Hàm thêm dữ liệu từ giao diện cho object ctPhieuXuatKho
        /// </summary>
        /// <param name="chiTiet"></param>
        /// <param name="duLieu">Chuỗi dữ liệu được lấy khi ajax gửi về. Dữ liệu là các giá trị trong textbox nhập vào chi tiết
        /// có dạng: maNguyenLieu|maNhaCungCap|soLuongNhap|donGiaNhap|ghiChu</param>
        private void layDuLieuTuViewChiTiet(ctPhieuXuatKho chiTiet, string duLieu)
        {
            string loi = ""; qlCaPheEntities db = new qlCaPheEntities();
            //------Thực hiện xử lý cắt chuỗi duLieu để lấy giá trị của các thuộc tính
            int maNguyenLieu = xulyDuLieu.doiChuoiSangInteger(duLieu.Split('|')[0]); double soLuong = xulyDuLieu.doiChuoiSangDouble(duLieu.Split('|')[1]);
            long donGia = xulyDuLieu.doiChuoiSangLong(duLieu.Split('|')[2]); string ghiChu = xulyDuLieu.xulyKyTuHTML(duLieu.Split('|')[3]);

            //--Gán giá trị cho các thuộc tính
            chiTiet.maNguyenLieu = maNguyenLieu;
            if (chiTiet.maNguyenLieu <= 0)
                loi += "Vui lòng chọn nguyên liệu cần nhập <br/>";
            //------Gán các giá trị references nguyenLieu, nhaCungCap cho chi tiết
            chiTiet.nguyenLieu = db.nguyenLieux.SingleOrDefault(s => s.maNguyenLieu == chiTiet.maNguyenLieu);
            //---------Chuyển đổi số lượng ra đơn vị pha chế: VD: 1 kg = 1000g (soLuongNhapVao * tyLeChuyenDoi)
            double soLuongXuatChuyenDoi = new bNguyenLieu().chuyenDoiDonViTuLonSangNho(soLuong, chiTiet.nguyenLieu);
            double soLuongTon = new bTonKho().laySoLuongNguyenLieuTonThucTeTrongKho(chiTiet.maNguyenLieu, db);
            chiTiet.soLuongXuat = soLuongXuatChuyenDoi;
            if (chiTiet.soLuongXuat <= 0 || soLuongXuatChuyenDoi > soLuongTon)
                loi += "Số lượng nguyên liệu xuất kho không hợp lệ hoặc số lượng xuất lớn hơn số lượng tồn kho <br/>";

            chiTiet.donGiaXuat = donGia;
            if (chiTiet.donGiaXuat <= 0)
                loi += "Vui lòng nhập đơn giá của nguyên liệu tại thời điểm nhập vào phiếu <br/>";

            chiTiet.ghiChu = ghiChu;
            if (loi.Length > 0)
                throw new Exception(loi);
        }
        #endregion
        #endregion
    }
}