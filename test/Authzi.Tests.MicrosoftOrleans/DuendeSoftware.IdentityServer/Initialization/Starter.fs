namespace Initialization

open Xunit.Abstractions
open Xunit.Sdk
open Authzi.Tests.MicrosoftOrleans.DuendeSoftware.IdentityServer
open Authzi.Tests.MicrosoftOrleans.Grains

[<assembly: Orleans.ApplicationPartAttribute("Authzi.Tests.MicrosoftOrleans.Grains")>]
()

type Starter(messageSink: IMessageSink) =
    inherit XunitTestFramework(messageSink)
    do
        let siloHost = SiloHostBuilder.Build()
        siloHost.StartAsync().Wait()

module CurrentAssembly =
    [<Literal>]
    let TypeName = "Initialization.Starter"
    [<Literal>]
    let Name = "Authzi.Tests.MicrosoftOrleans.DuendeSoftware.IdentityServer"

[<assembly: Xunit.TestFramework(CurrentAssembly.TypeName, CurrentAssembly.Name)>]
()