namespace AuthZI.Tests.MicrosoftOrleans.MicrosoftEntra.MicrosoftEntraID.Common.Authorization

open AuthZI.Security
open AuthZI.Tests.MicrosoftOrleans.Grains.ClaimsBasedAuthorization
open AuthZI.Tests.MicrosoftOrleans.Grains.PolicyBasedAuthorization
open AuthZI.Tests.MicrosoftOrleans.MicrosoftEntra.MicrosoftEntraID.Common
open RootConfiguration
open System
open System.Security
open System.Threading.Tasks
open Xunit

type ClaimsBasedAuthorizationTestsBase() =
    [<Theory>]
    [<MemberData(nameof(TestData.UserWithScopeAdeleV), MemberType = typeof<TestData>, DisableDiscoveryEnumeration = true)>]
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
    [<MemberData(nameof(TestData.UserWithScopeAlexW), MemberType = typeof<TestData>, DisableDiscoveryEnumeration = true)>]
    member _.``A user without an appropriate claim shouldn't have access to the method`` (userName: string) (password: string)
        (scope: string list) =
        async {
            // Arrange
            let! accessToken = AccessTokenProvider.getAccessTokenForUserOnWebClient1Async
                                           userName password |> Async.AwaitTask

            let clusterClient = getClusterClient accessToken
            let userGrain = clusterClient.GetGrain<IPolicyGrain>(userName)
           
           // Act and Assert
            do! Assert.ThrowsAsync<SecurityException>(fun () ->
                          userGrain.GetWithMangerPolicy(String.Empty) :> Task)
                      |> Async.AwaitTask 
                      |> Async.Ignore
        }

    [<Theory>]
    [<MemberData(nameof(TestData.UserWithScopeAlexW), MemberType = typeof<TestData>, DisableDiscoveryEnumeration = true)>]
    member _.``A user with an appropriate claim and without an appropriate claim value shouldn't have access to the method``
        (userName: string) (password: string) (scope: string list) =
        async {
            // Arrange
            let! accessToken = AccessTokenProvider.getAccessTokenForUserOnWebClient1Async
                                           userName password |> Async.AwaitTask

            // Act
            let action =
                    let clusterClient = getClusterClient accessToken
                    let claimGrain = clusterClient.GetGrain<IClaimGrain>(userName)
                    claimGrain.DoSomething("Secret")


            do! Assert.ThrowsAsync<SecurityException>(fun () -> action) 
              |> Async.AwaitTask |> Async.Ignore
        }
