using AuthZI.Deploy.MicrosoftEntra.Configuration;
using AuthZI.Identity.MicrosoftEntra;
using AuthZI.MicrosoftOrleans.Authorization;
using AuthZI.MicrosoftOrleans.MicrosoftEntra;
using AuthZI.Security;
using AuthZI.Tests.MicrosoftOrleans.Grains;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Identity.Client;
using Orleans;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace AuthZI.Tests.MicrosoftOrleans.MicrosoftEntra.MicrosoftEntraID.Common.Initialization;

public class MainTestFixture : IAsyncLifetime
{
  private readonly AccessTokenProvider _accessTokenProvider = new();
  protected MicrosoftEntraCredentials Credentials;

  public IAccessTokenRetriever AccessTokenRetriever { get; protected set; }
  private IHost _siloHost = null!;
  private IHost _siloClientHost = null!;

  private IClusterClient ClusterClient { get; set; } = null!;

  public IClusterClient GetClusterClient(string accessToken)
  {
    _accessTokenProvider.AccessToken = accessToken;

    return ClusterClient;
  }

  public async ValueTask InitializeAsync()
  {
    ConfigureTestData();

    _siloHost = SiloHostBuilder.Build(ConfigureSiloHost);
    await _siloHost.StartAsync();

    _siloClientHost = SiloClientBuilder.Build(ConfigureSiloClient);
    await _siloClientHost.StartAsync();

    ClusterClient = _siloClientHost.Services.GetRequiredService<IClusterClient>();
    TestData.IClusterClient = ClusterClient;
  }

  public async ValueTask DisposeAsync()
  {
    if (_siloClientHost is not null)
    {
      await _siloClientHost.StopAsync();
      _siloClientHost.Dispose();
    }

    if (_siloHost is not null)
    {
      await _siloHost.StopAsync();
      _siloHost.Dispose();
    }
  }

  private void ConfigureTestData()
  {
    var web1ClientApp = CreateMicrosoftEntraApp(Credentials, Credentials.WebClient1);
    var web2ClientApp = CreateMicrosoftEntraApp(Credentials, Credentials.WebClient2);

    TestData.UserWithScopeAdeleV =
    [
      [Credentials.AdeleV.Name, new[] { "Api1", "Orleans" }]
    ];

    TestData.UserWithScopeAlexW =
    [
      [Credentials.AlexW.Name, new[] { "Api1", "Orleans" }]
    ];

    TestData.Users =
    [
      [Credentials.AdeleV.Name]
    ];

    TestData.UserPasswords = new Dictionary<string, string>
    {
      [Credentials.AdeleV.Name] = Credentials.AdeleV.Password,
      [Credentials.AlexW.Name] = Credentials.AlexW.Password,
      [Deploy.MicrosoftEntra.Configuration.Credentials.AzureActiveDirectoryB2C1.AdeleV.Name] = 
        Deploy.MicrosoftEntra.Configuration.Credentials.AzureActiveDirectoryB2C1.AdeleV.Password
    };

    TestData.WebClient1 = web1ClientApp;
    TestData.WebClient2 = web2ClientApp;
  }

  protected virtual MicrosoftEntraApp CreateMicrosoftEntraApp(
    MicrosoftEntraCredentials credentials,
    Client client) =>
    new MicrosoftEntraIDApp(
      credentials.DirectoryId,
      client.Id,
      client.Secret,
      client.AllowedScopes,
      AadAuthorityAudience.AzureAdMyOrg);

  private static void ConfigureSiloHost(IServiceCollection services)
  {
    services.AddOrleansAuthorization(
      TestData.WebClient1,
      config => config.ConfigureAuthorizationOptions = 
        AuthorizationConfig.ConfigureOptions,
      new AuthorizationConfiguration(false));

    AuthorizationConfig.ConfigureServices(services);
  }

  private void ConfigureSiloClient(IServiceCollection services)
  {
    AuthorizationConfig.ConfigureServices(services);
    services.AddSingleton<IAccessTokenProvider>(_ => _accessTokenProvider);
    services.AddOrleansClientAuthorization(
      TestData.WebClient1,
      config => config.ConfigureAuthorizationOptions = AuthorizationConfig.ConfigureOptions);
  }

  private sealed class AccessTokenProvider : IAccessTokenProvider
  {
    public string AccessToken { private get; set; } = string.Empty;

    public Task<string> RetrieveTokenAsync() => Task.FromResult(AccessToken);
  }
}
