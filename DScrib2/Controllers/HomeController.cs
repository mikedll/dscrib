using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DScrib2.Controllers
{
    public class HomeController : Controller
    {
        public async Task<ActionResult> Index()
        {
            HttpContext.Session.SetString("myval", "hi");
            var myVal = HttpContext.Session.GetString("myval");
            return View();
        }

        public async Task<ActionResult> YourEmail()
        {
            return View();
        }
    }
}