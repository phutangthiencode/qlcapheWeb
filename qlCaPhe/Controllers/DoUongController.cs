using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using qlCaPhe.Models;
using qlCaPhe.App_Start;

namespace qlCaPhe.Controllers
{
    public class DoUongController : Controller
    {
        private static string idOfPage = "402";
        #region CREATE
        /// <summary>
        /// Hàm thực hiện tạo giao diện thêm mới đồ uống
        /// </summary>
        /// <returns></returns>
        public ActionResult du_TaoMoiDoUong()
        {
            if (xulyChung.duocTruyCap(idOfPage))
            {
                this.resetDuLieu();
                try
                {
                    taoDuLieuChoCbbTao(new qlCaPheEntities());
                    xulyChung.ghiNhatKyDtb(1, "Tạo mới sản phẩm - đồ uống");
                }
                catch (Exception ex)
                {

                    xulyFile.ghiLoi("Class: DoUongController - Function: du_TaoMoiDoUong_Get", ex.Message);
                }
            }
            return View();
        }
        /// <summary>
        /// Hàm thực hiện thêm mới 1 sản phẩm đồ uống vào CSDL
        /// </summary>
        /// <param name="sp"></param>
        /// <param name="f"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult du_TaoMoiDoUong(sanPham sp, FormCollection f)
        {
            if (xulyChung.duocCapNhat(idOfPage, "7"))
            {
                string ndThongBao = ""; int kqLuu = 0;
                qlCaPheEntities db = new qlCaPheEntities();
                try
                {
                    this.layDuLieuTuView(sp, f);
                    db.sanPhams.Add(sp);
                    kqLuu=db.SaveChanges();
                    if (kqLuu > 0)
                    {
                        ndThongBao = this.htmlTaoNoiDungThongBaoLuu(sp);
                        this.resetDuLieu();
                        this.taoDuLieuChoCbbTao(db);
                        xulyChung.ghiNhatKyDtb(2, ndThongBao);
                    }
                }
                catch (Exception ex)
                {
                    ndThongBao = ex.Message;
                    xulyFile.ghiLoi("Class: DoUongController - Function: du_TaoMoiDoUong_Post", ex.Message);
                    this.resetDuLieu();
                    this.doDuLieuLenView(sp, db);
                }
                ViewBag.ThongBao = createHTML.taoThongBaoLuu(ndThongBao);
            }
            return View();
        }
        #endregion
        #region READ
        /// <summary>
        /// Hàm thực hiện cấu hình tham số cho session
        /// </summary>
        /// <param name="page">Trang cần chuyển</param>
        /// <param name="request">Tham số truyền vào trang</param>
        private void cauHinhSession(string page, string request)
        {
            string param = "page=" + page + "|request=" + request;
            //------Thiết lập lại session
            Session.Remove("urlAction"); Session.Add("urlAction", "");
            Session["urlAction"] = param;
        }

        /// <summary>
        /// Hàm vào trang danh sách đồ uông được phép bán. CÓ TRẠNG THÁI = 1
        /// </summary>
        /// <returns></returns>
        public ActionResult RouteDoUongDuocBan()
        {
            //---------Thêm tham số vào session. TRẠNG THÁI ĐỒ UỐNG ĐƯỢC PHÉP BÁN
            cauHinhSession("du_TableDoUong", "1");
            xulyChung.ghiNhatKyDtb(1, "Danh mục sản phẩm được phép bán");
            return RedirectToAction("du_TableDoUong");
        }

        public ActionResult RouteDoUongChoDuyet()
        {
            //---------Thêm tham số vào session. TRẠNG THÁI ĐỒ UỐNG ĐƯỢC PHÉP BÁN
            cauHinhSession("du_TableDoUong", "0");
            xulyChung.ghiNhatKyDtb(1, "Danh mục sản phẩm chờ kiểm duyệt");
            return RedirectToAction("du_TableDoUong");
        }

        public ActionResult RouteDoUongDaHuy()
        {
            //---------Thêm tham số vào session. TRẠNG THÁI ĐỒ UỐNG ĐƯỢC PHÉP BÁN
            cauHinhSession("du_TableDoUong", "2");
            xulyChung.ghiNhatKyDtb(1, "Danh mục sản phẩm được hủy");
            return RedirectToAction("du_TableDoUong");
        }

        /// <summary>
        /// hàm thực hiện tạo giao diện danh sách đồ uống đang bán
        /// </summary>
        /// <param name="page">Số trang hiện hành</param>
        /// <returns></returns>
        public ActionResult du_TableDoUong(int? page)
        {
            if (xulyChung.duocTruyCap(idOfPage))
            {
                try
                {
                    int trangHienHanh = (page ?? 1);
                    string urlAction = (string)Session["urlAction"];
                    string request = urlAction.Split('|')[1]; //-------request có dạng: request=trangThai
                    request = request.Replace("request=", ""); //-------request có dạng trangThai
                    int trangThai = xulyDuLieu.doiChuoiSangInteger(request);
                    this.thietLapThongSoChung(trangThai);

                    qlCaPheEntities db = new qlCaPheEntities();

                    int soPhanTu = db.sanPhams.Where(s => s.trangThai == trangThai).Count();
                    ViewBag.PhanTrang = createHTML.taoPhanTrang(soPhanTu, trangHienHanh, "/DoUong/du_TableDoUong"); //------cấu hình phân trang

                    List<sanPham> listSP = db.sanPhams.Where(s => s.trangThai == trangThai).OrderBy(s => s.tenSanPham).Skip((trangHienHanh - 1) * createHTML.pageSize).Take(createHTML.pageSize).ToList(); //-----Lấy danh sách các sản phẩm được phân trang
                    this.createTableDoUong(listSP);
                }
                catch (Exception ex)
                {
                    xulyFile.ghiLoi("Class: DoUongController - Function: du_TableDoUong", ex.Message);
                }
            }
            return View();
        }

        /// <summary>
        /// Hàm thực hiện tạo table danh mục đồ uống
        /// </summary>
        /// <param name="listSP">List object sản phẩm cần hiển thị</param>
        private void createTableDoUong(List<sanPham> listSP)
        {
            try
            {
                string htmlTable = "";
                foreach (sanPham sp in listSP)
                {
                    htmlTable += "<tr role=\"row\" class=\"odd\">";
                    htmlTable += "      <td>";
                    htmlTable += "          <a href=\"#\"  maHinh=\"" + sp.maSanPham.ToString() + "\" class=\"goiY\">";
                    htmlTable += xulyDuLieu.traVeKyTuGoc(sp.tenSanPham);
                    htmlTable += "              <span class=\"noiDungGoiY-right\">Click để xem hình</span>";
                    htmlTable += "          </a>";
                    htmlTable += "      </td>";
                    htmlTable += "      <td>" + xulyDuLieu.traVeKyTuGoc(sp.loaiSanPham.tenLoai) + "</td>";
                    htmlTable += "      <td>" + xulyDuLieu.traVeKyTuGoc(sp.moTa) + "</td>";
                    htmlTable += "      <td>" + sp.donGia.ToString() + " VNĐ</td>";
                    htmlTable += "      <td>" + sp.thoiGianPhaChe.ToString() + " phút</td>";
                    htmlTable += "      <td>";
                    htmlTable += this.thietLapNutChucNangTable(sp);
                    htmlTable += "      </td>";
                    htmlTable += "</tr>";
                }
                ViewBag.TableData = htmlTable;
            }
            catch (Exception ex)
            {
                xulyFile.ghiLoi("Class: DoUongController - Function: du_TableDoUong", ex.Message);
                RedirectToAction("ServerError", "Home");
            }
        }
        /// <summary>
        /// Hàm tạo danh sách các nút chức năng tương ứng trên bảng
        /// </summary>
        /// <returns></returns>
        private string thietLapNutChucNangTable(sanPham sp)
        {
            string htmlTable = "";
            htmlTable += "          <div class=\"btn-group\">";
            htmlTable += "              <button type=\"button\" class=\"btn btn-primary dropdown-toggle\" data-toggle=\"dropdown\">";
            htmlTable += "                  Chức năng <span class=\"caret\"></span>";
            htmlTable += "              </button>";
            htmlTable += "              <ul class=\"dropdown-menu\" role=\"menu\">";
            htmlTable += "                  <li><a task=\"" + xulyChung.taoUrlCoTruyenThamSo("/DoUong/du_ChinhSuaDoUong", sp.maSanPham.ToString()) + "\" class=\"guiRequest col-blue\"><i class=\"material-icons\">mode_edit</i>Chỉnh sửa</a></li>";
            if (sp.congThucPhaChes.Count() > 0)
            {
                congThucPhaChe ctSanPham = sp.congThucPhaChes.SingleOrDefault(c => c.trangThai == true);
                if (ctSanPham != null)
                    htmlTable += "                  <li><a maCongThuc=\"" + ctSanPham.maCongThuc.ToString() + "\" class=\"goiYCongThuc col-blue\"><i class=\"material-icons\">search</i>Xem công thức</a></li>";
            }

            switch (sp.trangThai) //-----Dựa vào trạng thái để hiện chức năng phù hợp
            {
                case 0: //---Cho sản phẩm đang chờ duyệt. Cập nhật trạng thái thành 1
                    htmlTable += "          <li><a task=\"" + xulyChung.taoUrlCoTruyenThamSo("/DoUong/capNhatTrangThai", sp.maSanPham.ToString() + "&1") + "\" class=\"guiRequest col-orange\"><i class=\"material-icons\">done</i>Duyệt</a></li>";
                    break;
                case 1://---Cho sản phẩm đang được bán. Cập nhật trạng thái thành 2
                    htmlTable += "          <li><a task=\"" + xulyChung.taoUrlCoTruyenThamSo("/DoUong/capNhatTrangThai", sp.maSanPham.ToString() + "&2") + "\" class=\"guiRequest col-orange\"><i class=\"material-icons\">clear</i>Ngưng bán</a></li>";
                    break;
                case 2://---Cho sản phẩm ngưng bán. Cập nhật trạng thái thành 1
                    htmlTable += "          <li><a task=\"" + xulyChung.taoUrlCoTruyenThamSo("/DoUong/capNhatTrangThai", sp.maSanPham.ToString() + "&1") + "\" class=\"guiRequest col-orange\"><i class=\"material-icons\">refresh</i>Bán lại</a></li>";
                    break;
            }
            htmlTable += "                  <li><a  maXoa=\"" + sp.maSanPham.ToString() + "\" href=\"#\" class=\"xoa col-red\"><i class=\"material-icons\">delete</i>Xoá bỏ</a></li>";
            htmlTable += "              </ul>";
            htmlTable += "          </div>";
            return htmlTable;
        }
        /// <summary>
        /// Hàm thực hiện hiển thị hình ảnh của đồ uống trên modal khi người dùng click vào tên đồ uống
        /// </summary>
        /// <param name="maDoUong"></param>
        /// <returns></returns>
        public string hienHinhDoUong(int maDoUong)
        {
            string kq = "";
            try
            {
                sanPham sp = new qlCaPheEntities().sanPhams.SingleOrDefault(s => s.maSanPham == maDoUong);
                if (sp != null)
                {
                    kq += "<div class=\"modal-header\">";
                    kq += "      <button type=\"button\" class=\"close\" data-dismiss=\"modal\">×</button>";
                    kq += "     <h4 class=\"modal-title\" id=\"defaultModalLabel\">" + xulyDuLieu.traVeKyTuGoc(sp.tenSanPham) + "</h4>";
                    kq += "</div>";
                    kq += "<div class=\"modal-body\">";
                    if (sp.hinhAnh != null)
                        kq += "     <img src=\"" + xulyDuLieu.chuyenByteHinhThanhSrcImage(sp.hinhAnh) + "\" style=\"max-width:100%; height:auto; width:500px;\" />";
                    kq += "</div>";
                    kq += "<div class=\"modal-footer\">";
                    kq += "     <button type=\"button\" class=\"btn btn-link waves-effect\" data-dismiss=\"modal\">Đóng</button>";
                    kq += "</div>";
                }
            }
            catch (Exception ex)
            {
                xulyFile.ghiLoi("Class: DoUongController - Function: hienHinhDoUong", ex.Message);
            }
            return kq;
        }
        /// <summary>
        /// Hàm thực hiện lấy thông tin đồ uống nhúng vào trang tạo công thức khi người dùng chọn một đồ uống trong danh sách.
        /// </summary>
        /// <param name="maDoUong"></param>
        /// <returns></returns>
        public string getInfoDoUongForCreateCongThuc(int maDoUong)
        {
            string kq = "";
            try
            {
                sanPham sp = new qlCaPheEntities().sanPhams.SingleOrDefault(s => s.maSanPham == maDoUong);
                if (sp != null)
                {
                    kq += "<img id=\"hinhDoUong\" class='img img-responsive img-thumbnail'";
                    kq += "src=\"" + xulyDuLieu.chuyenByteHinhThanhSrcImage(sp.hinhAnh) + "\" width=\"250px\" height=\"auto\" />";
                    kq += "<p class=\"font-bold col-pink\">Tên sản phẩm: " + xulyDuLieu.traVeKyTuGoc(sp.tenSanPham) + "</p>";
                    kq += "<p class=\"font-bold col-blue\">Đơn giá: " + sp.donGia.ToString() + "</p>";
                }
                else
                    throw new Exception("Sản phẩm có mã " + maDoUong.ToString() + " không tồn tại để lấy thông tin");
            }
            catch (Exception ex)
            {
                xulyFile.ghiLoi("Class: DoUongController - Function: getInfoThanhVienForCreateCongThuc", ex.Message);
            }
            return kq;
        }
        #endregion
        #region UPDATE
        /// <summary>
        /// hàm thực hiện cập nhật trạng thái của 1 sản phẩm
        /// 0: Chờ duyệt, 1: Đã duyệt, 2: Tạm ngưng - Hủy
        /// </summary>
        /// <param name="maDoUong">Mã đồ uống cần cập nhật</param>
        /// <param name="trangThai">Giá trị trạng thái thực tế cần cập nhật</param>
        /// <returns></returns>
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
                        string request = urlAction.Split('|')[1]; //-------request có dạng: request=maSanPham;trangThai
                        request = request.Replace("request=", ""); //-------request có dạng maSanPham;trangThai
                        int maDoUong = xulyDuLieu.doiChuoiSangInteger(request.Split('&')[0]);
                        int trangThai = xulyDuLieu.doiChuoiSangInteger(request.Split('&')[1]);
                        //----Kiểm tra trạng thái. Chỉ nhận và cập nhật 1 trong 3 trạng thái
                        if (trangThai == 0 || trangThai == 1 || trangThai == 2)
                        {
                            qlCaPheEntities db = new qlCaPheEntities();
                            sanPham spSua = db.sanPhams.SingleOrDefault(s => s.maSanPham == maDoUong);
                            if (spSua != null)
                            {
                                int ttTemp = spSua.trangThai; //Lưu lại trạng thái tạm trước khi cập nhật để chuyển đến danh sách sản phẩm với trạng thái cũ
                                spSua.trangThai = trangThai;
                                db.Entry(spSua).State = System.Data.Entity.EntityState.Modified;
                                kqLuu= db.SaveChanges();
                                if (kqLuu > 0)
                                {
                                    xulyChung.ghiNhatKyDtb(4, "Cập nhật trạng thái sản phẩm\" " + xulyDuLieu.traVeKyTuGoc(spSua.tenSanPham) + " \"");
                                    if (ttTemp == 1)
                                        return RedirectToAction("RouteDoUongDuocBan");
                                    else if (ttTemp == 2)
                                        return RedirectToAction("RouteDoUongDaHuy");
                                }
                            }
                            else throw new Exception("Sản phẩm có mã " + maDoUong.ToString() + " không tồn tại để cập nhật");
                        }
                        else throw new Exception("Trạng thái muốn cập nhật sản phẩm không hợp lệ");
                    }
                    else throw new Exception("Lỗi chưa nhận tham số");
                }
                catch (Exception ex)
                {
                    xulyFile.ghiLoi("Class: DoUongController - Function: capNhatTrangThai", ex.Message);
                    return RedirectToAction("ServerError", "Home");
                }
                //Chuyển về giao diện mặc định nếu chưa có giao diện nào chuyển về
            }
            return RedirectToAction("RouteDoUongChoDuyet");
        }
        /// <summary>
        /// Hàm tạo giao diện cập nhật thông tin đồ uống
        /// </summary>
        /// <param name="maDoUong"></param>
        /// <returns></returns>
        public ActionResult du_ChinhSuaDoUong()
        {
            if (xulyChung.duocCapNhat(idOfPage, "7"))
            {
                this.resetDuLieu();
                try
                {
                    //-------Nhận request có trong session
                    string urlAction = (string)Session["urlAction"];
                    if (urlAction.Length > 0) //Nếu có request
                    {
                        //=======Xử lý request
                        string request = urlAction.Split('|')[1];
                        int maDoUong = xulyDuLieu.doiChuoiSangInteger(request.Replace("request=", ""));
                        qlCaPheEntities db = new qlCaPheEntities();
                        sanPham spSua = db.sanPhams.SingleOrDefault(s => s.maSanPham == maDoUong);
                        if (spSua != null)
                        {
                            this.doDuLieuLenView(spSua, db);
                            ViewBag.ScriptCavasLichSuGia = this.scriptBieuDoLichSuGia(spSua);
                            xulyChung.ghiNhatKyDtb(1, "Chỉnh sửa sản phẩm \" " + xulyDuLieu.traVeKyTuGoc(spSua.tenSanPham) + " \"");
                        }
                        else
                            throw new Exception("Sản phẩm có mã " + maDoUong.ToString() + " không tồn tại để cập nhật");
                    }
                    else
                        return RedirectToAction("PageNotFound", "Home");
                }
                catch (Exception ex)
                {
                    xulyFile.ghiLoi("Class: DoUongController - Function: du_ChinhSuaDoUong_Get", ex.Message);
                    return RedirectToAction("PageNotFound", "Home");
                }
            }
            return View();
        }

        /// <summary>
        /// Hàm cập nhật thông tin đồ uống trong csdl
        /// </summary>
        /// <param name="maDoUong"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult du_ChinhSuaDoUong(FormCollection f)
        {
            if (xulyChung.duocCapNhat(idOfPage, "7"))
            {
                int kqLuu = 0;
                sanPham spSua = new sanPham(); qlCaPheEntities db = new qlCaPheEntities();
                try
                {
                    int maSP = Convert.ToInt32(f["txtMaDoUong"]);
                    spSua = db.sanPhams.SingleOrDefault(s => s.maSanPham == maSP);
                    if (spSua != null)
                    {
                        this.layDuLieuTuView(spSua, f);
                        spSua.trangThai = 0; //Set lại trạng thái của sản phẩm là chưa duyệt
                        db.Entry(spSua).State = System.Data.Entity.EntityState.Modified;
                        kqLuu=db.SaveChanges();
                        if (kqLuu > 0)
                        {
                            this.resetDuLieu();
                            xulyChung.ghiNhatKyDtb(4, "Sản phẩm \" " + xulyDuLieu.traVeKyTuGoc(spSua.tenSanPham) + " \"");
                            return RedirectToAction("RouteDoUongChoDuyet");
                        }
                    }
                }
                catch (Exception ex)
                {
                    ViewBag.ThongBao = createHTML.taoThongBaoLuu(ex.Message);
                    xulyFile.ghiLoi("Class: DoUongController - Function: du_ChinhSuaDoUong_Post", ex.Message);
                    //-----Hiện lại dữ liệu trên view khi thông báo lỗi
                    this.resetDuLieu();
                    this.doDuLieuLenView(spSua, db);
                }
            }
            return View();
        }
        #endregion
        #region DELETE
        /// <summary>
        /// Hàm thực hiện xóa một đồ uống, sản phẩm khỏi csdl
        /// </summary>
        /// <param name="maDoUong"></param>
        public void xoaDoUong(int maDoUong)
        {
            if (xulyChung.duocCapNhat(idOfPage, "7"))
                try
                {
                    int kqLuu = 0;
                    qlCaPheEntities db = new qlCaPheEntities();
                    sanPham spXoa = db.sanPhams.SingleOrDefault(s => s.maSanPham == maDoUong);
                    if (spXoa != null)
                    {
                        db.sanPhams.Remove(spXoa);
                        kqLuu=db.SaveChanges();
                        if(kqLuu>0)
                            xulyChung.ghiNhatKyDtb(3, "Sản phẩm \"" + xulyDuLieu.traVeKyTuGoc(spXoa.tenSanPham) + " \"");
                    }
                    else
                        throw new Exception("Sản phẩm có mã " + maDoUong + " không tồn tại để xóa");
                }
                catch (Exception ex)
                {
                    xulyFile.ghiLoi("Class: DoUongController - Function: xoaDoUong", ex.Message);
                    RedirectToAction("ServerError", "Home");
                }
        }
        #endregion
        #region ORTHERS
        /// <summary>
        /// Hàm thực hiện tạo dữ liệu cho combobox và gán vào view thêm mới
        /// </summary>
        /// <param name="db"></param>
        private void taoDuLieuChoCbbTao(qlCaPheEntities db)
        {
            string htmlCbb = "";
            foreach (loaiSanPham loai in db.loaiSanPhams.ToList().OrderBy(l => l.tenLoai))
                htmlCbb += "<option value=\"" + loai.maLoai.ToString() + "\">" + xulyDuLieu.traVeKyTuGoc(loai.tenLoai) + "</option>";
            ViewBag.cbbLoai = htmlCbb;
        }
        /// <summary>
        /// Hàm thực hiện nhận dữ liệu từ giao diện và gán cho đối tượng sanPham3
        /// </summary>
        /// <param name="sp"></param>
        /// <param name="f"></param>
        private void layDuLieuTuView(sanPham sp, FormCollection f)
        {
            string loi = "";
            sp.tenSanPham = xulyDuLieu.xulyKyTuHTML(f["txtTenDoUong"]);
            if (sp.tenSanPham.Length <= 0)
                loi += "Vui lòng nhập tên sản phẩm cần tạo <br/>";
            sp.donGia = xulyDuLieu.doiChuoiSangInteger(f["txtDonGia"]);
            if (sp.donGia < 0)
                loi += "Đơn giá của sản phẩm phải lớn hơn hoặc bằng 0 <br/>";
            sp.moTa = xulyDuLieu.xulyKyTuHTML(f["txtMoTa"]);
            sp.thoiGianPhaChe = xulyDuLieu.doiChuoiSangInteger(f["txtThoiGian"]);
            if (sp.thoiGianPhaChe <= 0)
                loi += "Thời gian pha chế sản phẩm phải lớn hơn 0 <br/>";
            sp.ghiChu = xulyDuLieu.xulyKyTuHTML(f["txtGhiChu"]);
            sp.maLoai = xulyDuLieu.doiChuoiSangInteger(f["cbbLoai"]);
            if (sp.maLoai <= 0)
                loi += "Vui lòng chọn loại sản phẩm đồ uống <br/>";
            string pathHinh = f["pathHinh"];
            //------Kiểm tra xem có chọn hình không. 
            if (!pathHinh.Equals("")) //Nếu có chọn hình  thì luu hình vào csdl
            {
                pathHinh = xulyMaHoa.Decrypt(pathHinh);//Giải mã lại chuỗi đường dẫn lưu hình ảnh trên đĩa đã được mã hóa trong ajax
                sp.hinhAnh = xulyDuLieu.chuyenDoiHinhSangByteArray(pathHinh); //Lưu hình vào thuộc tính hinhAnh
            }
            else //-------Nếu không có chọn hình
                if (f["txtMaDoUong"] == null) //--------Kiểm tra xem có mã sản phẩm không. Nếu không tức là thêm mới và báo lỗi
                    loi += "Vui lòng chọn hình ảnh cho sản phẩm <br/>";
            if (loi.Length > 0)
                throw new Exception(loi);
        }
        /// <summary>
        /// Hàm thực hiện đổ dữ liệu của sản phẩm lên giao diện chỉnh sửa
        /// </summary>
        /// <param name="maDoUong"></param>
        /// <param name="db"></param>
        private void doDuLieuLenView(sanPham sp, qlCaPheEntities db)
        {
            ViewBag.txtMaDoUong = sp.maSanPham.ToString();
            //Hiển thị combobox loại
            string htmlCbb = "";
            foreach (loaiSanPham loai in db.loaiSanPhams.ToList())
                if (loai.maLoai == sp.maLoai) //-----Thiết lập thuộc tính select cho loại của sản phẩm.
                    htmlCbb += "<option selected value=\"" + loai.maLoai.ToString() + "\">" + xulyDuLieu.traVeKyTuGoc(loai.tenLoai) + "</option>";
                else
                    htmlCbb += "<option value=\"" + loai.maLoai.ToString() + "\">" + xulyDuLieu.traVeKyTuGoc(loai.tenLoai) + "</option>";
            ViewBag.cbbLoai = htmlCbb;
            ViewBag.txtTenDoUong = xulyDuLieu.traVeKyTuGoc(sp.tenSanPham);
            ViewBag.txtDonGia = sp.donGia.ToString();
            ViewBag.txtMoTa = xulyDuLieu.traVeKyTuGoc(sp.moTa);
            ViewBag.txtThoiGian = sp.thoiGianPhaChe.ToString();
            ViewBag.txtGhiChu = xulyDuLieu.traVeKyTuGoc(sp.ghiChu);
            //Hiển thị hình ảnh
            if (sp.hinhAnh != null)
                ViewBag.hinhDD = xulyDuLieu.chuyenByteHinhThanhSrcImage(sp.hinhAnh);

            ViewBag.TaskCongThuc = xulyChung.taoUrlCoTruyenThamSo("/CongThuc/ct_TableCongThuc", sp.maSanPham.ToString());
        }
        /// <summary>
        /// Hàm thực hiện reset lại dữ liệu ban đầu cho việc cập nhật hình ảnh
        /// </summary>
        private void resetDuLieu()
        {
            //Gán hình mặc định 
            ViewBag.hinhDD = "/images/gallery-upload-256.png";
            //Xóa tất cả các hình đã tải lên trong tập tin tạm            
            xulyFile.donDepTM(Server.MapPath("~/pages/temp/doUong"));
            //Nhúng script ajax thực hiện tải và cắt hình
            ViewBag.ScriptCropImage = createScriptAjax.scriptAjaxUpLoadAndCropImage("doUong", 268, 185, 268, 185);
            //Gán modal up và crop hình ảnh lên view
            ViewBag.modalChonHinh = createHTML.taoModalUpHinh();
        }
        /// <summary>
        /// Hàm tạo nội dung cho trang tùy thuộc vào trạng thái danh sách cần lấy
        /// </summary>
        /// <param name="trangThai"></param>
        private void thietLapThongSoChung(int trangThai)
        {
            //------Gán css active class ul tab danh sách
            if (trangThai == 1)
            {
                ViewBag.Style1 = "active"; ViewBag.Style2 = ""; ViewBag.Style3 = "";
            }
            else if (trangThai == 2)
            {
                ViewBag.Style3 = "active"; ViewBag.Style2 = ""; ViewBag.Style1 = "";
            }
            else
            {
                ViewBag.Style2 = "active"; ViewBag.Style3 = ""; ViewBag.Style1 = "";
            }
            //-------Gán script
            ViewBag.ScriptAjax = createScriptAjax.scriptAjaxXoaDoiTuong("DoUong/xoaDoUong?maDoUong=");
            ViewBag.ModalXoa = createHTML.taoCanhBaoXoa("Đồ uống");
            //----Nhúng script ajax hiển thị hình khi người dùng click vào tên sản phẩm
            ViewBag.ScriptAjaxXemHinh = createScriptAjax.scriptAjaxXemChiTietKhiClick("goiY", "maHinh", "DoUong/hienHinhDoUong?maDoUong=", "vungChiTiet", "modalChiTiet");
            //----Nhúng các tag html cho modal chi tiết
            ViewBag.ModalChiTiet = createHTML.taoModalChiTiet("modalChiTiet", "vungChiTiet", 1);
            //---Script hiện modal công thức pha chế của sản phẩm
            ViewBag.ScriptAjaxXemCongThuc = createScriptAjax.scriptAjaxXemChiTietKhiClick("goiYCongThuc", "maCongThuc", "CongThuc/AjaxXemChiTietCongThuc?maCongThuc=", "vungCongThuc", "modalCongThuc");
            //-----Tạo modal dạng lớn để chứa chi tiết các bước thực hiện của công thức
            ViewBag.ModalCongThuc = createHTML.taoModalChiTiet("modalCongThuc", "vungCongThuc", 3);
        }

        /// <summary>
        /// Hàm tạo script vẽ biểu đồ lịch sử giá
        /// </summary>
        /// <param name="sp"></param>
        /// <returns></returns>
        private string scriptBieuDoLichSuGia(sanPham sp)
        {
            string kq = "";
            List<lichSuGia> listLichSu = sp.lichSuGias.ToList();
            List<string> ngayCapNhat = new List<string>();
            List<string> giaCapNhats = new List<string>();
            if (listLichSu.Count > 0)
            {
                foreach (lichSuGia lichSu in listLichSu)
                    ngayCapNhat.Add(lichSu.ngayCapNhat.ToString());

                foreach (lichSuGia lichSu in listLichSu)
                    giaCapNhats.Add(lichSu.donGia.ToString());

                kq += createScriptCarvas.taoBieuDoMotDuong("configLichSuGia", ngayCapNhat, "Đơn giá", "blue", giaCapNhats, "Ngày cập nhật", "Đơn giá");

                string[] scriptTaoBieuDo = new string[] { createScriptCarvas.khoiTaoCarvas("canvasLichSuGia", "configLichSuGia") };
                kq += createScriptCarvas.loadCarvas(scriptTaoBieuDo);
            }
            return kq;
        }
        /// <summary>
        /// Hàm tạo nội dung thông báo lưu khi thêm sản phẩm thành công
        /// Có thêm chức năng chuyển đến trang tạo công thức pha chế cho sản phẩm vừa tạo
        /// </summary>
        /// <param name="sp"></param>
        /// <returns></returns>
        private string htmlTaoNoiDungThongBaoLuu(sanPham sp)
        {
            string kq = "";
            kq += "     <b>Đồ uống " + " &quot;<b>" + xulyDuLieu.traVeKyTuGoc(sp.tenSanPham) + "</b>&quot; đã lưu thành công. </b>";
            kq += "<ul>";
            kq += "   <li> &quot;<a class=\"col-red font-bold\" href=\"/DoUong/RouteDoUongChoDuyet\">Click vào đây </a>&quot; để chuyển đến trang danh sách sản phẩm</li>";
            kq += "   <li>&quot;<a class=\"col-blue font-bold guiRequest\" task=\"" + xulyChung.taoUrlCoTruyenThamSo("/CongThuc/ct_TaoMoiCongThuc", sp.maSanPham.ToString()) + "\" >Click vào đây </a>&quot; để chức năng cập nhật công thức pha chế cho sản phẩm ";
            kq += "</ul>";
            return kq;
        }
        #endregion
    }
}