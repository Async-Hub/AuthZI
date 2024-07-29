module SiloClient

open Authzi.MicrosoftOrleans.MicrosoftEntra
open Authzi.Tests.MicrosoftOrleans.MicrosoftEntra.MicrosoftEntraID.Common
open Authzi.Tests.MicrosoftOrleans.Grains
open Authzi.Tests.MicrosoftOrleans.Grains.SimpleAuthorization
open Authzi.Security
open Authzi.Security.Authorization
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Hosting
open Orleans.Configuration;
open Orleans;
open RootConfiguration;
open System
open System.Net.Http

let private clusterClient =
    SiloHost.startSilo() |> ignore
            
    let configure (services: IServiceCollection) =
        let configureCluster (config: Configuration) = 
            config.ConfigureAuthorizationOptions <- 
                Action<AuthorizationOptions>(AuthorizationConfig.ConfigureOptions)
            AuthorizationConfig.ConfigureServices(services)
        services.AddSingleton<IAccessTokenProvider>( fun _ -> accessTokenProvider) |> ignore
        // Add Azure Active Directory authorization.
        services.AddOrleansClientAuthorization(TestData.AzureActiveDirectoryApp,
            fun config -> configureCluster(config)) |> ignore

    let hostBuilder = new HostBuilder()
    hostBuilder.UseOrleansClient(fun (clientBuilder : Hosting.IClientBuilder) ->
        clientBuilder.Services.Configure<ClusterOptions>(fun (options: ClusterOptions) ->
            options.ClusterId <- "Orleans.Security.Test"
            options.ServiceId <- "ServiceId") |> ignore
        
        configure(clientBuilder.Services)) |> ignore
    
    let host = hostBuilder.Build()
    host.StartAsync() |> ignore
    
    host.Services.GetService<IClusterClient>();
    
let getClusterClient() = clusterClient
let getIHttpClientFactory = clusterClient.ServiceProvider.GetService<IHttpClientFactory>()