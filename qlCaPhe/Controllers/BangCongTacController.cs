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
                    kqLuu=db.SaveChanges();

                    if (kqLuu > 0)
                    {
                        this.themCtBangGiaoViecVaoDatabase(bgv, db);
                        ndThongBao = createHTML.taoNoiDungThongBao("Phân công cho thành viên", xulyDuLieu.traVeKyTuGoc(bgv.tenDangNhap), "bct_TableDieuPhoi");
                        this.taoDuLieuChoCbb(db, 0);
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
        public ActionResult bct_TableDieuPhoi()
        {
            if (xulyChung.duocTruyCap(idOfPage))
            {
                string htmlTable = "";
                try
                {
                    //--------Lặp qua danh sách bảng công tác được sắp xếp theo trạng thái
                    foreach (BangGiaoViec bgv in new qlCaPheEntities().BangGiaoViecs.ToList().OrderBy(c => c.trangThai))
                    {
                        htmlTable += "<tr role=htmlTable+=\"row\" class=\"odd\">";
                        htmlTable += "    <td>";
                        htmlTable += "        <a href=\"#\" data-toggle=\"modal\" data-target=\"#modalChiTiet\"";
                        htmlTable += "            class=\"goiY\">" + xulyDuLieu.traVeKyTuGoc(bgv.taiKhoan.tenDangNhap) + "<span class=\"noiDungGoiY-right\">Click để xem chi tiết</span></a>";
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
        private void taoDuLieuChoCbb(qlCaPheEntities db, int maTV)
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
            foreach (caLamViec ca in db.caLamViecs.ToList().OrderBy(c=> c.buoi).ThenBy(c=>c.maCa))
                htmlCa += "<option value=\"" + ca.maCa.ToString() + "\">" + xulyDuLieu.traVeKyTuGoc(ca.tenCa) + " (" + ca.batDau.ToString() + " - " + ca.ketThuc.ToString() + ")</option>";
            ViewBag.cbbCaLamViec = htmlCa;
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
                if (bgv.taiKhoan != null)
                {
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

        #region AJAX

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
        /// Hàm tạo giao diện cho vùng chi tiết ca làm việc
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
            foreach (ctBangGiaoViec ct in cart.getList().OrderBy(c=>c.caLamViec.buoi).ThenBy(c=>c.caLamViec.maCa))
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
    }
}