using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using qlCaPhe.App_Start;
using qlCaPhe.Models;
using qlCaPhe.Models.Entities;
using qlCaPhe.Models.Business;
using qlCaPhe.App_Start;

namespace qlCaPhe.Controllers
{
    public class Tools_VungLamViecController : Controller
    {
        private static string idOfpage = "1";
        /// <summary>
        /// Hàm tạo giao diện vùng làm việc
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            if (xulyChung.duocTruyCap(idOfpage))
                xulyChung.ghiNhatKyDtb(1, "Vùng làm việc chính");
            return View();
        }
        /// <summary>
        /// Hàm tạo vùng giao diện thống kê số lượng bàn theo trạng thái: <para/>
        /// Bàn trống, bàn chờ order, bàn đã order, bàn chờ thanh toán, bàn đã thanh toán
        /// </summary>
        /// <returns></returns>
        public ActionResult tools_PartThongKeSoLuongBan()
        {
            SoLuongBan soLuong = new SoLuongBan();
            if (xulyChung.duocTruyCap(idOfpage))
                try
                {
                    List<int> listSoLuong = new bNghiepVuBan().thongKeBanTheoTrangThai();
                    int index = 1; //------Biến lưu lại vị trí đang duyệt trong listSoLUong để gán vào thuộc tính của soLUongBan
                    foreach (int giaTriSoLuong in listSoLuong)
                    {
                        switch (index)
                        {
                            case 1: soLuong.trong = giaTriSoLuong; break;
                            case 2: soLuong.choOrder = giaTriSoLuong; break;
                            case 3: soLuong.daOrder = giaTriSoLuong; break;
                            case 4: soLuong.choThanhToan = giaTriSoLuong; break;
                            case 5: soLuong.daThanhToan = giaTriSoLuong; break;
                        }
                        index++;
                    }
                    //-----Lấy tổng số bàn còn hoạt động
                    soLuong.tongCongBan = listSoLuong.Sum();
                }
                catch (Exception ex)
                {
                    xulyFile.ghiLoi("Class: Tools_VungLamViecController - Function: tools_PartThongKeSoLuongBan", ex.Message);
                }
            return PartialView(soLuong);
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
        /// <summary>
        /// Hàm tạo vùng giao diện đặt bàn online
        /// </summary>
        /// <returns></returns>
        public ActionResult tools_PartDatBanOnline()
        {
            List<datBanOnline> listDatBan = new List<datBanOnline>();
            if (xulyChung.duocTruyCap("504"))
                try
                {
                    listDatBan = new qlCaPheEntities().datBanOnlines.Where(d => d.trangThai == 0).ToList();
                }
                catch (Exception ex)
                {
                    xulyFile.ghiLoi("Class: Tools_VungLamViecController - Function: tools_PartDatBanOnline", ex.Message);
                }
            return PartialView(listDatBan);
        }
        /// <summary>
        /// Hàm tạo vùng giao diện liệt kê các phản hồi chưa trả lời
        /// </summary>
        /// <returns></returns>
        public ActionResult tools_PartPhanHoi()
        {
            List<Feedback> listPhanHoi = new List<Feedback>();
            if(xulyChung.duocTruyCap("1002"))
                try
                {
                    listPhanHoi = new qlCaPheEntities().Feedbacks.Where(f => f.trangThai == 0).ToList();
                }
                catch (Exception ex)
                {
                    xulyFile.ghiLoi("Class: Tools_VungLamViecController - Function: tools_PartPhanHoi", ex.Message);
                }
            return PartialView(listPhanHoi);
        }
    }
}