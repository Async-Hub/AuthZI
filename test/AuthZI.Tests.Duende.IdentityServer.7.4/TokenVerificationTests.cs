using System.Threading.Tasks;
using AuthZI.Identity.Duende.IdentityServer;
using Xunit;

namespace AuthZI.Tests.Duende.IdentityServer;

public class TokenVerificationTests : TestBase
{
  [Theory]
  [InlineData("WebClient", "Secret", "Api1")]
  public async Task VerifyAccessToken_WithCorrectScope_ShouldBeSuccessful(string clientId,
      string clientSecret, string scope)
  {
    // Arrange
    var accessToken = await RequestClientCredentialsTokenAsync(clientId, clientSecret, scope);

    // Act
    var claims = JwtSecurityTokenVerifier.Verify(accessToken, scope, DiscoveryDocument);

    // Assert
    Assert.True(claims != null);
  }
}