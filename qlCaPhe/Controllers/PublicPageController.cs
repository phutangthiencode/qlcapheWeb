using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using qlCaPhe.App_Start;
using qlCaPhe.Models;
using qlCaPhe.App_Start.Cart;

namespace qlCaPhe.Controllers
{
    public class PublicPageController : Controller
    {
        /// <summary>
        /// Hàm tọa giao diện vùng header chứa menu navigation trên giao diện
        /// </summary>
        /// <returns></returns>
        public ActionResult Part_Header()
        {
            cauHinh cauHinh = new cauHinh();
            try
            {
                cauHinh = new qlCaPheEntities().cauHinhs.First();
            }
            catch (Exception ex)
            {
                xulyFile.ghiLoi("Class: PublicPageController - Function: Part_Header", ex.Message);
                Response.Redirect(xulyChung.layTenMien() + "Home/ServerError");
            }
            return PartialView(cauHinh);
        }

        #region Trang chủ
        /// <summary>
        /// Giao diện cho trang chủ
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View();
        }


        /// <summary>
        /// Hàm tạo giao diện phần banner 1 trên trang chủ
        /// Chứa hình ảnh, slideshow...
        /// </summary>
        /// <returns>Trả về danh sách slideshow được hiển thị </returns>
        public ActionResult PartHome_Banner1()
        {
            List<slide> listSlide = new List<slide>();
            try
            {
                listSlide = new qlCaPheEntities().slides.Where(s => s.trangThai == true).ToList();
            }
            catch (Exception ex)
            {
                xulyFile.ghiLoi("Class: PublicPageController - Function: PartHome_Banner1", ex.Message);
                Response.Redirect(xulyChung.layTenMien() + "Home/ServerError");
            }
            return PartialView(listSlide);
        }

        /// <summary>
        /// Hàm tạo giao diện vùng bài viết tại trang chủ
        /// "Thông tin nổi bật"
        /// </summary>
        /// <returns>List object bài viết hiện trang chủ</returns>
        public ActionResult PartHome_BaiViet()
        {
            List<baiViet> listBaiViet = new List<baiViet>();
            try
            {
                listBaiViet = new qlCaPheEntities().baiViets.Where(s => s.trangThai == 1 && s.hienTrangChu == true).ToList();
            }
            catch (Exception ex)
            {
                xulyFile.ghiLoi("Class: PublicPageController - Function: PartHome_BaiViet", ex.Message);
                Response.Redirect(xulyChung.layTenMien() + "Home/ServerError");
            }
            return PartialView(listBaiViet);
        }

        /// <summary>
        /// Hàm tạo giao diện vùng các nguyên liệu bán chạy
        /// Lấy 4 sản phẩm bán chạy nhất 
        /// </summary>
        /// <returns>List nguyên liệu bán chạy (Có nhiều trong ctHoaDonOrder)</returns>
        public ActionResult PartHome_SanPhamNoiTroi()
        {
            List<sanPham> listSanPham = new List<sanPham>();
            try
            {
                qlCaPheEntities db = new qlCaPheEntities();
                //----Khởi tạo biến chứa danh sách object Mã sản phẩm bán chạy và số lần bán
                var listSellest = db.ctHoaDonOrders.GroupBy(c => c.maSanPham).Select(c => new { maSP = c.Key, count = c.Count() }).OrderByDescending(c => c.count).Take(4).ToList(); //---Lấy 4 sản phẩm bán chạy 1
                for (int i = 0; i < listSellest.Count; i++)
                {
                    //-----Lấy giá trị là mã sản phẩm có trong listSellest
                    var itemSellest = listSellest[i];
                    int maSP = xulyDuLieu.doiChuoiSangInteger(xulyDuLieu.layThuocTinhTrongMotObject(itemSellest, "maSP"));

                    //------Lấy thông tin sản phẩm và gán vào listSanPham
                    sanPham spSellest = db.sanPhams.SingleOrDefault(s => s.maSanPham == maSP);
                    if (spSellest != null)
                        listSanPham.Add(spSellest);
                }
            }
            catch (Exception ex)
            {
                xulyFile.ghiLoi("Class: PublicPageController - Function: PartHome_SanPhamNoiTroi", ex.Message);
                Response.Redirect(xulyChung.layTenMien() + "Home/ServerError");
            }
            return PartialView(listSanPham);
        }
        /// <summary>
        /// Hàm tạo vùng giao diện Danh sách loại sản phẩm trên trang chủ
        /// </summary>
        /// <returns>List object loại sản phẩm có sản phẩm</returns>
        public ActionResult PartHome_LoaiSanPham()
        {
            List<loaiSanPham> listSanPham = new List<loaiSanPham>();
            try
            {
                listSanPham = new qlCaPheEntities().loaiSanPhams.Where(l => l.sanPhams.Count > 0).ToList();
            }
            catch (Exception ex)
            {
                xulyFile.ghiLoi("Class: PublicPageController - Function: PartHome_LoaiSanPham", ex.Message);
                Response.Redirect(xulyChung.layTenMien() + "Home/ServerError");
            }
            return PartialView(listSanPham);
        }
        #endregion

        #region Trang Sản phẩm


        /// <summary>
        /// Giao diện trang danh mục sản phẩm
        /// </summary>
        /// <returns></returns>
        public ActionResult DanhSachSanPham(int? page)
        {
            List<sanPham> listSanPham = new List<sanPham>(); int pageSize = 8;
            try
            {
                int trangHienHanh = (page ?? 1);
                qlCaPheEntities db = new qlCaPheEntities();
                int soPhanTu = db.sanPhams.Where(s => s.trangThai == 1).Count();
                ViewBag.PhanTrang = createHTML.taoPhanTrang(soPhanTu, pageSize, trangHienHanh, "/PublicPage/DanhSachSanPham"); //------cấu hình phân trang
                listSanPham = db.sanPhams.Where(s => s.trangThai == 1).OrderBy(c => c.tenSanPham).Skip((trangHienHanh - 1) * pageSize).Take(pageSize).ToList();
                ViewBag.ListSanPham = taoDanhSachSanPham(listSanPham);
            }
            catch (Exception ex)
            {
                xulyFile.ghiLoi("Class: PublicPageController - Function: DanhSachSanPham", ex.Message);
                Response.Redirect(xulyChung.layTenMien() + "Home/ServerError");
            }
            return View();
        }
        /// <summary>
        /// Hàm tạo danh sách sản phầm và hiện lên giao diện
        /// </summary>
        /// <param name="list">List chứa object sản phẩm cần hiển thị</param>
        /// <returns>Trả về chuổi hmlt toàn bộ danh sách hiển thị</returns>
        private string taoDanhSachSanPham(List<sanPham> list)
        {
            string kq = ""; int index = 0;
            foreach (sanPham sp in list)
            {
                index++;
                if (index == 1)
                    kq += "<div class=\"special-top\">";
                kq += "   <div class=\"col-md-3 special-in\">";
                kq += "       <a href=\"~/PublicPage/ChiTietSanPham\">";
                kq += "           <img src=\"" + xulyDuLieu.chuyenByteHinhThanhSrcImage(sp.hinhAnh) + "\" class=\"img-responsive\" alt=\"" + xulyDuLieu.traVeKyTuGoc(sp.tenSanPham) + "\">";
                kq += "       </a>";
                kq += "       <h5><a href=\"~/PublicPage/ChiTietSanPham\">" + xulyDuLieu.traVeKyTuGoc(sp.tenSanPham) + "</a></h5>";
                kq += "       <p>" + xulyDuLieu.doiVND(sp.donGia) + "</p>";
                kq += "   </div>";
                if (index == 4)
                {
                    kq += "</div>";
                    index = 0;
                }
            }
            return kq;
        }
        /// <summary>
        /// Giao diện chi tiết sản phẩm
        /// </summary>
        /// <returns></returns>
        public ActionResult ChiTietSanPham()
        {
            return View();
        }
        #endregion

        #region Trang Đặt bàn
        /// <summary>
        /// Giao diện danh sách bàn có trong quán để đặt bàn
        /// </summary>
        /// <returns></returns>
        public ActionResult DanhSachBan()
        {
            try
            {
                this.taoCbbKhuVucDanhMucBan(new qlCaPheEntities());
            }
            catch (Exception ex)
            {
                xulyFile.ghiLoi("Class: PublicPageController - Function: DanhSachBan", ex.Message);
                Response.Redirect(xulyChung.layTenMien() + "Home/ServerError");
            }
            return View();
        }

        /// <summary>
        /// Hàm xử lý sự kiện liệt kê danh mục bàn
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DanhSachBan(FormCollection f)
        {
            try
            {
                int maKV = xulyDuLieu.doiChuoiSangInteger(f["cbbKhuVuc"]);
                int soLuongChoNgoi = xulyDuLieu.doiChuoiSangInteger(f["txtSoLuong"]);
                this.lietKeDanhMucBan(maKV, soLuongChoNgoi);
            }
            catch (Exception ex)
            {
                xulyFile.ghiLoi("Class: PublicPageController - Function: DanhSachBan_Post", ex.Message);
                Response.Redirect(xulyChung.layTenMien() + "Home/ServerError");
            }
            this.taoCbbKhuVucDanhMucBan(new qlCaPheEntities());
            return View();
        }

        private void lietKeDanhMucBan(int maKV, int soLuongChoNgoi)
        {
            string kq = "<br><h3>Không có bàn theo mục tiêu đã liệt kê</h3>";
            try
            {
                int index = 0;
                List<BanChoNgoi> listBan = new qlCaPheEntities().BanChoNgois.Where(b => b.trangThai == 1 && b.maKhuVuc == maKV && b.sucChua == soLuongChoNgoi).ToList();
                if (listBan.Count > 0)
                {
                    kq = "";
                    foreach (BanChoNgoi ban in listBan)
                    {
                        index++;
                        if (index == 1)
                            kq += "<div class=\"special-top\">";
                        kq += "     <div class=\"col-md-3 special-in\">";
                        kq += "         <a data-toggle=\"modal\" data-target=\"#modal-detail-table\">";
                        kq += "             <img src=\"" + xulyDuLieu.chuyenByteHinhThanhSrcImage(ban.hinhAnh) + "\" class=\"img-responsive\" alt=\"hinh_anh_ban" + xulyDuLieu.traVeKyTuGoc(ban.tenBan) + "\">";
                        kq += "         </a>";
                        kq += "         <div class=\"item-detail-table\">";
                        kq += "             <h5><a task=\"" + ban.maBan.ToString() + "\" class=\"js-btn-openDetail\"><b>" + xulyDuLieu.traVeKyTuGoc(ban.tenBan) + "</b> - " + ban.sucChua.ToString() + " chỗ</a></h5>";
                        kq += "             <button class=\"js-btn-datBan\" task=\"" + ban.maBan.ToString() + "\"><span class=\"glyphicon glyphicon-check\" style=\"color:#ffffff; padding-right:20px;\"></span>Đặt bàn</button>";
                        kq += "         </div>";
                        kq += "     </div>";
                        if (index == 4)
                        {
                            kq += "     <div class=\"clearfix\"></div>";
                            kq += " </div>"; //---Đóng tag special-top
                            index = 0;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                xulyFile.ghiLoi("Class: PublicPageController - Function: lietKeDanhMucBan", ex.Message);
                Response.Redirect(xulyChung.layTenMien() + "Home/ServerError");
            }
            ViewBag.DanhMucBan = kq;
        }

        /// <summary>
        /// Hàm tạo danh sách khu vực cho giao diện Danh mục bàn
        /// </summary>
        /// <param name="db"></param>
        private void taoCbbKhuVucDanhMucBan(qlCaPheEntities db)
        {
            string kq = "";
            try
            {
                foreach (khuVuc kv in db.khuVucs.ToList())
                    kq += "<option value=\"" + kv.maKhuVuc.ToString() + "\">" + xulyDuLieu.traVeKyTuGoc(kv.tenKhuVuc) + "</option>";
            }
            catch (Exception ex)
            {
                xulyFile.ghiLoi("Class: PublicPageController - Function: taoCbbKhuVucDanhMucBan", ex.Message);
                Response.Redirect(xulyChung.layTenMien() + "Home/ServerError");
            }
            ViewBag.CbbKhuVuc = kq;
        }
        /// <summary>
        /// Hàm tạo giao diện modal chi tiết bàn khi ajax gọi đến
        /// </summary>
        /// <param name="param">Tham số chứa mã bàn cần lấy thông tin</param>
        /// <returns>Trả về chuỗi html tạo modal chi tiết bàn</returns>
        public string AjaxXemChiTietBan(string param)
        {
            string kq = "";
            try
            {
                int maBan = xulyDuLieu.doiChuoiSangInteger(param);
                BanChoNgoi ban = new qlCaPheEntities().BanChoNgois.SingleOrDefault(b => b.trangThai == 1 && b.maBan == maBan);
                if (ban != null)
                {
                    kq += "<div class=\"modal fade\" id=\"modal-detail-table\" role=\"dialog\" style=\"display: none;\">";
                    kq += "    <div class=\"modal-dialog modal-lg\">";
                    kq += "        <div class=\"modal-content\">";
                    kq += "            <div class=\"modal-header\">";
                    kq += "                <button type=\"button\" class=\"close\" data-dismiss=\"modal\">×</button>";
                    kq += "                <h4 class=\"modal-title\">Thông tin chi tiết bàn \"" + xulyDuLieu.traVeKyTuGoc(ban.tenBan) + "\"</h4>";
                    kq += "            </div>";
                    kq += "            <div class=\"modal-body\">";
                    kq += "                 <div class=\"row\">";
                    kq += "                     <div class=\"col-md-6 col-lg-6 col-xs-6 col-sm-6\">";
                    kq += "                         <img src=\"" + xulyDuLieu.chuyenByteHinhThanhSrcImage(ban.hinhAnh) + "\" class=\"img-responsive\" alt=\"\">";
                    kq += "                     </div>";
                    kq += "                     <div class=\"col-md-6 col-lg-6 col-xs-6 col-sm-6\">";
                    kq += "                         <ul class=\"detail-table\">";
                    kq += "                             <li><strong>Tên bàn: </strong>" + xulyDuLieu.traVeKyTuGoc(ban.tenBan) + "</li>";
                    kq += "                             <li><strong>Khu vực:;</strong>" + xulyDuLieu.traVeKyTuGoc(ban.khuVuc.tenKhuVuc) + "</li>";
                    kq += "                             <li><strong>Sức chứa:</strong>" + ban.sucChua.ToString() + " chỗ</li>";
                    kq += "                             <li><strong>Giới thiệu:</strong>" + xulyDuLieu.traVeKyTuGoc(ban.gioiThieu) + " </li>";
                    kq += "                         </ul>";
                    kq += "                     </div>";
                    kq += "               </div>";
                    kq += "               <div class=\"row\">";
                    kq += "                 <div class=\"col-md-9 col-lg-9 col-xs-9 col-sm-9\"></div>";
                    kq += "                 <div class=\"col-md-3 col-lg-3 col-xs-3 col-sm-3\">";
                    kq += "                     <button type=\"button\" task=\"" + ban.maBan.ToString() + "\" class=\"js-btn-datBan btn btn-primary\"><span class=\"glyphicon glyphicon-check\" style=\"color:#ffffff; padding-right:20px;\"></span>Đặt bàn</button>";
                    kq += "                 </div>";
                    kq += "               </div>";
                    kq += "            </div>";
                    kq += "        </div>";
                    kq += "    </div>";
                    kq += "</div>";
                }
            }
            catch (Exception ex)
            {
                xulyFile.ghiLoi("Class: PublicPageController - Function: AjaxXemChiTietBan", ex.Message);
                Response.Redirect(xulyChung.layTenMien() + "Home/ServerError");
            }
            return kq;
        }
        /// <summary>
        /// Hàm thực hiện Thêm mới 1 bàn vào giỏ đặt bàn
        /// </summary>
        /// <param name="param">Tham số chứa mã bàn cần dặt</param>
        /// <returns>Chuỗi chứa số lượng bàn</returns>
        public string AjaxDatBan(string param)
        {
            int soLuongBanDat = 0;
            cartDatBan cart = (cartDatBan)Session["datBan"];
            try
            {
                int maBan = xulyDuLieu.doiChuoiSangInteger(param);
                qlCaPheEntities db = new qlCaPheEntities();
                BanChoNgoi banDat = db.BanChoNgois.SingleOrDefault(b => b.maBan == maBan);
                if (banDat != null)
                {
                    //------Thêm bàn cần đặt vào giỏ 
                    ctDatBan ctAdd = new ctDatBan();
                    ctAdd.maBan = banDat.maBan;
                    ctAdd.ghiChu = "";
                    ctAdd.BanChoNgoi = banDat;
                    cart.addCart(ctAdd);
                    Session["datBan"] = cart;
                }
                soLuongBanDat = cart.Info.Count;
            }
            catch (Exception ex)
            {
                xulyFile.ghiLoi("Class: PublicPageController - Function: AjaxDatBan", ex.Message);
                Response.Redirect(xulyChung.layTenMien() + "Home/ServerError");
            }
            return "Đã đặt: " + soLuongBanDat.ToString() + " bàn";
        }
        /// <summary>
        /// hàm tạo giao diện checkout
        /// </summary>
        /// <returns></returns>
        public ActionResult GioDatBan()
        {
            cartDatBan cart = (cartDatBan)Session["datBan"];
            try
            {
                //-----Nếu trong giỏ chưa có bàn thì chuyển đến giao diện đặt bàn
                if (cart.Info.Values.Count > 0)
                {
                    ViewBag.TableCheckout = taoDuLieuBangBanDaDat();
                    ViewBag.TongSucChua = "Tổng sức chứa " + cart.getTotalCapacity();
                }
                else
                    return RedirectToAction("DanhSachBan");
            }
            catch (Exception ex)
            {
                xulyFile.ghiLoi("Class: PublicPageController - Function: GioDatBan", ex.Message);
                Response.Redirect(xulyChung.layTenMien() + "Home/ServerError");
            }
            return View();
        }
        /// <summary>
        /// Hàm xóa 1 bàn đã order
        /// </summary>
        /// <param name="param">Mã bàn đã order cần xóa</param>
        /// <returns>Chuồi html tạo bảng các bàn còn lại | tổng số lượng bàn đã xóa</returns>
        public string AjaxXoaOrderBan(string param)
        {
            string kq = "";
            try
            {
                cartDatBan cart = (cartDatBan)Session["datBan"];
                int maBanXoa = xulyDuLieu.doiChuoiSangInteger(param);
                cart.removeItem(maBanXoa);
                kq += taoDuLieuBangBanDaDat() + "&&&Tổng sức chứa: " + cart.getTotalCapacity();
            }
            catch (Exception ex)
            {
                xulyFile.ghiLoi("Class: PublicPageController - Function: AjaxXoaOrderBan", ex.Message);
                Response.Redirect(xulyChung.layTenMien() + "Home/ServerError");
            }
            return kq;
        }
        /// <summary>
        /// hàm tạo dữ liệu cho bảng bàn đã order
        /// </summary>
        /// <returns>Chuỗi html tạo giao diện bàn</returns>
        private string taoDuLieuBangBanDaDat()
        {
            string kq = "";
            try
            {
                cartDatBan cart = (cartDatBan)Session["datBan"];
                foreach (ctDatBan item in cart.Info.Values)
                {
                    kq += "<tr>";
                    kq += "      <td>";
                    kq += "          <img src=\""+xulyDuLieu.chuyenByteHinhThanhSrcImage(item.BanChoNgoi.hinhAnh)+"\" style=\"width:100%; height:auto;\">";
                    kq += "          <b class=\"table-ordered-tablename\">"+xulyDuLieu.traVeKyTuGoc(item.BanChoNgoi.tenBan)+"</b>";
                    kq += "      </td>";
                    kq += "      <td>"+xulyDuLieu.traVeKyTuGoc(item.BanChoNgoi.khuVuc.tenKhuVuc)+"</td>";
                    kq += "      <td>"+item.BanChoNgoi.sucChua.ToString()+"</td>";
                    kq += "      <td><button task=\""+item.maBan.ToString()+"\" class=\"js-btn-xoaban btn btn-danger btn-sm\"><i class=\"glyphicon glyphicon-trash\"></i></button></td>";
                    kq += "  </tr>";
                }
            }
            catch (Exception ex)
            {
                xulyFile.ghiLoi("Class: PublicPageController - Function: taoDuLieuBangBanDaDat", ex.Message);
                Response.Redirect(xulyChung.layTenMien() + "Home/ServerError");
            }
            return kq;
        }
        /// <summary>
        /// Hàm thêm mới một đặt bàn vào CSDL
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GioDatBan(datBanOnline datBan, FormCollection f)
        {
            cartDatBan cart = (cartDatBan)Session["datBan"];
            int kqLuu = 0;
            if(cart.Info.Count>0)
                try
                {
                    qlCaPheEntities db = new qlCaPheEntities();
                    if (ModelState.IsValid)
                    {
                        this.layDuLieuTuViewCheckout(f, datBan);
                        db.datBanOnlines.Add(datBan);
                        kqLuu = db.SaveChanges();
                        if (kqLuu > 0)
                        {
                            this.themChiTietDatBan(db, cart, datBan.maDatBan);
                            this.xoaSessionDatBan();
                            return RedirectToAction("Index");
                        }
                    }else{
                        ViewBag.TableCheckout = taoDuLieuBangBanDaDat();
                        ViewBag.TongSucChua = "Tổng sức chứa " + cart.getTotalCapacity();
                    }
                }
                catch (Exception ex)
                {
                    xulyFile.ghiLoi("Class: PublicPageController - Function: GioDatBan", ex.Message);
                }
            return View(datBan);
        }
        /// <summary>
        /// Hàm nhận dữ liệu từ giao diện 
        /// </summary>
        /// <param name="f"></param>
        /// <param name="d"></param>
        private void layDuLieuTuViewCheckout(FormCollection f, datBanOnline d)
        {
            d.hoTenKH = xulyDuLieu.xulyKyTuHTML(f["hoTenKH"]).Trim();
            d.SDT = xulyDuLieu.xulyKyTuHTML(f["SDT"]).Trim();
            d.soLuongKhach = xulyDuLieu.doiChuoiSangInteger(f["soLuongKhach"]);
            d.ngayDenDuKien = xulyDuLieu.doiChuoiSangDateTime(f["ngayDenDuKien"]);
            d.ngayDat = DateTime.Now;
            d.trangThai = 0;
            d.yeuCauThem = xulyDuLieu.xulyKyTuHTML(f["yeuCauThem"]);
        }
        /// <summary>
        /// Hàm thực hiện thêm chi tiết đặt bàn online vào CSDL
        /// </summary>
        /// <param name="db"></param>
        /// <param name="cart">Object chứa danh sách bàn đã đặt trong giỏ</param>
        private void themChiTietDatBan(qlCaPheEntities db, cartDatBan cart, int maDatBan)
        {
            try
            {
                foreach(ctDatBan item in cart.Info.Values)
                {
                    ctDatBan ctAdd = new ctDatBan();
                    ctAdd.ghiChu = "";
                    ctAdd.maBan = item.maBan;
                    ctAdd.maDatBan = maDatBan;
                    db.ctDatBans.Add(ctAdd);
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                xulyFile.ghiLoi("Class: PublicPageController - Function: themChiTietDatBan", ex.Message);
            }
        }
        /// <summary>
        /// Hàm xóa bỏ session đặt bàn và tạo mới lại
        /// </summary>
        private void xoaSessionDatBan()
        {
            Session.Remove("datBan"); 
            Session.Add("datBan", new cartDatBan()); 
        }
        #endregion

        /// <summary>
        /// Giao diện danh sách bài viết
        /// </summary>
        /// <returns></returns>
        public ActionResult DanhSachBaiViet()
        {
            return View();
        }
        /// <summary>
        /// Giao diện chi tiết bài viết
        /// </summary>
        /// <returns></returns>
        public ActionResult ChiTietBaiViet()
        {
            return View();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult ThongTinLienHe()
        {
            return View();
        }
        /// <summary>
        /// Giao diện gửi phản hồi
        /// </summary>
        /// <returns></returns>
        public ActionResult GuiPhanHoi()
        {
            return View();
        }

        /// <summary>
        /// Hàm tạo vùng giao diện footer
        /// </summary>
        /// <returns></returns>
        public ActionResult Part_Footer()
        {
            cauHinh cauHinh = new cauHinh();
            try
            {
                cauHinh = new qlCaPheEntities().cauHinhs.First();
            }
            catch (Exception ex)
            {
                xulyFile.ghiLoi("Class: PublicPageController - Function: Part_Footer", ex.Message);
                Response.Redirect(xulyChung.layTenMien() + "Home/ServerError");
            }
            return PartialView(cauHinh);
        }
    }
}