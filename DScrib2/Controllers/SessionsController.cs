using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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
            }

            public WebConfig web;
        }

        public async Task<ActionResult> TokenLogin()
        {
            var idToken = Request["idToken"];

            var validPayload = await GoogleJsonWebSignature.ValidateAsync(idToken);

            // Should be able to insert this into SQL Server.
            var sub = validPayload.Subject;

            return Json(new { ID = 2 }); ;
        }
    }
}