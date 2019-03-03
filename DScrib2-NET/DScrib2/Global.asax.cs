using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using System.Web.Routing;
using Newtonsoft.Json;
using System.Web.Mvc;

namespace DScrib2
{
    public class Global : HttpApplication
    {
        void RegisterFlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleGSecretFilter());
        }

        protected void Application_Start(object sender, EventArgs e)
        {
            RegisterFlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };

        }

    }
}