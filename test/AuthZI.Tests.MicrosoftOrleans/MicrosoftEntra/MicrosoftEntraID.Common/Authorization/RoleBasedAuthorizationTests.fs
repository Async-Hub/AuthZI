namespace AuthZI.Tests.MicrosoftOrleans.MicrosoftEntra.MicrosoftEntraID.Common.Authorization

open AuthZI.Tests.MicrosoftOrleans.MicrosoftEntra.MicrosoftEntraID.Common
open AuthZI.Tests.MicrosoftOrleans.Grains.RoleBasedAuthorization
open AuthZI.Security
open RootConfiguration
open Xunit

type RoleBasedAuthorizationTestsBase() =
  [<Theory>]
  [<MemberData(nameof (TestData.UserWithScopeAdeleV), MemberType = typeof<TestData>)>]
  member _.``Multiple roles as a comma separated list should work when the user has both roles``
    (userName: string)
    (password: string)
    (scope: string list)
    =
    task {
      // Arrange
      let! accessToken = AccessTokenProvider.getAccessTokenForUserOnWebClient1Async userName password

      let clusterClient = getClusterClient accessToken
      let userGrain = clusterClient.GetGrain<IManagerGrain>(userName)
      let! value = userGrain.GetWithCommaSeparatedRoles("Secret")

      Assert.True(value.Equals "Secret")
    }

  [<Theory>]
  [<MemberData(nameof (TestData.UserWithScopeAlexW), MemberType = typeof<TestData>)>]
  member _.``Multiple roles as a comma separated list shouldn't work when the user has only one role``
    (userName: string)
    (password: string)
    (scope: string list)
    =
    task {
      // Arrange
      let! accessToken = AccessTokenProvider.getAccessTokenForUserOnWebClient1Async userName password

      let clusterClient = getClusterClient accessToken
      let userGrain = clusterClient.GetGrain<IManagerGrain>(userName)

      // Act
      let action =
        task {
          let! value = userGrain.GetWithCommaSeparatedRoles("Secret")
          return value
        }

      Assert.ThrowsAsync<AuthorizationException>(fun () -> action) |> ignore
    }

  [<Theory>]
  [<MemberData(nameof (TestData.UserWithScopeAdeleV), MemberType = typeof<TestData>)>]
  member _.``Multiple roles applied as multiple attributes should work when the user has both roles``
    (userName: string)
    (password: string)
    (scope: string list)
    =
    task {
      // Arrange
      let! accessToken = AccessTokenProvider.getAccessTokenForUserOnWebClient1Async userName password

      let clusterClient = getClusterClient accessToken
      let userGrain = clusterClient.GetGrain<IManagerGrain>(userName)
      let! value = userGrain.GetWithMultipleRoleAttributes("Secret")

      Assert.True(value.Equals "Secret")
    }

  [<Theory>]
  [<MemberData(nameof (TestData.UserWithScopeAlexW), MemberType = typeof<TestData>)>]
  member _.``Multiple roles applied as multiple attributes shouldn't work when the user has only one role``
    (userName: string)
    (password: string)
    (scope: string list)
    =
    task {
      // Arrange
      let! accessToken = AccessTokenProvider.getAccessTokenForUserOnWebClient1Async userName password

      let clusterClient = getClusterClient accessToken
      let userGrain = clusterClient.GetGrain<IManagerGrain>(userName)

      // Act
      let action =
        task {
          let! value = userGrain.GetWithMultipleRoleAttributes("Secret")
          return value
        }

      Assert.ThrowsAsync<AuthorizationException>(fun () -> action) |> ignore
    }