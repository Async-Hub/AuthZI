namespace AuthZI.Tests.MicrosoftOrleans.MicrosoftEntra

open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Hosting               
open Orleans.Configuration;
open Orleans.Hosting;
open System
open System.Net

module SiloHostBuilder =
    let Build (configureDelegate: Action<IServiceCollection>) =
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
                       .ConfigureServices(configureDelegate) |> ignore)
        builder.Build()