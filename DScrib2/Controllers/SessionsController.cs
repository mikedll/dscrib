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
using System.Net;

namespace DScrib2.Controllers
{
    public class SessionsController : Controller
    {
        private AppDbContext db = new AppDbContext();

        public async Task<ActionResult> TokenLogin()
        {
            var idToken = Request["idToken"];
            if (idToken == null)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return null;
            }

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

            var user = db.Users.FirstOrDefault(u => u.VendorID == subject);
            if(user == null)
            {
                var newUser = new User() { Email = email, VendorID = subject };
                db.Users.Add(newUser);
                db.SaveChanges();
            }

            Session["userID"] = user.ID;

            // Should be able to insert this into SQL Server.
            return Content(JsonConvert.SerializeObject(user), "application/json");
        }
    }
}