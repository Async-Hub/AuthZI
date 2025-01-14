[![Build Status](https://dev.azure.com/async-hub/AuthZI/_apis/build/status/AuthZI-CI?branchName=master)](https://dev.azure.com/async-hub/AuthZI/_build/latest?definitionId=1&branchName=master)
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=Async-Hub_AuthZI&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=Async-Hub_AuthZI)
[![Lines of Code](https://sonarcloud.io/api/project_badges/measure?project=Async-Hub_AuthZI&metric=ncloc)](https://sonarcloud.io/summary/new_code?id=Async-Hub_AuthZI)
[![Security Rating](https://sonarcloud.io/api/project_badges/measure?project=Async-Hub_AuthZI&metric=security_rating)](https://sonarcloud.io/summary/new_code?id=Async-Hub_AuthZI)
[![Reliability Rating](https://sonarcloud.io/api/project_badges/measure?project=Async-Hub_AuthZI&metric=reliability_rating)](https://sonarcloud.io/summary/new_code?id=Async-Hub_AuthZI)
[![Technical Debt](https://sonarcloud.io/api/project_badges/measure?project=Async-Hub_AuthZI&metric=sqale_index)](https://sonarcloud.io/summary/new_code?id=Async-Hub_AuthZI)
# **Auth**ori**z**ation **I**nteroperability – AuthZI for .NET

## Do you like authorization philosophy implemented in **Microsoft ASP.NET**? **AuthZI** allows to use the same ability in Microsoft Orleans, Azure Functions with Microsoft Entra and Duende IdentityServer.

## AuthZI project is open-source set of libraries for authorization interoperability for .NET 8 and above by using OAuth 2.0.

## Fore more information please visite the [project documentation site](https://authzi.asynchub.org/).

![AuthZI for Microsoft Orleans](docs-builder/documents/microsoft-orleans/authzi-for-microsoft-orleans.svg)

## Getting started
Below is a simple example of using Duende IdentityServer with Microsoft Orleans 9. For more details, [please visit the repository](https://github.com/Async-Hub/AuthZI/tree/master/samples/microsoft-orleans/IdentityServer) link.

### IdentityServer

```csharp
using AuthZI.MicrosoftOrleans.IdentityServer.SampleIdentityServer;

Console.Title = "IdentityServer";

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthorization();

builder.Services.AddIdentityServer()
  .AddDeveloperSigningCredential()
  .AddInMemoryApiScopes(IdentityServerConfig.GetApiScopes())
  .AddInMemoryApiResources(IdentityServerConfig.GetApiResources())
  .AddInMemoryIdentityResources(IdentityServerConfig.GetIdentityResources())
  .AddInMemoryClients(IdentityServerConfig.GetClients())
  .AddTestUsers(IdentityServerConfig.GetUsers());

var app = builder.Build();

app.UseRouting();

app.UseIdentityServer();
app.UseAuthorization();

app.MapGet("/", () => "IdentityServer is running...");

await app.RunAsync();
```

### Microsoft Orleans 9.*

```csharp
using AuthZI.MicrosoftOrleans.Authorization;
using AuthZI.Security.Authorization;

namespace AuthZI.MicrosoftOrleans.IdentityServer.SampleApiAndSiloHost;

public interface IProtectedGrain : IGrainWithStringKey
{
  [Authorize(Roles = "Admin")]
  Task<string> TakeSecret();
}

public class ProtectedGrain(SecureGrainContext secureGrainContext) : 
  SecureGrain(secureGrainContext), IProtectedGrain
{
  public Task<string> TakeSecret()
  {
    return Task.FromResult("Success! You see the data which is available only for Admin role.");
  }
}

using AuthZI.Identity.Duende.IdentityServer;
using AuthZI.MicrosoftOrleans.Authorization;
using AuthZI.MicrosoftOrleans.Duende.IdentityServer;
using AuthZI.MicrosoftOrleans.IdentityServer.SampleApiAndSiloHost;
using AuthZI.Security;

Console.Title = @"Api and SiloHost";

var builder = WebApplication.CreateBuilder(args);

var identityServerConfig = new IdentityServerConfig("https://localhost:7171",
  "Cluster", @"@3x3g*RLez$TNU!_7!QW", "Cluster");

builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient<IAccessTokenProvider, AccessTokenProvider>();

builder.Host.UseOrleans((context, siloBuilder) =>
  {
    siloBuilder.UseLocalhostClustering()
      .ConfigureServices(services =>
      {
        services.AddOrleansAuthorization(identityServerConfig, config => { },
          new AuthorizationConfiguration(true));
      });
  })
  .UseConsoleLifetime()
  .ConfigureLogging(loggingBuilder =>
  {
    loggingBuilder.AddConsole();
  });

var app = builder.Build();

app.MapGet("/", () => "Running...");

app.MapGet("/protected-grain",
  static async (IGrainFactory grains, HttpRequest request) =>
  {
    var protectedGrain =
      grains.GetGrain<IProtectedGrain>(nameof(IProtectedGrain));

    try
    {
      var secret = await protectedGrain.TakeSecret();
      return Results.Ok(secret);
    }
    catch (AuthorizationException ex)
    {
      Console.WriteLine(ex.Message);
      return Results.Unauthorized();
    }
  });

await app.RunAsync();
```

### Sample Client
```csharp
using AuthZI.MicrosoftOrleans.IdentityServer.SampleWebClient;
using IdentityModel.Client;

Console.Title = "WebClient";

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

Console.WriteLine("Please press 's' to start.");
Console.ReadLine();

var identityServerUrl = "https://localhost:7171";
var accessToken = await AccessTokenRetriever.RetrieveToken(identityServerUrl);
Console.WriteLine($"AccessToken: {accessToken}");

var apiUrl = "https://localhost:7116";
var httpClient = new HttpClient() { BaseAddress = new Uri(apiUrl) };
httpClient.SetBearerToken(accessToken);

var response = await httpClient.GetAsync($"/protected-grain");
if (!response.IsSuccessStatusCode)
{
  Console.WriteLine(response.StatusCode);
  Console.WriteLine(response.Content.ToString());
}
else
{
  var content = await response.Content.ReadAsStringAsync();
  Console.WriteLine(content);
}

app.MapGet("/", () => "WebClient is running...");

await app.RunAsync();
```

## Please [see sample](https://github.com/Async-Hub/AuthZI-Samples) solutions for more details.