namespace Authzi.Tests.MicrosoftOrleans.MicrosoftEntra.MicrosoftEntraID.Common

open Authzi.MicrosoftEntra
open Xunit
open Xunit.Abstractions

type DiscoveryDocumentProviderTests(output: ITestOutputHelper) =
    [<Fact>]
    member _.``The system can obtain Discovery Document from Azure AD endpoint`` () =
        async {
            // Arrange
            let discoveryDocumentProvider = 
                DiscoveryDocumentProvider(TestData.AzureActiveDirectoryApp.DiscoveryEndpointUrl)


            let! discoveryDocument = discoveryDocumentProvider.GetDiscoveryDocumentAsync() |> Async.AwaitTask
            
            if discoveryDocument.IsSome then
                output.WriteLine(discoveryDocument.Value.DiscoveryEndpoint)
            
            // Act
            Assert.True(discoveryDocument.IsSome)
         }

