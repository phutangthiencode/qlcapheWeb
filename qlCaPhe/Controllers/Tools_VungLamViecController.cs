using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
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
    }
}