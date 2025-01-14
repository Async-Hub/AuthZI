namespace AuthZI.Tests.MicrosoftOrleans.MicrosoftEntra.MicrosoftEntraID.Common

open AuthZI.MicrosoftEntra
open Xunit
open Xunit.Abstractions

type DiscoveryDocumentProviderTestsBase(output: ITestOutputHelper) =
  [<Fact>]
  member _.``The system can obtain Discovery Document from Microsoft Entra ID endpoint``() =
    async {
      // Arrange
      let discoveryDocumentProvider = DiscoveryDocumentProvider(TestData.Web1ClientApp.DiscoveryEndpointUrl)

      let! discoveryDocument = discoveryDocumentProvider.GetDiscoveryDocumentAsync() |> Async.AwaitTask

      if discoveryDocument.IsSome then
        output.WriteLine(discoveryDocument.Value.DiscoveryEndpoint)

      // Act
      Assert.True(discoveryDocument.IsSome)
    }