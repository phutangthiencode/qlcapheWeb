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
    public class TonKhoController : Controller
    {
        private string idOfPage = "804";

        #region KIỂM KHO NEW
        /// <summary>
        /// Hàm tạo giao diện cho chức năng kiểm kê kho hàng
        /// </summary>
        /// <returns></returns>
        public ActionResult tkho_KiemKho()
        {
            if (xulyChung.duocTruyCap(idOfPage))
            {
                try
                {
                    //-----Khởi tạo lại dữ liệu cho các giỏ nguyên liệu
                    this.xoaSession();
                    cartKiemKho cartTruoc = (cartKiemKho)Session["truocKiemKho"];
                    List<ctTonKho> listTon = new bTonKho().layDanhSachTon();
                    //------Lặp qua danh sách tồn kho theo tháng
                    foreach (ctTonKho item in listTon)
                    {
                        cartTruoc.addCart(item);
                        Session["kiemKho"] = cartTruoc;
                    }
                    xulyChung.ghiNhatKyDtb(1, "Kiểm kho");
                }
                catch (Exception ex)
                {
                    xulyFile.ghiLoi("Class: TonKhoController - Function: tkho_KiemKho", ex.Message);
                }
            }
            return View();
        }
        /// <summary>
        /// Hàm thực hiện lưu thông tin kiểm kho của tất cả nguyên liệu vào CSDL
        /// Khi người dùng click vào nút Hoàn tất kiểm kho
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult tkho_KiemKho(FormCollection f)
        {
            if (xulyChung.duocCapNhat(idOfPage, "7"))
            {
                string ndThongBao = ""; int kqLuu = 0;
                try
                {
                    cartKiemKho cartTruoc = (cartKiemKho)Session["truocKiemKho"];
                    if (cartTruoc.Info.Values.Count > 0)
                        throw new Exception("<i class=\"col-red\">*Còn nguyên liệu chưa được kiểm kê, vui lòng kiểm kê lại nguyên liệu</i>");

                    cartKiemKho cartSau = (cartKiemKho)Session["sauKiemKho"];
                    qlCaPheEntities db = new qlCaPheEntities();
                    TonKho tonKho = new TonKho();
                    tonKho.ngayKiem = DateTime.Now;
                    tonKho.tongTien = cartSau.getTotalPrice();
                    tonKho.nguoiKiem = ((taiKhoan)Session["login"]).tenDangNhap;
                    tonKho.ghiChu = xulyDuLieu.xulyKyTuHTML(f["txtGhiChu"]);
                    //-----Thêm mới tồn kho vào CSDL
                    db.TonKhoes.Add(tonKho);
                    kqLuu = db.SaveChanges();
                    if (kqLuu > 0)
                    {
                        themChiTietTonKho(tonKho.maSoKy, cartSau, db);
                        ndThongBao = createHTML.taoNoiDungThongBao("Kiểm kê kho", tonKho.maSoKy.ToString(), "tkho_TableTonKho");
                        this.xoaSession();
                        xulyChung.ghiNhatKyDtb(2, "Kiểm kê kho \" " + tonKho.maSoKy.ToString() + " \"");
                    }
                }
                catch (Exception ex)
                {
                    ndThongBao = ex.Message;
                    ViewBag.txtGhiChu = f["txtGhiChu"];
                    xulyFile.ghiLoi("Class: TonKhoController - Function: tkho_KiemKho_Post", ex.Message);
                }
                ViewBag.ThongBao = createHTML.taoThongBaoLuu(ndThongBao);
            }
            return View();
        }
        /// <summary>
        /// Hàm thực hiện thêm các nguyên liệu đã kiểm kê vào bảng chi tiết tồn kho
        /// </summary>
        /// <param name="maSoKy">Mã số kỳ kiểm kê</param>
        /// <param name="cart">Object chứa giỏ nguyên liệu đã kiểm kê</param>
        /// <param name="db"></param>
        private void themChiTietTonKho(int maSoKy, cartKiemKho cart, qlCaPheEntities db)
        {
            foreach (ctTonKho item in cart.Info.Values)
            {
                ctTonKho ctAdd = new ctTonKho();
                ctAdd.maSoKy = maSoKy;
                ctAdd.maNguyenLieu = item.maNguyenLieu;
                ctAdd.donGia = item.donGia;
                ctAdd.soLuongDauKy = item.soLuongDauKy;
                ctAdd.soLuongCuoiKyLyThuyet = item.soLuongCuoiKyLyThuyet;
                ctAdd.soLuongThucTe = item.soLuongThucTe;
                ctAdd.tyLeHaoHut = item.tyLeHaoHut;
                ctAdd.nguyenNhanHaoHut = item.nguyenNhanHaoHut;
                db.ctTonKhoes.Add(ctAdd);
                db.SaveChanges();
                new bKiemKho().capNhatTongTienTonKho(ctAdd, db);
            }
        }

        private void xoaSession()
        {
            Session.Remove("truocKiemKho"); Session.Remove("sauKiemKho");
            Session.Add("truocKiemKho", new cartKiemKho()); Session.Add("sauKiemKho", new cartKiemKho());
        }

        /// <summary>
        /// Hàm được Ajax gọi thực thi lấy danh sách các nguyên liệu cần kiểm và hiển thị lên modal
        /// </summary>
        /// <returns>Chuỗi html tạo nên content của modal</returns>
        public string AjaxLayDanhSachNguyenLieuCanKiem()
        {
            string kq = "";
            try
            {
                qlCaPheEntities db = new qlCaPheEntities(); cauHinh cauHinh = db.cauHinhs.First();
                kq += "         <div class=\"row text-center\">";
                kq += "             <h4>" + xulyDuLieu.traVeKyTuGoc(cauHinh.tenQuan) + "</h4><label><i>Địa chỉ: " + xulyDuLieu.traVeKyTuGoc(cauHinh.diaChi) + "</i></label><label><i>ĐT: 0283182333</i></label><br><label>*********************</label>";
                kq += "         </div>";
                kq += "         <div class=\"row\">";
                kq += "             <h4 class=\"text-center\">DANH SÁCH NGUYÊN LIỆU CẦN KIỂM KÊ</h4><b class=\"pull-right\">Ngày " + DateTime.Now.ToString() + "</b>";
                kq += "         </div>";
                kq += "         <div class=\"row\">";
                kq += "             <div class=\"col-lg-12 col-md-12 table-responsive\">";
                kq += "                 <table class=\"table table-bordered\">";
                kq += "                     <thead>";
                kq += "                         <tr><th width=\"5%\">#</th><th width=\"25%\">Nguyên liệu</th><th width=\"15%\">Số lượng</th><th width=\"15%\">Đơn vị tính</th><th width=\"25%\">Nguyên nhân hao hụt</th><th width=\"15%\">Ghi chú</th></tr>";
                kq += "                     </thead>";
                kq += "                     <tbody>";
                int index = 1;
                //---------Lấy danh sách các nguyên liệu cần kiểm kho. Có trong nhập kho trong 1 tháng trước
                foreach (ctTonKho item in new bTonKho().layDanhSachTon())
                {
                    kq += "                         <tr>";
                    kq += "                             <td>" + index.ToString() + "</td>";
                    kq += "                             <td>" + xulyDuLieu.traVeKyTuGoc(item.nguyenLieu.tenNguyenLieu) + "</td>";
                    kq += "                             <td></td>";
                    kq += "                             <td>" + xulyDuLieu.traVeKyTuGoc(item.nguyenLieu.donViPhaChe) + "</td>";
                    kq += "                             <td></td>";
                    kq += "                             <td></td>";
                    kq += "                         </tr>";
                    index++;
                }
                kq += "                     </tbody>";
                kq += "                 </table>";
                kq += "             </div>";
                kq += "         </div>";
                kq += "         <br>";
                kq += "         <div class=\"row\">";
                taiKhoan tkLogin = (taiKhoan)Session["login"];
                kq += "             <div class=\"pull-right\"><b>Người kiểm</b><br /><br /><br /><b>" + xulyDuLieu.traVeKyTuGoc(tkLogin.thanhVien.hoTV) + " " + xulyDuLieu.traVeKyTuGoc(tkLogin.thanhVien.tenTV) + "</b>";
                kq += "             </div>";
                kq += "         </div>";
                xulyChung.ghiNhatKyDtb(5, "Nguyên liệu tồn kho ngày \"" + DateTime.Now.ToString() + " \"");
            }
            catch (Exception ex)
            {
                xulyFile.ghiLoi("Class: TonKhoController - Function: AjaxLayDanhSachNguyenLieuCanKiem", ex.Message);
            }
            return kq;
        }

        /// <summary>
        /// Hàm tạo một hàng cho bảng danh sách các nguyên liệu kiểm kho
        /// </summary>
        /// <param name="item">Object chứa thông tin nguyên liệu kiểm kho</param>
        /// <param name="loaiDS">Loại bảng cần lấy các thuộc tính <para/> 1: Lấy danh sách nguyên liệu chưa kiểm kê - 2: Lấy danh sách nguyên liệu đã kiểm kê</param>
        /// <returns></returns>
        private string taoDongNguyenLieuKiemKho(ctTonKho item, int loaiDS)
        {
            string kq = "";
            kq += "<tr role=\"row\" class=\"odd\">";
            kq += "    <td><b>" + xulyDuLieu.traVeKyTuGoc(item.nguyenLieu.tenNguyenLieu) + "</b></td>";
            kq += "    <td>" + xulyDuLieu.doiVND(item.donGia) + "</td>";
            kq += "    <td>   <input  min=\"0\" class=\"form-control\" id=\"txtSoLuongThucTe" + item.maNguyenLieu.ToString() + "\" style=\"width:100%\"></td>";
            kq += "    <td>" + xulyDuLieu.traVeKyTuGoc(item.nguyenLieu.donViPhaChe) + "</td>";
            kq += "    <td>   <input type=\"text\" min=\"0\" class=\"form-control\" id=\"txtNguyenNhan" + item.maNguyenLieu.ToString() + "\" placeholder=\"Nguyên nhân hao hụt\" style=\"width:100%\"></td>";
            kq += "    <td>      <button type=\"button\" manl=\"" + item.maNguyenLieu.ToString() + "\" class=\"btnKiemHang btn btn-info  waves-effect\">Nhập </button>  </td>";
            kq += "</tr>";
            return kq;
        }
        /// <summary>
        /// Hàm tạo dữ liệu cho bảng danh sách nguyên liệu cần kiểm kho
        /// Sự kiện xảy ra khi người dùng click vào tab "Nhập số liệu"
        /// </summary>
        /// <returns>Trả về chuỗi html danh sách các hàng dữ liệu nguyên liệu cho bảng</returns>
        public string AjaxLayNguyenLieuKiemKho()
        {
            string kq = "";
            try
            {
                cartKiemKho cartTruoc = (cartKiemKho)Session["truocKiemKho"];
                foreach (ctTonKho item in cartTruoc.Info.Values)
                {
                    kq += "<tr role=\"row\" class=\"odd\">";
                    kq += "    <td><b>" + xulyDuLieu.traVeKyTuGoc(item.nguyenLieu.tenNguyenLieu) + "</b></td>";
                    kq += "    <td>" + xulyDuLieu.doiVND(item.donGia)+ "</td>";
                    kq += "    <td>   <input type=\"number\" min=\"0\" class=\"form-control\" id=\"txtSoLuongThucTe" + item.maNguyenLieu.ToString() + "\" style=\"width:100%\"></td>";
                    kq += "    <td>" + xulyDuLieu.traVeKyTuGoc(item.nguyenLieu.donViPhaChe) + "</td>";
                    kq += "    <td>   <input type=\"text\" min=\"0\" class=\"form-control\" id=\"txtNguyenNhan" + item.maNguyenLieu.ToString() + "\" placeholder=\"Nguyên nhân hao hụt\" style=\"width:100%\"></td>";
                    kq += "    <td>      <button type=\"button\" manl=\"" + item.maNguyenLieu.ToString() + "\" class=\"btnKiemHang btn btn-info  waves-effect\">Nhập </button>  </td>";
                    kq += "</tr>";
                }
            }
            catch (Exception ex)
            {
                xulyFile.ghiLoi("Class: TonKhoController - Function: AjaxLayNguyenLieuKiemKho", ex.Message);
            }
            return kq;
        }


        /// <summary>
        /// Hàm xử lý sự kiện click vào nút "Nhập" để ghi nhận số lượng tồn kho thực tế
        /// </summary>
        /// <param name="param">Tham số của nguyên liệu đang kiểm kê <para/> param có dạng: maNguyenLieu|soLuongThucTe|nguyenNhanHaoHut</param>
        /// <returns>Chuổi html danh sách các nguyên liệu chưa kiểm kho</returns>
        public string AjaxSuKienKiemHang(string param)
        {
            string kq = "";
            try
            {
                cartKiemKho cartTruoc = (cartKiemKho)Session["truocKiemKho"];
                cartKiemKho cartSau = (cartKiemKho)Session["sauKiemKho"];
                if (param.Split('|').Count() == 3) //---Nếu tham số đúng định dạng
                {
                    //----Xử lý tham số
                    int maNguyenLieu = xulyDuLieu.doiChuoiSangInteger(param.Split('|')[0]);
                    int soLuongThucTe = xulyDuLieu.doiChuoiSangInteger(param.Split('|')[1]);
                    string nguyenNhanHaoHut = xulyDuLieu.xulyKyTuHTML(param.Split('|')[2]);
                    //-------Lấy thông tin nguyên liệu đang kiểm kê
                    ctTonKho itemDangKiem = cartTruoc.getInfo(maNguyenLieu);
                    //----------Gán dữ liệu của nguyên liệu đang kiểm vào đã kiểm
                    ctTonKho itemDaKiem = new ctTonKho();

                    //---Gán dữ liệu cho object mới
                    itemDaKiem.soLuongThucTe = soLuongThucTe;
                    itemDaKiem.nguyenNhanHaoHut = nguyenNhanHaoHut;
                    this.ganDuLieuTuItemCuSangItemMoi(itemDangKiem, itemDaKiem);

                    //-----Thêm nguyên liệu vào cart đã kiểm
                    cartSau.addCart(itemDaKiem);
                    Session["sauKiemKho"] = cartSau;
                    //----------Xóa nguyên liệu trong cartTruocKiemKho
                    cartTruoc.removeItem(maNguyenLieu);
                    Session["truocKiemKho"] = cartTruoc;
                }
                kq = this.AjaxLayNguyenLieuKiemKho();
            }
            catch (Exception ex)
            {
                xulyFile.ghiLoi("Class: TonKhoController - Function: AjaxSuKienKiemHang", ex.Message);
            }
            return kq;
        }

        /// <summary>
        /// Hàm chuyển đổi dữ liệu giữa nguyên liệu chưa kiểm sang đã kiểm
        /// </summary>
        /// <param name="ctCu">Object tồn kho cũ cần đổi</param>
        /// <param name="ctMoi">Object tồn kho mới cần đổi</param>
        /// <param name="param">Chuỗi dữ liệu lấy từ giao diện</param>
        private void ganDuLieuTuItemCuSangItemMoi(ctTonKho oldItem, ctTonKho newItem)
        {
            double soLuongLyThuyet = (double)oldItem.soLuongCuoiKyLyThuyet;
            newItem.maNguyenLieu = oldItem.maNguyenLieu;
            newItem.donGia = oldItem.donGia;
            newItem.nguyenLieu = oldItem.nguyenLieu;
            newItem.soLuongCuoiKyLyThuyet = soLuongLyThuyet;
            newItem.soLuongDauKy = oldItem.soLuongDauKy;
            //------Tính tỷ lệ hao hụt
            double haoHut = (double)(soLuongLyThuyet - newItem.soLuongThucTe);
            newItem.tyLeHaoHut = (double)(haoHut * 100 / soLuongLyThuyet);
        }

        /// <summary>
        /// Hàm thực hiện lấy danh sách các nguyên liệu đã kiểm kê và hiện lên giao diện
        /// Xảy ra khi người dùng click vào tab Đối chiếu
        /// </summary>
        /// <returns>Chuỗi html các hàng cho bảng</returns>
        public string AjaxDoiChieuSoLuong()
        {
            string kq = "";
            try
            {
                cartKiemKho cartSau = (cartKiemKho)Session["sauKiemKho"];
                foreach (ctTonKho item in cartSau.Info.Values)
                {
                    string donViTinh = xulyDuLieu.traVeKyTuGoc(item.nguyenLieu.donViPhaChe);
                    kq += "<tr role=\"row\" class=\"odd\">";
                    kq += "    <td><b>" + xulyDuLieu.traVeKyTuGoc(item.nguyenLieu.tenNguyenLieu) + "</b></td>";
                    kq += "    <td>" + xulyDuLieu.doiVND(item.donGia) + "</td>";
                    kq += "    <td>" + item.soLuongDauKy.ToString() + " (" + donViTinh + ")</td>";
                    kq += "    <td>" + item.soLuongCuoiKyLyThuyet.ToString() + " (" + donViTinh + ")</td>";
                    kq += "    <td><b>" + item.soLuongThucTe.ToString() + " (" + donViTinh + ")</b></td>";
                    kq += "    <td class=\"col-red\">" + Math.Round((double)item.tyLeHaoHut, 3).ToString() + " %</td>";
                    kq += "    <td><i>" + xulyDuLieu.traVeKyTuGoc(item.nguyenNhanHaoHut) + "</i></td>";
                    kq += "    <td><button type=\"button\" manl=\"" + item.maNguyenLieu.ToString() + "\" class=\"btnKiemLai btn btn-warning  waves-effect\">Kiểm lại</button>";
                    kq += "    </td>";
                    kq += "</tr>";
                }
            }
            catch (Exception ex)
            {
                xulyFile.ghiLoi("Class: TonKhoController - Function: AjaxDoiChieuSoLuong", ex.Message);
            }
            return kq;
        }

        /// <summary>
        /// Hàm thực hiện cập nhật lại nguyên liệu sang trạng thái chưa kiểm kê
        /// Xảy ra khi người dùng click vào nút "Kiểm lại" trên danh sách
        /// </summary>
        /// <param name="param">Tham số chứa mã nguyên liệu cần kiểm lại</param>
        /// <returns>Chuỗi html tạo dữ liệu cho bảng nguyên liệu đã kiểm</returns>
        public string AjaxSuKienKiemLai(int param)
        {
            try
            {
                cartKiemKho cartTruoc = (cartKiemKho)Session["truocKiemKho"];
                cartKiemKho cartSau = (cartKiemKho)Session["sauKiemKho"];
                //-------Lấy thông tin nguyên liệu cần kiểm lại
                ctTonKho itemDaKiem = cartSau.getInfo(param);
                if (itemDaKiem != null)
                {
                    //-------Khỡi tạo object chưa kiểm kê và chuyển dữ liệu từ đã kiểm sang chưa kiểm
                    ctTonKho itemChuaKiem = new ctTonKho();
                    itemChuaKiem.soLuongThucTe = 0;
                    itemChuaKiem.nguyenNhanHaoHut = "";
                    this.ganDuLieuTuItemCuSangItemMoi(itemDaKiem, itemChuaKiem);

                    //-------Thêm nguyên liệu vào giỏ chưa kiểm
                    cartTruoc.addCart(itemChuaKiem);
                    Session["truocKiemKho"] = cartTruoc;

                    //------Xóa nguyên liệu từ cart đã kiểm
                    cartSau.removeItem(param);
                    Session["sauKiemKho"] = cartSau;
                }
            }
            catch (Exception ex)
            {
                xulyFile.ghiLoi("Class: TonKhoController - Function: AjaxSuKienKiemLai", ex.Message);
            }
            return AjaxDoiChieuSoLuong();
        }
        #endregion

        #region DANH MỤC TỒN KHO

        /// <summary>
        /// Hàm tạo giao diện danh sách tất cả lịch sử kiểm kho
        /// </summary>
        /// <returns></returns>
        public ActionResult tkho_TableTonKho(int? page)
        {
            if (xulyChung.duocTruyCap(idOfPage))
            {
                string html = "";
                try
                {
                    int trangHienHanh = (page ?? 1);
                    qlCaPheEntities db = new qlCaPheEntities();
                    int soPhanTu = db.TonKhoes.Count();
                    ViewBag.PhanTrang = createHTML.taoPhanTrang(soPhanTu, createHTML.pageSize, trangHienHanh, "/TonKho/tkho_TableTonKho"); //------cấu hình phân trang
                    foreach (TonKho tonKho in db.TonKhoes.ToList().OrderByDescending(c => c.ngayKiem).Skip((trangHienHanh - 1) * createHTML.pageSize).Take(createHTML.pageSize))
                    {
                        html += "<tr role=\"row\" class=\"odd\">";
                        html += "    <td>" + tonKho.maSoKy.ToString() + "</td>";
                        html += "    <td>" + xulyDuLieu.traVeKyTuGoc(tonKho.taiKhoan.thanhVien.hoTV) + " " + xulyDuLieu.traVeKyTuGoc(tonKho.taiKhoan.thanhVien.tenTV) + "</td>";
                        html += "    <td>" + tonKho.ngayKiem.ToString() + "</td>";
                        html += "    <td>" + xulyDuLieu.doiVND(tonKho.tongTien) + "</td>";
                        html += "    <td><a task=\"" + xulyChung.taoUrlCoTruyenThamSo("TonKho/tkho_TableCtTonKho", tonKho.maSoKy.ToString()) + "\"  class=\"guiRequest btn btn-info waves-effect\">Xem chi tiết </a></td>";
                        html += "</tr>";
                    }
                    xulyChung.ghiNhatKyDtb(1, "Danh mục lịch sử kiểm kho");
                }
                catch (Exception ex)
                {
                    xulyFile.ghiLoi("Class: TonKhoController - Function: tkho_TableTonKho", ex.Message);
                }
                ViewBag.TableData = html;
            }
            return View();
        }


        /// <summary>
        /// Hàm tạo danh sách nguyên liệu đã kiểm kể trong 1 lịch sử kiểm kê tồn kho
        /// </summary>
        /// <returns></returns>
        public ActionResult tkho_TableCtTonKho(int? page)
        {
            if (xulyChung.duocTruyCap(idOfPage))
            {
                string html = "";
                try
                {
                    int trangHienHanh = (page ?? 1);
                    string urlAction = (string)Session["urlAction"]; //urlAction có dạng: page=TonKho/tkho_TableCtTonKho|request=maSoKy
                    //-----Xử lý request trong session
                    urlAction = urlAction.Split('|')[1];  //urlAction có dạng: request=maSoKy
                    urlAction = urlAction.Replace("request=", ""); //urlAction có dạng: maSoKy
                    if (urlAction.Length > 0)
                    {
                        int maSoKy = xulyDuLieu.doiChuoiSangInteger(urlAction);

                        qlCaPheEntities db = new qlCaPheEntities();
                        int soPhanTu = db.ctTonKhoes.Where(c => c.maSoKy == maSoKy).Count();
                        ViewBag.PhanTrang = createHTML.taoPhanTrang(soPhanTu, createHTML.pageSize, trangHienHanh, "/TonKho/tkho_TableCtTonKho"); //------cấu hình phân trang
                        foreach (ctTonKho item in db.ctTonKhoes.Where(c => c.maSoKy == maSoKy).ToList().Skip((trangHienHanh - 1) * createHTML.pageSize).Take(createHTML.pageSize))
                        {
                            html += "<tr role=\"row\" class=\"odd\">";
                            html += "  <td><b>" + xulyDuLieu.traVeKyTuGoc(item.nguyenLieu.tenNguyenLieu) + "</b></td>";
                            html += "  <td>" + xulyDuLieu.doiVND(item.donGia) + "</td>";
                            html += "  <td>" + item.soLuongDauKy.ToString() + " (" + xulyDuLieu.traVeKyTuGoc(item.nguyenLieu.donViPhaChe) + ")</td>";
                            html += "  <td>" + item.soLuongCuoiKyLyThuyet.ToString() + " (" + xulyDuLieu.traVeKyTuGoc(item.nguyenLieu.donViPhaChe) + ")</td>";
                            html += "  <td><b>" + item.soLuongThucTe.ToString() + " (" + xulyDuLieu.traVeKyTuGoc(item.nguyenLieu.donViPhaChe) + ")</b></td>";
                            html += "   <td>" + item.tyLeHaoHut.ToString() + "</td>";
                            html += "   <td>" + xulyDuLieu.traVeKyTuGoc(item.nguyenNhanHaoHut) + "</td>";
                            html += "</tr>";
                        }
                        ViewBag.MaSoKy = maSoKy.ToString();
                        xulyChung.ghiNhatKyDtb(1, "Chi tiết kiểm kho kỳ \" " + maSoKy.ToString() + " \"");
                    }
                    else throw new Exception("không nhận được tham số");
                }
                catch (Exception ex)
                {
                    xulyFile.ghiLoi("Class: TonKhoController - Function: tkho_TableCtTonKho", ex.Message);
                    return RedirectToAction("PageNotFound", "Home");
                }
                ViewBag.TableData = html;
            }
            return View();
        }

        /// <summary>
        /// Hàm tạo giao diện danh sách nguyên liệu còn tồn trong kho theo thời gian hiện tại
        /// </summary>
        /// <returns></returns>
        public ActionResult tkho_TonThucTe()
        {
            if (xulyChung.duocTruyCap(idOfPage))
            {
                string htmlTable = "";
                try
                {
                    int index = 1;
                    //-----------Lấy danh sách nguyên liệu tồn kho trong 1 tháng 
                    List<ctTonKho> listTon = new bTonKho().layDanhSachTon();
                    foreach (ctTonKho item in listTon)
                    {
                        string donViTinh = xulyDuLieu.traVeKyTuGoc(item.nguyenLieu.donViPhaChe);
                        htmlTable += "<tr role=\"row\" class=\"odd\">";
                        htmlTable += "<td>" + index.ToString() +"</td>";
                        htmlTable += "    <td><b>" + xulyDuLieu.traVeKyTuGoc(item.nguyenLieu.tenNguyenLieu) + "</b></td>";
                        htmlTable += "    <td>" + xulyDuLieu.doiVND(item.donGia)+ "</td>";
                        htmlTable += "    <td class=\"col-green\"><b>" + item.soLuongDauKy.ToString() + " (" + donViTinh + ")</b></td>";
                        htmlTable += "    <td class=\"col-blue\"><b>" + item.soLuongCuoiKyLyThuyet.ToString() + " (" + donViTinh + ")</b></td>";
                        string soLieuHaoHut="";
                        if (item.tyLeHaoHut > 0) //-----Thêm icon phù hợp với số liệu đã thay đổi
                            soLieuHaoHut = "<i class=\"material-icons\">arrow_drop_down</i>" + item.tyLeHaoHut.ToString();
                        else if (item.tyLeHaoHut < 0)
                            soLieuHaoHut = "<i class=\"material-icons\">arrow_drop_up</i>" + (item.tyLeHaoHut * (-1)).ToString();
                        else
                            soLieuHaoHut = item.tyLeHaoHut.ToString();
                        htmlTable += "    <td class=\"col-red\"><b>" + soLieuHaoHut + " (" + donViTinh + ")</b></td>";
                        htmlTable += "</tr>";
                        index++;
                    }
                    xulyChung.ghiNhatKyDtb(1, "Danh mục tồn kho thực tế");
                }
                catch (Exception ex)
                {
                    xulyFile.ghiLoi("Class: TonKhoController - Function: tkho_TonThucTe", ex.Message);
                    return RedirectToAction("PageNotFound", "Home");
                }
                ViewBag.TableData = htmlTable;
            }
            return View();
        }

        #endregion
    }
}