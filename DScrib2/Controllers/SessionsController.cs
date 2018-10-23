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
        public async Task<ActionResult> TokenLogin()
        {
            var idToken = Request["idToken"];

            // Probably should use Auth class from Google lib.
            var jsonContents = System.IO.File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "credentials.json"));
            var credJson = JsonConvert.DeserializeObject<GoogleCredentialConfig>(jsonContents);

            var settings = new GoogleJsonWebSignature.ValidationSettings() { Audience = new List<string>() { credJson.Web.ClientID }  };

            string subject = null;
            try
            {
                var validPayload = await GoogleJsonWebSignature.ValidateAsync(idToken, settings);
                subject = validPayload.Subject;
            }
            catch (InvalidJwtException e)
            {
                Response.StatusCode = 403;
                Response.StatusDescription = "Your token was invalid.";
                // Should probably log this.
                return null;
            }

            //// Should be able to insert this into SQL Server.
            return Json(new { ID = 2 }); ;
        }
    }
}