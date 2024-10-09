namespace Authzi.Tests.MicrosoftOrleans.DuendeSoftware.IdentityServer

open Authzi.Security
open Authzi.Tests.MicrosoftOrleans.DuendeSoftware.IdentityServer.GlobalConfig
open Authzi.Tests.MicrosoftOrleans.Grains.RoleBasedAuthorization
open Xunit

open AccessTokenFactory

module RoleBasedAuthorizationTests =
    [<Theory>]
    [<InlineData("Carol", "Pass123$", "Api1 Orleans")>]
    let ``Multiple roles as a comma separated list should work when the user has both roles``
        (userName: string) (password: string) (scope: string) =
        task {
            // Arrange
            let! accessTokenResponse = getAccessTokenForUserOnWebClient1Async userName password scope

            let clusterClient = getClusterClient accessTokenResponse.AccessToken
            let userGrain = clusterClient.GetGrain<IManagerGrain>(userName)
            let! value = userGrain.GetWithCommaSeparatedRoles("Secret")

            Assert.True(value.Equals "Secret")
        }

    [<Theory>]
    [<InlineData("Alice", "Pass123$", "Api1 Orleans")>]
    let ``Multiple roles as a comma separated list shouldn't work when the user has only one role``
        (userName: string) (password: string) (scope: string) =
        task {
            // Arrange
            let! accessTokenResponse = getAccessTokenForUserOnWebClient1Async userName password scope

            let clusterClient = getClusterClient accessTokenResponse.AccessToken
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
    [<InlineData("Carol", "Pass123$", "Api1 Orleans")>]
    let ``Multiple roles applied as multiple attributes should work when the user has both roles``
        (userName: string) (password: string) (scope: string) =
        task {
            // Arrange
            let! accessTokenResponse = getAccessTokenForUserOnWebClient1Async userName password scope

            let clusterClient = getClusterClient accessTokenResponse.AccessToken
            let userGrain = clusterClient.GetGrain<IManagerGrain>(userName)
            let! value = userGrain.GetWithMultipleRoleAttributes("Secret")

            Assert.True(value.Equals "Secret")
        }

    [<Theory>]
    [<InlineData("Alice", "Pass123$", "Api1 Orleans")>]
    let ``Multiple roles applied as multiple attributes shouldn't work when the user has only one role``
        (userName: string) (password: string) (scope: string) =
        task {
            // Arrange
            let! accessTokenResponse = getAccessTokenForUserOnWebClient1Async userName password scope

            let clusterClient = getClusterClient accessTokenResponse.AccessToken
            let userGrain = clusterClient.GetGrain<IManagerGrain>(userName)

            // Act
            let action =
                task {
                    let! value = userGrain.GetWithMultipleRoleAttributes("Secret")
                    return value
                }

            Assert.ThrowsAsync<AuthorizationException>(fun () -> action) |> ignore
        }