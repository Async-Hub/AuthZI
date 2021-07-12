using Authzi.IdentityServer4.AccessToken;
using Authzi.IdentityServer4.IntegrationTests.Configuration;
using IdentityModel.Client;
using System.Net.Http;
using System.Threading.Tasks;

namespace Authzi.IdentityServer4.IntegrationTests
{
    public class TestBase
    {
        private readonly HttpClient _identityServer4Client;

        protected DiscoveryDocument DiscoveryDocument { get; }

        protected TestBase()
        {
            var identityServer4 = TestIdentityServer4Builder.StartNew();
            _identityServer4Client = identityServer4.CreateClient();

            var discoveryResponse = _identityServer4Client.GetDiscoveryDocumentAsync().Result;

            DiscoveryDocument = new DiscoveryDocument
            {
                IntrospectionEndpoint = discoveryResponse.IntrospectionEndpoint,
                Issuer = discoveryResponse.Issuer,
                Keys = discoveryResponse.KeySet.Keys,
                TokenEndpoint = discoveryResponse.TokenEndpoint
            };
        }
        
        protected async Task<string> RequestClientCredentialsTokenAsync(string clientId, string clientSecret,
            string scope)
        {
            var response = await _identityServer4Client.RequestClientCredentialsTokenAsync(
                new ClientCredentialsTokenRequest()
                {
                    Address = DiscoveryDocument.TokenEndpoint,
                    Scope = scope,
                    ClientId = clientId,
                    ClientSecret = clientSecret
                });

            return response.AccessToken;
        }
    }
}
