using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace qlCaPhe.Controllers
{
    public class PublicPageController : Controller
    {
        /// <summary>
        /// Giao diện cho trang chủ
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// Giao diện danh mục sản phẩm
        /// </summary>
        /// <returns></returns>
        public ActionResult DanhSachSanPham()
        {
            return View();
        }
        /// <summary>
        /// Giao diện chi tiết sản phẩm
        /// </summary>
        /// <returns></returns>
        public ActionResult ChiTietSanPham()
        {
            return View();
        }
        /// <summary>
        /// Giao diện danh sách bàn có trong quán để đặt bàn
        /// </summary>
        /// <returns></returns>
        public ActionResult DanhSachBan()
        {
            return View();
        }
        /// <summary>
        /// Giao diện nhập thông tin đặt bàn
        /// </summary>
        /// <returns></returns>
        public ActionResult DatBan()
        {
            return View();
        }
        /// <summary>
        /// Giao diện danh sách bài viết
        /// </summary>
        /// <returns></returns>
        public ActionResult DanhSachBaiViet()
        {
            return View();
        }
        /// <summary>
        /// Giao diện chi tiết bài viết
        /// </summary>
        /// <returns></returns>
        public ActionResult ChiTietBaiViet()
        {
            return View();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult ThongTinLienHe()
        {
            return View();
        }
        /// <summary>
        /// Giao diện gửi phản hồi
        /// </summary>
        /// <returns></returns>
        public ActionResult GuiPhanHoi()
        {
            return View();
        }
	}
}