using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DScrib2
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View("~/Index.cshtml");
        }
    }
}