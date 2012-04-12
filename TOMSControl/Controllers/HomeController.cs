using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TOMSControl.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Status = "Stopped";
            return View();
        }

        public ActionResult About()
        {
            return View();
        }
    }
}
