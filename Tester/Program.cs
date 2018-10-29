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
            p.AmazonTest();
            //p.DbExec();
        }

        public void AmazonTest()
        {
            // Tolerable to have a made-up email when debugging.
            var sc = new AmazonWebClient("testing@example.com");

            var body = sc.GetTestReview(); // sc.GetReviewPage("Sandalwood-Patchouli-Different-Scents-Karma", "B06Y274RR8");
            if (body == null)
            {
                Console.WriteLine("Got null response from GetReviewPage.");
                return;
            }
            var results = sc.ParseSearch(body);

            //var review = sc.GetReview("Eucalan-Lavender-Fine-Fabric-Ounce", "B001DEJMPG");
            //Console.WriteLine(review.Item1);
            //Console.WriteLine(review.Item2);
            //var destFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "sample-review.html");
            //Console.WriteLine($"Writing {body.Length} characters to {AppDomain.CurrentDomain.BaseDirectory}");
            //System.IO.File.WriteAllText(destFile, body, Encoding.UTF8);

            Console.WriteLine("Finished.");
        }

        public void DbExec()
        {
            var db = new DbWrapper();
            var user = db.GetUserByVendorID("12345");
            var user2 = db.GetUserByVendorID("12");

            if (user != null && user.ID == 1 && user2 == null)
            {
                Console.WriteLine("Okay");
            }

            var newUser = new User { VendorID = "9090112", Email = "sam2@example.com" };
            var newUser2 = db.CreateUser(newUser);
            if (newUser2.ID > 0)
            {
                Console.WriteLine("Okay insert.");
            }

            try
            {
                var newUser3 = db.CreateUser(newUser);
            }
            catch (Exception ex)
            {
                if(ex.Message == "Insertion would have violated unique key constraint.")
                {
                    Console.WriteLine("Dup caught.");
                } else
                {
                    throw;
                }
            }
        }
    }
}
