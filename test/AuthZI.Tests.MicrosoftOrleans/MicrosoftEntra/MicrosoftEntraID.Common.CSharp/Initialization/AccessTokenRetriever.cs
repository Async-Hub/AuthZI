using AuthZI.Deploy.MicrosoftEntra.Configuration;
using AuthZI.Identity.MicrosoftEntra;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace AuthZI.Tests.MicrosoftOrleans.MicrosoftEntra.MicrosoftEntraID.Common.Initialization;

public sealed class AccessTokenRetriever : IAccessTokenRetriever
{
  private readonly bool _isExternalId;
  private readonly MicrosoftEntraRefreshTokenStore _refreshTokenStore;

  public AccessTokenRetriever(bool isExternalId)
  {
    _isExternalId = isExternalId;
    string refreshTokensEnvironmentVariableName = isExternalId
      ? "MICROSOFT_ENTRA_EXTERNAL_ID_REFRESH_TOKENS_STORE"
      : "MICROSOFT_ENTRA_ID_REFRESH_TOKENS_STORE";

    string refreshTokenStoreJson = Environment.GetEnvironmentVariable(refreshTokensEnvironmentVariableName);

    if (string.IsNullOrWhiteSpace(refreshTokenStoreJson))
    {
      if (!isExternalId)
      {
        refreshTokenStoreJson = RefreshTokenStore.MicrosoftEntraID1;
      }
      else
      {
        refreshTokenStoreJson = RefreshTokenStore.MicrosoftEntraExternalID1;
      }
    }

    _refreshTokenStore = LoadRefreshTokenStore(refreshTokenStoreJson);
  }

  public Task<string> GetAccessTokenForUserAsync(string appName, string userName)
  {
    var entraIdApp = GetMicrosoftEntraApp(appName);

    if (!TestData.UserPasswords.TryGetValue(userName, out var password))
    {
      throw new InvalidOperationException($"Password for test user '{userName}' is not configured.");
    }

    if(_isExternalId)
    {
      var accessToken = GetTokenByRefreshTokenForEntraExternalIdTenant(entraIdApp, userName, _refreshTokenStore);
      return Task.FromResult(accessToken);
    }

    return Task.FromResult(GetTokenByRefreshTokenForEntraIdTenant(entraIdApp, userName, _refreshTokenStore));
  }

  private static MicrosoftEntraApp GetMicrosoftEntraApp(string appName)
  {
    if (string.Equals(appName, nameof(TestData.WebClient1), StringComparison.OrdinalIgnoreCase))
    {
      return TestData.WebClient1;
    }

    if (string.Equals(appName, nameof(TestData.WebClient2), StringComparison.OrdinalIgnoreCase))
    {
      return TestData.WebClient2;
    }

    throw new InvalidOperationException($"Microsoft Entra app '{appName}' is not configured.");
  }

  private MicrosoftEntraRefreshTokenStore LoadRefreshTokenStore(string refreshTokenStoreJson)
  {
    if (string.IsNullOrWhiteSpace(refreshTokenStoreJson))
    {
      throw new InvalidOperationException("Refresh token store JSON is null or empty.");
    }

    var refreshTokenStore = JsonSerializer.Deserialize<MicrosoftEntraRefreshTokenStore>(refreshTokenStoreJson);

    if (refreshTokenStore?.Tokens is not { Length: > 0 })
    {
      throw new InvalidOperationException("Refresh token store does not contain any tokens.");
    }

    return refreshTokenStore;
  }
  
  public string GetTokenByUserNameAndPasswordForEntraIdTenant(
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
      .AcquireTokenByUsernamePassword(entraIdApp.AllowedScopes, userName, password)
      .ExecuteAsync()
      .Result;

    return result.AccessToken;
  }
  
  public string GetTokenByUserNameAndPasswordForEntraExternalIdTenant(
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
      .AcquireTokenByUsernamePassword(entraExternalIdApp.AllowedScopes, userName, password)
      .ExecuteAsync()
      .Result;

    return result.AccessToken;
  }

  private string GetTokenByRefreshTokenForEntraIdTenant(
    MicrosoftEntraApp entraIdApp,
    string userName,
    MicrosoftEntraRefreshTokenStore refreshTokenStore)
  {
    var tokenEndpoint = $"https://login.microsoftonline.com/{entraIdApp.DirectoryId}/oauth2/v2.0/token";

    return GetTokenByRefreshToken(entraIdApp, userName, refreshTokenStore, tokenEndpoint);
  }

  private string GetTokenByRefreshTokenForEntraExternalIdTenant(
    MicrosoftEntraApp entraExternalIdApp,
    string userName,
    MicrosoftEntraRefreshTokenStore refreshTokenStore)
  {
    var tokenEndpoint =
      $"https://{entraExternalIdApp.DirectoryId}.ciamlogin.com/{entraExternalIdApp.DirectoryId}/oauth2/v2.0/token";

    return GetTokenByRefreshToken(entraExternalIdApp, userName, refreshTokenStore, tokenEndpoint);
  }

  private static string GetTokenByRefreshToken(
    MicrosoftEntraApp entraIdApp,
    string userName,
    MicrosoftEntraRefreshTokenStore refreshTokenStore,
    string tokenEndpoint)
  {
    var refreshToken = FindRefreshToken(entraIdApp, userName, refreshTokenStore);

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