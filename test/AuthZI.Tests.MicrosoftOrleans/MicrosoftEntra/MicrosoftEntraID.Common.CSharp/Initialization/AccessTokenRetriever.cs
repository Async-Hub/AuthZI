using AuthZI.Identity.MicrosoftEntra;
using Microsoft.Identity.Client;

namespace AuthZI.Tests.MicrosoftOrleans.MicrosoftEntra;

public static class AccessTokenRetriever
{
  public static string getTokenByUserNameAndPasswordForEntraIDTenant(
    MicrosoftEntraApp entraIDApp,
    string userName,
    string password)
  {
    var appConfig = new PublicClientApplicationOptions
    {
      TenantId = entraIDApp.DirectoryId,
      ClientId = entraIDApp.ClientId,
      AadAuthorityAudience = AadAuthorityAudience.AzureAdMyOrg,
      AzureCloudInstance = AzureCloudInstance.AzurePublic
    };

    var app = PublicClientApplicationBuilder.CreateWithApplicationOptions(appConfig).Build();

    var result = app
      .AcquireTokenByUsernamePassword(entraIDApp.AllowedScopes, userName, password.ToString())
      .ExecuteAsync()
      .Result;

    return result.AccessToken;
  }

  public static string getTokenByUserNameAndPasswordForEntraExternalIDTenant(
    MicrosoftEntraApp entraExternalIDApp,
    string userName,
    string password)
  {
    var appConfig = new PublicClientApplicationOptions
    {
      TenantId = entraExternalIDApp.DirectoryId,
      ClientId = entraExternalIDApp.ClientId,
      Instance = $"https://{entraExternalIDApp.DirectoryId}.ciamlogin.com/",
      AadAuthorityAudience = AadAuthorityAudience.AzureAdMyOrg
    };

    var app = PublicClientApplicationBuilder.CreateWithApplicationOptions(appConfig).Build();

    var result = app
      .AcquireTokenByUsernamePassword(entraExternalIDApp.AllowedScopes, userName, password.ToString())
      .ExecuteAsync()
      .Result;

    return result.AccessToken;
  }
}
