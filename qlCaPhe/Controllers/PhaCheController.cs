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
    public class PhaCheController : Controller
    {
        private string idOfPage = "601";
        #region READ
        /// <summary>
        /// Hàm lấy danh sách bàn đang chờ pha chế
        /// </summary>
        /// <returns></returns>
        public string AjaxLayDanhSachBanCanPhaChe(int? page)
        {
            string html = ""; int trangHienHanh = (page ?? 1); int soPhanTu = 0;
            if (xulyChung.duocCapNhat(idOfPage, "7"))
            {
                try
                {
                    qlCaPheEntities db = new qlCaPheEntities();
                    soPhanTu = db.hoaDonTams.Where(h => h.trangthaiphucVu == 0 || h.trangthaiphucVu == 1).OrderBy(h => h.ngayLap).Count();
                    //----Lặp qua danh sách hoaDonTam có trangthaiphucVu=0 hoặc trangThaiphucvu=1 --Chờ pha chế hoặc đang pha chếDanh sách được sort theo ngày lập hóa đơn tăng dần.
                    foreach (hoaDonTam hoaDon in db.hoaDonTams.Where(h => h.trangthaiphucVu == 0 || h.trangthaiphucVu == 1).OrderBy(h => h.ngayLap).Skip((trangHienHanh - 1) * createHTML.pageSize).Take(createHTML.pageSize).ToList())
                    {
                        html += "<tr role=\"row\" class=\"odd\">";
                        html += "   <td>";
                        html += "       <a class=\"goiY\" maBan=\"" + hoaDon.maBan.ToString() + "\" style=\"cursor:pointer\">";
                        html += xulyDuLieu.traVeKyTuGoc(hoaDon.BanChoNgoi.tenBan) + "<span class=\"noiDungGoiY-right\">Click để xem chi tiết món đã đặt</span></a>";
                        html += "   </td>";
                        html += "   <td>" + hoaDon.ngayLap.ToString() + "</td>";
                        html += "   <td>" + xulyDuLieu.traVeKyTuGoc(hoaDon.taiKhoan.thanhVien.hoTV) + " " + xulyDuLieu.traVeKyTuGoc(hoaDon.taiKhoan.thanhVien.tenTV) + "</td>";//-----Hiện họ và tên thành viên phục vụ bàn này
                        html += "   <td>" + xulyDuLieu.doiVND(new bHoaDonTam().layTongTienSanPham(hoaDon)) + "</td>";
                        html += "   <td>";
                        html += "       <div class=\"btn-group\">";
                        html += "             <a task=\"" + xulyChung.taoUrlCoTruyenThamSo("PhaChe/pc_ThucHienPhaCheTheoBan", hoaDon.maBan.ToString()) + "\" class=\" guiRequest  btn btn-success waves-effect\"><i class=\"material-icons\">local_dining</i><span>Pha chế</span></a>";
                        html += "       </div>";
                        html += "   </td>";
                        html += "</tr>";
                    }
                    html += "&&"; //------Ký tự xác định chuỗi html để gán lên giao diện
                    html += createHTML.taoPhanTrang(soPhanTu, createHTML.pageSize, trangHienHanh, "/PhaChe/pc_PhaCheTheoBan");
                }
                catch (Exception ex)
                {
                    xulyFile.ghiLoi("Class: PhaCheController - Function: AjaxLayDanhSachBanCanPhaChe", ex.Message);
                }
            }
            return html;
        }


        /// <summary>
        /// Hàm tạo danh sách pha chế theo bàn
        /// Các món, sản phẩm được nhóm theo bàn. Danh sách được sắp xếp theo thời gian nhận pha chế. Thứ tự tg tăng dần
        /// </summary>
        /// <returns></returns>
        public ActionResult pc_PhaCheTheoBan()
        {
            if (xulyChung.duocCapNhat(idOfPage, "7"))
            {
                //-------Tạo modal xem chi tiết hóa đơn
                ViewBag.ModalChiTiet = createHTML.taoModalChiTiet("modalChiTiet", "vungChiTiet", 2);
                xulyChung.ghiNhatKyDtb(1, "Danh mục pha chế theo bàn");
                return View();
            }
            return null;
        }


        /// <summary>
        /// Hàm ajax lấy danh sách các món đã order có trong bảng ctHoaDonTam và hiện lên modal chi tiết
        /// </summary>
        /// <param name="maBan"></param>
        /// <returns>Chuỗi html tạo nên modal</returns>
        public string AjaxXemChitietHoaDon(int maBan)
        {
            string htmlDetails = "";
            if (xulyChung.duocTruyCap(idOfPage))
            {
                try
                {
                    qlCaPheEntities db = new qlCaPheEntities();
                    //--------Lấy thông tin hóa dơn
                    hoaDonTam hoaDon = db.hoaDonTams.SingleOrDefault(h => h.maBan == maBan);
                    if (hoaDon != null)
                    {
                        htmlDetails += "<div class=\"modal-header\">";
                        htmlDetails += "      <button type=\"button\" class=\"close\" data-dismiss=\"modal\">×</button>";
                        htmlDetails += "    <h3 class=\"modal-title\" id=\"largeModalLabel\">XEM CHI TIẾT HÓA ĐƠN</h3>";
                        htmlDetails += "</div>";
                        htmlDetails += "<div class=\"modal-body\">";
                        htmlDetails += "    <div class=\"row\">";
                        htmlDetails += "        <div class=\"col-md-12 col-lg-12\">";
                        htmlDetails += "            <div class=\"card\">";
                        htmlDetails += "                <div class=\"header bg-cyan\">";
                        htmlDetails += "                    <h2>Danh mục món cần pha chế cho bàn " + xulyDuLieu.traVeKyTuGoc(hoaDon.BanChoNgoi.tenBan) + "</h2>";
                        htmlDetails += "                </div>";
                        htmlDetails += "                <div class=\"body table-responsive\">";
                        htmlDetails += "                <!--Nội dung-->";
                        htmlDetails += this.taoBangChiTietHoaDon(hoaDon);
                        htmlDetails += "                </div>";
                        htmlDetails += "            </div>";
                        htmlDetails += "        </div>";
                        htmlDetails += "</div>";
                        htmlDetails += "<div class=\"modal-footer\">";
                        htmlDetails += "    <div class=\"col-md-4\">         ";
                        htmlDetails += "        <div class=\"pull-left\">             ";
                        htmlDetails += "            <label class=\"pull-left col-blue-grey\"><i>* Ghi chú: " + xulyDuLieu.traVeKyTuGoc(hoaDon.ghiChu) + "</i></label>        ";
                        htmlDetails += "        </div>   ";
                        htmlDetails += "    </div>";
                        htmlDetails += "    <div class=\"col-md-8\">          ";
                        htmlDetails += "             <a task=\"" + xulyChung.taoUrlCoTruyenThamSo("PhaChe/pc_ThucHienPhaCheTheoBan", hoaDon.maBan.ToString()) + "\" class=\" guiRequest  btn btn-success waves-effect\"><i class=\"material-icons\">local_dining</i><span>Pha chế</span></a>";
                        htmlDetails += "        <a class=\"btn btn-default waves-effect\"  data-dismiss=\"modal\"><i class=\"material-icons\">close</i>Đóng lại</a>";
                        htmlDetails += "    </div>";
                        htmlDetails += "</div>";
                    }
                }
                catch (Exception ex)
                {
                    xulyFile.ghiLoi("Class: PhaCheController - Function: AjaxXemChitietHoaDon", ex.Message);
                }
            }
            return htmlDetails;
        }
        /// <summary>
        /// hàm tạo bảng danh sách các món đã order trong hoaDonTam được lấy từ database
        /// </summary>
        /// <param name="maBan"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        private string taoBangChiTietHoaDon(hoaDonTam hoaDon)
        {
            string html = "";
            html += "<table class=\"table table-hover\">";
            html += "   <thead>";
            html += "       <tr>";
            html += "           <th style=\"width:70%\">Tên món</th>";
            html += "           <th>Số lượng</th>";
            html += "       </tr>";
            html += "   </thead>";
            html += "   <tbody>";
            //-------Lặp qua danh sách các món trong ctHoaDonTam chưa pha chế
            foreach (ctHoaDonTam ct in hoaDon.ctHoaDonTams.Where(c => c.trangThaiPhaChe == 0).ToList())
            {
                html += "       <tr>";
                html += "           <td>";
                html += "               <img width=\"50px;\" height=\"50px;\" src=\"" + xulyDuLieu.chuyenByteHinhThanhSrcImage(ct.sanPham.hinhAnh) + "\">";
                html += "               <b>" + xulyDuLieu.traVeKyTuGoc(ct.sanPham.tenSanPham) + "</b>";
                html += "           </td>";
                html += "           <td>" + ct.soLuong.ToString() + "</td>";
                html += "       </tr>";
            }
            html += "   </tbody>";
            html += "</table>";
            return html;
        }
        #endregion


        #region NHÓM HÀM PHA CHẾ SẢN PHẨM THEO BÀN
        /// <summary>
        /// Hàm tạo giao diện pha chế theo bàn <para/> và CẬP NHẬT TRẠNG THÁI pha chế
        /// </summary>
        /// <param name="maBan"></param>
        /// <returns></returns>
        public ActionResult pc_ThucHienPhaCheTheoBan()
        {
            if (xulyChung.duocCapNhat(idOfPage, "7"))
            {
                try
                {
                    int kqLuu = 0;
                    string param = xulyChung.nhanThamSoTrongSession();//param = maBan
                    if (param.Length > 0)
                    {
                        int maBan = xulyDuLieu.doiChuoiSangInteger(param);
                        qlCaPheEntities db = new qlCaPheEntities();
                        hoaDonTam hoaDon = db.hoaDonTams.SingleOrDefault(h => h.maBan == maBan);
                        if (hoaDon != null)
                        {
                            ViewBag.TieuDeTrang = "PHA CHẾ CHO BÀN \"" + xulyDuLieu.traVeKyTuGoc(hoaDon.BanChoNgoi.tenBan) + "\"";
                            ViewBag.MaBan = hoaDon.maBan.ToString();//Gán mã bàn để xác định bàn lấy danh sách sản phẩm
                            ViewBag.GhiChu = hoaDon.ghiChu;
                            //-----Cập nhật trạng thái thành pha chế
                            hoaDon.trangthaiphucVu = 1;
                            db.Entry(hoaDon).State = System.Data.Entity.EntityState.Modified;
                            kqLuu = db.SaveChanges();
                            if (kqLuu > 0)
                            {
                                this.insertNewPhieuXuatKho(maBan, db);
                                xulyChung.ghiNhatKyDtb(1, "Tiếp nhận pha chế");
                            }
                        }
                    }
                    else throw new Exception("không nhận được tham số");
                }
                catch (Exception ex)
                {
                    xulyFile.ghiLoi("Class: PhaCheController - Function: pc_ThucHienPhaCheTheoBan", ex.Message);
                    return RedirectToAction("PageNotFound", "Home");
                }
                //-----Tạo modal dạng lớn để chứa chi tiết các bước thực hiện của công thức
                ViewBag.ModalChiTiet = createHTML.taoModalChiTiet("modalChiTiet", "vungChiTiet", 3);
            }
            return View();
        }
        #region Tạo element hiện dữ liệu
        /// <summary>
        /// hàm tạo danh sách các sản phẩm của bàn đả order
        /// </summary>
        /// <param name="maBan"></param>
        /// <returns></returns>
        public string taoBangSanPham(int maBan)
        {

            string html = "";
            try
            {
                qlCaPheEntities db = new qlCaPheEntities();
                //-----Tạo danh sách sản phẩm đang chờ pha chế của bàn
                html += this.taoBangDanhSachSanPhamTheoTrangThaiPhaChe(maBan, 0, db);
                //----Tạo danh sách sản phẩm đang pha chế
                html += "|" + this.taoBangDanhSachSanPhamTheoTrangThaiPhaChe(maBan, 1, db);
                //----Tạo danh sách sản phẩm đã pha chế
                html += "|" + this.taoBangDanhSachSanPhamTheoTrangThaiPhaChe(maBan, 2, db);
            }
            catch (Exception ex)
            {
                xulyFile.ghiLoi("Class: PhaCheController - Function: taoBangSanPham", ex.Message);
            }
            return html;
        }
        /// <summary>
        /// Hàm tạo bảng danh sách sản phẩm trong ctHoaDonTam dựa vào trangThaiPhaChe
        /// </summary>
        /// <param name="maBan"></param>
        /// <param name="trangThai"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        private string taoBangDanhSachSanPhamTheoTrangThaiPhaChe(int maBan, int trangThai, qlCaPheEntities db)
        {
            string html = "";
            html += "<table class=\"table table-hover\">";
            html += "   <thead>";
            html += "       <tr>";
            html += "           <th style=\"width:60%\">Tên món</th> <th width=\"20%\">Số lượng</th> <th width=\"20%\"></th>";
            html += "       </tr>";
            html += "   </thead>";
            html += "<tbody>";
            html += lapQuaSanPham(maBan, trangThai, db);
            html += "</tbody></table>";
            return html;
        }

        /// <summary>
        /// Hàm lặp qua danh sách sản phẩm có trong ctHoadon
        /// </summary>
        /// <param name="maBan"></param>
        /// <param name="trangThai"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        private string lapQuaSanPham(int maBan, int trangThai, qlCaPheEntities db)
        {
            string html = "";
            try
            {
                //--------Lặp qua danh sách sản phẩm trong hóa đơn theo trạng thái cần lấy
                int indexHoaDonTam = 0;
            LapLaiDanhSachOrder: //-------Làm móc để xác định vị trí cần chạy tiếp theo
                ctHoaDonTam ctTam = new ctHoaDonTam();
                foreach (ctHoaDonTam ct in db.ctHoaDonTams.Where(c => c.maBan == maBan && c.trangThaiPhaChe == trangThai).ToList().Skip(indexHoaDonTam))
                {
                    ctTam = ct;
                    indexHoaDonTam++;
                    break;
                }
                if (ctTam.maCtTam > 0)
                {
                    html += ganDuLieuChiTietHoaDon(ctTam);
                    goto LapLaiDanhSachOrder; //-----Lặp lại lấy danh sách sản phẩm tiếp theo trong hóa đơn
                }
            }
            catch (Exception ex)
            {
                xulyFile.ghiLoi("Class: PhaCheController - Function: lapQuaSanPham", ex.Message);
            }
            return html;
        }

        /// <summary>
        /// Hàm gán dữ liệu của 1 object ctHoaDonTam vào 1 dòng trên bảng
        /// </summary>
        /// <param name="ct"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        private string ganDuLieuChiTietHoaDon(ctHoaDonTam ct)
        {
            string html = "";
            //----------Gán dữ liệu cho giao diện
            html += "<tr>";
            html += "   <td>";
            html += "       <div class=\"col-md-4 col-sm-4 col-xs-4 col-lg-4\">";
            html += "           <img width=\"50px;\" height=\"50px;\" src=\"" + xulyDuLieu.chuyenByteHinhThanhSrcImage(ct.sanPham.hinhAnh) + "\">";
            html += "       </div>";
            html += "       <div class=\"col-md-8 col-sm-8 col-xs-8 col-lg-8\">";
            html += "          <h4>" + xulyDuLieu.traVeKyTuGoc(ct.sanPham.tenSanPham) + "</h4>";
            html += "       </div>";
            html += "   </td>";
            html += "   <td>" + ct.soLuong.ToString() + "</td>";
            //----Kiểm tra trạng thái của hóa đơn để thiết lập các nút chức năng tương ứng
            if (ct.trangThaiPhaChe == 0) //-----Trạng thái đang pha chế --- CÓ CHỨC NĂNG PHA CHẾ SẢN PHẨM HOẶC HỦY
            {
                if (new bSanPham().kiemTraSanPhamKhaThi(ct.sanPham))
                    html += "    <td><button type=\"button\" maCt=\"" + ct.maCtTam.ToString() + "\"  class=\"btnPhaChe btn btn-success btn-circle waves-effect waves-circle waves-float\"><i class=\"material-icons\">send</i></button></td>";
                else
                    html += "    <td><button type=\"button\" maCt=\"" + ct.maCtTam.ToString() + "\"  class=\"btnThayThe btn btn-warning btn-circle waves-effect waves-circle waves-float\"><i class=\"material-icons\">undo</i></button></td>";
            }
            else if (ct.trangThaiPhaChe == 1) //----Trạng thái đã pha chế --- CÓ CHỨC NĂNG CẬP NHẬT TRẠNG THÁI PHA CHẾ XONG HOẶC XEM CÔNG THỨC
            {
                congThucPhaChe ctSanPham = ct.sanPham.congThucPhaChes.SingleOrDefault(c => c.trangThai == true);
                if (ctSanPham != null) //-----Tạo nút xem công thức
                    html += "    <td><button type=\"button\" maCongThuc=\"" + ctSanPham.maCongThuc.ToString() + "\"  class=\"btnXemCongThuc btn btn-info btn-circle waves-effect waves-circle waves-float\"><i class=\"material-icons\">info_outline</i></button></td>";
                else
                    html += "   <td></td>";
                html += "    <td><button type=\"button\" maCt=\"" + ct.maCtTam.ToString() + "\"  class=\"btnHoanTat btn btn-danger btn-circle waves-effect waves-circle waves-float\"><i class=\"material-icons\">done</i></button></td>";
            }
            html += "</tr>";
            return html;
        }
        #endregion

        /// <summary>
        /// Hàm xử lý pha chế sản phẩm. Thực hiện các giai đoạn:
        /// Trừ số lượng nguyên liệu sử dụng trong kho
        /// Cập nhật trạng thái
        /// Trả danh sách sản phẩm
        /// </summary>
        /// <param name="maCt"></param>
        /// <returns>Chuổi html tạo danh sách sản phẩm pha chế (Chờ pha chế - Đang pha chế)</returns>
        public string AjaxPhaCheSanPham(int maCt)
        {
            string html = "";
            if (xulyChung.duocCapNhat(idOfPage, "7"))
            {
                try
                {
                    qlCaPheEntities db = new qlCaPheEntities();
                    ctHoaDonTam ctSua = db.ctHoaDonTams.SingleOrDefault(c => c.maCtTam == maCt);
                    if (ctSua != null)
                    {
                        int indexSoLuong = 1; bool hetHang = false;
                        //--------Lặp số lượng sản phẩm để thêm vào phiếu xuất cho hợp lý với tổng số nguyên liệu sử dụng cho 1 order
                        for (int i = 1; i <= ctSua.soLuong; i++)
                        {
                            if (new bSanPham().kiemTraSanPhamKhaThi(ctSua.sanPham)) //-----Nếu còn nguyên liệu để pha chế
                            {
                                this.insertNewCtPhieuXuat(ctSua, db);
                                indexSoLuong = i;
                            }
                            else //------Hết nguyên liệu giữa chừng
                            {
                                hetHang = true; break;
                            }
                        }
                        if (hetHang)
                        {
                            //-----Tạo mới 1 record để đề xuất thay thế
                            ctHoaDonTam ctDeXuat = new ctHoaDonTam();
                            ctDeXuat.maSP = ctSua.maSP;
                            ctDeXuat.donGia = ctSua.donGia;
                            ctDeXuat.hoaDonTam = ctSua.hoaDonTam;
                            ctDeXuat.maBan = ctSua.maBan;
                            ctDeXuat.sanPham = ctSua.sanPham;
                            ctDeXuat.soLuong = ctSua.soLuong - indexSoLuong; //------Số lượng còn lại = tổng số lượng đã đặt - số lượng đã tiếp nhận
                            ctDeXuat.trangThaiPhaChe = 4;
                            db.ctHoaDonTams.Add(ctDeXuat);
                            db.SaveChanges();

                            //----------Cập nhật lại tổng tiền của hóa đơn
                            hoaDonTam hdSua = ctSua.hoaDonTam;
                            hdSua.tongTien = new bHoaDonTam().layTongTienSanPham(hdSua);
                            db.Entry(hdSua).State = System.Data.Entity.EntityState.Modified;
                            db.SaveChanges();

                            //---------------Gọi thread tạo thông báo yêu cầu đề xuất


                        }
                        //----Cập nhật trạng thái đang pha chế sản phẩm này.
                        ctSua.soLuong = indexSoLuong;
                        ctSua.trangThaiPhaChe = 1;
                        db.Entry(ctSua).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                        html += this.taoBangSanPham(ctSua.maBan); //--Tải lại danh sách tất cả sản phầm
                        xulyChung.ghiNhatKyDtb(2, "Pha chế cho sản phẩm \" " + xulyDuLieu.traVeKyTuGoc(ctSua.sanPham.tenSanPham) + " \"");
                    }
                }
                catch (Exception ex)
                {
                    xulyFile.ghiLoi("Class: PhaCheController - Function: AjaxPhaCheSanPham", ex.Message);
                }
            }
            return html;
        }
        /// <summary>
        /// Hàm cập nhật trangThaiPhaChe của 1 ctHoaDonTam sang 4 (ĐỀ XUẤT THAY THẾ) 
        /// khi click vào nút btnThayThe
        /// </summary>
        /// <param name="param">Chi tiết cần cập nhật</param>
        /// <returns>Danh sách bảng sản phẩm</returns>
        public string AjaxDeXuatThayThe(int param)
        {
            string kq = "";
            if (xulyChung.duocCapNhat(idOfPage, "7"))
            {
                try
                {
                    qlCaPheEntities db = new qlCaPheEntities();
                    ctHoaDonTam ctSua = db.ctHoaDonTams.SingleOrDefault(ct => ct.maCtTam == param);
                    if (ctSua != null)
                    {
                        //--------CẬP NHẬT SANG ĐỀ XUẤT THAY THẾ
                        ctSua.trangThaiPhaChe = 4; //-----Trạng thái thay thế
                        db.Entry(ctSua).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();

                        //----------Cập nhật lại tổng tiền cho hóa đơn
                        hoaDonTam hdSua = ctSua.hoaDonTam;
                        hdSua.tongTien = new bHoaDonTam().layTongTienSanPham(hdSua);
                        db.Entry(hdSua).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                        //------------Trả về bảng danh sách sản phẩm
                        kq += this.taoBangSanPham(ctSua.maBan);
                        this.taoThongBao(db, hdSua.nguoiPhucVu, "Thay thế \"" + xulyDuLieu.traVeKyTuGoc(ctSua.sanPham.tenSanPham) + "\" tại Bàn \"" + xulyDuLieu.traVeKyTuGoc(hdSua.BanChoNgoi.tenBan) + "\"");
                        xulyChung.ghiNhatKyDtb(4, "Đề xuất thay thế sản phẩm");
                    }
                }
                catch (Exception ex)
                {
                    xulyFile.ghiLoi("Class: PhaCheController - Function: AjaxPhaCheSanPham", ex.Message);
                }
            }
            return kq;
        }

        ///// <summary>
        ///// Hàm cập nhật số lượng nguyên liệu trong kho khi pha chế
        ///// </summary>
        ///// <param name="ct"></param>
        ///// <param name="db"></param>
        //private void capNhatSoLuongNguyenLieu(ctHoaDonTam ct, qlCaPheEntities db)
        //{
        //    congThucPhaChe congThucSanPham = ct.sanPham.congThucPhaChes.SingleOrDefault(c=>c.trangThai==true);
        //    if (congThucSanPham != null)
        //    {
        //        //----------Lặp qua từng nguyên liệu trong công thức pha chế sản phẩm
        //        foreach (ctCongThuc ctCongThuc in congThucSanPham.ctCongThucs.ToList())
        //        {
        //            if (ctCongThuc.nguyenLieu != null)
        //            {
        //                //nguyenLieuDaXuat tonKho = ctCongThuc.nguyenLieu.nguyenLieuDaXuat;
        //                //double soLuongTon = tonKho.soLuongTon; //------Lấy số lượng nguyên liệu hiện tại trong kho
        //                //if (soLuongTon > 0)
        //                //{
        //                //    double soLuongSuDung = (double)ctCongThuc.soLuongNguyenLieu;
        //                //    ///-------Cập nhật số lượng nguyên liệu trong kho
        //                //    tonKho.soLuongTon -= soLuongSuDung;
        //                //    db.Entry(tonKho).State = System.Data.Entity.EntityState.Modified;
        //                //    db.SaveChanges();
        //                //}
        //            }
        //        }
        //    }
        //}

        /// <summary>
        /// Hàm cập nhật trạng thái pha chế của 1 ctHoaDonTam sang hoàn tất
        /// </summary>
        /// <param name="maCt"></param>
        /// <returns></returns>
        public string AjaxHoanTatMotMon(int maCt)
        {
            string kq = "";
            try
            {
                int kqLuu = 0;
                qlCaPheEntities db = new qlCaPheEntities();
                ctHoaDonTam ctSua = db.ctHoaDonTams.SingleOrDefault(c => c.maCtTam == maCt);
                if (ctSua != null)
                {
                    //----Cập nhật trạng thái hoàn tất pha chế cho sản phẩm này.
                    ctSua.trangThaiPhaChe = 2;
                    db.Entry(ctSua).State = System.Data.Entity.EntityState.Modified;
                    kqLuu = db.SaveChanges();
                    if (kqLuu > 0)
                    {
                        kq += this.taoBangSanPham(ctSua.maBan); //--Tải lại danh sách sản phầm
                        xulyChung.ghiNhatKyDtb(4, "Pha chế hoàn tất cho sản phẩm tại bàn \" " + xulyDuLieu.traVeKyTuGoc(ctSua.hoaDonTam.BanChoNgoi.tenBan) + " \"");
                    }
                }
            }
            catch (Exception ex)
            {
                xulyFile.ghiLoi("Class: PhaCheController - Function: AjaxHoanTatMotMon", ex.Message);
            }
            return kq;
        }

        /// <summary>
        /// Hàm cập nhật trạng thái của hóa đơn khi pha chế hoàn tất
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult hoanTatPhaCheHoaDon(FormCollection f)
        {
            if (xulyChung.duocCapNhat(idOfPage, "7"))
            {
                try
                {
                    //-----Nhận mã hóa đơn cần cập nhật từ SEssion
                    int maHoaDonSua = this.layThamSoTrongSession(0); int kqLuu = 0;
                    if (maHoaDonSua > 0)
                    {
                        qlCaPheEntities db = new qlCaPheEntities();
                        //-----Nếu tất cả sản phẩm đã pha chế xong thì cho phép cập nhật
                        hoaDonTam hoaDonSua = db.hoaDonTams.SingleOrDefault(h => h.maBan == maHoaDonSua);
                        if (hoaDonSua != null)
                        {
                            //------Kiểm tra. Sản phẩm đã pha chế hoàn tất thì cập nhật
                            if (this.kiemTraPhaCheHoanTat(hoaDonSua.ctHoaDonTams.ToList()))
                            {
                                hoaDonSua.ghiChu = xulyDuLieu.xulyKyTuHTML(f["txtGhiChu"]);
                                hoaDonSua.trangthaiphucVu = 2; //Trạng thái đã pha chế - Chờ giao
                                db.Entry(hoaDonSua).State = System.Data.Entity.EntityState.Modified;
                                kqLuu = db.SaveChanges();
                                if (kqLuu > 0)
                                {
                                    taoThongBao(db, hoaDonSua.taiKhoan.tenDangNhap, "Bàn \"" + xulyDuLieu.traVeKyTuGoc(hoaDonSua.BanChoNgoi.tenBan) + "\" đã pha chế");
                                    xulyChung.ghiNhatKyDtb(4, "Hoàn tất pha chế cho bàn \" " + xulyDuLieu.traVeKyTuGoc(hoaDonSua.BanChoNgoi.tenBan) + " \"");
                                }
                            }
                        }
                    }
                    else throw new Exception("KHÔNG nhận được tham số");

                }
                catch (Exception ex)
                {
                    xulyFile.ghiLoi("Class: PhaCheController - Function: hoanTatPhaCheHoaDon", ex.Message);
                    return RedirectToAction("PageNotFound", "Home");
                }
            }
            return RedirectToAction("pc_PhaCheTheoBan");
        }
        /// <summary>
        /// Hàm kiểm tra tình trạng của tất cả sản phẩm trong ctHoaDonTam
        /// </summary>
        /// <param name="list">Danh sách các sản phẩm trong hóa đơn tạm</param>
        /// <returns>True: Tất cả sản phẩm Đã pha chế hoàn tất <para/> False: Còn sản phẩm đang pha chế</returns>
        public bool kiemTraPhaCheHoanTat(List<ctHoaDonTam> list)
        {
            int flagCheck = 0; //-Biến cờ để xác định
            foreach (ctHoaDonTam ct in list)
                if (ct.trangThaiPhaChe == 2 || ct.trangThaiPhaChe == 3 || ct.trangThaiPhaChe == 4) //-----Nếu đã xử lý sản phẩm thì ....
                    flagCheck++; //-----Cộng dồn những sản phẩm đã pha chế hoàn tất
            if (flagCheck == list.Count) //-----Nếu số lượng sản phẩm đã hoàn tất bằng với số sản phẩm trong hóa đơn. 
                return true; //---Sản phẩm đã pha chế hết.
            return false;
        }
        /// <summary>
        /// Hàm tạo một thông báo mới dành cho nhân viên phục vụ để yêu cầu giao hàng hoặc thay thế sản phẩm
        /// </summary>
        /// <param name="db"></param>
        /// <param name="tenDangNhap">Tên đăng nhập của nhân viên phục vụ</param>
        private void taoThongBao(qlCaPheEntities db, string tenDangNhap, string noiDung)
        {
            try
            {
                thongBao thongBaoAdd = new thongBao();
                thongBaoAdd.ndThongBao = noiDung;
                thongBaoAdd.taiKhoan = tenDangNhap;
                thongBaoAdd.ngayTao = DateTime.Now;
                thongBaoAdd.daXem = false;
                thongBaoAdd.ghiChu = "Pha chế";
                db.thongBaos.Add(thongBaoAdd);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                xulyFile.ghiLoi("Class: PhaCheController - Function: taoThongBaoGiaoHang", ex.Message);
            }
        }
        #endregion
        #region NHÓM HÀM LIÊN QUAN ĐẾN XUẤT KHO
        /// <summary>
        /// Hàm thêm mới 1 phiếu xuất kho trong CSDL
        /// </summary>
        /// <param name="maBan">Mã bàn để ghi chú cho phiếu xuất kho.</param>
        /// <param name="db"></param>
        private void insertNewPhieuXuatKho(int maBan, qlCaPheEntities db)
        {
            try
            {
                phieuXuatKho phieuXuatAdd = new phieuXuatKho();
                phieuXuatAdd.maKho = db.khoHangs.First().maKhoHang; //-------Lấy thông tin kho hàng
                phieuXuatAdd.ngayXuat = DateTime.Now;
                phieuXuatAdd.nguoiLapPhieu = xulyChung.layTenDangNhap();
                phieuXuatAdd.ghiChu = "XUẤT NGUYÊN LIỆU PHA CHẾ CHO BÀN CÓ MÃ: " + maBan.ToString();
                db.phieuXuatKhoes.Add(phieuXuatAdd);
                db.SaveChanges();
                //--------Gán lại mã phiếu vào session để xác định phiếu cần thêm nguyên liệu xuất
                Session["urlAction"] = "page=pc_ThucHienPhaCheTheoBan|request?maBan=" + maBan.ToString() + "&maPhieu=" + phieuXuatAdd.maPhieu.ToString(); ;
            }
            catch (Exception ex)
            {
                xulyFile.ghiLoi("Class: PhaCheController - Function: insertNewPhieuXuatKho", ex.Message);
            }
        }

        /// <summary>
        /// Hàm thêm mới dữ liệu vào bảng ctPhieuXuatKho khi tiếp nhận pha chế 1 sản phẩm
        /// </summary>
        /// <param name="ct"></param>
        /// <param name="db"></param>
        private void insertNewCtPhieuXuat(ctHoaDonTam ct, qlCaPheEntities db)
        {
            try
            {
                //------Nhận mã phiếu xuất vửa thêm khi vào trang.
                int maPhieuXuat = this.layThamSoTrongSession(1);
                //---------DUYỆT QUA NGUYÊN LIỆU LÀM NÊN SẢN PHẨM ĐỂ THÊM VÀO PHIẾU XUẤT
                congThucPhaChe congThucSanPham = ct.sanPham.congThucPhaChes.SingleOrDefault(c => c.trangThai == true);
                if (congThucSanPham != null)
                {
                    //----------Lặp qua từng nguyên liệu trong công thức pha chế sản phẩm
                    foreach (ctCongThuc ctCongThuc in congThucSanPham.ctCongThucs.Where(c => c.maNguyenLieu > 0).ToList())
                    {
                        //---------Lấy thông tin chi tiết để xem nguyên đang duyệt có trong phiếu chưa
                        ctPhieuXuatKho ctXuat = db.ctPhieuXuatKhoes.SingleOrDefault(c => c.maPhieu == maPhieuXuat && c.maNguyenLieu == ctCongThuc.maNguyenLieu);
                        if (ctXuat == null) //--------Chưa có nguyên liệu này trong phiếu => tạo mới
                        {
                            ctXuat = new ctPhieuXuatKho();
                            ctXuat.maPhieu = maPhieuXuat;
                            ctXuat.maNguyenLieu = (int)ctCongThuc.maNguyenLieu;
                            ctXuat.soLuongXuat = ctCongThuc.soLuongNguyenLieu;
                            ctXuat.donGiaXuat = new bTonKho().layDonGiaNguyenLieuTonKho(ctXuat.maNguyenLieu);
                            ctXuat.ghiChu = "XUẤT NGUYÊN LIỆU KHI PHA CHẾ";
                            db.ctPhieuXuatKhoes.Add(ctXuat);
                            db.SaveChanges();
                        }
                        else //-------Cập nhật số lượng nguyên liệu
                            this.capNhatSoLuongNguyenLieuTrongChiTiet(db, ctXuat, (double)ctCongThuc.soLuongNguyenLieu);
                    }
                }
            }
            catch (Exception ex)
            {
                xulyFile.ghiLoi("Class: PhaCheController - Function: insertCtPhieuXuatKho", ex.Message);
            }
        }

        /// <summary>
        /// Thực hiện xóa ct cũ và cập nhật lại để thực hiện trigger
        /// </summary>
        /// <param name="db"></param>
        /// <param name="ctXuat">Số lượng nguyên liệu</param>
        private void capNhatSoLuongNguyenLieuTrongChiTiet(qlCaPheEntities db, ctPhieuXuatKho ctXuat, double soLuong)
        {
            ctPhieuXuatKho ctNew = new ctPhieuXuatKho();
            ctNew = ctXuat;
            //--------Cộng dồn số lượng nguyên liệu
            ctNew.soLuongXuat += soLuong;
            db.ctPhieuXuatKhoes.Remove(ctXuat);
            db.SaveChanges();
            //----Thêm mới
            db.ctPhieuXuatKhoes.Add(ctNew);
            db.SaveChanges();
        }
      
        /// <summary>
        /// Hàm lấy 1 tham số từ trong Session
        /// </summary>
        /// <param name="index">vị trí tham số trong session cần lấy. <para/> 1: maBan <para/> 2: maPhieuXuat</param>
        /// <returns>tham số cần lấy</returns>
        private int layThamSoTrongSession(int index)
        {
            int kq = 0;
            int[] mangThamSo = new int[2];
            //------Dữ liệu trong session có dạng: page=pc_ThucHienPhaCheTheoBan|request?maBan=MABAN&maPhieu=MAPHIEU
            string param = (string)Session["urlAction"];
            param = param.Split('|')[1]; //-------param = request?maBan=MABAN&maPhieu=MAPHIEU
            param = param.Replace("request?", ""); //------param = maBan=MABAN&maPhieu=MAPHIEU
            //---------Lấy mã bàn (mã hóa đơn tạm
            string strMaBan = param.Split('&')[0]; //------strMaBan = maBan=MABAN;
            strMaBan = strMaBan.Replace("maBan=", "");//-----strMaBan = MABAN
            mangThamSo[0] = xulyDuLieu.doiChuoiSangInteger(strMaBan); //------Gán mã bàn vào vị trí đầu tiên
            //---------Lấy mã phiếu xuất kho
            string strMaPhieu = param.Split('&')[1]; //--------strMaPhieu = maPhieu=MAPHIEU
            strMaPhieu = strMaPhieu.Replace("maPhieu=", "");//-------strMaPhieu = MAPHIEU
            mangThamSo[1] = xulyDuLieu.doiChuoiSangInteger(strMaPhieu); //-------Gán mã phiếu vào vị trí thứ 2 trong mảng
            kq = mangThamSo[index];
            return kq;
        }
        #endregion
    }
}