using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Google.Apis.Auth;
using Newtonsoft.Json;
using DScrib2.Models;

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
            string email = null;
            try
            {
                var validPayload = await GoogleJsonWebSignature.ValidateAsync(idToken, settings);
                subject = validPayload.Subject;
                email = validPayload.Email;
            }
            catch (InvalidJwtException)
            {
                Response.StatusCode = 403;
                Response.StatusDescription = "Your token was invalid.";
                // Should probably log this.
                return null;
            }

            General dbWrapper = new General();
            var user = dbWrapper.GetUserByVendorID(subject);
            if(user == null)
            {
                var newUser = new User() { Email = email, VendorID = subject };
                user = dbWrapper.CreateUser(newUser);
            }

            Session["userID"] = user.ID;

            // Should be able to insert this into SQL Server.
            return Json(user); ;
        }
    }
}