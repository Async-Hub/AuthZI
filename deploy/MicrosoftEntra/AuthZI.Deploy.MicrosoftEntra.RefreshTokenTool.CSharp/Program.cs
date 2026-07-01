using Microsoft.Identity.Client;
using AuthZI.Deploy.MicrosoftEntra.Configuration;
using System.Text.Json;
using static AuthZI.Deploy.MicrosoftEntra.Configuration.Credentials;

namespace AuthZI.Deploy.MicrosoftEntra.RefreshTokenTool.CSharp;

static class Program
{
  static async Task Main(string[] args)
  {
    // Refresh tokens for MicrosoftEntraID1.
    // Capture refresh tokens for users in the same tenant across both client app registrations.
    var refreshTokens = new[]
    {
      await AcquireRefreshToken(MicrosoftEntraID1.DirectoryId, MicrosoftEntraID1.WebClient1.Id,
        MicrosoftEntraID1.AdeleV.Name, MicrosoftEntraExternalID1.AdeleV.Password, MicrosoftEntraID1.WebClient1.AllowedScopes),

      await AcquireRefreshToken(MicrosoftEntraID1.DirectoryId, MicrosoftEntraID1.WebClient2.Id,
        MicrosoftEntraID1.AdeleV.Name, MicrosoftEntraExternalID1.AdeleV.Password, MicrosoftEntraID1.WebClient2.AllowedScopes),

      await AcquireRefreshToken(MicrosoftEntraID1.DirectoryId, MicrosoftEntraID1.WebClient1.Id,
        MicrosoftEntraID1.AlexW.Name, MicrosoftEntraExternalID1.AlexW.Password, MicrosoftEntraID1.WebClient1.AllowedScopes),

      await AcquireRefreshToken(MicrosoftEntraID1.DirectoryId, MicrosoftEntraID1.WebClient2.Id,
        MicrosoftEntraID1.AlexW.Name, MicrosoftEntraExternalID1.AlexW.Password, MicrosoftEntraID1.WebClient2.AllowedScopes)
    };

    var refreshTokenStore = new MicrosoftEntraRefreshTokenStore
    {
      Tokens = refreshTokens
    };

    var refreshTokenStoreJson = JsonSerializer.Serialize(
      refreshTokenStore, new JsonSerializerOptions { WriteIndented = true });

    Console.WriteLine("\n=== REFRESH TOKEN STORE JSON ===");
    Console.WriteLine(refreshTokenStoreJson);
    Console.WriteLine("================================\n");
  }

  private static async Task<MicrosoftEntraRefreshToken> AcquireRefreshToken(
    string tenantId,
    string clientId,
    string userName,
    string password,
    IEnumerable<string> scopes)
  {
    var requestedScopes = scopes.ToArray();

    Console.WriteLine($"Acquiring refresh token for {userName} {password} on client {clientId}.");

    var app = PublicClientApplicationBuilder.Create(clientId)
      .WithAuthority(AzureCloudInstance.AzurePublic, tenantId)
      .Build();

    string rawRefreshToken = string.Empty;

    // FIX: Changed from SetBeforeAccess to SetAfterAccess
    app.UserTokenCache.SetAfterAccess(args =>
    {
      // Only process if the cache state has changed (tokens were successfully fetched)
      if (!args.HasStateChanged)
      {
        return;
      }

      byte[] cacheBytes = args.TokenCache.SerializeMsalV3();
      if (cacheBytes is not { Length: > 0 })
      {
        return;
      }

      using var jsonDoc = JsonDocument.Parse(cacheBytes);

      // Dig into the schema dictionary to grab individual refresh tokens
      if (jsonDoc.RootElement.TryGetProperty("RefreshToken", out JsonElement rtNode))
      {
        foreach (var property in rtNode.EnumerateObject())
        {
          if (property.Value.TryGetProperty("secret", out JsonElement secretNode))
          {
            rawRefreshToken = secretNode.GetString() ?? string.Empty;
          }
        }
      }
    });

    // Prompt the flow
    Console.WriteLine("Initializing Device Code Flow...");

    var result = await app.AcquireTokenWithDeviceCode(requestedScopes, deviceCodeResult =>
    {
      Console.WriteLine(deviceCodeResult.Message);
      return Task.CompletedTask;
    }).ExecuteAsync();

    // Output results to the terminal window
    if (!string.IsNullOrEmpty(rawRefreshToken))
    {
      Console.WriteLine($"Captured refresh token for {result.Account.Username} on client {clientId}.");

      return new MicrosoftEntraRefreshToken
      {
        DirectoryId = tenantId,
        ClientId = clientId,
        UserName = userName,
        RefreshToken = rawRefreshToken,
        Scopes = requestedScopes,
        CreatedAtUtc = DateTimeOffset.UtcNow
      };
    }

    throw new InvalidOperationException("Authorization finished but cache processing failed.");
  }
}