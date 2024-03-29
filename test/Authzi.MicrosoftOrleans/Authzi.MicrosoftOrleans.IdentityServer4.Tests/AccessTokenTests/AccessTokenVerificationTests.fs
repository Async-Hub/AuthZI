﻿module AccessTokenVerificationTests

open Xunit
open Microsoft.IdentityModel.Tokens
open FluentAssertions;
open Authzi.IdentityServer4.AccessToken

[<Theory>]
[<InlineData(GlobalConfig.WebClient1, "Secret1", "Api1")>]
let ``Access token verification with valid scope should be passed``
    (clientId: string) (clientSecret: string) (scope: string) =
    async {
        // Arrange
        let! accessTokenResponse = IdSTokenFactory.getAccessTokenForClientAsync clientId clientSecret scope
                                   |> Async.AwaitTask

        // Act
        let claims =
            JwtSecurityTokenVerifier.Verify accessTokenResponse.AccessToken scope IdSInstance.discoveryDocument

        // Assert
        Assert.True(claims |> Seq.exists (fun c -> c.Type = "aud" && c.Value = scope))
    }

[<Theory>]
[<InlineData(GlobalConfig.WebClient1, "Secret1", "Api2")>]
let ``Access token verification with invalid scope should be failed``
    (clientId: string) (clientSecret: string) (scope: string) =
    async {
        // Arrange
        let! accessTokenResponse = IdSTokenFactory.getAccessTokenForClientAsync clientId clientSecret "Api1"
                                   |> Async.AwaitTask

        // Act
        let verify =
            fun () ->
                JwtSecurityTokenVerifier.Verify
                    accessTokenResponse.AccessToken scope IdSInstance.discoveryDocument |> ignore

        // Assert
        Assert.Throws<SecurityTokenInvalidAudienceException>(verify) |> ignore
    }
