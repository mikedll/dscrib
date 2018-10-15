using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using AngleSharp.Parser.Html;

namespace DScrib2
{
    public class SearchController : Controller
    {
        private string ParseResults(string body)
        {
            var urlRegex = @"(http(s)?://www.amazon.com/[^/]+/dp/[^/]+/)";
            // Reviews look like this: https://www.amazon.com/Eucalan-Lavender-Fine-Fabric-Ounce/product-reviews/B001DEJMPG/

            var parser = new HtmlParser();
            var doc = parser.Parse(body);
            var items = doc.QuerySelectorAll(".s-result-list .a-link-normal.s-access-detail-page");
            var n = items.Count();
            Console.WriteLine("Found {0} items.", n);
            foreach (var item in items)
            {
                var link = item.GetAttribute("href");
                var name = item.QuerySelector("h2").TextContent;

                var match = new Regex(urlRegex, RegexOptions.Compiled).Match(link);

                var simpleUrl = "";
                if (match != null)
                {
                    simpleUrl = match.Value;
                }
            }
            return "";
        }
        public string HitAmazon()
        {
            var s = "https://www.amazon.com/s/?field-keywords=nice+smelling+soap";
            var request = (HttpWebRequest)WebRequest.Create(s);
            var response = request.GetResponse();

            string body = "";
            using (var str = response.GetResponseStream())
            {
                byte[] buf = new byte[1024];
                int n, offset = 0;
                n = str.Read(buf, offset, buf.Length);
                while (n > 0)
                {
                    body += Encoding.UTF8.GetString(buf, 0, n);
                    offset += n;
                    n = str.Read(buf, 0, buf.Length);
                }
            }

            return body;
        }

        public ActionResult Index(string q)
        {
            var res = HitAmazon();
            var results = new List<string>() { $"fake{q}result1", "result2", "result3", "Amazon started off with: " + res.Substring(0, 10) };
            return Json(results, JsonRequestBehavior.AllowGet);
        }
    }
}