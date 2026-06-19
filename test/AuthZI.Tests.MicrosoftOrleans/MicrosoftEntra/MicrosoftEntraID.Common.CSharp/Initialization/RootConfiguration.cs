using System;
using System.Threading.Tasks;
using AuthZI.Security;
using AuthZI.Tests.MicrosoftOrleans.MicrosoftEntra.MicrosoftEntraID.Common;
using Orleans;

public static class RootConfiguration
{
  private static readonly ConfigurableAccessTokenProvider GlobalAccessToken = new();

  public static IAccessTokenProvider accessTokenProvider => GlobalAccessToken;

  public static IClusterClient getClusterClient(string accessToken)
  {
    GlobalAccessToken.AccessToken = accessToken;

    return TestData.IClusterClient;
  }

  private sealed class ConfigurableAccessTokenProvider : IAccessTokenProvider
  {
    public string AccessToken { private get; set; } = string.Empty;

    public Task<string> RetrieveTokenAsync() => Task.FromResult(AccessToken);
  }
}
