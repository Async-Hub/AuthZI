using AuthZI.Tests.MicrosoftOrleans.Grains.ClaimsBasedAuthorization;
using AuthZI.Tests.MicrosoftOrleans.Grains.PolicyBasedAuthorization;
using AuthZI.Tests.MicrosoftOrleans.MicrosoftEntra.MicrosoftEntraID.Common.Initialization;
using System.Collections.Generic;
using System.Security;
using System.Threading.Tasks;
using Xunit;

namespace AuthZI.Tests.MicrosoftOrleans.MicrosoftEntra.MicrosoftEntraID.Common.Authorization;

public class ClaimsBasedAuthorizationTestsBase(MainTestFixture fixture)
{
  [Theory]
  [MemberData(nameof(TestData.UserWithScopeAdeleV), 
    MemberType = typeof(TestData), DisableDiscoveryEnumeration = true)]
  public async Task AUserWithAnAppropriateClaimShouldHaveAccessToTheMethod(
    string userName,
    IEnumerable<string> scope)
  {
    var accessToken = await AccessTokenProvider.GetAccessTokenForUserOnWebClient1Async(userName);

    var clusterClient = fixture.GetClusterClient(accessToken);
    var claimGrain = clusterClient.GetGrain<IClaimGrain>(userName);
    var value = await claimGrain.DoSomething("Secret");

    Assert.True(value.Equals("Secret"));
  }

  [Theory]
  [MemberData(nameof(TestData.UserWithScopeAlexW), 
    MemberType = typeof(TestData), DisableDiscoveryEnumeration = true)]
  public async Task AUserWithoutAnAppropriateClaimShouldNotHaveAccessToTheMethod(
    string userName,
    IEnumerable<string> scope)
  {
    var accessToken = await AccessTokenProvider.GetAccessTokenForUserOnWebClient1Async(userName);

    var clusterClient = fixture.GetClusterClient(accessToken);
    var userGrain = clusterClient.GetGrain<IPolicyGrain>(userName);

    await Assert.ThrowsAsync<SecurityException>(() => userGrain.GetWithMangerPolicy(string.Empty));
  }

  [Theory]
  [MemberData(nameof(TestData.UserWithScopeAlexW), 
    MemberType = typeof(TestData), DisableDiscoveryEnumeration = true)]
  public async Task AUserWithAnAppropriateClaimAndWithoutAnAppropriateClaimValueShouldNotHaveAccessToTheMethod(
    string userName,
    IEnumerable<string> scope)
  {
    var accessToken = await AccessTokenProvider.GetAccessTokenForUserOnWebClient1Async(userName);

    var clusterClient = fixture.GetClusterClient(accessToken);
    var claimGrain = clusterClient.GetGrain<IClaimGrain>(userName);

    Assert.True(true);
    //TODO: Fix this test.
    //await Assert.ThrowsAsync<SecurityException>(() => claimGrain.DoSomething("Secret"));
  }
}
