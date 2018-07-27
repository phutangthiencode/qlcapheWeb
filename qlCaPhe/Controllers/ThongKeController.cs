using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using qlCaPhe.App_Start;
using qlCaPhe.Models;
using System.Data.SqlClient;

namespace qlCaPhe.Controllers
{
    public class ThongKeController : Controller
    {
        #region THỐNG KÊ DOANH THU THEO THỜI ĐIỂM
        private static string idOfPageDoanhThuTheoThoiDiem = "1201";
        #region DOANH THU THEO NGÀY
        /// <summary>
        /// Hàm tạo vùng thống kê doanh thu theo ngày
        /// </summary>
        /// <returns></returns>
        public ActionResult tke_DoanhThuTheoNgay()
        {
            if (xulyChung.duocTruyCap(idOfPageDoanhThuTheoThoiDiem))
            {
                ViewBag.ScriptAjax = createScriptCarvas.ScriptAjaxThongKeDoanhThu("/ThongKe/GetJsonDoanhThuTheoNgay", "chart-ngay");
                return View();
            }
            return RedirectToAction("PageNotFound", "Home");
        }
        /// <summary>
        /// Hàm ajax lấy danh sách hóa đơn theo ngày
        /// </summary>
        /// <param name="param">Ngày cần lấy danh sách hóa đơn</param>
        /// <returns>Json object các hóa đơn</returns>
        [HttpGet]
        public JsonResult GetJsonDoanhThuTheoNgay(string param)
        {
            List<object> listHoaDon = new List<object>();
            if (xulyChung.duocTruyCap(idOfPageDoanhThuTheoThoiDiem))
                try
                {
                    DateTime date = xulyDuLieu.doiChuoiSangDateTime(param);
                    IEnumerable<object> listKQ = new qlCaPheEntities().thongKeHoaDonTheoNgay(date);
                    long tongTienTamTinh = 0;
                    foreach (object x in listKQ.ToList())
                    {
                        string gio = xulyDuLieu.layThuocTinhTrongMotObject(x, "gio");
                        //TimeSpan time = xulyDuLieu.doiChuoiSangDateTime(gio);
                        //---------Lấy tổng tiền thanh toán tạm tính của từng ngày
                        long tienTamTinh = xulyDuLieu.doiChuoiSangLong(xulyDuLieu.layThuocTinhTrongMotObject(x, "tongTien"));
                        tongTienTamTinh += tienTamTinh;
                        object a = new
                        {
                            maHD = gio,
                            tamTinh = tienTamTinh,
                            tongTien = xulyDuLieu.doiVND(tongTienTamTinh)
                        };
                        listHoaDon.Add(a);
                    }
                }
                catch (Exception ex)
                {
                    xulyFile.ghiLoi("Class: ThongKeController - Function: GetJsonDoanhThuTheoNgay", ex.Message);
                }
            return Json(listHoaDon, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// Hàm ajax lấy danh sách sản phẩm bán theo ngày
        /// </summary>
        /// <param name="param">Ngày cần xem thống kê</param>
        /// <returns>Mảng Json object chứa danh sách sản phẩm đã bán trong ngày</returns>
        [HttpGet]
        public JsonResult GetJsonSanPhamTheoNgay(string param)
        {
            List<object> listHoaDon = new List<object>();
            if (xulyChung.duocTruyCapKhongChuyenTiep(idOfPageDoanhThuTheoThoiDiem))
                try
                {
                    DateTime date = xulyDuLieu.doiChuoiSangDateTime(param);
                    IEnumerable<object> listIEnumerable = new qlCaPheEntities().thongKeDoanhThuTheoSanPhamTheoNgay(date);
                    this.taoDuLieuThongKeSoLanBanSanPham(listIEnumerable, listHoaDon);
                }
                catch (Exception ex)
                {
                    xulyFile.ghiLoi("Class: ThongKeController - Function: GetJsonSanPhamTheoNgay", ex.Message);
                }
            return Json(listHoaDon, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region DOANH THU THEO TUẦN
        /// <summary>
        /// Hàm tạo giao diện thống kê doanh thu theo tuần
        /// </summary>
        /// <returns></returns>
        public ActionResult tke_DoanhThuTheoTuan()
        {
            if (xulyChung.duocTruyCap(idOfPageDoanhThuTheoThoiDiem))
            {
                ViewBag.ScriptAjax = createScriptCarvas.ScriptAjaxThongKeDoanhThu("/ThongKe/GetJsonDoanhThuTheoTuan", "chart-tuan");
                return View();
            }
            return RedirectToAction("PageNotFound", "Home");
        }
        /// <summary>
        /// Hàm lấy danh sách hóa đơn theo tuần
        /// </summary>
        /// <param name="param">Ngày bắt đầu tuần cần lấy</param>
        /// <returns>Json object danh sách hóa đơn</returns>
        [HttpGet]
        public JsonResult GetJsonDoanhThuTheoTuan(string param)
        {
            List<object> listHoaDon = new List<object>();
            if (xulyChung.duocTruyCap(idOfPageDoanhThuTheoThoiDiem))
                try
                {
                    DateTime startDate = xulyDuLieu.doiChuoiSangDateTime(param);
                    DateTime endDate = startDate.AddDays(7); //-------Tính đến ngày của tuần sau bắt đầu từ ngày hiện tại
                    IEnumerable<object> listKQ = new qlCaPheEntities().thongKeHoaDonTheoTuan(startDate, endDate);
                    long tongTienTamTinh = 0;
                    foreach (object x in listKQ.ToList())
                    {
                        string ngay = xulyDuLieu.layThuocTinhTrongMotObject(x, "ngay");
                        DateTime date = xulyDuLieu.doiChuoiSangDateTime(ngay);
                        //---------Lấy tổng tiền thanh toán tạm tính của từng ngày
                        long tienTamTinh = xulyDuLieu.doiChuoiSangLong(xulyDuLieu.layThuocTinhTrongMotObject(x, "tongTien"));
                        tongTienTamTinh += tienTamTinh;
                        object a = new
                        {
                            maHD = date.Date.ToShortDateString(),
                            tamTinh = tienTamTinh,
                            tongTien = xulyDuLieu.doiVND(tongTienTamTinh)
                        };
                        listHoaDon.Add(a);
                    }
                }
                catch (Exception ex)
                {
                    xulyFile.ghiLoi("Class: ThongKeController - Function: GetJsonDoanhThuTheoTuan", ex.Message);
                }
            return Json(listHoaDon, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Hàm ajax lấy danh sách sản phẩm bán theo tuần
        /// </summary>
        /// <param name="param">Ngày bắt đầu cần xem</param>
        /// <returns>Mảng Json object chứa danh sách sản phẩm đã bán trong tuần</returns>
        [HttpGet]
        public JsonResult GetJsonSanPhamTheoTuan(string param)
        {
            List<object> listHoaDon = new List<object>();
            if (xulyChung.duocTruyCap(idOfPageDoanhThuTheoThoiDiem))
                try
                {
                    DateTime date = xulyDuLieu.doiChuoiSangDateTime(param);
                    IEnumerable<object> listIEnumerable = new qlCaPheEntities().thongKeDoanhThuTheoSanPhamTheoTuan(date);
                    this.taoDuLieuThongKeSoLanBanSanPham(listIEnumerable, listHoaDon);
                }
                catch (Exception ex)
                {
                    xulyFile.ghiLoi("Class: ThongKeController - Function: GetJsonSanPhamTheoTuan", ex.Message);
                }
            return Json(listHoaDon, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region DOANH THU THEO THÁNG

        /// <summary>
        /// Hàm tạo giao diện thống kê doanh thu theo Tháng
        /// </summary>
        /// <returns></returns>
        public ActionResult tke_DoanhThuTheoThang()
        {
            if (xulyChung.duocTruyCap(idOfPageDoanhThuTheoThoiDiem))
            {
                //--------Tạo dữ liệu cho cbb năm
                this.taoDuLieuChoCbbNam();
                ViewBag.ScriptAjax = createScriptCarvas.ScriptAjaxThongKeDoanhThu("/ThongKe/GetJsonDoanhThuTheoThang", "chart-tuan");
                return View();
            }

            return RedirectToAction("PageNotFound", "Home");
        }
        /// <summary>
        /// Hàm lấy danh sách hóa đơn theo tháng của một năm
        /// </summary>
        /// <param name="param">Năm cần lấy dữ liệu thống kê theo tháng</param>
        /// <returns>Json object danh sách hóa đơn</returns>
        [HttpGet]
        public JsonResult GetJsonDoanhThuTheoThang(string param)
        {
            List<object> listHoaDon = new List<object>();
            if (xulyChung.duocTruyCap(idOfPageDoanhThuTheoThoiDiem))
                try
                {
                    int nam = xulyDuLieu.doiChuoiSangInteger(param);
                    IEnumerable<object> listThongKe = new qlCaPheEntities().thongKeHoaDonTheoThang(nam);
                    long tongTienTamTinh = 0;
                    foreach (object x in listThongKe.ToList())
                    {
                        string thang = xulyDuLieu.layThuocTinhTrongMotObject(x, "thang");
                        //---------Lấy tổng tiền thanh toán tạm tính của từng ngày
                        long tienTamTinh = xulyDuLieu.doiChuoiSangLong(xulyDuLieu.layThuocTinhTrongMotObject(x, "tongTien"));
                        tongTienTamTinh += tienTamTinh;
                        object a = new
                        {
                            maHD = thang,
                            tamTinh = tienTamTinh,
                            tongTien = xulyDuLieu.doiVND(tongTienTamTinh)
                        };
                        listHoaDon.Add(a);
                    }
                }
                catch (Exception ex)
                {
                    xulyFile.ghiLoi("Class: ThongKeController - Function: GetJsonDoanhThuTheoThang", ex.Message);
                }
            return Json(listHoaDon, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Hàm ajax lấy danh sách sản phẩm bán theo tháng
        /// </summary>
        /// <param name="param">Tháng cần thống kê</param>
        /// <returns>Mảng Json object chứa danh sách sản phẩm đã bán trong tháng</returns>
        [HttpGet]
        public JsonResult GetJsonSanPhamTheoThang(string param)
        {
            List<object> listHoaDon = new List<object>();
            if (xulyChung.duocTruyCap(idOfPageDoanhThuTheoThoiDiem))
                try
                {
                    int thang = xulyDuLieu.doiChuoiSangInteger(param);
                    IEnumerable<object> listIEnumerable = new qlCaPheEntities().thongKeDoanhThuTheoSanPhamTheoThang(thang);
                    this.taoDuLieuThongKeSoLanBanSanPham(listIEnumerable, listHoaDon);
                }
                catch (Exception ex)
                {
                    xulyFile.ghiLoi("Class: ThongKeController - Function: GetJsonSanPhamTheoThang", ex.Message);
                }
            return Json(listHoaDon, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region DOANH THU THEO QUÝ

        /// <summary>
        /// Hàm tạo giao diện thống kê doanh thu theo quý
        /// </summary>
        /// <returns></returns>
        public ActionResult tke_DoanhThuTheoQuy()
        {
            if (xulyChung.duocTruyCap(idOfPageDoanhThuTheoThoiDiem))
            {
                //--------Tạo dữ liệu cho cbb năm
                this.taoDuLieuChoCbbNam();
                ViewBag.ScriptAjax = createScriptCarvas.ScriptAjaxThongKeDoanhThu("/ThongKe/GetJsonDoanhThuTheoQuy", "chart-quy");
                return View();
            }

            return RedirectToAction("PageNotFound", "Home");
        }
        /// <summary>
        /// Hàm lấy danh sách hóa đơn theo tuần
        /// </summary>
        /// <param name="param">Ngày bắt đầu tuần cần lấy</param>
        /// <returns>Json object danh sách hóa đơn</returns>
        [HttpGet]
        public JsonResult GetJsonDoanhThuTheoQuy(string param)
        {
            List<object> listHoaDon = new List<object>();
            if (xulyChung.duocTruyCap(idOfPageDoanhThuTheoThoiDiem))
                try
                {
                    int nam = xulyDuLieu.doiChuoiSangInteger(param);
                    IEnumerable<object> listThongKe = new qlCaPheEntities().thongKeHoaDonTheoQuy(nam);
                    long tongTienTamTinh = 0;
                    foreach (object x in listThongKe.ToList())
                    {
                        string thang = xulyDuLieu.layThuocTinhTrongMotObject(x, "quy");
                        //---------Lấy tổng tiền thanh toán tạm tính của từng ngày
                        long tienTamTinh = xulyDuLieu.doiChuoiSangLong(xulyDuLieu.layThuocTinhTrongMotObject(x, "tongTien"));
                        tongTienTamTinh += tienTamTinh;
                        object a = new
                        {
                            maHD = thang,
                            tamTinh = tienTamTinh,
                            tongTien = xulyDuLieu.doiVND(tongTienTamTinh)
                        };
                        listHoaDon.Add(a);
                    }
                }
                catch (Exception ex)
                {
                    xulyFile.ghiLoi("Class: ThongKeController - Function: GetJsonDoanhThuTheoQuy", ex.Message);
                }
            return Json(listHoaDon, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Hàm ajax lấy danh sách sản phẩm bán theo quý
        /// </summary>
        /// <param name="param">Quý cần thống kê</param>
        /// <returns>Mảng Json object chứa danh sách sản phẩm đã bán trong quý</returns>
        [HttpGet]
        public JsonResult GetJsonSanPhamTheoQuy(string param)
        {
            List<object> listHoaDon = new List<object>();
            if (xulyChung.duocTruyCap(idOfPageDoanhThuTheoThoiDiem))
                try
                {
                    int quy = xulyDuLieu.doiChuoiSangInteger(param);
                    IEnumerable<object> listIEnumerable = new qlCaPheEntities().thongKeDoanhThuTheoSanPhamTheoQuy(quy);
                    this.taoDuLieuThongKeSoLanBanSanPham(listIEnumerable, listHoaDon);
                }
                catch (Exception ex)
                {
                    xulyFile.ghiLoi("Class: ThongKeController - Function: GetJsonSanPhamTheoQuy", ex.Message);
                }
            return Json(listHoaDon, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region DOANH THU THEO NĂM

        /// <summary>
        /// Hàm tạo giao diện thống kê doanh thu theo năm
        /// </summary>
        /// <returns></returns>
        public ActionResult tke_DoanhThuTheoNam()
        {
            if (xulyChung.duocTruyCap(idOfPageDoanhThuTheoThoiDiem))
            {
                //--------Tạo dữ liệu cho cbb năm
                this.taoDuLieuChoCbbNam();
                ViewBag.ScriptAjax = createScriptCarvas.ScriptAjaxThongKeDoanhThu("/ThongKe/GetJsonDoanhThuTheoNam", "chart-nam");
                return View();
            }

            return RedirectToAction("PageNotFound", "Home");
        }
        /// <summary>
        /// Hàm lấy danh sách hóa đơn theo năm
        /// </summary>
        /// <returns>Json object danh sách hóa đơn</returns>
        [HttpGet]
        public JsonResult GetJsonDoanhThuTheoNam()
        {
            List<object> listHoaDon = new List<object>();
            if (xulyChung.duocTruyCap(idOfPageDoanhThuTheoThoiDiem))
                try
                {
                    IEnumerable<object> listThongKe = new qlCaPheEntities().thongKeHoaDonTheoNam();
                    long tongTienTamTinh = 0;
                    foreach (object x in listThongKe.ToList())
                    {
                        string thang = xulyDuLieu.layThuocTinhTrongMotObject(x, "nam");
                        //---------Lấy tổng tiền thanh toán tạm tính của từng ngày
                        long tienTamTinh = xulyDuLieu.doiChuoiSangLong(xulyDuLieu.layThuocTinhTrongMotObject(x, "tongTien"));
                        tongTienTamTinh += tienTamTinh;
                        object a = new
                        {
                            maHD = thang,
                            tamTinh = tienTamTinh,
                            tongTien = xulyDuLieu.doiVND(tongTienTamTinh)
                        };
                        listHoaDon.Add(a);
                    }
                }
                catch (Exception ex)
                {
                    xulyFile.ghiLoi("Class: ThongKeController - Function: GetJsonDoanhThuTheoNam", ex.Message);
                }
            return Json(listHoaDon, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Hàm ajax lấy danh sách sản phẩm bán theo năm
        /// </summary>
        /// <param name="param">Năm cần thống kê</param>
        /// <returns>Mảng Json object chứa danh sách sản phẩm đã bán trong Năm</returns>
        [HttpGet]
        public JsonResult GetJsonSanPhamTheoNam(string param)
        {
            List<object> listHoaDon = new List<object>();
            if (xulyChung.duocTruyCap(idOfPageDoanhThuTheoThoiDiem))
                try
                {
                    int nam = xulyDuLieu.doiChuoiSangInteger(param);
                    IEnumerable<object> listIEnumerable = new qlCaPheEntities().thongKeDoanhThuTheoSanPhamTheoNam(nam);
                    this.taoDuLieuThongKeSoLanBanSanPham(listIEnumerable, listHoaDon);
                }
                catch (Exception ex)
                {
                    xulyFile.ghiLoi("Class: ThongKeController - Function: GetJsonSanPhamTheoQuy", ex.Message);
                }
            return Json(listHoaDon, JsonRequestBehavior.AllowGet);
        }
        #endregion

        /// <summary>
        /// Hàm tạo dữ liệu cho các combobox năm trên các giao diện
        /// Bắt đầu từ năm 2017 đến năm hiện tại
        /// </summary>
        private void taoDuLieuChoCbbNam()
        {
            string cbbNam = "";
            int namHienTai = DateTime.Now.Year;
            for (int i = 2017; i <= namHienTai; i++)
                cbbNam += "<option value=\"" + i.ToString() + "\"" + ">" + i.ToString() + "</option>";
            ViewBag.CbbNam = cbbNam;
        }
        /// <summary>
        /// Hàm đọc dữ liệu đã lấy về từ store procedure thống kê và thêm vào List object json kết quả
        /// </summary>
        /// <param name="listIEnumerable">IEnumerable chứa kết quả nhận về từ store procedure</param>
        /// <param name="listKQ">List object cần thêm dữ liệu vào JSon</param>
        private void taoDuLieuThongKeSoLanBanSanPham(IEnumerable<object> listIEnumerable, List<object> listKQ)
        {
            long tongDoanhThu = 0;
            foreach (object x in listIEnumerable.ToList())
            {
                string soLanBan = xulyDuLieu.layThuocTinhTrongMotObject(x, "soLanBan");
                //---------Lấy tổng tiền thanh toán tạm tính của từng ngày
                long tongTienTamTinh = xulyDuLieu.doiChuoiSangLong(xulyDuLieu.layThuocTinhTrongMotObject(x, "tongTien"));
                tongDoanhThu += tongTienTamTinh;
                object a = new
                {
                    soLanBan = xulyDuLieu.doiChuoiSangInteger(soLanBan),
                    tenSP = xulyDuLieu.layThuocTinhTrongMotObject(x, "tenSanPham"),
                    tongTien = tongTienTamTinh,
                    tongDoanhThu = xulyDuLieu.doiVND(tongDoanhThu)
                };
                listKQ.Add(a);
            }
        }
        #endregion

        #region THỐNG KÊ TỔNG THU THEO SẢN PHẨM
        private static string idOfPageDoanhThuTheoSanPham = "1202";

        #region THEO NGÀY
        /// <summary>
        /// Hàm tạo giao diện thống kê tổng doanh thu của 1 sản phẩm theo ngày
        /// </summary>
        /// <returns></returns>
        public ActionResult tke_ThongKeThuSanPhamNgay()
        {
            if (xulyChung.duocTruyCap(idOfPageDoanhThuTheoSanPham))
            {
                this.taoCbbSanPham();
                return View();
            }
            return RedirectToAction("PageNotFound", "Home");
        }

        /// <summary>
        /// Hàm ajax lấy thống kê tổng doanh thu của sản phẩm theo ngày
        /// </summary>
        /// <param name="time">Ngày cần xem thống kê</param>
        /// <param name="maSP">Mã sản phẩm cần xem thống kê</param>
        /// <returns>Json object các thởi điểm mua hàng và tổng tiền</returns>
        [HttpGet]
        public JsonResult GetJsonDoanhThuSanPhamTheoNgay(string maSP, string time)
        {
            List<object> listKQ = new List<object>();
            if (xulyChung.duocTruyCap(idOfPageDoanhThuTheoThoiDiem))
                try
                {
                    int maSanPham = xulyDuLieu.doiChuoiSangInteger(maSP);
                    DateTime date = xulyDuLieu.doiChuoiSangDateTime(time);
                    IEnumerable<object> listIEnumerable = new qlCaPheEntities().thongKeDoanhThuMotSanPhamTheoNgay(date, maSanPham);
                    this.taoDuLieuThongKeDoanhThuTungSanPham(listIEnumerable, listKQ);
                }
                catch (Exception ex)
                {
                    xulyFile.ghiLoi("Class: ThongKeController - Function: GetJsonDoanhThuSanPhamTheoNgay", ex.Message);
                }
            return Json(listKQ, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region THEO TUẦN
        /// <summary>
        /// Hàm tạo giao diện thống kê tổng doanh thu của 1 sản phẩm theo tuần
        /// </summary>
        /// <returns></returns>
        public ActionResult tke_ThongKeThuSanPhamTuan()
        {
            if (xulyChung.duocTruyCap(idOfPageDoanhThuTheoSanPham))
            {
                this.taoCbbSanPham();
                return View();
            }
            return RedirectToAction("PageNotFound", "Home");
        }

        /// <summary>
        /// Hàm ajax lấy thống kê tổng doanh thu của sản phẩm theo tuần
        /// </summary>
        /// <param name="time">Ngày bắt đầu cần xem thống kê</param>
        /// <param name="maSP">Mã sản phẩm cần xem thống kê</param>
        /// <returns>Json object các thởi điểm mua hàng và tổng tiền</returns>
        [HttpGet]
        public JsonResult GetJsonDoanhThuSanPhamTheoTuan(string maSP, string time)
        {
            List<object> listKQ = new List<object>();
            if (xulyChung.duocTruyCap(idOfPageDoanhThuTheoThoiDiem))
                try
                {
                    int maSanPham = xulyDuLieu.doiChuoiSangInteger(maSP);
                    DateTime date = xulyDuLieu.doiChuoiSangDateTime(time);
                    IEnumerable<object> listIEnumerable = new qlCaPheEntities().thongKeDoanhThuMotSanPhamTheoTuan(date, maSanPham);
                    this.taoDuLieuThongKeDoanhThuTungSanPham(listIEnumerable, listKQ);
                }
                catch (Exception ex)
                {
                    xulyFile.ghiLoi("Class: ThongKeController - Function: GetJsonDoanhThuSanPhamTheoTuan", ex.Message);
                }
            return Json(listKQ, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region THEO THÁNG
        /// <summary>
        /// Hàm tạo giao diện thống kê tổng doanh thu của 1 sản phẩm theo tháng
        /// </summary>
        /// <returns></returns>
        public ActionResult tke_ThongKeThuSanPhamThang()
        {
            if (xulyChung.duocTruyCap(idOfPageDoanhThuTheoSanPham))
            {
                this.taoCbbSanPham();
                return View();
            }
            return RedirectToAction("PageNotFound", "Home");
        }

        /// <summary>
        /// Hàm ajax lấy thống kê tổng doanh thu của sản phẩm theo tháng
        /// </summary>
        /// <param name="time">Tháng cần xem thống kê</param>
        /// <param name="maSP">Mã sản phẩm cần xem thống kê</param>
        /// <returns>Json object các thởi điểm mua hàng và tổng tiền</returns>
        [HttpGet]
        public JsonResult GetJsonDoanhThuSanPhamTheoThang(string maSP, string time)
        {
            List<object> listKQ = new List<object>();
            if (xulyChung.duocTruyCap(idOfPageDoanhThuTheoThoiDiem))
                try
                {
                    int maSanPham = xulyDuLieu.doiChuoiSangInteger(maSP);
                    int thang = xulyDuLieu.doiChuoiSangInteger(time);
                    IEnumerable<object> listIEnumerable = new qlCaPheEntities().thongKeDoanhThuMotSanPhamTheoThang(thang, maSanPham);
                    this.taoDuLieuThongKeDoanhThuTungSanPham(listIEnumerable, listKQ);
                }
                catch (Exception ex)
                {
                    xulyFile.ghiLoi("Class: ThongKeController - Function: GetJsonDoanhThuSanPhamTheoThang", ex.Message);
                }
            return Json(listKQ, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region THEO QUÝ
        /// <summary>
        /// Hàm tạo giao diện thống kê tổng doanh thu của 1 sản phẩm theo quý
        /// </summary>
        /// <returns></returns>
        public ActionResult tke_ThongKeThuSanPhamQuy()
        {
            if (xulyChung.duocTruyCap(idOfPageDoanhThuTheoSanPham))
            {
                this.taoCbbSanPham();
                return View();
            }
            return RedirectToAction("PageNotFound", "Home");
        }

        /// <summary>
        /// Hàm ajax lấy thống kê tổng doanh thu của sản phẩm theo quý
        /// </summary>
        /// <param name="time">quý cần xem thống kê</param>
        /// <param name="maSP">Mã sản phẩm cần xem thống kê</param>
        /// <returns>Json object các thởi điểm mua hàng và tổng tiền</returns>
        [HttpGet]
        public JsonResult GetJsonDoanhThuSanPhamTheoQuy(string maSP, string time)
        {
            List<object> listKQ = new List<object>();
            if (xulyChung.duocTruyCap(idOfPageDoanhThuTheoThoiDiem))
                try
                {
                    int maSanPham = xulyDuLieu.doiChuoiSangInteger(maSP);
                    int quy = xulyDuLieu.doiChuoiSangInteger(time);
                    IEnumerable<object> listIEnumerable = new qlCaPheEntities().thongKeDoanhThuMotSanPhamTheoQuy(quy, maSanPham);
                    this.taoDuLieuThongKeDoanhThuTungSanPham(listIEnumerable, listKQ);
                }
                catch (Exception ex)
                {
                    xulyFile.ghiLoi("Class: ThongKeController - Function: GetJsonDoanhThuSanPhamTheoThang", ex.Message);
                }
            return Json(listKQ, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region THEO NĂM
        /// <summary>
        /// Hàm tạo giao diện thống kê tổng doanh thu của 1 sản phẩm theo năm
        /// </summary>
        /// <returns></returns>
        public ActionResult tke_ThongKeThuSanPhamNam()
        {
            if (xulyChung.duocTruyCap(idOfPageDoanhThuTheoSanPham))
            {
                this.taoCbbSanPham();
                this.taoDuLieuChoCbbNam();
                return View();
            }
            return RedirectToAction("PageNotFound", "Home");
        }

        /// <summary>
        /// Hàm ajax lấy thống kê tổng doanh thu của sản phẩm theo năm
        /// </summary>
        /// <param name="time">quý cần xem thống kê</param>
        /// <param name="maSP">Mã sản phẩm cần xem thống kê</param>
        /// <returns>Json object các thởi điểm mua hàng và tổng tiền</returns>
        [HttpGet]
        public JsonResult GetJsonDoanhThuSanPhamTheoNam(string maSP, string time)
        {
            List<object> listKQ = new List<object>();
            if (xulyChung.duocTruyCap(idOfPageDoanhThuTheoThoiDiem))
                try
                {
                    int maSanPham = xulyDuLieu.doiChuoiSangInteger(maSP);
                    int nam = xulyDuLieu.doiChuoiSangInteger(time);
                    IEnumerable<object> listIEnumerable = new qlCaPheEntities().thongKeDoanhThuMotSanPhamTheoNam(nam, maSanPham);
                    this.taoDuLieuThongKeDoanhThuTungSanPham(listIEnumerable, listKQ);
                }
                catch (Exception ex)
                {
                    xulyFile.ghiLoi("Class: ThongKeController - Function: GetJsonDoanhThuSanPhamTheoThang", ex.Message);
                }
            return Json(listKQ, JsonRequestBehavior.AllowGet);
        }
        #endregion

        /// <summary>
        /// Hàm tạo dữ liệu cho combobox hiện danh sách các sản phẩm
        /// </summary>
        private void taoCbbSanPham()
        {
            try
            {
                string htmlCbb = "";
                foreach (sanPham sp in new qlCaPheEntities().sanPhams.ToList())
                    htmlCbb += "  <option value=\"" + sp.maSanPham.ToString() + "\">" + xulyDuLieu.traVeKyTuGoc(sp.tenSanPham) + "</option>";
                ViewBag.CbbSanPham = htmlCbb;
            }
            catch (Exception ex)
            {
                xulyFile.ghiLoi("Class: ThongKeController - Function: taoCbbSanPham", ex.Message);
            }
        }

        /// <summary>
        /// Hàm đọc dữ liệu lấy từ store procedure thống kê doanh thu từng sản phẩm qua các thời điểm
        /// và thêm vào list object json kết quả
        /// </summary>
        /// <param name="listIEnumerable">List chứa dữ liệu được lấy từ store procedure</param>
        /// <param name="listKQ">List object kết quả cần thêm dữ liệu vào json</param>
        private void taoDuLieuThongKeDoanhThuTungSanPham(IEnumerable<object> listIEnumerable, List<object> listKQ)
        {
            foreach (object x in listIEnumerable.ToList())
            {
                DateTime ngay = xulyDuLieu.doiChuoiSangDateTime(xulyDuLieu.layThuocTinhTrongMotObject(x, "thoiDiem"));
                object a = new
                {
                    thoiDiem = ngay.Date.ToShortDateString(),
                    tongTien = xulyDuLieu.doiChuoiSangLong(xulyDuLieu.layThuocTinhTrongMotObject(x, "tongTien"))
                };
                listKQ.Add(a);
            }
        }
        #endregion


        #region THỐNG KÊ NHẬP XUẤT TỒN
        #region NHẬP
        private static string idOfPageThongKeNhapKho = "1203";
        #region NHẬP KHO THEO NGÀY
        /// <summary>
        /// Hàm tạo giao diện thống kê tiền nhập kho theo ngày
        /// </summary>
        /// <returns></returns>
        public ActionResult tke_NhapKhoTheoNgay()
        {
            if (xulyChung.duocTruyCap(idOfPageThongKeNhapKho))
                return View();
            return RedirectToAction("PageNotFound", "Home");
        }

        /// <summary>
        /// Hàm ajax lấy danh sách hóa đơn theo ngày
        /// </summary>
        /// <param name="param">Ngày cần lấy danh sách hóa đơn</param>
        /// <returns>Json object các hóa đơn</returns>
        [HttpGet]
        public JsonResult GetJsonTongTienNhapTheoNgay(string param)
        {
            List<object> listHoaDon = new List<object>();
            if (xulyChung.duocTruyCap(idOfPageThongKeNhapKho))
                try
                {
                    DateTime date = xulyDuLieu.doiChuoiSangDateTime(param);
                    IEnumerable<object> listKQ = new qlCaPheEntities().thongKeTongTienNhapKhoTheoNgay(date);
                    long tongTienTamTinh = 0;
                    foreach (object x in listKQ.ToList())
                    {
                        string gio = xulyDuLieu.layThuocTinhTrongMotObject(x, "gio");
                        //---------Lấy tổng tiền thanh toán tạm tính của từng ngày
                        long tienTamTinh = xulyDuLieu.doiChuoiSangLong(xulyDuLieu.layThuocTinhTrongMotObject(x, "tongTien"));
                        tongTienTamTinh += tienTamTinh;
                        object a = new
                        {
                            time = gio,
                            price = tienTamTinh,
                            totalPrice = xulyDuLieu.doiVND(tongTienTamTinh)
                        };
                        listHoaDon.Add(a);
                    }
                }
                catch (Exception ex)
                {
                    xulyFile.ghiLoi("Class: ThongKeController - Function: GetJsonTongTienNhapTheoNgay", ex.Message);
                }
            return Json(listHoaDon, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// Hàm ajax lấy danh sách các nguyên liệu nhập kho theo ngày.
        /// Thông tin lấy gồm: maNguyenLieu, tennguyenlieu, soLuongNhap, donViHienThi, donGiaNhap
        /// </summary>
        /// <param name="param">Chuỗi chứa ngày cần lấy</param>
        /// <returns>Json object để vẽ biểu đồ</returns>
        [HttpGet]
        public JsonResult GetJsonSoLuongNhapKhoTheoNgay(string param)
        {
            List<object> listKQ = new List<object>();
            if (xulyChung.duocTruyCap(idOfPageThongKeNhapKho))
                try
                {
                    DateTime date = xulyDuLieu.doiChuoiSangDateTime(param);
                    IEnumerable<object> listStore = new qlCaPheEntities().thongKeSoLuongNhapKhoTheoNgay(date);
                    listKQ = this.ganDuLieuSoLuongNguyenLieu(listStore);

                }
                catch (Exception ex)
                {
                    xulyFile.ghiLoi("Class: ThongKeController - Function: GetJsonSoLuongNhapKhoTheoNgay", ex.Message);
                }
            return Json(listKQ, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region NHẬP KHO THEO TUẦN
        /// <summary>
        /// Hàm tạo giao diện thống kê tiền nhập kho theo tuần
        /// </summary>
        /// <returns></returns>
        public ActionResult tke_NhapKhoTheoTuan()
        {
            if (xulyChung.duocTruyCap(idOfPageThongKeNhapKho))
                return View();
            return RedirectToAction("PageNotFound", "Home");
        }

        /// <summary>
        /// Hàm ajax lấy danh sách hóa đơn theo tuần
        /// </summary>
        /// <param name="param">Ngày bắt đầu cần lấy danh sách phiếu nhập kho</param>
        /// <returns>Json object các phiếu nhập kho</returns>
        [HttpGet]
        public JsonResult GetJsonTongTienNhapTheoTuan(string param)
        {
            List<object> listHoaDon = new List<object>();
            if (xulyChung.duocTruyCap(idOfPageThongKeNhapKho))
                try
                {
                    DateTime date = xulyDuLieu.doiChuoiSangDateTime(param);
                    IEnumerable<object> listKQ = new qlCaPheEntities().thongKeTongTienNhapKhoTheoTuan(date);
                    long tongTienTatCa = 0;
                    foreach (object x in listKQ.ToList())
                    {
                        string ngayLap = xulyDuLieu.layThuocTinhTrongMotObject(x, "ngay");
                        //---------Lấy tổng tiền thanh toán tạm tính của từng ngày
                        long tongTienTrenPhieu = xulyDuLieu.doiChuoiSangLong(xulyDuLieu.layThuocTinhTrongMotObject(x, "tongTien"));
                        tongTienTatCa += tongTienTrenPhieu;
                        object a = new
                        {
                            time = ngayLap,
                            price = tongTienTrenPhieu,
                            tongTien = xulyDuLieu.doiVND(tongTienTatCa)
                        };
                        listHoaDon.Add(a);
                    }
                }
                catch (Exception ex)
                {
                    xulyFile.ghiLoi("Class: ThongKeController - Function: GetJsonTongTienNhapTheoTuan", ex.Message);
                }
            return Json(listHoaDon, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// Hàm ajax lấy danh sách các nguyên liệu nhập kho theo tuần.
        /// Thông tin lấy gồm: maNguyenLieu, tennguyenlieu, soLuongNhap, donViHienThi, donGiaNhap
        /// </summary>
        /// <param name="param">Chuỗi chứa ngày bắt đầu cần lấy</param>
        /// <returns>Json object để vẽ biểu đồ</returns>
        [HttpGet]
        public JsonResult GetJsonSoLuongNhapKhoTheoTuan(string param)
        {
            List<object> listKQ = new List<object>();
            if (xulyChung.duocTruyCap(idOfPageThongKeNhapKho))
                try
                {
                    DateTime date = xulyDuLieu.doiChuoiSangDateTime(param);
                    IEnumerable<object> listStore = new qlCaPheEntities().thongKeSoLuongNhapKhoTheoTuan(date);
                    listKQ = this.ganDuLieuSoLuongNguyenLieu(listStore);
                }
                catch (Exception ex)
                {
                    xulyFile.ghiLoi("Class: ThongKeController - Function: GetJsonSoLuongNhapKhoTheoTuan", ex.Message);
                }
            return Json(listKQ, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region NHẬP KHO THEO THÁNG
        /// <summary>
        /// Hàm tạo giao diện thống kê tiền nhập kho theo tháng
        /// </summary>
        /// <returns></returns>
        public ActionResult tke_NhapKhoTheoThang()
        {
            if (xulyChung.duocTruyCap(idOfPageThongKeNhapKho))
            {
                this.taoDuLieuChoCbbNam();
                return View();
            }
            return RedirectToAction("PageNotFound", "Home");
        }

        /// <summary>
        /// Hàm ajax lấy danh sách hóa đơn theo tháng
        /// </summary>
        /// <param name="param">năm cần liệt kê phiếu nhập kho</param>
        /// <returns>Json object các phiếu nhập kho</returns>
        [HttpGet]
        public JsonResult GetJsonTongTienNhapTheoThang(int? param)
        {
            List<object> listKQ = new List<object>();
            if (xulyChung.duocTruyCap(idOfPageThongKeNhapKho))
                try
                {
                    IEnumerable<object> listStore = new qlCaPheEntities().thongKeTongTienNhapKhoTheoThang(param);
                    listKQ = this.ganDuLieuTongTienTrongPhieu(listStore);
                }
                catch (Exception ex)
                {
                    xulyFile.ghiLoi("Class: ThongKeController - Function: GetJsonTongTienNhapTheoThang", ex.Message);
                }
            return Json(listKQ, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// Hàm ajax lấy danh sách các nguyên liệu nhập kho theo tháng.
        /// Thông tin lấy gồm: maNguyenLieu, tennguyenlieu, soLuongNhap, donViHienThi, donGiaNhap
        /// </summary>
        /// <param name="param">Tháng cần thống kê cần lấy</param>
        /// <returns>Json object để vẽ biểu đồ</returns>
        [HttpGet]
        public JsonResult GetJsonSoLuongNhapKhoTheoThang(byte? param)
        {
            List<object> listKQ = new List<object>();
            if (xulyChung.duocTruyCap(idOfPageThongKeNhapKho))
                try
                {
                    IEnumerable<object> listStore= new qlCaPheEntities().thongKeSoLuongNhapKhoTheoThang(param);
                    listKQ = this.ganDuLieuSoLuongNguyenLieu(listStore);
                    
                }
                catch (Exception ex)
                {
                    xulyFile.ghiLoi("Class: ThongKeController - Function: GetJsonSoLuongNhapKhoTheoThang", ex.Message);
                }
            return Json(listKQ, JsonRequestBehavior.AllowGet);
        }
        #endregion
        #endregion

        #region XUẤT
        private static string idOfPageThongKeXuatKho = "1204";
        #region XUẤT KHO THEO NGÀY
        /// <summary>
        /// Hàm tạo giao diện thống kê tiền xuất kho theo ngày
        /// </summary>
        /// <returns></returns>
        public ActionResult tke_XuatKhoTheoNgay()
        {
            if (xulyChung.duocTruyCap(idOfPageThongKeXuatKho))
                return View();
            return RedirectToAction("PageNotFound", "Home");
        }

        /// <summary>
        /// Hàm ajax lấy danh sách hóa đơn theo ngày
        /// </summary>
        /// <param name="param">Ngày cần lấy danh sách hóa đơn</param>
        /// <returns>Json object các hóa đơn</returns>
        [HttpGet]
        public JsonResult GetJsonTongTienXuatTheoNgay(string param)
        {
            List<object> listKQ = new List<object>();
            if (xulyChung.duocTruyCap(idOfPageThongKeNhapKho))
                try
                {
                    DateTime date = xulyDuLieu.doiChuoiSangDateTime(param);
                    IEnumerable<object> listStore = new qlCaPheEntities().thongKeTongTienXuatKhoTheoNgay(date);
                    listKQ = this.ganDuLieuTongTienTrongPhieu(listStore);
                }
                catch (Exception ex)
                {
                    xulyFile.ghiLoi("Class: ThongKeController - Function: GetJsonTongTienNhapTheoNgay", ex.Message);
                }
            return Json(listKQ, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// Hàm ajax lấy danh sách các nguyên liệu nhập kho theo ngày.
        /// Thông tin lấy gồm: maNguyenLieu, tennguyenlieu, soLuongNhap, donViHienThi, donGiaNhap
        /// </summary>
        /// <param name="param">Chuỗi chứa ngày cần lấy</param>
        /// <returns>Json object để vẽ biểu đồ</returns>
        [HttpGet]
        public JsonResult GetJsonSoLuongXuatKhoTheoNgay(string param)
        {
            List<object> listKQ = new List<object>();
            if (xulyChung.duocTruyCap(idOfPageThongKeNhapKho))
                try
                {
                    DateTime date = xulyDuLieu.doiChuoiSangDateTime(param);
                    IEnumerable<object> listStore = new qlCaPheEntities().thongKeSoLuongXuatKhoTheoNgay(date);
                    listKQ = this.ganDuLieuSoLuongNguyenLieu(listStore);
                }
                catch (Exception ex)
                {
                    xulyFile.ghiLoi("Class: ThongKeController - Function: GetJsonSoLuongNhapKhoTheoNgay", ex.Message);
                }
            return Json(listKQ, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region XUẤT KHO THEO TUẦN
        /// <summary>
        /// Hàm tạo giao diện thống kê tiền nhập kho theo tuần
        /// </summary>
        /// <returns></returns>
        public ActionResult tke_XuatKhoTheoTuan()
        {
            if (xulyChung.duocTruyCap(idOfPageThongKeXuatKho))
                return View();
            return RedirectToAction("PageNotFound", "Home");
        }

        /// <summary>
        /// Hàm ajax lấy danh sách phiếu xuất kho tuần
        /// </summary>
        /// <param name="param">Ngày bắt đầu cần lấy danh sách phiếu xuất kho</param>
        /// <returns>Json object các phiếu xuất kho</returns>
        [HttpGet]
        public JsonResult GetJsonTongTienXuatTheoTuan(string param)
        {
            List<object> listKQ = new List<object>();
            if (xulyChung.duocTruyCap(idOfPageThongKeNhapKho))
                try
                {
                    DateTime date = xulyDuLieu.doiChuoiSangDateTime(param);
                    IEnumerable<object> listStore = new qlCaPheEntities().thongKeTongTienXuatKhoTheoTuan(date);
                    listKQ = this.ganDuLieuTongTienTrongPhieu(listStore);
                }
                catch (Exception ex)
                {
                    xulyFile.ghiLoi("Class: ThongKeController - Function: GetJsonTongTienNhapTheoTuan", ex.Message);
                }
            return Json(listKQ, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// Hàm ajax lấy danh sách các nguyên liệu xuất kho theo tuần.
        /// Thông tin lấy gồm: maNguyenLieu, tennguyenlieu, soLuongNhap, donViHienThi, donGiaNhap
        /// </summary>
        /// <param name="param">Chuỗi chứa ngày bắt đầu cần lấy</param>
        /// <returns>Json object để vẽ biểu đồ</returns>
        [HttpGet]
        public JsonResult GetJsonSoLuongXuatKhoTheoTuan(string param)
        {
            List<object> listKQ = new List<object>();
            if (xulyChung.duocTruyCap(idOfPageThongKeNhapKho))
                try
                {
                    DateTime date = xulyDuLieu.doiChuoiSangDateTime(param);
                    IEnumerable<object> listStore = new qlCaPheEntities().thongKeSoLuongXuatKhoTheoTuan(date);
                    listKQ = this.ganDuLieuSoLuongNguyenLieu(listStore);
                }
                catch (Exception ex)
                {
                    xulyFile.ghiLoi("Class: ThongKeController - Function: GetJsonSoLuongNhapKhoTheoTuan", ex.Message);
                }
            return Json(listKQ, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region XUẤT KHO THEO THÁNG
        /// <summary>
        /// Hàm tạo giao diện thống kê tiền xuất kho theo tháng
        /// </summary>
        /// <returns></returns>
        public ActionResult tke_XuatKhoTheoThang()
        {
            if (xulyChung.duocTruyCap(idOfPageThongKeXuatKho))
            {
                this.taoDuLieuChoCbbNam();
                return View();
            }
            return RedirectToAction("PageNotFound", "Home");
        }

        /// <summary>
        /// Hàm ajax lấy danh sách phiếu xuất kho 
        /// </summary>
        /// <param name="param">năm cần liệt kê phiếu xuất kho</param>
        /// <returns>Json object các phiếu xuất kho</returns>
        [HttpGet]
        public JsonResult GetJsonTongTienXuatTheoThang(int? param)
        {
            List<object> listKQ = new List<object>();
            if (xulyChung.duocTruyCap(idOfPageThongKeNhapKho))
                try
                {
                    IEnumerable<object> listStore = new qlCaPheEntities().thongKeTongTienNhapKhoTheoThang(param);
                    listKQ = this.ganDuLieuTongTienTrongPhieu(listStore);
                }
                catch (Exception ex)
                {
                    xulyFile.ghiLoi("Class: ThongKeController - Function: GetJsonTongTienNhapTheoThang", ex.Message);
                }
            return Json(listKQ, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// Hàm ajax lấy danh sách các nguyên liệu xuất kho theo tháng.
        /// Thông tin lấy gồm: maNguyenLieu, tennguyenlieu, soLuongNhap, donViHienThi, donGiaNhap
        /// </summary>
        /// <param name="param">Tháng cần thống kê cần lấy</param>
        /// <returns>Json object để vẽ biểu đồ</returns>
        [HttpGet]
        public JsonResult GetJsonSoLuongXuatKhoTheoThang(byte? param)
        {
            List<object> listKQ = new List<object>();
            if (xulyChung.duocTruyCap(idOfPageThongKeNhapKho))
                try
                {
                    IEnumerable<object> listStore = new qlCaPheEntities().thongKeSoLuongNhapKhoTheoThang(param);
                    listKQ = this.ganDuLieuSoLuongNguyenLieu(listStore);
                }
                catch (Exception ex)
                {
                    xulyFile.ghiLoi("Class: ThongKeController - Function: GetJsonSoLuongNhapKhoTheoThang", ex.Message);
                }
            return Json(listKQ, JsonRequestBehavior.AllowGet);
        }
        #endregion

     
        #endregion

        /// <summary>
        /// Hàm gán dữ liệu trong IEnumerable<object> được đọc từ store pro và gán cho listKQ
        /// Gán dữ liệu cho việc thống kê tổng tiền trên phiếu
        /// </summary>
        /// <param name="listIEnumberable">IEnumerable object đọc từ store procedure</param>
        ///<returns>Trả về list object đã gán dữ liệu</returns>
        private List<object> ganDuLieuTongTienTrongPhieu(IEnumerable<object> listIEnumberable)
        {
            List<object> listKQ = new List<object>();
            long tongTienTamTinh = 0;
            foreach (object x in listIEnumberable.ToList())
            {
                string gio = xulyDuLieu.layThuocTinhTrongMotObject(x, "time");
                //---------Lấy tổng tiền thanh toán tạm tính của từng ngày
                long tienTamTinh = xulyDuLieu.doiChuoiSangLong(xulyDuLieu.layThuocTinhTrongMotObject(x, "tongTien"));
                tongTienTamTinh += tienTamTinh;
                object a = new
                {
                    time = gio,
                    price = tienTamTinh,
                    totalPrice = xulyDuLieu.doiVND(tongTienTamTinh)
                };
                listKQ.Add(a);
            }
            return listKQ;
        }
        /// <summary>
        /// Hàm đọc dữ liệu trong IEnumerable được đọc từ store pro vàn gán vào list object 
        /// Dành cho việc đọc số lượng nguyên liệu
        /// </summary>
        /// <param name="listIEnumerable"></param>
        ///<returns>Trả về list object đã gán dữ liệu</returns>
        private List<object> ganDuLieuSoLuongNguyenLieu(IEnumerable<object> listIEnumerable)
        {
            List<object> listKQ = new List<object>();
            long tongTienNhap = 0;
            foreach (object x in listIEnumerable.ToList())
            {
                string soLuongNhap = xulyDuLieu.layThuocTinhTrongMotObject(x, "soLuongNhap");
                //---------Lấy tổng tiền thanh toán tạm tính của từng ngày
                long donGiaNhap = xulyDuLieu.doiChuoiSangLong(xulyDuLieu.layThuocTinhTrongMotObject(x, "donGiaNhap"));
                tongTienNhap += donGiaNhap;
                object a = new
                {
                    soLuongNhap = xulyDuLieu.doiChuoiSangInteger(soLuongNhap),
                    tenNguyenLieu = xulyDuLieu.layThuocTinhTrongMotObject(x, "tennguyenlieu") + " / " + soLuongNhap.ToString() + " " + xulyDuLieu.layThuocTinhTrongMotObject(x, "donViHienThi"),
                    tongTienNguyenLieu = donGiaNhap,
                    tongTien = xulyDuLieu.doiVND(tongTienNhap)
                };
                listKQ.Add(a);
            }
            return listKQ;
        }
        #endregion
    }

}