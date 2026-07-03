using AuthZI.Deploy.MicrosoftEntra.Configuration;
using AuthZI.Tests.MicrosoftOrleans.MicrosoftEntra.MicrosoftEntraID.Common.Initialization;
using Orleans;
using System.Text.Json;
using Xunit;

[assembly: ApplicationPart("AuthZI.Tests.MicrosoftOrleans.Grains")]
[assembly: AssemblyFixture(
  typeof(AuthZI.Tests.MicrosoftOrleans.MicrosoftEntra.MicrosoftEntraID.CSharp.EntraIdMainTestFixture))]
[assembly: CollectionBehavior(DisableTestParallelization = true)]

namespace AuthZI.Tests.MicrosoftOrleans.MicrosoftEntra.MicrosoftEntraID.CSharp;

public class EntraIdMainTestFixture : MainTestFixture
{
  public EntraIdMainTestFixture()
  {
    var microsoftEntraIdCredentialsJson = Literals.microsoftEntraCredentialsJson;
    var credentials = JsonSerializer.Deserialize<MicrosoftEntraCredentials>(microsoftEntraIdCredentialsJson)!;
    Credentials = credentials;
    AccessTokenRetriever = new AccessTokenRetriever(false);
  }
}