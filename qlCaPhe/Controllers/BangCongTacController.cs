using qlCaPhe.App_Start;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using qlCaPhe.Models;
using qlCaPhe.App_Start.Cart;

namespace qlCaPhe.Controllers
{
    public class BangCongTacController : Controller
    {

        private string idOfPage = "902";
        #region CREATE
        /// <summary>
        /// Hàm tạo giao diện điều phối nhân viên.
        /// </summary>
        /// <returns></returns>
        public ActionResult bct_TaoMoiDieuPhoi()
        {
            if (xulyChung.duocTruyCap(idOfPage))
            {
                try
                {
                    this.resetData();
                    this.taoDuLieuChoCbb(new qlCaPheEntities(), 0);
                    ViewBag.ScriptAjax = createScriptAjax.scriptGetInfoComboboxClick("cbbThanhVien", "ThanhVien/AjaxGetInforThanhVienByTaiKhoan?param=", "vungThanhVien");
                    xulyChung.ghiNhatKyDtb(1, "Tạo mới điều phối");
                }
                catch (Exception ex)
                {
                    xulyFile.ghiLoi("Class: BangCongTacController - Function: bct_TaoMoiDieuPhoi", ex.Message);
                }
            }
            return View();
        }
        /// <summary>
        /// Hàm thực hiện lưu mới điều phối công việc phân công cho nhân viên
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult bct_TaoMoiDieuPhoi(FormCollection f)
        {
            if (xulyChung.duocCapNhat(idOfPage, "7"))
            {
                string ndThongBao = ""; int kqLuu = 0;
                qlCaPheEntities db = null;
                BangGiaoViec bgv = new BangGiaoViec();
                try
                {
                    db = new qlCaPheEntities();

                    this.layDuLieuTrenView(bgv, f, db);
                    db.BangGiaoViecs.Add(bgv);
                    kqLuu = db.SaveChanges();

                    if (kqLuu > 0)
                    {
                        this.themCtBangGiaoViecVaoDatabase(bgv, db);
                        ndThongBao = createHTML.taoNoiDungThongBao("Phân công cho thành viên", xulyDuLieu.traVeKyTuGoc(bgv.tenDangNhap), "bct_TableDieuPhoi");
                        this.taoDuLieuChoCbb(db, 0);
                        xulyChung.ghiNhatKyDtb(2, "Điều phối công tác của \" " + xulyDuLieu.traVeKyTuGoc(bgv.tenDangNhap) + " \"");
                    }
                }
                catch (Exception ex)
                {
                    this.doDuLieuLenView(bgv, db);
                    ndThongBao = ex.Message;
                    xulyFile.ghiLoi("Class: BangCongTacController - Function: bct_TaoMoiDieuPhoi_Post", ex.Message);
                }
                ViewBag.ScriptAjax = createScriptAjax.scriptGetInfoComboboxClick("cbbThanhVien", "ThanhVien/AjaxGetInforThanhVienByTaiKhoan?param=", "vungThanhVien");
                ViewBag.ThongBao = createHTML.taoThongBaoLuu(ndThongBao);
            }
            return View();
        }
        /// <summary>
        /// Hàm thực hiện thêm chi tiết bảng giao việc trong giỏ vào database
        /// </summary>
        /// <param name="bgv"></param>
        /// <param name="db"></param>
        private void themCtBangGiaoViecVaoDatabase(BangGiaoViec bgv, qlCaPheEntities db)
        {
            try
            {
                cartDieuPhoi cart = (cartDieuPhoi)Session["dieuPhoi"];
                foreach (ctBangGiaoViec item in cart.Info.Values)
                {
                    ctBangGiaoViec ctAdd = new ctBangGiaoViec();
                    ctAdd.ghiChu = item.ghiChu;
                    ctAdd.maBang = bgv.maBang;
                    ctAdd.maCa = item.maCa;
                    db.ctBangGiaoViecs.Add(ctAdd);
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion
        #region READ
        /// <summary>
        /// Hàm tạo giao diện danh mục phân công công tác cho nhân viên
        /// </summary>
        /// <returns></returns>
        public ActionResult bct_TableDieuPhoi(int ?page)
        {
            if (xulyChung.duocTruyCap(idOfPage))
            {
                string htmlTable = ""; int trangHienHanh = (page ?? 1);
                try
                {
                    qlCaPheEntities db = new qlCaPheEntities();
                    int soPhanTu = db.BangGiaoViecs.Count();
                    ViewBag.PhanTrang = createHTML.taoPhanTrang(soPhanTu, createHTML.pageSize, trangHienHanh, "/BangCongTac/bct_TableDieuPhoi"); //------cấu hình phân trang
                    //--------Lặp qua danh sách bảng công tác được sắp xếp theo trạng thái. Hiển thị trạng thái "còn sử dụng" trước
                    foreach (BangGiaoViec bgv in db.BangGiaoViecs.ToList().OrderByDescending(c => c.trangThai).Skip((trangHienHanh - 1) * createHTML.pageSize).Take(createHTML.pageSize))
                    {
                        htmlTable += "<tr role=\"row\" class=\"odd\">";
                        htmlTable += "    <td>";
                        htmlTable += "        <a href=\"#\" data-toggle=\"modal\" data-target=\"#modalChiTiet\"";
                        htmlTable += "            class=\"goiY\" task=\"" + bgv.maBang.ToString() + "\">" + xulyDuLieu.traVeKyTuGoc(bgv.taiKhoan.tenDangNhap) + "<span class=\"noiDungGoiY-right\">Click để xem chi tiết</span></a>";
                        htmlTable += "    </td>";
                        htmlTable += "    <td>" + bgv.ngayLap.ToShortDateString() + "</td>";
                        htmlTable += "    <td>" + (bgv.trangThai ? "Còn sử dụng" : "Ngưng sử dụng") + "</td>";
                        htmlTable += "    <td>" + xulyDuLieu.traVeKyTuGoc(bgv.ghiChu) + "</td>";
                        htmlTable += "    <td>";
                        htmlTable += "        <div class=\"btn-group\">";
                        htmlTable += "            <button type=\"button\" class=\"btn btn-primary dropdown-toggle\" data-toggle=\"dropdown\" aria-expanded=\"true\">";
                        htmlTable += "                Chức năng <span class=\"caret\"></span>";
                        htmlTable += "            </button>";
                        htmlTable += "            <ul class=\"dropdown-menu\" role=\"menu\">";
                        htmlTable += createTableData.taoNutChinhSua("/BangCongTac/bct_ChinhSuaDieuPhoi", bgv.maBang.ToString());
                        htmlTable += createTableData.taoNutCapNhat("BangCongTac/capNhatTrangThai", bgv.maBang.ToString(), "col-orange", "clear", "Chuyển đổi");
                        htmlTable += createTableData.taoNutXoaBo(bgv.maBang.ToString());
                        htmlTable += "            </ul>";
                        htmlTable += "        </div>";
                        htmlTable += "    </td>";
                        htmlTable += "</tr>";
                    }
                    ViewBag.ScriptAjax = createScriptAjax.scriptAjaxXoaDoiTuong("BangCongTac/xoaDieuPhoi?maBang=");
                    ViewBag.ModalXoa = createHTML.taoCanhBaoXoa("Bảng công tác");
                    //----Nhúng script ajax hiển thị chi tiết hóa đơn khi người dùng click vào mã số hóa đơn trên bảng
                    ViewBag.ScriptAjaxXemChiTiet = createScriptAjax.scriptAjaxXemChiTietKhiClick("goiY", "task", "BangCongTac/AjaxXemChiTietDieuPhoi?param=", "vungChiTiet", "modalChiTiet");
                    //----Nhúng các tag html cho modal chi tiết
                    ViewBag.ModalChiTiet = createHTML.taoModalChiTiet("modalChiTiet", "vungChiTiet", 2);
                    xulyChung.ghiNhatKyDtb(1, "Danh mục bảng điều phối công tác");
                }
                catch (Exception ex)
                {
                    xulyFile.ghiLoi("Class: BangCongTacController - Function: bct_TableDieuPhoi", ex.Message);
                    return RedirectToAction("PageNotFound", "Home");
                }
                ViewBag.TableData = htmlTable;
            }
            return View();
        }
        #endregion
        #region UPDATE
        /// <summary>
        /// Hàm thực hiện cập nhật trạng thái của 1 công tác.
        /// Tham số là mã bảng được lưu trữ trong Session
        /// </summary>
        /// <returns>Trả về giao diện danh mục Bảng công tác</returns>
        public ActionResult capNhatTrangThai()
        {
            try
            {
                if (xulyChung.duocCapNhat(idOfPage, "7"))
                {
                    int kqLuu = 0;
                    string param = xulyChung.nhanThamSoTrongSession(); //param = maBang
                    if (param.Length > 0)
                    {
                        int maBang = xulyDuLieu.doiChuoiSangInteger(param);
                        qlCaPheEntities db = new qlCaPheEntities();
                        BangGiaoViec bgv = db.BangGiaoViecs.SingleOrDefault(s => s.maBang == maBang);
                        if (bgv != null)
                        {
                            bgv.trangThai = !bgv.trangThai;
                            db.Entry(bgv).State = System.Data.Entity.EntityState.Modified;
                            kqLuu=    db.SaveChanges();
                            if(kqLuu>0)
                                xulyChung.ghiNhatKyDtb(4, "Cập nhật trạng thái điều phối của\" " + xulyDuLieu.traVeKyTuGoc(bgv.tenDangNhap) + " \"");
                        }
                        else
                            throw new Exception("Bảng công tác có mã " + maBang.ToString() + " không tồn tại để cập nhật");
                    }
                    else throw new Exception("không nhận được tham số");
                }
            }
            catch (Exception ex)
            {
                xulyFile.ghiLoi("Class: BangCongTacController - Function: capNhatTrangThai", ex.Message);
            }
            return RedirectToAction("bct_TableDieuPhoi");
        }

        /// <summary>
        /// Hàm tạo giao diện form chỉnh sửa điều phối của nhân viên
        /// </summary>
        /// <returns></returns>
        public ActionResult bct_ChinhSuaDieuPhoi()
        {
            try
            {
                if (xulyChung.duocTruyCap(idOfPage))
                {
                    string param = xulyChung.nhanThamSoTrongSession(); //param = maBang;
                    if (param.Length > 0)
                    {
                        int maBang = xulyDuLieu.doiChuoiSangInteger(param);
                        qlCaPheEntities db = new qlCaPheEntities();
                        BangGiaoViec bgv = db.BangGiaoViecs.SingleOrDefault(s => s.maBang == maBang);
                        if (bgv != null)
                        {
                            //-----Đổ dữ liệu chi tiết lên giao diện
                            this.docChiTietVaThemVaoGio(bgv);
                            //-----Đổ thông tin điều phối lên giao diện
                            this.doDuLieuLenView(bgv, db);
                            //----Chèn script ajax hiện thông tin thành viên khi người dùng click chọn trên combobox
                            ViewBag.ScriptAjax = createScriptAjax.scriptGetInfoComboboxClick("cbbThanhVien", "ThanhVien/AjaxGetInforThanhVienByTaiKhoan?param=", "vungThanhVien");
                            xulyChung.ghiNhatKyDtb(1, "Chỉnh sửa điều phối của \" " + xulyDuLieu.traVeKyTuGoc(bgv.tenDangNhap) + " \"");
                        }
                        else throw new Exception("Bảng giao việc không tồn tại để cập nhật");
                    }
                    else throw new Exception("Chưa nhận được tham số");
                }
            }
            catch (Exception ex)
            {
                xulyFile.ghiLoi("Class: BangCongTacController - Function: capNhatTrangThai", ex.Message);
                return RedirectToAction("/Home/PageNotFound");
            }
            return View();
        }

        /// <summary>
        /// Hàm Thực hiện cập nhật lại Điều phối trong CSDL
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult bct_ChinhSuaDieuPhoi(FormCollection f)
        {
            string ndThongBao = ""; 
            qlCaPheEntities db = null;
            BangGiaoViec bgv = new BangGiaoViec();
            try
            {
                if (xulyChung.duocCapNhat(idOfPage, "7"))
                {
                    string param = xulyChung.nhanThamSoTrongSession(); //param = maBang;
                    if (param.Length > 0)
                    {
                        int maBang = xulyDuLieu.doiChuoiSangInteger(param), kqLuu=0;
                        db = new qlCaPheEntities();
                        bgv = db.BangGiaoViecs.SingleOrDefault(s => s.maBang == maBang);
                        if (bgv != null)
                        {
                            List<ctBangGiaoViec> listChiTiet = bgv.ctBangGiaoViecs.ToList();
                            this.layDuLieuTrenView(bgv, f, db);
                            //---------Cập nhật dự liệu trong bảng BangGiaoViec
                            db.Entry(bgv).State = System.Data.Entity.EntityState.Modified;
                            kqLuu = db.SaveChanges();
                            if (kqLuu > 0)
                            {
                                //-------Cập nhật bảng chi tiết. THỰC HIỆN XÓA DỮ LIỆU TRONG BẢNG CHI TIẾT VÀ TẠO MỚI LẠI
                                this.xoaChiTietDieuPhoi(db, listChiTiet);
                                this.themCtBangGiaoViecVaoDatabase(bgv, db);
                                xulyChung.ghiNhatKyDtb(4, "Điều phối công tác của \" " + xulyDuLieu.traVeKyTuGoc(bgv.tenDangNhap) + " \"");
                                return RedirectToAction("bct_TableDieuPhoi");
                            }
                        }
                        else throw new Exception("Bảng giao việc không tồn tại để cập nhật");
                    }
                    else throw new Exception("Chưa nhận được tham số");
                }
            }
            catch (Exception ex)
            {
                xulyFile.ghiLoi("Class: BangCongTacController - Function: capNhatTrangThai", ex.Message);
                ndThongBao = ex.Message;
                this.doDuLieuLenView(bgv, db);
            }
            ViewBag.ThongBao = createHTML.taoThongBaoLuu(ndThongBao);
            return View();
        }
        #endregion
        #region DELETE
        /// <summary>
        /// Hàm thực hiện xóa 1 điều phối khỏi CSDL
        /// Sau khi xóa sẽ thực hiện load lại danh sách bằng javascript
        /// </summary>
        /// <param name="maBang">Mã bảng điều phối cần xóa</param>
        public void xoaDieuPhoi(string maBang)
        {
            try
            {
                if (xulyChung.duocCapNhat(idOfPage, "7"))
                {
                    int maXoa = xulyDuLieu.doiChuoiSangInteger(maBang); int kqLuu = 0;
                    if (maXoa > 0)
                    {
                        qlCaPheEntities db = new qlCaPheEntities();
                        BangGiaoViec bgv = db.BangGiaoViecs.SingleOrDefault(b => b.maBang == maXoa);
                        if (bgv != null)
                        {
                            //-------Xóa tất cả chi tiết của điều phối trước.
                            List<ctBangGiaoViec> listChiTiet = bgv.ctBangGiaoViecs.ToList();
                            int soLuongChiTiet = listChiTiet.Count, chiTietDaXoa = 0;
                            if (listChiTiet.Count > 0)
                                chiTietDaXoa = xoaChiTietDieuPhoi(db, listChiTiet);
                            if (soLuongChiTiet == chiTietDaXoa)
                            {   //----Nếu đã xóa hết chi tiết thì xóa bảng DieuPhoi
                                db.BangGiaoViecs.Remove(bgv);
                                kqLuu= db.SaveChanges();
                                if(kqLuu>0)
                                    xulyChung.ghiNhatKyDtb(3, "Điều phối của  \"" + xulyDuLieu.traVeKyTuGoc(bgv.tenDangNhap) + " \"");
                            }
                        }
                        else throw new Exception("Bảng giao việc cần xóa không tồn tại");
                    }
                    else throw new Exception("Mã bảng giao việc cần xóa không chính xác");
                }
            }
            catch (Exception ex)
            {
                xulyFile.ghiLoi("Class: BangCongTacController - Function: capNhatTrangThai", ex.Message);
                Response.Redirect(xulyChung.layTenMien() + "/Home/PageNotFound");
            }
        }
        /// <summary>
        /// Hàm thực hiện xóa tất cả dữ liệu trong bảng chi tiết của 1 điều phối
        /// </summary>
        /// <param name="db"></param>
        /// <param name="bgv">Bảng điều phối có chi tiết cần xóa</param>
        private int xoaChiTietDieuPhoi(qlCaPheEntities db, List<ctBangGiaoViec> listChiTiet)
        {
            int kq = 0;
            //------Lặp qua danh sách các item chi tiết 
            foreach (ctBangGiaoViec ctXoa in listChiTiet)
            {
                db.ctBangGiaoViecs.Remove(ctXoa);
                kq += db.SaveChanges();
            }
            return kq;
        }
        #endregion
        #region ORTHERS
        /// <summary>
        /// Hàm thực hiện xóa dữ liệu trong session
        /// </summary>
        private void resetData()
        {
            Session.Remove("dieuPhoi");
            Session.Add("dieuPhoi", new cartDieuPhoi());
            ViewBag.txtGhiChu = "";
            ViewBag.hinhDD = "";
            ViewBag.cbbThanhVien = "";
            ViewBag.cbbCaLamViec = "";
        }

        /// <summary>
        /// Hàm tạo dữ liệu cho combobox 
        /// </summary>
        /// <param name="db"></param>
        /// <param name="maTV">Mã thành viên đã chọn </param>
        private void taoDuLieuChoCbb(qlCaPheEntities db, int maTV)
        {
            try
            {
                //-----Tạo dữ liệu combobox cho thành viên
                string html = "";
                foreach (taiKhoan tk in db.taiKhoans.ToList().Where(t => t.trangThai == true))
                    if (tk.thanhVien.maTV == maTV) //-------Nếu mã tài khoản đang duyệt đã chọn thì hiển lên combobobox
                        html += "<option selected value=\"" + xulyDuLieu.traVeKyTuGoc(tk.tenDangNhap) + "\">" + xulyDuLieu.traVeKyTuGoc(tk.tenDangNhap) + " - " + xulyDuLieu.traVeKyTuGoc(tk.thanhVien.hoTV) + " " + xulyDuLieu.traVeKyTuGoc(tk.thanhVien.tenTV) + "</option>";
                    else
                        html += "<option value=\"" + xulyDuLieu.traVeKyTuGoc(tk.tenDangNhap) + "\">" + xulyDuLieu.traVeKyTuGoc(tk.tenDangNhap) + " - " + xulyDuLieu.traVeKyTuGoc(tk.thanhVien.hoTV) + " " + xulyDuLieu.traVeKyTuGoc(tk.thanhVien.tenTV) + "</option>";
                ViewBag.cbbThanhVien = html;

                //-----Tạo dữ liệu combobox cho ca làm việc
                string htmlCa = "";
                foreach (caLamViec ca in db.caLamViecs.ToList().OrderBy(c => c.buoi).ThenBy(c => c.maCa))
                    htmlCa += "<option value=\"" + ca.maCa.ToString() + "\">" + xulyDuLieu.traVeKyTuGoc(ca.tenCa) + " (" + ca.batDau.ToString() + " - " + ca.ketThuc.ToString() + ")</option>";
                ViewBag.cbbCaLamViec = htmlCa;
            }
            catch (Exception ex)
            {
                xulyFile.ghiLoi("Class: BangCongTacController - Function: taoDuLieuChoCbb", ex.Message);
            }
        }

        /// <summary>
        /// Hàm thực hiện lấy dữ liệu có trên giao diện gán cho các thuộc tính của object BangGiaoViec
        /// </summary>
        /// <param name="bgv"></param>
        /// <param name="f"></param>
        private void layDuLieuTrenView(BangGiaoViec bgv, FormCollection f, qlCaPheEntities db)
        {
            bgv.ngayLap = DateTime.Now;
            bgv.ghiChu = xulyDuLieu.xulyKyTuHTML(f["txtGhiChu"]);
            bgv.trangThai = true;

            bgv.tenDangNhap = xulyDuLieu.xulyKyTuHTML(f["cbbThanhVien"]);
            if (bgv.tenDangNhap.Equals("-1"))
                throw new Exception("Vui lòng chọn tài khoản thành viên cần điều phối");
            bgv.taiKhoan = db.taiKhoans.SingleOrDefault(t => t.tenDangNhap.Equals(bgv.tenDangNhap));

            cartDieuPhoi cart = (cartDieuPhoi)Session["dieuPhoi"];
            if (cart.Info.Values.Count <= 0)
                throw new Exception("Vui lòng chọn ca cần điều phối cho nhân viên");
        }

        /// <summary>
        /// Hàm thực hiện đổ dữ liệu của bảng giao việc đã có sẵn lên view
        /// </summary>
        /// <param name="bgv">Object chứa các thông tin bảng giao việc</param>
        /// <param name="db"></param>
        private void doDuLieuLenView(BangGiaoViec bgv, qlCaPheEntities db)
        {
            try
            {
                ViewBag.txtGhiChu = xulyDuLieu.traVeKyTuGoc(bgv.ghiChu);
                ViewBag.VungChiTiet = this.taoGiaoDienChiTiet();
                if (bgv.taiKhoan != null)
                {
                    //-----------Hiển thị thông tin thành viên
                    ViewBag.hinhDD = xulyChung.DrawInforThanhVienWithTaiKhoan(bgv.taiKhoan);
                    this.taoDuLieuChoCbb(db, bgv.taiKhoan.maTV);
                }
                else
                    this.taoDuLieuChoCbb(db, 0);
            }
            catch (Exception ex)
            {
                xulyFile.ghiLoi("Class: BangCongTacController - Function: doDuLieuLenView", ex.Message);
            }
        }
        /// <summary>
        /// Hàm thực hiện đọc các thông tin chi tiết có trong database và thêm vào giỏ
        /// </summary>
        /// <param name="bgv"></param>
        private void docChiTietVaThemVaoGio(BangGiaoViec bgv)
        {
            try
            {
                this.resetData();
                cartDieuPhoi cart = (cartDieuPhoi)Session["dieuPhoi"];
                foreach (ctBangGiaoViec ct in bgv.ctBangGiaoViecs.ToList())
                {
                    ctBangGiaoViec ctTam = new ctBangGiaoViec();
                    ctTam.maCa = ct.maCa;
                    ctTam.BangGiaoViec = bgv;
                    ctTam.caLamViec = ct.caLamViec;
                    ctTam.ghiChu = ct.ghiChu;
                    ctTam.maBang = ct.maBang;
                    cart.addCart(ctTam);
                    Session["dieuPhoi"] = cart;
                }
            }
            catch (Exception ex)
            {
                xulyFile.ghiLoi("Class: BangCongTacController - Function: docChiTietVaThemVaoGio", ex.Message);
            }
        }
        #endregion
        #region AJAX
        #region NHÓM HÀM XỬ LÝ LỰA CHỌN CA LÀM VIỆC CẦN ĐIỀU PHỐI

        /// <summary>
        /// Hàm thực hiện nhận Ajax thêm mới 1 ca làm việc đã chọn vào giỏ
        /// </summary>
        /// <param name="param">Tham số truyền vào gồm param= maCa|ghiChuCt</param>
        /// <returns>Trả về chuỗi html tạo giao diện cho vùng chi tiết ca làm việc đã chọn</returns>
        public string AjaxThemMotCaLamViecVaoDieuPhoi(string param)
        {
            try
            {
                int maCa = xulyDuLieu.doiChuoiSangInteger(param.Split('|')[0]);
                string ghiChuCt = xulyDuLieu.xulyKyTuHTML(param.Split('|')[1]);
                if (maCa > 0)
                {
                    cartDieuPhoi cart = (cartDieuPhoi)Session["dieuPhoi"];
                    ctBangGiaoViec ctAdd = new ctBangGiaoViec();
                    ctAdd.caLamViec = new qlCaPheEntities().caLamViecs.SingleOrDefault(s => s.maCa == maCa);
                    ctAdd.maCa = maCa;
                    ctAdd.ghiChu = ghiChuCt;
                    cart.addCart(ctAdd);
                    Session["dieuPhoi"] = cart;
                }
            }
            catch (Exception ex)
            {
                xulyFile.ghiLoi("Class: BangCongTacController - Function: AjaxThemMotCaLamViecVaoDieuPhoi", ex.Message);
            }
            return taoGiaoDienChiTiet();
        }
        /// <summary>
        /// Hàm xừ lý sự kiện Ajax xóa bỏ 1 ca làm việc trong giỏ
        /// Xảy ra khi người dùng click vào nút "Bỏ ca này" trên giao diện
        /// </summary>
        /// <param name="param">Tham số chứa mã ca truyền vào </param>
        /// <returns>Chuỗi html tạo giao diện cho vùng "Danh sách ca đã chọn"</returns>
        public string AjaxXoaBoCaDaChon(string param)
        {
            try
            {
                int maCaXoa = xulyDuLieu.doiChuoiSangInteger(param);
                if (maCaXoa > 0)
                {
                    cartDieuPhoi cart = (cartDieuPhoi)Session["dieuPhoi"];
                    cart.removeItem(maCaXoa);
                    Session["dieuPhoi"] = cart;
                }
            }
            catch (Exception ex)
            {
                xulyFile.ghiLoi("Class: BangCongTacController - Function: AjaxXoaBoCaDaChon", ex.Message);
            }
            return taoGiaoDienChiTiet();
        }
        /// <summary>
        /// Hàm xử lý sự kiện Ajax xóa tất cả các ca làm việc có trong giỏ
        /// Sự kiện xảy ra khi người dùng click vào nút "Xóa tất cả ca đã chọn"
        /// </summary>
        public void AjaxXoaTatCaCaLamViecDaChon()
        {
            try
            {
                resetData();
            }
            catch (Exception ex)
            {
                xulyFile.ghiLoi("Class: BangCongTacController - Function: AjaxXoaTatCaCaLamViecDaChon", ex.Message);
            }
        }
        /// <summary>
        /// Hàm tạo giao diện cho vùng chi tiết ca làm việc tại giao diện nhập thông tin cho Điều phối
        /// </summary>
        /// <returns></returns>
        private string taoGiaoDienChiTiet()
        {

            string kq = "";
            try
            {
                cartDieuPhoi cart = (cartDieuPhoi)Session["dieuPhoi"];
                if (cart.Info.Values.Count > 0)
                {
                    kq += "<div class=\"cart\">";
                    kq += "     <div class=\"header\">";
                    kq += "         <h2>Danh sách ca làm việc đã chọn</h2>";
                    kq += "     </div>";
                    kq += "     <div class=\"body table-responsive\">";
                    kq += this.taoBangChiTietDieuPhoi(cart);
                    kq += "     </div>";
                    kq += "</div>";
                }
            }
            catch (Exception ex)
            {
                xulyFile.ghiLoi("Class: BangCongTacController - Function: taoGiaoDienChiTiet", ex.Message);
            }
            return kq;
        }
        /// <summary>
        /// Hàm tạo bảng danh sách ca đã chọn
        /// </summary>
        /// <param name="cart">Object chứa giỏ ca làm việc đã chọn</param>
        /// <returns></returns>
        private string taoBangChiTietDieuPhoi(cartDieuPhoi cart)
        {
            string kq = "";
            kq += "    <table class=\"table table-bordered table-striped table-hover js-basic-example dataTable\" id=\"DataTables_Table_0\" role=\"grid\" aria-describedby=\"DataTables_Table_0_info\">";
            kq += "        <thead><tr><th>Tên ca</th><th>Buổi</th><th>Thời gian làm việc</th><th>Ghi chú</th><th></th></tr></thead>";
            kq += "        <tbody>";
            //--------Lặp qua danh sách các ca làm việc có trên giỏ
            foreach (ctBangGiaoViec ct in cart.getList().OrderBy(c => c.caLamViec.buoi).ThenBy(c => c.caLamViec.maCa))
            {
                kq += "            <tr role=\"row\" class=\"odd\">";
                kq += "                <td>" + xulyDuLieu.traVeKyTuGoc(ct.caLamViec.tenCa) + "</td>";
                kq += "                <td>" + this.layBuoiLamViec(ct.caLamViec.buoi) + "</td>";
                kq += "                <td>" + ct.caLamViec.batDau.ToString() + " - " + ct.caLamViec.ketThuc.ToString() + "</td>";
                kq += "                <td>" + xulyDuLieu.traVeKyTuGoc(ct.ghiChu) + "</td>";
                kq += "                <td><button maCa=\"" + ct.maCa.ToString() + "\" class=\"btn btn-danger btnBoCa\">Bỏ ca này</button></td>";
                kq += "            </tr>";
            }
            kq += "        </tbody>";
            kq += "    </table>";
            kq += "<button type=\"button\" id=\"js-btnXoaTatCaDieuPhoi\" class=\"btn btn-danger m-t-15 waves-effect\">Xoá tất cả ca làm việc</button>";
            return kq;
        }
        /// <summary>
        /// Hàm trả về thông tin buổi của ca làm việc
        /// </summary>
        /// <param name="buoi">Số buổi làm việc trong ngày</param>
        /// <returns>Sáng chiều tối</returns>
        private string layBuoiLamViec(int buoi)
        {
            string kq = "";
            switch (buoi)
            {
                case 1:
                    kq = "Sáng";
                    break;

                case 2:
                    kq = "Chiều";
                    break;

                case 3:
                    kq = "Tối";
                    break;

                default: break;
            }
            return kq;
        }

        #endregion

        #region NHÓM HÀM TẠI DANH MỤC CHI TIẾT CA LÀM VIỆC
        /// <summary>
        /// Hàm tạo dữ liệu cho bảng chi tiết có trên modal xem chi tiết điều phối của 1 nhân viên
        /// Sự kiện xảy ra khi người dùng click vào tên nhân viên trong danh mục để xem chi tiết
        /// </summary>
        /// <param name="param">Mã bảng công tác cần xem chi tiết</param>
        /// <returns>Chuỗi html tạo nội dung chi tiết</returns>
        public string AjaxXemChiTietDieuPhoi(string param)
        {
            string htmlDetails = "";
            try
            {
                int maBang = xulyDuLieu.doiChuoiSangInteger(param);
                if (xulyChung.duocTruyCap(idOfPage))
                {
                    BangGiaoViec bgv = new qlCaPheEntities().BangGiaoViecs.SingleOrDefault(b => b.maBang == maBang);
                    if (bgv != null)
                    {
                        htmlDetails += "<div class=\"modal-header\">";
                        htmlDetails += "      <button type=\"button\" class=\"close\" data-dismiss=\"modal\">×</button>";
                        htmlDetails += "    <h3 class=\"modal-title\" id=\"largeModalLabel\">CHI TIẾT PHÂN CÔNG CHO \"" + xulyDuLieu.traVeKyTuGoc(bgv.taiKhoan.thanhVien.hoTV) + " " + xulyDuLieu.traVeKyTuGoc(bgv.taiKhoan.thanhVien.tenTV) + "\" </h3>";
                        htmlDetails += "</div>";
                        htmlDetails += "<div class=\"modal-body\">";
                        htmlDetails += "    <div class=\"row\">";
                        htmlDetails += "        <div class=\"col-md-12 col-lg-12\">";
                        htmlDetails += "            <div class=\"card\">";
                        htmlDetails += "                <div class=\"header bg-cyan\">";
                        htmlDetails += "                    <h2>Danh mục ca đã phân công</h2>";
                        htmlDetails += "                </div>";
                        htmlDetails += "                <div class=\"body table-responsive\">";
                        //////-------------------HIỆN DANH SÁCH CHI TIẾT
                        htmlDetails += this.taoBangChiTietDieuPhoi(bgv);
                        htmlDetails += "                </div>";
                        htmlDetails += "            </div>";
                        htmlDetails += "        </div>";
                        htmlDetails += "</div>";
                        htmlDetails += "<div class=\"modal-footer\">";
                        htmlDetails += "    <div class=\"col-md-4\"></div>";
                        htmlDetails += "    <div class=\"col-md-8\">          ";
                        htmlDetails += "        <a class=\"btn btn-default waves-effect\"  data-dismiss=\"modal\"><i class=\"material-icons\">close</i>Đóng lại</a>";
                        htmlDetails += "    </div>";
                        htmlDetails += "</div>";
                    }
                }
            }
            catch (Exception ex)
            {
                xulyFile.ghiLoi("Class: BangCongTacController - Function: AjaxXemChiTietCaCuaTaiKhoan", ex.Message);
            }
            return htmlDetails;
        }
        /// <summary>
        /// Hàm tạo bảng chi tiết có trong modal chi tiết phân công tại danh mục
        /// </summary>
        /// <param name="bgv">Object chứa Điều phối cần xem</param>
        /// <returns>Chuỗi html tạo bảng nội dung</returns>
        private string taoBangChiTietDieuPhoi(BangGiaoViec bgv)
        {
            string htmlDetail = "";
            try
            {
                htmlDetail += " <div class=\"body table-responsive\">";
                htmlDetail += "    <table class=\"table table-bordered table-striped table-hover js-basic-example dataTable\" id=\"DataTables_Table_0\" role=\"grid\" aria-describedby=\"DataTables_Table_0_info\">";
                htmlDetail += "        <thead>";
                htmlDetail += "            <tr>";
                htmlDetail += "                <th>Tên ca</th>";
                htmlDetail += "                <th>Buổi</th>";
                htmlDetail += "                <th>Thời gian làm việc</th>";
                htmlDetail += "                <th>Ghi chú</th>";
                htmlDetail += "            </tr>";
                htmlDetail += "        </thead>";
                htmlDetail += "        <tbody>";
                //----Lặp qua bảng chi tiết để xem chi tiết đã phân công
                foreach (ctBangGiaoViec ct in bgv.ctBangGiaoViecs.ToList().OrderBy(c => c.caLamViec.buoi))
                {
                    htmlDetail += "            <tr role=\"row\" class=\"odd\">";
                    htmlDetail += "                <td>" + xulyDuLieu.traVeKyTuGoc(ct.caLamViec.tenCa) + "</td>";
                    htmlDetail += "                <td>" + this.layBuoiLamViec(ct.caLamViec.buoi) + "</td>";
                    htmlDetail += "                <td>" + ct.caLamViec.batDau.ToString() + " - " + ct.caLamViec.ketThuc.ToString() + "</td>";
                    htmlDetail += "                <td>" + xulyDuLieu.traVeKyTuGoc(ct.ghiChu)+ "</td>";
                    htmlDetail += "            </tr>";
                }
                htmlDetail += "        </tbody>";
                htmlDetail += "    </table>";
                htmlDetail += "</div>";
            }
            catch (Exception ex)
            {
                xulyFile.ghiLoi("Class: BangCongTacController - Function: AjaxXemChiTietCaCuaTaiKhoan", ex.Message);
            }
            return htmlDetail;
        }
        #endregion
        #endregion
    }
}

