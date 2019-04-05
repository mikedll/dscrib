using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;

namespace DScrib2
{
    public class HandleGSecretFilter : IActionFilter
    {
        private IConfiguration _config;

        public HandleGSecretFilter(IConfiguration config)
        {
            _config = config;
        }
        public void OnActionExecuting(ActionExecutingContext context)
        {
            ((Controller)context.Controller).ViewBag.ClientID = _config["GoogleClientId"];
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {

        }
    }
}