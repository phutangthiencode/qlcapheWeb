using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using qlCaPhe.App_Start;
using qlCaPhe.Models;

namespace qlCaPhe.Controllers
{
    public class ThongKeController : Controller
    {
        private static string idOfPageDoanhThuTheoThoiDiem= "1201";
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
                return View();
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
                    xulyFile.ghiLoi("Class: ThongKeController - Function: tke_DoanhThuTheoThoiDiem", ex.Message);
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
                return View();
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
                    foreach (hoaDonOrder hoaDon in new qlCaPheEntities().hoaDonOrders.ToList().Where(h => h.ngayLap>=startDate && h.ngayLap<=endDate))
                    {
                        object x = new
                        {
                            maHD = hoaDon.ngayLap.Value.Date.ToShortDateString(), //-----Thay đổi mã hóa đơn thành ngày lập.
                            tamTinh = hoaDon.tamTinh,
                        };
                        listHoaDon.Add(x);
                    }
                }
                catch (Exception ex)
                {
                    xulyFile.ghiLoi("Class: ThongKeController - Function: tke_DoanhThuTheoThoiDiem", ex.Message);
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
                return View();
            return RedirectToAction("PageNotFound", "Home");
        }
        /// <summary>
        /// Hàm lấy danh sách hóa đơn theo năm
        /// </summary>
        /// <param name="param">Ngày bắt đầu tuần cần lấy</param>
        /// <returns>Json object danh sách hóa đơn</returns>
        [HttpGet]
        public JsonResult GetJsonDoanhThuTheoThang(string param)
        {
            List<object> listHoaDon = new List<object>();
            if (xulyChung.duocTruyCap(idOfPageDoanhThuTheoThoiDiem))
                try
                {
                    DateTime startDate = xulyDuLieu.doiChuoiSangDateTime(param);
                    DateTime endDate = startDate.AddDays(30); //-------Tính đến ngày của tuần sau bắt đầu từ ngày hiện tại
                    foreach (hoaDonOrder hoaDon in new qlCaPheEntities().hoaDonOrders.ToList().Where(h => h.ngayLap >= startDate && h.ngayLap <= endDate))
                    {
                        object x = new
                        {
                            maHD = hoaDon.ngayLap.Value.Date.ToShortDateString(), //-----Thay đổi mã hóa đơn thành ngày lập.
                            tamTinh = hoaDon.tamTinh,
                        };
                        listHoaDon.Add(x);
                    }
                }
                catch (Exception ex)
                {
                    xulyFile.ghiLoi("Class: ThongKeController - Function: tke_DoanhThuTheoThoiDiem", ex.Message);
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
                    DateTime startDate = xulyDuLieu.doiChuoiSangDateTime(param);
                    DateTime endDate = startDate.AddMonths(3); //-------Tính đến ngày của tuần sau bắt đầu từ ngày hiện tại
                    foreach (hoaDonOrder hoaDon in new qlCaPheEntities().hoaDonOrders.ToList().Where(h => h.ngayLap >= startDate && h.ngayLap <= endDate))
                    {
                        object x = new
                        {
                            maHD = hoaDon.ngayLap.Value.Date.Month.ToString(),
                            tamTinh = hoaDon.tamTinh,
                        };
                        listHoaDon.Add(x);
                    }
                }
                catch (Exception ex)
                {
                    xulyFile.ghiLoi("Class: ThongKeController - Function: tke_DoanhThuTheoThoiDiem", ex.Message);
                }
            return Json(listHoaDon, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }

}