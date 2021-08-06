namespace Authzi.AzureActiveDirectory

open Authzi.Security.AccessToken
open Microsoft.Extensions.Logging
open System.Net.Http

type public AccessTokenIntrospectionService(httpClientFactory: IHttpClientFactory,
    discoveryDocumentProvider:DiscoveryDocumentProvider,
    logger: ILogger<AccessTokenIntrospectionService>) =
    
    interface IAccessTokenIntrospectionService with
        member this.IntrospectTokenAsync accessToken allowOfflineValidation =
          async {
              let accessTokenType = AccessTokenAnalyzer.GetTokenType accessToken
              let! discoveryResponse = discoveryDocumentProvider.GetDiscoveryDocumentAsync() |> Async.AwaitTask
              
              let claims = []
              
              // TODO: Implement Access Token validation here.

              return AccessTokenIntrospectionResult(accessTokenType, claims, true, "")
          } |> Async.StartAsTask
        end