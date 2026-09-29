using AuthZI.Deploy.MicrosoftEntra.Configuration;
using AuthZI.Identity.MicrosoftEntra;
using AuthZI.Tests.MicrosoftOrleans.MicrosoftEntra.MicrosoftEntraExternalID.CSharp;
using AuthZI.Tests.MicrosoftOrleans.MicrosoftEntra.MicrosoftEntraID.Common.Initialization;
using Microsoft.Identity.Client;
using Orleans;
using System;
using System.Text.Json;
using Xunit;
using Xunit.Sdk;
using Xunit.v3;

[assembly: ApplicationPart("AuthZI.Tests.MicrosoftOrleans.Grains")]
[assembly: AssemblyFixture(typeof(ExternalIdMainTestFixture))]
[assembly: Parallelization(Mode = ParallelMode.None)]

namespace AuthZI.Tests.MicrosoftOrleans.MicrosoftEntra.MicrosoftEntraExternalID.CSharp;

public class ExternalIdMainTestFixture : MainTestFixture
{
  public ExternalIdMainTestFixture()
  {
    var microsoftEntraExternalIdCredentialsJson =
      Environment.GetEnvironmentVariable("microsoftEntraExternalIdCredentials");

    if (string.IsNullOrWhiteSpace(microsoftEntraExternalIdCredentialsJson))
    {
      microsoftEntraExternalIdCredentialsJson = Literals.microsoftEntraExternalIDCredentialsJson;
    }

    var credentials = JsonSerializer.Deserialize<MicrosoftEntraCredentials>(microsoftEntraExternalIdCredentialsJson)!;
    Credentials = credentials;
    AccessTokenRetriever = new AccessTokenRetriever(true);
  }

  protected override MicrosoftEntraApp CreateMicrosoftEntraApp(
    MicrosoftEntraCredentials credentials,
    Client client) =>
    new MicrosoftEntraExternalIDApp(
      credentials.DirectoryId,
      client.Id,
      client.Secret,
      client.AllowedScopes,
      AadAuthorityAudience.AzureAdMyOrg,
      credentials.Api1.Id);
}
