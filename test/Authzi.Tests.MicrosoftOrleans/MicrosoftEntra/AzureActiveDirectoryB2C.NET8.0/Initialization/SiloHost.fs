﻿module SiloHost

open Authzi.MicrosoftOrleans.MicrosoftEntra
open Authzi.Tests.MicrosoftOrleans.MicrosoftEntra.MicrosoftEntraID.Common
open Authzi.Tests.MicrosoftOrleans.Grains
open Authzi.Tests.MicrosoftOrleans.Grains.SimpleAuthorization
open Authzi.Security.Authorization
open Microsoft.Extensions.Hosting               
open Orleans.Configuration;
open Orleans.Hosting;
open Orleans;
open System
open System.Net

let startSilo () =
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
                       // Add Azure Active Directory authorization.
                       services.AddOrleansAuthorization(TestData.AzureActiveDirectoryApp,       
                           fun (config:Authzi.Security.Configuration) ->         
                           config.ConfigureAuthorizationOptions <- Action<AuthorizationOptions>(         
                               AuthorizationConfig.ConfigureOptions)         
                           ignore())
                       // Some custom authorization services.
                       AuthorizationConfig.ConfigureServices(services)
                       ignore()) |> ignore         
               )         

    let host  = builder.Build()
    host.StartAsync().Wait()
    host