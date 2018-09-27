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

            // If this request is for an Identity Token, the name will be present.
            if (context.Subject.Identity.Name != null)
            {
                //Set the email claim for the user
                var name = context.Subject.Identity.Name;
                    context.IssuedClaims.Add(new Claim(JwtClaimTypes.Email, $"{name}@{name}.com"));
                // Add role claim specific to this App and User
                    context.IssuedClaims.Add(new Claim(JwtClaimTypes.Role, $"{context.Client.ClientId}.{name}"));


            }// end if name not null
            return Task.CompletedTask;
        }

        public Task IsActiveAsync(IsActiveContext context)
        {
            context.IsActive = true;
            return Task.FromResult(true);
        }
    }
}
