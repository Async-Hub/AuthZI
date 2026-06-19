using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AuthZI.Security;
using AuthZI.Tests.MicrosoftOrleans.Grains.ClaimsBasedAuthorization;
using AuthZI.Tests.MicrosoftOrleans.Grains.PolicyBasedAuthorization;
using Xunit;

namespace AuthZI.Tests.MicrosoftOrleans.MicrosoftEntra.MicrosoftEntraID.Common.Authorization;

public class ClaimsBasedAuthorizationTestsBase
{
  [Theory]
  [MemberData(nameof(TestData.UserWithScopeAdeleV), MemberType = typeof(TestData), DisableDiscoveryEnumeration = true)]
  public async Task AUserWithAnAppropriateClaimShouldHaveAccessToTheMethod(
    string userName,
    string password,
    IEnumerable<string> scope)
  {
    var accessToken = await AccessTokenProvider.getAccessTokenForUserOnWebClient1Async(userName, password);

    var clusterClient = RootConfiguration.getClusterClient(accessToken);
    var claimGrain = clusterClient.GetGrain<IClaimGrain>(userName);
    var value = await claimGrain.DoSomething("Secret");

    Assert.True(value.Equals("Secret"));
  }

  [Theory]
  [MemberData(nameof(TestData.UserWithScopeAlexW), MemberType = typeof(TestData), DisableDiscoveryEnumeration = true)]
  public async Task AUserWithoutAnAppropriateClaimShouldNotHaveAccessToTheMethod(
    string userName,
    string password,
    IEnumerable<string> scope)
  {
    var accessToken = await AccessTokenProvider.getAccessTokenForUserOnWebClient1Async(userName, password);

    var clusterClient = RootConfiguration.getClusterClient(accessToken);
    var userGrain = clusterClient.GetGrain<IPolicyGrain>(userName);

    await Assert.ThrowsAsync<AuthorizationException>(() => userGrain.GetWithMangerPolicy(string.Empty));
  }

  [Theory]
  [MemberData(nameof(TestData.UserWithScopeAlexW), MemberType = typeof(TestData), DisableDiscoveryEnumeration = true)]
  public async Task AUserWithAnAppropriateClaimAndWithoutAnAppropriateClaimValueShouldNotHaveAccessToTheMethod(
    string userName,
    string password,
    IEnumerable<string> scope)
  {
    var accessToken = await AccessTokenProvider.getAccessTokenForUserOnWebClient1Async(userName, password);

    var clusterClient = RootConfiguration.getClusterClient(accessToken);
    var claimGrain = clusterClient.GetGrain<IClaimGrain>(userName);

    await Assert.ThrowsAsync<AuthorizationException>(() => claimGrain.DoSomething("Secret"));
  }
}
