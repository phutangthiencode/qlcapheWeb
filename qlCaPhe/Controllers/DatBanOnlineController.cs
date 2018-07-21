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
        #region READ
        /// <summary>
        /// Hàm tạo giao diện hiện đặt bàn chờ tiếp nhận
        /// </summary>
        /// <returns>View giao diện trang</returns>
        public ActionResult db_DatBanChoTiepNhan(int? page)
        {
            if (xulyChung.duocTruyCap(idOfPage))
            {
                try
                {
                    qlCaPheEntities db = new qlCaPheEntities();
                    int trangHienHanh = (page ?? 1);
                    //------Lấy danh sách đặt bàn online chờ tiếp nhận
                    int soPhanTu = db.datBanOnlines.ToList().Where(f => f.trangThai == 0).Count();
                    //------cấu hình phân trang
                    ViewBag.PhanTrang = createHTML.taoPhanTrang(soPhanTu, createHTML.pageSize, trangHienHanh, "/DatBanOnline/db_DatBanDaChoTiepNhan");
                    //-----Nhúng script để xủ lý 1 số chức năng
                    this.nhungScript();
                    xulyChung.ghiNhatKyDtb(1, "Danh mục đặt bàn đang chờ tiếp nhận");
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
                    xulyChung.ghiNhatKyDtb(1, "Danh mục đặt bàn đã tiếp nhận");
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
                    xulyChung.ghiNhatKyDtb(1, "Danh mục đặt bàn đã hủy");
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
        public ActionResult db_PartTable(int? page, int? trangThai)
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
                    datBan = new qlCaPheEntities().datBanOnlines.SingleOrDefault(d => d.maDatBan == maDatBan);
                }
                catch (Exception ex)
                {
                    xulyFile.ghiLoi("Class: DatBanOnlineController - Function: db_DatBanDaChoTiepNhan", ex.Message);
                    return RedirectToAction("ServerError", "Home");
                }
            }
            return PartialView(datBan);
        }
        #endregion
        #region UPDATE
        /// <summary>
        /// Hàm thực hiện cập nhật trạng thái Tiếp nhận, từ chối đặt bàn
        /// </summary>
        /// <param name="trangThai">Trạng thái cần cập nhật: 1: Tiếp nhận - 2: Từ chối</param>
        /// <returns>Trả về giao diện chờ tiếp nhận</returns>
        public ActionResult capNhatTrangThai()
        {
            int trangThai = xulyDuLieu.doiChuoiSangInteger(xulyChung.nhanThamSoTrongSession(1));
            if (trangThai > 0)
                if (xulyChung.duocCapNhat(idOfPage, "7"))
                {
                    try
                    {
                        int kqLuu = 0;
                        int maDatBan = xulyDuLieu.doiChuoiSangInteger(xulyChung.nhanThamSoTrongSession(0)); //-----Nhận mã đặt bàn
                        qlCaPheEntities db = new qlCaPheEntities();
                        datBanOnline datBan = db.datBanOnlines.SingleOrDefault(f => f.maDatBan == maDatBan);
                        if (datBan != null)
                        {
                            datBan.trangThai = trangThai; //------Cập nhật trạng thái feedback đã phản hồi
                            db.Entry(datBan).State = System.Data.Entity.EntityState.Modified;
                            kqLuu = db.SaveChanges();
                            if (kqLuu > 0)
                                xulyChung.ghiNhatKyDtb(4, "Cập nhật trạng thái của đặt bàn của khách\" " + xulyDuLieu.traVeKyTuGoc(datBan.hoTenKH) + " \"");
                        }
                    }
                    catch (Exception ex)
                    {
                        xulyFile.ghiLoi("Class: DatBanOnlineController - Function: capNhatTrangThai", ex.Message);
                    }
                }
            return RedirectToAction("db_DatBanChoTiepNhan");
        }
        #endregion
        /// <summary>
        /// Hàm thực hiện nhúng các đoạn script xử lý vào giao diện
        /// </summary>
        private void nhungScript()
        {
            ViewBag.ScriptAjaxXemChiTiet = createScriptAjax.scriptAjaxXemChiTietKhiClick("js-xem-chitiet", "maDatBan", "DatBanOnline/db_PartModalChiTiet?maDatBan=", "vungChiTiet", "modalChiTiet");
        }
    }
}