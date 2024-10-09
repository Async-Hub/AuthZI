namespace Authzi.Tests.MicrosoftOrleans.DuendeSoftware.IdentityServer

open Authzi.Security
open Authzi.Tests.MicrosoftOrleans.DuendeSoftware.IdentityServer.GlobalConfig
open Authzi.Tests.MicrosoftOrleans.Grains.ClaimsBasedAuthorization
open Authzi.Tests.MicrosoftOrleans.Grains.PolicyBasedAuthorization
open System
open Xunit

open AccessTokenFactory

module ClaimsBasedAuthorizationTests =
    [<Theory>]
    [<InlineData("Alice", "Pass123$", "Api1 Orleans")>]
    let ``A user with an appropriate claim should have access to the method``
        (userName: string) (password: string) (scope: string) =
        task {
            // Arrange
            let! accessTokenResponse = getAccessTokenForUserOnWebClient1Async userName password scope

            // Act
            let clusterClient = getClusterClient accessTokenResponse.AccessToken
            let claimGrain = clusterClient.GetGrain<IClaimGrain>(userName)
            let! value = claimGrain.DoSomething("Secret")

            Assert.True(value.Equals "Secret")
        }

    [<Theory>]
    [<InlineData("Bob", "Pass123$", "Api1 Orleans")>]
    let ``A user without an appropriate claim shouldn't have access to the method``
        (userName: string) (password: string) (scope: string) =
        task {
            // Arrange
            let! accessTokenResponse = getAccessTokenForUserOnWebClient1Async userName password scope

            let clusterClient = getClusterClient accessTokenResponse.AccessToken
            let userGrain = clusterClient.GetGrain<IPolicyGrain>(userName)

            // Act
            let action =
                task {
                    let! value = userGrain.GetWithMangerPolicy(String.Empty)
                    return value
                }

            Assert.ThrowsAsync<AuthorizationException>(fun () -> action) |> ignore
        }

    [<Theory>]
    [<InlineData("Carol", "Pass123$", "Api1 Orleans")>]
    let ``A user with an appropriate claim and without an appropriate claim value shouldn't have access to the method``
        (userName: string) (password: string) (scope: string) =
        task {
            // Arrange
            let! accessTokenResponse = getAccessTokenForUserOnWebClient1Async userName password scope

            // Act
            let action =
                task {
                    let clusterClient = getClusterClient accessTokenResponse.AccessToken
                    let claimGrain = clusterClient.GetGrain<IClaimGrain>(userName)
                    let! value = claimGrain.DoSomething("Secret")
                    return value
                }

            Assert.ThrowsAsync<AuthorizationException>(fun () -> action) |> ignore
        }