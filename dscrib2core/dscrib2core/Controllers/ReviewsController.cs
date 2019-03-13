using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.AspNetCore.Http;
using DScrib2.Filters;
using DScrib2.Models;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace DScrib2
{
    public class ReviewsController : Controller
    {
        private AmazonWebClient client;
        private AppDbContext _db;
        private User user;

        public ReviewsController(AppDbContext db)
        {
            _db = db;
        }

        private bool RequireUser()
        {
            if (user != null && _db != null) return true;

            if (HttpContext.Session.GetString("userID") == null)
            {
                Response.StatusCode = (int)HttpStatusCode.Forbidden;
                return false;
            }

            user = _db.Users.Find(Convert.ToInt32(HttpContext.Session.GetString("userID")));
            if (user == null)
            {
                Response.StatusCode = (int)HttpStatusCode.Forbidden;
                return false;
            }

            return true;
        }

        private bool RequireAmazonClient()
        {
            if (client != null) return true;

            if (!RequireUser()) return false;

            client = new AmazonWebClient(user.Email);
            return true;
        }

        [HttpPut]
        [ValidateJsonAntiForgeryToken]
        public async Task<ActionResult> Update(int id)
        {
            if (!RequireUser()) return null;

            var review = user.Reviews.FirstOrDefault(r => r.ID == id);
            if(review == null)
            {
                Response.StatusCode = (int)HttpStatusCode.NotFound;
                return null;
            }

            // new string[] { "Name", "Text", "Date", "Slug", "AmazonID", "Unsave" }
            if (await TryUpdateModelAsync<Review>(review, "", r => r.Name, r => r.Text, r => r.Date, r => r.Slug, r => r.AmazonID, r => r.Unsave))
            {
                try
                {
                    if (review.Unsave)
                    {
                        // was modified; change to deleted.
                        _db.Entry(review).State = EntityState.Deleted;
                    }
                    _db.SaveChanges();
                }
                catch (DataException)
                {
                    Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    return null;
                }
            } else
            {
                Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                return null;
            }

            return Content(JsonConvert.SerializeObject(review), "application/json");
        }

        [HttpPost]
        [ValidateJsonAntiForgeryToken]
        public ActionResult Create([FromBody][Bind("Name, Text, Date, Slug, AmazonID")]Review review)
        {
            if (!RequireUser()) return null;

            try
            {
                review.User = user;
                _db.Reviews.Add(review);
                _db.SaveChanges();   
            }
            catch (DataException)
            {
                // ? 
                // ModelState.AddModelError("Unable to save.");
            }

            return Content(JsonConvert.SerializeObject(review), "application/json");
        }

        public ActionResult Fetch(string linkSlug, string productID)
        {
            if (!RequireUser()) return null;
            if (!RequireAmazonClient()) return null;

            var review = _db.Reviews.FirstOrDefault(r => r.Slug == linkSlug && r.AmazonID == productID);
            if(review != null) return Content(JsonConvert.SerializeObject(review), "application/json");

            var result = client.GetReview(linkSlug, productID);
            if (result == null)
            {
                Response.StatusCode = (int)HttpStatusCode.NotFound;
                return Json(null);
            }

            var unsavedReview = new
            {
                Name = result.Item3,
                Date = result.Item1,
                Text = result.Item2,
                Slug = linkSlug,
                AmazonID = productID
            };

            return Content(JsonConvert.SerializeObject(unsavedReview), "application/json");
        }

        public ActionResult Search(string q)
        {
            if (!RequireAmazonClient()) return null;

            return Json(client.Search(q).Select(v => new Dictionary<string, string>(){
                { "Name", v.Item1 },
                { "Slug", v.Item2 },
                { "AmazonID", v.Item3 }}));
        }

        public ActionResult Index()
        {
            if (!RequireUser()) return null;

            ViewBag.Reviews = user.Reviews;
            return View();
        }

    }
}