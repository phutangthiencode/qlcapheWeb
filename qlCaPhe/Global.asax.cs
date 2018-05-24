using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using qlCaPhe.App_Start;
using qlCaPhe.Models;
using qlCaPhe.App_Start.Session;
using qlCaPhe.App_Start.Cart;

namespace qlCaPhe
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
        void Session_Start(object sender, EventArgs e)
        {
            //--Tạo session object chứa chi tiết công thức pha chế
            Session["congThuc"] = new cartCongThuc();
            //--Tạo session object chứa thông tin chi tiết phiếu nhập kho
            Session["ctNhapKho"] = new cartNhapKho();
            //--Tạo session object chứa thông tin chi tiết của hóa đơn tạm
            Session["hoaDonTam"] = new cartHoaDonTam();
            //--Tạo session lưu đường dẫn và request 
            Session["urlAction"] = "";
            //--Tạo session object chứa thông tin chi tiết phiếu xuất kho
            Session["ctXuatKho"] = new cartXuatKho();
            //--Tạo session object chứa thông tin nguyên liệu cần kiểm kho
            Session["truocKiemKho"] = new cartKiemKho();
            //--Tạo session object chứa thông tin nguyên liệu đã kiểm 
            Session["sauKiemKho"] = new cartKiemKho();
            //--Tạo session chứa danh sách các trang được phép truy cập
            Session["quyenHan"] = new cartQuyenHan();
            //--Tạo session chứa thông tin chi tiết điều phối
            Session["dieuPhoi"] = new cartDieuPhoi();
            //--Tạo session object chứa thông tin người dùng đăng nhập hệ thống
            Session["login"] = new taiKhoan();
        }
    }
}
