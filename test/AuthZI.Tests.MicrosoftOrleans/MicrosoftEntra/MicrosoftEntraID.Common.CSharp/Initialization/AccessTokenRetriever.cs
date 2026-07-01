using AuthZI.Deploy.MicrosoftEntra.Configuration;
using AuthZI.Identity.MicrosoftEntra;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;

namespace AuthZI.Tests.MicrosoftOrleans.MicrosoftEntra.MicrosoftEntraID.Common.Initialization;

public static class AccessTokenRetriever
{
  public static string GetTokenByUserNameAndPasswordForEntraIdTenant(
    MicrosoftEntraApp entraIdApp,
    string userName,
    string password)
  {
    var appConfig = new PublicClientApplicationOptions
    {
      TenantId = entraIdApp.DirectoryId,
      ClientId = entraIdApp.ClientId,
      AadAuthorityAudience = AadAuthorityAudience.AzureAdMyOrg,
      AzureCloudInstance = AzureCloudInstance.AzurePublic
    };

    var app = PublicClientApplicationBuilder.CreateWithApplicationOptions(appConfig).Build();

    var result = app
      .AcquireTokenByUsernamePassword(entraIdApp.AllowedScopes, userName, password.ToString())
      .ExecuteAsync()
      .Result;

    return result.AccessToken;
  }

  public static string GetTokenByUserNameAndPasswordForEntraExternalIdTenant(
    MicrosoftEntraApp entraExternalIdApp,
    string userName,
    string password)
  {
    var appConfig = new PublicClientApplicationOptions
    {
      TenantId = entraExternalIdApp.DirectoryId,
      ClientId = entraExternalIdApp.ClientId,
      Instance = $"https://{entraExternalIdApp.DirectoryId}.ciamlogin.com/",
      AadAuthorityAudience = AadAuthorityAudience.AzureAdMyOrg
    };

    var app = PublicClientApplicationBuilder.CreateWithApplicationOptions(appConfig).Build();

    var result = app
      .AcquireTokenByUsernamePassword(entraExternalIdApp.AllowedScopes, userName, password.ToString())
      .ExecuteAsync()
      .Result;

    return result.AccessToken;
  }

  public static string GetTokenByRefreshTokenForEntraIdTenant(
    MicrosoftEntraApp entraIdApp,
    string userName,
    MicrosoftEntraRefreshTokenStore refreshTokenStore)
  {
    var refreshToken = FindRefreshToken(entraIdApp, userName, refreshTokenStore);

    var tokenEndpoint = $"https://login.microsoftonline.com/{entraIdApp.DirectoryId}/oauth2/v2.0/token";
    var tokenRequest = new Dictionary<string, string>
    {
      ["client_id"] = entraIdApp.ClientId,
      ["grant_type"] = "refresh_token",
      ["refresh_token"] = refreshToken.RefreshToken,
      ["scope"] = string.Join(" ", entraIdApp.AllowedScopes)
    };

    using var httpClient = new HttpClient();
    using var response = httpClient
      .PostAsync(tokenEndpoint, new FormUrlEncodedContent(tokenRequest))
      .Result;

    var responseJson = response.Content.ReadAsStringAsync().Result;

    if (!response.IsSuccessStatusCode)
    {
      throw new InvalidOperationException(
        $"Failed to acquire access token from refresh token. Status: {response.StatusCode}. Body: {responseJson}");
    }

    using var tokenResponse = JsonDocument.Parse(responseJson);

    return tokenResponse.RootElement.TryGetProperty("access_token", out var accessTokenNode)
      ? accessTokenNode.GetString() ?? string.Empty
      : throw new InvalidOperationException("Token endpoint response does not contain an access_token.");
  }

  private static MicrosoftEntraRefreshToken FindRefreshToken(
    MicrosoftEntraApp entraIdApp,
    string userName,
    MicrosoftEntraRefreshTokenStore refreshTokenStore)
  {
    var tokens = refreshTokenStore.Tokens ?? Array.Empty<MicrosoftEntraRefreshToken>();

    var refreshToken = tokens.FirstOrDefault(token =>
      string.Equals(token.DirectoryId, entraIdApp.DirectoryId, StringComparison.OrdinalIgnoreCase) &&
      string.Equals(token.ClientId, entraIdApp.ClientId, StringComparison.OrdinalIgnoreCase) &&
      string.Equals(token.UserName, userName, StringComparison.OrdinalIgnoreCase));

    return refreshToken ?? throw new InvalidOperationException(
      $"Refresh token for user '{userName}' on client '{entraIdApp.ClientId}' is not configured.");
  }
}
