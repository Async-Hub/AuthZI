namespace AuthZI.Tests.MicrosoftOrleans.MicrosoftEntra

open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Hosting
open Orleans.Configuration
open Orleans.Hosting
open System

module SiloClientBuilder =
    let Build (configureDelegate: Action<IServiceCollection>) =
        let hostBuilder = HostBuilder().UseOrleansClient(fun clientBuilder ->
                clientBuilder.UseLocalhostClustering()
                    .Configure<ClusterOptions>(fun (options: ClusterOptions) ->
                        options.ClusterId <- "Orleans.Security.Test"
                        options.ServiceId <- "ServiceId")
                    .ConfigureServices(configureDelegate) |> ignore)
        hostBuilder.Build()