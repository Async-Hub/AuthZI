namespace Initialization

open AuthZI.Tests.MicrosoftOrleans.Duende.IdentityServer
open AuthZI.Tests.MicrosoftOrleans.Duende.IdentityServer.GlobalConfig
open Microsoft.Extensions.DependencyInjection
open Orleans
open Xunit.Abstractions
open Xunit.Sdk

[<assembly: Orleans.ApplicationPartAttribute("AuthZI.Tests.MicrosoftOrleans.Grains")>]
()

type Starter(messageSink: IMessageSink) =
    inherit XunitTestFramework(messageSink)
    do
        // Start Orleans silo host.
        let siloHost = SiloHostBuilder.Build()
        siloHost.StartAsync().Wait()
        
        // Start IdentityServer.
        let identityServer = IdentityServerBuilder.build(identityServerUrl).Build()
        identityServer.StartAsync().Wait()
        discoveryDocument <- IdentityServerBuilder.getDiscoveryDocument identityServerUrl

        // Start Orleans client.
        let siloClientHost = SiloClientBuilder.build accessTokenProvider identityServerConfig
        siloClientHost.StartAsync().Wait()
        let clusterClient = siloClientHost.Services.GetService<IClusterClient>()
        GlobalConfig.clusterClient <- clusterClient

        ClusterSetup.initDocumentsRegistry getClusterClient

module CurrentAssembly =
    [<Literal>]
    let TypeName = "Initialization.Starter"
    [<Literal>]
    let Name = "AuthZI.Tests.MicrosoftOrleans.Duende.IdentityServer"

[<assembly: Xunit.TestFramework(CurrentAssembly.TypeName, CurrentAssembly.Name)>]
()