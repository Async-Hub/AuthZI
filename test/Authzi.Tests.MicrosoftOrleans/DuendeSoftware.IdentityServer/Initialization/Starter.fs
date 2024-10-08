namespace Initialization

open Authzi.Tests.MicrosoftOrleans.DuendeSoftware.IdentityServer
open Authzi.Tests.MicrosoftOrleans.DuendeSoftware.IdentityServer.GlobalConfig
open Authzi.Tests.MicrosoftOrleans.Grains
open Microsoft.Extensions.DependencyInjection
open Orleans
open Xunit.Abstractions
open Xunit.Sdk

[<assembly: Orleans.ApplicationPartAttribute("Authzi.Tests.MicrosoftOrleans.Grains")>]
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
        let siloClientHost = SiloClientBuilder.build accessTokenProvider identityServer4Info
        siloClientHost.StartAsync().Wait()
        let clusterClient = siloClientHost.Services.GetService<IClusterClient>()
        GlobalConfig.clusterClient <- clusterClient

        ClusterSetup.initDocumentsRegistry getClusterClient

module CurrentAssembly =
    [<Literal>]
    let TypeName = "Initialization.Starter"
    [<Literal>]
    let Name = "Authzi.Tests.MicrosoftOrleans.DuendeSoftware.IdentityServer"

[<assembly: Xunit.TestFramework(CurrentAssembly.TypeName, CurrentAssembly.Name)>]
()