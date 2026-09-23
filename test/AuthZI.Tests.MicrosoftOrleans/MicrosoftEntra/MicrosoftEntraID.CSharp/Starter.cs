using AuthZI.Deploy.MicrosoftEntra.Configuration;
using AuthZI.Tests.MicrosoftOrleans.MicrosoftEntra.MicrosoftEntraID.Common.Initialization;
using AuthZI.Tests.MicrosoftOrleans.MicrosoftEntra.MicrosoftEntraID.CSharp;
using Orleans;
using System.Text.Json;
using Xunit;
using Xunit.Sdk;
using Xunit.v3;

[assembly: ApplicationPart("AuthZI.Tests.MicrosoftOrleans.Grains")]
[assembly: AssemblyFixture(typeof(EntraIdMainTestFixture))]
[assembly: Parallelization(Mode = ParallelMode.None)]

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