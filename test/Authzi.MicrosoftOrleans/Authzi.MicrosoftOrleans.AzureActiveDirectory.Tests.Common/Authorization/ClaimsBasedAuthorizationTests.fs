namespace Authzi.MicrosoftOrleans.AzureActiveDirectory.Tests.Authorization

open Authzi.MicrosoftOrleans.Grains.ClaimsBasedAuthorization
open Authzi.MicrosoftOrleans.Grains.PolicyBasedAuthorization
open Authzi.Security
open Credentials.Users
open RootConfiguration
open System
open System.Threading.Tasks
open Xunit

type ClaimsBasedAuthorizationTestsBase() =
    [<Theory>]
    [<InlineData(AdeleVB2B1, GeneralPassword, "Api1 Orleans")>]
    member _.``A user with an appropriate claim should have access to the method`` (userName: string) (password: string)
        (scope: string) =
        async {
            // Arrange
            let! accessToken = AccessTokenProvider.getAccessTokenForUserOnB2BWebClient1Async
                                           userName password |> Async.AwaitTask

            // Act
            let clusterClient = getClusterClient accessToken
            let claimGrain = clusterClient.GetGrain<IClaimGrain>(userName)
            let! value = claimGrain.DoSomething("Secret") |> Async.AwaitTask

            Assert.True(value.Equals "Secret")
        }

    [<Theory>]
    [<InlineData(AlexWB2B1, GeneralPassword, "Api1 Orleans")>]
    member _.``A user without an appropriate claim shouldn't have access to the method`` (userName: string) (password: string)
        (scope: string) =
        async {
            // Arrange
            let! accessToken = AccessTokenProvider.getAccessTokenForUserOnB2BWebClient1Async
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
    [<InlineData(AlexWB2B1, GeneralPassword, "Api1 Orleans")>]
    member _.``A user with an appropriate claim and without an appropriate claim value shouldn't have access to the method``
        (userName: string) (password: string) (scope: string) =
        async {
            // Arrange
            let! accessToken = AccessTokenProvider.getAccessTokenForUserOnB2BWebClient1Async
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
