using System.Collections.Generic;
using System.Threading.Tasks;
using AuthZI.Security;
using AuthZI.Tests.MicrosoftOrleans.Grains.RoleBasedAuthorization;
using Xunit;

namespace AuthZI.Tests.MicrosoftOrleans.MicrosoftEntra.MicrosoftEntraID.Common.Authorization;

public class RoleBasedAuthorizationTestsBase
{
  [Theory]
  [MemberData(nameof(TestData.UserWithScopeAdeleV), MemberType = typeof(TestData), DisableDiscoveryEnumeration = true)]
  public async Task MultipleRolesAsACommaSeparatedListShouldWorkWhenTheUserHasBothRoles(
    string userName,
    string password,
    IEnumerable<string> scope)
  {
    var accessToken = await AccessTokenProvider.getAccessTokenForUserOnWebClient1Async(userName, password);

    var clusterClient = RootConfiguration.getClusterClient(accessToken);
    var userGrain = clusterClient.GetGrain<IManagerGrain>(userName);
    var value = await userGrain.GetWithCommaSeparatedRoles("Secret");

    Assert.True(value.Equals("Secret"));
  }

  [Theory]
  [MemberData(nameof(TestData.UserWithScopeAlexW), MemberType = typeof(TestData), DisableDiscoveryEnumeration = true)]
  public async Task MultipleRolesAsACommaSeparatedListShouldNotWorkWhenTheUserHasOnlyOneRole(
    string userName,
    string password,
    IEnumerable<string> scope)
  {
    var accessToken = await AccessTokenProvider.getAccessTokenForUserOnWebClient1Async(userName, password);

    var clusterClient = RootConfiguration.getClusterClient(accessToken);
    var userGrain = clusterClient.GetGrain<IManagerGrain>(userName);

    await Assert.ThrowsAsync<AuthorizationException>(() => userGrain.GetWithCommaSeparatedRoles("Secret"));
  }

  [Theory]
  [MemberData(nameof(TestData.UserWithScopeAdeleV), MemberType = typeof(TestData), DisableDiscoveryEnumeration = true)]
  public async Task MultipleRolesAppliedAsMultipleAttributesShouldWorkWhenTheUserHasBothRoles(
    string userName,
    string password,
    IEnumerable<string> scope)
  {
    var accessToken = await AccessTokenProvider.getAccessTokenForUserOnWebClient1Async(userName, password);

    var clusterClient = RootConfiguration.getClusterClient(accessToken);
    var userGrain = clusterClient.GetGrain<IManagerGrain>(userName);
    var value = await userGrain.GetWithMultipleRoleAttributes("Secret");

    Assert.True(value.Equals("Secret"));
  }

  [Theory]
  [MemberData(nameof(TestData.UserWithScopeAlexW), MemberType = typeof(TestData), DisableDiscoveryEnumeration = true)]
  public async Task MultipleRolesAppliedAsMultipleAttributesShouldNotWorkWhenTheUserHasOnlyOneRole(
    string userName,
    string password,
    IEnumerable<string> scope)
  {
    var accessToken = await AccessTokenProvider.getAccessTokenForUserOnWebClient1Async(userName, password);

    var clusterClient = RootConfiguration.getClusterClient(accessToken);
    var userGrain = clusterClient.GetGrain<IManagerGrain>(userName);

    await Assert.ThrowsAsync<AuthorizationException>(() => userGrain.GetWithMultipleRoleAttributes("Secret"));
  }
}
