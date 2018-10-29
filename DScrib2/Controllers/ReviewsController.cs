using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using DScrib2.Models;

namespace DScrib2
{
    public class ReviewsController : Controller
    {
        private AmazonWebClient client;
        private DbWrapper db;
        private User user;

        private bool RequireUserAndDb()
        {
            if (user != null && db != null) return true;

            if (Session["userID"] == null)
            {
                Response.StatusCode = (int)HttpStatusCode.Forbidden;
                return false;
            }

            db = new DbWrapper();
            user = db.GetUser((int)Session["userID"]);
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

            if (!RequireUserAndDb()) return false;

            client = new AmazonWebClient(user.Email);
            return true;
        }

        public ActionResult Show(string linkSlug, string productID)
        {
            if (!RequireUserAndDb()) return null;
            if (!RequireAmazonClient()) return null;

            var review = db.GetReview(linkSlug, productID);
            if(review != null) return Json(review, JsonRequestBehavior.AllowGet);

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
            review = db.SaveReview(review);

            return Json(review, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Index(string q)
        {
            if (!RequireAmazonClient()) return null;

            return Json(client.Search(q).Select(v => new Dictionary<string, string>(){
                { "Name", v.Item1 },
                { "Slug", v.Item2 },
                { "AmazonID", v.Item3 }}), JsonRequestBehavior.AllowGet);
        }
    }
}