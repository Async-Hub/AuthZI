namespace Authzi.MicrosoftOrleans.AzureActiveDirectory.Tests.Authorization

open Authzi.MicrosoftOrleans.AzureActiveDirectory.Tests
open Authzi.MicrosoftOrleans.Grains.RoleBasedAuthorization
open Authzi.Security
open RootConfiguration
open System.Threading.Tasks
open Xunit

type RoleBasedAuthorizationTestsBase() =
    [<Theory>]
    [<MemberData(nameof(TestData.UserWithScopeAdeleV), MemberType = typeof<TestData>)>] 
    member _.``Multiple roles as a comma separated list should work when the user has both roles``
        (userName: string) (password: string) (scope: string list) =
        async {
            // Arrange
            let! accessToken = AccessTokenProvider.getAccessTokenForUserOnWebClient1Async
                                           userName password |> Async.AwaitTask

            let clusterClient = getClusterClient accessToken
            let userGrain = clusterClient.GetGrain<IManagerGrain>(userName)
            let! value = userGrain.GetWithCommaSeparatedRoles("Secret") |> Async.AwaitTask

            Assert.True(value.Equals "Secret")
        }

    [<Theory>]
    [<MemberData(nameof(TestData.UserWithScopeAlexW), MemberType = typeof<TestData>)>] 
    member _.``Multiple roles as a comma separated list shouldn't work when the user has only one role``
        (userName: string) (password: string) (scope: string list) =
        async {
            // Arrange
            let! accessToken = AccessTokenProvider.getAccessTokenForUserOnWebClient1Async
                                           userName password |> Async.AwaitTask

            let clusterClient = getClusterClient accessToken
            let userGrain = clusterClient.GetGrain<IManagerGrain>(userName)

            // Act
            let action =
                async {
                    let! value = userGrain.GetWithCommaSeparatedRoles("Secret") |> Async.AwaitTask
                    return value } |> Async.StartAsTask :> Task

            Assert.ThrowsAsync<AuthorizationException>(fun () -> action) |> ignore
        }

    [<Theory>]
    [<MemberData(nameof(TestData.UserWithScopeAdeleV), MemberType = typeof<TestData>)>]
    member _.``Multiple roles applied as multiple attributes should work when the user has both roles``
        (userName: string) (password: string) (scope: string list) =
        async {
            // Arrange
            let! accessToken = AccessTokenProvider.getAccessTokenForUserOnWebClient1Async
                                           userName password |> Async.AwaitTask

            let clusterClient = getClusterClient accessToken
            let userGrain = clusterClient.GetGrain<IManagerGrain>(userName)
            let! value = userGrain.GetWithMultipleRoleAttributes("Secret") |> Async.AwaitTask

            Assert.True(value.Equals "Secret")
        }

    [<Theory>]
    [<MemberData(nameof(TestData.UserWithScopeAlexW), MemberType = typeof<TestData>)>]
    member _.``Multiple roles applied as multiple attributes shouldn't work when the user has only one role``
        (userName: string) (password: string) (scope: string list) =
        async {
            // Arrange
            let! accessToken = AccessTokenProvider.getAccessTokenForUserOnWebClient1Async
                                           userName password |> Async.AwaitTask

            let clusterClient = getClusterClient accessToken
            let userGrain = clusterClient.GetGrain<IManagerGrain>(userName)

            // Act
            let action =
                async {
                    let! value = userGrain.GetWithMultipleRoleAttributes("Secret") |> Async.AwaitTask
                    return value } |> Async.StartAsTask :> Task

            Assert.ThrowsAsync<AuthorizationException>(fun () -> action) |> ignore
        }
