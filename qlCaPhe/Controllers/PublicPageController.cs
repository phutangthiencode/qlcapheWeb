using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using qlCaPhe.App_Start;
using qlCaPhe.Models;

namespace qlCaPhe.Controllers
{
    public class PublicPageController : Controller
    {
        /// <summary>
        /// Hàm tọa giao diện vùng header chứa menu navigation trên giao diện
        /// </summary>
        /// <returns></returns>
        public ActionResult Part_Header()
        {
            cauHinh cauHinh = new cauHinh();
            try
            {
                cauHinh = new qlCaPheEntities().cauHinhs.First();
            }
            catch (Exception ex)
            {
                xulyFile.ghiLoi("Class: PublicPageController - Function: Part_Header", ex.Message);
                Response.Redirect(xulyChung.layTenMien() + "Home/ServerError");
            }
            return PartialView(cauHinh);
        }

        #region Trang chủ
        /// <summary>
        /// Giao diện cho trang chủ
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View();
        }


        /// <summary>
        /// Hàm tạo giao diện phần banner 1 trên trang chủ
        /// Chứa hình ảnh, slideshow...
        /// </summary>
        /// <returns>Trả về danh sách slideshow được hiển thị </returns>
        public ActionResult PartHome_Banner1()
        {
            List<slide> listSlide = new List<slide>();
            try
            {
                listSlide = new qlCaPheEntities().slides.Where(s => s.trangThai == true).ToList();
            }
            catch (Exception ex)
            {
                xulyFile.ghiLoi("Class: PublicPageController - Function: PartHome_Banner1", ex.Message);
                Response.Redirect(xulyChung.layTenMien() + "Home/ServerError");
            }
            return PartialView(listSlide);
        }

        /// <summary>
        /// Hàm tạo giao diện vùng bài viết tại trang chủ
        /// "Thông tin nổi bật"
        /// </summary>
        /// <returns>List object bài viết hiện trang chủ</returns>
        public ActionResult PartHome_BaiViet()
        {
            List<baiViet> listBaiViet = new List<baiViet>();
            try
            {
                listBaiViet = new qlCaPheEntities().baiViets.Where(s => s.trangThai == 1 && s.hienTrangChu == true).ToList();
            }
            catch (Exception ex)
            {
                xulyFile.ghiLoi("Class: PublicPageController - Function: PartHome_BaiViet", ex.Message);
                Response.Redirect(xulyChung.layTenMien() + "Home/ServerError");
            }
            return PartialView(listBaiViet);
        }

        /// <summary>
        /// Hàm tạo giao diện vùng các nguyên liệu bán chạy
        /// Lấy 4 sản phẩm bán chạy nhất 
        /// </summary>
        /// <returns>List nguyên liệu bán chạy (Có nhiều trong ctHoaDonOrder)</returns>
        public ActionResult PartHome_SanPhamNoiTroi()
        {
            List<sanPham> listSanPham = new List<sanPham>();
            try
            {
                qlCaPheEntities db = new qlCaPheEntities();
                //----Khởi tạo biến chứa danh sách object Mã sản phẩm bán chạy và số lần bán
                var listSellest = db.ctHoaDonOrders.GroupBy(c => c.maSanPham).Select(c => new { maSP = c.Key, count = c.Count() }).OrderByDescending(c => c.count).Take(4).ToList(); //---Lấy 4 sản phẩm bán chạy 1
                for (int i = 0; i < listSellest.Count; i++)
                {
                    //-----Lấy giá trị là mã sản phẩm có trong listSellest
                    var itemSellest = listSellest[i];
                    int maSP = xulyDuLieu.doiChuoiSangInteger(xulyDuLieu.layThuocTinhTrongMotObject(itemSellest, "maSP"));

                    //------Lấy thông tin sản phẩm và gán vào listSanPham
                    sanPham spSellest = db.sanPhams.SingleOrDefault(s => s.maSanPham == maSP);
                    if (spSellest != null)
                        listSanPham.Add(spSellest);
                }
            }
            catch (Exception ex)
            {
                xulyFile.ghiLoi("Class: PublicPageController - Function: PartHome_SanPhamNoiTroi", ex.Message);
                Response.Redirect(xulyChung.layTenMien() + "Home/ServerError");
            }
            return PartialView(listSanPham);
        }
        /// <summary>
        /// Hàm tạo vùng giao diện Danh sách loại sản phẩm trên trang chủ
        /// </summary>
        /// <returns>List object loại sản phẩm có sản phẩm</returns>
        public ActionResult PartHome_LoaiSanPham()
        {
            List<loaiSanPham> listSanPham = new List<loaiSanPham>();
            try
            {
                listSanPham = new qlCaPheEntities().loaiSanPhams.Where(l => l.sanPhams.Count > 0).ToList();
            }
            catch (Exception ex)
            {
                xulyFile.ghiLoi("Class: PublicPageController - Function: PartHome_LoaiSanPham", ex.Message);
                Response.Redirect(xulyChung.layTenMien() + "Home/ServerError");
            }
            return PartialView(listSanPham);
        }
        #endregion

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

        /// <summary>
        /// Hàm tạo vùng giao diện footer
        /// </summary>
        /// <returns></returns>
        public ActionResult Part_Footer()
        {
            cauHinh cauHinh = new cauHinh();
            try
            {
                cauHinh = new qlCaPheEntities().cauHinhs.First();
            }
            catch (Exception ex)
            {
                xulyFile.ghiLoi("Class: PublicPageController - Function: Part_Footer", ex.Message);
                Response.Redirect(xulyChung.layTenMien() + "Home/ServerError");
            }
            return PartialView(cauHinh);
        }
    }
}