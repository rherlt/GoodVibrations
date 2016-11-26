﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Serialization;

namespace GoodVibrations.Web.Models
{
    public class PhoneCallRequest
    {
        [JsonProperty("toPhoneNumber")]
        public string ToPhoneNumber { get; set; }

        [JsonProperty("fromPhoneNumber")]
        public string FromPhoneNumber { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("currentLocation")]
        public string CurrentLocation { get; set; }


    }
}
