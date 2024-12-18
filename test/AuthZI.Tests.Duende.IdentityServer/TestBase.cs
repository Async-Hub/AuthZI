using System.Net.Http;
using System.Threading.Tasks;
using AuthZI.Identity.Duende.IdentityServer;
using AuthZI.Tests.Duende.IdentityServer.Configuration;
using IdentityModel.Client;

namespace AuthZI.Tests.Duende.IdentityServer;

public class TestBase
{
  private readonly HttpClient _identityServerClient;

  protected DiscoveryDocument DiscoveryDocument { get; }

  protected TestBase()
  {
    var identityServer4 = TestIdentityServer4Builder.StartNew();
    _identityServerClient = identityServer4.CreateClient();

    var discoveryResponse = _identityServerClient.GetDiscoveryDocumentAsync().Result;

    DiscoveryDocument = new DiscoveryDocument
    {
      IntrospectionEndpoint = discoveryResponse.IntrospectionEndpoint,
      Issuer = discoveryResponse.Issuer,
      Keys = discoveryResponse.KeySet.Keys,
      TokenEndpoint = discoveryResponse.TokenEndpoint
    };
  }

  protected async Task<string> RequestClientCredentialsTokenAsync(string clientId, string clientSecret,
      string scope)
  {
    var response = await _identityServerClient.RequestClientCredentialsTokenAsync(
        new ClientCredentialsTokenRequest()
        {
          Address = DiscoveryDocument.TokenEndpoint,
          Scope = scope,
          ClientId = clientId,
          ClientSecret = clientSecret
        });

    return response.AccessToken;
  }
}