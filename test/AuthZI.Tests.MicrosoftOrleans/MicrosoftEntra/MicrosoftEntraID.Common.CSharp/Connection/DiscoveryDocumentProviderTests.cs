using System.Threading.Tasks;
using AuthZI.MicrosoftEntra;
using Microsoft.FSharp.Core;
using Xunit;

namespace AuthZI.Tests.MicrosoftOrleans.MicrosoftEntra.MicrosoftEntraID.Common;

public class DiscoveryDocumentProviderTestsBase(ITestOutputHelper output)
{
  [Fact]
  public async Task TheSystemCanObtainDiscoveryDocumentFromMicrosoftEntraIDEndpoint()
  {
    var discoveryDocumentProvider = new DiscoveryDocumentProvider(TestData.Web1ClientApp.DiscoveryEndpointUrl);

    var discoveryDocument = await discoveryDocumentProvider.GetDiscoveryDocumentAsync();
    var hasDiscoveryDocument = FSharpOption<DiscoveryDocument>.get_IsSome(discoveryDocument);

    if (hasDiscoveryDocument)
    {
      output.WriteLine(discoveryDocument.Value.DiscoveryEndpoint);
    }

    Assert.True(hasDiscoveryDocument);
  }
}
