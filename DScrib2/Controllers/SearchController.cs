using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using DScrib2.Models;

namespace DScrib2
{
    public class SearchController : Controller
    {
        private AmazonWebClient client;

        private bool EnsureClientDefined()
        {
            if (client != null) return true;

            if(Session["userID"] == null)
            {
                Response.StatusCode = (int)HttpStatusCode.Forbidden;
                return false;
            }

            var db = new General();
            var user = db.GetUser((int)Session["userID"]);
            if (user == null)
            {
                Response.StatusCode = (int)HttpStatusCode.Forbidden;
                return false;
            }

            client = new AmazonWebClient(user.Email);

            return true;
        }
        public ActionResult Review(string linkSlug, string productID)
        {
            if (!EnsureClientDefined()) return null;

            var result = client.GetReviewPage(linkSlug, productID);
            if (result == null) return Json(null, JsonRequestBehavior.AllowGet);
            return Json(new { reviewDate = result.Item1, review = result.Item2 }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Index(string q)
        {
            if (!EnsureClientDefined()) return null;

            return Json(client.Search(q).Select(v => new Dictionary<string, string>(){
                { "name", v.Item1 },
                { "linkSlug", v.Item2 },
                { "productID", v.Item3 }}), JsonRequestBehavior.AllowGet);
        }
    }
}