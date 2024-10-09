namespace Authzi.Tests.MicrosoftOrleans.DuendeSoftware.IdentityServer

open Authzi.Security
open Authzi.Tests.MicrosoftOrleans.DuendeSoftware.IdentityServer.GlobalConfig
open Authzi.Tests.MicrosoftOrleans.Grains.SimpleAuthorization
open System
open Xunit

open AccessTokenFactory

module SimpleAuthorizationTests =
    [<Theory>]
    [<InlineData("Bob", "Pass123$", "Api1 Orleans")>]
    let ``An authenticated user can invoke the grain method``
        (userName: string) (password: string) (scope: string) =
        task {
            // Arrange
            let! accessTokenResponse = getAccessTokenForUserOnWebClient1Async userName password scope

            let clusterClient = getClusterClient accessTokenResponse.AccessToken
            let simpleGrain = clusterClient.GetGrain<ISimpleGrain>(Guid.NewGuid())
            let! value = simpleGrain.GetWithAuthenticatedUser("Secret")

            Assert.True(value.Equals "Secret")
        }
    
    [<Theory>]
    [<InlineData("Alice", "Pass123$", "Api1")>]
    let ``An authenticated user on an unauthenticated client can't invoke the grain method``
        (userName: string) (password: string) (scope: string) =
        task {
            // Arrange
            let! accessTokenResponse = getAccessTokenForUserOnWebClient2Async userName password scope

            let clusterClient = getClusterClient accessTokenResponse.AccessToken
            let simpleGrain = clusterClient.GetGrain<ISimpleGrain>(Guid.NewGuid())
        
            // Act
            let action =
                task {
                    let! value = simpleGrain.GetValue()
                    return value
                }

            Assert.ThrowsAsync<AuthorizationException>(fun () -> action) |> ignore
        }
    
    [<Fact>]
    let ``An anonymous user can't invoke the grain method`` () =
        task {
            // Arrange
            let accessToken = String.Empty
            let userName = "Empty"
        
            let clusterClient = getClusterClient accessToken
            let simpleGrain = clusterClient.GetGrain<ISimpleGrain>(Guid.NewGuid())
        
            // Act
            let action =
                task {
                    let! value = simpleGrain.GetWithAuthenticatedUser(String.Empty)
                    return value
                }

            Assert.ThrowsAsync<AuthorizationException>(fun () -> action) |> ignore
        }

    [<Fact>]
    let ``An anonymous user can invoke a grain method with [AllowAnonymous] attribute`` () =
        async {
            // Arrange
            let accessToken = String.Empty

            let clusterClient = getClusterClient accessToken
            let simpleGrain = clusterClient.GetGrain<ISimpleGrain>(Guid.Empty)

            let! value = simpleGrain.GetWithAnonymousUser("Secret") |> Async.AwaitTask

            Assert.True(value.Equals "Secret")
        }