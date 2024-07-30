namespace Initialization

open Authzi.MicrosoftEntra
open Authzi.Deploy.MicrosoftEntra.Configuration.Common
open Authzi.Deploy.MicrosoftEntra.Configuration.Common.Credentials.AzureActiveDirectoryB2B1
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
        let mutable microsoftEntraIdCredentialsJson = Environment.GetEnvironmentVariable("microsoftEntraIdCredentials")

        if String.IsNullOrWhiteSpace(microsoftEntraIdCredentialsJson) then
            microsoftEntraIdCredentialsJson <- Literals.microsoftEntraCredentialsJson

        let credentials = JsonSerializer.Deserialize<MicrosoftEntraCredentials>(microsoftEntraIdCredentialsJson)

        // Initialize the test data.
        let azureActiveDirectoryAppB2B1 =
            AzureActiveDirectoryApp(credentials.DirectoryId, credentials.WebClient1.Id, 
                credentials.WebClient1.Secret, credentials.WebClient1.AllowedScopes)

        TestData.Users <- [[| credentials.AdeleV.Name; credentials.AdeleV.Password |]]
        TestData.AzureActiveDirectoryApp <- azureActiveDirectoryAppB2B1
        TestData.Web1Client <- credentials.WebClient1

        siloClientProvider.SiloClient <- SiloClient.getClusterClient()

module CurrentAssembly =
    [<Literal>]
    let TypeName = "Initialization.Starter"
    [<Literal>]
    let Name = "Authzi.Tests.MicrosoftOrleans.MicrosoftEntra.MicrosoftEntraID.NET8.0"

[<assembly: Xunit.TestFramework(CurrentAssembly.TypeName, CurrentAssembly.Name)>]
()