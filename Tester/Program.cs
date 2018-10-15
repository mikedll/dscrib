using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DScrib2;
using AngleSharp.Parser.Html;

namespace Tester
{
    class Program
    {
        private static string GetDataRemote()
        {
            var controller = new SearchController();
            return controller.HitAmazon();
        }

        private static string GetDataLocal()
        {
            var body = "";
            using (var sr = new System.IO.StreamReader("output.html", Encoding.UTF8))
            {
                body += sr.ReadToEnd();
            }
            return body;
        }

        static void Main(string[] args)
        {
            var parser = new HtmlParser();
            var doc = parser.Parse(GetDataLocal());
            var items = doc.QuerySelectorAll(".s-result-list .s-result-item");
            foreach(var item in items)
            {
                var name = item.QuerySelector("h2");
                Console.WriteLine("Item: {0}", name.TextContent);
            }
            // System.IO.File.WriteAllText(@"output.html", body);
            
        }
    }
}
