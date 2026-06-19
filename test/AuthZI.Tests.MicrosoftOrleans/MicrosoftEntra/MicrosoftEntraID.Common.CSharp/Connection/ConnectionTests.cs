using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AuthZI.Deploy.MicrosoftEntra.Configuration;
using AuthZI.Tests.MicrosoftOrleans.MicrosoftEntra.MicrosoftEntraID.Common;
using Xunit;

namespace AuthZI.Tests.MicrosoftOrleans.MicrosoftEntra.MicrosoftEntraID.Common.Connection;

public class MicrosoftEntraIdTestsBase(ITestOutputHelper output)
{
  [Theory]
  [MemberData(nameof(TestData.Users), MemberType = typeof(TestData), DisableDiscoveryEnumeration = true)]
  public async Task TheSystemCanObtainAccessTokenFromMicrosoftEntraIDEndpoint(string userName, string password)
  {
    var accessToken = await AccessTokenProvider.getAccessTokenForUserOnWebClient1Async(userName, password);
    output.WriteLine(accessToken);

    Assert.False(string.IsNullOrWhiteSpace(accessToken));
  }
}

public class AzureActiveDirectoryB2CTestsBase(ITestOutputHelper output)
{
  public static IEnumerable<object[]> Input { get; } =
    new[] { new object[] { Credentials.AzureActiveDirectoryB2C1.AdeleV.Name, Credentials.AzureActiveDirectoryB2C1.AdeleV.Password } };

  [Theory]
  [MemberData(nameof(Input))]
  public async Task TheSystemCanObtainAccessTokenFromAzureADB2CEndpoint(string userName, string password)
  {
    var accessToken = await AccessTokenProvider.getAccessTokenForUserOnWebClient1Async(userName, password);
    output.WriteLine(accessToken);

    Assert.False(string.IsNullOrWhiteSpace(accessToken));
  }
}
