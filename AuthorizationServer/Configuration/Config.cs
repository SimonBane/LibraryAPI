using IdentityServer4;
using IdentityServer4.Models;
using System.Collections.Generic;
using Resources = AuthorizationServer.Properties.Resources;

namespace AuthorizationServer.Configuration
{
    public class Config
    {
        public static IEnumerable<ApiResource> ApiResources()
        {
            return new ApiResource[]
            {
                new ApiResource("libraryAPI", "Library API")
                {
                    ApiSecrets = {new Secret("APIsecret".Sha256())}
                },
            };
        }

        public static IEnumerable<Client> Clients()
        {
            return new Client[]
            {
                new Client()
                {
                    ClientId = "library-API",
                    ClientName = "LibraryAPI",
                    ClientSecrets = {new Secret("library".Sha256())},

                    AllowedGrantTypes = GrantTypes.HybridAndClientCredentials,
                    UpdateAccessTokenClaimsOnRefresh = true,
                    AllowAccessTokensViaBrowser = true,

                    RequireConsent = false,
                    AllowOfflineAccess = true,
                    RedirectUris = new [] {$"{Resources.ClientAddress}/signin-oidc"},
                    PostLogoutRedirectUris = {$"{Resources.ClientAddress}/signout-callback-oidc"},
                    EnableLocalLogin = true,
                    AlwaysIncludeUserClaimsInIdToken = true,

                    AllowedScopes =

                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Email,
                        "libraryAPI"
                    },
                },
            };
        }
    }
}
