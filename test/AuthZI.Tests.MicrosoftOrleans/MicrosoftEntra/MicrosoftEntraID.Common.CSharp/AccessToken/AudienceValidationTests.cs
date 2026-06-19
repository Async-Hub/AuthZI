using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace AuthZI.Tests.MicrosoftOrleans.MicrosoftEntra.MicrosoftEntraID.Common;

public class AudienceValidationTestsBase(ITestOutputHelper output)
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
  public async Task RealAccessTokenAudienceIsIncludedInExpectedAudiencesForConfiguredApp(string userName, string password)
  {
    var accessToken = await AccessTokenProvider.getAccessTokenForUserOnWebClient1Async(userName, password);
    output.WriteLine(accessToken);

    var audienceClaims = GetAudienceClaimValues(accessToken);
    var expectedAudiences = TestData.Web1ClientApp.ValidAudiences.ToArray();
    var hasExpectedAudience = audienceClaims.Any(expectedAudiences.Contains);

    Assert.True(hasExpectedAudience);
  }

  [Theory]
  [MemberData(nameof(TestData.Users), MemberType = typeof(TestData), DisableDiscoveryEnumeration = true)]
  public async Task RealAccessTokenFromAnotherAppHasAudienceOutsideConfiguredExpectedAudiences(string userName, string password)
  {
    var accessToken = await AccessTokenProvider.getAccessTokenForUserOnWebClient2Async(userName, password);
    output.WriteLine(accessToken);

    var audienceClaims = GetAudienceClaimValues(accessToken);
    var expectedAudiences = TestData.Web1ClientApp.ValidAudiences.ToArray();
    var hasUnexpectedAudience = audienceClaims.Any(audience => !expectedAudiences.Contains(audience));

    Assert.True(hasUnexpectedAudience);
  }
}
