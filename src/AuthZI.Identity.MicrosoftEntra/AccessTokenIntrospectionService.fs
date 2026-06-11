namespace AuthZI.Identity.MicrosoftEntra

open AuthZI.MicrosoftEntra
open AuthZI.Security
open AuthZI.Security.AccessToken
open Microsoft.Extensions.Logging
open Microsoft.IdentityModel.Tokens
open System.IdentityModel.Tokens.Jwt

type public AccessTokenIntrospectionService
  (
    microsoftEntraApp: MicrosoftEntraApp,
    discoveryDocumentProvider: DiscoveryDocumentProvider,
    claimTypeResolver: IClaimTypeResolver,
    logger: ILogger<AccessTokenIntrospectionService>
  ) =

  let verify
    (jwtToken: string)
    (app: MicrosoftEntraApp)
    (discoveryDocument: DiscoveryDocument)
    (claimTypeResolver: IClaimTypeResolver)
    =
    let parameters = TokenValidationParameters()
    parameters.ValidIssuer <- app.IssuerUrl
    parameters.ValidAudiences <- app.ValidAudiences
    parameters.IssuerSigningKeys <- discoveryDocument.SigningKeys
    parameters.NameClaimType <- claimTypeResolver.Resolve ClaimType.Name
    parameters.RoleClaimType <- claimTypeResolver.Resolve ClaimType.Role
    parameters.RequireSignedTokens <- true
    parameters.ValidateAudience <- true

    let handler = JwtSecurityTokenHandler()
    handler.InboundClaimTypeMap.Clear()
    let mutable validatedToken: SecurityToken = null
    let claimsPrincipal = handler.ValidateToken(jwtToken, parameters, &validatedToken)
    
    claimsPrincipal.Claims

  interface IAccessTokenIntrospectionService with
    member _.IntrospectTokenAsync accessToken =
      task {
        let! discoveryDocument = discoveryDocumentProvider.GetDiscoveryDocumentAsync()

        try
          let claims = verify accessToken microsoftEntraApp discoveryDocument.Value claimTypeResolver
          return Ok(claims)
        with
        | ex ->
          logger.LogError(ex, "Exception during access token introspection.")
          return Error(ex.Message)
      }