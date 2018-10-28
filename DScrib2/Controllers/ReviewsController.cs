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
        private General db;
        private User user;

        private bool WebClientOrError()
        {
            if (client != null) return true;

            if(Session["userID"] == null)
            {
                Response.StatusCode = (int)HttpStatusCode.Forbidden;
                return false;
            }

            var db = new General();
            user = db.GetUser((int)Session["userID"]);
            if (user == null)
            {
                Response.StatusCode = (int)HttpStatusCode.Forbidden;
                return false;
            }

            client = new AmazonWebClient(user.Email);

            return true;
        }

        public ActionResult Show(string linkSlug, string productID)
        {
            var review = db.GetReview(linkSlug, productID);
            if(review != null)
            {
                return Json(new { reviewDate = review.Date, review = review.Text }, JsonRequestBehavior.AllowGet);
            }

            if (!WebClientOrError()) return null;

            var result = client.GetReview(linkSlug, productID);
            if (result == null) return Json(null, JsonRequestBehavior.AllowGet);

            review = new Review
            {
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
            if (!WebClientOrError()) return null;

            return Json(client.Search(q).Select(v => new Dictionary<string, string>(){
                { "name", v.Item1 },
                { "linkSlug", v.Item2 },
                { "productID", v.Item3 }}), JsonRequestBehavior.AllowGet);
        }
    }
}