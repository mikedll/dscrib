using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DScrib2;
using AngleSharp.Parser.Html;
using System.Text.RegularExpressions;
using System.IO;
using DScrib2.Models;

namespace Tester
{
    class Program
    {

        static void Main(string[] args)
        {
            var p = new Program();
            p.DbExec();
        }

        public void AmazonTest()
        {
            var sc = new AmazonWebClient();

            var body = sc.GetTestReview(); // sc.GetReviewPage("Sandalwood-Patchouli-Different-Scents-Karma", "B06Y274RR8");

            if (body == null)
            {
                Console.WriteLine("Got null response from GetReviewPage.");
                return;
            }

            var review = sc.GetReviewPage("Eucalan-Lavender-Fine-Fabric-Ounce", "B001DEJMPG");
            Console.WriteLine(review.Item1);
            Console.WriteLine(review.Item2);
            //var destFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "sample-review.html");
            //Console.WriteLine($"Writing {body.Length} characters to {AppDomain.CurrentDomain.BaseDirectory}");
            //System.IO.File.WriteAllText(destFile, body, Encoding.UTF8);

            Console.WriteLine("Finished.");
        }

        public void DbExec()
        {
            var db = new General();
            db.Rock();
        }
    }
}
