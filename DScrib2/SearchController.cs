using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace DScrib2
{
    public class SearchController : Controller
    {
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