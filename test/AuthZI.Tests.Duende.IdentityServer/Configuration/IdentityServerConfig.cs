using System.Collections.Generic;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Test;
using IdentityModel;

namespace AuthZI.Tests.Duende.IdentityServer.Configuration
{
  public static class IdentityServerConfig
  {
    public static IEnumerable<ApiScope> GetApiScopes()
    {
      return new List<ApiScope>
            {
                new ApiScope(name: "Api1",   displayName: "Api1")
            };
    }

    public static IEnumerable<ApiResource> GetApiResources()
    {
      var resources = new List<ApiResource>();

      var api1 = new ApiResource("Api1", new[] { JwtClaimTypes.Email, JwtClaimTypes.Role });
      api1.ApiSecrets.Add(new Secret("Secret".Sha256()));
      resources.Add(api1);
      api1.Scopes.Add("Api1");

      return resources;
    }

    public static IEnumerable<Client> GetClients()
    {
      return new List<Client>
            {
                new Client
                {
                    ClientId = "WebClient",
                    AccessTokenType = AccessTokenType.Jwt,
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    AllowOfflineAccess = true,
                    ClientSecrets =
                    {
                        new Secret("Secret".Sha256())
                    },
                    AllowedScopes =
                    {
                        "Api1"
                    }
                }
            };
    }

    public static List<TestUser> GetUsers()
    {
      return TestUsers.Users;
    }

    public static IEnumerable<IdentityResource> GetIdentityResources()
    {
      return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Email()
            };
    }
  }
}