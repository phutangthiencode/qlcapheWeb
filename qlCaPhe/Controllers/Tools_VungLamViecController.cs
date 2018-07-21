using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using qlCaPhe.App_Start;
using qlCaPhe.Models;
using qlCaPhe.App_Start;

namespace qlCaPhe.Controllers
{
    public class Tools_VungLamViecController : Controller
    {
        //
        // GET: /Tools_VungLamViec/
        public ActionResult Index()
        {
            xulyChung.ghiNhatKyDtb(1, "Vùng làm việc chính");
            return View();
        }
        /// <summary>
        /// Hàm tạo vùng giao diện tải ứng dụng cho người phục vụ và quản lý kho.
        /// </summary>
        /// <returns>Vùng giao diện</returns>
        public ActionResult tools_PartDownloadApp()
        {
            //-----Kiểm tra quyền hạn: Nếu có quyền đặt bàn, hoặc quyền giao sản phẩm đã pha chế và quyền kiểm kho
            taiKhoan tkLogin = (taiKhoan)Session["login"];
            return PartialView(tkLogin);
        }
        /// <summary>
        /// Hàm thực hiện tải file cài đặt ứng dụng CoffeeManager từ hệ thống
        /// </summary>
        /// <returns>Open hộp thoại download file</returns>
        public FileResult DownloadApp()
        {
            //-----Kiểm tra quyền hạn: Nếu có quyền đặt bàn, hoặc quyền giao sản phẩm đã pha chế và quyền kiểm kho
            taiKhoan tkLogin = (taiKhoan)Session["login"];
            string quyenHan = tkLogin.nhomTaiKhoan.quyenHan;
            if (quyenHan.Contains(":501") || quyenHan.Contains(":602") || quyenHan.Contains(":803"))
            {
                byte[] fileBytes = System.IO.File.ReadAllBytes(xulyChung.layDuongDanHost() + "/pages/app/coffeemanager.apk");
                string fileName = "coffeemanager.apk";
                xulyChung.ghiNhatKyDtb(1, "Download app coffeemanager từ hệ thống");
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
            }
            return null;
        }
    }
}