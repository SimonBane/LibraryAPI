using IdentityServer4;
using IdentityServer4.Models;
using System.Collections.Generic;

namespace AuthorizationServer.Configuration
{
    public class Config
    {
        public static IEnumerable<ApiResource> ApiResources()
        {
            return new ApiResource[]
            {
                new ApiResource("libraryAPI", "LibraryAPI")
                {
                    ApiSecrets = {new Secret("library".Sha256())},
                },
            };
        }

        public static IEnumerable<Client> Clients()
        {
            return new Client[]
            {
                new Client()
                {
                    ClientId = "library-api",
                    ClientName = "LibraryAPI",
                    AllowedGrantTypes = GrantTypes.Code,
                    AllowAccessTokensViaBrowser = true,
                    RequireConsent = false,
                    RedirectUris = {"https://www.getpostman.com/oauth2/callback"},
                    EnableLocalLogin = true,
                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Email,
                        "library_api"
                    },
                    ClientSecrets = new List<Secret>(){new Secret("library".Sha256())}
                },
            };
        }
    }
}
