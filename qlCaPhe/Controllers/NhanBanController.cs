using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace qlCaPhe.Controllers
{
    public class NhanBanController : Controller
    {
        #region NHÓM HÀM XỬ LÝ VIỆC QUẢN LÝ ĐẶT BÀN TẠI QUÁN
        public ActionResult QuanLyDatBan()
        {
            return View();
        }
        #endregion
        /// <summary>
        /// Hàm tạo bảng danh sách đặt bàn online
        /// </summary>
        /// <returns></returns>
        public ActionResult TableNhanBanOnline()
        {
            return View();
        }
        /// <summary>
        /// Hàm thực hiện tạo view danh sách cần pha chế
        /// </summary>
        /// <returns></returns>
        public ActionResult TablePhaChe()
        {
            return View();
        }
        /// <summary>
        /// Hàm tạo View danh sách phục vụ
        /// </summary>
        /// <returns></returns>
        public ActionResult TablePhucVu()
        {
            return View();
        }
	}
}