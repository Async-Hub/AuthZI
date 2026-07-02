using AuthZI.Deploy.MicrosoftEntra.Configuration;
using AuthZI.Identity.MicrosoftEntra;
using AuthZI.Tests.MicrosoftOrleans.MicrosoftEntra.MicrosoftEntraID.Common.Initialization;
using Microsoft.Identity.Client;
using Orleans;
using Xunit;

[assembly: ApplicationPart("AuthZI.Tests.MicrosoftOrleans.Grains")]
[assembly: AssemblyFixture(
  typeof(AuthZI.Tests.MicrosoftOrleans.MicrosoftEntra.MicrosoftEntraExternalID.CSharp.Initialization.ExternalIdMainTestFixture))]
[assembly: CollectionBehavior(DisableTestParallelization = true)]

namespace AuthZI.Tests.MicrosoftOrleans.MicrosoftEntra.MicrosoftEntraExternalID.CSharp.Initialization;

public class ExternalIdMainTestFixture : MainTestFixture
{
  public ExternalIdMainTestFixture()
    : base(
      "microsoftEntraExternalIdCredentials",
      Literals.microsoftEntraExternalIDCredentialsJson,
      "microsoftEntraExternalIdRefreshTokens",
      "microsoftEntraExternalIdRefreshTokensPath",
      RefreshTokenStore.MicrosoftEntraExternalID1)
  {
  }

  protected override MicrosoftEntraApp CreateMicrosoftEntraApp(
    MicrosoftEntraCredentials credentials,
    Client client) =>
    new MicrosoftEntraExternalIDApp(
      credentials.DirectoryId,
      client.Id,
      client.Secret,
      client.AllowedScopes,
      AadAuthorityAudience.AzureAdMyOrg);

  protected override string GetTokenByUserNameAndPassword(
    MicrosoftEntraApp entraIdApp,
    string userName,
    string password) =>
    AccessTokenRetriever.GetTokenByUserNameAndPasswordForEntraExternalIdTenant(entraIdApp, userName, password);

  protected override string GetTokenByRefreshToken(
    MicrosoftEntraApp entraIdApp,
    string userName,
    MicrosoftEntraRefreshTokenStore refreshTokenStore) =>
    AccessTokenRetriever.GetTokenByRefreshTokenForEntraExternalIdTenant(entraIdApp, userName, refreshTokenStore);
}
