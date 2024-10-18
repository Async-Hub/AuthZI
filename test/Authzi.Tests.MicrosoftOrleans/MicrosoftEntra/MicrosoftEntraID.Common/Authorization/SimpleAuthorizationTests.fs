namespace AuthZI.Tests.MicrosoftOrleans.MicrosoftEntra.MicrosoftEntraID.Common.Authorization

open AuthZI.Tests.MicrosoftOrleans.MicrosoftEntra.MicrosoftEntraID.Common
open AuthZI.Tests.MicrosoftOrleans.Grains.SimpleAuthorization
open AuthZI.Security
open RootConfiguration
open System
open System.Threading.Tasks
open Xunit

type SimpleAuthorizationTestsBase() =
    [<Theory>]
    [<MemberData(nameof(TestData.UserWithScopeAdeleV), MemberType = typeof<TestData>)>] 
    member _.``An authenticated user can invoke the grain method``
        (userName: string) (password: string) (scope: string list) =
        async {
            // Arrange
            let! accessToken = AccessTokenProvider.getAccessTokenForUserOnWebClient1Async
                                           userName password |> Async.AwaitTask

            let clusterClient = getClusterClient accessToken
            let simpleGrain = clusterClient.GetGrain<ISimpleGrain>(Guid.NewGuid())
            let! value = simpleGrain.GetWithAuthenticatedUser("Secret") |> Async.AwaitTask

            Assert.True(value.Equals "Secret")
        }
    
    //[<Theory>]
    //[<InlineData("Alice", "Pass123$", "Api1")>]
    //member _.``An authenticated user on an unauthenticated client can't invoke the grain method``
    //    (userName: string) (password: string) (scope: string) =
    //    async {
    //        // Arrange
    //        let! accessTokenResponse = IdSTokenFactory.getAccessTokenForUserOnWebClient2Async
    //                                       userName password scope |> Async.AwaitTask

    //        let clusterClient = SiloClient.getClusterClient accessTokenResponse.AccessToken
    //        let simpleGrain = clusterClient.GetGrain<ISimpleGrain>(Guid.NewGuid())
        
    //        // Act
    //        let action =
    //            async{
    //                let! value = simpleGrain.GetValue() |> Async.AwaitTask
    //            return value } |> Async.StartAsTask :> Task

    //        Assert.ThrowsAsync<NotAuthorizedException>(fun () -> action) |> ignore
    //    }
    
    [<Fact>]
    member _.``An anonymous user can't invoke the grain method`` () =
        async {
            // Arrange
            let accessToken = String.Empty
            let userName = "Empty"
        
            let clusterClient = getClusterClient accessToken
            let simpleGrain = clusterClient.GetGrain<ISimpleGrain>(Guid.NewGuid())
        
            // Act
            let action =
                async {
                    let! value = simpleGrain.GetWithAuthenticatedUser(String.Empty) |> Async.AwaitTask
                    return value } |> Async.StartAsTask :> Task

            Assert.ThrowsAsync<AuthorizationException>(fun () -> action) |> ignore
        }

    [<Fact>]
    member _.``An anonymous user can invoke a grain method with [AllowAnonymous] attribyte`` () =
        async {
            // Arrange
            let accessToken = String.Empty

            let clusterClient = getClusterClient accessToken
            let simpleGrain = clusterClient.GetGrain<ISimpleGrain>(Guid.Empty)

            let! value = simpleGrain.GetWithAnonymousUser("Secret") |> Async.AwaitTask

            Assert.True(value.Equals "Secret")
        }
