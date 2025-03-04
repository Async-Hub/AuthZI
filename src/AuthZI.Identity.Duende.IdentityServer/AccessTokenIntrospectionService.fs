namespace AuthZI.Identity.Duende.IdentityServer

open AuthZI.Security.AccessToken
open IdentityModel.Client
open Microsoft.Extensions.Logging
open System.Net.Http

type public AccessTokenIntrospectionService
    (
        httpClientFactory: IHttpClientFactory,
        identityServerConfig: IdentityServerConfig,
        discoveryDocumentProvider: DiscoveryDocumentProvider,
        logger: ILogger<AccessTokenIntrospectionService>
    ) =
    let httpClient = httpClientFactory.CreateClient "IdS"

    member private this.IntrospectTokenOnlineAsync
        (accessToken: string)
        (accessTokenType: AccessTokenType)
        (discoveryDocument: DiscoveryDocument)
        =
        async {
            let request = new TokenIntrospectionRequest()
            request.Address <- discoveryDocument.IntrospectionEndpoint
            request.ClientId <- identityServerConfig.ClientId
            request.Token <- accessToken
            request.ClientSecret <- identityServerConfig.ClientSecret

            let! introspectionResponse = httpClient.IntrospectTokenAsync request |> Async.AwaitTask

            let nameOfTokenType =
                match accessTokenType with
                | AccessTokenType.Jwt -> "JWT"
                | _ -> "Reference"

            if (not introspectionResponse.IsError) && introspectionResponse.IsActive then
                return Ok(introspectionResponse.Claims)
            else
                // TODO: Log trace.
                return Error("Access token introspection failed.")
        }
        |> Async.StartAsTask

    interface IAccessTokenIntrospectionService with
        member this.IntrospectTokenAsync accessToken allowOfflineValidation =
            async {
                let accessTokenType = AccessTokenAnalyzer.GetTokenType accessToken
                let! discoveryResponse = discoveryDocumentProvider.GetDiscoveryDocumentAsync() |> Async.AwaitTask

                if accessTokenType = AccessTokenType.Jwt && allowOfflineValidation then
                    let claims =
                        JwtSecurityTokenVerifier.Verify accessToken identityServerConfig.Audience discoveryResponse

                    return Ok(claims)
                else
                    let! res =
                        this.IntrospectTokenOnlineAsync accessToken accessTokenType discoveryResponse
                        |> Async.AwaitTask

                    return res
            }
            |> Async.StartAsTask

//using System;
//using System.Net.Http;
//using System.Threading;
//using System.Threading.Tasks;
//using IdentityModel.Client;
//using Microsoft.Extensions.Logging;

//namespace Orleans.Security.AccessToken
//{
//    internal class AccessTokenIntrospectionServiceDefault : IAccessTokenIntrospectionService
//    {
//        private readonly HttpClient _httpClient;
//        private readonly IdentityServer4Info _identityServer4Info;
//        private readonly IdS4DiscoveryDocumentProvider _discoveryDocumentProvider;
//        private readonly ILogger<AccessTokenIntrospectionServiceDefault> _logger;

//        public AccessTokenIntrospectionServiceDefault(IHttpClientFactory clientFactory,
//            IdentityServer4Info identityServer4Info,
//            IdS4DiscoveryDocumentProvider discoveryDocumentProvider,
//            ILogger<AccessTokenIntrospectionServiceDefault> logger)
//        {
//            _httpClient = clientFactory.CreateClient("IdS4");
//            _identityServer4Info = identityServer4Info;
//            _discoveryDocumentProvider = discoveryDocumentProvider;
//            _logger = logger;
//        }

//        public async Task<AccessTokenIntrospectionResult> IntrospectTokenAsync(string accessToken,
//            bool allowOfflineValidation = false)
//        {
//            var accessTokenType = AccessTokenAnalyzer.GetTokenType(accessToken);
//            var discoveryResponse = await _discoveryDocumentProvider.GetDiscoveryDocumentAsync();

//            if (accessTokenType == AccessTokenType.Jwt && allowOfflineValidation)
//            {
//                var claims = JwtSecurityTokenVerifier.Verify(accessToken,
//                    _identityServer4Info.AllowedScope, discoveryResponse);

//                return new AccessTokenIntrospectionResult(accessTokenType, claims, true);
//            }

//            var introspectionResult = await
//                IntrospectTokenOnlineAsync(accessToken, accessTokenType, discoveryResponse);

//            return introspectionResult;
//        }

//        private async Task<AccessTokenIntrospectionResult> IntrospectTokenOnlineAsync(string accessToken,
//            AccessTokenType accessTokenType, DiscoveryDocumentShortInfo discoveryDocument)
//        {

//            // TODO: This approach should be reconsidered in future.
//            /*
//            The solution below allows use "Orleans.Security" with IdentityServer4 v2.x and IdentityServer4 v3.x.
//            At the same time, DLR with Reflection is a bad idea.
//            */

//            const string fullyQualifiedNameOfType =
//                "IdentityModel.Client.HttpClientTokenIntrospectionExtensions, IdentityModel";

//            dynamic request =
//                Activator.CreateInstance(Type.GetType("IdentityModel.Client.TokenIntrospectionRequest, IdentityModel",
//                    true));

//            request.Address = discoveryDocument.IntrospectionEndpoint;
//            request.ClientId = _identityServer4Info.ClientId;
//            request.Token = accessToken;
//            request.ClientSecret = _identityServer4Info.ClientSecret;

//            var cancellationToken = default(CancellationToken);
//            var param = new object[] { _httpClient,request, cancellationToken };

//            var introspectionResponse =
//                await RuntimeMethodBinder.InvokeAsync(fullyQualifiedNameOfType,
//                    "IntrospectTokenAsync", param, 3);

//            // TODO: This should be used normally.

//            /*
//            var request = new TokenIntrospectionRequest
//            {
//                Address = discoveryDocument.IntrospectionEndpoint,
//                ClientId = _identityServer4Info.ClientId,
//                Token = accessToken,
//                ClientSecret = _identityServer4Info.ClientSecret,
//            };

//            var introspectionResponse = await _httpClient.IntrospectTokenAsync(request);
//            */

//            var nameOfTokenType = accessTokenType == AccessTokenType.Jwt ? "JWT" : "Reference";

//            // ReSharper disable once ConvertIfStatementToReturnStatement
//            if (!introspectionResponse.IsError)
//            {
//                return new AccessTokenIntrospectionResult(accessTokenType, introspectionResponse.Claims,
//                    introspectionResponse.IsActive, null);
//            }

//            _logger.LogTrace(LoggingEvents.AccessTokenValidationFailed,
//                $"{LoggingEvents.AccessTokenValidationFailed.Name} Token type: {nameOfTokenType} " +
//                $"Reason: {introspectionResponse.Error} " +
//                $"Token value: {accessToken}");

//            return new AccessTokenIntrospectionResult(accessTokenType, introspectionResponse.Claims, false, null);
//        }
//    }
//}
