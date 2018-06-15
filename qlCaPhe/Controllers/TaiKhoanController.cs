using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using qlCaPhe.Models;
using qlCaPhe.App_Start;

namespace qlCaPhe.Controllers
{
    public class TaiKhoanController : Controller
    {
        private static string idOfPage = "202";
        #region CREATE
        /// <summary>
        /// Hàm tạo giao diện tạo mới tài khoản thành viên
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult tk_TaoMoiTaiKhoan()
        {
            if (xulyChung.duocTruyCap(idOfPage))
            {
                taoDuLieuChoCbb(new qlCaPheEntities(), 0, 0);
                //Nhúng script ajax lấy thông tin thành viên
                ViewBag.ScriptAjax = createScriptAjax.scriptGetInfoComboboxClick("cbbThanhVien", "ThanhVien/getInfoThanhVienForCreateTaiKhoan?maTV=", "vungThongTin");
                xulyChung.ghiNhatKyDtb(1, "Tạo mới tài khoản");
            }
            return View();
        }
        /// <summary>
        /// Hàm thực hiện thêm mới 1 tài khoản vào CSDL
        /// </summary>
        /// <param name="f"></param>
        /// <param name="tk"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult tk_TaoMoiTaiKhoan(FormCollection f, taiKhoan tk)
        {
            if (xulyChung.duocCapNhat(idOfPage, "7"))
            {
                string noiDungTB = ""; int kqLuu = 0;
                qlCaPheEntities db = new qlCaPheEntities();
                try
                {
                    layDuLieuTuView(f, tk);
                    kiemTraTaiKhoanTrung(db, tk.tenDangNhap);
                    db.taiKhoans.Add(tk);
                    kqLuu = db.SaveChanges();
                    if (kqLuu > 0)
                    {
                        noiDungTB = createHTML.taoNoiDungThongBao("Tài khoản", tk.tenDangNhap, "/TaiKhoan/RouteTaiKhoanHoatDong");
                        xulyChung.ghiNhatKyDtb(2, "Tài khoản \" " + xulyDuLieu.traVeKyTuGoc(tk.tenDangNhap) + " \"");
                    }
                }
                catch (Exception ex)
                {
                    noiDungTB = ex.Message;
                    this.doDuLieuLenView(tk);
                    xulyFile.ghiLoi("Class TaiKhoanController - Function: tk_TaoMoiTaiKhoan_Post", ex.Message);
                }
                //Nhúng script ajax lấy thông tin thành viên
                ViewBag.ScriptAjax = createScriptAjax.scriptGetInfoComboboxClick("cbbThanhVien", "ThanhVien/getInfoThanhVienForCreateTaiKhoan?maTV=", "vungThongTin");
                ViewBag.ThongBao = createHTML.taoThongBaoLuu(noiDungTB);
            }
            return View();
        }
        #endregion
        #region READ

        /// <summary>
        /// hàm vào danh sách tài khoản được phép hoạt động
        /// </summary>
        /// <returns></returns>
        public ActionResult RouteTaiKhoanHoatDong()
        {
            xulyChung.cauHinhSession("tk_TableTaiKhoan", "1");
            xulyChung.ghiNhatKyDtb(1, "Danh mục tài khoản còn hoạt động");
            return RedirectToAction("tk_TableTaiKhoan");
        }
        /// <summary>
        /// Hàm vào danh sách tài khoản bị cấm truy cập
        /// </summary>
        /// <returns></returns>
        public ActionResult RouteTaiKhoanCamTruyCap()
        {
            xulyChung.cauHinhSession("tk_TableTaiKhoan", "0");
            xulyChung.ghiNhatKyDtb(1, "Danh mục tài khoản cấm hoạt động");
            return RedirectToAction("tk_TableTaiKhoan");
        }
        /// <summary>
        /// Hàm tạo giao diện danh sách tài khoản theo trạng thái
        /// </summary>
        /// <returns></returns>
        public ActionResult tk_TableTaiKhoan(int? page)
        {
            int trangHienHanh = (page ?? 1);
            if (xulyChung.duocTruyCap(idOfPage))
            {
                string htmlTable = "";
                try
                {
                    string urlAction = (string)Session["urlAction"];
                    if (urlAction.Length > 0)
                    {
                        urlAction = urlAction.Split('|')[1]; urlAction = urlAction.Replace("request=", "");
                        bool trangThai = urlAction.Equals("1") ? true : false; //-----Nếu request là 1 thì trạng thái true và ngược lại
                        this.thietLapThongSoChung(trangThai);

                        qlCaPheEntities db = new qlCaPheEntities();
                        int soPhanTu = db.taiKhoans.Where(t => t.trangThai == trangThai).Count();
                        ViewBag.PhanTrang = createHTML.taoPhanTrang(soPhanTu, trangHienHanh, "/TaiKhoan/tk_TableTaiKhoan");

                        List<taiKhoan> listTaiKhoan = db.taiKhoans.Where(t => t.trangThai == trangThai).OrderBy(t => t.tenDangNhap).Skip((trangHienHanh - 1) * createHTML.pageSize).Take(createHTML.pageSize).ToList();
                        foreach (taiKhoan tk in listTaiKhoan)
                            htmlTable += this.createRowTable(tk);
                    }
                    else throw new Exception("không có tham số ");
                }
                catch (Exception ex)
                {
                    xulyFile.ghiLoi("Class TaiKhoanController - Function: tk_TableTaiKhoan", ex.Message);
                    return RedirectToAction("PageNotFound", "Home");
                }
                ViewBag.TableData = htmlTable;
            }
            return View();
        }
        /// <summary>
        /// Hàm tạo một dòng dữ liệu trên bảng
        /// </summary>
        /// <param name="tk"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        private string createRowTable(taiKhoan tk)
        {
            string kq = "";
            kq += "<tr role=\"row\" class=\"odd\">";
            kq += "     <td>" + tk.tenDangNhap + "</td>";
            kq += "     <td>";
            kq += xulyDuLieu.traVeKyTuGoc(tk.nhomTaiKhoan.tenNhom);
            kq += "     </td>";
            kq += "     <td>" + xulyDuLieu.traVeKyTuGoc(tk.thanhVien.hoTV) + " " + xulyDuLieu.traVeKyTuGoc(tk.thanhVien.tenTV) + "</td>";
            kq += "     <td>" + (tk.trangThai ? "Được phép truy cập" : "Đã bị cấm") + "</td>";
            kq += "     <td>";
            kq += "         <div class=\"btn-group\">";
            kq += "             <button type=\"button\" class=\"btn btn-primary dropdown-toggle\" data-toggle=\"dropdown\">";
            kq += "                 Chức năng <span class=\"caret\"></span>";
            kq += "             </button>";
            kq += "             <ul class=\"dropdown-menu\" role=\"menu\">";
            kq += createTableData.taoNutChinhSua("/TaiKhoan/tk_ChinhSuaTaiKhoan", tk.tenDangNhap);
            kq += createTableData.taoNutCapNhat("TaiKhoan/capNhatTrangThai", tk.tenDangNhap, "col-orange", "spellcheck", "Cấm/Cho phép truy cập");
            kq += createTableData.taoNutXoaBo(tk.tenDangNhap);
            kq += "             </ul>";
            kq += "         </div>";
            kq += "     </td>";
            kq += "</tr>";
            return kq;
        }
        #endregion
        #region UPDATE
        /// <summary>
        /// Hàm thực hiện cập nhật lại trạng thái truy cập của 1 tài khoản
        /// </summary>
        public ActionResult capNhatTrangThai()
        {
            if (xulyChung.duocCapNhat(idOfPage, "7"))
            {
                try
                {
                    int kqLuu = 0;
                    string urlAction = (string)Session["urlAction"];
                    if (urlAction.Length > 0)
                    {
                        string tenTK = urlAction.Split('|')[1]; tenTK = tenTK.Replace("request=", "");
                        qlCaPheEntities db = new qlCaPheEntities();
                        taiKhoan taiKhoanSua = db.taiKhoans.SingleOrDefault(tk => tk.tenDangNhap == tenTK);
                        if (taiKhoanSua != null)
                        {
                            //------Xác định đúng tài khoản đang đăng nhập
                            taiKhoan tkLogin = (taiKhoan)Session["login"];
                            if (!taiKhoanSua.tenDangNhap.Equals(tkLogin.tenDangNhap))//-----Nếu tài khoản đăng nhập khác với tài khoản cần chuyển đổi 
                            {
                                bool trangThaiCu = taiKhoanSua.trangThai;
                                //Thực hiện cập nhật trạng thái ngược với trạng thái hiện hành
                                taiKhoanSua.trangThai = !trangThaiCu;
                                db.Entry(taiKhoanSua).State = System.Data.Entity.EntityState.Modified;
                                kqLuu = db.SaveChanges();
                                if (kqLuu > 0)
                                {
                                    xulyChung.ghiNhatKyDtb(4, "Cập nhật trạng thái của tài khoản \" " + xulyDuLieu.traVeKyTuGoc(tenTK) + " \"");
                                    if (trangThaiCu == true) //-------Chuyển đến danh sách tài khoản còn hoạt động
                                        return RedirectToAction("RouteTaiKhoanHoatDong");
                                    else
                                        return RedirectToAction("RouteTaiKhoanCamTruyCap");
                                }
                            }

                        }
                    }
                }
                catch (Exception ex)
                {
                    xulyFile.ghiLoi("Class: TaiKhoanController - Function: capNhatTrangThai", ex.Message);
                }
            }
            return RedirectToAction("RouteTaiKhoanHoatDong");
        }
        /// <summary>
        /// Hàm vào giao diện chỉnh sửa tài khoản
        /// </summary>
        /// <param name="f"></param>
        /// <param name="tenTK"></param>
        /// <returns></returns>
        public ActionResult tk_ChinhSuaTaiKhoan()
        {
            if (xulyChung.duocCapNhat(idOfPage, "7"))
            {
                try
                {
                    string urlAction = (string)Session["urlAction"];
                    if (urlAction.Length > 0)
                    {
                        string tenTK = urlAction.Split('|')[1];
                        qlCaPheEntities db = new qlCaPheEntities();
                        taiKhoan tkSua = db.taiKhoans.SingleOrDefault(t => t.tenDangNhap == tenTK.Replace("request=", ""));
                        if (tkSua != null)
                        {
                            this.doDuLieuLenView(tkSua);
                            xulyChung.ghiNhatKyDtb(1, "Chỉnh sửa tài khoản \" " + xulyDuLieu.traVeKyTuGoc(tkSua.tenDangNhap) + " \"");
                        }
                        else throw new Exception("Tài khoản " + tenTK + " không tồn tại");
                    }
                    else throw new Exception("không nhận được tham số");
                }
                catch (Exception ex)
                {
                    xulyFile.ghiLoi("Class: TaiKhoanController - Function: tk_ChinhSuaTaiKhoan_get", ex.Message);
                    return RedirectToAction("ServerError", "Home");
                }
                ViewBag.ScriptAjax = createScriptAjax.scriptGetInfoComboboxClick("cbbThanhVien", "ThanhVien/getInfoThanhVienForCreateTaiKhoan?maTV=", "vungThongTin");
            }
            return View();
        }
        /// <summary>
        /// Hàm thực hiện cập nhật tài khoản trong CSDL
        /// </summary>
        /// <param name="tenTK"></param>
        /// <param name="f"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult tk_ChinhSuaTaiKhoan(FormCollection f)
        {
            string ndThongBao = ""; int kqLuu = 0;
            taiKhoan taiKhoanSua = new taiKhoan();
            qlCaPheEntities db = new qlCaPheEntities();
            try
            {
                string tenTK = f["txtTenTK"];
                taiKhoanSua = db.taiKhoans.Single(n => n.tenDangNhap == tenTK);
                this.layDuLieuTuView(f, taiKhoanSua);
                db.Entry(taiKhoanSua).State = System.Data.Entity.EntityState.Modified;
                kqLuu = db.SaveChanges();
                if (kqLuu > 0)
                {
                    xulyChung.ghiNhatKyDtb(4, "Tài khoản \" " + xulyDuLieu.traVeKyTuGoc(taiKhoanSua.tenDangNhap) + " \"");
                    return RedirectToAction("RouteTaiKhoanHoatDong");
                }
            }
            catch (Exception ex)
            {
                ndThongBao = ex.Message;
                this.doDuLieuLenView(taiKhoanSua);
                xulyFile.ghiLoi("Class: TaiKhoanController - Function: tk_ChinhSuaTaiKhoan_Post", ex.Message);
                ViewBag.ScriptAjax = createScriptAjax.scriptGetInfoComboboxClick("cbbThanhVien", "ThanhVien/getInfoThanhVienForCreateTaiKhoan?maTV=", "vungThongTin");
            }
            ViewBag.ThongBao = createHTML.taoThongBaoLuu(ndThongBao);
            return View();

        }
        #endregion
        #region DELETE
        /// <summary>
        /// Hàm thực hiện xóa 1 tài khoản khỏi csdl
        /// </summary>
        /// <param name="tenTK">Tên tài khoản cần xóa</param>
        public void xoaTaiKhoan(string tenTK)
        {
            if (xulyChung.duocCapNhat(idOfPage, "7"))
                try
                {
                    int kqLuu = 0;
                    qlCaPheEntities db = new qlCaPheEntities();
                    var taiKhoanXoa = db.taiKhoans.First(tk => tk.tenDangNhap == tenTK);
                    if (taiKhoanXoa != null)
                    {
                        //------------------KIỂM TRA SESSION TRÁNH XÓA TÀI KHOẢN ĐANG ĐĂNG NHẬP
                        taiKhoan tkLogin = (taiKhoan)Session["login"];
                        if (tkLogin.tenDangNhap != taiKhoanXoa.tenDangNhap)
                        {
                            db.taiKhoans.Remove(taiKhoanXoa);
                            kqLuu = db.SaveChanges();
                            if (kqLuu > 0)
                                xulyChung.ghiNhatKyDtb(3, "Tài khoản \"" + xulyDuLieu.traVeKyTuGoc(taiKhoanXoa.tenDangNhap) + " \"");
                        }
                    }
                }
                catch (Exception ex)
                {
                    xulyFile.ghiLoi("Class: TaiKhoanController - Function: xoaTaiKhoan", ex.Message);
                }
        }
        #endregion
        #region ORTHERS
        /// <summary>
        /// hàm thực hiện lấy dữ liệu mà người dùng nhập vào từ giao diện
        /// </summary>
        /// <param name="f"></param>
        /// <param name="tk"></param>
        private void layDuLieuTuView(FormCollection f, taiKhoan tk)
        {
            string loi = "";
            tk.maTV = xulyDuLieu.doiChuoiSangInteger(f["cbbThanhVien"]);
            if (tk.maTV <= 0)
                loi += "Vui lòng chọn thành viên cần cung cấp tài khoản<br/>";
            tk.maNhomTK = xulyDuLieu.doiChuoiSangInteger(f["cbbNhomTK"]);
            if (tk.maNhomTK <= 0)
                loi += "Vui lòng chọn nhóm tài khoản <br/>";
            tk.tenDangNhap = f["txtTenDangNhap"].Trim().ToLower();
            if (tk.tenDangNhap.Length < 5)
                loi += "Vui lòng nhập tên đăng nhập có ít nhất 5 ký tự và không khoảng trắng<br/>";
            if (f["txtMatKhau"].Trim().Length < 8)
                loi += "Vui lòng nhập mật khẩu có ít nhất 8 ký tự và không khoảng trắng<br/>";
            else
            {
                if (!f["txtMatKhau"].Equals(f["txtXacNhan"]))
                    loi += "Mật khẩu không trùng khớp, vui lòng nhập lại <br/>";
                tk.matKhau = xulyMaHoa.Encrypt(f["txtMatKhau"]);
            }
            tk.trangThai = true;
            tk.ghiChu = xulyDuLieu.xulyKyTuHTML(f["txtGhiChu"]);
            if (loi.Length > 0)
                throw new Exception(loi);
        }
        /// <summary>
        /// Hàm thực hiện kiểm tra xem tài khoản có bị trùng khi tạo mới
        /// </summary>
        /// <param name="db"></param>
        /// <param name="tenDN"></param>
        private void kiemTraTaiKhoanTrung(qlCaPheEntities db, string tenDN)
        {
            try
            {
                //Kiểm tra tài khoản có trùng.
                var taiKhoanTrung = db.taiKhoans.FirstOrDefault(m => m.tenDangNhap == tenDN);
                if (taiKhoanTrung != null)
                    throw new Exception("Tài khoản <b>" + tenDN + "</b> đã tồn tại, vui lòng nhập lại một tài khoản mới");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        /// <summary>
        /// Hàm tạo dữ liệu combobox trên giao diện
        /// </summary>
        /// <param name="db">Object để truy xuất và dtb lấy danh sách nhóm tài khoản và thành viên</param>
        /// <param name="maTV">Mã thành viên để xác định thành viên lựa chọn <para/> 0: không lựa chọn</param>
        /// <param name="maNhom">Mã nhóm để xác định nhóm tài khoản lựa chọn <para/> 0: không lựa chọn</param>
        private void taoDuLieuChoCbb(qlCaPheEntities db, int maTV, int maNhom)
        {
            //Hiển thị dropdownlist lựa chọn nhóm tài khoản
            string htmlCbbNhomTK = "";
            foreach (nhomTaiKhoan n in db.nhomTaiKhoans.ToList())
            {
                htmlCbbNhomTK += "<option ";
                if (n.maNhomTK == maNhom)
                    htmlCbbNhomTK += " selected ";
                htmlCbbNhomTK += " value=\"" + n.maNhomTK.ToString() + "\">" + xulyDuLieu.traVeKyTuGoc(n.tenNhom) + "</option>";
            }
            ViewBag.cbbNhomTK = htmlCbbNhomTK;

            //Hiển thị dropdownList lựa chọn thành viên
            string htmlCbbThanhVien = "";
            foreach (thanhVien tv in db.thanhViens.ToList())
            {
                htmlCbbThanhVien += "<option ";
                if (tv.maTV == maTV)
                    htmlCbbThanhVien += " selected ";
                htmlCbbThanhVien += " class=\"chonTV\" value=\"" + tv.maTV.ToString() + "\" maLay=\"" + tv.maTV.ToString() + "\">" + xulyDuLieu.traVeKyTuGoc(tv.hoTV) + " " + xulyDuLieu.traVeKyTuGoc(tv.tenTV) + "</option>";
            }
            ViewBag.cbbThanhVien = htmlCbbThanhVien;
        }
        /// <summary>
        /// Hàm thực hiện hiển thị thông tin tài khoản lên giao diện
        /// </summary>
        /// <param name="tenTK"></param>
        private void doDuLieuLenView(taiKhoan tk)
        {
            try
            {
                qlCaPheEntities db = new qlCaPheEntities();
                //------------Hiện thông tin của tài khoản-------------
                ViewBag.txtTenTK = tk.tenDangNhap;
                ViewBag.TenDangNhap = tk.tenDangNhap;
                ViewBag.GhiChu = xulyDuLieu.traVeKyTuGoc(tk.ghiChu);
                if (tk.maTV > 0)
                {
                    ViewBag.VungThanhVien = new ThanhVienController().getInfoThanhVienForCreateTaiKhoan(tk.maTV);
                    if (tk.maNhomTK > 0)
                        //---------Cấu hình cbb lựa chọn thành viên và nhóm tài khoản
                        this.taoDuLieuChoCbb(db, tk.maTV, tk.maNhomTK);
                    else
                        //---------Cấu hình cbb lựa chọn thành viên
                        this.taoDuLieuChoCbb(db, tk.maTV, 0);
                }
                else
                    this.taoDuLieuChoCbb(db, 0, 0);

            }
            catch (Exception ex)
            {
                xulyFile.ghiLoi("Class: TaiKhoanController - Function: doDuLieuLenView", ex.Message);
            }
        }
        /// <summary>
        /// Hàm tạo nội dung cho trang tùy thuộc vào trạng thái danh sách cần lấy
        /// </summary>
        /// <param name="trangThai"></param>
        private void thietLapThongSoChung(bool trangThai)
        {
            ViewBag.ScriptAjax = createScriptAjax.scriptAjaxXoaDoiTuong("TaiKhoan/xoaTaiKhoan?tenTK=");
            ViewBag.ModalXoa = createHTML.taoCanhBaoXoa("Tài khoản");
            if (trangThai)//Đối với danh sách tài khoản còn sử dụng
            {
                ViewBag.Style1 = "active";//Thiết lập css chọn tab này
                ViewBag.Style2 = "null"; //reset lại css cho tab
            }
            else
            {
                ViewBag.Style1 = "null";//Thiết lập css chọn tab này
                ViewBag.Style2 = "active"; //reset lại css cho tab
            }
        }
        #endregion
    }
}