// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;
using System.Collections.Generic;
using System.Security.Claims;

namespace QuickstartIdentityServer
{
    public class Config
    {
        // scopes define the resources in your system
        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Email(),
                new IdentityResource {
                    Name = "ddsinfo",
                    UserClaims = new List<string> { "role","region","pin","call_path" }
                }
            };
        }

        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource>
            {
                new ApiResource("api1", "External API",new List<string>{JwtClaimTypes.Email,"region","pin","call_path"}),
                new ApiResource("iapi", "Internal API",new List<string>{JwtClaimTypes.Email,"region","pin","call_path"})
            };
        }

        // clients want to access resources (aka scopes)
        public static IEnumerable<Client> GetClients()
        {
            // client credentials client
            return new List<Client>
            {
                new Client
                {
                    ClientId = "console_client",
                    ClientName = "Console Client",
                    AllowedGrantTypes = GrantTypes.ClientCredentials,

                    ClientSecrets = 
                    {
                        new Secret("secret".Sha256())
                    },
                    AllowedScopes = { "api1" }
                },

                new Client
                {
                    ClientId = "iapi_client",
                    ClientName = "External API",
                    AllowedGrantTypes = {"delegation" },

                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    },
                    AllowedScopes = new List<string>
                    {
                        "iapi","ddsinfo","email"
                    }
                },


                // OpenID Connect hybrid flow and client credentials client (MVC)
                new Client
                {
                    ClientId = "mvc_client",
                    ClientName = "MVC Client Application",
                    AllowedGrantTypes = GrantTypes.HybridAndClientCredentials,
                    
                    ClientSecrets = 
                    {
                        new Secret("secret".Sha256())
                    },
                    
                    RedirectUris = { "http://localhost:5002/signin-oidc" },
                    PostLogoutRedirectUris = { "http://localhost:5002/signout-callback-oidc" },


                 //If we set AlwaysIncludeUserClaimsInIdToken, the custom profile service will add custom claims at login
                  AlwaysIncludeUserClaimsInIdToken = true,
                    AllowedScopes = 
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "api1",
                        "ddsinfo",
                        JwtClaimTypes.Email
                    },
                    
                    AllowOfflineAccess = true
                }
            };
        }

        public static List<TestUser> GetUsers()
        {
            return new List<TestUser>
            {
                new TestUser
                {
                    SubjectId = "1",
                    Username = "alice",
                    Password = "password",
                   

                    Claims = new List<Claim>
                    {
                        new Claim("role", "MVC.Admin.CO"),
                        new Claim("region","CO"),
                        new Claim("pin","0"),
                        new Claim("email","Alice@Alice.com"),
                        new Claim("call_path","")
                    }
                },
                new TestUser
                {
                    SubjectId = "2",
                    Username = "bob",
                    Password = "password",

                    Claims = new List<Claim>
                    {
                        new Claim("role", "MVC.Admin.NE"),
                        new Claim("region","NE"),
                        new Claim("pin","0"),
                        new Claim("email","Bob@Bob.com")
                    }
                }
            };
        }
    }
}