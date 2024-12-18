namespace AuthZI.Tests.MicrosoftOrleans.Duende.IdentityServer

open AuthZI.MicrosoftOrleans.Duende.IdentityServer
open AuthZI.Security
open AuthZI.Security.Authorization
open AuthZI.Tests.MicrosoftOrleans.Grains
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Logging
open Microsoft.Extensions.Hosting
open Orleans.Configuration;
open Orleans.Hosting
open System
open AuthZI.MicrosoftOrleans.Authorization

module SiloClientBuilder =
    let build accessTokenProvider identityServerConfig =
        let configureDelegate (services: IServiceCollection) =
            let configureCluster (config: Configuration) = 
                config.ConfigureAuthorizationOptions <- 
                    Action<AuthorizationOptions>(AuthorizationConfig.ConfigureOptions)
                AuthorizationConfig.ConfigureServices(services)
            services.AddSingleton<IAccessTokenProvider>( fun _ -> accessTokenProvider) |> ignore
        
            // Add IdentityServer authorization.
            services.AddOrleansClientAuthorizationNew(identityServerConfig, configureCluster)

        let hostBuilder = HostBuilder().UseOrleansClient(fun clientBuilder ->
                    clientBuilder
                        .Configure<ClusterOptions>(fun (options: ClusterOptions) ->
                            options.ClusterId <- "Orleans.Security.Test"
                            options.ServiceId <- "ServiceId")
                        .UseLocalhostClustering()
                        .ConfigureServices(configureDelegate) |> ignore)
                            .ConfigureLogging(fun logging -> logging.AddDebug() |> ignore)
    
        hostBuilder.Build()