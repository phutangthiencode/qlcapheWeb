using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using qlCaPhe.Models;
using qlCaPhe.App_Start;

namespace qlCaPhe.Controllers
{
    public class DatBanOnlineController : Controller
    {
        private static string idOfPage = "504";
        /// <summary>
        /// Hàm tạo giao diện hiện đặt bàn chờ tiếp nhận
        /// </summary>
        /// <returns>View giao diện trang</returns>
        public ActionResult db_DatBanChoTiepNhan(int ?page)
        {
            if (xulyChung.duocTruyCap(idOfPage))
            {
                try {
                    qlCaPheEntities db = new qlCaPheEntities();
                    int trangHienHanh = (page ?? 1);
                    //------Lấy danh sách đặt bàn online chờ tiếp nhận
                    int soPhanTu = db.datBanOnlines.ToList().Where(f => f.trangThai == 0).Count();
                    //------cấu hình phân trang
                    ViewBag.PhanTrang = createHTML.taoPhanTrang(soPhanTu, createHTML.pageSize, trangHienHanh, "/DatBanOnline/db_DatBanDaChoTiepNhan");
                    //-----Nhúng script để xủ lý 1 số chức năng
                    this.nhungScript();
                  //  xulyChung.ghiNhatKyDtb(1, "Danh mục đặt bàn đang chờ tiếp nhận");
                }
                catch (Exception ex)
                {
                    xulyFile.ghiLoi("Class: DatBanOnlineController - Function: db_DatBanDaChoTiepNhan", ex.Message);
                    return RedirectToAction("ServerError", "Home");
                }
            }
            return View();
        }

        /// <summary>
        /// Hàm tạo giao diện hiện đặt bàn đã tiếp nhận
        /// </summary>
        /// <returns>View giao diện trang</returns>
        public ActionResult db_DatBanDaTiepNhan(int? page)
        {
            if (xulyChung.duocTruyCap(idOfPage))
            {
                try
                {
                    qlCaPheEntities db = new qlCaPheEntities();
                    int trangHienHanh = (page ?? 1);
                    //------Lấy danh sách đặt bàn online chờ tiếp nhận
                    int soPhanTu = db.datBanOnlines.ToList().Where(f => f.trangThai == 1).Count();
                    //------cấu hình phân trang
                    ViewBag.PhanTrang = createHTML.taoPhanTrang(soPhanTu, createHTML.pageSize, trangHienHanh, "/DatBanOnline/db_DatBanDaChoTiepNhan");
                    //-----Nhúng script để xủ lý 1 số chức năng
                    this.nhungScript();
                    //  xulyChung.ghiNhatKyDtb(1, "Danh mục đặt bàn đang đã tiếp nhận");
                }
                catch (Exception ex)
                {
                    xulyFile.ghiLoi("Class: DatBanOnlineController - Function: db_DatBanDaChoTiepNhan", ex.Message);
                    return RedirectToAction("ServerError", "Home");
                }
            }
            return View();
        }

        /// <summary>
        /// Hàm tạo giao diện hiện đặt bàn đã tiếp nhận
        /// </summary>
        /// <returns>View giao diện trang</returns>
        public ActionResult db_DatBanDaTuChoi(int? page)
        {
            if (xulyChung.duocTruyCap(idOfPage))
            {
                try
                {
                    qlCaPheEntities db = new qlCaPheEntities();
                    int trangHienHanh = (page ?? 1);
                    //------Lấy danh sách đặt bàn online chờ tiếp nhận
                    int soPhanTu = db.datBanOnlines.ToList().Where(f => f.trangThai == 2).Count();
                    //------cấu hình phân trang
                    ViewBag.PhanTrang = createHTML.taoPhanTrang(soPhanTu, createHTML.pageSize, trangHienHanh, "/DatBanOnline/db_DatBanDaChoTiepNhan");
                    //-----Nhúng script để xủ lý 1 số chức năng
                    this.nhungScript();
                    //  xulyChung.ghiNhatKyDtb(1, "Danh mục đặt bàn đang đã hủy");
                }
                catch (Exception ex)
                {
                    xulyFile.ghiLoi("Class: DatBanOnlineController - Function: db_DatBanDaChoTiepNhan", ex.Message);
                    return RedirectToAction("ServerError", "Home");
                }
            }
            return View();
        }
        /// <summary>
        /// Hàm tạo giao diện vùng bảng dữ liệu
        /// </summary>
        /// <param name="page">Trang hiện hành</param>
        /// <param name="trangThai">Trạng thái cần lấy danh sách</param>
        /// <returns>Phần <table></table> trên trang</returns>
        public ActionResult db_PartTable(int? page,int? trangThai)
        {
            List<datBanOnline> listDatBan = new List<datBanOnline>();
            if (xulyChung.duocTruyCap(idOfPage))
            {
                try
                {
                    qlCaPheEntities db = new qlCaPheEntities();
                    int trangHienHanh = (page ?? 1);
                    listDatBan = db.datBanOnlines.Where(f => f.trangThai == trangThai).OrderBy(t => t.ngayDat).Skip((trangHienHanh - 1) * createHTML.pageSize).Take(createHTML.pageSize).ToList();
                }
                catch (Exception ex)
                {
                    xulyFile.ghiLoi("Class: DatBanOnlineController - Function: db_DatBanDaChoTiepNhan", ex.Message);
                    return RedirectToAction("ServerError", "Home");
                }
            }
            return PartialView(listDatBan);
        }
        /// <summary>
        /// Hàm tạo vùng modal chi tiết một đặt bàn
        /// </summary>
        /// <returns></returns>
        public ActionResult db_PartModalChiTiet(int maDatBan)
        {
            datBanOnline datBan = new datBanOnline();
            if (xulyChung.duocTruyCap(idOfPage))
            {
                try
                {
                    datBan = new qlCaPheEntities().datBanOnlines.SingleOrDefault(d=>d.maDatBan==maDatBan);
                }
                catch (Exception ex)
                {
                    xulyFile.ghiLoi("Class: DatBanOnlineController - Function: db_DatBanDaChoTiepNhan", ex.Message);
                    return RedirectToAction("ServerError", "Home");
                }
            }
            return PartialView(datBan);
        }
        /// <summary>
        /// Hàm thực hiện nhúng các đoạn script xử lý vào giao diện
        /// </summary>
        private void nhungScript()
        {
            ViewBag.ScriptAjaxXemChiTiet = createScriptAjax.scriptAjaxXemChiTietKhiClick("js-xem-chitiet", "maDatBan", "DatBanOnline/db_PartModalChiTiet?maDatBan=", "vungChiTiet", "modalChiTiet");
        }
	}
}