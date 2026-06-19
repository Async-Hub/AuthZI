using AuthZI.MicrosoftEntra;
using AuthZI.Tests.MicrosoftOrleans.MicrosoftEntra.MicrosoftEntraID.Common.Initialization;
using Microsoft.FSharp.Core;
using System.Threading.Tasks;
using Xunit;

namespace AuthZI.Tests.MicrosoftOrleans.MicrosoftEntra.MicrosoftEntraID.Common.Connection;

public class DiscoveryDocumentProviderTestsBase(ITestOutputHelper output)
{
  [Fact]
  public async Task TheSystemCanObtainDiscoveryDocumentFromMicrosoftEntraIdEndpoint()
  {
    var discoveryDocumentProvider = new DiscoveryDocumentProvider(TestData.Web1ClientApp.DiscoveryEndpointUrl);

    var discoveryDocument = await discoveryDocumentProvider.GetDiscoveryDocumentAsync();
    var hasDiscoveryDocument = FSharpOption<DiscoveryDocument>.get_IsSome(discoveryDocument);

    if (hasDiscoveryDocument)
    {
#if DEBUG
      output.WriteLine(discoveryDocument.Value.DiscoveryEndpoint);
#endif
    }

    Assert.True(hasDiscoveryDocument);
  }
}
