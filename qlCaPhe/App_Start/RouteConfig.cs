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

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "PublicPage", action = "Index", id = UrlParameter.Optional }
            );



        }
    }
}
