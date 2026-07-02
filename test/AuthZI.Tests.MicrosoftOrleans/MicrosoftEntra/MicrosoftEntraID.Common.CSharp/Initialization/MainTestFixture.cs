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
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace AuthZI.Tests.MicrosoftOrleans.MicrosoftEntra.MicrosoftEntraID.Common.Initialization;

public class MainTestFixture : IAsyncLifetime
{
  private const string RefreshTokensEnvironmentVariableName = "microsoftEntraRefreshTokens";
  private const string RefreshTokensPathEnvironmentVariableName = "microsoftEntraRefreshTokensPath";

  private readonly ConfigurableAccessTokenProvider _accessTokenProvider = new();
  private readonly string _credentialsEnvironmentVariableName;
  private readonly string _fallbackRefreshTokenStoreJson;
  private readonly string _fallbackCredentialsJson;
  private readonly string _refreshTokensEnvironmentVariableName;
  private readonly string _refreshTokensPathEnvironmentVariableName;

  private IHost _siloHost = null!;
  private IHost _siloClientHost = null!;

  public MainTestFixture()
    : this(
      "microsoftEntraIdCredentials",
      Literals.microsoftEntraCredentialsJson,
      RefreshTokensEnvironmentVariableName,
      RefreshTokensPathEnvironmentVariableName,
      RefreshTokenStore.MicrosoftEntraID1)
  {
  }

  protected MainTestFixture(
    string credentialsEnvironmentVariableName,
    string fallbackCredentialsJson,
    string refreshTokensEnvironmentVariableName,
    string refreshTokensPathEnvironmentVariableName,
    string fallbackRefreshTokenStoreJson)
  {
    _credentialsEnvironmentVariableName = credentialsEnvironmentVariableName;
    _fallbackCredentialsJson = fallbackCredentialsJson;
    _refreshTokensEnvironmentVariableName = refreshTokensEnvironmentVariableName;
    _refreshTokensPathEnvironmentVariableName = refreshTokensPathEnvironmentVariableName;
    _fallbackRefreshTokenStoreJson = fallbackRefreshTokenStoreJson;
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

    var web1ClientApp = CreateMicrosoftEntraApp(credentials, credentials.WebClient1);
    var web2ClientApp = CreateMicrosoftEntraApp(credentials, credentials.WebClient2);

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

    var refreshTokenStore = LoadRefreshTokenStore();

    TestData.GetAccessTokenForUserOnMicrosoftEntraAppAsync = refreshTokenStore is null
      ? GetTokenByUserNameAndPassword
      : (entraIdApp, userName, _) =>
        GetTokenByRefreshToken(entraIdApp, userName, refreshTokenStore);
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

  protected virtual string GetTokenByUserNameAndPassword(
    MicrosoftEntraApp entraIdApp,
    string userName,
    string password) =>
    AccessTokenRetriever.GetTokenByUserNameAndPasswordForEntraIdTenant(entraIdApp, userName, password);

  protected virtual string GetTokenByRefreshToken(
    MicrosoftEntraApp entraIdApp,
    string userName,
    MicrosoftEntraRefreshTokenStore refreshTokenStore) =>
    AccessTokenRetriever.GetTokenByRefreshTokenForEntraIdTenant(entraIdApp, userName, refreshTokenStore);

  private MicrosoftEntraRefreshTokenStore LoadRefreshTokenStore()
  {
    var refreshTokenStoreJson = Environment.GetEnvironmentVariable(_refreshTokensEnvironmentVariableName);

    if (string.IsNullOrWhiteSpace(refreshTokenStoreJson))
    {
      var refreshTokenStorePath = Environment.GetEnvironmentVariable(_refreshTokensPathEnvironmentVariableName);

      if (!string.IsNullOrWhiteSpace(refreshTokenStorePath))
      {
        refreshTokenStoreJson = File.ReadAllText(refreshTokenStorePath);
      }
    }

    if (string.IsNullOrWhiteSpace(refreshTokenStoreJson))
    {
      refreshTokenStoreJson = _fallbackRefreshTokenStoreJson;
    }

    if (string.IsNullOrWhiteSpace(refreshTokenStoreJson))
    {
      return null;
    }

    var refreshTokenStore = JsonSerializer.Deserialize<MicrosoftEntraRefreshTokenStore>(refreshTokenStoreJson);

    if (refreshTokenStore?.Tokens is not { Length: > 0 })
    {
      throw new InvalidOperationException("Refresh token store does not contain any tokens.");
    }

    return refreshTokenStore;
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
