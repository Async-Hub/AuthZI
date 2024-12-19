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
        let mutable microsoftEntraIdCredentialsJson = Environment.GetEnvironmentVariable("microsoftEntraIdCredentials")

        if String.IsNullOrWhiteSpace(microsoftEntraIdCredentialsJson) then
            microsoftEntraIdCredentialsJson <- Literals.microsoftEntraCredentialsJson

        let credentials = JsonSerializer.Deserialize<MicrosoftEntraCredentials>(microsoftEntraIdCredentialsJson)

        // Initialize the test data.
        let entraApp =
            MicrosoftEntraIDApp(credentials.DirectoryId, credentials.WebClient1.Id, 
                credentials.WebClient1.Secret, credentials.WebClient1.AllowedScopes, AadAuthorityAudience.AzureAdMyOrg)

        TestData.UserWithScopeAdeleV <- [[|credentials.AdeleV.Name; credentials.AdeleV.Password; ["Api1"; "Orleans"]|]]
        TestData.UserWithScopeAlexW <- [[|credentials.AlexW.Name; credentials.AlexW.Password; ["Api1"; "Orleans"]|]]
        TestData.Users <- [[| credentials.AdeleV.Name; credentials.AdeleV.Password |]]
        TestData.AzureActiveDirectoryApp <- entraApp
        TestData.Web1Client <- credentials.WebClient1

        TestData.GetAccessTokenForUserOnWebClient1Async <- 
            AccessTokenRetriever.getTokenByUserNameAndPasswordForEntraIDTenant

        let configureSiloHost = fun (services: IServiceCollection) ->
            // Add Azure Active Directory authorization.
            services.AddOrleansAuthorization(TestData.AzureActiveDirectoryApp, 
                (fun (config: AuthZI.Security.Configuration) -> 
                    config.ConfigureAuthorizationOptions 
                      <- Action<AuthorizationOptions>(AuthorizationConfig.ConfigureOptions)), 
                      AuthorizationConfiguration(false))
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

module CurrentAssembly =
    [<Literal>]
    let TypeName = "Initialization.Starter"
    [<Literal>]
    let Name = "AuthZI.Tests.MicrosoftOrleans.MicrosoftEntra.MicrosoftEntraID.NET9.0"

[<assembly: Xunit.TestFramework(CurrentAssembly.TypeName, CurrentAssembly.Name)>]
()