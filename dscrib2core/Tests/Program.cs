﻿using System;
using System.IO;
using System.Linq;
using DScrib2;
using DScrib2.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Tests
{
    class Program
    {
        private AppDbContext db;

        public Program(AppDbContext dbCtx)
        {
            db = dbCtx;
        }

        static void Main(string[] args)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("config.json", optional: false, reloadOnChange: true)
                .Build();

            var dbConf = config["Data:connectionString"];

            var services = new ServiceCollection();
            services.AddDbContext<AppDbContext>(options => options.UseSqlServer(dbConf));

            var provider = services.BuildServiceProvider();
            var p = new Program(provider.GetService<AppDbContext>());
            //p.AmazonTest();
            p.AmazonTestSearch();
            // p.DbExec();
        }

        public void AmazonTestSearch()
        {
            // Tolerable to have a made-up email when debugging.
            var sc = new AmazonWebClient("testing@example.com");

            var searchBody = sc.GetTestSearch();
            if (searchBody == null)
            {
                Console.WriteLine("Error when retrieving test search.");
                return;
            }
            var results = sc.ParseSearch(searchBody);
        }

        public void AmazonTest()
        {
            // Tolerable to have a made-up email when debugging.
            var sc = new AmazonWebClient("testing@example.com");

            //var body = sc.GetTestReview();
            //var review = sc.GetReview("Sandalwood-Patchouli-Different-Scents-Karma", "B06Y274RR8");
            //if (body == null)
            //{
            //    Console.WriteLine("Got null response from GetReviewPage.");
            //    return;
            //}
            //var results = sc.ParseSearch(body);

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
            var user = db.Users.FirstOrDefault(u => u.VendorID == "12345");
            var user2 = db.Users.FirstOrDefault(u => u.VendorID == "12");

            if (user != null && user.ID == 1 && user2 == null)
            {
                Console.WriteLine("Okay");
            }

            var newUser = new User { VendorID = "9090112", Email = "sam2@example.com" };
            db.Users.Add(newUser);
            db.SaveChanges();
            if (newUser.ID > 0)
            {
                Console.WriteLine("Okay insert.");
            }
        }

    }
}
