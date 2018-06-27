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
        /// <summary>
        /// Hàm tạo giao diện thống kê tổng doanh thu theo ngày
        /// </summary>
        /// <returns></returns>
        public ActionResult tke_DoanhThuTheoThoiDiem()
        {
            if (xulyChung.duocTruyCap(idOfPageDoanhThuTheoThoiDiem))
                try
                {

                }
                catch (Exception ex)
                {
                    xulyFile.ghiLoi("Class: ThongKeController - Function: tke_DoanhThuTheoThoiDiem", ex.Message);
                }
            return View();
        }
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

                    foreach (hoaDonOrder hoaDon in new qlCaPheEntities().hoaDonOrders.ToList().Where(h => h.ngayLap.Value.Date == date))
                    {
                        object x = new
                        {
                            maHD = hoaDon.maHoaDon.ToString(),
                            tamTinh = hoaDon.tamTinh,
                        };
                        listHoaDon.Add(x);
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
            if (xulyChung.duocTruyCap(idOfPageDoanhThuTheoThoiDiem))
                try
                {
                    DateTime date = xulyDuLieu.doiChuoiSangDateTime(param);
                    IEnumerable<object> listKQ = new qlCaPheEntities().thongKeDoanhThuTheoSanPhamTheoNgay(date);
                    foreach (object x in listKQ.ToList())
                    {
                        string soLanBan = xulyDuLieu.layThuocTinhTrongMotObject(x, "soLanBan");
                        //---------Lấy tổng tiền thanh toán tạm tính của từng ngày
                        long tongTienTamTinh = xulyDuLieu.doiChuoiSangLong(xulyDuLieu.layThuocTinhTrongMotObject(x, "tongTien"));
                        object a = new
                        {
                            soLanBan = xulyDuLieu.doiChuoiSangInteger(soLanBan),
                            tenSP = xulyDuLieu.layThuocTinhTrongMotObject(x, "tenSanPham"),
                            tongTien = tongTienTamTinh
                        };
                        listHoaDon.Add(a);
                    }
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
                    foreach (object x in listKQ.ToList())
                    {
                        string ngay = xulyDuLieu.layThuocTinhTrongMotObject(x, "ngay");
                        DateTime date = xulyDuLieu.doiChuoiSangDateTime(ngay);
                        //---------Lấy tổng tiền thanh toán tạm tính của từng ngày
                        long tongTienTamTinh = xulyDuLieu.doiChuoiSangLong(xulyDuLieu.layThuocTinhTrongMotObject(x, "tongTien"));
                        object a = new
                        {
                            maHD = date.Date.ToShortDateString(),
                            tamTinh = tongTienTamTinh
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
                    IEnumerable<object> listKQ = new qlCaPheEntities().thongKeDoanhThuTheoSanPhamTheoTuan(date);
                    foreach (object x in listKQ.ToList())
                    {
                        string soLanBan = xulyDuLieu.layThuocTinhTrongMotObject(x, "soLanBan");
                        //---------Lấy tổng tiền thanh toán tạm tính của từng ngày
                        long tongTienTamTinh = xulyDuLieu.doiChuoiSangLong(xulyDuLieu.layThuocTinhTrongMotObject(x, "tongTien"));
                        object a = new
                        {
                            soLanBan = xulyDuLieu.doiChuoiSangInteger(soLanBan),
                            tenSP = xulyDuLieu.layThuocTinhTrongMotObject(x, "tenSanPham"),
                            tongTien = tongTienTamTinh
                        };
                        listHoaDon.Add(a);
                    }
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
                string cbbNam = "";
                int namHienTai = DateTime.Now.Year;
                for (int i = 2017; i <= namHienTai; i++)
                    cbbNam += "<option value=\"" + i.ToString() + "\"" + ">" + i.ToString() + "</option>";
                ViewBag.CbbNam = cbbNam;
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
                    foreach (object x in listThongKe.ToList())
                    {
                        string thang = xulyDuLieu.layThuocTinhTrongMotObject(x, "thang");
                        //---------Lấy tổng tiền thanh toán tạm tính của từng ngày
                        long tongTienTamTinh = xulyDuLieu.doiChuoiSangLong(xulyDuLieu.layThuocTinhTrongMotObject(x, "tongTien"));
                        object a = new
                        {
                            maHD = thang,
                            tamTinh = tongTienTamTinh
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
                    IEnumerable<object> listKQ = new qlCaPheEntities().thongKeDoanhThuTheoSanPhamTheoThang(thang);
                    foreach (object x in listKQ.ToList())
                    {
                        string soLanBan = xulyDuLieu.layThuocTinhTrongMotObject(x, "soLanBan");
                        //---------Lấy tổng tiền thanh toán tạm tính của từng ngày
                        long tongTienTamTinh = xulyDuLieu.doiChuoiSangLong(xulyDuLieu.layThuocTinhTrongMotObject(x, "tongTien"));
                        object a = new
                        {
                            soLanBan = xulyDuLieu.doiChuoiSangInteger(soLanBan),
                            tenSP = xulyDuLieu.layThuocTinhTrongMotObject(x, "tenSanPham"),
                            tongTien = tongTienTamTinh
                        };
                        listHoaDon.Add(a);
                    }
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
                string cbbNam = "";
                int namHienTai = DateTime.Now.Year;
                for (int i = 2017; i <= namHienTai; i++)
                    cbbNam += "<option value=\"" + i.ToString() + "\"" + ">" + i.ToString() + "</option>";
                ViewBag.CbbNam = cbbNam;
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
                    foreach (object x in listThongKe.ToList())
                    {
                        string thang = xulyDuLieu.layThuocTinhTrongMotObject(x, "quy");
                        //---------Lấy tổng tiền thanh toán tạm tính của từng ngày
                        long tongTienTamTinh = xulyDuLieu.doiChuoiSangLong(xulyDuLieu.layThuocTinhTrongMotObject(x, "tongTien"));
                        object a = new
                        {
                            maHD = thang,
                            tamTinh = tongTienTamTinh
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
                    IEnumerable<object> listKQ = new qlCaPheEntities().thongKeDoanhThuTheoSanPhamTheoQuy(quy);
                    foreach (object x in listKQ.ToList())
                    {
                        string soLanBan = xulyDuLieu.layThuocTinhTrongMotObject(x, "soLanBan");
                        //---------Lấy tổng tiền thanh toán tạm tính của từng ngày
                        long tongTienTamTinh = xulyDuLieu.doiChuoiSangLong(xulyDuLieu.layThuocTinhTrongMotObject(x, "tongTien"));
                        object a = new
                        {
                            soLanBan = xulyDuLieu.doiChuoiSangInteger(soLanBan),
                            tenSP = xulyDuLieu.layThuocTinhTrongMotObject(x, "tenSanPham"),
                            tongTien = tongTienTamTinh
                        };
                        listHoaDon.Add(a);
                    }
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
                string cbbNam = "";
                int namHienTai = DateTime.Now.Year;
                for (int i = 2017; i <= namHienTai; i++)
                    cbbNam += "<option value=\"" + i.ToString() + "\"" + ">" + i.ToString() + "</option>";
                ViewBag.CbbNam = cbbNam;
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
                    foreach (object x in listThongKe.ToList())
                    {
                        string thang = xulyDuLieu.layThuocTinhTrongMotObject(x, "nam");
                        //---------Lấy tổng tiền thanh toán tạm tính của từng ngày
                        long tongTienTamTinh = xulyDuLieu.doiChuoiSangLong(xulyDuLieu.layThuocTinhTrongMotObject(x, "tongTien"));
                        object a = new
                        {
                            maHD = thang,
                            tamTinh = tongTienTamTinh
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
                    IEnumerable<object> listKQ = new qlCaPheEntities().thongKeDoanhThuTheoSanPhamTheoNam(nam);
                    foreach (object x in listKQ.ToList())
                    {
                        string soLanBan = xulyDuLieu.layThuocTinhTrongMotObject(x, "soLanBan");
                        //---------Lấy tổng tiền thanh toán tạm tính của từng ngày
                        long tongTienTamTinh = xulyDuLieu.doiChuoiSangLong(xulyDuLieu.layThuocTinhTrongMotObject(x, "tongTien"));
                        object a = new
                        {
                            soLanBan = xulyDuLieu.doiChuoiSangInteger(soLanBan),
                            tenSP = xulyDuLieu.layThuocTinhTrongMotObject(x, "tenSanPham"),
                            tongTien = tongTienTamTinh
                        };
                        listHoaDon.Add(a);
                    }
                }
                catch (Exception ex)
                {
                    xulyFile.ghiLoi("Class: ThongKeController - Function: GetJsonSanPhamTheoQuy", ex.Message);
                }
            return Json(listHoaDon, JsonRequestBehavior.AllowGet);
        }
        #endregion
        #endregion
    }

}