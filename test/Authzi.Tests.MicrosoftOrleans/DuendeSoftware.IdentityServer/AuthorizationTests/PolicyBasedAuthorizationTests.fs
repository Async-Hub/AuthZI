namespace Authzi.Tests.MicrosoftOrleans.DuendeSoftware.IdentityServer

open Authzi.Security
open Authzi.Tests.MicrosoftOrleans.DuendeSoftware.IdentityServer.GlobalConfig
open Authzi.Tests.MicrosoftOrleans.Grains.PolicyBasedAuthorization
open FluentAssertions
open System
open System.Threading.Tasks
open Xunit

module PolicyBasedAuthorizationTests =
    [<Theory>]
    [<InlineData("Alice", "Pass123$", "Api1 Orleans")>]
    let ``A user with an appropriate role should have access to the method``
        (userName: string) (password: string) (scope: string) =
        async {
            // Arrange
            let! accessTokenResponse = AccessTokenFactory.getAccessTokenForUserOnWebClient1Async
                                           userName password scope |> Async.AwaitTask

            let clusterClient = getClusterClient accessTokenResponse.AccessToken
            let userGrain = clusterClient.GetGrain<IPolicyGrain>(userName)
            let! value = userGrain.GetWithMangerPolicy("Secret") |> Async.AwaitTask

            Assert.True(value.Equals "Secret")
        }
    
    [<Theory>]
    [<InlineData("Bob", "Pass123$", "Api1 Orleans")>]
    let ``A user without an appropriate role shouldn't have access to the method``
        (userName: string) (password: string) (scope: string) =
        async {
            // Arrange
            let! accessTokenResponse = AccessTokenFactory.getAccessTokenForUserOnWebClient1Async
                                           userName password scope |> Async.AwaitTask

            let clusterClient = getClusterClient accessTokenResponse.AccessToken
            let userGrain = clusterClient.GetGrain<IPolicyGrain>(userName)
        
            // Act
            let action =
                async {
                    let! value = userGrain.GetWithMangerPolicy(String.Empty) |> Async.AwaitTask
                    return value } |> Async.StartAsTask :> Task

            Assert.ThrowsAsync<AuthorizationException>(fun () -> action) |> ignore
        }