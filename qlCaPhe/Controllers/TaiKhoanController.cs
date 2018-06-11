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
                taoDuLieuChoCbb(new qlCaPheEntities());
                //Nhúng script ajax lấy thông tin thành viên
                ViewBag.ScriptAjax = createScriptAjax.scriptGetInfoComboboxClick("cbbThanhVien", "ThanhVien/getInfoThanhVienForCreateTaiKhoan?maTV=", "vungThongTin");
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
            if (xulyChung.duocCapNhat(idOfPage,"7"))
            {
                string noiDungTB = "";
                qlCaPheEntities db = new qlCaPheEntities();
                try
                {
                    layDuLieuTuView(f, tk);
                    kiemTraTaiKhoanTrung(db, tk.tenDangNhap);
                    db.taiKhoans.Add(tk);
                    db.SaveChanges();
                    noiDungTB = createHTML.taoNoiDungThongBao("Tài khoản", tk.tenDangNhap, "/TaiKhoan/tk_TableTaiKhoan?trangThai=true");
                }
                catch (Exception ex)
                {
                    noiDungTB = ex.Message;
                    this.doDuLieuLenView(tk);
                    taoDuLieuChoCbb(new qlCaPheEntities());
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
        /// Hàm thiết lập session khi vào trang
        /// </summary>
        /// <param name="page"></param>
        /// <param name="request"></param>
        private void cauHinhSession(string page, string request)
        {
            string param = "page=" + page + "|request=" + request;
            //------Thiết lập lại session
            Session.Remove("urlAction"); Session.Add("urlAction", "");
            Session["urlAction"] = param;
        }
        /// <summary>
        /// hàm vào danh sách tài khoản được phép hoạt động
        /// </summary>
        /// <returns></returns>
        public ActionResult RouteTaiKhoanHoatDong()
        {
            cauHinhSession("tk_TableTaiKhoan", "1");
            return RedirectToAction("tk_TableTaiKhoan");
        }
        /// <summary>
        /// Hàm vào danh sách tài khoản bị cấm truy cập
        /// </summary>
        /// <returns></returns>
        public ActionResult RouteTaiKhoanCamTruyCap()
        {
            cauHinhSession("tk_TableTaiKhoan", "0");
            return RedirectToAction("tk_TableTaiKhoan");
        }
        /// <summary>
        /// Hàm tạo giao diện danh sách tài khoản theo trạng thái
        /// </summary>
        /// <returns></returns>
        public ActionResult tk_TableTaiKhoan(int ?page)
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
            if (xulyChung.duocCapNhat(idOfPage,"7"))
            {
                try
                {
                    string urlAction = (string)Session["urlAction"];
                    if (urlAction.Length > 0)
                    {
                        string tenTK = urlAction.Split('|')[1]; tenTK = tenTK.Replace("request=", "");
                        qlCaPheEntities db = new qlCaPheEntities();
                        taiKhoan taiKhoanSua = db.taiKhoans.SingleOrDefault(tk => tk.tenDangNhap == tenTK);
                        bool trangThaiCu = taiKhoanSua.trangThai;
                        //Thực hiện cập nhật trạng thái ngược với trạng thái hiện hành
                        taiKhoanSua.trangThai = !trangThaiCu;
                        db.Entry(taiKhoanSua).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                        if (trangThaiCu == true) //-------Chuyển đến danh sách tài khoản còn hoạt động
                            return RedirectToAction("RouteTaiKhoanHoatDong");
                        else
                            return RedirectToAction("RouteTaiKhoanCamTruyCap");
                    }
                }
                catch (Exception ex)
                {
                    xulyFile.ghiLoi("Class: TaiKhoanController - Function: capNhatTrangThai", ex.Message);
                }
            }
            return RedirectToAction("ServerError", "Home");
        }
        /// <summary>
        /// Hàm vào giao diện chỉnh sửa tài khoản
        /// </summary>
        /// <param name="f"></param>
        /// <param name="tenTK"></param>
        /// <returns></returns>
        public ActionResult tk_ChinhSuaTaiKhoan()
        {
            if (xulyChung.duocCapNhat(idOfPage,"7"))
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
                            this.taoDuLieuCbbChinhSua(db, tkSua.maTV, tkSua.maNhomTK);
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
            string ndThongBao = "";
            taiKhoan taiKhoanSua = new taiKhoan();
            qlCaPheEntities db = new qlCaPheEntities();
            try
            {
                string tenTK = f["txtTenTK"];
                taiKhoanSua = db.taiKhoans.Single(n => n.tenDangNhap == tenTK);
                this.layDuLieuTuView(f, taiKhoanSua);
                db.Entry(taiKhoanSua).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("RouteTaiKhoanHoatDong");
            }
            catch (Exception ex)
            {
                ndThongBao = ex.Message;
                this.doDuLieuLenView(taiKhoanSua);
                this.taoDuLieuCbbChinhSua(db, taiKhoanSua.maTV, taiKhoanSua.maNhomTK);
                xulyFile.ghiLoi("Class: TaiKhoanController - Function: tk_ChinhSuaTaiKhoan_Post", ex.Message);
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
            try
            {
                qlCaPheEntities db = new qlCaPheEntities();
                var taiKhoanXoa = db.taiKhoans.First(tk => tk.tenDangNhap == tenTK);
                //------------------KIỂM TRA SESSION TRÁNH XÓA TÀI KHOẢN ĐANG ĐĂNG NHẬP
                db.taiKhoans.Remove(taiKhoanXoa);
                db.SaveChanges();
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





            //string loi = "";
            //string tenDN = f["txtTenDangNhap"];
            //if (tenDN.Length < 5)
            //    loi += "Vui lòng nhập tên đăng nhập có ít nhất 5 ký tự";
            //else
            //    tk.tenDangNhap = tenDN.Trim().ToLower();
            //string mk = f["txtMatKhau"];
            //if (mk.Length <= 8)
            //    loi += "Vui lòng nhập mật khẩu có ít nhất 8 ký tự";
            //else
            //{
            //    if (!mk.Equals(f["txtXacNhan"]))
            //        loi += "Mật khẩu không trùng khớp, vui lòng nhập lại <br/>";
            //    tk.matKhau = xulyMaHoa.Encrypt(mk);
            //}
            //int maTV = int.Parse(f["cbbThanhVien"]);
            //if (maTV <= 0)
            //    loi += "Vui lòng chọn thành viên";
            //tk.maTV = maTV;
            //int maNhomTK = int.Parse(f["cbbNhomTK"]);
            //if (maNhomTK <= 0)
            //    loi += "Vui lòng chọn nhóm tài khoản";
            //tk.maNhomTK = maNhomTK;
            //tk.trangThai = true;
            //tk.ghiChu = xulyDuLieu.xulyKyTuHTML(f["txtGhiChu"]);
            //if (loi.Length > 0)
            //    throw new Exception(loi);
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
        /// hàm thực hiện hiện dữ liệu lên combobox
        /// </summary>
        /// <param name="db"></param>
        private void taoDuLieuChoCbb(qlCaPheEntities db)
        {
            //Hiển thị dropdownList lựa chọn thành viên
            string htmlCbbThanhVien = "";
            foreach (thanhVien tv in db.thanhViens.ToList())
                htmlCbbThanhVien += "<option class=\"chonTV\" value=\"" + tv.maTV.ToString() + "\" maLay=\"" + tv.maTV.ToString() + "\">" + xulyDuLieu.traVeKyTuGoc(tv.hoTV) + " " + xulyDuLieu.traVeKyTuGoc(tv.tenTV) + "</option>";
            htmlCbbThanhVien += "</select>";
            ViewBag.cbbThanhVien = htmlCbbThanhVien;

            //Hiển thị dropdownlist lựa chọn nhóm tài khoản
            string htmlCbbNhomTK = "";
            foreach (nhomTaiKhoan n in db.nhomTaiKhoans.ToList())
                htmlCbbNhomTK += "<option value=\"" + n.maNhomTK.ToString() + "\">" + xulyDuLieu.traVeKyTuGoc(n.tenNhom) + "</option>";
            ViewBag.cbbNhomTK = htmlCbbNhomTK;
            //Nhúng script ajax lấy thông tin thành viên
            ViewBag.ScriptAjax = createScriptAjax.taoScript("tk_TaoMoiTaiKhoan");

        }
        /// <summary>
        /// Hàm tạo dữ liệu combobox cho trang chỉnh sửa tài khoản
        /// </summary>
        /// <param name="db"></param>
        /// <param name="maTV"></param>
        /// <param name="maNhom"></param>
        private void taoDuLieuCbbChinhSua(qlCaPheEntities db, int maTV, int maNhom)
        {
            //Hiển thị dropdownlist lựa chọn nhóm tài khoản
            string htmlCbbNhomTK = "";
            foreach (nhomTaiKhoan n in db.nhomTaiKhoans.ToList())
                if (n.maNhomTK == maNhom)
                    htmlCbbNhomTK += "<option selected value=\"" + n.maNhomTK.ToString() + "\">" + xulyDuLieu.traVeKyTuGoc(n.tenNhom) + "</option>";
                else
                    htmlCbbNhomTK += "<option value=\"" + n.maNhomTK.ToString() + "\">" + xulyDuLieu.traVeKyTuGoc(n.tenNhom) + "</option>";
            ViewBag.cbbNhomTK = htmlCbbNhomTK;

            //Hiển thị dropdownList lựa chọn thành viên
            string htmlCbbThanhVien = "";
            thanhVien tv = db.thanhViens.Single(t => t.maTV == maTV);
            //kiểm tra nếu duyệt qua trùng mã thành viên thì đặt thuộc tính selected
            if (tv.maTV == maTV)
                htmlCbbThanhVien += "<option selected class=\"chonTV\" value=\"" + tv.maTV.ToString() + "\" maLay=\"" + tv.maTV.ToString() + "\">" + xulyDuLieu.traVeKyTuGoc(tv.hoTV) + " " + xulyDuLieu.traVeKyTuGoc(tv.tenTV) + "</option>";
            htmlCbbThanhVien += "</select>";
            ViewBag.cbbThanhVien = htmlCbbThanhVien;
        }
        /// <summary>
        /// Hàm thực hiện hiển thị thông tin tài khoản lên giao diện
        /// </summary>
        /// <param name="tenTK"></param>
        private void doDuLieuLenView(taiKhoan tk)
        {
            //------------Hiện thông tin của tài khoản-------------
            ViewBag.txtTenTK = tk.tenDangNhap;
            ViewBag.TenDangNhap = tk.tenDangNhap;
            ViewBag.GhiChu = xulyDuLieu.traVeKyTuGoc(tk.ghiChu);
            ViewBag.HinhDD = xulyDuLieu.chuyenByteHinhThanhSrcImage(tk.thanhVien.hinhDD);
            ViewBag.TenTV = xulyDuLieu.traVeKyTuGoc(tk.thanhVien.hoTV) + " " + xulyDuLieu.traVeKyTuGoc(tk.thanhVien.tenTV);
            ViewBag.SDT = tk.thanhVien.soDT;
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