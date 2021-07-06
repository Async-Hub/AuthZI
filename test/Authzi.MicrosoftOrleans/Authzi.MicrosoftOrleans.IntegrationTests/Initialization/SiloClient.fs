module SiloClient

open Microsoft.Extensions.DependencyInjection
open Orleans.Configuration;
open Authzi.Security
open Authzi.MicrosoftOrleans.Client
open Authzi.MicrosoftOrleans.Grains
open Authzi.MicrosoftOrleans.Grains.SimpleAuthorization
open Orleans;
open System
open System.Threading.Tasks
open Authzi.Security.Authorization

type AccessTokenProvider() =
    let mutable accessToken = String.Empty
    member this.AccessToken
            with set (value) = accessToken <- value
    
    interface IAccessTokenProvider with
        member this.RetrieveTokenAsync() =
            Task.FromResult(accessToken);

let private globalAccessToken = AccessTokenProvider()
let accessTokenProvider = globalAccessToken :> IAccessTokenProvider

let private clusterClient =
    SiloHost.startSilo() |> ignore
                         
    let builder = ClientBuilder()
                    .UseLocalhostClustering()
                    .Configure<ClusterOptions>(fun (options: ClusterOptions) ->
                        options.ClusterId <- "Orleans.Security.Test"
                        options.ServiceId <- "ServiceId"
                        ignore())
                    .ConfigureApplicationParts(fun parts -> 
                                  parts.AddApplicationPart(typeof<SimpleGrain>.Assembly).WithReferences() |> ignore)
                    .ConfigureServices(fun services ->
                                  services.AddOrleansClusteringAuthorization(GlobalConfig.identityServer4Info,
                                      fun (config:Configuration) ->
                                      config.ConfigureAuthorizationOptions <- Action<AuthorizationOptions>(fun options ->
                                          AuthorizationConfig.ConfigureOptions(options) |> ignore)
                                      ignore())
                                  // Some custom authorization services.
                                  AuthorizationConfig.ConfigureServices(services)
                                  services.AddSingleton<IAccessTokenProvider>( fun _ -> accessTokenProvider)
                                  |> ignore)

    let clusterClient = builder.Build()
    clusterClient.Connect().Wait()
    ClusterSetup.initDocumentsRegistry (fun accessToken ->
        globalAccessToken.AccessToken <- accessToken
        clusterClient)
    
    clusterClient
    
let getClusterClient (accessToken: string) =
    globalAccessToken.AccessToken <- accessToken
    clusterClient