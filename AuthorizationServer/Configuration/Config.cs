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
                new ApiResource("libraryAPI", "LibraryAPI"),
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
                    AllowedGrantTypes = GrantTypes.HybridAndClientCredentials,
                    AllowAccessTokensViaBrowser = true,
                    RequireConsent = false,
                    EnableLocalLogin = true,
                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Email,
                        "libraryAPI"
                    },
                    ClientSecrets = new List<Secret>(){new Secret("library".Sha256())}
                },
            };
        }
    }
}
