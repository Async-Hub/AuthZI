module SiloClient

open Authzi.MicrosoftOrleans.AzureActiveDirectory
open Authzi.MicrosoftOrleans.Grains
open Authzi.MicrosoftOrleans.Grains.SimpleAuthorization
open Authzi.Security
open Authzi.Security.Authorization
open Microsoft.Extensions.DependencyInjection
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
        services.AddOrleansClientAuthorization(GlobalConfig.azureActiveDirectoryAppB2B1,
            fun config -> configureCluster(config)) |> ignore

    let builder = 
        ClientBuilder().UseLocalhostClustering()
            .Configure<ClusterOptions>(fun (options: ClusterOptions) ->
                options.ClusterId <- "Orleans.Security.Test"
                options.ServiceId <- "ServiceId")
            .ConfigureApplicationParts(fun parts -> 
                parts.AddApplicationPart(typeof<SimpleGrain>.Assembly).WithReferences() |> ignore)
            .ConfigureServices(fun services -> configure(services))

    let clusterClient = builder.Build()
    clusterClient.Connect().Wait()
    clusterClient
    
let getClusterClient() = clusterClient

let getIHttpClientFactory = clusterClient.ServiceProvider.GetService<IHttpClientFactory>()