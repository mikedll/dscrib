using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Google.Apis.Auth;
using Newtonsoft.Json;

namespace DScrib2.Controllers
{
    public class SessionsController : Controller
    {
        private class CredConfig
        {
            public class WebConfig
            {
                public string client_id;
                public string client_secret;
                public string project_id;
                public string auth_uri;
                public string token_uri;
                public string auth_provider_x509_cert_url;
                public string javascript_origins;
            }

            public WebConfig web;
        }

        public ActionResult TokenLogin()
        {
            
            var jsonContents = System.IO.File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "credentials.json"));
            var json = JsonConvert.DeserializeObject<CredConfig>(jsonContents);
            string clientId = json.web.client_id;
            var idToken = Request["token"];


            var validPayload = GoogleJsonWebSignature.ValidateAsync(idToken);
            return null;
        }
    }
}