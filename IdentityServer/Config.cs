using System;
using System.Collections.Generic;
using System.Security.Claims;
using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;

namespace IdentityServer
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> IdentityResources =>
            new IdentityResource[]
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Address(),
                new IdentityResource("roles", "User role(s)", new[] {"role"}),
                new IdentityResource("country", "Users country", new[] {"country"})
            };

        public static IEnumerable<ApiResource> ApiResources =>
            new[]
            {
                new ApiResource("weatherforecastapi", "Weather Forecast API")
                {
                    Scopes =
                    {
                        "weatherforecastapi"
                    }
                }
            };

        public static IEnumerable<ApiScope> Scopes =>
            new[]
            {
                new ApiScope("weatherforecastapi", "Weather Forecast API")
            };

        public static IEnumerable<Client> Clients =>
            new Client[]
            {
                new()
                {
                    ClientName = "WeatherForecastClient",
                    ClientId = "weatherforecastclient",
                    AccessTokenLifetime = 120,
                    AllowOfflineAccess = true,
                    UpdateAccessTokenClaimsOnRefresh = true,
                    ClientSecrets =
                    {
                        new Secret("WeatherForecastClientSecret".Sha512())
                    },
                    AllowedGrantTypes = GrantTypes.Code,
                    RequirePkce = true,
                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Address,
                        "roles",
                        "weatherforecastapi",
                        "country"
                    },
                    RedirectUris =
                    {
                        "https://localhost:5010/signin-oidc"
                    },
                    PostLogoutRedirectUris =
                    {
                        "https://localhost:5010/signout-callback-oidc"
                    }
                }
            };

        public static List<TestUser> TestUsers =>
            new()
            {
                new TestUser
                {
                    SubjectId = Guid.NewGuid().ToString(),
                    Username = "John",
                    Password = "JohnPassword",
                    Claims =
                    {
                        new Claim("given_name", "John"),
                        new Claim("family_name", "Doe"),
                        new Claim("address", "John Doe's Boulevard 323"),
                        new Claim("role", "Administrator"),
                        new Claim("country", "USA")
                    }
                },
                new TestUser
                {
                    SubjectId = Guid.NewGuid().ToString(),
                    Username = "Jane",
                    Password = "JanePassword",
                    Claims =
                    {
                        new Claim("given_name", "Jane"),
                        new Claim("family_name", "Doe"),
                        new Claim("address", "Jane Doe's Avenue 214"),
                        new Claim("role", "Viewer"),
                        new Claim("country", "USA")
                    }
                }
            };
    }
}