using AuthZI.Tests.MicrosoftOrleans.MicrosoftEntra.MicrosoftEntraID.Common.Initialization;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace AuthZI.Tests.MicrosoftOrleans.MicrosoftEntra.MicrosoftEntraID.Common.AccessToken;

public class AudienceValidationTestsBase(
  ITestOutputHelper output,
  IAccessTokenRetriever accessTokenProvider)
{
  private static string[] GetAudienceClaimValues(string accessToken)
  {
    var token = new JwtSecurityTokenHandler().ReadJwtToken(accessToken);

    return token.Claims
      .Where(claim => claim.Type == "aud")
      .Select(claim => claim.Value)
      .ToArray();
  }

  [Theory]
  [MemberData(nameof(TestData.Users), MemberType = typeof(TestData), DisableDiscoveryEnumeration = true)]
  public async Task RealAccessTokenAudienceIsIncludedInExpectedAudiencesForConfiguredApp(string userName)
  {
    var accessToken = await accessTokenProvider.GetAccessTokenForUserAsync(nameof(TestData.WebClient1), userName);
    output.WriteLine(accessToken);

    var audienceClaims = GetAudienceClaimValues(accessToken);
    var expectedAudiences = TestData.WebClient1.ValidAudiences.ToArray();
    var hasExpectedAudience = audienceClaims.Any(expectedAudiences.Contains);

    Assert.True(hasExpectedAudience);
  }

  [Theory]
  [MemberData(nameof(TestData.Users), MemberType = typeof(TestData), DisableDiscoveryEnumeration = true)]
  public async Task RealAccessTokenFromAnotherAppHasAudienceOutsideConfiguredExpectedAudiences(string userName)
  {
    var accessToken = await accessTokenProvider.GetAccessTokenForUserAsync(nameof(TestData.WebClient2), userName);
    output.WriteLine(accessToken);

    var audienceClaims = GetAudienceClaimValues(accessToken);
    var expectedAudiences = TestData.WebClient1.ValidAudiences.ToArray();
    var hasUnexpectedAudience = audienceClaims.Any(audience => !expectedAudiences.Contains(audience));

    Assert.True(hasUnexpectedAudience);
  }
}
