namespace Initialization

open RootConfiguration
open Xunit.Abstractions
open Xunit.Sdk

type Starter(messageSink: IMessageSink) =
    inherit XunitTestFramework(messageSink)
    do siloClientProvider.SiloClient <- SiloClient.getClusterClient()

module CurrentAssembly =
    [<Literal>]
    let TypeName = "Initialization.Starter"
    [<Literal>]
    let Name = "Authzi.MicrosoftOrleans.AzureActiveDirectory.Tests.NET6.0"

[<assembly: Xunit.TestFramework(CurrentAssembly.TypeName, CurrentAssembly.Name)>]
do()