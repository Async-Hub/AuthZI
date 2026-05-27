namespace Initialization

open AuthZI.MicrosoftEntra
open AuthZI.Deploy.MicrosoftEntra.Configuration
open AuthZI.Tests.MicrosoftOrleans.MicrosoftEntra.MicrosoftEntraID.Common
open RootConfiguration
open System
open System.Text.Json

[<assembly: Orleans.ApplicationPartAttribute("AuthZI.Tests.MicrosoftOrleans.Grains")>]
()

type Starter() =
    do 
        // Read the configuration.
        let mutable credentialsJson = Environment.GetEnvironmentVariable("microsoftAzureADB2CCredentials")

        if String.IsNullOrWhiteSpace(credentialsJson) then
            credentialsJson <- Literals.azureADB2C1Json

        let credentials = JsonSerializer.Deserialize<MicrosoftEntraCredentials>(credentialsJson)

        // Initialize the test data.
        //let azureActiveDirectoryAppB2C1 =
        //    AzureActiveDirectoryApp(credentials.DirectoryId, credentials.WebClient1.Id, 
        //        credentials.WebClient1.Secret, true, credentials.WebClient1.AllowedScopes)

        TestData.UserWithScopeAdeleV <- [[|credentials.AdeleV.Name; credentials.AdeleV.Password; ["Api1"; "Orleans"]|]]
        TestData.UserWithScopeAlexW <- [[|credentials.AlexW.Name; credentials.AlexW.Password; ["Api1"; "Orleans"]|]]
        TestData.Users <- [[| credentials.AdeleV.Name; credentials.AdeleV.Password |]]
        //TestData.AzureActiveDirectoryApp <- azureActiveDirectoryAppB2C1

        //TestData.SiloClient <- SiloClient.getClusterClient()

[<assembly: Xunit.AssemblyFixture(typeof<Starter>)>]
()