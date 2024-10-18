namespace AuthZI.Tests.MicrosoftOrleans.DuendeSoftware.IdentityServer

open AuthZI.Identity.DuendeSoftware.IdentityServer
open AuthZI.Tests.MicrosoftOrleans.DuendeSoftware.IdentityServer.GlobalConfig
open Microsoft.IdentityModel.Tokens
open Xunit

module AccessTokenVerificationTests =
    [<Theory>]
    [<InlineData(GlobalConfig.WebClient1, "Secret1", "Api1")>]
    let ``Access token verification with valid scope should be passed``
        (clientId: string) (clientSecret: string) (scope: string) =
        task {
            // Arrange
            let! accessTokenResponse =
                AccessTokenFactory.getAccessTokenForClientAsync clientId clientSecret scope
                                 
            // Act
            let claims =
                JwtSecurityTokenVerifier.Verify accessTokenResponse.AccessToken scope discoveryDocument

            // Assert
            Assert.True(claims |> Seq.exists (fun c -> c.Type = "aud" && c.Value = scope))
        }

    [<Theory>]
    [<InlineData(GlobalConfig.WebClient1, "Secret1", "Api2")>]
    let ``Access token verification with invalid scope should be failed``
        (clientId: string) (clientSecret: string) (scope: string) =
        task {
            // Arrange
            let! accessTokenResponse =
                AccessTokenFactory.getAccessTokenForClientAsync clientId clientSecret "Api1"

            // Act
            let verify =
                fun () ->
                    JwtSecurityTokenVerifier.Verify
                        accessTokenResponse.AccessToken scope discoveryDocument |> ignore

            // Assert
            Assert.Throws<SecurityTokenInvalidAudienceException>(verify) |> ignore
        }