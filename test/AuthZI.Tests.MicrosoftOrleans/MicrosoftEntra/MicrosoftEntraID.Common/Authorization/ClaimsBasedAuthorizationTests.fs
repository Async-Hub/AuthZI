namespace AuthZI.Tests.MicrosoftOrleans.MicrosoftEntra.MicrosoftEntraID.Common.Authorization

open AuthZI.Tests.MicrosoftOrleans.Grains.ClaimsBasedAuthorization
open AuthZI.Tests.MicrosoftOrleans.Grains.PolicyBasedAuthorization
open AuthZI.Security
open RootConfiguration
open System
open System.Threading.Tasks
open Xunit
open AuthZI.Tests.MicrosoftOrleans.MicrosoftEntra.MicrosoftEntraID.Common

type ClaimsBasedAuthorizationTestsBase() =
    [<Theory>]
    [<MemberData(nameof(TestData.UserWithScopeAdeleV), MemberType = typeof<TestData>)>]
    member _.``A user with an appropriate claim should have access to the method`` (userName: string) (password: string)
        (scope: string list) =
        async {
            // Arrange
            let! accessToken = AccessTokenProvider.getAccessTokenForUserOnWebClient1Async
                                           userName password |> Async.AwaitTask

            // Act
            let clusterClient = getClusterClient accessToken
            let claimGrain = clusterClient.GetGrain<IClaimGrain>(userName)
            let! value = claimGrain.DoSomething("Secret") |> Async.AwaitTask

            Assert.True(value.Equals "Secret")
        }

    [<Theory>]
    [<MemberData(nameof(TestData.UserWithScopeAlexW), MemberType = typeof<TestData>)>]
    member _.``A user without an appropriate claim shouldn't have access to the method`` (userName: string) (password: string)
        (scope: string list) =
        async {
            // Arrange
            let! accessToken = AccessTokenProvider.getAccessTokenForUserOnWebClient1Async
                                           userName password |> Async.AwaitTask

            let clusterClient = getClusterClient accessToken
            let userGrain = clusterClient.GetGrain<IPolicyGrain>(userName)

            // Act
            let action =
                async {
                    let! value = userGrain.GetWithMangerPolicy(String.Empty) |> Async.AwaitTask
                    return value } |> Async.StartAsTask :> Task

            Assert.ThrowsAsync<AuthorizationException>(fun () -> action) |> ignore
        }

    [<Theory>]
    [<MemberData(nameof(TestData.UserWithScopeAlexW), MemberType = typeof<TestData>)>]
    member _.``A user with an appropriate claim and without an appropriate claim value shouldn't have access to the method``
        (userName: string) (password: string) (scope: string list) =
        async {
            // Arrange
            let! accessToken = AccessTokenProvider.getAccessTokenForUserOnWebClient1Async
                                           userName password |> Async.AwaitTask

            // Act
            let action =
                async {
                    let clusterClient = getClusterClient accessToken
                    let claimGrain = clusterClient.GetGrain<IClaimGrain>(userName)
                    let! value = claimGrain.DoSomething("Secret") |> Async.AwaitTask
                    return value
                } |> Async.StartAsTask :> Task


            Assert.ThrowsAsync<AuthorizationException>(fun () -> action) |> ignore
        }
