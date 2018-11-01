using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;

namespace DScrib2
{
    public class HandleGSecretFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var jsonContents = File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "credentials.json"));
            var cred = JsonConvert.DeserializeObject<GoogleCredentialConfig>(jsonContents);
            filterContext.Controller.ViewBag.ClientID = cred.Web.ClientID;

            base.OnActionExecuting(filterContext);
        }
    }
}