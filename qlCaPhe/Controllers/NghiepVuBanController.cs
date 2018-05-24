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
    public class NghiepVuBanController : Controller
    {
        private string idOfPage = "501";
        /// <summary>
        /// Hàm tạo giao diện danh sách bàn tại quán.
        /// Người phục vụ sẽ vào đây để tiếp nhận và cập nhật thông tin 
        /// </summary>
        /// <returns></returns>
        public ActionResult nvb_QuanLyDatBanTaiQuan()
        {
            if (xulyChung.duocCapNhat(idOfPage, "7"))
            {
                try
                {
                    qlCaPheEntities db = new qlCaPheEntities();
                    this.taoDuLieuChoCbbKhuVuc(db);
                    this.resetData();
                }
                catch (Exception ex)
                {
                    xulyFile.ghiLoi("Class: NghiepVuBanController - Fucntion: nvb_QuanLyDatBanTaiQuan", ex.Message);
                }
            }
            return View();
        }
        /// <summary>
        /// Hàm tạo dữ liệu cho combobox KHU VỰC
        /// </summary>
        /// <param name="db"></param>
        private void taoDuLieuChoCbbKhuVuc(qlCaPheEntities db)
        {
            string htmlCbb = "";
            foreach (khuVuc kv in db.khuVucs.ToList().OrderBy(kv => kv.tenKhuVuc))
                htmlCbb += "<option value=\"" + kv.maKhuVuc.ToString() + "\">" + xulyDuLieu.traVeKyTuGoc(kv.tenKhuVuc) + "</option>";
            ViewBag.cbbKhuVuc = htmlCbb;
        }


        #region NHÓM HÀM LIỆT KÊ DANH SÁCH BÀN

        /// <summary>
        /// Hàm thực hiện lấy danh sách BÀN khi người dùng click CHỌN vào KHU VỰC hoặc TRẠNG THÁI BÀN
        /// </summary>
        /// <param name="maKV">Mã khu vực cần lấy</param>
        /// <param name="trangThai">Trạng thái bàn cần lấy: -1: Bàn trống - 0: Bàn đã tiếp nhận - 1:- Đã gọi món - 3: Đã thanh toán</param>
        /// <returns>Html tag danh sách tất cả bản theo điều kiện tìm kiếm</returns>
        public string AjaxLoadDanhSachBan(string param)
        {
            string html = "";
            try
            {
                int maKV = xulyDuLieu.doiChuoiSangInteger(param.Split('|')[0]);
                int trangThai = xulyDuLieu.doiChuoiSangInteger(param.Split('|')[1]);
                qlCaPheEntities db = new qlCaPheEntities();
                //-----Nếu lựa chọn liệt kê ở trạng thái bàn trống
                if (trangThai < 0)
                    //------Lấy danh sách tất cả các bàn còn trống -- BÀN KHÔNG TỒN TẠI TRONG HÓA ĐƠN TẠM
                    html += this.layDanhSachBanTrong(db, maKV);
                else //--- Ngược lại lấy danh sách bàn đã có trong hóa đơn tạm. Bàn đã có người ngồi.
                    html += this.layDanhSachBanDaCoKhach(db, maKV, trangThai);
            }
            catch (Exception ex)
            {
                xulyFile.ghiLoi("Class: NghiepVuBanController - Fucntion: AjaxLoadDanhSachBanTheoKhuVuc", ex.Message);
            }
            return html;
        }
        /// <summary>
        /// Hàm lấy danh sách bàn không tồn tại trong hóa đơn tạm -  BÀN TRỐNG - CHƯA AI NGỒI
        /// </summary>
        /// <param name="db"></param>
        /// <param name="maKV">Mã khu vực cần lấy bàn trống</param>
        /// <returns>Chuỗi html danh sách bàn</returns>
        private string layDanhSachBanTrong(qlCaPheEntities db, int maKV)
        {
            string html = "";
            //-------Lặp qua danh sách bàn KHÔNG có trong hóa đơn tạm. Với trạng thái BÀN ĐƯỢC PHÉP SỬ DỤNG 
            foreach (BanChoNgoi b in db.BanChoNgois.Where(b => b.maKhuVuc == maKV && b.trangThai == 1 && !db.hoaDonTams.Any(hd => hd.maBan == b.maBan)))
                html += this.htmlItemBan(b, -1);//--Hiện danh sách cho bàn trống. Có chức năng tiếp nhận
            return html;
        }
        /// <summary>
        /// Hàm lấy danh sách bàn đã có khách ngồi - ĐANG ĐƯỢC SỬ DỤNG TRONG QUÁN
        /// </summary>
        /// <param name="db"></param>
        /// <param name="maKV">Khu vực cần lấy danh sách bàn</param>
        /// <param name="trangThai">Trạng thái cần lấy</param>
        /// <returns></returns>
        private string layDanhSachBanDaCoKhach(qlCaPheEntities db, int maKV, int trangThai)
        {
            string html = "";
            //------Lặp qua từng hóa đơn tạm để lấy thông tin bàn trong hóa đơn
            foreach (hoaDonTam hd in db.hoaDonTams.ToList().Where(t => t.trangThaiHoadon == trangThai && t.BanChoNgoi.maKhuVuc == maKV))
                html += this.htmlItemBan(hd.BanChoNgoi, trangThai);
            return html;
        }
        /// <summary>
        /// Hàm tạo html cho mỗi bàn đang được duyệt 
        /// </summary>
        /// <param name="b">Đối tượng bàn đang duyệt để gán dữ liệu</param>
        /// <param name="trangThai">Dựa vào trạng thái cần lấy danh sách để gán dữ liệu cho title và chức năng có trên item</param>
        /// <returns>html cho mỗi item bàn</returns>
        private string htmlItemBan(BanChoNgoi b, int trangThai)
        {
            string html = "";
            html += "<div class=\"col-lg-4 col-md-4 col-sm-6 col-xs-12\">";
            html += "   <div class=\"card\">";
            html += this.taoHeaderItemBan(b, trangThai);//---Thiết lập title cho bàn trống (Màu sắc)
            //-----------Body cho cart item bàn
            html += "       <div class=\"body\">";
            html += "          <img  src=\"" + xulyDuLieu.chuyenByteHinhThanhSrcImage(b.hinhAnh) + "\" width=\"100%;\" height=\"auto;\" />";
            html += "       </div>";
            html += "       <div class=\"footer\">";
            html += this.listButtonFunction(trangThai, b.maBan);
            html += "       </div>";
            html += "   </div>";
            html += "</div>";
            return html;
        }
        /// <summary>
        /// Hàm tạo header cho item cart bàn
        /// <param name="b">Object bàn để gán các thuộc tính lên header</param>
        /// <param name="trangThai">-1: Danh sách bàn trống - 0:Trạng thái bàn vừa nhận (Khách vừa vào và chưa gọi món) -  1:Trạng thái bàn đã order -- 3:Trạng thái bàn đang chờ dọn dẹp</param>
        /// </summary>
        /// <returns>Html cho div title để xác định màu</returns>
        private string taoHeaderItemBan(BanChoNgoi b, int trangThai)
        {
            string html = "<div class=\"header ";
            switch (trangThai) //-------Thiết lập màu header phụ thuộc vào trạng thái của bàn
            {
                case -1: //---Danh sách bàn trống. Màu XÁM
                    html += " bg-grey"; break;
                case 0://Trạng thái bàn vừa nhận (Khách vừa vào và chưa gọi món) . Màu XANH LÁ MẠ
                    html += " bg-light-green "; break;
                case 1: //Trạng thái bàn khách đã gọi đồ uống gọi món. Màu ĐỎ
                    html += "bg-red "; break;
                case 3://Trạng thái bàn đang chờ trả bàn reset lại. Màu TÍM
                    html += " bg-purple "; break;
                default: break;
            }
            html += "\" >"; //Đóng starttag header
            html += "           <h2>" + xulyDuLieu.traVeKyTuGoc(b.tenBan) + " - Sức chứa: " + b.sucChua.ToString() + " chỗ";
            //------------------------------Gán thông tin GIỚI THIỆU BÀN lên giao diện. Thực hiện CẮT CHUỔI đối với bàn có thông tin giới thiệu LỚN HƠN 38 ký tự
            html += "               <small>" + xulyDuLieu.traVeKyTuGoc(xulyDuLieu.xulyCatChuoi(b.gioiThieu, 38)) + "</small>";
            html += "           </h2>";
            if (trangThai == 0 || trangThai == 1) //------Nếu trạng thái bàn CÓ KHÁCH NGỒI CHƯA ORDER hoặc bàn ĐÃ ORDER thì thêm chức năng ĐỔI CHỔ
            {
                html += "             <ul class=\"header-dropdown m-r--5\">";
                html += "                 <li class=\"dropdown\">";
                html += "                     <a class=\"dropdown-toggle\" data-toggle=\"dropdown\" role=\"button\" aria-haspopup=\"true\" aria-expanded=\"false\">";
                html += "                     <i class=\"material-icons\">more_vert</i>";
                html += "                     </a>";
                html += "                     <ul class=\"dropdown-menu pull-right\">";
                html += "                       <li><a  data-toggle=\"modal\" data-target=\"#modalDoiBan\"  maban=\"" + b.maBan.ToString() + "\" class=\"doiCho col-cyan waves-effect\"><i class=\"bg-red material-icons\">update</i>Đổi chổ</a></li>";
                html += "                     </ul>";
                html += "                 </li>";
                html += "             </ul>";
            }
            html += "</div>"; //-------Đóng div header
            return html;
        }
        /// <summary>
        /// Hàm tạo danh sách các NÚT CHỨC NĂNG cho bàn gán vào footer của mỗi item bàn
        /// </summary>
        /// <param name="trangThai">Dựa vào Trạng thái để gán các chức năng xử lý với bàn</param>
        /// <param name="maBan">Mã bàn để xác định bàn cần thực hiện chức năng</param>
        /// <returns></returns>
        private string listButtonFunction(int trangThai, int maBan)
        {
            string html = "<div class=\"text-center\">";
            switch (trangThai)
            {
                case -1: //---Danh sách bàn trống - Có chức năng tiếp nhận bàn
                    html += "<a maban=\"" + maBan.ToString() + "\" class=\"tiepNhan btn bg-green waves-effect \"><i class=\"material-icons\">golf_course</i>Tiếp nhận</a>";
                    break;
                case 0://Trạng thái bàn vừa nhận (Khách vừa vào và chưa gọi món) --Có chức năng gọi món và đổi bàn
                    html += "<a data-toggle=\"modal\" data-target=\"#modalChonDoUong\" maban=\"" + maBan.ToString() + "\" class=\"goiMon btn bg-red waves-effect\"><i class=\"material-icons\">reorder</i>Gọi món</a>&nbsp;&nbsp;&nbsp;";
                    html += "<a maban=\"" + maBan.ToString() + "\" class=\"resetBan btn bg-purple waves-effect\"><i class=\"material-icons\">clear_all</i>Dọn bàn</a>";
                    break;
                case 1: //Trạng thái bàn khách ĐÃ GỌI ĐỒ UỐNG-- có chức năng GỌI THÊM món hoặc THANH TOÁN
                    html += "<a data-toggle=\"modal\" data-target=\"#modalChonDoUong\" maban=\"" + maBan.ToString() + "\" class=\"goiThem btn bg-red waves-effect\"><i class=\"material-icons\">reorder</i>Gọi thêm</a>&nbsp;&nbsp;&nbsp;";
                    html += "<a maban=\"" + maBan.ToString() + "\" class=\"thanhToan btn bg-orange waves-effect \"><i class=\"material-icons\">payment</i>Thanh toán</a>";
                    break;
                case 3://Trạng thái bàn đang chờ trả bàn reset lại
                    html += "<a maban=\"" + maBan.ToString() + "\" class=\"resetBan btn bg-purple waves-effect\"><i class=\"material-icons\">clear_all</i>Dọn bàn</a>";
                    break;
                default: break;
            }
            html += "</div>";
            return html;
        }

        #endregion

        #region NHÓM HÀM HIỆN DANH SÁCH SẢN PHẨM TRÊN MODAL

        /// <summary>
        /// Hàm thực hiện tạo giao diện danh sách sản phẩm khi người dùng click chọn 1 loại trên cbb
        /// </summary>
        /// <param name="param">Mã loại sản phẩm cần tìm kiếm</param>
        /// <returns>Chuỗi html cho list sản phẩm</returns>
        public string AjaxLaySanPhamTheoLoai()
        {
            string html = "";
            try
            {
                html += "<div class=\"panel-group\" id=\"accordion_1\" role=\"tablist\" aria-multiselectable=\"true\">";
                html += taoDanhSachLoaiSanPham();
                html += "</div>";
            }
            catch (Exception ex)
            {
                xulyFile.ghiLoi("Class: NghiepVuBanController - Fucntion: AjaxLaySanPhamTheoLoai", ex.Message);
            }
            return html;
        }
        /// <summary>
        /// Hàm tạo collape cho danh sách loại sản phẩm và sản phẩm hiện lên modal
        /// </summary>
        /// <returns></returns>
        private string taoDanhSachLoaiSanPham()
        {
            qlCaPheEntities db = new qlCaPheEntities();
            string html = "";
            int indexLoai = 0;
            LapLaiDanhSachLoai://Vị trí cho lần lặp tiếp theo
            loaiSanPham loai= new loaiSanPham();
            //---------Lặp qua loại sản phẩm có tồn tại sản phẩm
            foreach (loaiSanPham loaiDuyet in db.loaiSanPhams.ToList().Where(l => l.sanPhams.Count > 0).Skip(indexLoai))
            {
                loai = loaiDuyet;
                indexLoai++;
                break;
            }
            if (loai.maLoai > 0)
            {
                //----------Tạo collapse danh mục LOẠI SẢN PHẨM
                html += "    <div class=\"panel panel-primary\">";
                html += "        <div class=\"panel-heading\" role=\"tab\" id=\"headingOne_" + loai.maLoai.ToString() + "\">";
                html += "            <h4 class=\"panel-title\">";
                html += "                <a role=\"button\" data-toggle=\"collapse\" data-parent=\"#accordion_1\" href=\"#" + loai.maLoai.ToString() + "\" aria-controls=\"" + loai.maLoai.ToString() + "\">";
                html += xulyDuLieu.traVeKyTuGoc(loai.tenLoai);
                html += "                </a>";
                html += "            </h4>";
                html += "        </div>";
                html += "        <div id=\"" + loai.maLoai.ToString() + "\" class=\"panel-collapse collapse\" role=\"tabpanel\" aria-labelledby=\"headingOne_1\">";
                html += "            <div class=\"panel-body\">";
                html += this.taoDanhSachSanPhamCuaLoai(loai);
                html += "            </div>";
                html += "        </div>";
                html += "    </div>";
                goto LapLaiDanhSachLoai; //-----Quay lại vòng lặp
            }
            return html;
        }
        /// <summary>
        /// Tạo danh sách sản phẩm của loại
        /// </summary>
        /// <param name="loai"></param>
        /// <returns></returns>
        private string taoDanhSachSanPhamCuaLoai(loaiSanPham loai)
        {
            string html = "";
            //--------Lặp qua những sản phẩm còn bán
            foreach (sanPham sp in loai.sanPhams.Where(s => s.trangThai == 1).ToList())
                html += this.itemSanPhamModal(sp, new bSanPham().kiemTraSanPhamKhaThi(sp));
            return html;
        }

        /// <summary>
        /// Ham thực hiện lấy danh sách sản phẩm theo tên sản phẩm cần tìm kiếm
        /// </summary>
        /// <param name="tenSP">Tên sản phẩm cần tìm</param>
        /// <returns></returns>
        public string AjaxTimKiemSanPham(string param)
        {
            string html = "";
            try
            {
                //------------Lấy danh sách sản phẩm theo tên cần tìm kiếm. CHỈ LẤY 6 SẢN PHẨM. ĐỂ TRÁNH TRƯỜNG HỢP CHẾT SERVER
                List<sanPham> list = new qlCaPheEntities().sanPhams.Where(n => n.tenSanPham.StartsWith(param) && n.trangThai == 1).Take(6).ToList();
                if (list.Count() > 0)
                    foreach (sanPham sp in list)
                        if (new bSanPham().kiemTraSanPhamKhaThi(sp))
                            html += this.itemSanPhamModal(sp, true);
                        else
                            html += this.itemSanPhamModal(sp, false);
                else
                    html += "<h4 class=\"col-red\">Sản phẩm không tồn tại trên hệ thống</h4>";
            }
            catch (Exception ex)
            {
                xulyFile.ghiLoi("Class: NghiepVuBanController - Fucntion: AjaxTimKiemSanPham", ex.Message);
            }
            return html;
        }

        /// <summary>
        /// Hàm tạo giao diện cho một item sản phẩm hiển thị trên modal
        /// </summary>
        /// <param name="sp">Sản phẩm cần gán thông tin</param>
        /// <param name="conHang">Sản phẩm này còn hàng không. True: còn hàng, false: hết hàng</param>
        /// <returns></returns>
        private string itemSanPhamModal(sanPham sp, bool conHang)
        {
            string html = "";
            html += "<div class=\"col-sm-4 col-md-4\">";
            html += "    <div class=\"thumbnail\">";
            html += "        <img width=\"100%\" height=\"auto;\" src=\"" + xulyDuLieu.chuyenByteHinhThanhSrcImage(sp.hinhAnh) + "\">";
            if (!conHang) //------Nếu trạng thái hết hàng thì hiện hình ảnh hết hàng và không cho click chọn
                html += "     <div class=\"hetHang\"></div>";
            html += "         <div class=\"caption\">";
            html += "            <h3>" + xulyDuLieu.traVeKyTuGoc(sp.tenSanPham) + "</h3>";
            html += "            <label class=\"font-13 font-bold col-pink\">" + sp.donGia.ToString() + " VNĐ</label>";
            html += "            <button class=\"btn btn-info btnChonSP\" maSP=\"" + sp.maSanPham.ToString() + "\">Chọn</button>";
            html += "         </div>";
            html += "    </div>";
            html += "</div>";
            return html;
        }

        /// <summary>
        /// Hàm thực hiện lấy danh sách sản phẩm và hiển thị lên modal checkout để có thể cập nhật trạng  thái thanh toán
        /// </summary>
        /// <param name="maBan">Mã bàn cần thanh toán</param>
        /// <returns>Chuỗi html danh sach sản phẩm cần thanh tóan</returns>
        public string AjaxHienCheckout(int maBan)
        {
            string html = "";
            try
            {
                qlCaPheEntities db = new qlCaPheEntities();
                html += "<div class=\"body table-responsive\">";
                //-------Hiện bảng danh sách sản phẩm của bàn. CHỈ LẤY DANH SÁCH SẢN PHẨM ĐƯỢC BÁN
                html += this.taoBangDanhSachSanPhamTrenBan(db.ctHoaDonTams.Where(c => c.maBan == maBan && c.trangThaiPhaChe!=4).ToList());
                //--------Hiện tổng tiền hóa đơn
                html += "   <p class=\"font-16 font-bold col-pink\">Tạm tính: <i>" + db.hoaDonTams.SingleOrDefault(h => h.maBan == maBan).tongTien.ToString() + " VNĐ</i></p>";
                html += "</div>";
            }
            catch (Exception ex)
            {
                xulyFile.ghiLoi("Class: NghiepVuBanController - Fucntion: AjaxHienCheckout", ex.Message);
            }
            return html;
        }

        /// <summary>
        /// Hàm tạo bảng danh sách sản phẩm dựa vào list
        /// </summary>
        ///<param name="list">Danh sách sản phẩm để lấy</param>
        /// <returns>chuỗi html table</returns>
        private string taoBangDanhSachSanPhamTrenBan(List<ctHoaDonTam> list)
        {
            string html = "";
            html += "   <table class=\"table table-hover\">";
            html += "       <thead>";
            html += "           <tr>";
            html += "              <th>Tên món</th>";
            html += "               <th style=\"width:20%\">Số lượng</th>";
            html += "               <th>Đơn giá</th>";
            html += "           </tr>";
            html += "       </thead>";
            html += "       <tbody>";
            
            foreach (ctHoaDonTam ct in list)
            {
                html += "           <tr>";
                html += "               <td>";
                html += "                   <img width=\"50px;\" height=\"50px;\" src=\"" + xulyDuLieu.chuyenByteHinhThanhSrcImage(ct.sanPham.hinhAnh) + "\">";
                html += "                   <b>" + xulyDuLieu.traVeKyTuGoc(ct.sanPham.tenSanPham) + "</b>";
                html += "               </td>";
                html += "               <td>" + ct.soLuong.ToString() + "</td>";
                html += "               <td>" + ct.donGia.ToString() + " VNĐ</td>";
                html += "           </tr>";
            }
            html += "       </tbody>";
            html += "   </table>";
            return html;
        }

        #endregion

        #region GIỎ HÀNG
        //------------------CREATE----------

        /// <summary>
        /// Hàm thêm sản phẩm vào cart hoaDonTam trong session 
        /// </summary>
        /// <param name="param">Mã sản phẩm cần add</param>
        /// <returns>Trả về danh sách sản phẩm có trong sesion</returns>
        public string AjaxThemSanPhamVaoGio(int param)
        {
            string htmlListOrder = "";
            if (xulyChung.duocCapNhat(idOfPage, "7"))
            {
                try
                {
                    cartHoaDonTam cart = (cartHoaDonTam)Session["hoaDonTam"];
                    //------Lấy thông tin sản phẩm để thêm vào chi tiết
                    sanPham sp = new qlCaPheEntities().sanPhams.SingleOrDefault(s => s.maSanPham == param);
                    //------Kiểm tra xem sản phẩm có trong cart chưa
                    ctHoaDonTam ct = cart.getItem(param);
                    if (ct == null)
                    {   //-----TẠO MỚI
                        ct = new ctHoaDonTam();
                        ct.maCtTam = param;//----Lấy mã sản phẩm làm key trong trường hợp nếu có sản phẩm trùng thì tăng số lượng
                        ct.maSP = param;
                        ct.donGia = sp.donGia; ct.soLuong = 1;
                        ct.trangThaiPhaChe = 0;//Thiết lập trạng thái vừa tiếp nhận
                        ct.sanPham = sp;
                        cart.addCart(ct);
                    }
                    else //------Nếu như đã có sản phẩm trong cart. TĂNG SỐ LƯỢNG
                    {
                        ct.soLuong++;
                        cart.updateItem(ct);
                    }
                    Session["hoaDonTam"] = cart;
                    htmlListOrder = this.taoBangChiTietTuSession(cart.getList(), cart.tongTienDtb + cart.getTotalAmount());
                }
                catch (Exception ex)
                {
                    xulyFile.ghiLoi("Class: NghiepVuBanController - Fucntion: AjaxThemSanPhamVaoGio", ex.Message);
                    Response.Redirect("http://localhost:54272/Home/h_AccessDenied");
                }
            }
            return htmlListOrder;
        }

        //-----------------READ-------------
        /// <summary>
        /// Hàm thực hiện hiển thị danh sách bảng các món đã chọn TRONG SESSION
        /// </summary>
        /// <returns>Trả về chuỗi html tạo nên bảng và số tổng tiền 
        /// Có dạng: htmlTable|tongTien</returns>
        public string taoBangChiTietTuSession(List<ctHoaDonTam> list, long tongTien)
        {
            string html = "";
            try
            {
                //-------Tạo bản danh sách sản phẩm có trong cart session
                html += this.taoBangDanhSachSanPhamDaOrder(list, "txtSoLuongS", "btnUpdateCart", "btnRemoveCart");
                html += "<div class=\"col-md-9\" id=\"divTongTien\" style=\"display:block\"><p class=\"font-16 font-bold col-pink\">Tạm tính: <i>" + tongTien.ToString() + " VNĐ</p>";
                //html += "   <label for=\"txtTenQuan\">Ghi chú</label>";
                //html += "   <div class=\"form-group\">";
                //html += "       <div class=\"form-line\">";
                //html += "           <input type=\"text\" id=\"txtGhiChu\" class=\"form-control\" placeholder=\"Nhập thêm yêu cầu...\"/>";
                //html += "       </div>    ";
                //html += "   </div>";
                html += "</div>";

            }
            catch (Exception ex)
            {
                xulyFile.ghiLoi("Class: NghiepVuBanController - Fucntion: taoBangChiTietTuSession", ex.Message);
            }
            return html;
        }

        //-----------------UPDATE----------------
        /// <summary>
        /// Hàm thực hiện cập nhật lại số lượng chọn sản phẩm trong session
        /// </summary>
        /// <param name="param">Tham số có dạng: maSP|soLuong</param>
        /// <returns>Chuỗi html danh sách sản phẩm và đơn giá của sản phẩm cập nhật</returns>
        public string AjaxUpdateSoLuongSanPhamTrongGio(string param)
        {
            cartHoaDonTam cart = (cartHoaDonTam)Session["hoaDonTam"];
            if (xulyChung.duocCapNhat(idOfPage, "7"))
            {
                try
                {
                    int maSP = xulyDuLieu.doiChuoiSangInteger(param.Split('|')[0]);
                    int soLuong = xulyDuLieu.doiChuoiSangInteger(param.Split('|')[1]);
                    ctHoaDonTam ct = cart.getItem(maSP);
                    if (ct != null)
                    {
                        ct.soLuong = soLuong;
                        cart.updateItem(ct);
                        Session["hoaDonTam"] = cart;
                    }
                }
                catch (Exception ex)
                {
                    xulyFile.ghiLoi("Class: NghiepVuBanController - Fucntion: AjaxUpdateSoLuongSanPhamTrongGio", ex.Message);
                }
            }
            return this.taoBangChiTietTuSession(cart.getList(), cart.getTotalAmount() + cart.tongTienDtb);
        }

        //--------------DELETE----------------------
        /// <summary>
        /// Hàm thực hiện xóa 1 sản phẩm khỏi giỏ
        /// </summary>
        /// <param name="param">Mã sản phẩm cần xóa</param>
        /// <returns>Chuỗi html danh sách sản phẩm còn lại trong giỏ</returns>
        public string AjaxXoaMotSanPhamKhoiGio(int param)
        {
            string html = "";
            if (xulyChung.duocCapNhat(idOfPage, "7"))
            {
                try
                {
                    cartHoaDonTam cart = (cartHoaDonTam)Session["hoaDonTam"];
                    //------Kiểm tra xem sản phẩm có trong cart chưa
                    ctHoaDonTam ct = cart.getItem(param);
                    if (ct != null)
                    {
                        long donGia = ct.donGia;
                        //----Thực hiện xóa khỏi cart
                        cart.removeItem(ct);
                        //-----Cập nhật lại Session
                        Session["hoaDonTam"] = cart;
                    }
                    html += this.taoBangChiTietTuSession(cart.getList(), cart.tongTienDtb + cart.getTotalAmount());
                }
                catch (Exception ex)
                {
                    xulyFile.ghiLoi("Class: NghiepVuBanController - Fucntion: AjaxThemSanPhamVaoGio", ex.Message);
                }
            }
            return html;
        }

        /// <summary>
        /// Hàm thực hiện xóa tất cả các phần tử trong cart
        /// </summary>
        /// <returns>Chuỗi html rỗng</returns>
        public string AjaxXoaTatCaPhanTuCart()
        {
            if (xulyChung.duocCapNhat(idOfPage, "7"))
            {
                try
                {
                    this.resetData();
                }
                catch (Exception ex)
                {
                    xulyFile.ghiLoi("Class: NghiepVuBanController - Fucntion: AjaxXoaMotSanPhamKhoiGio", ex.Message);
                }
            }
            return "";
        }
        #endregion

        #region NHÓM HÀM CẬP NHẬT VÀO DATABASE
        #region NHÓM HÀM CHUYỂN ĐỔI TRẠNG THÁI CỦA HÓA ĐƠN TẠM

        /// <summary>
        /// Hàm thực hiện thêm mới 1 bàn vào hóa đơn tạm khi có khách đến trong CSDL <para />
        /// TIẾP NHẬN BÀN
        /// </summary>
        /// <param name="param">Chuỗi chứa mã bàn có dạng maBanMoi|maBanCu</param>
        /// <returns>Trả về danh sách bàn của khu vực và của trạng thái</returns>
        public void AjaxTiepNhanBan(string param)
        {

            if (xulyChung.duocCapNhat(idOfPage, "7"))
            {
                try
                {
                    bHoaDonTam bHoaDonTam = new bHoaDonTam();
                    //-------Tiến hành xử lý chuỗi param chia thành 2 tham số maBan mới cần chuyển sang và mã bàn cũ
                    int maBanMoi = xulyDuLieu.doiChuoiSangInteger(param.Split('|')[0]);
                    int maBanCu = xulyDuLieu.doiChuoiSangInteger(param.Split('|')[1]);
                    qlCaPheEntities db = new qlCaPheEntities();
                    //-----Lấy thông tin bàn cũ
                    hoaDonTam hoaDonCu = db.hoaDonTams.SingleOrDefault(h => h.maBan == maBanCu);
                    //-----Nếu chưa có bàn
                    if (hoaDonCu == null)
                        //this.AddHoaDonTamDatabase(db, maBanMoi);
                        bHoaDonTam.themMoiHoaDonTam(db, maBanMoi, ((taiKhoan)Session["login"]).tenDangNhap);
                    else
                        //-----BÀN CŨ ĐANG SỬ DỤNG. cho phép ĐỔI BÀN
                        //    this.UpdateHoaDonTamDatabase(db, hoaDonCu, maBanMoi);
                        bHoaDonTam.capNhatHoaDonTam(db, hoaDonCu, maBanMoi);
                }
                catch (Exception ex)
                {
                    xulyFile.ghiLoi("Class: NghiepVuBanController - Fucntion: AjaxTiepNhanBan", ex.Message);
                }
            }
        }

        //-----------------Cập nhật trạng thái sang 2-------------
        /// <summary>
        /// Hàm thực hiện cập nhật hóa đơn thành trạng thái số 2: Đã TIẾP NHẬN THANH TOÁN
        /// </summary>
        /// <param name="maBan"></param>
        /// <returns></returns>
        public void AjaxCapNhatChoThanhToan(int maBan)
        {
            if (xulyChung.duocCapNhat(idOfPage, "7"))
            {
                try
                {
                    new bNghiepVuBan().tiepNhanThanhToan(maBan);
                }
                catch (Exception ex)
                {
                    xulyFile.ghiLoi("Class: NghiepVuBanController - Fucntion: AjaxCapNhatThanhToan", ex.Message);
                }
            }
        }

        /// <summary>
        /// Hàm xóa dữ liệu bàn trong bảng HoaDonTam trong Database.
        /// Tức là reset lại bàn khi khách rời khỏi
        /// </summary>
        /// <param name="maBan"></param>
        /// <returns>Chuỗi html tạo danh sách bàn trống.</returns>
        public void AjaxResetBan(int maBan)
        {
            if (xulyChung.duocCapNhat(idOfPage, "7"))
            {
                try
                {
                    new bNghiepVuBan().resetBan(maBan);
                }
                catch (Exception ex)
                {
                    xulyFile.ghiLoi("Class: NghiepVuBanController - Fucntion: AjaxResetBan", ex.Message);
                }
            }
        }
        #endregion

        /// <summary>
        /// Hàm ĐỔ DỮ LIỆU từ bảng ctHoaDonTam lên view modal sản phẩm đã order
        /// </summary>
        /// <param name="param">Mã bàn cần lấy danh sách các món đã order</param>
        /// <returns>Trả về html có chuỗi bảng và tổng tiền</returns>
        public string AjaxDoChiTietLenView(int param)
        {
            string html = "";
            try
            {
                qlCaPheEntities db = new qlCaPheEntities();
                this.resetData();//---Reset lại sesion
                hoaDonTam hdTam = db.hoaDonTams.SingleOrDefault(h => h.maBan == param);
                this.luuTongTienVaoSession(hdTam);
                //------Hiện danh sách sản phẩm từ bảng ctHoaDonTam
                html += this.taoBangChiTietTuDatabase(hdTam);
                html += "|" + hdTam.ghiChu;
            }
            catch (Exception ex)
            {
                xulyFile.ghiLoi("Class: NghiepVuBanController - Fucntion: AjaxDoChiTietLenView", ex.Message);
            }
            return html;
        }
        /// <summary>
        /// Hàm tạo bảng dữ liệu từ table ctHoaDonTam trong database theo mã bàn hóa đơn
        /// </summary>
        /// <param name="list"></param>
        /// <returns>Trả về chuỗi html tạo nên bảng và số tổng tiền </returns>
        private string taoBangChiTietTuDatabase(hoaDonTam hoaDon)
        {
            string html = "";
            try
            {
                html += this.taoBangDanhSachSanPhamDaOrder(hoaDon.ctHoaDonTams.ToList(), "txtSoLuong", "btnUpdateDtb", "btnRemoveDtb");
                //--------Lấy tổng tiền trong bảng hoaDonTam có trong session
                cartHoaDonTam tongTien = (cartHoaDonTam)Session["hoaDonTam"];
                //--------Hiện tổng số tiền của hóa đơn
                html += "<div class=\"col-md-9\" id=\"divTongTien\" style=\"display:block\"><p class=\"font-16 font-bold col-pink\">Tạm tính: <i>" + tongTien.tongTienDtb.ToString() + " VNĐ</p></div>";
            }
            catch (Exception ex)
            {
                xulyFile.ghiLoi("Class: NghiepVuBanController - Fucntion: taoBangChiTietTuDatabase", ex.Message);
            }
            return html;
        }

        #region NHÓM HÀM AJAX CẬP NHẬT DỮ LIỆU VÀO DATABASE
        //------------------Cập nhật trạng thái sang 1----------------
        /// <summary>
        /// Hám thực hiện thêm dữ liệu vào bảng chi tiết hóa đơn trong database <para/>
        /// Khi hoàn tất order
        /// </summary>
        /// <param name="param">Chuỗi chứa mã bàn cần cập nhật và ghi chú <para/> VD: 12|GhiChu</param>
        /// <returns></returns>
        public void AjaxThemChiTietHoaDon(string param)
        {
            if (xulyChung.duocCapNhat(idOfPage, "7"))
            {
                try
                {
                    cartHoaDonTam cart = (cartHoaDonTam)Session["hoaDonTam"];
                    if (cart.Item.Count > 0)//Kiểm tra xem có sản phẩm trên session. Nếu có mới cho thêm
                    {
                        int kqLuu = 0;
                        int maBan = xulyDuLieu.doiChuoiSangInteger(param.Split('|')[0]);
                        bNghiepVuBan bNghiepVu = new bNghiepVuBan();
                        //--------cập nhật trạng thái hóa đơn tạm sang đã order
                        long tongTien = cart.tongTienDtb + cart.getTotalAmount();
                        kqLuu = bNghiepVu.capNhatDaOrder(maBan, tongTien, xulyDuLieu.xulyKyTuHTML(param.Split('|')[1]));
                        if(kqLuu>-1) //----Nhận order thành công
                        {
                            int soLuongItem = cart.getList().Count;
                            foreach (ctHoaDonTam ctSession in cart.getList())
                                //---KqLuu tăng lên sau mỗi lần cập nhật thành công
                                kqLuu += bNghiepVu.themChiTietHoaDonTam(maBan, ctSession.maSP, ctSession.donGia, ctSession.soLuong, ctSession.trangThaiPhaChe);
                            //----Nếu kqLuu lớn hơn số lần thêm chi tiết thành công và số lần cập nhật trạng thái
                            if(kqLuu>soLuongItem)
                                this.resetData();//Xóa tất cả dữ liệu trong session
                        }
                    }
                }
                catch (Exception ex)
                {
                    xulyFile.ghiLoi("Class: NghiepVuBanController - Function: AjaxThemChiTietHoaDon", ex.Message);
                }
            }
        }
        /// <summary>
        /// Hàm cập nhật số lượng sản phẩm của 1 chi tiết trong database
        /// </summary>
        /// <param name="param">Chuỗi chứa chi tiết cần cập nhật và số lượng cần cập nhật <para /> Có dạng: maCt|soLuong</param>
        /// <returns></returns>
        public string AjaxUpdateSoLuongSanPhamTrongDatabase(string param)
        {
            string html = "";
            if (xulyChung.duocCapNhat(idOfPage, "7"))
            {
                try
                {
                    qlCaPheEntities db = new qlCaPheEntities();
                    int maCt = xulyDuLieu.doiChuoiSangInteger(param.Split('|')[0]);
                    int soLuong = xulyDuLieu.doiChuoiSangInteger(param.Split('|')[1]);
                    ctHoaDonTam ctSua = db.ctHoaDonTams.SingleOrDefault(c => c.maCtTam == maCt);
                    if (ctSua != null)
                    {
                        //------Cập nhật lại số lượng trong bảng chi tiết
                        ctSua.soLuong = soLuong;
                        db.Entry(ctSua).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                        this.UpdateTongTienHoaDonTamDatabase(db, ctSua.hoaDonTam);
                        this.luuTongTienVaoSession(ctSua.hoaDonTam);
                        html += this.taoBangChiTietTuDatabase(ctSua.hoaDonTam);
                    }
                }
                catch (Exception ex)
                {
                    xulyFile.ghiLoi("Class: NghiepVuBanController - Fucntion: AjaxUpdateSoLuongSanPhamTrongDatabase", ex.Message);
                }
            }
            return html;
        }

        /// <summary>
        /// Hàm thực hiện xóa một chi tiết trong database
        /// </summary>
        /// <param name="param">Mã chi tiết cần xóa</param>
        /// <returns></returns>
        public string AjaxXoaMotChiTietTam(int param)
        {
            string html = "";
            if (xulyChung.duocCapNhat(idOfPage, "7"))
            {
                try
                {
                    qlCaPheEntities db = new qlCaPheEntities();
                    ctHoaDonTam ctTam = db.ctHoaDonTams.SingleOrDefault(c => c.maCtTam == param);
                    if (ctTam != null)
                    {
                        //----Lưu lại hóa đơn tạm để lấy danh sách
                        hoaDonTam hoaDon = ctTam.hoaDonTam;
                        //----Xóa dữ liệu
                        db.ctHoaDonTams.Remove(ctTam);
                        db.SaveChanges();
                        this.UpdateTongTienHoaDonTamDatabase(db, hoaDon);
                        //------Cập nhật tổng tiền
                        this.luuTongTienVaoSession(hoaDon);
                        html += this.taoBangChiTietTuDatabase(hoaDon);
                    }
                }
                catch (Exception ex)
                {
                    xulyFile.ghiLoi("Class: NghiepVuBanController - Fucntion: AjaxXoaMotChiTietTam", ex.Message);
                }
            }
            return html;
        }

        /// <summary>
        /// hàm thực hiện lưu tổng tiền từ bảng ctHoaDonTam vào session
        /// </summary>
        /// <param name="maBan"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        private void luuTongTienVaoSession(hoaDonTam hdTam)
        {
            cartHoaDonTam tongTien = (cartHoaDonTam)Session["hoaDonTam"];
            tongTien.tongTienDtb = (long)hdTam.tongTien;
            Session["hoaDonTam"] = tongTien;
        }
        #endregion

        /// <summary>
        /// Hàm CẬP NHẬT LẠI BÀN CHO HÓA ĐƠN TẠM <para />
        /// Khi ĐỔI BÀN
        /// </summary>
        /// <param name="db"></param>
        /// <param name="hoaDonCu">Object chứa các giá trị của hóa đơn cũ</param>
        /// <param name="maBanMoi">Mã bàn mới cần cập nhật sang</param>
        //private void UpdateHoaDonTamDatabase(qlCaPheEntities db, hoaDonTam hoaDonCu, int maBanMoi)
        //{
        //    //thiết lập lại các thuộc tính của bàn mới
        //    hoaDonTam hoaDonMoi = new hoaDonTam();
        //    hoaDonMoi.ghiChu = hoaDonCu.ghiChu;
        //    hoaDonMoi.ngayLap = hoaDonCu.ngayLap;
        //    hoaDonMoi.nguoiPhucVu = hoaDonCu.nguoiPhucVu;
        //    hoaDonMoi.thoiDiemDen = hoaDonCu.thoiDiemDen;
        //    hoaDonMoi.tongTien = hoaDonCu.tongTien;
        //    hoaDonMoi.trangThaiHoadon = hoaDonCu.trangThaiHoadon;
        //    hoaDonMoi.trangthaiphucVu = hoaDonCu.trangthaiphucVu;
        //    hoaDonMoi.maBan = maBanMoi;
        //    db.hoaDonTams.Add(hoaDonMoi);
        //    db.SaveChanges();
        //    //----Update tất chi tiết từ bàn cũ sang bàn mới
        //    foreach (ctHoaDonTam ct in hoaDonCu.ctHoaDonTams.ToList())
        //    {
        //        ct.maBan = maBanMoi;
        //        db.Entry(ct).State = System.Data.Entity.EntityState.Modified;
        //        db.SaveChanges();
        //    }
        //    //------Dọn bàn cũ
        //    db.hoaDonTams.Remove(hoaDonCu);
        //    db.SaveChanges();
        //}
        /// <summary>
        /// Hàm thêm mới 1 hóa đơn khi tiếp nhận bàn vào Database
        /// </summary>
        /// <param name="db"></param>
        /// <param name="maBanMoi"></param>
        //private void AddHoaDonTamDatabase(qlCaPheEntities db, int maBanMoi)
        //{
        //    //-----Kiểm tra bàn có tồn tại trong bảng tạm không trước khi thêm
        //    hoaDonTam hoaDonAdd = db.hoaDonTams.SingleOrDefault(h => h.maBan == maBanMoi);
        //    //-----Nếu chưa tồn tại thì cho thêm
        //    if (hoaDonAdd == null)
        //    {
        //        hoaDonAdd = new hoaDonTam();
        //        hoaDonAdd.maBan = maBanMoi;
        //        hoaDonAdd.trangThaiHoadon = 0;//-----Thiết lập trạng thái hóa đơn 0 - VỪA VÀO BÀN
        //        hoaDonAdd.trangthaiphucVu = -1;//------Thiết lập trạng thái phục vụ - Trước khi nhận order
        //        hoaDonAdd.thoiDiemDen = DateTime.Now;
        //        hoaDonAdd.nguoiPhucVu = ((taiKhoan)Session["login"]).tenDangNhap;
        //        db.hoaDonTams.Add(hoaDonAdd);
        //        db.SaveChanges();
        //    }
        //}
        /// <summary>
        /// Hàm cập nhật lại tổng tiền của 1 hóa đơn tạm
        /// </summary>
        /// <param name="db"></param>
        /// <param name="hoaDonSua"></param>
        private void UpdateTongTienHoaDonTamDatabase(qlCaPheEntities db, hoaDonTam hoaDonSua)
        {
            hoaDonSua.tongTien = this.layTongTienTuChiTiet(hoaDonSua);
            db.Entry(hoaDonSua).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
        }
        #endregion

        /// <summary>
        /// Hàm thực hiện thiết lập lại SEssion
        /// </summary>
        private void resetData()
        {
            //--Xoá session chứa danh sách sản phẩm trong hóa đơn tạm
            Session.Remove("hoaDonTam");
            //Tạo mới lại session
            Session.Add("hoaDonTam", new cartHoaDonTam());
        }
        /// <summary>
        /// Hàm tính tổng tiền sản phẩm có trong ctHoaDonTam
        /// </summary>
        /// <param name="maBan"></param>
        /// <returns></returns>
        private long layTongTienTuChiTiet(hoaDonTam hoaDon)
        {
            long kq = 0;
            try
            {
                foreach (ctHoaDonTam ct in hoaDon.ctHoaDonTams.ToList())
                    if(ct.trangThaiPhaChe!=4) //------Nếu trạng thái sản phẩm không bị hủy bỏ (ĐỀ XUẤT THAY THẾ)
                        kq += ct.soLuong * ct.donGia;
            }
            catch (Exception ex)
            {
                xulyFile.ghiLoi("Class: NghiepVuBanController - Fucntion: layTongTienTuChiTiet", ex.Message);
            }
            return kq;
        }
        /// <summary>
        /// Hàm tạo tag table cho danh sách sản phẩm đã order có trên bàn
        /// </summary>
        /// <param name="list">Danh sách lấy sản phẩm có trên bàn</param>
        /// <param name="classNameRemove">Tên class để bắt sự kiện xóa sản phẩm khỏi bàn --- THƯỜNG LÀ btnRemoveCart HOẶC btnRemoveDtb với btnRemoveCart đối với danh sách lấy từ session</param>
        /// <param name="classNameUpdate">Tên class để cập nhật số lượng sản phẩm trên bàn THƯỜNG LÀ btnUpdateCart hoặc btnUpadateDtb với btnUpdateCart đối với danh sách lấy từ session</param>
        /// <param name="idTextBoxSoLuong">Tên Id textbox số lượng để xác định số lượng cần cập nhật -- Thường là txtSoLuong hoặc txtSoLuongS với txtSoLuongS đối với danh sách lấy từ sesion</param>
        /// <returns>Chuỗi html bảng danh sách sản phẩm đã đặt</returns>
        private string taoBangDanhSachSanPhamDaOrder(List<ctHoaDonTam> list, string idTextBoxSoLuong, string classNameUpdate, string classNameRemove)
        {
            string html = "";
            html += "<table class=\"table table-hover\">";
            html += "   <thead>";
            html += "       <tr>"; //---Tạo tiêu đề
            html += "           <th width=\"30%\">Tên món</th><th style=\"width:30%\">SL</th><th width=\"15%\">Giá</th><th width=\"25%\"></th>";
            html += "       </tr>";
            html += "   </thead>";
            html += "   <tbody>";
            foreach (ctHoaDonTam ct in list)
            {
                html += "       <tr>";
                html += "           <td>" + xulyDuLieu.traVeKyTuGoc(ct.sanPham.tenSanPham) + "</td>";
                html += "           <td>";
                html += "               <input type=\"number\" value=\"" + ct.soLuong.ToString() + "\" min=1 class=\"form-control\" id=\"" + idTextBoxSoLuong + ct.maCtTam.ToString() + "\" style=\"width:100%\"";
                if (ct.trangThaiPhaChe > 0) html += " disabled "; html += "/>";//----Kiểm tra nếu sản phẩm đang pha chế hoặc đã giao thì không được cập nhật số lượng
                html += "           </td>";
                html += "           <td>" + ct.donGia.ToString() + "</td>";
                html += "           <td>";
                if (ct.trangThaiPhaChe <= 1) //Nếu trạng thái sản phẩm CHƯA PHA CHẾ thì mới được CẬP NHẬT
                {
                    html += "          <div class=\"btn-group\">";
                    html += "               <button type=\"button\" class=\"btn btn-primary dropdown-toggle\" data-toggle=\"dropdown\" aria-expanded=\"true\">   Chức năng               <span class=\"caret\"></span>              </button>";
                    html += "                 <ul class=\"dropdown-menu\" role=\"menu\">";
                    html += "                     <li class=\"" + classNameUpdate + "\"  maCt=\"" + ct.maCtTam.ToString() + "\" ><a class=\"col-blue waves-effect waves-block\"><i class=\"material-icons\">update</i>Cập nhật</a></li>";
                    html += "                     <li class=\"" + classNameRemove + "\"  maCt=\"" + ct.maCtTam.ToString() + "\" ><a class=\"col-red waves-effect waves-block\"><i class=\"material-icons\">clear</i>Bỏ chọn</a></li>";
                    html += "               </ul>";
                    html += "          </div>";
                }
                else if (ct.trangThaiPhaChe == 3) //Nếu trạng thái ĐÃ GIAO đồ uống. KHÔNG ĐƯỢC CẬP NHẬT
                    html += "<label class=\"font-13 col-orange\">Đă giao</label>";
                else if (ct.trangThaiPhaChe == 4) //-------Trạng thái Đề xuất thay thế
                    html += "<button class=\"btnRemoveDtb btn btn-danger waves-effect\" maCt=\"" + ct.maCtTam.ToString() + "\"><i class=\"material-icons\">clear</i>Loại bỏ</button>";
                html += "           </td>";
                html += "       </tr>";
            }
            html += "   </tbody>";
            html += "</table>";
            return html;
        }
    }

}