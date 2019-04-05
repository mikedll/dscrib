using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace DScrib2
{
    public class HandleGSecretFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            IConfiguration config = context.HttpContext.RequestServices.GetService<IConfiguration>();            
            ((Controller)context.Controller).ViewBag.ClientID = config["GoogleClientId"];
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {

        }
    }
}