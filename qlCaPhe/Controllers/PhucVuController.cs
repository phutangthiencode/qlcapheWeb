using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using qlCaPhe.Models;
using qlCaPhe.App_Start;
using qlCaPhe.Models.Business;

namespace qlCaPhe.Controllers
{
    public class PhucVuController : Controller
    {
        private string idOfPage = "602";
        /// <summary>
        /// Hàm lấy danh sách bàn có sản phẩm đã pha chế.
        /// </summary>
        /// <returns></returns>
        public string AjaxLayDanhSachBanCanPhucVu(int? page)
        {
            string html = ""; int trangHienHanh = (page ?? 1); int soPhanTu = 0;
            if (xulyChung.duocCapNhat(idOfPage, "7"))
            {
                try
                {
                    qlCaPheEntities db = new qlCaPheEntities();
                    soPhanTu = db.hoaDonTams.Where(h => h.trangthaiphucVu == 2).OrderBy(h => h.ngayLap).Count();
                    //----Lặp qua danh sách hoaDonTam có trangthaiphucVu=2 Đã pha chế Danh sách được sort theo ngày lập hóa đơn tăng dần.
                    foreach (hoaDonTam hoaDon in db.hoaDonTams.Where(h => h.trangthaiphucVu == 2).OrderBy(h => h.ngayLap).Skip((trangHienHanh - 1) * createHTML.pageSize).Take(createHTML.pageSize).ToList())
                    {
                        html += "<tr role=\"row\" class=\"odd\">";
                        html += "   <td>";
                        html += "       <a class=\"goiY\" maBan=\"" + hoaDon.maBan.ToString() + "\" style=\"cursor:pointer\">";
                        html += xulyDuLieu.traVeKyTuGoc(hoaDon.BanChoNgoi.tenBan) + "<span class=\"noiDungGoiY-right\">Click để xem chi tiết món đã đặt</span></a>";
                        html += "   </td>";
                        html += "   <td>" + hoaDon.ngayLap.ToString() + "</td>";
                        html += "   <td>" + new bHoaDonTam().layTongSoLuongSanPhamTrongHoaDon(hoaDon, 2) + "</td>";
                        html += "   <td>" + hoaDon.tongTien.ToString() + "</td>";
                        html += "   <td>";
                        html += "       <div class=\"btn-group\">";
                        html += "             <a task=\"" + xulyChung.taoUrlCoTruyenThamSo("Phucvu/capNhatSangDaGiao", hoaDon.maBan.ToString()) + "\" class=\"addSession btnGiaoTatCa  btn btn-warning waves-effect\"><i class=\"material-icons\">done_all</i><span>Giao tất cả</span></a>";
                        html += "       </div>";
                        html += "   </td>";
                        html += "</tr>";
                    }
                    html += "&&"; //------Ký tự xác định chuỗi html để gán lên giao diện
                    html += createHTML.taoPhanTrang(soPhanTu, createHTML.pageSize, trangHienHanh, "/PhucVu/pv_DanhSachBanCanPhucVuDoUong");
                }
                catch (Exception ex)
                {
                    xulyFile.ghiLoi("Class: PhucVuController - Function: AjaxLayDanhSachBanCanPhucVu", ex.Message);
                }
            }
            return html;
        }

        /// <summary>
        /// Hàm tạo giao diện danh sách bàn cần giao đồ uống đã pha chế
        /// </summary>
        /// <returns></returns>
        public ActionResult pv_DanhSachBanCanPhucVuDoUong()
        {
            if (xulyChung.duocCapNhat(idOfPage, "7"))
            {
                //-------Tạo modal xem chi tiết các sản phẩm đã order trong ctHoaDonTam
                ViewBag.ModalChiTiet = createHTML.taoModalChiTiet("modalChiTiet", "vungChiTiet", 2);
                xulyChung.ghiNhatKyDtb(1, "Danh mục bàn cần phục vụ");
                return View();
            }
            return null;
        }

        /// <summary>
        /// Hàm lấy danh sách các sản phẩm đã pha chế của 1 hóa đơn hiện lên modal
        /// </summary>
        /// <param name="maBan">Mã bàn cần lấy</param>
        /// <returns></returns>
        public string AjaxXemChiTietCacSanPhamCanPhucVuc(int maBan)
        {
            string htmlDetails = "";
            try
            {
                hoaDonTam hoaDon = new qlCaPheEntities().hoaDonTams.SingleOrDefault(h => h.maBan == maBan);
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
                    htmlDetails += "             <a task=\"" + xulyChung.taoUrlCoTruyenThamSo("Phucvu/capNhatSangDaGiao", hoaDon.maBan.ToString()) + "\" class=\" addSession btnGiaoTatCa btn btn-warning waves-effect\"><i class=\"material-icons\">done_all</i><span>Giao tất cả</span></a>";
                    htmlDetails += "        <a class=\"btn btn-default waves-effect\"  data-dismiss=\"modal\"><i class=\"material-icons\">close</i>Đóng lại</a>";
                    htmlDetails += "    </div>";
                    htmlDetails += "</div>";
                    xulyChung.ghiNhatKyDtb(5, "Chi tiết sản phẩm của bàn \"" + xulyDuLieu.traVeKyTuGoc(hoaDon.BanChoNgoi.tenBan) + " \"");
                }
            }
            catch (Exception ex)
            {
                xulyFile.ghiLoi("Class: PhucVuController - Function: AjaxXemChiTietCacSanPhamCanPhucVuc", ex.Message);
            }
            return htmlDetails;
        }

        /// <summary>
        /// hàm tạo bảng danh sách các món đã pha chế trong hoaDonTam được lấy từ database
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
            html += "           <th style=\"width:50%\">Tên món</th>";
            html += "           <th width=\"25%\">Số lượng</th>";
            html += "       </tr>";
            html += "   </thead>";
            html += "   <tbody>";
            //-------Lặp qua danh sách các món trong ctHoaDonTam chưa pha chế
            foreach (ctHoaDonTam ct in hoaDon.ctHoaDonTams.Where(c => c.trangThaiPhaChe == 2).ToList())
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

        /// <summary>
        /// Hàm cập nhật trạng thái của tất cả chi tiết trong hóa đơn thành đã giao
        /// </summary>
        /// <returns></returns>
        public string capNhatSangDaGiao(string param)
        {
            if (xulyChung.duocCapNhat(idOfPage, "7"))
            {
                //------Thay thế khoảng trắng thành dấu + khi truyền 2 tham số
                param = param.Replace(" ", "+");
                param = xulyMaHoa.DecryptWithKey(param, DateTime.Now.ToShortDateString());
                try
                {
                    param = param.Split('|')[1]; //-----param = request=maban
                    param = param.Replace("request=", ""); //----param = maban
                    int maBan = xulyDuLieu.doiChuoiSangInteger(param);
                    //-----Cập nhật dữ liệu trong chi tiết
                    qlCaPheEntities db = new qlCaPheEntities();
                    hoaDonTam hdSua = db.hoaDonTams.SingleOrDefault(s => s.maBan == maBan);
                    if (hdSua != null)
                    {
                        //------Chỉ lấy những sản phẩm đã pha chế và cập nhật
                        foreach (ctHoaDonTam ct in hdSua.ctHoaDonTams.Where(c => c.trangThaiPhaChe == 2).ToList())
                        {
                            ct.trangThaiPhaChe = 3;//Cập nhật sang sản phẩm đã giao
                            db.Entry(ct).State = System.Data.Entity.EntityState.Modified;
                            db.SaveChanges();
                        }
                        //------Cập nhật trạng thái phục vụ của hóa đơn
                        hdSua.trangthaiphucVu = -1;//------Cập nhật bàn sang trạng thái chờ order tiếp theo
                        db.Entry(hdSua).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                        xulyChung.ghiNhatKyDtb(4, "Đã bàn giao sản phẩm cho bàn \" " + xulyDuLieu.traVeKyTuGoc(hdSua.BanChoNgoi.tenBan) + " \"");
                    }
                    //-----reset session
                    Session["urlAction"] = "";
                }
                catch (Exception ex)
                {
                    xulyFile.ghiLoi("Class: PhucVuController - Function: capNhatSangDaGiao", ex.Message);
                }
            }
            return AjaxLayDanhSachBanCanPhucVu(1);
        }
    }
}