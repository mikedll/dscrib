using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
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

        [Route("reviews/{id:regex(\\d+)}/unsave")]
        [HttpPut]
        [ValidateAntiForgeryToken]
        public ActionResult Unsave(int id)
        {
            if (!RequireUser()) return null;

            var review = _db.Reviews.FirstOrDefault(r => r.UserID == user.ID && r.ID == id);
            if (review == null)
            {
                Response.StatusCode = (int)HttpStatusCode.NotFound;
                return Json(null);
            }

            try
            {
                _db.Reviews.Remove(review);
                _db.SaveChanges();
            }
            catch(DataException)
            {
                Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                return Json(null);
            }

            Response.StatusCode = (int)HttpStatusCode.OK;
            review.Unsave = true;
            return Content(JsonConvert.SerializeObject(review), "application/json");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
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
            if (review != null) return Content(JsonConvert.SerializeObject(review), "application/json");

            var result = client.GetReview(linkSlug, productID);
            if (review == null)
            {
                Response.StatusCode = (int)HttpStatusCode.NotFound;
                return Json(null);
            }

            var reviewData = new
            {
                Name = result.Item3,
                Date = result.Item1,
                Text = result.Item2,
                Slug = linkSlug,
                AmazonID = productID
            };

            return Content(JsonConvert.SerializeObject(reviewData), "application/json");
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