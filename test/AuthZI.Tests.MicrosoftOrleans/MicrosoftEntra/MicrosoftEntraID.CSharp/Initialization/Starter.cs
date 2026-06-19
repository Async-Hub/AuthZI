using System;
using System.Text.Json;
using AuthZI.Deploy.MicrosoftEntra.Configuration;
using AuthZI.Identity.MicrosoftEntra;
using AuthZI.MicrosoftEntra;
using AuthZI.MicrosoftOrleans.Authorization;
using AuthZI.MicrosoftOrleans.MicrosoftEntra;
using AuthZI.Security;
using AuthZI.Security.Authorization;
using AuthZI.Tests.MicrosoftOrleans.Grains;
using AuthZI.Tests.MicrosoftOrleans.MicrosoftEntra;
using AuthZI.Tests.MicrosoftOrleans.MicrosoftEntra.MicrosoftEntraID.Common;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Client;
using Orleans;
using Xunit;
using static RootConfiguration;

[assembly: ApplicationPart("AuthZI.Tests.MicrosoftOrleans.Grains")]
[assembly: AssemblyFixture(typeof(AuthZI.Tests.MicrosoftOrleans.MicrosoftEntra.MicrosoftEntraID.CSharp.Starter))]
[assembly: CollectionBehavior(DisableTestParallelization = true)]

namespace AuthZI.Tests.MicrosoftOrleans.MicrosoftEntra.MicrosoftEntraID.CSharp;

public class Starter
{
  public Starter()
  {
    var microsoftEntraIdCredentialsJson = Environment.GetEnvironmentVariable("microsoftEntraIdCredentials");

    if (string.IsNullOrWhiteSpace(microsoftEntraIdCredentialsJson))
    {
      microsoftEntraIdCredentialsJson = Literals.microsoftEntraCredentialsJson;
    }

    var credentials = JsonSerializer.Deserialize<MicrosoftEntraCredentials>(microsoftEntraIdCredentialsJson);

    var web1ClientApp = new MicrosoftEntraIDApp(
      credentials.DirectoryId,
      credentials.WebClient1.Id,
      credentials.WebClient1.Secret,
      credentials.WebClient1.AllowedScopes,
      AadAuthorityAudience.AzureAdMyOrg);

    var web2ClientApp = new MicrosoftEntraIDApp(
      credentials.DirectoryId,
      credentials.WebClient2.Id,
      credentials.WebClient2.Secret,
      credentials.WebClient2.AllowedScopes,
      AadAuthorityAudience.AzureAdMyOrg);

    TestData.UserWithScopeAdeleV = new[]
    {
      new object[] { credentials.AdeleV.Name, credentials.AdeleV.Password, new[] { "Api1", "Orleans" } }
    };

    TestData.UserWithScopeAlexW = new[]
    {
      new object[] { credentials.AlexW.Name, credentials.AlexW.Password, new[] { "Api1", "Orleans" } }
    };

    TestData.Users = new[]
    {
      new object[] { credentials.AdeleV.Name, credentials.AdeleV.Password }
    };

    TestData.Web1ClientApp = web1ClientApp;
    TestData.Web2ClientApp = web2ClientApp;
    TestData.GetAccessTokenForUserOnMicrosoftEntraAppAsync =
      AccessTokenRetriever.getTokenByUserNameAndPasswordForEntraIDTenant;

    var siloHost = SiloHostBuilder.Build(ConfigureSiloHost);
    siloHost.StartAsync().Wait();

    var siloClientHost = SiloClientBuilder.Build(ConfigureSiloClient);
    siloClientHost.StartAsync().Wait();
    TestData.IClusterClient = siloClientHost.Services.GetService<IClusterClient>();
  }

  private static void ConfigureSiloHost(IServiceCollection services)
  {
    services.AddOrleansAuthorization(
      TestData.Web1ClientApp,
      config => config.ConfigureAuthorizationOptions = new Action<AuthorizationOptions>(AuthorizationConfig.ConfigureOptions),
      new AuthorizationConfiguration(false));

    AuthorizationConfig.ConfigureServices(services);
  }

  private static void ConfigureSiloClient(IServiceCollection services)
  {
    AuthorizationConfig.ConfigureServices(services);
    services.AddSingleton<IAccessTokenProvider>(_ => accessTokenProvider);
    services.AddOrleansClientAuthorization(
      TestData.Web1ClientApp,
      config => config.ConfigureAuthorizationOptions = new Action<AuthorizationOptions>(AuthorizationConfig.ConfigureOptions));
  }
}
