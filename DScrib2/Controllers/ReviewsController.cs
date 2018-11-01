using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using DScrib2.Models;
using Newtonsoft.Json;

namespace DScrib2
{
    public class ReviewsController : Controller
    {
        private AmazonWebClient client;
        private AppDbContext db = new AppDbContext();
        private User user;

        private bool RequireUser()
        {
            if (user != null && db != null) return true;

            if (Session["userID"] == null)
            {
                Response.StatusCode = (int)HttpStatusCode.Forbidden;
                return false;
            }

            user = db.Users.Find((int)Session["userID"]);
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
        // [ValidateAntiForgeryToken]
        public ActionResult Update(int id)
        {
            if (!RequireUser()) return null;

            var review = user.Reviews.FirstOrDefault(r => r.ID == id);
            if(review == null)
            {
                Response.StatusCode = (int)HttpStatusCode.NotFound;
                return null;
            }

            if (Request["Unsave"] != null)
            {
                try
                {
                    user.Reviews.Remove(review);
                    db.SaveChanges();
                }
                catch (DataException)
                {
                    Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    // Log error?
                }
            } else
            {
                if(TryUpdateModel(review, "", new string[] { "Name", "Text", "Date", "Slug", "AmazonID" }))
                {
                    try
                    {
                        db.SaveChanges();
                    }
                    catch (DataException)
                    {
                        Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        // ? 
                        // ModelState.AddModelError("Unable to save.");
                    }
                }
            }

            return Content(JsonConvert.SerializeObject(review), "application/json");
        }

        [HttpPost]
        // [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include= "Name, Text, Date, Slug, AmazonID")]Review review)
        {
            if (!RequireUser()) return null;

            try
            {
                review.User = user;
                db.Reviews.Add(review);
                db.SaveChanges();   
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

            var review = db.Reviews.FirstOrDefault(r => r.Slug == linkSlug && r.AmazonID == productID);
            if(review != null) return Content(JsonConvert.SerializeObject(review), "application/json");

            var result = client.GetReview(linkSlug, productID);
            if (result == null)
            {
                Response.StatusCode = (int)HttpStatusCode.NotFound;
                return Json(null, JsonRequestBehavior.AllowGet);
            }

            review = new Review
            {
                Name = result.Item3,
                Date = result.Item1,
                Text = result.Item2,
                Slug = linkSlug,
                AmazonID = productID,
                UserID = user.ID
            };
            db.Reviews.Add(review);
            db.SaveChanges();

            return Content(JsonConvert.SerializeObject(review), "application/json");
        }

        public ActionResult Search(string q)
        {
            if (!RequireAmazonClient()) return null;

            return Json(client.Search(q).Select(v => new Dictionary<string, string>(){
                { "Name", v.Item1 },
                { "Slug", v.Item2 },
                { "AmazonID", v.Item3 }}), JsonRequestBehavior.AllowGet);
        }

        public ActionResult Index()
        {
            if (!RequireUser()) return null;

            ViewBag.Reviews = user.Reviews;
            return View();
        }

    }
}