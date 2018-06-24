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
        /// <summary>
        /// Hàm tạo giao diện thống kê tổng doanh thu theo ngày
        /// </summary>
        /// <returns></returns>
        public ActionResult tke_DoanhThuTheoThoiDiem()
        {
            if (xulyChung.duocTruyCap("1201"))
                try
                {

                }
                catch (Exception ex)
                {
                    xulyFile.ghiLoi("Class: ThongKeController - Function: tke_DoanhThuTheoThoiDiem", ex.Message);
                }
            return View();
        }
        /// <summary>
        /// Hàm tạo vùng thống kê doanh thu theo ngày
        /// </summary>
        /// <returns></returns>
        public ActionResult tke_DoanhThuTheoNgay()
        {
            return View();
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
            if (xulyChung.duocTruyCap("1201"))
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


    }

}