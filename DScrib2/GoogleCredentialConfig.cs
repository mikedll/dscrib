using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace DScrib2
{
    public class GoogleCredentialConfig
    {
        public class WebConfig
        {
            [JsonProperty("client_id")]
            public string ClientID;

            [JsonProperty("client_secret")]
            public string ClientSecret;
        }

        [JsonProperty("web")]
        public WebConfig Web;
    }
}