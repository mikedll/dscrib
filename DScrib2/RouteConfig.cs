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
                defaults: new { controller = "Reviews", action = "Search" }
            );

            routes.MapRoute(
                name: "Review",
                url: "reviews/fetch",
                defaults: new { controller = "Reviews", action = "Fetch" }
            );

            routes.MapRoute(
                name: "Index",
                url: "me/{controller}",
                defaults: new { action = "Index" }
            );

            routes.MapRoute(
                name: "Creates",
                url: "{controller}",
                defaults: new { action = "Create" }
            );

            routes.MapRoute(
                name: "Updates",
                url: "{controller}/{id}",
                defaults: new { action = "Update" },
                constraints: new { id = "\\d+" }
            );

            routes.MapRoute(
                name: "Defaults",
                url: "{controller}/{action}"
            );
        }
    }

}