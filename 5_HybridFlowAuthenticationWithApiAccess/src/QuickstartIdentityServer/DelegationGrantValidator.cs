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

            var result = await _validator.ValidateAccessTokenAsync(userToken);
            if (result.IsError)
            {
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant);
                return;
            }

            // get user's identity
            var sub = result.Claims.FirstOrDefault(c => c.Type == "sub").Value;

            //get client ID of caller
            var callerId = context.Request.Client.ClientId;

            // get current call_path claim if it exists 
            //string newCallPath;
            //var call_path = context.Request.ClientClaims?.Where(c => c.Type == "call_path");

            //if (call_path.Count() > 0)
            //{
            //    Claim pathClaim = call_path.First();
            //     newCallPath = pathClaim.Value + "_" + callerId;
            //}
            //else
            //{
            //    newCallPath = "";
            //}


            //newCallPath =  callerId;

            //List<Claim> claims = new List<Claim>();
            //// add delegation claim to identify api path from previous client
            //Claim call_path_claim = new Claim("call_path", newCallPath);

            //claims.Add(call_path_claim);
            //context.Result = new GrantValidationResult(sub, GrantType, claims);
            context.Result = new GrantValidationResult(sub, GrantType);
            return;
        }
    }
}
