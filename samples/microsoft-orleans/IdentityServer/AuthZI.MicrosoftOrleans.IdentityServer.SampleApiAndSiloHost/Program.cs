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