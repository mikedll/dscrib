using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DScrib2;
using AngleSharp.Parser.Html;
using System.Text.RegularExpressions;

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
            Console.WriteLine("Booting...");
        }
    }
}
