namespace Initialization

open Authzi.MicrosoftOrleans.MicrosoftEntra
open Authzi.Security.Authorization
open Authzi.Deploy.MicrosoftEntra.Configuration
open Authzi.MicrosoftEntra
open Authzi.Security
open Authzi.Security.Authorization
open Authzi.Tests.MicrosoftOrleans.MicrosoftEntra
open Authzi.Tests.MicrosoftOrleans.Grains
open Authzi.Tests.MicrosoftOrleans.Grains.SimpleAuthorization
open Authzi.Tests.MicrosoftOrleans.MicrosoftEntra.MicrosoftEntraID.Common
open Microsoft.Extensions.DependencyInjection
open Orleans
open RootConfiguration
open System
open System.Text.Json
open Xunit.Abstractions
open Xunit.Sdk
open Authzi.Identity.MicrosoftEntra
open Microsoft.Identity.Client

[<assembly: Orleans.ApplicationPartAttribute("Authzi.Tests.MicrosoftOrleans.Grains")>]
()

type Starter(messageSink: IMessageSink) =
    inherit XunitTestFramework(messageSink)
    do 
        // Read the configuration.
        let mutable microsoftEntraIdCredentialsJson = Environment.GetEnvironmentVariable("microsoftEntraExternalIdCredentials")

        if String.IsNullOrWhiteSpace(microsoftEntraIdCredentialsJson) then
            microsoftEntraIdCredentialsJson <- Literals.microsoftEntraExternalIDCredentialsJson

        let credentials = JsonSerializer.Deserialize<MicrosoftEntraCredentials>(microsoftEntraIdCredentialsJson)

        // Initialize the test data.
        let entraApp =
            MicrosoftEntraExternalIDApp(credentials.DirectoryId, credentials.WebClient1.Id, 
                credentials.WebClient1.Secret, credentials.WebClient1.AllowedScopes, AadAuthorityAudience.AzureAdMyOrg)

        TestData.UserWithScopeAdeleV <- [[|credentials.AdeleV.Name; credentials.AdeleV.Password; ["Api1"; "Orleans"]|]]
        TestData.UserWithScopeAlexW <- [[|credentials.AlexW.Name; credentials.AlexW.Password; ["Api1"; "Orleans"]|]]
        TestData.Users <- [[| credentials.AdeleV.Name; credentials.AdeleV.Password |]]
        TestData.AzureActiveDirectoryApp <- entraApp
        TestData.Web1Client <- credentials.WebClient1

        TestData.GetAccessTokenForUserOnWebClient1Async <- 
            AccessTokenRetriever.getTokenByUserNameAndPasswordForEntraExternalIDTenant

        let configureSiloHost = fun (services: IServiceCollection) ->
            // Add Azure Active Directory authorization.
            services.AddOrleansAuthorization(TestData.AzureActiveDirectoryApp, 
                fun (config: Authzi.Security.Configuration) -> 
                    config.ConfigureAuthorizationOptions <- Action<AuthorizationOptions>(AuthorizationConfig.ConfigureOptions)
                    ignore())
            // Some custom authorization services.
            AuthorizationConfig.ConfigureServices(services)
        
        // Initialize Orleans test cluster.
        let siloHost = SiloHostBuilder.Build(configureSiloHost)
        siloHost.StartAsync().Wait()

        // Initialize the SiloClient.
        let configureSiloClient = fun (services: IServiceCollection) ->
            let configureCluster (config: Configuration) = 
                                    config.ConfigureAuthorizationOptions <- 
                                        Action<AuthorizationOptions>(AuthorizationConfig.ConfigureOptions)
            AuthorizationConfig.ConfigureServices(services)
            services.AddSingleton<IAccessTokenProvider>( fun _ -> accessTokenProvider) |> ignore
            // Add Azure Active Directory authorization.
            services.AddOrleansClientAuthorization(TestData.AzureActiveDirectoryApp,
                fun config -> configureCluster(config)) |> ignore

        let siloClientHost = SiloClientBuilder.Build(configureSiloClient)
        siloClientHost.StartAsync().Wait()
        TestData.IClusterClient <- siloClientHost.Services.GetService<IClusterClient>()

        //siloClientProvider.SiloClient <- SiloClient.getClusterClient()

module CurrentAssembly =
    [<Literal>]
    let TypeName = "Initialization.Starter"
    [<Literal>]
    let Name = "Authzi.Tests.MicrosoftOrleans.MicrosoftEntra.MicrosoftEntraExternalID.NET8.0"

[<assembly: Xunit.TestFramework(CurrentAssembly.TypeName, CurrentAssembly.Name)>]
()