namespace Initialization

open AuthZI.Deploy.MicrosoftEntra.Configuration
open AuthZI.Identity.MicrosoftEntra
open AuthZI.MicrosoftEntra
open AuthZI.MicrosoftOrleans.MicrosoftEntra
open AuthZI.Security
open AuthZI.Security.Authorization
open AuthZI.Tests.MicrosoftOrleans.Grains
open AuthZI.Tests.MicrosoftOrleans.MicrosoftEntra
open AuthZI.Tests.MicrosoftOrleans.MicrosoftEntra.MicrosoftEntraID.Common
open Microsoft.Extensions.DependencyInjection
open Microsoft.Identity.Client
open Orleans
open RootConfiguration
open System
open System.Threading.Tasks
open AuthZI.MicrosoftOrleans.Authorization

[<assembly: Orleans.ApplicationPartAttribute("AuthZI.Tests.MicrosoftOrleans.Grains")>]
()

type TestDataPipelineStartup() =
  interface Xunit.v3.ITestPipelineStartup with
    member _.StartAsync(_diagnosticMessageSink: Xunit.Sdk.IMessageSink) =
      let credentialsJson =
        TestDataInitialization.getCredentialsJson "microsoftEntraExternalIdCredentials" Literals.microsoftEntraExternalIDCredentialsJson

      TestDataInitialization.initialize
        credentialsJson
        (fun credentials ->
          MicrosoftEntraExternalIDApp(
            credentials.DirectoryId,
            credentials.WebClient1.Id,
            credentials.WebClient1.Secret,
            credentials.WebClient1.AllowedScopes,
            AadAuthorityAudience.AzureAdMyOrg
          )
          :> MicrosoftEntraApp)
        (fun credentials ->
          MicrosoftEntraExternalIDApp(
            credentials.DirectoryId,
            credentials.WebClient2.Id,
            credentials.WebClient2.Secret,
            credentials.WebClient2.AllowedScopes,
            AadAuthorityAudience.AzureAdMyOrg
          )
          :> MicrosoftEntraApp)
        AccessTokenRetriever.getTokenByUserNameAndPasswordForEntraExternalIDTenant

      ValueTask()

    member _.StopAsync() = ValueTask()

type Starter() =
  do
    let configureSiloHost =
      fun (services: IServiceCollection) ->
        // Add Azure Active Directory authorization.
        services.AddOrleansAuthorization(
          TestData.Web1ClientApp,
          (fun (config: AuthZI.Security.Configuration) ->
            config.ConfigureAuthorizationOptions <- Action<AuthorizationOptions>(AuthorizationConfig.ConfigureOptions)),
          AuthorizationConfiguration(false)
        )
        // Some custom authorization services.
        AuthorizationConfig.ConfigureServices(services)

    // Initialize Orleans test cluster.
    let siloHost = SiloHostBuilder.Build(configureSiloHost)
    siloHost.StartAsync().Wait()

    // Initialize the SiloClient.
    let configureSiloClient =
      fun (services: IServiceCollection) ->
        let configureCluster (config: Configuration) =
          config.ConfigureAuthorizationOptions <- Action<AuthorizationOptions>(AuthorizationConfig.ConfigureOptions)

        AuthorizationConfig.ConfigureServices(services)
        services.AddSingleton<IAccessTokenProvider>(fun _ -> accessTokenProvider) |> ignore
        // Add Azure Active Directory authorization.
        services.AddOrleansClientAuthorization(TestData.Web1ClientApp, fun config -> configureCluster (config))
        |> ignore

    let siloClientHost = SiloClientBuilder.Build(configureSiloClient)
    siloClientHost.StartAsync().Wait()
    TestData.IClusterClient <- siloClientHost.Services.GetService<IClusterClient>()

[<assembly: Xunit.v3.TestPipelineStartup(typeof<TestDataPipelineStartup>)>]
[<assembly: Xunit.AssemblyFixture(typeof<Starter>)>]
()
