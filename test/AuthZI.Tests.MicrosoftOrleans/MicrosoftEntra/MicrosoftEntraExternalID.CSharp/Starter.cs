using AuthZI.Deploy.MicrosoftEntra.Configuration;
using AuthZI.Tests.MicrosoftOrleans.MicrosoftEntra.MicrosoftEntraExternalID.CSharp;
using AuthZI.Tests.MicrosoftOrleans.MicrosoftEntra.MicrosoftEntraID.Common.Initialization;
using Orleans;
using System.Text.Json;
using Xunit;

[assembly: ApplicationPart("AuthZI.Tests.MicrosoftOrleans.Grains")]
[assembly: AssemblyFixture(typeof(ExternalIdMainTestFixture))]
[assembly: CollectionBehavior(DisableTestParallelization = true)]

namespace AuthZI.Tests.MicrosoftOrleans.MicrosoftEntra.MicrosoftEntraExternalID.CSharp;

public class ExternalIdMainTestFixture : MainTestFixture
{
  public ExternalIdMainTestFixture()
  {
    var microsoftEntraIdCredentialsJson = Literals.microsoftEntraExternalIDCredentialsJson;
    var credentials = JsonSerializer.Deserialize<MicrosoftEntraCredentials>(microsoftEntraIdCredentialsJson)!;
    Credentials = credentials;
    AccessTokenRetriever = new AccessTokenRetriever(true);
  }
}