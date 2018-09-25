using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using qlCaPhe.App_Start;
using qlCaPhe.Models;
using System.IO;

namespace qlCaPhe.Controllers
{
    public class CauHinhController : Controller
    {
        private string idOfPage = "1201";
        #region UPDATE
        /// <summary>
        /// Hàm tạo giao diện thiết lập thông số chung
        /// </summary>
        /// <returns></returns>
        public ActionResult ch_ThietLapThongSoChung()
        {
            if (xulyChung.duocCapNhat(idOfPage, "7"))
                try
                {
                    this.resetDuLieu();
                    qlCaPheEntities db = new qlCaPheEntities();
                    cauHinh chSua = db.cauHinhs.First();
                    if (chSua != null)
                    {
                        this.doDuLieuLenView(chSua);
                        this.taoDuLieuChoCbbTaiKhoan(db);
                        xulyChung.ghiNhatKyDtb(1, "Thiết lập thông số chung");
                    }
                }
                catch (Exception ex)
                {
                    xulyFile.ghiLoi("Class: CauHinhController - Function: ch_ThietLapThongSoChung_Get", ex.Message);
                }
            return View();
        }
        /// <summary>
        /// Hàm cập nhật thông tin cấu hình vào CSDL
        /// </summary>
        /// <param name="f"></param>
        /// <param name="fileUpload"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ch_ThietLapThongSoChung(FormCollection f, HttpPostedFileBase fileUpload)
        {

            if (xulyChung.duocCapNhat(idOfPage, "7"))
            {
                string ndthongBao = ""; int kqLuu = 0;
                cauHinh chSua = new cauHinh();
                qlCaPheEntities db = new qlCaPheEntities();
                try
                {
                    chSua = db.cauHinhs.First();
                    this.doDuLieuLenView(chSua);
                    this.layDuLieuTuView(chSua, f, fileUpload);
                    db.Entry(chSua).State = System.Data.Entity.EntityState.Modified;
                    kqLuu=db.SaveChanges();
                    if (kqLuu > 0)
                    {
                        ndthongBao = "Cấu hình website thành công";
                        this.resetDuLieu();
                        xulyChung.ghiNhatKyDtb(4, ndthongBao);
                    }
                }
                catch (Exception ex)
                {
                    ndthongBao = ex.Message;
                    xulyFile.ghiLoi("Class: CauHinhController - Function: ch_ThietLapThongSoChung_Post", ex.Message);
                    this.resetDuLieu();
                }
                ViewBag.ThongBao = createHTML.taoThongBaoLuu(ndthongBao);
                this.doDuLieuLenView(chSua);
            }
            return View();
        }

        /// <summary>
        /// Hàm cập nhật thời điểm kiểm kê kho hàng
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ch_LenLichKiemKe(FormCollection f)
        {
            if (xulyChung.duocCapNhat(idOfPage, "7"))
            {
                string ndthongBao = ""; int kqLuu = 0;
                cauHinh chSua = new cauHinh();
                qlCaPheEntities db = new qlCaPheEntities();
                try
                {
                    chSua = db.cauHinhs.First();
                    this.layDuLieuTuViewKiemKe(chSua, f);
                    db.Entry(chSua).State = System.Data.Entity.EntityState.Modified;
                    kqLuu = db.SaveChanges();
                    if (kqLuu > 0)
                    {
                        //------Tạo thông báo đến thành viên kiểm kê
                        kqLuu+=  this.taoThongBaoNhacNhoKiemKeDtb(db, f["cbbTaiKhoan"], chSua);
                        if (kqLuu > 1)
                        {
                            ndthongBao = "Cấu hình kế hoạch kiểm kê thành công";
                            this.resetDuLieu();
                            xulyChung.ghiNhatKyDtb(4, ndthongBao);
                        }
                    }
                }
                catch (Exception ex)
                {
                    ndthongBao = ex.Message;
                    xulyFile.ghiLoi("Class: CauHinhController - Function: ch_ThietLapThongSoChung_Post", ex.Message);
                    this.resetDuLieu();
                }
                ViewBag.ThongBao = createHTML.taoThongBaoLuu(ndthongBao);
                this.doDuLieuLenView(chSua);
            }
            return View("ch_ThietLapThongSoChung");
        }
        #endregion
        #region ORTHERS
        /// <summary>
        /// Hàm thực hiện đổ dữ liệu có trong bảng cấu hình lên giao diện
        /// </summary>
        /// <param name="ch"></param>
        private void doDuLieuLenView(cauHinh ch)
        {
            ViewBag.txtTenQuan = xulyDuLieu.traVeKyTuGoc(ch.tenQuan);
            ViewBag.txtGioiThieu = xulyDuLieu.traVeKyTuGoc(ch.gioiThieu);
            ViewBag.txtDiaChi = xulyDuLieu.traVeKyTuGoc(ch.diaChi);
            ViewBag.txtHotline = xulyDuLieu.traVeKyTuGoc(ch.hotLine);
            ViewBag.txtEmail = xulyDuLieu.traVeKyTuGoc(ch.email);
            ViewBag.txtFacebook = xulyDuLieu.traVeKyTuGoc(ch.facebook);
            ViewBag.txtGooglePlus = xulyDuLieu.traVeKyTuGoc(ch.googlePlus);
            ViewBag.txtTwitter = xulyDuLieu.traVeKyTuGoc(ch.twitter);
            ViewBag.txtKinhDo = xulyDuLieu.traVeKyTuGoc(ch.kinhDo);
            ViewBag.txtViDo = xulyDuLieu.traVeKyTuGoc(ch.viDo);
            ViewBag.txtGhiChu = xulyDuLieu.traVeKyTuGoc(ch.ghiChu);
            if (ch.loGo != null)
                ViewBag.Logo = xulyDuLieu.chuyenByteHinhThanhSrcImage(ch.loGo);

        }
        /// <summary>
        /// hàm thực hiện lấy dữ liệu từ giao diện và gán cho các thuộc tính có trong cauHinh
        /// </summary>
        /// <param name="ch"></param>
        /// <param name="f"></param>
        /// <param name="fileUpload"></param>
        private void layDuLieuTuView(cauHinh ch, FormCollection f, HttpPostedFileBase fileUpload)
        {
            string loi = "";
            ch.tenQuan = xulyDuLieu.xulyKyTuHTML(f["txtTenQuan"]);
            if (ch.tenQuan.Length <= 0)
                loi += "Vui lòng nhập tên quán <br/>";
            ch.gioiThieu = xulyDuLieu.xulyKyTuHTML(f["txtGioiThieu"]);
            if (ch.gioiThieu.Length <= 0)
                loi += "Vui lòng nhập thông tin giới thiệu cho quán <br/>";
            ch.diaChi = xulyDuLieu.xulyKyTuHTML(f["txtDiaChi"]);
            if (ch.diaChi.Length <= 0)
                loi += "Vui lòng nhập địa chỉ của quán <br/>";
            ch.hotLine = xulyDuLieu.xulyKyTuHTML(f["txtHotline"]);
            ch.email = xulyDuLieu.xulyKyTuHTML(f["txtEmail"]);
            ch.facebook = xulyDuLieu.xulyKyTuHTML(f["txtFacebook"]);
            ch.googlePlus = xulyDuLieu.xulyKyTuHTML(f["txtGooglePlus"]);
            ch.twitter = xulyDuLieu.xulyKyTuHTML(f["txtTwitter"]);
            ch.kinhDo = xulyDuLieu.xulyKyTuHTML(f["txtKinhDo"]);
            ch.viDo = xulyDuLieu.xulyKyTuHTML(f["txtViDo"]);
            ch.ghiChu = xulyDuLieu.xulyKyTuHTML(f["txtGhiChu"]);
            string pathHinh = f["pathHinh"];
            //------Kiểm tra xem có chọn hình không. 
            if (!pathHinh.Equals("")) //Nếu có chọn hình  thì luu hình vào csdl
            {
                pathHinh = xulyMaHoa.Decrypt(pathHinh);//Giải mã lại chuỗi đường dẫn lưu hình ảnh trên đĩa đã được mã hóa trong ajax
                ch.loGo= xulyDuLieu.chuyenDoiHinhSangByteArray(pathHinh); //Lưu hình vào thuộc tính hinhAnh
            }
            if (loi.Length > 0)
                throw new Exception(loi);
        }
        /// <summary>
        /// Hàm thực hiện thiết lập lại các thông số để nhận hình logo
        /// </summary>
        private void resetDuLieu()
        {
            //Gán hình mặc định 
            ViewBag.Logo = "/images/gallery-upload-256.png";
            //Xóa tất cả các hình đã tải lên trong tập tin tạm            
            xulyFile.donDepTM(Server.MapPath("~/pages/temp/cauHinh"));
            //Nhúng script ajax thực hiện tải và cắt hình
            ViewBag.ScriptCropImage = createScriptAjax.scriptAjaxUpLoadAndCropImage("cauHinh", 120, 120, 120, 120);
            //Gán modal up và crop hình ảnh lên view
            ViewBag.modalChonHinh = createHTML.taoModalUpHinh();
        }

        /// <summary>
        /// Hàm tạo dữ liệu cho combobox tài khoản thành viên được phân công kiểm kê
        /// </summary>
        /// <param name="db"></param>
        /// <param name="maTV">Mã thành viên đã chọn </param>
        private void taoDuLieuChoCbbTaiKhoan(qlCaPheEntities db)
        {
            try
            {
                //-----Tạo dữ liệu combobox cho thành viên
                string html = "";
                foreach (taiKhoan tk in db.taiKhoans.ToList().Where(t => t.trangThai == true))
                    html += "<option value=\"" + xulyDuLieu.traVeKyTuGoc(tk.tenDangNhap) + "\">" + xulyDuLieu.traVeKyTuGoc(tk.tenDangNhap) + " - " + xulyDuLieu.traVeKyTuGoc(tk.thanhVien.hoTV + " " + tk.thanhVien.tenTV)+ "</option>";
                ViewBag.cbbTaiKhoan = html;
            }
            catch (Exception ex)
            {
                xulyFile.ghiLoi("Class: BangCongTacController - Function: taoDuLieuChoCbb", ex.Message);
            }
        }
        /// <summary>
        /// Hàm lấy dữ liệu tại form kế hoạch kiểm kho
        /// </summary>
        /// <param name="ch"></param>
        /// <param name="f"></param>
        private void layDuLieuTuViewKiemKe(cauHinh ch, FormCollection f)
        {
            string loi = "";
            ch.batDauKiem = xulyDuLieu.doiChuoiSangDateTime(f["txtNgayBatDau"]);
            if (ch.batDauKiem < DateTime.Now.Date)
                loi += "Ngày bắt đầu kiểm phải lớn hơn hoặc bằng ngày hiện tại";
            ch.ketThucKiem = xulyDuLieu.doiChuoiSangDateTime(f["txtNgayKetThuc"]);
            if (ch.ketThucKiem < DateTime.Now.Date)
                loi += "Ngày kết thúc phải lớn hơn ngày hiện tại";
            if (loi.Length > 0)
                throw new Exception(loi);
        }

        /// <summary>
        /// Hàm tạo thông báo nhắc nhở kiểm kê trong CSDL
        /// </summary>
        /// <param name="db"></param>
        /// <param name="tenTK">Tên tài khoản cần tạo thông báo</param>
        /// <param name="ch">Object cấu hình để lấy ngày bắt đầu và kết thúc</param>
        private int taoThongBaoNhacNhoKiemKeDtb(qlCaPheEntities db, string tenTK, cauHinh ch)
        {
            int kq = 0;
            try
            {
                if (tenTK.Length >= 5)
                {
                    thongBao tbAdd = new thongBao();
                    tbAdd.ndThongBao = "Đến đợt kiểm kê kho hàng " + ch.batDauKiem.ToString() + " - " + ch.ketThucKiem.ToString();
                    tbAdd.taiKhoan = tenTK;
                    tbAdd.ngayTao = DateTime.Now;
                    tbAdd.daXem = false;
                    tbAdd.ghiChu = "Thông báo kiểm kho";
                    db.thongBaos.Add(tbAdd);
                    kq=   db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                xulyFile.ghiLoi("Class: CauHinhController - Function: taoThongBaoNhacNhoKiemKeDtb", ex.Message);
            }
            return kq;
        }
        #endregion
    }
}