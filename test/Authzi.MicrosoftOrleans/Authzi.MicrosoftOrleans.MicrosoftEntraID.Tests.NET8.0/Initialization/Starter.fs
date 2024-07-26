namespace Initialization

open Authzi.AzureActiveDirectory.Configuration.Common.Credentials.AzureActiveDirectoryB2B1
open Authzi.MicrosoftOrleans.AzureActiveDirectory.Tests
open RootConfiguration
open Xunit.Abstractions
open Xunit.Sdk

type Starter(messageSink: IMessageSink) =
    inherit XunitTestFramework(messageSink)
    do 
        siloClientProvider.SiloClient <- SiloClient.getClusterClient()
        TestData.Users <- [[| AdeleV.Name; AdeleV.Password |]]

module CurrentAssembly =
    [<Literal>]
    let TypeName = "Initialization.Starter"
    [<Literal>]
    let Name = "Authzi.MicrosoftOrleans.MicrosoftEntraID.Tests.NET8.0"

[<assembly: Xunit.TestFramework(CurrentAssembly.TypeName, CurrentAssembly.Name)>]
do()