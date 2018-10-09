using IdentityServer4.Models;
using IdentityServer4.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace QuickstartIdentityServer
{
    public class DelegationGrantValidator : IExtensionGrantValidator
    {
        private readonly ITokenValidator _validator;

        public DelegationGrantValidator(ITokenValidator validator)
        {
            _validator = validator;
        }

        public string GrantType => "delegation";

        public async Task ValidateAsync(ExtensionGrantValidationContext context)
        {
            var userToken = context.Request.Raw.Get("token");

            if (string.IsNullOrEmpty(userToken))
            {
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant);
                return;
            }

            var validationResult = await _validator.ValidateAccessTokenAsync(userToken);
            if (validationResult.IsError)
            {
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant);
                return;
            }

            // get user's identity
            var sub_claim = validationResult.Claims.FirstOrDefault(c => c.Type == "sub").Value;
            var email_claim = validationResult.Claims.FirstOrDefault(c => c.Type == "email");

            // get the current client
            string caller = context.Request.Client.ClientId;
            
            //get ID of previous api using the audience claim from the original token.
            // There will also be an aud claim for Identity Server itself which we filter out with a Linq query.
            string prevClient = validationResult.Client.ClientId;

            string prevApi = validationResult.Claims.FirstOrDefault(c => c.Type == "aud" && !c.Value.ToUpper().Contains("HTTP")).Value;
            string thisClient = context.Request.Client.ClientId;

            // get current call_path claim if it exists 
            string newCallPath;
            var call_path = context.Request.ClientClaims?.Where(c => c.Type == "call_path");

            if (call_path.Count() == 0)
            {
                // if this is the first call,  construct a call path string including the intial client

                newCallPath = $"{prevClient}>{prevApi}>{thisClient}";
            }
            else
            {
                // if this is not the first call, add only the previous api and this client to the existing call path string
                // We don't need to record previous client here because it was already recorded as the current client in the previous call.
                newCallPath = $"{call_path.First().Value}>{prevApi}>{thisClient}";

            }

            List<Claim> claims = new List<Claim>();
            // add email claim so it is available for logging
            claims.Add(email_claim);
            // add call_path claim to identify api path from previous client
            Claim call_path_claim = new Claim("call_path", newCallPath);

            claims.Add(call_path_claim);


            context.Result = new GrantValidationResult(sub_claim, GrantType, claims);
            // context.Result = new GrantValidationResult(sub, GrantType);
            return;
        }
    }
}
