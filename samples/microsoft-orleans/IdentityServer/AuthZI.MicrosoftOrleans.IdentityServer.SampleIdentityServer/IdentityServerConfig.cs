using Duende.IdentityServer.Models;
using Duende.IdentityServer.Test;
using IdentityModel;

namespace AuthZI.MicrosoftOrleans.IdentityServer.SampleIdentityServer;

public static class IdentityServerConfig
{
  public static IEnumerable<ApiScope> GetApiScopes()
  {
    return new List<ApiScope>
      {
        new ApiScope(name: "Api1", displayName: "Api1"),
        new ApiScope(name: "Api1.Read", displayName: "Api1.Read"),
        new ApiScope(name: "Api1.Write", displayName: "Api1.Read"),
        new ApiScope(name: "Cluster", displayName: "Cluster")
      };
  }

  public static IEnumerable<ApiResource> GetApiResources()
  {
    var resources = new List<ApiResource>();

    var api1 = new ApiResource("Api1", [JwtClaimTypes.Email, JwtClaimTypes.Role]);

    api1.ApiSecrets.Add(new Secret("TFGB=?Gf3UvH+Uqfu_5p".Sha256()));
    api1.Scopes.Add("Api1.Read");
    api1.Scopes.Add("Api1.Write");

    resources.Add(api1);

    var orleans = new ApiResource("Cluster");

    orleans.ApiSecrets.Add(new Secret("@3x3g*RLez$TNU!_7!QW".Sha256()));
    orleans.Scopes.Add("Cluster");

    resources.Add(orleans);

    return resources;
  }

  public static IEnumerable<Client> GetClients()
  {
    return new List<Client>
      {
        new Client
        {
          ClientId = "ConsoleClient",
          ClientName = "Console Client",
          AllowedGrantTypes = GrantTypes.ResourceOwnerPasswordAndClientCredentials,
          ClientSecrets =
          {
            new Secret("KHG+TZ8htVx2h3^!vJ65".Sha256())
          },
          Claims = new List<ClientClaim> { new ClientClaim(JwtClaimTypes.Role, "Admin") },
          AllowedScopes =
          {
            "Api1", "Api1.Read", "Api1.Write", "Cluster",
            JwtClaimTypes.Email,
            JwtClaimTypes.Role
          },
          AllowOfflineAccess = true
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