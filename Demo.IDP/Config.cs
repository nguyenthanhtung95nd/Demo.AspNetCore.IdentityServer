// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityServer4;
using IdentityServer4.Models;
using System.Collections.Generic;

namespace Demo.IDP
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> IdentityResources =>
            new IdentityResource[]
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Address(),
                new IdentityResource("roles", "User role(s)", new List<string> { "role" }),
                new IdentityResource("country", "Your country", new List<string> { "country" })
            };

        public static IEnumerable<ApiScope> ApiScopes =>
            new ApiScope[]
            {
                new ApiScope("demoapi.scope", "Demo API Scope")
            };

        public static IEnumerable<ApiResource> Apis =>
            new ApiResource[]
            {
                new ApiResource("demoapi", "Demo API")
                {
                    Scopes = { "demoapi.scope" }
                }
            };

        public static IEnumerable<Client> Clients =>
            new Client[]
            {
                new Client
                    {
                        ClientName = "Demo.Client",
                        ClientId = "Demo.Client",
                        AllowedGrantTypes = GrantTypes.Code,
                        RedirectUris = new List<string>{ "https://localhost:5010/signin-oidc" },
                        AllowedScopes = { 
                            IdentityServerConstants.StandardScopes.OpenId,
                            IdentityServerConstants.StandardScopes.Profile,
                            IdentityServerConstants.StandardScopes.Address,
                            "roles",
                            "demoapi.scope",
                            "country"
                        },
                        ClientSecrets = { new Secret("DemoClientSecret".Sha512()) },
                        RequirePkce = true,
                        PostLogoutRedirectUris = new List<string> { "https://localhost:5010/signout-callback-oidc" },
                        RequireConsent = true,
                        AccessTokenLifetime = 120, // 2 minutes
                        AllowOfflineAccess = true, // The offline access scope is required to support refresh tokens
                        UpdateAccessTokenClaimsOnRefresh = true // our claims to be updated as soon as the token refreshes
                    }
            };
    }
}