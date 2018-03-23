using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.Models;
using System.Linq;

namespace AuthorizationServer.Configuration
{
    public static class ConfigurationDbContextExtensions
    {
        public static void SeedData(this ConfigurationDbContext context)
        {
            if (!context.Clients.Any())
            {
                foreach (Client client in Config.Clients())
                {
                    context.Clients.Add(client.ToEntity());
                }
                context.SaveChanges();
            }

            if (!context.ApiResources.Any())
            {
                foreach (ApiResource apiResource in Config.ApiResources())
                {
                    context.ApiResources.Add(apiResource.ToEntity());
                }
                context.SaveChanges();
            }
        }
    }
}
