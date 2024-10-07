module SiloClient

open Authzi.MicrosoftOrleans.DuendeSoftware.IdentityServer
open Authzi.Tests.MicrosoftOrleans.Grains
open Authzi.Tests.MicrosoftOrleans.Grains.SimpleAuthorization
open Authzi.Security
open Authzi.Security.Authorization
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Hosting
open Orleans.Configuration;
open Orleans.Hosting
open System
open System.Threading.Tasks
open Orleans

type AccessTokenProvider() =
    let mutable accessToken = String.Empty
    member _.AccessToken with set (value) = accessToken <- value
    
    interface IAccessTokenProvider with
        member _.RetrieveTokenAsync() = Task.FromResult(accessToken);

let private globalAccessToken = AccessTokenProvider()
let accessTokenProvider = globalAccessToken :> IAccessTokenProvider

let private clusterClient =      
    let configureDelegate (services: IServiceCollection) =
        let configureCluster (config: Configuration) = 
            config.ConfigureAuthorizationOptions <- 
                Action<AuthorizationOptions>(AuthorizationConfig.ConfigureOptions)
            AuthorizationConfig.ConfigureServices(services)
        services.AddSingleton<IAccessTokenProvider>( fun _ -> accessTokenProvider) |> ignore
        
        // Add IdentityServer4 authorization.
        services.AddOrleansClientAuthorization(GlobalConfig.identityServer4Info, 
            fun config -> configureCluster(config)) |> ignore

    let hostBuilder = HostBuilder().UseOrleansClient(fun clientBuilder ->
                clientBuilder.UseLocalhostClustering()
                    .Configure<ClusterOptions>(fun (options: ClusterOptions) ->
                        options.ClusterId <- "Orleans.Security.Test"
                        options.ServiceId <- "ServiceId")
                    .ConfigureServices(configureDelegate) |> ignore)
    
    let siloClientHost = hostBuilder.Build()
    siloClientHost.StartAsync().Wait() |> ignore
    let clusterClient = siloClientHost.Services.GetService<IClusterClient>()
    ClusterSetup.initDocumentsRegistry (fun accessToken ->
        globalAccessToken.AccessToken <- accessToken
        clusterClient)
    clusterClient
    
let getClusterClient (accessToken: string) =
    globalAccessToken.AccessToken <- accessToken
    clusterClient