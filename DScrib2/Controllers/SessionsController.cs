using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Google.Apis.Auth;
using Newtonsoft.Json;
using DScrib2.Models;
using System.Net;

namespace DScrib2.Controllers
{
    public class SessionsController : Controller
    {
        private AppDbContext _db;

        public SessionsController(AppDbContext db)
        {
            _db = db;
        }

        public async Task<ActionResult> TokenLogin(string idToken)
        {
            if (idToken == null)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return null;
            }

            // Probably should use Auth class from Google lib.
            var jsonContents = System.IO.File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "credentials.json"));
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
                // Provide body?

                // Should probably log this.
                return null;
            }

            var user = _db.Users.FirstOrDefault(u => u.VendorID == subject);
            if(user == null)
            {
                var newUser = new User() { Email = email, VendorID = subject };
                _db.Users.Add(newUser);
                _db.SaveChanges();
            }

            HttpContext.Session.SetString("userID", user.ID.ToString());

            // Should be able to insert this into SQL Server.
            return Content(JsonConvert.SerializeObject(user), "application/json");
        }
    }
}