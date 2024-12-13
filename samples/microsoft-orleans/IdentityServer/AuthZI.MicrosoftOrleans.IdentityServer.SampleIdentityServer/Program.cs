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

app.Run();