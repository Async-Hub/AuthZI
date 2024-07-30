namespace Initialization

open Authzi.Deploy.MicrosoftEntra.Configuration.Common
open Authzi.Deploy.MicrosoftEntra.Configuration.Common.Credentials.AzureActiveDirectoryB2C1
open Authzi.Tests.MicrosoftOrleans.MicrosoftEntra.MicrosoftEntraID.Common
open RootConfiguration
open Xunit.Abstractions
open Xunit.Sdk

[<assembly: Orleans.ApplicationPartAttribute("Authzi.Tests.MicrosoftOrleans.Grains")>]
()

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
    let Name = "Authzi.Tests.MicrosoftOrleans.MicrosoftEntra.AzureActiveDirectoryB2C.NET8.0"

[<assembly: Xunit.TestFramework(CurrentAssembly.TypeName, CurrentAssembly.Name)>]
()