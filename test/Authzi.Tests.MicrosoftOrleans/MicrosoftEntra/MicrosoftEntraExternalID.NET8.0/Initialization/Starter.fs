namespace Initialization

open Authzi.MicrosoftEntra
open Authzi.Deploy.MicrosoftEntra.Configuration
open Authzi.Tests.MicrosoftOrleans.MicrosoftEntra.MicrosoftEntraID.Common
open RootConfiguration
open System
open System.Text.Json
open Xunit.Abstractions
open Xunit.Sdk

[<assembly: Orleans.ApplicationPartAttribute("Authzi.Tests.MicrosoftOrleans.Grains")>]
()

type Starter(messageSink: IMessageSink) =
    inherit XunitTestFramework(messageSink)
    do 
        // Read the configuration.
        let mutable microsoftEntraIdCredentialsJson = Environment.GetEnvironmentVariable("microsoftEntraExternalIdCredentials")

        if String.IsNullOrWhiteSpace(microsoftEntraIdCredentialsJson) then
            microsoftEntraIdCredentialsJson <- Literals.microsoftEntraExternalIDCredentialsJson

        let credentials = JsonSerializer.Deserialize<MicrosoftEntraCredentials>(microsoftEntraIdCredentialsJson)

        // Initialize the test data.
        let entraApp =
            AzureActiveDirectoryApp(credentials.DirectoryId, credentials.WebClient1.Id, 
                credentials.WebClient1.Secret, false, credentials.WebClient1.AllowedScopes)

        TestData.UserWithScopeAdeleV <- [[|credentials.AdeleV.Name; credentials.AdeleV.Password; ["Api1"; "Orleans"]|]]
        TestData.UserWithScopeAlexW <- [[|credentials.AlexW.Name; credentials.AlexW.Password; ["Api1"; "Orleans"]|]]
        TestData.Users <- [[| credentials.AdeleV.Name; credentials.AdeleV.Password |]]
        TestData.AzureActiveDirectoryApp <- entraApp
        TestData.Web1Client <- credentials.WebClient1

        siloClientProvider.SiloClient <- SiloClient.getClusterClient()

module CurrentAssembly =
    [<Literal>]
    let TypeName = "Initialization.Starter"
    [<Literal>]
    let Name = "Authzi.Tests.MicrosoftOrleans.MicrosoftEntra.MicrosoftEntraExternalID.NET8.0"

[<assembly: Xunit.TestFramework(CurrentAssembly.TypeName, CurrentAssembly.Name)>]
()