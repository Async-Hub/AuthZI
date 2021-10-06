namespace Initialization

open Authzi.AzureActiveDirectory.Configuration.Common
open Authzi.AzureActiveDirectory.Configuration.Common.Credentials.AzureActiveDirectoryB2C1
open Authzi.MicrosoftOrleans.AzureActiveDirectory.Tests
open RootConfiguration
open Xunit.Abstractions
open Xunit.Sdk

type Starter(messageSink: IMessageSink) =
    inherit XunitTestFramework(messageSink)
    do 
        // Initiate test envieronment with B2C1 data.
        TestData.UserWithScopeAdeleV <- [[|AdeleV.Name; AdeleV.Password; ["Api1"; "Orleans"]|]]
        TestData.UserWithScopeAlexW <- [[|AlexW.Name; AlexW.Password; ["Api1"; "Orleans"]|]]
        TestData.Users <- [[| AdeleV.Name; AdeleV.Password |]]
        TestData.AzureActiveDirectoryApp <- Directories.azureActiveDirectoryAppB2C1
        TestData.Web1Client <- Credentials.AzureActiveDirectoryB2C1.WebClient1

        siloClientProvider.SiloClient <- SiloClient.getClusterClient()

module CurrentAssembly =
    [<Literal>]
    let TypeName = "Initialization.Starter"
    [<Literal>]
    let Name = "Authzi.MicrosoftOrleans.AzureActiveDirectory.Tests.B2C.NET6.0"

[<assembly: Xunit.TestFramework(CurrentAssembly.TypeName, CurrentAssembly.Name)>]
do()