namespace AuthZI.Identity.Duende.IdentityServer

open AuthZI.Security.AccessToken
open Duende.IdentityModel.Client
open Microsoft.Extensions.Logging
open System.Net.Http

type public AccessTokenIntrospectionService
  (
    httpClientFactory: IHttpClientFactory,
    identityServerConfig: IdentityServerConfig,
    discoveryDocumentProvider: DiscoveryDocumentProvider,
    accessTokenVerifierOptions: AccessTokenVerifierOptions,
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
        logger.LogWarning(
          "Access token introspection failed for {TokenType} token. Error: {Error}",
          nameOfTokenType,
          introspectionResponse.Error
        )
        return Error("Access token introspection failed.")
    }
    |> Async.StartAsTask

  interface IAccessTokenIntrospectionService with
    member this.IntrospectTokenAsync accessToken =
      async {
        // Read AllowOfflineValidation from configuration options.
        let allowOfflineValidation = accessTokenVerifierOptions.AllowOfflineValidation
        let accessTokenType = AccessTokenAnalyzer.GetTokenType accessToken
        let! discoveryResponse = discoveryDocumentProvider.GetDiscoveryDocumentAsync() |> Async.AwaitTask

        if accessTokenType = AccessTokenType.Jwt && allowOfflineValidation then
          let claims = JwtSecurityTokenVerifier.Verify accessToken identityServerConfig.Audience discoveryResponse

          return Ok(claims)
        else
          let! res = this.IntrospectTokenOnlineAsync accessToken accessTokenType discoveryResponse |> Async.AwaitTask

          return res
      }
      |> Async.StartAsTask
