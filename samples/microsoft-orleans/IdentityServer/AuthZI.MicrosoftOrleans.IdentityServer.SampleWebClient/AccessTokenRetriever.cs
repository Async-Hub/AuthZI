using IdentityModel.Client;
using System.Diagnostics;

namespace AuthZI.MicrosoftOrleans.IdentityServer.SampleWebClient
{
  public class AccessTokenRetriever
  {
    internal static async Task<string> RetrieveToken(string url)
    {
      var discoveryClient = new HttpClient()
      {
        BaseAddress = new Uri(url)
      };

      var discoveryResponse = await discoveryClient.GetDiscoveryDocumentAsync();

      if (discoveryResponse.IsError)
      {
        throw new Exception(discoveryResponse.Error);
      }

      var httpClient = new HttpClient();

      var passwordTokenRequest = new PasswordTokenRequest()
      {
        ClientId = "ConsoleClient",
        ClientSecret = "KHG+TZ8htVx2h3^!vJ65",
        Address = discoveryResponse.TokenEndpoint,
        UserName = "Bob",
        Password = "Pass123$",
        Scope = "Api1 Api1.Read Api1.Write Cluster"
      };

      var tokenResponse = await httpClient.RequestPasswordTokenAsync(passwordTokenRequest);

      if (tokenResponse.IsError)
      {
        throw new Exception(tokenResponse.Error);
      }

      if (tokenResponse is null)
      {
        throw new Exception("tokenResponse is null.");
      }

      Debug.Assert(tokenResponse.AccessToken != null, "tokenResponse.AccessToken != null");
      return tokenResponse.AccessToken;
    }
  }
}