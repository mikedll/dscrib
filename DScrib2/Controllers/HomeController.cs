using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace DScrib2.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            HttpContext.Session.SetString("myval", "hi");
            var myVal = HttpContext.Session.GetString("myval");
            return View();
        }

        public ActionResult YourEmail()
        {
            return View();
        }
    }
}