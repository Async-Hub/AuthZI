using Microsoft.Identity.Client;
using System.Text.Json;
using static AuthZI.Deploy.MicrosoftEntra.Configuration.Credentials;

namespace AuthZI.Deploy.MicrosoftEntra.RefreshTokenTool.CSharp;

static class Program
{
  static async Task Main(string[] args)
  {
    // Refresh tokens for MicrosoftEntraID1.
    // Print refresh tokens for AdeleV,
    // who are both in the same tenant and have the same client app registration.
    await PrintRefreshToken(MicrosoftEntraID1.DirectoryId, MicrosoftEntraID1.WebClient1.Id,
      MicrosoftEntraID1.AdeleV.Name, MicrosoftEntraID1.AdeleV.Password);

    await PrintRefreshToken(MicrosoftEntraID1.DirectoryId, MicrosoftEntraID1.WebClient2.Id,
      MicrosoftEntraID1.AdeleV.Name, MicrosoftEntraID1.AdeleV.Password);

    await PrintRefreshToken(MicrosoftEntraID1.DirectoryId, MicrosoftEntraID1.WebClient1.Id,
      MicrosoftEntraID1.AlexW.Name, MicrosoftEntraID1.AlexW.Password);

    await PrintRefreshToken(MicrosoftEntraID1.DirectoryId, MicrosoftEntraID1.WebClient2.Id,
      MicrosoftEntraID1.AlexW.Name, MicrosoftEntraID1.AlexW.Password);
  }

  private static async Task PrintRefreshToken(string tenantId, string clientId,
    string userName, string userPassword)
  {
    Console.WriteLine($"{userName} {userPassword}");

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
            rawRefreshToken = secretNode.GetString();
          }
        }
      }
    });

    // Prompt the flow
    var scopes = new[] { "User.Read" };
    Console.WriteLine("Initializing Device Code Flow...");

    var result = await app.AcquireTokenWithDeviceCode(scopes, deviceCodeResult =>
    {
      Console.WriteLine(deviceCodeResult.Message);
      return Task.CompletedTask;
    }).ExecuteAsync();

    // Output results to the terminal window
    if (!string.IsNullOrEmpty(rawRefreshToken))
    {
      Console.WriteLine("\n=== SUCCESS: COPY YOUR RAW REFRESH TOKEN ===");
      Console.WriteLine(rawRefreshToken);
      Console.WriteLine("============================================\n");
    }
    else
    {
      Console.WriteLine("\n[Error] Authorization finished but cache processing failed.");
    }
  }
}