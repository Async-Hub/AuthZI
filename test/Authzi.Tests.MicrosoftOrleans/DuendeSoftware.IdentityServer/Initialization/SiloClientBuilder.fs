namespace Authzi.Tests.MicrosoftOrleans.DuendeSoftware.IdentityServer

open Authzi.MicrosoftOrleans.DuendeSoftware.IdentityServer
open Authzi.Security
open Authzi.Security.Authorization
open Authzi.Tests.MicrosoftOrleans.Grains
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Logging
open Microsoft.Extensions.Hosting
open Orleans.Configuration;
open Orleans.Hosting
open System

module SiloClientBuilder =
    let build accessTokenProvider identityServer4Info =
        let configureDelegate (services: IServiceCollection) =
            let configureCluster (config: Configuration) = 
                config.ConfigureAuthorizationOptions <- 
                    Action<AuthorizationOptions>(AuthorizationConfig.ConfigureOptions)
                AuthorizationConfig.ConfigureServices(services)
            services.AddSingleton<IAccessTokenProvider>( fun _ -> accessTokenProvider) |> ignore
        
            // Add IdentityServer4 authorization.
            services.AddOrleansClientAuthorization(identityServer4Info, 
                fun config -> configureCluster(config)) |> ignore

        let hostBuilder = HostBuilder().UseOrleansClient(fun clientBuilder ->
                    clientBuilder
                        .Configure<ClusterOptions>(fun (options: ClusterOptions) ->
                            options.ClusterId <- "Orleans.Security.Test"
                            options.ServiceId <- "ServiceId")
                        .UseLocalhostClustering()
                        .ConfigureServices(configureDelegate) |> ignore)
                            .ConfigureLogging(fun logging -> logging.AddDebug() |> ignore)
    
        hostBuilder.Build()