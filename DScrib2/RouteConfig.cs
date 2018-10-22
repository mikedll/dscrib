using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace DScrib2
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Home",
                url: "",
                defaults: new { controller = "Home", action = "Index" }
            );

            routes.MapRoute(
                name: "Search",
                url: "search",
                defaults: new { controller = "Search", action = "Index" }
            );

            routes.MapRoute(
                name: "Review",
                url: "reviews",
                defaults: new { controller = "Search", action = "Review" }
            );
        }
    }

}