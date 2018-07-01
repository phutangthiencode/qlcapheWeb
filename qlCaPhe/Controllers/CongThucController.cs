using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using qlCaPhe.App_Start;
using qlCaPhe.Models;
using qlCaPhe.App_Start.Session;
using qlCaPhe.Models.Business;

namespace qlCaPhe.Controllers
{
    public class CongThucController : Controller
    {
        private static int maChiTietTam = 0;//Mã chi tiết tạm để xác định bước trong Session
        #region CREATE
        /// <summary>
        /// Hàm tạo giao diện tạo mới công thức pha chế
        /// </summary>
        /// <returns></returns>
        public ActionResult ct_TaoMoiCongThuc()
        {
            this.resetData();
            try
            {
                int maSanPham = this.xuLyRequestLayMaSanPham();
                sanPham sp = new qlCaPheEntities().sanPhams.SingleOrDefault(s => s.maSanPham == maSanPham);
                if (sp != null)
                {
                    //------Gán hình ảnh của sản phẩm lên giao diện
                    ViewBag.hinhSP = xulyDuLieu.chuyenByteHinhThanhSrcImage(sp.hinhAnh);
                    xulyChung.ghiNhatKyDtb(1, "Thêm mới công thức cho \" " + xulyDuLieu.traVeKyTuGoc(sp.tenSanPham) + " \"");
                }
            }
            catch (Exception ex)
            {
                xulyFile.ghiLoi("Class CongThucController - Function: ct_TaoMoiCongThuc_Get", ex.Message);
                return RedirectToAction("PageNotFound", "Home");
            }
            return View();
        }
        /// <summary>
        /// Hàm thâm mới công thức vào CSDL
        /// </summary>
        /// <param name="ct"></param>
        /// <param name="f"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ct_TaoMoiCongThuc(congThucPhaChe ct, FormCollection f)
        {
            string ndThongBao = ""; int kqLuu = 0;
            qlCaPheEntities db = new qlCaPheEntities();
            try
            {
                int maSP = this.xuLyRequestLayMaSanPham();
                this.layDuLieuTuViewCongThuc(ct, f);
                ct.sanPham = db.sanPhams.SingleOrDefault(s => s.maSanPham == maSP);
                //---Insert table lichSuGia
                this.themLichSuGiaVaoDatabase(db, f);
                //---Insert table congThucPhaChe
                db.congThucPhaChes.Add(ct);
                kqLuu = db.SaveChanges();
                if (kqLuu > 0)
                {
                    //---Insert table ctCongThuc
                    this.themChiTietVaoDatabase(ct.maCongThuc, db);
                    ndThongBao = createHTML.taoNoiDungThongBao("Công thức pha chế", xulyDuLieu.traVeKyTuGoc(ct.tenCongThuc), "/CongThuc/ct_TableCongThuc");
                    xulyChung.ghiNhatKyDtb(2, "Công thức pha chế của \" " + xulyDuLieu.traVeKyTuGoc(ct.sanPham.tenSanPham) + " \"");
                }
            }
            catch (Exception ex)
            {
                xulyFile.ghiLoi("Class CongThucController - Function: ct_TaoMoiCongThuc_Post", ex.Message);
                ndThongBao = ex.Message;
                this.doDuLieuCongThucLenView(ct, db);
            }
            this.resetData();
            ViewBag.ThongBao = createHTML.taoThongBaoLuu(ndThongBao);
            return View();
        }
        /// <summary>
        /// Hàm thêm giá của sản phẩm từ việc sử dụng nguyên liệu trong công thức vào bảng lichSuGia và cập nhật đơn giá sản phẩm
        /// </summary>
        /// <param name="db"></param>
        /// <param name="f"></param>
        private void themLichSuGiaVaoDatabase(qlCaPheEntities db, FormCollection f)
        {
            try
            {
                lichSuGia lichSu = new lichSuGia();
                lichSu.maSanPham = this.xuLyRequestLayMaSanPham();
                if (lichSu.maSanPham <= 0)
                    throw new Exception("Vui lòng chọn sản phẩm");
                //--------Lấy dữ liệu từ view
                lichSu.donGiaGoc = xulyDuLieu.doiChuoiSangLong(f["txtTongTienNguyenLieu"]);
                if (lichSu.donGiaGoc <= 0)
                    throw new Exception("Vui lòng chọn nguyên liệu");
                lichSu.donGia = xulyDuLieu.doiChuoiSangLong(f["txtDonGiaMongMuon"]);
                if (lichSu.donGia <= 0)
                    throw new Exception("Vui lòng nhập số tiền cho sản phẩm");
                if (lichSu.donGia < lichSu.donGiaGoc)
                    throw new Exception("Giá sản phẩm nhỏ hơn giá gốc");
                lichSu.ghiChu = "Thêm dựa vào công thức pha chế";
                lichSu.nguoiTao = xulyChung.layTenDangNhap();
                lichSu.ngayCapNhat = DateTime.Now;
                //-----Thêm vào bảng lichSuGia
                db.lichSuGias.Add(lichSu);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                xulyFile.ghiLoi("Class CongThucController - Function: themLichSuGiaVaoDatabase", ex.Message);
            }
        }
        /// <summary>
        /// hàm thực hiện thêm chi tiết công thức vào database
        /// </summary>
        /// <param name="maCongThuc"></param>
        private void themChiTietVaoDatabase(int maCongThuc, qlCaPheEntities db)
        {
            try
            {
                //------lấy danh sách các chi tiết trong session
                cartCongThuc cart = (cartCongThuc)Session["congThuc"];
                //------Lặp qua từng phần tử có trong list và thêm vào database
                foreach (ctCongThuc ct in cart.getList())
                {
                    ctCongThuc ctThem = new ctCongThuc();
                    //----Vì mã ct và ctThem khác nhau nên cần gán lại giá trị
                    ctThem.buocSo = ct.buocSo;
                    ctThem.ghiChu = ct.ghiChu;
                    ctThem.hanhDong = ct.hanhDong;
                    ctThem.maCongThuc = maCongThuc;
                    if (ct.maNguyenLieu != 0) //---Kiểm tra, nếu có nguyên liệu thì thêm các thuộc tính của nguyên liệu
                    {
                        ctThem.maNguyenLieu = ct.maNguyenLieu;
                        ctThem.soLuongNguyenLieu = ct.soLuongNguyenLieu;
                        ctThem.donViSuDung = ct.donViSuDung;
                    }
                    //----Thêm vào database
                    db.ctCongThucs.Add(ctThem);
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                xulyFile.ghiLoi("Class CongThucController - Function: themChiTietVaoDatabase", ex.Message);
            }

        }
        /// <summary>
        /// Hàm thực hiện ajax thêm các bước vào bảng và thực hiện lấy danh sách các bước có trong cart đổ lên giao diện
        /// </summary>
        /// <returns>Trả vê chuỗi htmlBangDanhSachCacBuoc|bảng danh sách nguyên liệu|tongGiaNguyenLieu</returns>
        public string themChiTietVaTraVeBang(string duLieu)
        {
            cartCongThuc cart = (cartCongThuc)Session["congThuc"];
            try
            {
                ctCongThuc chiTietAdd = new ctCongThuc();
                this.layDuLieuTuViewChiTiet(chiTietAdd, duLieu);
                //------Thêm chi tiết vào session
                cart.addCart(chiTietAdd);
                //-----Cập nhật lại session
                Session["congThuc"] = cart;
            }
            catch (Exception ex)
            {
                xulyFile.ghiLoi("Class CongThucController - Function: themChiTietVaTraVeBang", ex.Message);
            }
            //Trả vê chuỗi htmlBangDanhSachCacBuoc|bảng danh sách nguyên liệu|tongGiaNguyenLieu
            return taoBangChiTietVaNguyenLieuSuDungTuSession();
        }
        #endregion
        #region READ
        /// <summary>
        /// Hàm tạo giao diện danh sách công thức của 1 sản phẩm
        /// </summary>
        /// <returns></returns>
        public ActionResult ct_TableCongThuc(int? page)
        {
            string htmlTable = "";
            try
            {
                int maSanPham = this.xuLyRequestLayMaSanPham();
                int trangHienHanh = (page ?? 1);
                qlCaPheEntities db = new qlCaPheEntities();
                sanPham sp = db.sanPhams.SingleOrDefault(s => s.maSanPham == maSanPham);
                if (sp != null)
                {
                    int soPhanTu = sp.congThucPhaChes.Count();
                    ViewBag.PhanTrang = createHTML.taoPhanTrang(soPhanTu, createHTML.pageSize, trangHienHanh, "/CongThuc/ct_TableCongThuc"); //------cấu hình phân trang

                    htmlTable += this.taoBangDanhSachCongThuc(sp, trangHienHanh);
                    ViewBag.TitleTenSanPham = xulyDuLieu.traVeKyTuGoc(sp.tenSanPham);
                    this.thietLapThongSoChung();
                    ViewBag.HrefTaoCongThuc = xulyChung.taoUrlCoTruyenThamSo("CongThuc/ct_TaoMoiCongThuc", maSanPham.ToString());
                    xulyChung.ghiNhatKyDtb(1, "Danh mục công thức pha chể của \" " + xulyDuLieu.traVeKyTuGoc(sp.tenSanPham) + " \"");
                }
            }
            catch (Exception ex)
            {
                xulyFile.ghiLoi("Class CongThucController - Function: ct_TableCongThuc", ex.Message);
                return RedirectToAction("ServerError", "Home");
            }
            ViewBag.TableData = htmlTable;
            return View();
        }
        /// <summary>
        /// Hàm tạo danh sách công thức pha chế của 1 sản phẩm 
        /// </summary>
        /// <param name="sp">Công thức cho sản phẩm</param>
        /// <returns></returns>
        private string taoBangDanhSachCongThuc(sanPham sp, int trangHienHanh)
        {
            string htmlTable = "";
            //--------Duyệt qua danh sách công thức của sản phẩm. Được order theo trạng thái đang sử dụng lên trước
            foreach (congThucPhaChe ct in sp.congThucPhaChes.ToList().OrderByDescending(t => t.trangThai).Skip((trangHienHanh - 1) * createHTML.pageSize).Take(createHTML.pageSize))
            {
                htmlTable += "<tr role=\"row\" class=\"odd\">";
                htmlTable += "      <td>";
                htmlTable += "          <a class=\"goiY\" maCongThuc=\"" + ct.maCongThuc.ToString() + "\"  style=\"cursor:pointer\">";
                htmlTable += "              <b>" + xulyDuLieu.traVeKyTuGoc(ct.tenCongThuc) + "</b>";
                htmlTable += "              <span class=\"noiDungGoiY-right\">Click để xem chi tiết công thức</span>";
                htmlTable += "          </a>";
                htmlTable += "      </td>";
                htmlTable += "      <td>" + xulyDuLieu.traVeKyTuGoc(ct.dienGiai) + "</td>";
                htmlTable += "      <td>" + (ct.trangThai == true ? "Đang sử dụng" : "Đã hủy") + "</td>";
                htmlTable += "      <td>" + xulyDuLieu.traVeKyTuGoc(ct.nguoiTao) + "</td>";
                htmlTable += "      <td>" + xulyDuLieu.traVeKyTuGoc(ct.nguoiDuyet) + "</td>";
                htmlTable += "      <td>" + ct.ngayTao.ToString() + "</td>";
                htmlTable += "      <td>";
                htmlTable += "          <div class=\"btn-group\">";
                htmlTable += "              <button type=\"button\" class=\"btn btn-primary dropdown-toggle\" data-toggle=\"dropdown\" aria-expanded=\"true\">";
                htmlTable += "                  Chức năng <span class=\"caret\"></span>";
                htmlTable += "              </button>";
                htmlTable += "              <ul class=\"dropdown-menu\" role=\"menu\">";
                htmlTable += createTableData.taoNutChinhSua("/CongThuc/ct_ChinhSuaCongThuc", ct.maCongThuc.ToString());
                htmlTable += createTableData.taoNutCapNhat("/CongThuc/capNhatTrangThai", ct.maCongThuc.ToString(), "col-orange", "spellcheck", "Sử dụng");
                htmlTable += createTableData.taoNutXoaBo(ct.maCongThuc.ToString());
                htmlTable += "              </ul>";
                htmlTable += "          </div>";
                htmlTable += "      </td>";
                htmlTable += "</tr>";
            }
            return htmlTable;
        }

        /// <summary>
        /// Hàm thực hiện tạo modal xem chi tiết công thức
        /// </summary>
        /// <param name="maCongThuc"></param>
        /// <returns>Chuỗi html tạo modal chi tiết công thức</returns>
        public string AjaxXemChiTietCongThuc(int maCongThuc)
        {
            string kq = "";
            try
            {
                congThucPhaChe congThuc = new qlCaPheEntities().congThucPhaChes.SingleOrDefault(c => c.maCongThuc == maCongThuc);
                if (congThuc != null)
                {
                    kq += htmlModalChiTietCongThuc(congThuc);
                    xulyChung.ghiNhatKyDtb(5, "Công thức pha chế của \"" + xulyDuLieu.traVeKyTuGoc(congThuc.sanPham.tenSanPham) + " \"");
                }
            }
            catch (Exception ex)
            {
                xulyFile.ghiLoi("Class CongThucController - Function: AjaxXemChiTietCongThuc", ex.Message);
            }
            return kq;
        }

        #region HTML MODAL CHI TIẾT CÔNG THỨC

        /// <summary>
        /// Hàm tạo chuỗi html cho div modal nội dung các bước của công thức
        /// </summary>
        /// <param name="congThuc"></param>
        /// <returns></returns>
        private string htmlModalChiTietCongThuc(congThucPhaChe congThuc)
        {
            string kq = "";
            //-------Tạo tiêu đế cho modal và gán tên công thức pha chế
            kq += this.htmlHeaderModalChiTietCongthuc(xulyDuLieu.traVeKyTuGoc(congThuc.tenCongThuc));
            //-------Tạo body cho modal
            kq += this.htmlBodyModalChiTietCongThuc(congThuc);
            //-------Tạo footer cho modal
            kq += this.htmlFooterModalChiTietCongThuc(xulyDuLieu.traVeKyTuGoc(congThuc.ghiChu));
            return kq;
        }
        /// <summary>
        /// Hàm tạo html cho header của modal xem chi tiết công thức
        /// </summary>
        /// <param name="tenCongThuc">Tên công thức để gán vào tiêu đề</param>
        /// <returns></returns>
        private string htmlHeaderModalChiTietCongthuc(string tenCongThuc)
        {
            string kq = "";
            kq += "<div class=\"modal-header\">";
            kq += "      <button type=\"button\" class=\"close\" data-dismiss=\"modal\">×</button>";
            kq += "    <h3 class=\"modal-title\" id=\"largeModalLabel\">CHI TIẾT CÔNG THỨC &quot;" + tenCongThuc + "&quot;</h3>";
            kq += "</div>";
            return kq;
        }
        /// <summary>
        /// Hàm tạo html body của modal xem chi tiết công thức.
        /// </summary>
        /// <param name="congThuc"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        private string htmlBodyModalChiTietCongThuc(congThucPhaChe congThuc)
        {
            string kq = "";
            kq += "<div class=\"modal-body\">";
            kq += "    <div class=\"row\">";
            kq += "        <div class=\"col-md-12 col-lg-12\">";
            kq += "            <div class=\"card\">";
            kq += "                <div class=\"header bg-cyan\">";
            kq += "                    <h2>Danh mục các bước thực hiện</h2>";
            kq += "                </div>";
            kq += "                <div class=\"body\">";
            //---------Gán nội dung danh sách các bước và nội dung của từng bước
            kq += this.htmlTaoNoiDungTrongBodyChiTiet(congThuc);
            kq += "                </div>";//End div body 
            kq += "            </div>";
            kq += "        </div>";
            kq += "    </div>";
            kq += "</div>";
            return kq;
        }
        /// <summary>
        /// Hàm tạo footer cho modal xem chi tiết công thức
        /// </summary>
        /// <param name="ghiChuCongThuc"></param>
        /// <returns></returns>
        private string htmlFooterModalChiTietCongThuc(string ghiChuCongThuc)
        {
            string kq = "";
            kq += "<div class=\"modal-footer\">";
            kq += "     <div class=\"col-md-9\">";
            kq += "         <div class=\"pull-left\">";
            kq += "             <label class=\"pull-left col-blue-grey\"><i>* Ghi chú: </i>" + ghiChuCongThuc + "</label>";
            kq += "         </div>";
            kq += "     </div>";
            kq += "     <div class=\"col-md-3\">";
            kq += "           <button type=\"button\" class=\"btn btn-default waves-effect\" data-dismiss=\"modal\"><i class=\"material-icons\">exit_to_app</i>Đóng lại</button>";
            kq += "      </div>";
            kq += "</div>";
            return kq;
        }

        /// <summary>
        /// Hàm tạo html cho class body của modal nội dung công thức
        /// </summary>
        /// <param name="db"></param>
        /// <param name="congThuc"></param>
        /// <returns></returns>
        private string htmlTaoNoiDungTrongBodyChiTiet(congThucPhaChe congThuc)
        {
            string kq = "";
            kq += "                     <div class=\"wizard\">";
            kq += "                         <div class=\"wizard-inner\">";
            kq += "                             <div class=\"connecting-line\"></div>";
            kq += "                                 <ul class=\"nav nav-tabs\" role=\"tablist\">";
            //-----------Lặp qua chi tiết để hiện các bước chọn
            List<ctCongThuc> listChiTiet = congThuc.ctCongThucs.Where(c => c.maCongThuc == congThuc.maCongThuc).OrderBy(c => c.buocSo).ToList();
            int index = 0; //----Biến xác định. Nếu là phần tử đầu tiên thì add class active để chọn bước này
            foreach (ctCongThuc ct in listChiTiet)
            {
                //---------Gán danh sách các bước để người dùng chọn
                kq += this.htmlStepListModalChitietCongThuc(ct, index);
                index++;
            }
            kq += "                                   </ul></div>";     //-----Đóng ul và div cho khu vực danh sách các bước
            kq += "                           <div class=\"tab-content\">"; //------Khu vực nội dung bước
            int indexNoiDung = 0;
            foreach (ctCongThuc ct in listChiTiet)//----Lặp qua danh sách chi tiết để gán nội dung các bước thực hiện
            {
                kq += this.htmlNoiDungChoTungBuoc(ct, indexNoiDung);
                indexNoiDung++;
            }
            kq += "                          </div>";
            kq += "                    </div>";
            return kq;
        }

        /// <summary>
        /// Hàm tạo script cho mỗi bước của công thức. Khi người dùng click vào mỗi item dưới đây sẽ hiện chi tiết của công thức đó
        /// </summary>
        /// <param name="ct">Mỗi bước đang lặp qua</param>
        /// <param name="index">Thứ tự để gán class Active để kích hoạt chọn bước đầu tiên</param>
        /// <returns></returns>
        private string htmlStepListModalChitietCongThuc(ctCongThuc ct, int index)
        {
            string kq = "";
            kq += "<li role=\"presentation\"";
            if (index == 0) //--Active tabb bước 1
                kq += "                              class=\"active\">";
            else
                kq += "                             >"; //-----Đóng tag li
            kq += "                                      <a href=\"#step" + ct.buocSo.ToString() + "\" data-toggle=\"tab\" aria-controls=\"step1\" role=\"tab\" title=\"Bước " + ct.buocSo.ToString() + "\">";
            kq += "                                         <span class=\"round-tab\">" + ct.buocSo.ToString() + "</span></a></li>";
            return kq;
        }
        /// <summary>
        /// Hàm tạo nội dung chi tiết của từng bước
        /// </summary>
        /// <param name="ct"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        private string htmlNoiDungChoTungBuoc(ctCongThuc ct, int index)
        {
            string kq = "";
            if (index == 0) kq += "      <div class=\"tab-pane active\" role=\"tabpanel\" id=\"step" + ct.buocSo.ToString() + "\">"; //---Kiểm tra nếu là phần tử đầu tiên thì thêm class active
            else kq += "                        <div class=\"tab-pane\" role=\"tabpanel\" id=\"step" + ct.buocSo.ToString() + "\">";
            kq += "                                 <h3>Bước " + ct.buocSo.ToString() + "</h3>";
            kq += "                                 <div class=\"col-lg-6 col-md-6 col-xs-6\"><b>HÀNH ĐỘNG</b><p class=\"lead col-pink\">" + xulyDuLieu.traVeKyTuGoc(ct.hanhDong) + "</p></div>";
            if (ct.maNguyenLieu > 0)
            {//---Có nguyên liệu thì lấy thông tin nguyên liệu
                kq += "                               <div class=\"col-lg-6 col-md-6 col-xs-6\">";
                kq += "                                   <b>NGUYÊN LIỆU SỬ DUNG</b><img src=\"" + xulyDuLieu.chuyenByteHinhThanhSrcImage(ct.nguyenLieu.hinhAnh) + "\">";
                kq += "                                   <p class=\"font-bold col-orange\">Số lượng sử dụng: " + ct.soLuongNguyenLieu.ToString() + " " + xulyDuLieu.traVeKyTuGoc(ct.nguyenLieu.donViPhaChe) + " = " + xulyDuLieu.traVeKyTuGoc(ct.donViSuDung) + "</p>";
                kq += "                               </div>";
            }
            kq += "                                       <div class=\"col-lg-9 col-md-9 col-xs-9 \"><label class=\"pull-left col-teal\"><i>* Ghi chú cho bước: </i>" + xulyDuLieu.traVeKyTuGoc(ct.ghiChu) + "</label></div>";
            kq += "                             </div>";
            return kq;
        }
        #endregion
        #region BẢNG CHI TIẾT
        /// <summary>
        /// Hàm thực hiện lấy thông tin nguyên liệu khi đã click chọn trên modal
        /// </summary>
        /// <param name="maNL"></param>
        /// <returns></returns>
        public string layNguyenLieuModal(int maNL)
        {
            string kq = ""; bNhapKho bNhap = new bNhapKho();
            try
            {
                //---------Lấy thông tin nguyên liệu đã được nhập
                List<ctPhieuNhapKho> listNhap = new qlCaPheEntities().ctPhieuNhapKhoes.Where(ct => ct.maNguyenLieu == maNL).ToList();
                if (listNhap.Count > 0)
                {
                    foreach (ctPhieuNhapKho ctPhieu in listNhap)
                    {
                        kq += "<img id=\"hinhNguyenLieu\" class='img img-responsive img-thumbnail'";
                        kq += "src=\"" + xulyDuLieu.chuyenByteHinhThanhSrcImage(ctPhieu.nguyenLieu.hinhAnh) + "\" width=\"250px\" height=\"auto\" />";
                        kq += "<br />";
                        kq += "<label id=\"lbTenNguyenLieu\" class=\"font-15 font-italic font-bold col-orange\">" + xulyDuLieu.traVeKyTuGoc(ctPhieu.nguyenLieu.tenNguyenLieu) + " </label> ";
                        kq += "<input id=\"maNguyenLieuDaChon\" type=\"hidden\" value=\"" + ctPhieu.maNguyenLieu.ToString() + "\" />";

                        //-------Lấy giá nguyên liệu mới nhất trong phiếu(Thời điểm đang bán giá nguyên liệu giao động từ.....)
                        kq += "<label id=\"lbDonGiaNhap\" class=\"font-15 font-italic font-bold col-red\">Giá nhập bình quân: " + bNhap.tinhTienBinhQuanNguyenLieuNhap(ctPhieu.maNguyenLieu).ToString() + " - Đơn vị pha chế: " + ctPhieu.nguyenLieu.donViPhaChe + "</label>";
                        break;
                    }
                }
                else
                    //------Hiện thông báo nguyên liệu chưa có trong kho
                    kq += "<label>Nguyên liệu chưa nhập nên không thể xác định giá cả</label>";
            }
            catch (Exception ex)
            {
                xulyFile.ghiLoi("Class CongThucController - Function: layNguyenLieuModal", ex.Message);
            }
            return kq;
        }
        /// <summary>
        /// Hàm thiết kế bảng DANH SÁCH CHI TIẾT, NGUYÊN LIỆU SỬ DỤNG, TỔNG TIỀN NGUYÊN LIỆU được lấy từ Session 
        /// </summary>
        /// <returns>htmlBangChiTiet|htmlBangNguyenLieuSuDung</returns>
        public string taoBangChiTietVaNguyenLieuSuDungTuSession()
        {
            cartCongThuc cart = (cartCongThuc)Session["congThuc"];
            string kq = "";
            kq += "<table class=\"table table-hover\">";
            kq += "     <thead>";
            kq += "         <tr>";//------Tạo tiêu đề bảng
            kq += "             <th>Bước</th><th>Hành động</th><th>Tên nguyên liệu</th><th>Số lượng</th><th>Đơn vị</th><td>Chức năng</td>";
            kq += "         </tr>";
            kq += "     </thead>";
            kq += "     <tbody>";
            try
            {
                foreach (ctCongThuc ct in cart.getList().OrderBy(c => c.buocSo))
                {
                    kq += "     <tr>";
                    kq += "         <th scope=\"row\">" + ct.buocSo.ToString() + "</th>";
                    kq += "         <td>" + xulyDuLieu.traVeKyTuGoc(ct.hanhDong) + "</td>";
                    //---------Lấy tên nguyên liệu đã chọn
                    nguyenLieu nl = ct.nguyenLieu;
                    if (nl != null) kq += "     <td>" + xulyDuLieu.traVeKyTuGoc(nl.tenNguyenLieu) + "</td>";
                    else kq += "     <td></td>";
                    kq += "         <td>" + ct.soLuongNguyenLieu + "</td>";
                    kq += "         <td>" + xulyDuLieu.traVeKyTuGoc(ct.donViSuDung) + "</td>";
                    kq += "         <td>";
                    kq += "             <div class=\"btn-group open\">";
                    kq += "                 <button type=\"button\" class=\"btn btn-primary dropdown-toggle\" data-toggle=\"dropdown\" aria-expanded=\"true\">Chức năng <span class=\"caret\"></span></button>";
                    kq += "                 <ul class=\"dropdown-menu\" role=\"menu\">";
                    kq += "                     <li><a class=\"col-blue waves-effect waves-block suaBuoc\" maCt=\"" + ct.maChiTiet.ToString() + "\">";
                    kq += "                             <i class=\"material-icons\">mode_edit</i>Sửa bước";
                    kq += "                         </a></li>";
                    kq += "                     <li><a class=\"col-red waves-effect waves-block xoaBuoc\" maCt=\"" + ct.maChiTiet.ToString() + "\">";
                    kq += "                             <i class=\"material-icons\">delete</i>Xoá bước";
                    kq += "                         </a></li>";
                    kq += "                 </ul>";
                    kq += "             </div>";
                    kq += "         </td>";
                    kq += "     </tr>";
                }
            }
            catch (Exception ex)
            {
                xulyFile.ghiLoi("Class CongThucController - Function: taoBangChiTietTuSession", ex.Message);
            }
            kq += "     </tbody>";
            kq += "</table>";
            //Trả vê chuỗi htmlBangDanhSachCacBuoc|bảng danh sách nguyên liệu|tongGiaNguyenLieu
            return kq + "|" + this.taoBangNguyenLieuSuDung() + "|" + xulyDuLieu.doiVND(cart.getTotalPriceNguyenLieu()) ;
        }
        /// <summary>
        /// hàm tạo bảng danh sách nguyên liệu đã dùng và gán vào tag có id...
        /// </summary>
        /// <returns>Các dòng trong bảng</returns>
        public string taoBangNguyenLieuSuDung()
        {
            cartCongThuc cart = (cartCongThuc)Session["congThuc"];
            string kq = "";
            int stt = 1;
            foreach (ctCongThuc ct in cart.getListNguyenLieu())
            {
                if (ct.maNguyenLieu > 0) //Nếu bước đang duyệt có nguyên liệu thì hiện bảng
                {
                    kq += "<tr>";
                    kq += "     <th scope=\"row\">" + stt.ToString() + "</th>";
                    kq += "     <td>" + xulyDuLieu.traVeKyTuGoc(ct.nguyenLieu.tenNguyenLieu) + "</td>";
                    kq += "     <td>" + ct.soLuongNguyenLieu.ToString() + "</td>";
                    kq += "</tr>";
                }
                stt++;
            }
            return kq;
        }

        #endregion
        #endregion
        #region UPDATE
        /// <summary>
        /// Hàm thực hiện chỉnh sửa lại bước trong session
        /// </summary>
        /// <param name="maCt">Mã chi tiết để xác định chi tiết cần sửa</param>
        /// <param name="duLieu">Chuỗi chứa dữ liệu chỉnh sửa có dang: maNguyenLieu|buocSo|soLuong|donVi|hanhDong|ghiChu</param>
        /// <returns>Trả vê chuỗi htmlBangDanhSachCacBuoc|bảng danh sách nguyên liệu|tongGiaNguyenLieu</returns>
        public string chinhSuaBuoc(string duLieu, int maCt)
        {
            cartCongThuc cart = (cartCongThuc)Session["congThuc"];
            try
            {
                //-----Lấy ra 1 phần tử trong session với điều kiện.....
                ctCongThuc chiTietSua = cart.getItem(maCt);
                this.layDuLieuTuViewChiTiet(chiTietSua, duLieu);
                //----Xóa bước cũ trong session và tạo lại
                cart.removeItem(maCt);
                chiTietSua.maChiTiet = maCt;
                cart.addCart(chiTietSua);
                //------Cập nhật lại session
                Session["congThuc"] = cart;
            }
            catch (Exception ex)
            {
                xulyFile.ghiLoi("Class CongThucController - Function: chinhSuaBuoc", ex.Message);
            }
            return taoBangChiTietVaNguyenLieuSuDungTuSession();
        }
        /// <summary>
        /// Hàm tạo giao diện chỉnh sửa công thức
        /// </summary>
        /// <param name="maCongThuc"></param>
        /// <returns></returns>
        public ActionResult ct_ChinhSuaCongThuc()
        {
            this.resetData();
            try
            {
                string param = xulyChung.nhanThamSoTrongSession();
                if (param.Length > 0)
                {
                    int maCongThuc = xulyDuLieu.doiChuoiSangInteger(param);
                    qlCaPheEntities db = new qlCaPheEntities();
                    congThucPhaChe ctSua = db.congThucPhaChes.SingleOrDefault(c => c.maCongThuc == maCongThuc);
                    if (ctSua != null)
                    {
                        this.doDuLieuCongThucLenView(ctSua, db);
                        //------Gán dữ liệu trong bảng chi tiết vào session
                        cartCongThuc cart = (cartCongThuc)Session["congThuc"];
                        foreach (ctCongThuc ct in db.ctCongThucs.ToList().Where(c => c.maCongThuc == ctSua.maCongThuc))
                        {
                            cart.addCart(ct);
                            Session["congThuc"] = cart;
                        }
                        xulyChung.ghiNhatKyDtb(1, "Chỉnh sửa công thức pha chế của \" " + xulyDuLieu.traVeKyTuGoc(ctSua.sanPham.tenSanPham) + " \"");
                    }
                }
                else throw new Exception("không nhận được tham số");
            }
            catch (Exception ex)
            {
                xulyFile.ghiLoi("Class CongThucController - Function: ct_ChinhSuaCongThuc_Get", ex.Message);
                return RedirectToAction("PageNotFound", "Home");
            }
            return View();
        }
        /// <summary>
        /// Hàm thực hiện cập nhật thông tin công thức vào CSDL
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ct_ChinhSuaCongThuc(FormCollection f)
        {
            int kqLuu = 0;
            qlCaPheEntities db = new qlCaPheEntities(); congThucPhaChe ctSua = new congThucPhaChe();
            try
            {
                int maCT = xulyDuLieu.doiChuoiSangInteger(f["txtMaCongThuc"]);
                ctSua = db.congThucPhaChes.SingleOrDefault(c => c.maCongThuc == maCT);
                if (ctSua != null)
                {
                    //------Gán session dể chuyển đến danh sách  công thức của sản phẩm 
                    Session["urlAction"] = "page=/CongThuc/ct_TableCongThuc|request=" + ctSua.maSanPham.ToString();
                    //-------Cập nhật thông tin công thức
                    this.layDuLieuTuViewCongThuc(ctSua, f);
                    //-------Cập nhât lịch sử giá
                    this.themLichSuGiaVaoDatabase(db, f);
                    db.Entry(ctSua).State = System.Data.Entity.EntityState.Modified;
                    kqLuu = db.SaveChanges();
                    if (kqLuu > 0)
                    {
                        xulyChung.ghiNhatKyDtb(4, "Công thức pha chế của \" " + xulyDuLieu.traVeKyTuGoc(ctSua.sanPham.tenSanPham) + " \"");
                        //-----Tiến hành xóa những chi tiết công thức và thêm lại
                        this.xoaChiTiet(ctSua.maCongThuc, db);
                        this.themChiTietVaoDatabase(ctSua.maCongThuc, db);
                        //Xóa session
                        Session.Remove("congThuc");
                        return RedirectToAction("ct_TableCongThuc");
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.ThongBao = createHTML.taoThongBaoLuu(ex.Message);
                xulyFile.ghiLoi("Class CongThucController - Function: ct_ChinhSuaCongThuc_Post", ex.Message);
                this.doDuLieuCongThucLenView(ctSua, db);
            }
            return View();
        }



        /// <summary>
        /// Hàm thực hiện cập nhật trạng thái của 1 công thức
        /// Trạng thái cập nhật sẽ ngược với trạng thái hiện tại
        /// </summary>
        /// <param name="maCongThuc">Mã công thức cần cập nhật</param>
        public void capNhatTrangThai()
        {
            try
            {
                string param = xulyChung.nhanThamSoTrongSession(); int kqLuu = 0;
                if (param.Length > 0)
                {
                    int maCongThuc = xulyDuLieu.doiChuoiSangInteger(param);
                    qlCaPheEntities db = new qlCaPheEntities();
                    congThucPhaChe congThucSua = db.congThucPhaChes.SingleOrDefault(c => c.maCongThuc == maCongThuc);
                    this.capNhatCacCongThucFalse(congThucSua.sanPham, db);
                    if (congThucSua != null)
                    {
                        bool trangThaiCu = (bool)congThucSua.trangThai; //Lưu lại trạng thái cũ để chuyển đến danh sách tương ứng
                        congThucSua.trangThai = !trangThaiCu;//Cập nhật trạng thái ngược với trạng thái cũ
                        congThucSua.nguoiDuyet = ((taiKhoan)Session["login"]).tenDangNhap;
                        db.Entry(congThucSua).State = System.Data.Entity.EntityState.Modified;
                        kqLuu = db.SaveChanges();
                        if (kqLuu > 0)
                        {
                            xulyChung.ghiNhatKyDtb(4, "Cập nhật trạng thái công thức pha chế của \" " + xulyDuLieu.traVeKyTuGoc(congThucSua.sanPham.tenSanPham) + " \"");
                            //-------Thiết lập lại session để chuyển đến danh sách đúng nhất
                            Session["urlAction"] = "page=/CongThuc/ct_TableCongThuc|request=" + congThucSua.maSanPham.ToString();
                            Response.Redirect(xulyChung.layTenMien() + "/CongThuc/ct_TableCongThuc");
                        }
                    }
                    else
                        throw new Exception("Slide có mã " + maCongThuc + " không tồn tại để cập nhật");
                }
                else throw new Exception("không nhận được tham số");
            }
            catch (Exception ex)
            {
                xulyFile.ghiLoi("Class: CongThucController - Function: capNhatTrangThai", ex.Message);
                Response.Redirect(xulyChung.layTenMien() + "/Home/ServerError");
            }
        }
        /// <summary>
        /// Hàm cập nhật các công thức của sản phẩm true thành false
        /// </summary>
        /// <param name="sp"></param>
        /// <param name="db"></param>
        private void capNhatCacCongThucFalse(sanPham sp, qlCaPheEntities db)
        {
            foreach (congThucPhaChe ct in sp.congThucPhaChes.Where(c => c.trangThai == true).ToList())
            {
                ct.trangThai = false;
                db.Entry(ct).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
        }
        #endregion
        #region DELETE
        /// <summary>
        /// Hàm thực hiện xóa một công thức khỏi CSDL
        /// </summary>
        /// <param name="maCongThuc"></param>
        public void xoaCongThuc(int maCongThuc)
        {
            try
            {
                int kqLuu = 0;
                qlCaPheEntities db = new qlCaPheEntities();
                congThucPhaChe ctXoa = db.congThucPhaChes.SingleOrDefault(c => c.maCongThuc == maCongThuc);
                if (ctXoa != null)
                {
                    //-----xóa dữ liệu trong bảng chi tiết trước
                    this.xoaChiTiet(ctXoa.maCongThuc, db);
                    //------Tiến hành xóa công thức
                    db.congThucPhaChes.Remove(ctXoa);
                    kqLuu = db.SaveChanges();
                    if (kqLuu > 0)
                        xulyChung.ghiNhatKyDtb(3, "Công thức pha chế của \"" + xulyDuLieu.traVeKyTuGoc(ctXoa.sanPham.tenSanPham) + " \"");
                }
                else
                    throw new Exception("Công thức có mã " + maCongThuc.ToString() + " không tồn tại để xóa bỏ");
            }
            catch (Exception ex)
            {
                xulyFile.ghiLoi("Class CongThucController - Function: xoaCongThuc", ex.Message);
            }
        }
        /// <summary>
        /// hàm thực hiện xóa tất cả dữ liệu chi tiết của công thức trong Database
        /// </summary>
        /// <param name="maCongThuc"></param>
        private void xoaChiTiet(int maCongThuc, qlCaPheEntities db)
        {
            try
            {
                //------Lấy danh sách tất cả chi tiết của công thức
                List<ctCongThuc> dsChiTietXoa = db.ctCongThucs.Where(c => c.maCongThuc == maCongThuc).ToList();
                if (dsChiTietXoa.Count() > 0)
                {
                    db.ctCongThucs.RemoveRange(dsChiTietXoa);
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                xulyFile.ghiLoi("Class CongThucController - Function: xoaCongThuc", ex.Message);
            }
        }
        /// <summary>
        /// Hàm thực hiện xóa tất cả các bước chi tiết có trong Session
        /// </summary>
        /// <returns></returns>
        public string xoaTatCaBuoc()
        {
            string kq = "";
            //---Xóa sesion và tạo mới lại
            this.resetData();
            return kq;
        }
        /// <summary>
        /// Hàm thực hiện xóa 1 bước trong Session
        /// </summary>
        /// <param name="maCt">Mã chi tiết công thức cần xóa</param>
        /// <returns>Bảng danh sách các bước còn lại trong session</returns>
        public string xoaMotBuoc(int maCt)
        {
            cartCongThuc cart = (cartCongThuc)Session["congThuc"];
            if (maCt > 0)
            {
                //-------Lấy công thức cần xóa có trong cart
                ctCongThuc ctXoa = cart.getList().SingleOrDefault(c => c.maChiTiet == maCt);
                if (ctXoa != null)
                {
                    cart.removeItem(ctXoa.maChiTiet);
                    Session["congThuc"] = cart;
                }
            }
            //-----Trả về chuỗi bao gồm htmlTablBangDanhSachBuoc|BangDanhSachNguyenLieu|TongTienNguyenLieu
            return this.taoBangChiTietVaNguyenLieuSuDungTuSession();
        }
        #endregion
        #region ORTHERS
        /// <summary>
        /// Hàm lấy dữ liệu từ giao diện gán cho các thuộc tính có trong bảng congThucPhaChe
        /// </summary>
        /// <param name="congThuc"></param>
        /// <param name="f"></param>
        private void layDuLieuTuViewCongThuc(congThucPhaChe congThuc, FormCollection f)
        {
            string loi = "";
            congThuc.maSanPham = this.xuLyRequestLayMaSanPham();
            if (congThuc.maSanPham <= 0)
                loi += "Vui lòng chọn sản phẩm cần công thức pha chế <br/>";
            congThuc.tenCongThuc = xulyDuLieu.xulyKyTuHTML(f["txtTenCongThuc"]);
            if (congThuc.tenCongThuc.Length <= 0)
                loi += "Vui lòng nhập tên công thức <br/>";
            congThuc.dienGiai = xulyDuLieu.xulyKyTuHTML(f["txtDienGiai"]);
            if (congThuc.dienGiai.Length <= 0)
                loi += "Vui lòng nhập diễn giải về công thức <br/>";
            congThuc.ghiChu = xulyDuLieu.xulyKyTuHTML(f["txtGhiChu"]);
            congThuc.nguoiTao = ((taiKhoan)Session["login"]).tenDangNhap;
            congThuc.ngayTao = DateTime.Now;
            congThuc.trangThai = false; //-------Thiết lập trạng thái công thức chưa được sử dụng
            if (loi.Length > 0)
                throw new Exception(loi);
        }
        /// <summary>
        /// Hàm thực hiện lấy dữ liệu chi tiết gán vào các thuộc tính của chi tiết
        /// </summary>
        /// <param name="chiTiet"></param>
        /// <param name="duLieu">Chuỗi dữ liệu được lấy khi ajax gửi về. Dữ liệu là các giá trị trong textbox nhập vào chi tiết
        /// có dang: maNguyenLieu|buocSo|soLuong|donVi|hanhDong|ghiChu</param>
        private void layDuLieuTuViewChiTiet(ctCongThuc chiTiet, string duLieu)
        {
            string loi = "";
            //------Thực hiện xử lý cắt chuỗi duLieu để lấy giá trị của các thuộc tính
            int maNguyenLieu = xulyDuLieu.doiChuoiSangInteger(duLieu.Split('|')[0]);
            if (maNguyenLieu > 0)
            {   //-----Gán nguyên liệu vào chi tiết
                qlCaPheEntities db = new qlCaPheEntities();
                chiTiet.nguyenLieu = db.nguyenLieux.SingleOrDefault(c => c.maNguyenLieu == maNguyenLieu);
            }
            int buocSo = xulyDuLieu.doiChuoiSangInteger(duLieu.Split('|')[1]);
            if (buocSo <= 0)
                loi += "Vui lòng nhập bước thực hiện <br/>";
            double soLuongSuDung = xulyDuLieu.doiChuoiSangDouble(duLieu.Split('|')[2]);
            string donViSuDung = xulyDuLieu.xulyKyTuHTML(duLieu.Split('|')[3]);
            string hanhDong = xulyDuLieu.xulyKyTuHTML(duLieu.Split('|')[4]);
            if (hanhDong.Length <= 0)
                loi += "Vui lòng nhập hành động cho bước này <br/>";
            string ghiChu = xulyDuLieu.xulyKyTuHTML(duLieu.Split('|')[5]);
            if (loi.Length > 0)
                throw new Exception(loi);
            chiTiet.maChiTiet = ++maChiTietTam;
            chiTiet.maNguyenLieu = maNguyenLieu;
            chiTiet.buocSo = buocSo;
            chiTiet.soLuongNguyenLieu = soLuongSuDung;
            chiTiet.donViSuDung = donViSuDung;
            chiTiet.hanhDong = hanhDong;
            chiTiet.ghiChu = ghiChu;
        }
        /// <summary>
        /// Hàm thực hiện thiết lập lại các giá trị ban đầu
        /// Thiết lập lại session
        /// Gán script cho cbb chọn đồ uống cần pha chế
        /// </summary>
        private void resetData()
        {   //---Xóa sesion và tạo mới lại
            Session.Remove("congThuc"); Session.Add("congThuc", new cartCongThuc());
            maChiTietTam = 0;
        }
        /// <summary>
        /// Hàm thực hiện lấy thông tin chi tiết rồi đổ dữ liệu lên giao diện khi người dùng click vào chỉnh sửa chi tiết
        /// </summary>
        /// <param name="maCt"></param>
        /// <returns>Trả về chuỗi dữ liệu trong chi tiết theo dạng
        /// buocSo|hanhDong|ghiChu|soLuongSuDung|donViSuDung|maChiTiet|maNguyenLieuVaHinhAnh</returns>
        public string doDuLieuChiTietLenView(int maCt)
        {
            string kq = "";
            cartCongThuc cart = (cartCongThuc)Session["congThuc"];
            ctCongThuc ctSua = cart.getItem(maCt); ;
            if (ctSua != null)
            {
                string imgAndID = "";
                if (ctSua.nguyenLieu != null) //-----Kiểm tra xem có nguyên liệu để gán hình và mã nguyên liệu cập nhật
                {
                    //------Tạo chuỗi hiện hình nguyên liệu nếu bước có nguyên liệu. 
                    imgAndID += "<img id=\"hinhNguyenLieu\" class='img img-responsive img-thumbnail'";
                    imgAndID += "src=\"" + xulyDuLieu.chuyenByteHinhThanhSrcImage(ctSua.nguyenLieu.hinhAnh) + "\" width=\"250px\" height=\"auto\" />";
                    imgAndID += "<input id=\"maNguyenLieuDaChon\" type=\"hidden\" value=\"" + ctSua.maNguyenLieu.ToString() + "\" />";
                }
                kq += ctSua.buocSo.ToString() + "|" + xulyDuLieu.traVeKyTuGoc(ctSua.hanhDong) + "|" + xulyDuLieu.traVeKyTuGoc(ctSua.ghiChu) + "|" + ctSua.soLuongNguyenLieu.ToString() + "|" + xulyDuLieu.traVeKyTuGoc(ctSua.donViSuDung) + "|" + ctSua.maChiTiet.ToString() + "|" + imgAndID;
            }
            return kq;
        }
        /// <summary>
        /// Hàm thực hiện đổ dữ liệu của công thức lên giao diện
        /// </summary>
        /// <param name="congThuc"></param>
        private void doDuLieuCongThucLenView(congThucPhaChe congThuc, qlCaPheEntities db)
        {
            ViewBag.txtMaCongThuc = congThuc.maCongThuc.ToString();
            ViewBag.txtTenCongThuc = xulyDuLieu.traVeKyTuGoc(congThuc.tenCongThuc);
            ViewBag.txtDienGiai = xulyDuLieu.traVeKyTuGoc(congThuc.dienGiai);
            ViewBag.txtGhiChu = xulyDuLieu.traVeKyTuGoc(congThuc.ghiChu);

            ViewBag.TitleListCongThuc = xulyChung.taoUrlCoTruyenThamSo("CongThuc/ct_TableCongThuc", congThuc.maSanPham.ToString());
            ViewBag.TitleCreateCongThuc = xulyChung.taoUrlCoTruyenThamSo("CongThuc/ct_TaoMoiCongThuc", congThuc.maSanPham.ToString());
            ViewBag.hinhSP = xulyDuLieu.chuyenByteHinhThanhSrcImage(db.sanPhams.SingleOrDefault(s => s.maSanPham == congThuc.maSanPham).hinhAnh);
        }
        /// <summary>
        /// Hàm gán các script và html tạo modal trên view danh sách công thức
        /// </summary>
        private void thietLapThongSoChung()
        {
            ViewBag.ScriptAjax = createScriptAjax.scriptAjaxXoaDoiTuong("CongThuc/xoaCongThuc?maCongThuc=");
            ViewBag.ModalXoa = createHTML.taoCanhBaoXoa("Công thức pha chế");
            //-------Tạo script ajax cho việc click vào sản phẩm để xem chi tiết công thức
            ViewBag.ScriptAjaxXemChiTiet = createScriptAjax.scriptAjaxXemChiTietKhiClick("goiY", "maCongThuc", "CongThuc/AjaxXemChiTietCongThuc?maCongThuc=", "vungChiTiet", "modalChiTiet");
            //-----Tạo modal dạng lớn để chứa chi tiết các bước thực hiện của công thức
            ViewBag.ModalChiTiet = createHTML.taoModalChiTiet("modalChiTiet", "vungChiTiet", 3);
        }
        /// <summary>
        /// Hàm xử lý request có trong session để lấy MÃ SẢN PHẨM
        /// </summary>
        /// <returns>Mã sản phẩm có trong session</returns>
        private int xuLyRequestLayMaSanPham()
        {
            int maSanPham = 0;
            string param = (string)Session["urlAction"];//-----Nhận tham số mã sản phẩm cần tạo công thức. param = page=/CongThuc/ct_TaoMoiCongThuc|request=maSanPham
            if (param.Length > 0)
            {
                param = param.Split('|')[1];//------param = request=maSanPham
                maSanPham = xulyDuLieu.doiChuoiSangInteger(param.Replace("request=", ""));
            }
            else
                throw new Exception("không nhận được tham số");
            return maSanPham;
        }

        #endregion
    }
}