module SiloClient

open Authzi.MicrosoftOrleans.AzureActiveDirectory
open Authzi.MicrosoftOrleans
open Authzi.MicrosoftOrleans.Grains
open Authzi.MicrosoftOrleans.Grains.SimpleAuthorization
open Authzi.Security
open Authzi.Security.Authorization
open Microsoft.Extensions.DependencyInjection
open Orleans.Configuration;
open Orleans;
open System
open System.Threading.Tasks
open System.Net.Http

type AccessTokenProvider() =
    let mutable accessToken = String.Empty
    member _.AccessToken with set (value) = accessToken <- value
    
    interface IAccessTokenProvider with
        member _.RetrieveTokenAsync() = Task.FromResult(accessToken);

let private globalAccessToken = AccessTokenProvider()
let accessTokenProvider = globalAccessToken :> IAccessTokenProvider

let private clusterClient =
    SiloHost.startSilo() |> ignore
            
    let configure (services: IServiceCollection) =
        let configureCluster (config: Configuration) = 
            config.ConfigureAuthorizationOptions <- 
                Action<AuthorizationOptions>(AuthorizationConfig.ConfigureOptions)
            AuthorizationConfig.ConfigureServices(services)
        services.AddSingleton<IAccessTokenProvider>( fun _ -> accessTokenProvider) |> ignore
        // Add Azure Active Directory authorization for Orleans client.
        services.AddOrleansAzureActiveDirectoryAuthorization(GlobalConfig.azureActiveDirectoryAppB2B1)
        services.AddOrleansClientAuthorization(fun config -> configureCluster(config)) |> ignore

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
    //ClusterSetup.initDocumentsRegistry (fun accessToken ->
    //    globalAccessToken.AccessToken <- accessToken
    //    clusterClient)
    
    clusterClient
    
let getClusterClient (accessToken: string) =
    globalAccessToken.AccessToken <- accessToken
    clusterClient

let getIHttpClientFactory = clusterClient.ServiceProvider.GetService<IHttpClientFactory>()