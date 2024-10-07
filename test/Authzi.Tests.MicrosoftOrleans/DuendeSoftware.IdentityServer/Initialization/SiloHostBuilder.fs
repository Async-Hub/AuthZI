namespace Authzi.Tests.MicrosoftOrleans.DuendeSoftware.IdentityServer

open Authzi.Tests.MicrosoftOrleans.Grains
open Authzi.Tests.MicrosoftOrleans.Grains.SimpleAuthorization
open Authzi.MicrosoftOrleans.DuendeSoftware.IdentityServer
open Authzi.Security.Authorization
open Microsoft.Extensions.Hosting               
open Orleans.Configuration;
open Orleans.Hosting;
open Orleans;
open System
open System.Net

module SiloHostBuilder =
    let Build() = 
        let builder = 
                HostBuilder()
                    .UseEnvironment(Environments.Development)
                    .UseOrleans(fun context siloBuilder ->          
                       siloBuilder         
                           .UseLocalhostClustering()         
                           .Configure<ClusterOptions>(fun (options:ClusterOptions) ->         
                               options.ClusterId <- "Orleans.Security.Test"         
                               options.ServiceId <- "ServiceId"         
                               ignore())         
                           .Configure<EndpointOptions>(fun (options:EndpointOptions) ->          
                               options.AdvertisedIPAddress <- IPAddress.Loopback         
                               ignore())
                           .AddMemoryGrainStorage("MemoryGrainStorage")
                           .ConfigureServices(fun services ->
                               // Add IdentityServer4 authorization.
                               services.AddOrleansAuthorization(GlobalConfig.identityServer4InfoCluster,      
                                   fun (config:Authzi.Security.Configuration) ->         
                                   config.ConfigureAuthorizationOptions <- Action<AuthorizationOptions>(         
                                       AuthorizationConfig.ConfigureOptions)         
                                   ignore())
                               // Some custom authorization services.
                               AuthorizationConfig.ConfigureServices(services)
                               ignore()) |> ignore         
                       )
        builder.Build()




    
    