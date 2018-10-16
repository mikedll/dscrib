using System;
using System.Collections.Generic;
using System.IO;
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
        public string GetDataLocal()
        {
            var body = "";
            using (var sr = new System.IO.StreamReader(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "output.html"), Encoding.UTF8))
            {
                body += sr.ReadToEnd();
            }
            return body;
        }


        public List<Tuple<string, string, string>> ExtractProductInfo(string body)
        {
            var parser = new HtmlParser();
            var doc = parser.Parse(body);
            var items = doc.QuerySelectorAll(".s-result-list .a-link-normal.s-access-detail-page");
            var n = items.Count();

            var results = new List<Tuple<string, string, string>>();
            // Products look like this: https://www.amazon.com/Sandalwood-Patchouli-Different-Scents-Karma/dp/B06Y274RR8/
            // Reviews look like this: https://www.amazon.com/Eucalan-Lavender-Fine-Fabric-Ounce/product-reviews/B001DEJMPG/
            var productUrlRegex = new Regex(@"http(s)?://www.amazon.com/([^/]+)/dp/([^/]+)/", RegexOptions.Compiled);
            foreach (var item in items)
            {
                var link = item.GetAttribute("href");
                var name = item.QuerySelector("h2").TextContent;

                var match = productUrlRegex.Match(link);

                if (match.Success)
                {
                    results.Add(new Tuple<string, string, string>(name, match.Groups[2].Value, match.Groups[3].Value));
                }
            }
            return results;
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
            return Json(ExtractProductInfo(GetDataLocal()).Select(v => new Dictionary<string, string>(){
                { "name", v.Item1 },
                { "linkSlug", v.Item2 },
                { "productID", v.Item3 }}), JsonRequestBehavior.AllowGet);
        }
    }
}