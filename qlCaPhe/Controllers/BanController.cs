using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using qlCaPhe.Models;
using qlCaPhe.App_Start;

namespace qlCaPhe.Controllers
{
    public class BanController : Controller
    {
        private static string idOfPage = "302";
        #region CREATE
        /// <summary>
        /// Hàm thực hiện tạo View thêm mới bàn
        /// </summary>
        /// <returns></returns>
        public ActionResult b_TaoMoiBan()
        {
            if (xulyChung.duocTruyCap(idOfPage))
            {
                this.resetDuLieu();
                try
                {
                    taoDuLieuChoCbb(new qlCaPheEntities());
                    xulyChung.ghiNhatKyDtb(1, "Tạo mới bàn - chổ ngồi");
                }
                catch (Exception ex)
                {
                    xulyFile.ghiLoi("Class: BanController - Function: b_TaoMoiBan_Get", ex.Message);
                }
            }
            return View();
        }
        /// <summary>
        /// Hàm thực hiện thêm mới bàn vào CSDL
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult b_TaoMoiBan(FormCollection f, BanChoNgoi ban)
        {
            if (xulyChung.duocCapNhat(idOfPage, "7"))
            {
                string ndThongBao = ""; int kqLuu = 0;
                qlCaPheEntities db = new qlCaPheEntities();
                try
                {
                    this.layDuLieuTuView(f, ban);
                    kiemTraTenBanTrung(db, ban.tenBan);
                    ban.trangThai = 1; //Thiết lập trạng thái bàn đang sử dụng
                    db.BanChoNgois.Add(ban);
                    kqLuu=  db.SaveChanges();
                    if (kqLuu > 0)
                    {
                        ndThongBao = createHTML.taoNoiDungThongBao("Bàn", xulyDuLieu.traVeKyTuGoc(ban.tenBan), "b_TableBan");
                        this.resetDuLieu();
                        this.taoDuLieuChoCbb(db);
                        xulyChung.ghiNhatKyDtb(2, ndThongBao);
                    }
                }
                catch (Exception ex)
                {
                    //Tạo nội dung thông báo
                    ndThongBao = ex.Message;
                    xulyFile.ghiLoi("Class BanController - Function: b_TaoMoiBan", ex.Message);
                    this.resetDuLieu();
                    //----Hiện lại dữ liệu khi thông báo lỗi
                    this.doDuLieuLenView(ban, db);
                }
                ViewBag.ThongBao = createHTML.taoThongBaoLuu(ndThongBao);
            }
            return View();
        }
        #endregion
        #region READ
        /// <summary>
        /// Hàm thực hiện tạo giao diện danh sách bàn được phép sử dụng trangThai=1
        /// </summary>
        /// <returns></returns>
        public ActionResult b_TableBan(int ?page)
        {
            int trangHienHanh = (page ?? 1);
            if (xulyChung.duocTruyCap(idOfPage))
            {
                try
                {
                    this.createTableBan(1, trangHienHanh, "/Ban/b_TableBan");
                    xulyChung.ghiNhatKyDtb(1, "Danh mục bàn đang sử dụng");
                }
                catch (Exception ex)
                {
                    xulyFile.ghiLoi("Class: BanController - Function: b_TableBan", ex.Message);
                }
            }
            return View();
        }
        /// <summary>
        /// hàm lấy danh sách bàn đang sửa chữa trangThai=2
        /// </summary>
        /// <returns></returns>
        public ActionResult b_TableBanSuaChua(int ?page)
        {
            int trangHienHanh = (page ?? 1);
            if (xulyChung.duocTruyCap(idOfPage))
            {
                try
                {
                    this.createTableBan(2, trangHienHanh, "/Ban/b_TableBanSuaChua");
                    xulyChung.ghiNhatKyDtb(1, "Danh mục bàn đang sửa chữa");
                }
                catch (Exception ex)
                {
                    xulyFile.ghiLoi("Class: BanController - Function: b_TableBanSuaChua", ex.Message);
                }
            }
            return View();
        }
        /// <summary>
        /// Hàm lấy danh sách bàn bị hủy bỏ TrangThai=0
        /// </summary>
        /// <returns></returns>
        public ActionResult b_TableBanHuyBo(int? page)
        {
            int trangHienHanh = (page ?? 1);
            if (xulyChung.duocTruyCap(idOfPage))
            {
                try
                {
                    this.createTableBan(0, trangHienHanh, "/Ban/b_TableBanHuyBo");
                    xulyChung.ghiNhatKyDtb(1, "Danh mục bàn bị hủy bỏ");
                }
                catch (Exception ex)
                {
                    xulyFile.ghiLoi("Class: BanController - Function: b_TableBanSuaChua", ex.Message);
                }
            }
            return View();
        }
        /// <summary>
        /// Hảm thực hiện tạo bảng bàn
        /// </summary>
        /// <param name="trangThai">Trạng thái liệt kê danh mục bàn <para/> 1: Bàn được sử dụng, 2: Bàn đang sửa chữa, 0, bàn tạm ngưng</param>
        private void createTableBan(int trangThai, int trangHienHanh, string url)
        {
            string htmlTable = "";
            qlCaPheEntities db = new qlCaPheEntities();
            int soPhanTu = db.sanPhams.Where(s => s.trangThai == trangThai).Count();
            ViewBag.PhanTrang = createHTML.taoPhanTrang(soPhanTu, trangHienHanh, url); //------cấu hình phân trang

            foreach (BanChoNgoi ban in db.BanChoNgois.ToList().Where(b => b.trangThai == trangThai).OrderBy(b=>b.tenBan).Skip((trangHienHanh - 1) * createHTML.pageSize).Take(createHTML.pageSize))
            {
                htmlTable += "<tr role=\"row\" class=\"odd\">";
                htmlTable += "      <td>";
                htmlTable += "          <a href=\"#\"  maHinh=\"" + ban.maBan.ToString() + "\" class=\"goiY\">";
                htmlTable +=                    xulyDuLieu.traVeKyTuGoc(ban.tenBan);
                htmlTable += "              <span class=\"noiDungGoiY-right\">Click để xem hình</span>";
                htmlTable += "          </a>";
                htmlTable += "      </td>";
                htmlTable += "      <td>" + xulyDuLieu.traVeKyTuGoc(ban.gioiThieu) + "</td>";
                htmlTable += "      <td>" + xulyDuLieu.traVeKyTuGoc(ban.khuVuc.tenKhuVuc) + "</td>";
                htmlTable += "      <td>" + ban.sucChua.ToString() + " người </td>";
                htmlTable += "      <td>";
                htmlTable += this.taoNutChucNangChoBang(ban, trangThai);
                htmlTable += "      </td>";
                htmlTable += "</tr>";
            }
            ViewBag.TableData = htmlTable;
            ViewBag.ScriptAjax = createScriptAjax.scriptAjaxXoaDoiTuong("Ban/xoaBan?maBan=");
            ViewBag.ModalXoa = createHTML.taoCanhBaoXoa("Bàn");
            //----Nhúng script ajax hiển thị hình khi người dùng click vào tên sản phẩm
            ViewBag.ScriptAjaxXemHinh = createScriptAjax.scriptAjaxXemChiTietKhiClick("goiY", "maHinh", "Ban/hienHinhBan?maBan=", "vungChiTiet", "modalChiTiet");
            //----Nhúng các tag html cho modal chi tiết
            ViewBag.ModalChiTiet = createHTML.taoModalChiTiet("modalChiTiet", "vungChiTiet", 1);
        }
        /// <summary>
        /// Hàm tạo các nút chức năng cho bảng danh mục bàn
        /// </summary>
        /// <param name="ban"></param>
        /// <param name="trangThai"></param>
        /// <returns></returns>
        private string taoNutChucNangChoBang(BanChoNgoi ban, int trangThai)
        {
            string htmlTable = "";
            htmlTable += "          <div class=\"btn-group\">";
            htmlTable += "              <button type=\"button\" class=\"btn btn-primary dropdown-toggle\" data-toggle=\"dropdown\">";
            htmlTable += "                  Chức năng <span class=\"caret\"></span>";
            htmlTable += "              </button>";
            htmlTable += "              <ul class=\"dropdown-menu\" role=\"menu\">";
            htmlTable += createTableData.taoNutChinhSua("/Ban/b_ChinhSuaBan", ban.maBan.ToString());
            switch (trangThai)  //Thực hiện tạo các chức năng cập nhật trạng thái
            {
                case 0: //Cập nhật trạng thái là sửa hoặc sửa dụng
                    htmlTable += "                  <li><a href=\"capNhatTrangThai?maBan=" + ban.maBan.ToString() + "&trangThai=1\" class=\"col-green\"><i class=\"material-icons\">done_all</i>Sử dụng</a></li>";
                    htmlTable += "                  <li><a href=\"capNhatTrangThai?maBan=" + ban.maBan.ToString() + "&trangThai=2\" class=\"col-orange\"><i class=\"material-icons\">build</i>Sữa chữa</a></li>";
                    break;
                case 1:  //Cập nhật trạng thái là sửa hoặc ngưng sủ dụng
                    htmlTable += "                  <li><a href=\"capNhatTrangThai?maBan=" + ban.maBan.ToString() + "&trangThai=2\" class=\"col-orange\"><i class=\"material-icons\">build</i>Sữa chữa</a></li>";
                    htmlTable += "                  <li><a href=\"capNhatTrangThai?maBan=" + ban.maBan.ToString() + "&trangThai=0\" class=\"col-gray\"><i class=\"material-icons\">cancel</i>Ngưng sử dụng</a></li>";
                    break;
                case 2:  //Cập nhật trạng thái là sủ dụng hoặc ngưng sử dụng
                    htmlTable += "                  <li><a href=\"capNhatTrangThai?maBan=" + ban.maBan.ToString() + "&trangThai=1\" class=\"col-green\"><i class=\"material-icons\">done_all</i>Sử dụng</a></li>";
                    htmlTable += "                  <li><a href=\"capNhatTrangThai?maBan=" + ban.maBan.ToString() + "&trangThai=0\" class=\"col-gray\"><i class=\"material-icons\">build</i>Ngưng sử chữa</a></li>";
                    break;
            }
            htmlTable += createTableData.taoNutXoaBo(ban.maBan.ToString());
            htmlTable += "              </ul>";
            htmlTable += "          </div>";
            return htmlTable;
        }
        /// <summary>
        /// Hàm thực hiện hiển thị hình ảnh của đồ uống trên modal khi người dùng click vào tên đồ uống
        /// </summary>
        /// <param name="maDoUong"></param>
        /// <returns></returns>
        public string hienHinhBan(int maBan)
        {
            string kq = "";
            BanChoNgoi ban = new qlCaPheEntities().BanChoNgois.SingleOrDefault(n => n.maBan== maBan);
            if (ban != null)
            {
                kq += "<div class=\"modal-header\">";
                kq+= "      <button type=\"button\" class=\"close\" data-dismiss=\"modal\">×</button>";
                kq += "     <h4 class=\"modal-title\" id=\"defaultModalLabel\">" + xulyDuLieu.traVeKyTuGoc(ban.tenBan) + "</h4>";
                kq += "</div>";
                kq += "<div class=\"modal-body\">";
                if (ban.hinhAnh != null)
                    kq += "     <img src=\"" + xulyDuLieu.chuyenByteHinhThanhSrcImage(ban.hinhAnh) + "\" style=\"max-width:100%; height:auto; width:500px;\" />";
                kq += "</div>";
                kq += "<div class=\"modal-footer\">";
                kq += "     <button type=\"button\" class=\"btn btn-link waves-effect\" data-dismiss=\"modal\">Đóng</button>";
                kq += "</div>";
            }
            return kq;
        }
        #endregion
        #region UPDATE
        /// <summary>
        /// Hàm thực hiện cập nhật trạng thái của bàn
        /// </summary>
        /// <param name="maBan">Mã bàn cần cập nhật</param>
        /// <param name="trangThai">Trạng thái cần cập nhật: (0: ngừng sử dụng 1: Còn sử dụng, 2: Đang sửa)</param>
        /// <returns></returns>
        public ActionResult capNhatTrangThai(int maBan, int trangThai)
        {
            try
            {
                //----Kiểm tra trạng thái. Chỉ nhận và cập nhật 1 trong 3 trạng thái
                if (trangThai == 0 || trangThai == 1 || trangThai == 2)
                {
                    qlCaPheEntities db = new qlCaPheEntities(); int kqLuu = 0;
                    //---Lấy bàn cần cập nhật
                    var banSua = db.BanChoNgois.SingleOrDefault(b => b.maBan == maBan);
                    if (banSua != null)
                    {
                        int ttTemp = (int)banSua.trangThai;//-----Biến lưu lại trạng thái của bàn trước khi cập nhật để khi cập nhật xong sẽ chuyển về giao diện danh mục trước đó
                        banSua.trangThai = trangThai;
                        db.Entry(banSua).State = System.Data.Entity.EntityState.Modified;
                        kqLuu= db.SaveChanges();
                        if (kqLuu > 0)
                        {
                            xulyChung.ghiNhatKyDtb(4, "Cập nhật trạng thái bàn \" " + xulyDuLieu.traVeKyTuGoc(banSua.tenBan) + " \"");
                            //----Dựa vào trạng thái bàn trước đó để chuyển đến giao diện bàn tương ứng
                            if (ttTemp == 1)
                                return RedirectToAction("b_TableBan");
                            else if (ttTemp == 2)
                                return RedirectToAction("b_TableBanSuaChua");
                        }
                    }
                    else
                        throw new Exception("Bàn chổ ngồi có mã'"+maBan.ToString()+"' không tồn tại để cập nhật");
                }
                else
                    throw new Exception("Không thể cập nhật trạng thái '"+trangThai.ToString()+"' cho bàn");
            }
            catch (Exception ex)
            {
                xulyFile.ghiLoi("Class: BanController - Function: capNhatTrangThai", ex.Message);
                return RedirectToAction("ServerError", "Home");
            }
            //----Mặc định, dựa vào trạng thái để chuyển đến giao diện hợp lý. Nếu không thì sẽ trả về danh mục bàn bị hủy bỏ
            return RedirectToAction("b_TableBanHuyBo");
        }
        /// <summary>
        /// Hàm thực hiện tạo giao diện chỉnh sửa bàn
        /// </summary>
        /// <param name="maBan"></param>
        /// <returns></returns>
        public ActionResult b_ChinhSuaBan()
        {
            if (xulyChung.duocCapNhat(idOfPage, "7"))
            {
                try
                {
                    this.resetDuLieu();
                    //------Tạo biến nhận request từ session
                    string param = xulyChung.nhanThamSoTrongSession();
                    if (param.Length > 0)
                    {
                        int maBan = xulyDuLieu.doiChuoiSangInteger(param);
                        qlCaPheEntities db = new qlCaPheEntities();
                        BanChoNgoi banSua = db.BanChoNgois.SingleOrDefault(b => b.maBan == maBan);
                        if (banSua != null)
                        {
                            this.doDuLieuLenView(banSua, db);
                            xulyChung.ghiNhatKyDtb(1, "Chỉnh sửa bàn \" " + xulyDuLieu.traVeKyTuGoc(banSua.tenBan) + " \"");
                        }
                        else
                            throw new Exception("Bàn có mã '" + maBan.ToString() + "' không tồn tại để thực hiện cập nhật");
                    }
                    else throw new Exception("không được nhận tham số");
                }
                catch (Exception ex)
                {
                    xulyFile.ghiLoi("Class: BanController - Function: b_ChinhSuaBan_Get", ex.Message);
                    return RedirectToAction("PageNotFound", "Home");
                }
            }
            return View();
        }
        /// <summary>
        /// Hàm thực hiện cập nhật bàn trong csdl
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult b_ChinhSuaBan(FormCollection f)
        {
            if (xulyChung.duocCapNhat(idOfPage, "7"))
            {
                int kqLuu = 0;
                BanChoNgoi banSua = new BanChoNgoi(); qlCaPheEntities db = new qlCaPheEntities();
                try
                {
                    int maBan = Convert.ToInt32(f["txtMaBan"]);
                    banSua = db.BanChoNgois.Single(b => b.maBan == maBan);
                    string tenBanCu = banSua.tenBan; //--------Lưu lại tên bàn cũ để tiến hành so sánh tên bàn có tồn tại khi xảy ra thay đổi
                    this.layDuLieuTuView(f, banSua);
                    //----Kiểm tra tên bàn có thay đổi không
                    if (banSua.tenBan != tenBanCu) //-----Nếu tên bàn có thay đổi thì kiểm tra tên bàn đã có tồn tại chưa
                        this.kiemTraTenBanTrung(db, banSua.tenBan);
                    db.Entry(banSua).State = System.Data.Entity.EntityState.Modified;
                    kqLuu= db.SaveChanges();
                    if (kqLuu > 0)
                    {
                        this.resetDuLieu();
                        xulyChung.ghiNhatKyDtb(4, "Bàn \" " + xulyDuLieu.traVeKyTuGoc(banSua.tenBan) + " \"");
                        //---Dựa vào trạng thái bàn cập nhật hiện tại để chuyển đến giao diện tương ứng
                        if (banSua.trangThai == 0)
                            return RedirectToAction("b_TableBanHuyBo");
                        else if (banSua.trangThai == 1)
                            return RedirectToAction("b_TableBan");
                        else if (banSua.trangThai == 2)
                            return RedirectToAction("b_TableBanSuaChua");
                    }
                }
                catch (Exception ex)
                {
                    ViewBag.ThongBao = createHTML.taoThongBaoLuu(ex.Message);
                    xulyFile.ghiLoi("Class: BanController - Function: b_ChinhSuaBan", ex.Message);
                    //----Hiện lại dữ liệu khi thông báo lỗi
                    this.resetDuLieu();
                    this.doDuLieuLenView(banSua, db);
                }
            }
            return View();
        }
        #endregion
        #region DELETE
        /// <summary>
        /// Hàm thực hiện xóa bàn khỏi csdl
        /// </summary>
        /// <param name="maBan"></param>
        public void xoaBan(int maBan)
        {
            try
            {
                int kqLuu = 0;
                qlCaPheEntities db = new qlCaPheEntities();
                var banXoa = db.BanChoNgois.SingleOrDefault(b => b.maBan == maBan);
                if (banXoa != null)
                {
                    db.BanChoNgois.Remove(banXoa);
                    kqLuu=db.SaveChanges();
                    if(kqLuu>0)
                        xulyChung.ghiNhatKyDtb(3, "Bàn \"" + xulyDuLieu.traVeKyTuGoc(banXoa.tenBan) + " \"");
                }
                else
                    throw new Exception("Bàn có mã số '" + maBan.ToString() + "' không tồn tại để xóa");
            }
            catch (Exception ex)
            {
                xulyFile.ghiLoi("Class: BanController - Function: xoaBan", ex.Message);
                Response.Redirect(xulyChung.layTenMien() + "Home/ServerError");
            }
        }
        #endregion
        #region ORTHERS
        /// <summary>
        /// Hàm thực hiện đổ dữ liệu khu vực lên combobox
        /// </summary>
        /// <param name="db"></param>
        private void taoDuLieuChoCbb(qlCaPheEntities db)
        {
            string htmlCbb = "";
            foreach (khuVuc kv in db.khuVucs.ToList().OrderBy(m => m.tenKhuVuc))
                htmlCbb += "<option value=\"" + kv.maKhuVuc.ToString() + "\">" + xulyDuLieu.traVeKyTuGoc(kv.tenKhuVuc) + "</option>";
            ViewBag.cbbKhuVuc = htmlCbb;
        }
        /// <summary>
        /// hàm thực hiện lấy dữ liệu nhập vào từ giao diện
        /// </summary>
        /// <param name="f"></param>
        /// <param name="ban"></param>
        private void layDuLieuTuView(FormCollection f, BanChoNgoi ban)
        {
            string loi = "";
            ban.tenBan = xulyDuLieu.xulyKyTuHTML(f["txtTenBan"]);
            if (ban.tenBan.Length <= 0)
                loi += "Vui lòng nhập tên bàn - chổ ngồi <br/>";
            ban.gioiThieu = xulyDuLieu.xulyKyTuHTML(f["txtGioiThieu"]);
            if (ban.gioiThieu.Length <= 0)
                loi += "Vui lòng nhập thông tin giới thiệu cho bàn - chổ ngồi </br>";
            ban.sucChua = xulyDuLieu.doiChuoiSangInteger(f["txtSucChua"]);
            if (ban.sucChua <= 0)
                loi += "Vui lòng nhập sức chứa - chổ ngồi của bàn lớn hơn 0";
            ban.ghiChu = xulyDuLieu.xulyKyTuHTML(f["txtGhiChu"]);
            ban.maKhuVuc = Convert.ToInt32(f["cbbKhuVuc"]);
            if (ban.maKhuVuc <= 0)
                loi+="Vui lòng chọn khu vực quản lý bàn";
            string pathHinh = f["pathHinh"];
            //------Kiểm tra xem có chọn hình không. 
            if (!pathHinh.Equals("")) //Nếu có chọn hình  thì luu hình vào csdl
            {
                pathHinh = xulyMaHoa.Decrypt(pathHinh);//Giải mã lại chuỗi đường dẫn lưu hình ảnh trên đĩa đã được mã hóa trong ajax
                ban.hinhAnh = xulyDuLieu.chuyenDoiHinhSangByteArray(pathHinh); //Lưu hình vào thuộc tính hinhAnh
            }
            else //-------Nếu không có chọn hình
                if (f["txtMaBan"] == null) //--------Kiểm tra xem có mã sản phẩm không. Nếu không tức là thêm mới và báo lỗi
                    loi += "Vui lòng chọn hình ảnh cho bàn <br/>";
            if (loi.Length > 0)
                throw new Exception(loi);
        }
        /// <summary>
        /// Hàm thực hiện kiểm tra tên bản có bị trùng không. Nếu có thì thông báo
        /// </summary>
        /// <param name="db"></param>
        /// <param name="tenBan"></param>
        private void kiemTraTenBanTrung(qlCaPheEntities db, string tenBan)
        {
            try
            {
                //Kiểm tra tên bàn có trùng.
                var tenBanTrung = db.BanChoNgois.FirstOrDefault(m => m.tenBan == tenBan);
                if (tenBanTrung != null)
                    throw new Exception("Bàn <b>" + xulyDuLieu.traVeKyTuGoc(tenBan) + "</b> đã tồn tại, vui lòng nhập lại một bàn mới");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        /// <summary>
        /// Hàm thực hiện đổ dữ liệu bàn lên view để chỉnh sửa
        /// </summary>
        private void doDuLieuLenView(BanChoNgoi ban, qlCaPheEntities db)
        {
            if (ban != null)
            {
                ViewBag.txtMaBan = ban.maBan.ToString();
                //Hiển thị combobox khu vực
                string htmlCbb = "";
                foreach (khuVuc kv in db.khuVucs.ToList().OrderBy(m => m.tenKhuVuc))
                    if (kv.maKhuVuc == ban.maKhuVuc) //Nếu bàn thuộc khu vực đang duyệt thì combobox sẽ chọn là cbbKhu vực
                        htmlCbb += "<option selected value=\"" + kv.maKhuVuc.ToString() + "\">" + xulyDuLieu.traVeKyTuGoc(kv.tenKhuVuc) + "</option>";
                    else
                        htmlCbb += "<option value=\"" + kv.maKhuVuc.ToString() + "\">" + xulyDuLieu.traVeKyTuGoc(kv.tenKhuVuc) + "</option>";
                ViewBag.cbbKhuVuc = htmlCbb;
                ViewBag.txtTenBan = xulyDuLieu.traVeKyTuGoc(ban.tenBan);
                ViewBag.txtGioiThieu = xulyDuLieu.traVeKyTuGoc(ban.gioiThieu);
                ViewBag.txtSucChua = ban.sucChua.ToString();
                ViewBag.txtGhiChu = xulyDuLieu.traVeKyTuGoc(ban.ghiChu);
                if (ban.hinhAnh != null)
                    ViewBag.hinhDD = xulyDuLieu.chuyenByteHinhThanhSrcImage(ban.hinhAnh);
            }
        }
        /// <summary>
        /// Hàm thực hiện reset lại dữ liệu ban đầu 
        /// </summary>
        private void resetDuLieu()
        {
            //xulyChung.pathHinhAnhCrop = null; xulyChung.srcHinhCrop = null;
            //pathTempHinhCu = null;
            ViewBag.hinhDD = "/images/gallery-upload-256.png";
            //Xóa tất cả các hình đã tải lên trong tập tin tạm            
            xulyFile.donDepTM(Server.MapPath("~/pages/temp/banChoNgoi"));
            //Nhúng script ajax thực hiện tải và cắt hình
            ViewBag.ScriptCropImage = createScriptAjax.scriptAjaxUpLoadAndCropImage("banChoNgoi", 268, 185, 268, 185);
            //Gán modal up và crop hình ảnh lên view
            ViewBag.modalChonHinh = createHTML.taoModalUpHinh();
        }
        #endregion
    }
}