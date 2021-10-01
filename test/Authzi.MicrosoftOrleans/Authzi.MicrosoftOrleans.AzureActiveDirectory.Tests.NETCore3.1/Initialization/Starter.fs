namespace Initialization

open RootConfiguration
open Credentials.AzureActiveDirectoryB2B1
open Xunit.Abstractions
open Xunit.Sdk
open Authzi.MicrosoftOrleans.AzureActiveDirectory.Tests

type Starter(messageSink: IMessageSink) =
    inherit XunitTestFramework(messageSink)
    do 
        siloClientProvider.SiloClient <- SiloClient.getClusterClient()
        TestData.Users <- [[| AdeleV.Name; AdeleV.Password |]]

module CurrentAssembly =
    [<Literal>]
    let TypeName = "Initialization.Starter"
    [<Literal>]
    let Name = "Authzi.MicrosoftOrleans.AzureActiveDirectory.Tests.NETCore3.1"

[<assembly: Xunit.TestFramework(CurrentAssembly.TypeName, CurrentAssembly.Name)>]
do()