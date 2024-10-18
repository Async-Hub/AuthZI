﻿namespace AuthZI.Tests.MicrosoftOrleans.DuendeSoftware.IdentityServer

open AuthZI.Tests.MicrosoftOrleans.Grains
open AuthZI.MicrosoftOrleans.DuendeSoftware.IdentityServer
open AuthZI.Security.Authorization
open Microsoft.Extensions.Hosting   
open Microsoft.Extensions.Logging
open Orleans.Configuration;
open Orleans.Hosting;
open System

module SiloHostBuilder =
    let Build() = 
        let builder = 
                HostBuilder()
                    .UseEnvironment(Environments.Development)
                    .UseOrleans(fun context siloBuilder ->          
                       siloBuilder         
                           .UseLocalhostClustering()
                           .AddMemoryGrainStorage("MemoryGrainStorage")
                           .Configure<ClusterOptions>(fun (options:ClusterOptions) ->         
                               options.ClusterId <- "Orleans.Security.Test"         
                               options.ServiceId <- "ServiceId"         
                               ignore())
                           .ConfigureLogging(fun logging -> logging.AddDebug() |> ignore)
                           .ConfigureServices(fun services ->
                               // Add IdentityServer4 authorization.
                               services.AddOrleansAuthorization(GlobalConfig.identityServerConfigCluster,      
                                   fun (config:AuthZI.Security.Configuration) ->         
                                   config.ConfigureAuthorizationOptions <- Action<AuthorizationOptions>(         
                                       AuthorizationConfig.ConfigureOptions)         
                                   ignore())
                               // Some custom authorization services.
                               AuthorizationConfig.ConfigureServices(services)
                               ignore()) |> ignore         
                       )
        builder.Build()