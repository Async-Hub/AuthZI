module SiloClient

open Authzi.MicrosoftOrleans.DuendeSoftware.IdentityServer
open Authzi.Tests.MicrosoftOrleans.Grains
open Authzi.Tests.MicrosoftOrleans.Grains.SimpleAuthorization
open Authzi.Security
open Authzi.Security.Authorization
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Hosting
open Orleans.Configuration;
open Orleans;
open System
open System.Threading.Tasks

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
        
        // Add IdentityServer4 authorization.
        services.AddOrleansClientAuthorization(GlobalConfig.identityServer4Info, 
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

    //let builder = 
    //    ClientBuilder().UseLocalhostClustering()
    //        .Configure<ClusterOptions>(fun (options: ClusterOptions) ->
    //            options.ClusterId <- "Orleans.Security.Test"
    //            options.ServiceId <- "ServiceId")
    //        .ConfigureApplicationParts(fun parts -> parts.AddApplicationPart(typeof<SimpleGrain>.Assembly).WithReferences() |> ignore)
    //        .ConfigureServices(fun services -> configure(services))

    //let clusterClient = builder.Build()
    //clusterClient.Connect().Wait()
    //ClusterSetup.initDocumentsRegistry (fun accessToken ->
    //    globalAccessToken.AccessToken <- accessToken
    //    clusterClient)
    
    //clusterClient
    
let getClusterClient (accessToken: string) =
    globalAccessToken.AccessToken <- accessToken
    clusterClient