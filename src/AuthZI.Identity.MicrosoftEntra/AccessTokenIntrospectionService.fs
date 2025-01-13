namespace AuthZI.Identity.MicrosoftEntra

open AuthZI.MicrosoftEntra
open AuthZI.Security
open AuthZI.Security.AccessToken
open Microsoft.IdentityModel.Tokens
open System.Text
open System.IdentityModel.Tokens.Jwt
open Microsoft.Extensions.Logging

type public AccessTokenIntrospectionService
  (
    azureActiveDirectoryApp: MicrosoftEntraApp,
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
    parameters.ValidAudience <- app.ClientId
    parameters.IssuerSigningKeys <- discoveryDocument.SigningKeys
    parameters.NameClaimType <- claimTypeResolver.Resolve ClaimType.Name
    parameters.RoleClaimType <- claimTypeResolver.Resolve ClaimType.Role
    parameters.RequireSignedTokens <- true
    // TODO: Implement audience validation functionality
    parameters.ValidateAudience <- false
    parameters.IssuerSigningKey <- SymmetricSecurityKey(Encoding.ASCII.GetBytes(app.ClientSecret))

    let handler = JwtSecurityTokenHandler()
    handler.InboundClaimTypeMap.Clear()
    let mutable validatedToken: SecurityToken = null
    let claimsPrincipal = handler.ValidateToken(jwtToken, parameters, &validatedToken)
    claimsPrincipal.Claims

  interface IAccessTokenIntrospectionService with
    member _.IntrospectTokenAsync accessToken allowOfflineValidation =
      task {
        let! discoveryDocument = discoveryDocumentProvider.GetDiscoveryDocumentAsync()

        try
          let claims = verify accessToken azureActiveDirectoryApp discoveryDocument.Value claimTypeResolver
          return Ok(claims)
        with
        // TODO: Implement logging
        | :? System.Exception as ex -> 
          logger.LogError(ex, "Exception during token introspection.")
          return Error(ex.Message)
      }