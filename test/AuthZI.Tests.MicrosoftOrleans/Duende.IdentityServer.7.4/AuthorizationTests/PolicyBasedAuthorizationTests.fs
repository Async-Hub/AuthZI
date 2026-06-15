namespace AuthZI.Tests.MicrosoftOrleans.Duende.IdentityServer

open AuthZI.Security
open AuthZI.Tests.MicrosoftOrleans.Duende.IdentityServer.GlobalConfig
open AuthZI.Tests.MicrosoftOrleans.Grains.PolicyBasedAuthorization
open System
open Xunit

open AccessTokenFactory

module PolicyBasedAuthorizationTests =
  [<Theory>]
  [<InlineData("Alice", "Pass123$", "Api1 Orleans")>]
  let ``A user with an appropriate role should have access to the method``
    (userName: string)
    (password: string)
    (scope: string)
    =
    task {
      // Arrange
      let! accessTokenResponse = getAccessTokenForUserOnWebClient1Async userName password scope

      let clusterClient = getClusterClient accessTokenResponse.AccessToken
      let userGrain = clusterClient.GetGrain<IPolicyGrain>(userName)
      let! value = userGrain.GetWithMangerPolicy("Secret")

      Assert.True(value.Equals "Secret")
    }

  [<Theory>]
  [<InlineData("Bob", "Pass123$", "Api1 Orleans")>]
  let ``A user without an appropriate role shouldn't have access to the method``
    (userName: string)
    (password: string)
    (scope: string)
    =
    task {
      // Arrange
      let! accessTokenResponse = getAccessTokenForUserOnWebClient1Async userName password scope

      let clusterClient = getClusterClient accessTokenResponse.AccessToken
      let userGrain = clusterClient.GetGrain<IPolicyGrain>(userName)

      // Act
      let action =
        task {
          let! value = userGrain.GetWithMangerPolicy(String.Empty)
          return value
        }

      Assert.ThrowsAsync<AuthorizationException>(fun () -> action) |> ignore
    }