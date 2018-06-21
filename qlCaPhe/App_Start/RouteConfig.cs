using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace qlCaPhe
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            //----------route url cho trang login
            routes.MapRoute(
                name: "Login Page",
                url: "login/",
                defaults: new { controller = "Home", action = "Login", id = UrlParameter.Optional }
                );


            //---------Rewrite url cho trang danh mục tất cả sản phẩm
            routes.MapRoute(
                name: "All Product",
                url: "san-pham",
                defaults: new { controller = "PublicPage", action = "DanhSachSanPham", id = UrlParameter.Optional }
            );

            //---------Rewrite url cho trang chi tiết sản phẩm
            routes.MapRoute(
                name: "Detail Product",
                url: "san-pham/{tenSanPham}-{id}",
                defaults: new { controller = "PublicPage", action = "ChiTietSanPham", id = UrlParameter.Optional }
            );

            //---------Rewrite url cho trang sản phẩm theo loại
            routes.MapRoute(
                name: "Product Of Type",
                url: "loai-san-pham/{tenLoai}-{id}",
                defaults: new { controller = "PublicPage", action = "SanPhamTheoLoai", id = UrlParameter.Optional }
            );


            //---------Rewrite url cho trang danh mục bài viết
            routes.MapRoute(
                name: "All Article",
                url: "bai-viet",
                defaults: new { controller = "PublicPage", action = "DanhSachBaiViet", id = UrlParameter.Optional }
            );

            //---------Rewrite url cho trang chi tiết bài viết
            routes.MapRoute(
                name: "Detail Article",
                url: "bai-viet/{tenBaiViet}-{id}",
                defaults: new { controller = "PublicPage", action = "ChiTietBaiViet", id = UrlParameter.Optional }
            );


            //---------Rewrite url cho trang đặt bàn
            routes.MapRoute(
                name: "Booking",
                url: "dat-ban",
                defaults: new { controller = "PublicPage", action = "DanhSachBan", id = UrlParameter.Optional }
            );

            //---------Rewrite url cho trang kiểm tra đặt bàn
            routes.MapRoute(
                name: "Checking Booking",
                url: "checking",
                defaults: new { controller = "PublicPage", action = "GioDatBan", id = UrlParameter.Optional }
            );


            //---------Rewrite url cho trang liên hệ
            routes.MapRoute(
                name: "Contact",
                url: "lien-he",
                defaults: new { controller = "PublicPage", action = "ThongTinLienHe", id = UrlParameter.Optional }
            );

            //---------Rewrite url cho trang phản hồi
            routes.MapRoute(
                name: "Feedback",
                url: "phan-hoi",
                defaults: new { controller = "PublicPage", action = "GuiPhanHoi", id = UrlParameter.Optional }
            );


            //----------route url cho trang báo lỗi
            routes.MapRoute(
                name: "Error",
                url: "loi",
                defaults: new { controller = "PublicPage", action = "PageNotFound", id = UrlParameter.Optional }
            );

            //----------route url cho trang chủ
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "PublicPage", action = "Index", id = UrlParameter.Optional }
            );


        }
    }
}
