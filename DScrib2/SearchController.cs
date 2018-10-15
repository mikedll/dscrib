using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DScrib2
{
    public class SearchController : Controller
    {
        public ActionResult Index(string q)
        {
            var results = new List<string>() { $"fake{q}result1", "result2", "result3" };
            return Json(results, JsonRequestBehavior.AllowGet);
        }
    }
}