using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using DScrib2.Models;

namespace DScrib2.Controllers
{
    public class MyReviewsController : Controller
    {
        private AppDbContext _db;

        public MyReviewsController(AppDbContext db)
        {
            _db = db;
        }
    }
}