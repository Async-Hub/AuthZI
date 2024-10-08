namespace Authzi.Tests.MicrosoftOrleans.DuendeSoftware.IdentityServer

open Authzi.Security
open Authzi.Tests.MicrosoftOrleans.DuendeSoftware.IdentityServer.GlobalConfig
open Authzi.Tests.MicrosoftOrleans.Grains.RoleBasedAuthorization
open FluentAssertions
open System.Threading.Tasks
open Xunit

module RoleBasedAuthorizationTests =
    [<Theory>]
    [<InlineData("Carol", "Pass123$", "Api1 Orleans")>]
    let ``Multiple roles as a comma separated list should work when the user has both roles``
        (userName: string) (password: string) (scope: string) =
        async {
            // Arrange
            let! accessTokenResponse = AccessTokenFactory.getAccessTokenForUserOnWebClient1Async
                                           userName password scope |> Async.AwaitTask

            let clusterClient = getClusterClient accessTokenResponse.AccessToken
            let userGrain = clusterClient.GetGrain<IManagerGrain>(userName)
            let! value = userGrain.GetWithCommaSeparatedRoles("Secret") |> Async.AwaitTask

            Assert.True(value.Equals "Secret")
        }

    [<Theory>]
    [<InlineData("Alice", "Pass123$", "Api1 Orleans")>]
    let ``Multiple roles as a comma separated list shouldn't work when the user has only one role``
        (userName: string) (password: string) (scope: string) =
        async {
            // Arrange
            let! accessTokenResponse = AccessTokenFactory.getAccessTokenForUserOnWebClient1Async
                                           userName password scope |> Async.AwaitTask

            let clusterClient = getClusterClient accessTokenResponse.AccessToken
            let userGrain = clusterClient.GetGrain<IManagerGrain>(userName)

            // Act
            let action =
                async {
                    let! value = userGrain.GetWithCommaSeparatedRoles("Secret") |> Async.AwaitTask
                    return value } |> Async.StartAsTask :> Task

            Assert.ThrowsAsync<AuthorizationException>(fun () -> action) |> ignore
        }

    [<Theory>]
    [<InlineData("Carol", "Pass123$", "Api1 Orleans")>]
    let ``Multiple roles applied as multiple attributes should work when the user has both roles``
        (userName: string) (password: string) (scope: string) =
        async {
            // Arrange
            let! accessTokenResponse = AccessTokenFactory.getAccessTokenForUserOnWebClient1Async
                                           userName password scope |> Async.AwaitTask

            let clusterClient = getClusterClient accessTokenResponse.AccessToken
            let userGrain = clusterClient.GetGrain<IManagerGrain>(userName)
            let! value = userGrain.GetWithMultipleRoleAttributes("Secret") |> Async.AwaitTask

            Assert.True(value.Equals "Secret")
        }

    [<Theory>]
    [<InlineData("Alice", "Pass123$", "Api1 Orleans")>]
    let ``Multiple roles applied as multiple attributes shouldn't work when the user has only one role``
        (userName: string) (password: string) (scope: string) =
        async {
            // Arrange
            let! accessTokenResponse = AccessTokenFactory.getAccessTokenForUserOnWebClient1Async
                                           userName password scope |> Async.AwaitTask

            let clusterClient = getClusterClient accessTokenResponse.AccessToken
            let userGrain = clusterClient.GetGrain<IManagerGrain>(userName)

            // Act
            let action =
                async {
                    let! value = userGrain.GetWithMultipleRoleAttributes("Secret") |> Async.AwaitTask
                    return value } |> Async.StartAsTask :> Task

            Assert.ThrowsAsync<AuthorizationException>(fun () -> action) |> ignore
        }