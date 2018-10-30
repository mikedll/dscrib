using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DScrib2.Models;

namespace DScrib2.Controllers
{
    public class MyReviewsController : Controller
    {
        private AppDbContext db = new AppDbContext();

        public ActionResult Index()
        {
            if (Session["userID"] == null)
            {
                Response.StatusCode = (int)HttpStatusCode.Forbidden;
                return null;
            }

            var user = db.Users.Find((int)Session["userID"]);
            if (user == null)
            {
                Response.StatusCode = (int)HttpStatusCode.Forbidden;
                return null;
            }


            ViewBag.Reviews = user.Reviews;
            return View();
        }
    }
}