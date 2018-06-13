using qlCaPhe.App_Start;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using qlCaPhe.Models;

namespace qlCaPhe.Controllers
{
    public class NhaCungCapController : Controller
    {
        private string idOfPage = "703";
        #region CREATE
        /// <summary>
        /// Hàm tạo giao diện thêm mới nhà cung cấp
        /// </summary>
        /// <returns></returns>
        public ActionResult ncc_TaoMoiNhaCungCap()
        {
            if (xulyChung.duocTruyCap(idOfPage))
            {
                xulyChung.ghiNhatKyDtb(1, "Tạo mới nhà cung cấp");
                this.resetDuLieu();
            }
            return View();
        }
        /// <summary>
        /// Hàm thêm mới 1 nhà cung cấp vào CSDL
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ncc_TaoMoiNhaCungCap(nhaCungCap ncc, FormCollection f)
        {
            if (xulyChung.duocCapNhat(idOfPage, "7"))
            {
                string ndThongBao = ""; int kqLuu = 0;
                qlCaPheEntities db = new qlCaPheEntities();
                try
                {
                    this.layDuLieuTuView(ncc, f);
                    ncc.trangThai = true;
                    db.nhaCungCaps.Add(ncc);
                    kqLuu = db.SaveChanges();
                    if (kqLuu > 0)
                    {
                        ndThongBao = createHTML.taoNoiDungThongBao("Nhà cung cấp", xulyDuLieu.traVeKyTuGoc(ncc.tenNhaCC), "ncc_TableNhaCungCap");
                        xulyChung.ghiNhatKyDtb(2, ndThongBao);
                        this.resetDuLieu();
                    }
                }
                catch (Exception ex)
                {
                    ndThongBao = ex.Message;
                    xulyFile.ghiLoi("Class: NhaCungCapController - Function: ncc_TaoMoiNhaCungCap_Post", ex.Message);
                    this.resetDuLieu();
                    ////-----Hiện lại dữ liệu trên giao diện
                    this.doDuLieuLenView(ncc);

                }
                ViewBag.ThongBao = createHTML.taoThongBaoLuu(ndThongBao);
            }
            return View();
        }
        #endregion
        #region READ
        /// <summary>
        /// Hàm tạo giao diện danh sách nhà cung cấp đang cung cấp
        /// </summary>
        /// <returns></returns>
        public ActionResult ncc_TableNhaCungCap(int? page)
        {
            if (xulyChung.duocTruyCap(idOfPage))
                try
                {
                    this.createTableNhaCungCap(true, (page ?? 1), "/NhaCungCap/ncc_TableNhaCungCap");
                    xulyChung.ghiNhatKyDtb(1, "Danh mục nhà cung cấp");
                }
                catch (Exception ex)
                {
                    xulyFile.ghiLoi("Class: NhaCungCapController - Function: ncc_TableNhaCungCap", ex.Message);
                }
            return View();
        }
        /// <summary>
        /// hàm tạo giao diện danh sách nhà cung cấp tạm ngưng sử dụng
        /// </summary>
        /// <returns></returns>
        public ActionResult ncc_TableNhaCungCapNgung(int? page)
        {
            if (xulyChung.duocTruyCap(idOfPage))
                try
                {
                    this.createTableNhaCungCap(false, (page ?? 1), "/NhaCungCap/ncc_TableNhaCungCapNgung");
                    xulyChung.ghiNhatKyDtb(1, "Danh mục nhà cung cấp tạm ngưng cung cấp");
                }
                catch (Exception ex)
                {
                    xulyFile.ghiLoi("Class: NhaCungCapController - Function: ncc_TableNhaCungCapNgung", ex.Message);
                }
            return View();
        }
        /// <summary>
        /// Hàm thực hiện tạo bảng nhà cung cấp dựa vào trạng thái
        /// </summary>
        /// <param name="trangThai">Trạng thái nhà cung cấp cần lấy: 
        ///                          True: còn hợp tác, False: Ngừng hợp tác</param>
        /// <param name="trangHienHanh">Trang hiện tại đang duyệt danh sách</param>
        /// <param name="url">Đường dẫn đến danh sách tiếp theo</param>
        private void createTableNhaCungCap(bool trangThai, int trangHienHanh, string url)
        {
            string htmlTable = "";
            try
            {
                qlCaPheEntities db = new qlCaPheEntities();
                int soPhanTu = db.nhaCungCaps.Where(s => s.trangThai == trangThai).Count();
                ViewBag.PhanTrang = createHTML.taoPhanTrang(soPhanTu, trangHienHanh, url); //------cấu hình phân trang
                foreach (nhaCungCap ncc in db.nhaCungCaps.ToList().Where(n => n.trangThai == trangThai).Skip((trangHienHanh - 1) * createHTML.pageSize).Take(createHTML.pageSize))
                {
                    htmlTable += "<tr role=\"row\" class=\"odd\">";
                    htmlTable += "      <td>";
                    //-------Kiểm tra xem nhà cung cấp có website không. Nếu có thì thêm tag để vào website
                    if (!ncc.website.Equals(""))
                    {
                        htmlTable += "          <a href=\"" + xulyDuLieu.traVeKyTuGoc(ncc.website) + "\" target=\"_blank\" class=\"goiY\">" + xulyDuLieu.traVeKyTuGoc(ncc.tenNhaCC);
                        htmlTable += "              <span class=\"noiDungGoiY-right\">Click để vào trang web</span>";
                        htmlTable += "          </a>";
                    }
                    else
                        htmlTable += xulyDuLieu.traVeKyTuGoc(ncc.tenNhaCC);
                    htmlTable += "      </td>";
                    htmlTable += "      <td>" + xulyDuLieu.traVeKyTuGoc(ncc.maSoThue) + "</td>";
                    htmlTable += "      <td>" + xulyDuLieu.traVeKyTuGoc(ncc.diaChi) + "</td>";
                    htmlTable += "      <td>" + xulyDuLieu.traVeKyTuGoc(ncc.SDT) + "</td>";
                    htmlTable += "      <td>" + xulyDuLieu.traVeKyTuGoc(ncc.email) + "</td>";
                    htmlTable += "      <td>";
                    htmlTable += "          <div class=\"btn-group\">";
                    htmlTable += "              <button type=\"button\" class=\"btn btn-primary dropdown-toggle\" data-toggle=\"dropdown\">";
                    htmlTable += "                  Chức năng <span class=\"caret\"></span>";
                    htmlTable += "              </button>";
                    htmlTable += "              <ul class=\"dropdown-menu\" role=\"menu\">";
                    htmlTable += createTableData.taoNutChinhSua("/NhaCungCap/ncc_ChinhSuaNhaCungCap", ncc.maNhaCC.ToString());
                    //----Dựa vào trạng thái để hiện chức năng tương ứng
                    if (trangThai)//-------Trạng thái là còn hợp tác. Chuyển thành ngưng hợp tác
                        htmlTable += createTableData.taoNutCapNhat("/NhaCungCap/capNhatTrangThai", ncc.maNhaCC.ToString(), "col-orange", "clear", "Ngừng cung cấp");
                    else
                        htmlTable += createTableData.taoNutCapNhat("/NhaCungCap/capNhatTrangThai", ncc.maNhaCC.ToString(), "col-orange", "refresh", "Cung cấp lại");
                    htmlTable += createTableData.taoNutXoaBo(ncc.maNhaCC.ToString());
                    htmlTable += "              </ul>";
                    htmlTable += "          </div>";
                    htmlTable += "      </td>";
                    htmlTable += "</tr>";
                }
            }
            catch (Exception ex)
            {
                xulyFile.ghiLoi("Class: NhaCungCapController - Function: createTableNhaCungCap", ex.Message);
                Response.Redirect(xulyChung.layTenMien() + "/Home/ServerError");
            }
            ViewBag.TableData = htmlTable;
            ViewBag.ScriptAjax = createScriptAjax.scriptAjaxXoaDoiTuong("NhaCungCap/xoaNhaCungCap?maNhaCC=");
            ViewBag.ModalXoa = createHTML.taoCanhBaoXoa("Nhà cung cấp");
        }
        #endregion
        #region UPDATE
        /// <summary>
        /// Hàm thực hiện cập nhật trạng thái của nhà cung cấp
        /// Trạng thái cập nhật sẽ ngược lại trạng thái hiện tại
        /// </summary>
        /// <param name="maNhaCC"></param>
        public void capNhatTrangThai()
        {
            if (xulyChung.duocCapNhat(idOfPage, "7"))
                try
                {
                    int kqLuu = 0;
                    string param = xulyChung.nhanThamSoTrongSession();
                    if (param.Length > 0)
                    {
                        int maNhaCC = xulyDuLieu.doiChuoiSangInteger(param);
                        qlCaPheEntities db = new qlCaPheEntities();
                        nhaCungCap nccSua = db.nhaCungCaps.SingleOrDefault(n => n.maNhaCC == maNhaCC);
                        if (nccSua != null)
                        {
                            bool trangThaiCu = nccSua.trangThai;//Lưu lại trạng thái cũ để chuyển đến trang tương ứng
                            nccSua.trangThai = !nccSua.trangThai;
                            db.Entry(nccSua).State = System.Data.Entity.EntityState.Modified;
                            kqLuu = db.SaveChanges();
                            if (kqLuu > 0)
                            {
                                xulyChung.ghiNhatKyDtb(4, "Cập nhật trạng thái nhà cung cấp \" " + xulyDuLieu.traVeKyTuGoc(nccSua.tenNhaCC) + " \"");
                                if (trangThaiCu)//Chuyển đến danh sách nhà cung cấp còn cung cấp
                                    Response.Redirect(xulyChung.layTenMien() + "/NhaCungCap/ncc_TableNhaCungCap");
                                else
                                    Response.Redirect(xulyChung.layTenMien() + "/NhaCungCap/ncc_TableNhaCungCapNgung");
                            }
                        }
                        else
                            throw new Exception("Nhà cung cấp có mã " + maNhaCC.ToString() + " cần cập nhật không tồn tại");
                    }
                    else throw new Exception("không nhận được tham số");
                }
                catch (Exception ex)
                {
                    xulyFile.ghiLoi("Class: NhaCungCapController - Function: capNhatTrangThai", ex.Message);
                    Response.Redirect(xulyChung.layTenMien() + "/Home/ServerError");
                }
        }
        /// <summary>
        /// hàm tạo giao diện chình sửa thông tin nhà cung cấp
        /// </summary>
        /// <param name="maNhaCC"></param>
        /// <returns></returns>
        public ActionResult ncc_ChinhSuaNhaCungCap()
        {
            if (xulyChung.duocCapNhat(idOfPage, "7"))
                try
                {
                    string param = xulyChung.nhanThamSoTrongSession();
                    if (param.Length > 0)
                    {
                        int maNhaCC = xulyDuLieu.doiChuoiSangInteger(param);
                        this.resetDuLieu();
                        nhaCungCap nccSua = new qlCaPheEntities().nhaCungCaps.SingleOrDefault(n => n.maNhaCC == maNhaCC);
                        if (nccSua != null)
                        {
                            xulyChung.ghiNhatKyDtb(1, "Chỉnh sửa nhà cung cấp \" " + xulyDuLieu.traVeKyTuGoc(nccSua.tenNhaCC) + " \"");
                            this.doDuLieuLenView(nccSua);
                        }
                        else
                            throw new Exception("Nhà cung cấp có mã " + maNhaCC.ToString() + " không tồn tại để cập nhật");
                    }
                    else throw new Exception("không nhận được tham số");
                }
                catch (Exception ex)
                {
                    xulyFile.ghiLoi("Class: NhaCungCapController - Function: ncc_ChinhSuaNhaCungCap_Get", ex.Message);
                    return RedirectToAction("PageNotFound", "Home");
                }
            return View();
        }
        /// <summary>
        /// Hàm thực hiện cập nhật lại thông tin nhà cung cấp vào CSDL
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ncc_ChinhSuaNhaCungCap(FormCollection f)
        {
            if (xulyChung.duocCapNhat(idOfPage, "7"))
            {
                int kqLuu = 0;
                qlCaPheEntities db = new qlCaPheEntities(); nhaCungCap nccSua = new nhaCungCap();
                try
                {
                    int maNhaCC = Convert.ToInt32(f["txtMaNhaCungCap"]);
                    nccSua = db.nhaCungCaps.SingleOrDefault(n => n.maNhaCC == maNhaCC);
                    if (nccSua != null)
                    {
                        this.layDuLieuTuView(nccSua, f);
                        db.Entry(nccSua).State = System.Data.Entity.EntityState.Modified;
                        kqLuu = db.SaveChanges();
                        if (kqLuu > 0)
                        {
                            xulyChung.ghiNhatKyDtb(4, "Nhà cung cấp \" " + xulyDuLieu.traVeKyTuGoc(nccSua.tenNhaCC) + " \"");
                            this.resetDuLieu();
                            //------Dựa vào trạng thái của nhà cung cấp đã chỉnh sửa để chuyển đến giao diện tương ứng
                            if (nccSua.trangThai)
                                return RedirectToAction("ncc_TableNhaCungCap");
                            else
                                return RedirectToAction("ncc_TableNhaCungCapNgung");
                        }
                    }
                }
                catch (Exception ex)
                {
                    ViewBag.ThongBao = createHTML.taoThongBaoLuu(ex.Message);
                    xulyFile.ghiLoi("Class: NhaCungCapController - Function: ncc_ChinhSuaNhaCungCap_Post", ex.Message);
                    this.resetDuLieu();
                    ////-----Hiện lại dữ liệu trên giao diện
                    this.doDuLieuLenView(nccSua);
                }
            }
            return View();
        }
        #endregion
        #region DELETE
        /// <summary>
        /// Hàm thực hiện xóa 1 nhà cung cấp khỏi CSDL
        /// </summary>
        /// <param name="maNhaCC"></param>
        public void xoaNhaCungCap(int maNhaCC)
        {
            try
            {
                int kqLuu = 0;
                qlCaPheEntities db = new qlCaPheEntities();
                nhaCungCap nccXoa = db.nhaCungCaps.SingleOrDefault(n => n.maNhaCC == maNhaCC);
                if (nccXoa != null)
                {
                    bool trangThai = nccXoa.trangThai;//Lưu lại trạng thái để chuyển đến trang danh sách nhà cung cấp trước đó
                    db.nhaCungCaps.Remove(nccXoa);
                    kqLuu = db.SaveChanges();
                    if (kqLuu > 0)
                    {
                        xulyChung.ghiNhatKyDtb(4, "Nhà cung cấp \"" + xulyDuLieu.traVeKyTuGoc(nccXoa.tenNhaCC) + " \"");
                        if (trangThai)//Chuyển đến danh sách nhà cung cấp còn cung cấp
                            Response.Redirect(xulyChung.layTenMien() + "/NhaCungCap/ncc_TableNhaCungCap");
                        else
                            Response.Redirect(xulyChung.layTenMien() + "/NhaCungCap/ncc_TableNhaCungCapNgung");
                    }
                }
                else
                    throw new Exception("Nhà cung cấp có mã " + maNhaCC.ToString() + " không tồn tại để xóa");
            }
            catch (Exception ex)
            {
                xulyFile.ghiLoi("Class: NhaCungCapController - Function: xoaNhaCungCap", ex.Message);
                Response.Redirect(xulyChung.layTenMien() + "/Home/ServerError");
            }
        }
        #endregion
        #region ORTHERS
        /// <summary>
        /// hàm thực hiện reset lại dữ liệu trên view
        /// </summary>
        private void resetDuLieu()
        {
            //Gán hình mặc định 
            ViewBag.hinhDD = "/images/gallery-upload-256.png";
            //Xóa tất cả các hình đã tải lên trong tập tin tạm            
            xulyFile.donDepTM(Server.MapPath("~/pages/temp/nhaCungCap"));
            //Nhúng script ajax thực hiện tải và cắt hình
            ViewBag.ScriptCropImage = createScriptAjax.scriptAjaxUpLoadAndCropImage("nhaCungCap", 300, 300, 300, 300);
            //Gán modal up và crop hình ảnh lên view
            ViewBag.modalChonHinh = createHTML.taoModalUpHinh();
        }
        /// <summary>
        /// Hàm thực hiện lấy dữ liệu từ giao diện gán cho các thuộc tính của nhà cung cấp
        /// </summary>
        /// <param name="ncc"></param>
        /// <param name="f"></param>
        private void layDuLieuTuView(nhaCungCap ncc, FormCollection f)
        {
            string loi = "";
            ncc.tenNhaCC = xulyDuLieu.xulyKyTuHTML(f["txtTenNCC"]);
            if (ncc.tenNhaCC.Length <= 0)
                loi += "Vui lòng nhập tên cho nhà cung cấp <br/>";
            ncc.diaChi = xulyDuLieu.xulyKyTuHTML(f["txtDiaChi"]);
            if (ncc.diaChi.Length <= 0)
                loi += "Vui lòng nhập địa chỉ của nhà cung cấp <br/>";
            ncc.SDT = xulyDuLieu.xulyKyTuHTML(f["txtSDT"]);
            if (ncc.SDT.Length <= 0)
                loi += "Vui lòng nhập số điện thoại liên lạc của nhà cung cấp <br/>";
            ncc.FAX = xulyDuLieu.xulyKyTuHTML(f["txtFax"]);
            ncc.email = xulyDuLieu.xulyKyTuHTML(f["txtEmail"]);
            ncc.website = xulyDuLieu.xulyKyTuHTML(f["txtWebSite"]);
            ncc.ghiChu = xulyDuLieu.xulyKyTuHTML(f["txtGhiChu"]);
            ncc.maSoThue = xulyDuLieu.xulyKyTuHTML(f["txtMST"]);
            string ngayLap = f["txtNgayThanhLap"];
            if (!ngayLap.Equals(""))
                ncc.ngayThanhLap = DateTime.Parse(ngayLap);
            string pathHinh = f["pathHinh"];
            //------Kiểm tra xem có chọn hình không. 
            if (!pathHinh.Equals("")) //Nếu có chọn hình  thì luu hình vào csdl
            {
                pathHinh = xulyMaHoa.Decrypt(pathHinh);//Giải mã lại chuỗi đường dẫn lưu hình ảnh trên đĩa đã được mã hóa trong ajax
                ncc.logo = xulyDuLieu.chuyenDoiHinhSangByteArray(pathHinh); //Lưu hình vào thuộc tính hinhAnh
            }
            //Nếu có lỗi thì xuất thông báo
            if (loi.Length > 0)
                throw new Exception(loi);
        }
        /// <summary>
        /// hàm thực hiện đổ dữ liệu của 1 nhà cung cấp lên giao diện
        /// </summary>
        /// <param name="ncc"></param>
        private void doDuLieuLenView(nhaCungCap ncc)
        {
            ViewBag.txtMaNhaCungCap = ncc.maNhaCC.ToString();
            ViewBag.txtTenNCC = xulyDuLieu.traVeKyTuGoc(ncc.tenNhaCC);
            ViewBag.txtDiaChi = xulyDuLieu.traVeKyTuGoc(ncc.diaChi);
            ViewBag.txtSDT = xulyDuLieu.traVeKyTuGoc(ncc.SDT);
            ViewBag.txtFax = xulyDuLieu.traVeKyTuGoc(ncc.FAX);
            ViewBag.txtEmail = xulyDuLieu.traVeKyTuGoc(ncc.email);
            ViewBag.txtWebSite = xulyDuLieu.traVeKyTuGoc(ncc.website);
            ViewBag.txtGhiChu = xulyDuLieu.traVeKyTuGoc(ncc.ghiChu);
            ViewBag.txtMST = xulyDuLieu.traVeKyTuGoc(ncc.maSoThue);
            ViewBag.txtNgayThanhLap = string.Format("{0:yyyy-MM-dd}", ncc.ngayThanhLap);
            if (ncc.logo != null)
                ViewBag.hinhDD = xulyDuLieu.chuyenByteHinhThanhSrcImage(ncc.logo);
        }
        #endregion
    }
}