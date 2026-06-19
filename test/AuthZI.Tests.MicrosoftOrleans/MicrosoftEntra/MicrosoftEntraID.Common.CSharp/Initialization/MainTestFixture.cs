using AuthZI.Deploy.MicrosoftEntra.Configuration;
using AuthZI.Identity.MicrosoftEntra;
using AuthZI.MicrosoftOrleans.Authorization;
using AuthZI.MicrosoftOrleans.MicrosoftEntra;
using AuthZI.Security;
using AuthZI.Security.Authorization;
using AuthZI.Tests.MicrosoftOrleans.Grains;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Identity.Client;
using Orleans;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace AuthZI.Tests.MicrosoftOrleans.MicrosoftEntra.MicrosoftEntraID.Common.Initialization;

public class MainTestFixture : IAsyncLifetime
{
  private readonly ConfigurableAccessTokenProvider _accessTokenProvider = new();
  private readonly string _credentialsEnvironmentVariableName;
  private readonly string _fallbackCredentialsJson;

  private IHost _siloHost = null!;
  private IHost _siloClientHost = null!;

  public MainTestFixture()
    : this("microsoftEntraIdCredentials", Literals.microsoftEntraCredentialsJson)
  {
  }

  protected MainTestFixture(
    string credentialsEnvironmentVariableName,
    string fallbackCredentialsJson)
  {
    _credentialsEnvironmentVariableName = credentialsEnvironmentVariableName;
    _fallbackCredentialsJson = fallbackCredentialsJson;
  }

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
    var microsoftEntraIdCredentialsJson = 
      Environment.GetEnvironmentVariable(_credentialsEnvironmentVariableName);

    if (string.IsNullOrWhiteSpace(microsoftEntraIdCredentialsJson))
    {
      microsoftEntraIdCredentialsJson = _fallbackCredentialsJson;
    }

    var credentials = JsonSerializer.Deserialize<MicrosoftEntraCredentials>(microsoftEntraIdCredentialsJson)!;

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

    TestData.UserWithScopeAdeleV =
    [
      [credentials.AdeleV.Name, new[] { "Api1", "Orleans" }]
    ];

    TestData.UserWithScopeAlexW =
    [
      [credentials.AlexW.Name, new[] { "Api1", "Orleans" }]
    ];

    TestData.Users =
    [
      [credentials.AdeleV.Name]
    ];

    TestData.UserPasswords = new Dictionary<string, string>
    {
      [credentials.AdeleV.Name] = credentials.AdeleV.Password,
      [credentials.AlexW.Name] = credentials.AlexW.Password,
      [Credentials.AzureActiveDirectoryB2C1.AdeleV.Name] = Credentials.AzureActiveDirectoryB2C1.AdeleV.Password
    };

    TestData.Web1ClientApp = web1ClientApp;
    TestData.Web2ClientApp = web2ClientApp;
    TestData.GetAccessTokenForUserOnMicrosoftEntraAppAsync =
      AccessTokenRetriever.getTokenByUserNameAndPasswordForEntraIDTenant;
  }

  private static void ConfigureSiloHost(IServiceCollection services)
  {
    services.AddOrleansAuthorization(
      TestData.Web1ClientApp,
      config => config.ConfigureAuthorizationOptions = new Action<AuthorizationOptions>(AuthorizationConfig.ConfigureOptions),
      new AuthorizationConfiguration(false));

    AuthorizationConfig.ConfigureServices(services);
  }

  private void ConfigureSiloClient(IServiceCollection services)
  {
    AuthorizationConfig.ConfigureServices(services);
    services.AddSingleton<IAccessTokenProvider>(_ => _accessTokenProvider);
    services.AddOrleansClientAuthorization(
      TestData.Web1ClientApp,
      config => config.ConfigureAuthorizationOptions = new Action<AuthorizationOptions>(AuthorizationConfig.ConfigureOptions));
  }

  private sealed class ConfigurableAccessTokenProvider : IAccessTokenProvider
  {
    public string AccessToken { private get; set; } = string.Empty;

    public Task<string> RetrieveTokenAsync() => Task.FromResult(AccessToken);
  }
}
