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
using Microsoft.Extensions.Configuration;

namespace DScrib2.Controllers
{
    public class SessionsController : Controller
    {
        private AppDbContext _db;
        private IConfiguration _config;

        public SessionsController(AppDbContext db, IConfiguration config)
        {
            _db = db;
            _config = config;
        }

        public async Task<ActionResult> TokenLogin(string idToken)
        {
            if (idToken == null)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return null;
            }

            var settings = new GoogleJsonWebSignature.ValidationSettings() { Audience = new List<string>() { _config["GoogleClientId"] }  };

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