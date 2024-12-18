using Duende.IdentityServer.Models;
using Duende.IdentityServer.Test;
using IdentityModel;

namespace AuthZI.MicrosoftOrleans.IdentityServer.SampleIdentityServer;

public static class IdentityServerConfig
{
  private const string Api1 = "Api1";
  private const string Api1Read = "Api1.Read";
  private const string Api1Write = "Api1.Write";
  private const string Cluster = "Cluster";

  public static IEnumerable<ApiScope> GetApiScopes() =>
    [
        new(name: Api1, displayName: Api1),
        new(name: Api1Read, displayName: Api1Read),
        new(name: Api1Write, displayName: Api1Read),
        new(name: Cluster, displayName: Cluster)
    ];

  public static IEnumerable<ApiResource> GetApiResources()
  {
    var resources = new List<ApiResource>();

    var api1 = new ApiResource(Api1, [JwtClaimTypes.Email, JwtClaimTypes.Role]);

    api1.ApiSecrets.Add(new Secret("TFGB=?Gf3UvH+Uqfu_5p".Sha256()));
    api1.Scopes.Add(Api1Read);
    api1.Scopes.Add(Api1Write);

    resources.Add(api1);

    var orleans = new ApiResource(Cluster);

    orleans.ApiSecrets.Add(new Secret("@3x3g*RLez$TNU!_7!QW".Sha256()));
    orleans.Scopes.Add(Cluster);

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
            Api1, Api1Read, Api1Write, Cluster,
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

  public static IEnumerable<IdentityResource> GetIdentityResources() => [
        new IdentityResources.OpenId(),
        new IdentityResources.Profile(),
        new IdentityResources.Email()
      ];
}