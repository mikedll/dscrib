using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace DScrib2
{
    public class SearchController : Controller
    {
        private AmazonWebClient client;

        public SearchController()
        {
            client = new AmazonWebClient();
        }

        public ActionResult Review(string linkSlug, string productID)
        {
            var result = client.GetReviewPage(linkSlug, productID);
            return Json(new { reviewDate = result.Item1, review = result.Item2 }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Index(string q)
        {
            return Json(client.Search(q).Select(v => new Dictionary<string, string>(){
                { "name", v.Item1 },
                { "linkSlug", v.Item2 },
                { "productID", v.Item3 }}), JsonRequestBehavior.AllowGet);
        }
    }
}