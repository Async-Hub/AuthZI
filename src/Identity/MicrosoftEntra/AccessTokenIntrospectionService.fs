namespace Authzi.MicrosoftEntra

open Authzi.Security
open Authzi.Security.AccessToken
open Microsoft.IdentityModel.Tokens
open System.IdentityModel.Tokens.Jwt
open System.Text

type public AccessTokenIntrospectionService(azureActiveDirectoryApp: AzureActiveDirectoryApp,
    discoveryDocumentProvider: DiscoveryDocumentProvider, claimTypeResolver : IClaimTypeResolver) =
    
    let verify (jwtToken: string) (app: AzureActiveDirectoryApp) (discoveryDocument: DiscoveryDocument)
           (claimTypeResolver: IClaimTypeResolver) =
               let parameters = TokenValidationParameters()
               parameters.ValidIssuer <- Configuration.issuerUrl app.DirectoryId
               parameters.ValidAudience <- app.ClientId
               parameters.IssuerSigningKeys <- discoveryDocument.SigningKeys
               parameters.NameClaimType <- claimTypeResolver.Resolve ClaimType.Name
               parameters.RoleClaimType <- claimTypeResolver.Resolve ClaimType.Role
               parameters.RequireSignedTokens <- true
               parameters.ValidateAudience <- false
               parameters.IssuerSigningKey <- SymmetricSecurityKey(Encoding.ASCII.GetBytes(app.ClientSecret))
           
               let handler = JwtSecurityTokenHandler()
               handler.InboundClaimTypeMap.Clear()
               let mutable validatedToken : SecurityToken = null
               let claimsPrincipal = handler.ValidateToken(jwtToken, parameters, &validatedToken)
               claimsPrincipal.Claims

    interface IAccessTokenIntrospectionService with
        member _.IntrospectTokenAsync accessToken allowOfflineValidation =
          async {
              let accessTokenType = AccessTokenAnalyzer.GetTokenType accessToken
              let! discoveryDocument = discoveryDocumentProvider.GetDiscoveryDocumentAsync() |> Async.AwaitTask
              
              let claims = verify accessToken azureActiveDirectoryApp discoveryDocument.Value claimTypeResolver

              return AccessTokenIntrospectionResult(accessTokenType, claims, true, "")
          } |> Async.StartAsTask
        end