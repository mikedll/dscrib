using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;

namespace DScrib2
{
    public class HandleGSecretFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            var jsonContents = File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../../", "credentials.json"));
            var cred = JsonConvert.DeserializeObject<GoogleCredentialConfig>(jsonContents);
            
            ((Controller)context.Controller).ViewBag.ClientID = cred.Web.ClientID;

        }

        public void OnActionExecuted(ActionExecutedContext context)
        {

        }
    }
}