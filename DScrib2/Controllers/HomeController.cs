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
        private string clientId;
        private string secret;

        private class CredConfig
        {
            public class WebConfig
            {
                public string client_id;
            }

            public WebConfig web;
        }

        public ActionResult Index()
        {

            if(clientId == null)
            {
                var jsonContents = System.IO.File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "credentials.json"));
                var json = JsonConvert.DeserializeObject<CredConfig>(jsonContents);
                clientId = json.web.client_id;
            }

            ViewBag.ClientID = clientId;

            return View();
        }
    }
}