namespace AuthZI.Tests.MicrosoftOrleans.Duende.IdentityServer

open AuthZI.Security
open AuthZI.Tests.MicrosoftOrleans.Duende.IdentityServer.GlobalConfig
open AuthZI.Tests.MicrosoftOrleans.Grains.ResourceBasedAuthorization
open Xunit

open AccessTokenFactory

module ResourceBasedAuthorizationTests =
  [<Theory>]
  [<InlineData("Bob", "Pass123$", "Api1 Orleans")>]
  let ``A user with appropriate permission can read a document`` (userName: string) (password: string) (scope: string) =
    task {
      // Arrange
      let! accessTokenResponse = getAccessTokenForUserOnWebClient1Async userName password scope

      let clusterClient = getClusterClient accessTokenResponse.AccessToken
      let userGrain = clusterClient.GetGrain<IUserGrain>(userName)
      let! value = userGrain.GetDocumentContent("Document2")

      Assert.True(value.Equals "Some content 2.")
    }

  [<Theory>]
  [<InlineData("Carol", "Pass123$", "Api1 Orleans")>]
  let ``A user without appropriate permission can't read a document``
    (userName: string)
    (password: string)
    (scope: string)
    =
    task {
      // Arrange
      let! accessTokenResponse = getAccessTokenForUserOnWebClient1Async userName password scope

      let action =
        task {
          let clusterClient = getClusterClient accessTokenResponse.AccessToken
          let userGrain = clusterClient.GetGrain<IUserGrain>(userName)
          let! value = userGrain.GetDocumentContent("Document1")
          return value
        }

      Assert.ThrowsAsync<AuthorizationException>(fun () -> action) |> ignore
    }

  [<Theory>]
  [<InlineData("Alice", "Pass123$", "Api1 Orleans")>]
  let ``A user with appropriate permission can modify a document``
    (userName: string)
    (password: string)
    (scope: string)
    =
    task {
      // Arrange
      let! accessTokenResponse = getAccessTokenForUserOnWebClient1Async userName password scope

      let newContent = "Some new content!"
      let clusterClient = getClusterClient accessTokenResponse.AccessToken
      let userGrain = clusterClient.GetGrain<IUserGrain>(userName)
      let! value = userGrain.ModifyDocumentContent("Document1", newContent)

      Assert.True(value.Equals newContent)
    }

  [<Theory>]
  [<InlineData("Alice", "Pass123$", "Api1 Orleans")>]
  let ``A user without appropriate permission can't modify a document``
    (userName: string)
    (password: string)
    (scope: string)
    =
    task {
      // Arrange
      // This is Bob's document.
      let documentName = "Document2"
      let! accessTokenResponse = getAccessTokenForUserOnWebClient1Async userName password scope

      let newContent = "Some new content!"
      let clusterClient = getClusterClient accessTokenResponse.AccessToken
      let userGrain = clusterClient.GetGrain<IUserGrain>(userName)
      let! value = userGrain.ModifyDocumentContent(documentName, newContent)

      Assert.Null(value)
    }