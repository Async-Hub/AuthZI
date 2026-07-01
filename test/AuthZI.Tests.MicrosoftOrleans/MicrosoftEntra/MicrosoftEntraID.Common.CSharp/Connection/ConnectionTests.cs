using AuthZI.Deploy.MicrosoftEntra.Configuration;
using AuthZI.Tests.MicrosoftOrleans.MicrosoftEntra.MicrosoftEntraID.Common.Initialization;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace AuthZI.Tests.MicrosoftOrleans.MicrosoftEntra.MicrosoftEntraID.Common.Connection;

public class MicrosoftEntraIdTestsBase(ITestOutputHelper output)
{
  [Theory]
  [MemberData(nameof(TestData.Users), MemberType = typeof(TestData), DisableDiscoveryEnumeration = true)]
  public async Task TheSystemCanObtainAccessTokenFromMicrosoftEntraIdEndpoint(string userName)
  {
    var accessToken = await AccessTokenProvider.GetAccessTokenForUserOnWebClient1Async(userName);
    output.WriteLine(accessToken);

    Assert.False(string.IsNullOrWhiteSpace(accessToken));
  }
}

public class AzureActiveDirectoryB2CTestsBase(ITestOutputHelper output)
{
  public static IEnumerable<object[]> Input { get; } =
    [[Credentials.AzureActiveDirectoryB2C1.AdeleV.Name]];

  [Theory]
  [MemberData(nameof(Input))]
  public async Task TheSystemCanObtainAccessTokenFromAzureAdb2CEndpoint(string userName)
  {
    var accessToken = await AccessTokenProvider.GetAccessTokenForUserOnWebClient1Async(userName);
    output.WriteLine(accessToken);

    Assert.False(string.IsNullOrWhiteSpace(accessToken));
  }
}
