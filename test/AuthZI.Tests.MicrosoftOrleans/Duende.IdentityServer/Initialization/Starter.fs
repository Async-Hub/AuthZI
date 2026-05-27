namespace Initialization

open AuthZI.Tests.MicrosoftOrleans.Duende.IdentityServer
open AuthZI.Tests.MicrosoftOrleans.Duende.IdentityServer.GlobalConfig
open Microsoft.Extensions.DependencyInjection
open Orleans

[<assembly: Orleans.ApplicationPartAttribute("AuthZI.Tests.MicrosoftOrleans.Grains")>]
()

type Starter() =
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

[<assembly: Xunit.AssemblyFixture(typeof<Starter>)>]
()