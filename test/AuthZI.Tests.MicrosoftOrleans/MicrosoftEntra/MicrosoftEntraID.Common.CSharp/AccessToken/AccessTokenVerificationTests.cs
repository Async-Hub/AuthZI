using AuthZI.Identity.MicrosoftEntra;
using AuthZI.MicrosoftEntra;
using AuthZI.Security.AccessToken;
using AuthZI.Tests.Common.Xunit;
using AuthZI.Tests.MicrosoftOrleans.MicrosoftEntra.MicrosoftEntraID.Common.Initialization;
using System.Threading.Tasks;
using Xunit;

namespace AuthZI.Tests.MicrosoftOrleans.MicrosoftEntra.MicrosoftEntraID.Common.AccessToken;

public class AccessTokenVerificationTestsBase(ITestOutputHelper output)
{
  [Theory]
  [MemberData(nameof(TestData.Users), MemberType = typeof(TestData), DisableDiscoveryEnumeration = true)]
  public async Task TheSystemCanVerifyJwtTokenFromAzureAdEndpoint(string userName)
  {
    var discoveryDocumentProvider = new DiscoveryDocumentProvider(TestData.Web1ClientApp.DiscoveryEndpointUrl);

    var accessToken = await AccessTokenProvider.GetAccessTokenForUserOnWebClient1Async(userName);
    output.WriteLine(accessToken);

    var logger = new TestLogger<AccessTokenIntrospectionService>(output);
    IAccessTokenIntrospectionService accessTokenIntrospectionService =
      new AccessTokenIntrospectionService(
        TestData.Web1ClientApp,
        discoveryDocumentProvider,
        new ClaimTypeResolverDefault(),
        logger);

    var result = await accessTokenIntrospectionService.IntrospectTokenAsync(accessToken);

    Assert.True(result.IsOk);
  }

  [Theory]
  [MemberData(nameof(TestData.Users), MemberType = typeof(TestData), DisableDiscoveryEnumeration = true)]
  public async Task TheSystemRejectsJwtTokenWithInvalidAudience(string userName)
  {
    var discoveryDocumentProvider = new DiscoveryDocumentProvider(TestData.Web1ClientApp.DiscoveryEndpointUrl);

    var accessToken = await AccessTokenProvider.GetAccessTokenForUserOnWebClient2Async(userName);
    output.WriteLine(accessToken);

    var logger = new TestLogger<AccessTokenIntrospectionService>(output);
    IAccessTokenIntrospectionService accessTokenIntrospectionService =
      new AccessTokenIntrospectionService(
        TestData.Web1ClientApp,
        discoveryDocumentProvider,
        new ClaimTypeResolverDefault(),
        logger);

    var result = await accessTokenIntrospectionService.IntrospectTokenAsync(accessToken);

    Assert.True(result.IsError);
  }
}
