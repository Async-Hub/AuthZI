using Microsoft.Identity.Client;
using AuthZI.Deploy.MicrosoftEntra.Configuration;
using System.Text.Json;
using static AuthZI.Deploy.MicrosoftEntra.Configuration.Credentials;

namespace AuthZI.Deploy.MicrosoftEntra.RefreshTokenTool.CSharp;

static class Program
{
  private const string EntraIdTarget = "entra-id";
  private const string EntraExternalIdTarget = "entra-external-id";

  static async Task Main(string[] args)
  {
    var targets = args.Length == 0
      ? [EntraExternalIdTarget]
      : args.Select(arg => arg.Trim().ToLowerInvariant()).ToArray();

    if (targets.Contains(EntraIdTarget) || targets.Contains("all"))
    {
      await WriteRefreshTokenStore(
        "MicrosoftEntraID1",
        GetMicrosoftEntraIdRefreshTokenRequests(),
        BuildMicrosoftEntraIdApp);
    }

    if (targets.Contains(EntraExternalIdTarget) || targets.Contains("external-id") || targets.Contains("all"))
    {
      await WriteRefreshTokenStore(
        "MicrosoftEntraExternalID1",
        GetMicrosoftEntraExternalIdRefreshTokenRequests(),
        BuildMicrosoftEntraExternalIdApp);
    }
  }

  private static IEnumerable<RefreshTokenRequest> GetMicrosoftEntraIdRefreshTokenRequests() =>
  [
    new(
      MicrosoftEntraID1.DirectoryId,
      MicrosoftEntraID1.WebClient1.Id,
      MicrosoftEntraID1.AdeleV.Name,
      MicrosoftEntraID1.AdeleV.Password,
      MicrosoftEntraID1.WebClient1.AllowedScopes),

    new(
      MicrosoftEntraID1.DirectoryId,
      MicrosoftEntraID1.WebClient2.Id,
      MicrosoftEntraID1.AdeleV.Name,
      MicrosoftEntraID1.AdeleV.Password,
      MicrosoftEntraID1.WebClient2.AllowedScopes),

    new(
      MicrosoftEntraID1.DirectoryId,
      MicrosoftEntraID1.WebClient1.Id,
      MicrosoftEntraID1.AlexW.Name,
      MicrosoftEntraID1.AlexW.Password,
      MicrosoftEntraID1.WebClient1.AllowedScopes),

    new(
      MicrosoftEntraID1.DirectoryId,
      MicrosoftEntraID1.WebClient2.Id,
      MicrosoftEntraID1.AlexW.Name,
      MicrosoftEntraID1.AlexW.Password,
      MicrosoftEntraID1.WebClient2.AllowedScopes)
  ];

  private static IEnumerable<RefreshTokenRequest> GetMicrosoftEntraExternalIdRefreshTokenRequests() =>
  [
    //new(
    //  MicrosoftEntraExternalID1.DirectoryId,
    //  MicrosoftEntraExternalID1.WebClient1.Id,
    //  MicrosoftEntraExternalID1.AdeleV.Name,
    //  MicrosoftEntraExternalID1.AdeleV.Password,
    //  MicrosoftEntraExternalID1.WebClient1.AllowedScopes),

    new(
      MicrosoftEntraExternalID1.DirectoryId,
      MicrosoftEntraExternalID1.WebClient2.Id,
      MicrosoftEntraExternalID1.AdeleV.Name,
      MicrosoftEntraExternalID1.AdeleV.Password,
      MicrosoftEntraExternalID1.WebClient2.AllowedScopes),

    //new(
      //MicrosoftEntraExternalID1.DirectoryId,
      //MicrosoftEntraExternalID1.WebClient1.Id,
      //MicrosoftEntraExternalID1.AlexW.Name,
      //MicrosoftEntraExternalID1.AlexW.Password,
      //MicrosoftEntraExternalID1.WebClient1.AllowedScopes),

    new(
      MicrosoftEntraExternalID1.DirectoryId,
      MicrosoftEntraExternalID1.WebClient2.Id,
      MicrosoftEntraExternalID1.AlexW.Name,
      MicrosoftEntraExternalID1.AlexW.Password,
      MicrosoftEntraExternalID1.WebClient2.AllowedScopes)
  ];

  private static async Task WriteRefreshTokenStore(
    string storeName,
    IEnumerable<RefreshTokenRequest> requests,
    Func<string, string, IPublicClientApplication> buildPublicClientApplication)
  {
    var refreshTokens = new List<MicrosoftEntraRefreshToken>();

    foreach (var request in requests)
    {
      var requestedScopes = request.Scopes.ToArray();

      if (requestedScopes.Length == 0)
      {
        Console.WriteLine(
          $"Skipping refresh token for {request.UserName} on client {request.ClientId}; no scopes are configured.");

        continue;
      }

      refreshTokens.Add(await AcquireRefreshToken(request, requestedScopes, buildPublicClientApplication));
    }

    var refreshTokenStore = new MicrosoftEntraRefreshTokenStore
    {
      Tokens = refreshTokens.ToArray()
    };

    var refreshTokenStoreJson = JsonSerializer.Serialize(
      refreshTokenStore, new JsonSerializerOptions { WriteIndented = true });

    Console.WriteLine($"\n=== {storeName} REFRESH TOKEN STORE JSON ===");
    Console.WriteLine(refreshTokenStoreJson);
    Console.WriteLine("================================================\n");
  }

  private static IPublicClientApplication BuildMicrosoftEntraIdApp(string tenantId, string clientId) =>
    PublicClientApplicationBuilder.Create(clientId)
      .WithAuthority(AzureCloudInstance.AzurePublic, tenantId)
      .Build();

  private static IPublicClientApplication BuildMicrosoftEntraExternalIdApp(string tenantId, string clientId) =>
    PublicClientApplicationBuilder.Create(clientId)
      .WithAuthority($"https://{tenantId}.ciamlogin.com/{tenantId}")
      .Build();

  private static async Task<MicrosoftEntraRefreshToken> AcquireRefreshToken(
    RefreshTokenRequest request,
    string[] requestedScopes,
    Func<string, string, IPublicClientApplication> buildPublicClientApplication)
  {
    Console.WriteLine(
      $"Acquiring refresh token for {request.UserName} with password {request.Password} on client {request.ClientId}.");

    var app = buildPublicClientApplication(request.TenantId, request.ClientId);

    string rawRefreshToken = string.Empty;

    app.UserTokenCache.SetAfterAccess(args =>
    {
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

    Console.WriteLine("Initializing Device Code Flow...");

    var result = await app.AcquireTokenWithDeviceCode(requestedScopes, deviceCodeResult =>
    {
      Console.WriteLine(deviceCodeResult.Message);
      return Task.CompletedTask;
    }).ExecuteAsync();

    if (!string.IsNullOrEmpty(rawRefreshToken))
    {
      Console.WriteLine($"Captured refresh token for {result.Account.Username} on client {request.ClientId}.");

      return new MicrosoftEntraRefreshToken
      {
        DirectoryId = request.TenantId,
        ClientId = request.ClientId,
        UserName = request.UserName,
        RefreshToken = rawRefreshToken,
        Scopes = requestedScopes,
        CreatedAtUtc = DateTimeOffset.UtcNow
      };
    }

    throw new InvalidOperationException("Authorization finished but cache processing failed.");
  }

  private sealed record RefreshTokenRequest(
    string TenantId,
    string ClientId,
    string UserName,
    string Password,
    IEnumerable<string> Scopes);
}
