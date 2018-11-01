using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;

namespace DScrib2.Controllers
{
    public class HomeController : Controller
    {
        // Google Login params
        private GoogleCredentialConfig cred;

        public ActionResult Index()
        {

            if(cred == null)
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