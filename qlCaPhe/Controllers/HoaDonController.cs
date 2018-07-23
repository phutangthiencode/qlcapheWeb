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
    public class HoaDonController : Controller
    {
        private string idOfPage = "502";

        #region CRUD    

        /// <summary>
        /// Hàm tạo giao diện danh sách các hóa đơn đang chờ thanh toán
        /// </summary>
        /// <returns></returns>
        public ActionResult hd_TableHoaDonChoThanhToan()
        {
            if (xulyChung.duocTruyCap(idOfPage))
            {
                xulyChung.ghiNhatKyDtb(1, "Danh mục hóa đơn chờ thanh toán");
                return View();
            }
            return null;
        }
        /// <summary>
        /// Hàm lấy danh sách các hóa đơn tạm yêu cầu thanh toán.
        /// </summary>
        /// <returns></returns>
        public string AjaxLayDanhSachHoaDonTam(int ?page)
        {
            string kq = ""; int trangHienHanh = (page ?? 1); int soPhanTu = 0;
            if (xulyChung.duocCapNhat(idOfPage, "7"))
            {
                try
                {
                    qlCaPheEntities db = new qlCaPheEntities();
                    soPhanTu = db.hoaDonTams.Where(h => h.trangThaiHoadon == 2 || h.trangThaiHoadon == 1).Count();
                    //---------Lặp qua các hóa đơn ĐÃ GỌI MÓN hoặc ĐANG CHỜ THANH TOÁN
                    foreach (hoaDonTam hd in db.hoaDonTams.Where(h => h.trangThaiHoadon == 2 || h.trangThaiHoadon == 1).OrderBy(h => h.ngayLap).Skip((trangHienHanh - 1) * createHTML.pageSize).Take(createHTML.pageSize).ToList())
                    {
                        kq += "<tr role=\"row\" class=\"odd\">";
                        kq += "   <td>" + xulyDuLieu.traVeKyTuGoc(hd.BanChoNgoi.tenBan) + "</td>";
                        kq += "   <td>" + hd.ngayLap.ToString() + "</td>";
                        kq += "   <td>" + hd.thoiDiemDen.ToString() + "</td>";
                        kq += "   <td>" + xulyDuLieu.traVeKyTuGoc(hd.taiKhoan.thanhVien.hoTV) + " " + xulyDuLieu.traVeKyTuGoc(hd.taiKhoan.thanhVien.tenTV) + "</td>";
                        kq += "   <td>" + xulyDuLieu.doiVND(new bHoaDonTam().layTongTienSanPham(hd)) + "</td>";
                        kq += "   <td><button task=\"" + xulyChung.taoUrlCoTruyenThamSo("/HoaDon/hd_ThucHienThanhToan", hd.maBan.ToString()) + "\" class=\" guiRequest btn btn-primary waves-effect\" data-toggle=\"modal\" data-target=\"#modalThanhToan\"><i class=\"material-icons\">attach_money</i>Thanh toán</button></td>";
                        kq += " </tr>";
                    }
                    kq += "&&"; //------Ký tự xác định chuỗi html để gán lên giao diện
                    kq += createHTML.taoPhanTrang(soPhanTu, createHTML.pageSize, trangHienHanh, "/HoaDon/hd_TableHoaDonChoThanhToan");
                }
                catch (Exception ex)
                {
                    xulyFile.ghiLoi("Class: HoaDonController - Function: AjaxLayDanhSachHoaDonTam", ex.Message);
                }
            }
            return kq;
        }

        /// <summary>
        /// Hàm tạo giao diện thanh toán
        /// </summary>
        /// <returns>View với model hoaDonTam</returns>
        public ActionResult hd_ThucHienThanhToan()
        {
            hoaDonTam hoaDonThanhToan = new hoaDonTam();
            if (xulyChung.duocCapNhat(idOfPage, "7"))
            {
                try
                {
                    //--------Lấy thông tin hóa đơn
                    int maBan = this.layMaBanTuSession();
                    hoaDonThanhToan = new qlCaPheEntities().hoaDonTams.SingleOrDefault(s => s.maBan == maBan);
                    xulyChung.ghiNhatKyDtb(1, "Thực hiện thanh toán cho hóa đơn được lập vào \" " + xulyDuLieu.traVeKyTuGoc(hoaDonThanhToan.ngayLap.ToString()) + " \"");
                }
                catch (Exception ex)
                {
                    xulyFile.ghiLoi("Class: HoaDonController - Function: hd_ThucHienThanhToan", ex.Message);
                    return RedirectToAction("PageNotFound", "Home");
                }
            }
            return View(hoaDonThanhToan);
        }

        [HttpPost]
        /// <summary>
        /// Hàm hoàn tất thanh toán 1 hóa đơn.
        /// 1. Tạo hóa đơn order 
        /// 2. Xóa hóa đơn tạm
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        public ActionResult hd_ThucHienThanhToan(FormCollection f)
        {
            string modal = ""; int kqLuu = 0;
            hoaDonTam hoaDonThanhToan = new hoaDonTam(); hoaDonOrder hoaDonAdd = new hoaDonOrder();
            if (xulyChung.duocCapNhat(idOfPage, "7"))
            {
                try
                {
                    ////--------Lấy thông tin hóa đơn
                    int maBan = this.layMaBanTuSession();
                    qlCaPheEntities db = new qlCaPheEntities();
                    hoaDonThanhToan = db.hoaDonTams.SingleOrDefault(s => s.maBan == maBan);
                    if (hoaDonThanhToan != null)
                    {
                        //--------Thêm vào bảng hoaDonOrder
                        hoaDonAdd = this.InsertHoaDonOrder(hoaDonThanhToan, db, f);
                        if (hoaDonAdd != null)
                        {
                            //--------Thêm vào bảng ctHoaDonOrder
                            this.InsertCtHoaDonOrder(hoaDonAdd, hoaDonThanhToan.ctHoaDonTams.ToList(), db);
                            //--------Cập nhật trạng thái hóa đơn tạm thành 3 ĐÃ thanh toán => chờ reset
                            this.UpdateHoaDonTam(hoaDonThanhToan, db);
                            xulyChung.ghiNhatKyDtb(2, "Thêm mới hóa đơn có mã \" "+hoaDonAdd.maHoaDon.ToString()+ " \"");
                        }
                    }
                    modal = this.taoModalInBill(hoaDonAdd, db, f);
                }
                catch (Exception ex)
                {
                    modal = createHTML.taoThongBaoLuu(ex.Message);
                    xulyFile.ghiLoi("Class: HoaDonController - Function: hd_ThucHienThanhToan", ex.Message);
                    return RedirectToAction("PageNotFound", "Home");
                }
                ViewBag.ModalBill = modal;
            }
            return View(hoaDonThanhToan);
        }


        /// <summary>
        /// Hàm lấy mã bàn từ session
        /// Thực hiện xử lý chuỗi session để xác định mã bàn
        /// </summary>
        /// <returns>Mã bàn có trong session</returns>
        private int layMaBanTuSession()
        {
            int maBan = 0;
            string param = (string)Session["urlAction"]; //----param = page=HoaDon/hd_ThucHienThanhToan|request=maBan
            if (param.Length > 0)
            {
                //-------Xử lý request
                param = param.Split('|')[1]; //-------param = request=maBan
                param = param.Replace("request=", "");  //------param = maBan
                maBan = xulyDuLieu.doiChuoiSangInteger(param);
            }
            else throw new Exception("không nhận được tham số");
            return maBan;
        }

        /// <summary>
        /// Hàm thêm mới 1 hóa đơn vào csdl cho bảng hoDonOrder
        /// </summary>
        /// <param name="hoaDonTam">Hóa đơn tạm để gán dữ liệu</param>
        /// <param name="db"></param>
        /// <param name="f"></param>
        private hoaDonOrder InsertHoaDonOrder(hoaDonTam hoaDonTam, qlCaPheEntities db, FormCollection f)
        {
            int kqLuu = 0;
            hoaDonOrder hoaDonAdd = new hoaDonOrder();
            hoaDonAdd.maBan = hoaDonTam.maBan;
            hoaDonAdd.trangThai = 1;//----Thiết lập trạng thái hóa đơn đã thanh toán
            hoaDonAdd.ngayLap = DateTime.Now;
            hoaDonAdd.thoiDiemDen = hoaDonTam.thoiDiemDen;
            hoaDonAdd.nguoiPhucVu = hoaDonTam.nguoiPhucVu;
            hoaDonAdd.BanChoNgoi = hoaDonTam.BanChoNgoi;
            hoaDonAdd.nguoiXuatHD = ((taiKhoan)Session["login"]).tenDangNhap;
            hoaDonAdd.chietKhau = xulyDuLieu.doiChuoiSangInteger(f["txtTienChietKhau"]);
            hoaDonAdd.tongTien = new bHoaDonTam().layTongTienSanPham(hoaDonTam);
            hoaDonAdd.tamTinh = hoaDonAdd.tongTien - hoaDonAdd.chietKhau;
            hoaDonAdd.ghiChu = hoaDonTam.ghiChu;
            db.hoaDonOrders.Add(hoaDonAdd);
            kqLuu = db.SaveChanges();
            return hoaDonAdd;
        }
        /// <summary>
        /// Hàm thêm mới dữ liệu cho bảng ctHoaDonOrder trong DATABASE
        /// </summary>
        /// <param name="hoaDon">Chi tiết cho hóa đơn</param>
        /// <param name="listChiTietTam">Danh sách các sản phẩm trong hóa đơn tạm</param>
        private void InsertCtHoaDonOrder(hoaDonOrder hoaDon, List<ctHoaDonTam> listChiTietTam, qlCaPheEntities db)
        {
            //--------Lặp qua danh sách ctHoaDonTam và thêm vào ctHoaDonOrder
            foreach (ctHoaDonTam ctTam in listChiTietTam.Where(c=>c.trangThaiPhaChe!=4))
            {
                //-----Nếu ctHoaDonOrder đã tồn tại sản phẩm trong ctTam thì cập nhật số lượng
                ctHoaDonOrder ctAdd = hoaDon.ctHoaDonOrders.SingleOrDefault(c => c.maSanPham == ctTam.maSP);
                if (ctAdd == null)
                {
                    //-------Thêm mới chi tiết
                    ctAdd = new ctHoaDonOrder();
                    ctAdd.maSanPham = ctTam.maSP;
                    ctAdd.sanPham = ctTam.sanPham;
                    ctAdd.donGia = ctTam.donGia;
                    ctAdd.soLuong = ctTam.soLuong;
                    ctAdd.hoaDonOrder = hoaDon;
                    db.ctHoaDonOrders.Add(ctAdd);
                    db.SaveChanges();
                }
                else
                {
                    //-----Cập nhật chi tiết
                    ctAdd.soLuong += ctTam.soLuong;
                    db.Entry(ctAdd).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                }
            }
        }
        /// <summary>
        /// Hàm cập nhật trạng thái hóa đơn của hoaDonTam trong CSDL
        /// </summary>
        /// <param name="hoaDonTam"></param>
        private void UpdateHoaDonTam(hoaDonTam hoaDonSua, qlCaPheEntities db)
        {
            hoaDonSua.trangThaiHoadon = 3;//-------Thiết lập trạng thái đã thanh toán
            db.Entry(hoaDonSua).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
        }


        /// <summary>
        /// Hàm tạo modal in hóa đơn sau khi đã thanh toán
        /// </summary>
        /// <param name="hoaDon"></param>
        /// <returns></returns>
        private string taoModalInBill(hoaDonOrder hoaDon, qlCaPheEntities db, FormCollection f)
        {
            string kq = ""; cauHinh cauHinh = db.cauHinhs.First(); long tienMat = xulyDuLieu.doiChuoiSangLong(f["txtTienMat"]), tienTra = xulyDuLieu.doiChuoiSangLong(f["txtTienTra"]);
            kq+="<div class=\"modal fade in\" id=\"modalInHoaDon\" tabindex=\"-1\" role=\"dialog\" style=\"display: block;\">";
            kq+="    <div class=\"modal-dialog modal-sm\" role=\"document\">";
            kq+="        <div class=\"modal-content\">";
            kq+="            <div class=\"modal-header bg-blue\">";
            kq+="                <h4 class=\"modal-title\" id=\"smallModalLabel\">IN HÓA ĐƠN</h4>";
            kq+="            </div>";
            kq += "            <div class=\"modal-body\" id=\"printBill\">";
            kq+="                <div class=\"row text-center\">";
            kq+="                    <h4>"+xulyDuLieu.traVeKyTuGoc(cauHinh.tenQuan)+"</h4>";
            kq+="                    <label><i>Địa chỉ: "+xulyDuLieu.traVeKyTuGoc(cauHinh.diaChi)+"</i></label>";
            kq+="                    <label><i>ĐT: "+xulyDuLieu.traVeKyTuGoc(cauHinh.hotLine)+"</i></label><br/>";
            kq+="                    <label>*********************</label>";
            kq+="                </div>";
            kq+="                <div class=\"row\">";
            kq+="                    <h4 class=\"text-center\">HÓA ĐƠN THANH TOÁN</h4>";
            kq+="                    <p>Số TT: "+hoaDon.maHoaDon.ToString()+"</p>";
            kq+="                    <p><b>Bàn: "+xulyDuLieu.traVeKyTuGoc(hoaDon.BanChoNgoi.tenBan)+"</b> - Ngày: "+hoaDon.ngayLap.ToString()+"</p>";
            kq+="                </div>";
            kq+="                <div class=\"row\">";
            kq+="                    <table>";
            kq+="                        <thead>";
            kq+="                            <tr>";
            kq+="                                <th width=\"40%\">Tên hàng</th>";
            kq+="                                <th width=\"20%\">Đơn giá</th>";
            kq+="                                <th width=\"20%\">T.Tiền</th>";
            kq+="                            </tr>";
            kq+="                        </thead>";
            kq+="                        <tbody>";
            foreach(ctHoaDonOrder ct in hoaDon.ctHoaDonOrders.ToList())
            {
                kq+="                            <tr>";
                kq+="                                <td>"+xulyDuLieu.traVeKyTuGoc(ct.sanPham.tenSanPham)+" (x"+ct.soLuong.ToString()+")</td>";
                kq+="                                <td>"+xulyDuLieu.doiVND(ct.donGia).Replace("VNĐ", "")+"</td>";
                kq+="                                <td>"+xulyDuLieu.doiVND((ct.soLuong* ct.donGia)).Replace("VNĐ", "")+"</td>";
                kq+="                            </tr>";
            }
            kq+="                        </tbody>";
            kq+="                    </table>";
            kq+="                </div>";
            kq+="                <br/>";
            kq+="                <div class=\"font-16 bold\">";
            kq+="                    <label>T.Cộng:</label>";
            kq+="                    <label class=\"pull-right\">"+xulyDuLieu.doiVND(hoaDon.tongTien)+"</label>";
            kq+="                </div>";
            kq+="                <div class=\"font-18 bold\">";
            kq+="                    <label>Tạm tính:</label>";
            kq+="                    <label class=\"pull-right\">"+xulyDuLieu.doiVND(hoaDon.tamTinh)+"</label>";
            kq+="                </div>";
            kq+="                <div class=\"font-14 bold\">";
            kq+="                    <label>Nhận tiền mặt:</label>";
            kq+="                    <label class=\"pull-right\">"+xulyDuLieu.doiVND(tienMat)+"</label>";
            kq+="                </div>";
            kq+="                <div class=\"font-14 bold\">";
            kq+="                    <label>Tiền trả:</label>";
            kq+="                    <label class=\"pull-right\">"+xulyDuLieu.doiVND(tienTra)+"</label>";
            kq+="                </div>";
            kq+="                <div class=\"font-11 text-center\">";
            kq+="                    <label>Cám ơn quý khách - hẹn gặp lại !!!</label>";
            kq+="                </div>";
            kq+="            </div>";
            kq+="            <div class=\"modal-footer\">";
            kq += "<button id=\"print\" class=\"btn btn-primary waves-effect\" onclick=\"printContent('printBill');\" ><i class=\"material-icons\">print</i>In hóa đơn</button>";
            kq += "                <a href=\"/HoaDon/hd_TableHoaDonChoThanhToan\" class=\"btn btn-link waves-effect\">CLOSE</a>";
            kq+="            </div>";
            kq+="        </div>";
            kq+="    </div>";
            kq += "</div>";
            return kq;
        }
        /// <summary>
        /// Hàm tạo bill thanh toán cần in
        /// </summary>
        /// <param name="hoaDon">Hóa đơn cần hiển thị thông tin</param>
        /// <param name="cauHinh">Thông tin cấu hình của quán</param>
        /// <param name="tienMat">Số tiền khách đưa</param>
        /// <param name="tienTra">Số tiền trả lại cho khách</param>
        /// <returns></returns>
        private string scriptTaoHoaDon(hoaDonOrder hoaDon, cauHinh cauHinh, long tienMat, long tienTra)
        {
            string kq = "";
            kq += "                <div class=\"row text-center\">";
            kq += "                    <h4>" + xulyDuLieu.traVeKyTuGoc(cauHinh.tenQuan) + "</h4>";
            kq += "                    <label><i>Địa chỉ: " + xulyDuLieu.traVeKyTuGoc(cauHinh.diaChi) + "</i></label>";
            kq += "                    <label><i>ĐT: " + xulyDuLieu.traVeKyTuGoc(cauHinh.hotLine) + "</i></label><br/>";
            kq += "                    <label>*********************</label>";
            kq += "                </div>";
            kq += "                <div class=\"row\">";
            kq += "                    <h4 class=\"text-center\">HÓA ĐƠN THANH TOÁN</h4>";
            kq += "                    <p>Số TT: " + hoaDon.maHoaDon.ToString() + "</p>";
            kq += "                    <p><b>Bàn: " + xulyDuLieu.traVeKyTuGoc(hoaDon.BanChoNgoi.tenBan) + "</b> - Ngày: " + hoaDon.ngayLap.ToString() + "</p>";
            kq += "                </div>";
            kq += "                <div class=\"row\">";
            kq += "                    <table>";
            kq += "                        <thead>";
            kq += "                            <tr>";
            kq += "                                <th width=\"40%\">Tên hàng</th>";
            kq += "                                <th width=\"20%\">Đơn giá</th>";
            kq += "                                <th width=\"20%\">T.Tiền</th>";
            kq += "                            </tr>";
            kq += "                        </thead>";
            kq += "                        <tbody>";
            foreach (ctHoaDonOrder ct in hoaDon.ctHoaDonOrders.ToList())
            {
                kq += "                            <tr>";
                kq += "                                <td>" + xulyDuLieu.traVeKyTuGoc(ct.sanPham.tenSanPham) + " (x" + ct.soLuong.ToString() + ")</td>";
                kq += "                                <td>" + xulyDuLieu.doiVND(ct.donGia) + "</td>";
                kq += "                                <td>" + (ct.soLuong * ct.donGia).ToString() + "</td>";
                kq += "                            </tr>";
            }
            kq += "                        </tbody>";
            kq += "                    </table>";
            kq += "                </div>";
            kq += "                <br/>";
            kq += "                <div class=\"font-16 bold\">";
            kq += "                    <label>T.Cộng:</label>";
            kq += "                    <label class=\"pull-right\">" + xulyDuLieu.doiVND(hoaDon.tongTien) + "</label>";
            kq += "                </div>";
            kq += "                <div class=\"font-18 bold\">";
            kq += "                    <label>Tạm tính:</label>";
            kq += "                    <label class=\"pull-right\">" + xulyDuLieu.doiVND(hoaDon.tamTinh) + "</label>";
            kq += "                </div>";
            kq += "                <div class=\"font-14 bold\">";
            kq += "                    <label>Nhận tiền mặt:</label>";
            kq += "                    <label class=\"pull-right\">" + tienMat + "</label>";
            kq += "                </div>";
            kq += "                <div class=\"font-14 bold\">";
            kq += "                    <label>Tiền trả:</label>";
            kq += "                    <label class=\"pull-right\">" + tienTra + "</label>";
            kq += "                </div>";
            kq += "                <div class=\"font-11 text-center\">";
            kq += "                    <label>Cám ơn quý khách - hẹn gặp lại !!!</label>";
            kq += "                </div>";
            return kq;
        }

        #endregion

        #region HOADONORDER

        /// <summary>
        /// Hàm tạo danh sách TẤT CẢ hóa đơn ĐÃ THANH TOÁN
        /// </summary>
        /// <returns></returns>
        public ActionResult hd_TableTatCaHoaDonOrder(int ?page)
        {
            if (xulyChung.duocCapNhat(idOfPage, "7"))
            {
                try
                {
                    //-------Lấy danh sách tất cả hóa đơn
                    ViewBag.TableData = this.layDanhSachHoaDon(4, page);
                    xulyChung.ghiNhatKyDtb(1, "Danh mục tất cả hóa đơn");
                }
                catch (Exception ex)
                {
                    xulyFile.ghiLoi("Class: HoaDonController - Function: hd_TableHoaDonOrder", ex.Message);
                }
            }
            return View();
        }
        /// <summary>
        /// Hàm tạo danh sách các hóa đơn ĐÃ THANH TOÁN TRONG NGÀY
        /// </summary>
        /// <returns></returns>
        public ActionResult hd_TableHoaDonOrderTrongNgay(int ?page)
        {
            if (xulyChung.duocCapNhat(idOfPage, "7"))
            {
                try
                {
                    ViewBag.TableData = this.layDanhSachHoaDon(1, page);
                    xulyChung.ghiNhatKyDtb(1, "Danh mục hóa đơn có trong ngày");
                }
                catch (Exception ex)
                {
                    xulyFile.ghiLoi("Class: HoaDonController - Function: hd_TableHoaDonOrder", ex.Message);
                }
            }
            return View();
        }
        /// <summary>
        /// Hàm tạo giao diện danh sách hóa đơn theo TUẦN
        /// </summary>
        /// <returns></returns>
        public ActionResult hd_TableHoaDonOrderTrongTuan(int ?page)
        {
            if (xulyChung.duocCapNhat(idOfPage, "7"))
            {
                try
                {
                    ViewBag.TableData = this.layDanhSachHoaDon(3, page);
                    xulyChung.ghiNhatKyDtb(1, "Danh mục hóa đơn trong tuần");
                }
                catch (Exception ex)
                {
                    xulyFile.ghiLoi("Class: HoaDonController - Function: hd_TableHoaDonOrder", ex.Message);
                }
            }
            return View();
        }

        /// <summary>
        /// Hàm lấy danh sách hóa đơn
        /// </summary>
        /// <param name="loaiOrder">Loại hóa đơn cần lấy: <para/> 1: Theo ngày, 2: Theo ca, 3: Theo tuần, 4: tất cả</param>
        /// <returns>Danh sách hóa đơn đã được lấy theo điều kiện cần lấy</returns>
        private string layDanhSachHoaDon(int loaiOrder, int ?page)
        {
            string kq = "", url = "/HoaDon/";
            int pageSize = 25; int trangHienHanh = (page ?? 1); int soPhanTu = 0;//--------Số tất cả phần tử trong list            
            qlCaPheEntities db = new qlCaPheEntities();List<hoaDonOrder> listHoaDon = new List<hoaDonOrder>();
            switch (loaiOrder)
            {
                case 1: //---------Lấy hóa đơn theo ngày
                    url += "hd_TableHoaDonOrderTrongNgay";
                    soPhanTu = db.hoaDonOrders.Where(h => h.ngayLap.Value.Day == DateTime.Now.Day && h.ngayLap.Value.Month == DateTime.Now.Month && h.ngayLap.Value.Year == DateTime.Now.Year).Count();
                    listHoaDon = db.hoaDonOrders.Where(h => h.ngayLap.Value.Day == DateTime.Now.Day && h.ngayLap.Value.Month == DateTime.Now.Month && h.ngayLap.Value.Year == DateTime.Now.Year).OrderByDescending(h=>h.ngayLap).Skip((trangHienHanh - 1) * pageSize).Take(pageSize).ToList();
                    break;
                case 2: //--------Lấy hóa đơn theo ca
                   
                    break;
                case 3: //--------Lấy hóa đơn theo tuần
                    url += "hd_TableHoaDonOrderTrongTuan";
                    DateTime dateLastWeek = DateTime.Now.AddDays(-7); //-------Lấy ngảy này của tuần trước
                    soPhanTu = db.hoaDonOrders.Where(h => h.ngayLap >= dateLastWeek && h.ngayLap <= DateTime.Now).Count();
                    listHoaDon = db.hoaDonOrders.Where(h => h.ngayLap >= dateLastWeek && h.ngayLap <= DateTime.Now).OrderByDescending(h => h.ngayLap).Skip((trangHienHanh - 1) * pageSize).Take(pageSize).ToList();
                    break;
                case 4: //---------Lấy tất cả hóa đơn
                    url += "hd_TableTatCaHoaDonOrder";
                    soPhanTu = db.hoaDonOrders.ToList().Count();
                    listHoaDon = db.hoaDonOrders.OrderByDescending(n=>n.ngayLap).Skip((trangHienHanh-1) * pageSize).Take(pageSize).ToList();
                    break;
                default: break;
            }
            kq += taoBangHoaDon(listHoaDon);
            ViewBag.PhanTrang = createHTML.taoPhanTrang(soPhanTu, createHTML.pageSize, trangHienHanh, url); //------cấu hình phân trang
            //----Nhúng script ajax hiển thị chi tiết hóa đơn khi người dùng click vào mã số hóa đơn trên bảng
            ViewBag.ScriptAjaxXemChiTiet = createScriptAjax.scriptAjaxXemChiTietKhiClick("btnXemChiTiet", "maHD", "HoaDon/AjaxXemChiTietHoaDonOrder?maHD=", "vungChiTiet", "modalChiTiet");
            //----Nhúng các tag html cho modal chi tiết
            ViewBag.ModalChiTiet = createHTML.taoModalChiTiet("modalChiTiet", "vungChiTiet", 2);
            return kq;
        }
        /// <summary>
        /// Hàm tạo bảng danh sách hóa đơn
        /// </summary>
        /// <param name="listHoaDon">Danh sách hóa đơn cần lấy dữ liệu</param>
        /// <returns>Html các dòng dữ liệu cho bảng danh sách hóa đơn</returns>
        private string taoBangHoaDon(List<hoaDonOrder> listHoaDon)
        {
            string kq = "";
            foreach (hoaDonOrder hoaDon in listHoaDon)
            {
                kq += "<tr role=\"row\" class=\"odd\">";
                kq += "     <td><a href=\"#\" maHD=\""+hoaDon.maHoaDon.ToString()+"\" class=\"goiY\">"+hoaDon.maHoaDon.ToString()+"<span class=\"noiDungGoiY-right\">Click để xem chi tiết</span></a></td>";
                kq += "     <td>"+xulyDuLieu.traVeKyTuGoc(hoaDon.BanChoNgoi.tenBan)+"</td>";
                kq += "     <td>"+hoaDon.ngayLap.ToString()+"</td>";
                kq += "     <td>"+xulyDuLieu.traVeKyTuGoc(hoaDon.taiKhoan.thanhVien.hoTV) + " " + xulyDuLieu.traVeKyTuGoc(hoaDon.taiKhoan.thanhVien.tenTV)+"</td>";
                kq += "     <td>" + xulyDuLieu.traVeKyTuGoc(hoaDon.taiKhoan1.thanhVien.hoTV) + " " + xulyDuLieu.traVeKyTuGoc(hoaDon.taiKhoan1.thanhVien.tenTV) + "</td>";
                kq += "     <td>"+xulyDuLieu.doiVND(hoaDon.tongTien)+"</td>";
                kq += "     <td><button type=\"button\" maHD=\""+hoaDon.maHoaDon.ToString()+"\" class=\"btnXemChiTiet btn btn-info waves-effect\"><i class=\"material-icons\">info</i><span>Xem chi tiết</span></button></td>";
                kq += "</tr>";
            }
            return kq;
        }

        /// <summary>
        /// Hàm tạo danh sách các sản phẩm trong hóa đơn khi ajax vào nút xem chi tiết
        /// </summary>
        /// <param name="maHD">Mã hóa đơn cần xem chi tiết</param>
        /// <returns>html tạo modal chi tiết</returns>
        public string AjaxXemChiTietHoaDonOrder(int maHD)
        {
            string htmlDetails = "";
            if (xulyChung.duocTruyCap(idOfPage))
            {
                try
                {
                    hoaDonOrder hoaDon = new qlCaPheEntities().hoaDonOrders.SingleOrDefault(h => h.maHoaDon == maHD);
                    if (hoaDon != null)
                    {
                        htmlDetails += "<div class=\"modal-header\">";
                        htmlDetails += "      <button type=\"button\" class=\"close\" data-dismiss=\"modal\">×</button>";
                        htmlDetails += "    <h3 class=\"modal-title\" id=\"largeModalLabel\">XEM CHI TIẾT HÓA ĐƠN \"" + hoaDon.maHoaDon.ToString() + "\" </h3>";
                        htmlDetails += "</div>";
                        htmlDetails += "<div class=\"modal-body\">";
                        htmlDetails += "    <div class=\"row\">";
                        htmlDetails += "        <div class=\"col-md-12 col-lg-12\">";
                        htmlDetails += "            <div class=\"card\">";
                        htmlDetails += "                <div class=\"header bg-cyan\">";
                        htmlDetails += "                    <h2>Danh mục món cần phục vụ cho bàn cho bàn " + xulyDuLieu.traVeKyTuGoc(hoaDon.BanChoNgoi.tenBan) + "</h2>";
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
                        htmlDetails += "        <a class=\"btn btn-default waves-effect\"  data-dismiss=\"modal\"><i class=\"material-icons\">close</i>Đóng lại</a>";
                        htmlDetails += "    </div>";
                        htmlDetails += "</div>";
                        xulyChung.ghiNhatKyDtb(5, "Chi tiết hóa đơn \"" + hoaDon.maHoaDon.ToString()+ " \"");
                    }
                }
                catch (Exception ex)
                {
                    xulyFile.ghiLoi("Class: PhucVuController - Function: AjaxXemChiTietCacSanPhamCanPhucVuc", ex.Message);
                }
            }
            return htmlDetails;
        }


        /// <summary>
        /// hàm tạo bảng danh sách các món có trong hoaDonOrder
        /// </summary>
        /// <param name="hoaDon">Hóa đơn cần xem chi tiết</param>
        /// <returns>html vùng table danh sách các món trên modal</returns>
        private string taoBangChiTietHoaDon(hoaDonOrder hoaDon)
        {
            string html = "";
            html += "<table class=\"table table-hover\">";
            html += "   <thead>";
            html += "       <tr>";
            html += "           <th style=\"width:50%\">Tên món</th>";
            html += "           <th width=\"25%\">Số lượng</th>";
            html += "           <th width=\"25%\">Đơn giá</th>";
            html += "       </tr>";
            html += "   </thead>";
            html += "   <tbody>";
            //-------Lặp qua danh sách các món trong ctHoaDonTam chưa pha chế
            foreach (ctHoaDonOrder ct in hoaDon.ctHoaDonOrders.ToList())
            {
                html += "       <tr>";
                html += "           <td>";
                html += "               <img width=\"50px;\" height=\"50px;\" src=\"" + xulyDuLieu.chuyenByteHinhThanhSrcImage(ct.sanPham.hinhAnh) + "\">";
                html += "               <b>" + xulyDuLieu.traVeKyTuGoc(ct.sanPham.tenSanPham) + "</b>";
                html += "           </td>";
                html += "           <td>" + ct.soLuong.ToString() + "</td>";
                html += "           <td>" + xulyDuLieu.doiVND(ct.donGia) + "</td>";
                html += "       </tr>";
            }
            html += "   </tbody>";
            html += "</table>";
            return html;
        }
                
        #endregion

    }
}