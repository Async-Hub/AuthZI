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
open System.Text.Json
open Xunit.Abstractions
open Xunit.Sdk
open AuthZI.MicrosoftOrleans.Authorization

[<assembly: Orleans.ApplicationPartAttribute("AuthZI.Tests.MicrosoftOrleans.Grains")>]
()

type Starter(messageSink: IMessageSink) =
  inherit XunitTestFramework(messageSink)

  do
    // Read the configuration.
    let mutable microsoftEntraIdCredentialsJson =
      Environment.GetEnvironmentVariable("microsoftEntraExternalIdCredentials")

    if String.IsNullOrWhiteSpace(microsoftEntraIdCredentialsJson) then
      microsoftEntraIdCredentialsJson <- Literals.microsoftEntraExternalIDCredentialsJson

    let credentials = JsonSerializer.Deserialize<MicrosoftEntraCredentials>(microsoftEntraIdCredentialsJson)

    // Initialize the test data.
    let web1ClientApp =
      MicrosoftEntraExternalIDApp(
        credentials.DirectoryId,
        credentials.WebClient1.Id,
        credentials.WebClient1.Secret,
        credentials.WebClient1.AllowedScopes,
        AadAuthorityAudience.AzureAdMyOrg
      )
      
    let web2ClientApp =
      MicrosoftEntraExternalIDApp(
        credentials.DirectoryId,
        credentials.WebClient2.Id,
        credentials.WebClient2.Secret,
        credentials.WebClient2.AllowedScopes,
        AadAuthorityAudience.AzureAdMyOrg
      )

    TestData.UserWithScopeAdeleV <-
      [ [| credentials.AdeleV.Name; credentials.AdeleV.Password; [ "Api1"; "Orleans" ] |] ]

    TestData.UserWithScopeAlexW <- [ [| credentials.AlexW.Name; credentials.AlexW.Password; [ "Api1"; "Orleans" ] |] ]
    TestData.Users <- [ [| credentials.AdeleV.Name; credentials.AdeleV.Password |] ]
    TestData.Web1ClientApp <- web1ClientApp
    TestData.Web2ClientApp <- web2ClientApp

    TestData.GetAccessTokenForUserOnMicrosoftEntraAppAsync <-
      AccessTokenRetriever.getTokenByUserNameAndPasswordForEntraExternalIDTenant

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

//siloClientProvider.SiloClient <- SiloClient.getClusterClient()

module CurrentAssembly =
  [<Literal>]
  let TypeName = "Initialization.Starter"

  [<Literal>]
  let Name = "AuthZI.Tests.MicrosoftOrleans.MicrosoftEntra.MicrosoftEntraExternalID.NET9.0"

[<assembly: Xunit.TestFramework(CurrentAssembly.TypeName, CurrentAssembly.Name)>]
()