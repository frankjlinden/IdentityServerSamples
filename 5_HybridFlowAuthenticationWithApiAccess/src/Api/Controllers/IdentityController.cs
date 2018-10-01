// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Authentication;
using System.Threading.Tasks;
using IdentityModel.Client;
using System;

namespace Api.Controllers
{
    [Route("[controller]")]
    [Authorize]
    public class IdentityController : Controller
    {
        [HttpGet]
        public async System.Threading.Tasks.Task<IActionResult> GetAsync()
        {
            // Get User Token - required to request delegate token for Internal API
     
             var token = await HttpContext.GetTokenAsync("access_token");

            // exchange user token for delegate token
            var delTokenResponse = await DelegateAsync(token);
            
            //call internal api using delegate token
            var client = new HttpClient();
            client.SetBearerToken(delTokenResponse.AccessToken);


            var content = await client.GetStringAsync("http://localhost:5001/identity");

            ViewBag.Json = JArray.Parse(content).ToString();
            return View("Json");
        }


        public async Task<TokenResponse> DelegateAsync(string userToken)
        {
            // discover endpoints from metadata
            var disco = await DiscoveryClient.GetAsync("http://localhost:5000");
            if (disco.IsError)
            {
                Console.WriteLine(disco.Error);
                // return ; //TODO Fix Return
            }

            var payload = new
            {
                token = userToken
            };

            // create token client
            var client = new TokenClient(disco.TokenEndpoint, "iapi_client", "secret");

            // send custom grant to token endpoint, return response
            return await client.RequestCustomGrantAsync("delegation", "iapi", payload);
        }
    }
}