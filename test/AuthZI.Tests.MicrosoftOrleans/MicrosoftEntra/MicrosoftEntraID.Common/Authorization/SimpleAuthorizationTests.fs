namespace AuthZI.Tests.MicrosoftOrleans.MicrosoftEntra.MicrosoftEntraID.Common.Authorization

open AuthZI.Tests.MicrosoftOrleans.MicrosoftEntra.MicrosoftEntraID.Common
open AuthZI.Tests.MicrosoftOrleans.Grains.SimpleAuthorization
open AuthZI.Extensions.TaskExtensions
open AuthZI.Security
open RootConfiguration
open System
open System.Security
open System.Threading.Tasks
open Xunit
open AuthZI.MicrosoftOrleans.Authorization

type SimpleAuthorizationTestsBase() =
  [<Theory>]
  [<MemberData(nameof (TestData.UserWithScopeAdeleV), MemberType = typeof<TestData>)>]
  member _.``An authenticated user can invoke the grain method``
    (userName: string)
    (password: string)
    (scope: string list)
    =
    task {
      // Arrange
      let! accessToken = AccessTokenProvider.getAccessTokenForUserOnWebClient1Async userName password

      let clusterClient = getClusterClient accessToken
      let simpleGrain = clusterClient.GetGrain<ISimpleGrain>(Guid.NewGuid())
      let! value = simpleGrain.GetWithAuthenticatedUser("Secret")

      Assert.True(value.Equals "Secret")
    }

  [<Theory>]
  [<MemberData(nameof (TestData.UserWithScopeAdeleV), MemberType = typeof<TestData>)>]
  member _.``An authenticated user on an unauthenticated client can't invoke the grain method``
    (userName: string)
    (password: string)
    (scope: string list)
    =
    task {
      // Arrange
      let! accessToken = AccessTokenProvider.getAccessTokenForUserOnWebClient2Async userName password

      let clusterClient = getClusterClient accessToken
      let simpleGrain = clusterClient.GetGrain<ISimpleGrain>(Guid.NewGuid())

      // Act
      let action =
        task {
          let! value = simpleGrain.GetValue()
          return value
        }

      let! result = Assert.ThrowsAsync<SecurityException>(fun () -> action)
      Assert.True(result.Message = SecureGrain.AccessDeniedMessage)
    }

  [<Fact>]
  member _.``An anonymous user can't invoke the grain method``() =
    task {
      // Arrange
      let accessToken = String.Empty

      let clusterClient = getClusterClient accessToken
      let simpleGrain = clusterClient.GetGrain<ISimpleGrain>(Guid.NewGuid())

      // Act
      let action =
        task {
          let! value = simpleGrain.GetWithAuthenticatedUser(String.Empty)
          return value
        }

      let! result = Assert.ThrowsAsync<InvalidOperationException>(fun () -> action)
      Assert.True(true)
    }

  [<Fact>]
  member _.``An anonymous user can invoke a grain method with [AllowAnonymous] attribyte``() =
    async {
      // Arrange
      let accessToken = String.Empty

      let clusterClient = getClusterClient accessToken
      let simpleGrain = clusterClient.GetGrain<ISimpleGrain>(Guid.Empty)

      let! value = simpleGrain.GetWithAnonymousUser("Secret") |> Async.AwaitTask

      Assert.True(value.Equals "Secret")
    }