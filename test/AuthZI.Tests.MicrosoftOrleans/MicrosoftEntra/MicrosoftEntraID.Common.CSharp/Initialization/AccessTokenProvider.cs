using AuthZI.Identity.MicrosoftEntra;
using System;
using System.Threading.Tasks;

namespace AuthZI.Tests.MicrosoftOrleans.MicrosoftEntra.MicrosoftEntraID.Common.Initialization;

public static class AccessTokenProvider
{
  private static Task<string> GetAccessTokenForUserAsync(
    MicrosoftEntraApp entraIdApp,
    string userName)
  {
    if (!TestData.UserPasswords.TryGetValue(userName, out var password))
    {
      throw new InvalidOperationException($"Password for test user '{userName}' is not configured.");
    }

    var accessToken = TestData.GetAccessTokenForUserOnMicrosoftEntraAppAsync(entraIdApp, userName, password);

    return Task.FromResult(accessToken);
  }

  public static Task<string> GetAccessTokenForUserOnWebClient1Async(string userName) =>
    GetAccessTokenForUserAsync(TestData.Web1ClientApp, userName);

  public static Task<string> GetAccessTokenForUserOnWebClient2Async(string userName) =>
    GetAccessTokenForUserAsync(TestData.Web2ClientApp, userName);
}