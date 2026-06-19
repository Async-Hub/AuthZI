using System;
using System.Collections.Generic;
using System.Security;
using System.Threading.Tasks;
using AuthZI.MicrosoftOrleans.Authorization;
using AuthZI.Tests.MicrosoftOrleans.Grains.SimpleAuthorization;
using AuthZI.Tests.MicrosoftOrleans.MicrosoftEntra.MicrosoftEntraID.Common.Initialization;
using Xunit;

namespace AuthZI.Tests.MicrosoftOrleans.MicrosoftEntra.MicrosoftEntraID.Common.Authorization;

public class SimpleAuthorizationTestsBase(MicrosoftEntraIdTestFixture fixture)
{
  [Theory]
  [MemberData(nameof(TestData.UserWithScopeAdeleV), MemberType = typeof(TestData), DisableDiscoveryEnumeration = true)]
  public async Task AnAuthenticatedUserCanInvokeTheGrainMethod(
    string userName,
    string password,
    IEnumerable<string> scope)
  {
    var accessToken = await AccessTokenProvider.getAccessTokenForUserOnWebClient1Async(userName, password);

    var clusterClient = fixture.GetClusterClient(accessToken);
    var simpleGrain = clusterClient.GetGrain<ISimpleGrain>(Guid.NewGuid());
    var value = await simpleGrain.GetWithAuthenticatedUser("Secret");

    Assert.True(value.Equals("Secret"));
  }

  [Theory]
  [MemberData(nameof(TestData.UserWithScopeAdeleV), MemberType = typeof(TestData), DisableDiscoveryEnumeration = true)]
  public async Task AnAuthenticatedUserOnAnUnauthenticatedClientCannotInvokeTheGrainMethod(
    string userName,
    string password,
    IEnumerable<string> scope)
  {
    var accessToken = await AccessTokenProvider.getAccessTokenForUserOnWebClient2Async(userName, password);

    var clusterClient = fixture.GetClusterClient(accessToken);
    var simpleGrain = clusterClient.GetGrain<ISimpleGrain>(Guid.NewGuid());

    var result = await Assert.ThrowsAsync<SecurityException>(() => simpleGrain.GetValue());
    Assert.True(result.Message == SecureGrain.AccessDeniedMessage);
  }

  [Fact]
  public async Task AnAnonymousUserCannotInvokeTheGrainMethod()
  {
    var accessToken = string.Empty;

    var clusterClient = fixture.GetClusterClient(accessToken);
    var simpleGrain = clusterClient.GetGrain<ISimpleGrain>(Guid.NewGuid());

    await Assert.ThrowsAsync<InvalidOperationException>(() => simpleGrain.GetWithAuthenticatedUser(string.Empty));
    Assert.True(true);
  }

  [Fact]
  public async Task AnAnonymousUserCanInvokeAGrainMethodWithAllowAnonymousAttribute()
  {
    var accessToken = string.Empty;

    var clusterClient = fixture.GetClusterClient(accessToken);
    var simpleGrain = clusterClient.GetGrain<ISimpleGrain>(Guid.Empty);
    var value = await simpleGrain.GetWithAnonymousUser("Secret");

    Assert.True(value.Equals("Secret"));
  }
}
