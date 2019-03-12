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
        // Google Login params
        private GoogleCredentialConfig cred;

        public ActionResult Index()
        {
            HttpContext.Session.SetString("myval", "hi");
            var myVal = HttpContext.Session.GetString("myval");


            if (cred == null)
            {
            }

            return View();
        }

        public ActionResult YourEmail()
        {
            return View();
        }
    }
}