using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;

namespace IdentityServer
{
    public class Config
    {
        public static IEnumerable<Client> Clients
            => new Client[] {

                new Client {
                ClientId = "movieClient",
                AllowedGrantTypes = GrantTypes.ClientCredentials,
                ClientSecrets = { new Secret("secret".Sha256()) },
                AllowedScopes = {"movieAPI" }

                },
                 new Client
                    {
                        ClientId = "ro.client",
                        AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,

                        ClientSecrets =
                        {
                            new Secret("secret".Sha256())
                        },
                        AllowedScopes = new List<string>
                        {
                            IdentityServerConstants.StandardScopes.OpenId,
                            IdentityServerConstants.StandardScopes.Profile,
                             IdentityServerConstants.StandardScopes.Email,
                            "movieAPI"
                        }
                    },
                 new Client
                    {
                        ClientId = "mvc",
                        ClientSecrets = { new Secret("secret".Sha256()) },

                        AllowedGrantTypes = GrantTypes.Code,
                    
                        // where to redirect to after login
                        RedirectUris = { "https://localhost:5002/signin-oidc" },

                        // where to redirect to after logout
                        PostLogoutRedirectUris = { "https://localhost:5002/signout-callback-oidc" },

                        AllowedScopes = new List<string>
                        {
                            IdentityServerConstants.StandardScopes.OpenId,
                            IdentityServerConstants.StandardScopes.Profile,
                            "movieAPI"
                        }
                    },
                 new Client
                    {
                        ClientId = "resourceownerclient",

                        AllowedGrantTypes = GrantTypes.ResourceOwnerPasswordAndClientCredentials,
                        AccessTokenType = AccessTokenType.Jwt,
                        AccessTokenLifetime = 120, //86400,
                        IdentityTokenLifetime = 120, //86400,
                        UpdateAccessTokenClaimsOnRefresh = true,
                        SlidingRefreshTokenLifetime = 30,
                        AllowOfflineAccess = true,
                        RefreshTokenExpiration = TokenExpiration.Absolute,
                        RefreshTokenUsage = TokenUsage.OneTimeOnly,
                        AlwaysSendClientClaims = true,
                        Enabled = true,
                        ClientSecrets=  new List<Secret> { new Secret("secret".Sha256()) },
                        AllowedScopes = {
                            IdentityServerConstants.StandardScopes.OpenId,
                            IdentityServerConstants.StandardScopes.Profile,
                            IdentityServerConstants.StandardScopes.Email,
                            IdentityServerConstants.StandardScopes.OfflineAccess,
                            "movieAPI"
                        }
                    }


            };
        public static IEnumerable<ApiScope> ApiScopes
            => new ApiScope[] {
            new ApiScope("movieAPI", "Movie API")
            };

        public static IEnumerable<ApiResource> ApiResources
            => new ApiResource[] { };

        public static IEnumerable<IdentityResource> IdentityResources
            => new IdentityResource[] {
                 new IdentityResources.OpenId(),
                 new IdentityResources.Profile(),
                 new IdentityResources.Email()
            };

        public static List<TestUser> TestUsers
           => new TestUser[] { }.ToList();


    }
}
