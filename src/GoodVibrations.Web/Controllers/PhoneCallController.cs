﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GoodVibrations.Web.Twilio;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace GoodVibrations.Web.Controllers
{
    [Route("api/[controller]")]
    public class PhoneCallController : Controller
    {

        private readonly IOptions<TwilioOptions> _twilioSettings;

        public PhoneCallController(IOptions<TwilioOptions> twilioSettings)
        {
            _twilioSettings = twilioSettings;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var token = "wesrbhjknlm";
            var callbackUrl = Url.RouteUrl(PhoneCallBackController.GetPhoneCallbackRoute, new {token}, Request.Scheme, Request.Host.ToUriComponent());
            //var callbackUrl = "http://goodvibrations-app.azurewebsites.net/api/phonecallback";
            var toPhoneNumber = "+4915208982338";
            var twilioClient = new TwilioRestClient();
            var request = new TwilioRequest(_twilioSettings.Value.AccountSid, _twilioSettings.Value.AuthToken, _twilioSettings.Value.FromPhoneNumber, toPhoneNumber, callbackUrl);
            var result = await twilioClient.Post(request, _twilioSettings.Value);

            if (result.IsSuccessStatusCode)
                return Ok();
            else
                return Ok(result);
        }
    }
}