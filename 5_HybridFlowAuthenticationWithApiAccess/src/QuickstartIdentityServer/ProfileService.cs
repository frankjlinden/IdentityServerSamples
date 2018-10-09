using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace QuickstartIdentityServer
{
    public class ProfileService : IProfileService
    {

        public ProfileService()
        {
            
        }

        public Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            // FL This gets called each time IdentityServer needs to return claims about a user to client applications.
            //  If you request an identity and access token - it will get called twice (since you might be putting different claims into each token type)

            IList<Claim> userClaims = new List<Claim>();
            // Add test email claim
            //TODO Change to User Manager Query?? look at Plurasight user example
            Claim email = new Claim(JwtClaimTypes.Email, "Bob@Bob.com");
            // 
            if (email != null)
            {
                context.IssuedClaims.Add(email);
                // Add role claim specific to this App and User
               // context.IssuedClaims.Add(new Claim(JwtClaimTypes.Role, $"{context.Client.ClientId}.Admin.CO"));

                var call_path = context.Subject.FindFirst("call_path");
                if (call_path != null)
                    context.IssuedClaims.Add(call_path);
            }// end if email not null
            return Task.CompletedTask;
        }

        public Task IsActiveAsync(IsActiveContext context)
        {
            context.IsActive = true;
            return Task.FromResult(true);
        }
    }
}
