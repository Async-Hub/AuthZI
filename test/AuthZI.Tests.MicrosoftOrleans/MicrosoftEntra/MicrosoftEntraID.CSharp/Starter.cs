using AuthZI.Deploy.MicrosoftEntra.Configuration;
using AuthZI.Tests.MicrosoftOrleans.MicrosoftEntra.MicrosoftEntraID.CSharp;
using Orleans;
using System;
using System.Text.Json;
using AuthZI.Tests.MicrosoftOrleans.MicrosoftEntra.Common.Initialization;
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
    var microsoftEntraIdCredentialsJson =
      Environment.GetEnvironmentVariable("microsoftEntraIdCredentials");

    if(string.IsNullOrWhiteSpace(microsoftEntraIdCredentialsJson))
    {
      microsoftEntraIdCredentialsJson = Literals.microsoftEntraCredentialsJson;
    }

    var credentials = JsonSerializer.Deserialize<MicrosoftEntraCredentials>(microsoftEntraIdCredentialsJson)!;
    Credentials = credentials;
    AccessTokenRetriever = new AccessTokenRetriever(false);
  }
}