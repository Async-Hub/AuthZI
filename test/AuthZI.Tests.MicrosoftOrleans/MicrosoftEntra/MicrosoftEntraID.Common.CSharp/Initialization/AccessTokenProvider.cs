using System.Threading.Tasks;
using AuthZI.Identity.MicrosoftEntra;
using AuthZI.Tests.MicrosoftOrleans.MicrosoftEntra.MicrosoftEntraID.Common;
using AuthZI.Tests.MicrosoftOrleans.MicrosoftEntra.MicrosoftEntraID.Common.Initialization;

public static class AccessTokenProvider
{
  public static Task<string> getAccessTokenForUserAsync(
    MicrosoftEntraApp entraIDApp,
    string userName,
    string password)
  {
    var accessToken = TestData.GetAccessTokenForUserOnMicrosoftEntraAppAsync(entraIDApp, userName, password);

    return Task.FromResult(accessToken);
  }

  public static Task<string> getAccessTokenForUserOnWebClient1Async(string userName, string password) =>
    getAccessTokenForUserAsync(TestData.Web1ClientApp, userName, password);

  public static Task<string> getAccessTokenForUserOnWebClient2Async(string userName, string password) =>
    getAccessTokenForUserAsync(TestData.Web2ClientApp, userName, password);
}
