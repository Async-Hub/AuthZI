module SiloHost

open AuthZI.MicrosoftOrleans.MicrosoftEntra
open AuthZI.Tests.MicrosoftOrleans.MicrosoftEntra.MicrosoftEntraID.Common
open AuthZI.Tests.MicrosoftOrleans.Grains
open AuthZI.Tests.MicrosoftOrleans.Grains.SimpleAuthorization
open AuthZI.Security.Authorization
open Microsoft.Extensions.Hosting               
open Orleans.Configuration;
open Orleans.Hosting;
open Orleans;
open System
open System.Net
open AuthZI.MicrosoftOrleans.Authorization

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
                           (fun (config:AuthZI.Security.Configuration) ->         
                           config.ConfigureAuthorizationOptions <- Action<AuthorizationOptions>(         
                               AuthorizationConfig.ConfigureOptions)), AuthorizationConfiguration(false))
                       // Some custom authorization services.
                       AuthorizationConfig.ConfigureServices(services)
                       ignore()) |> ignore         
               )         

    let host  = builder.Build()
    host.StartAsync().Wait()
    host