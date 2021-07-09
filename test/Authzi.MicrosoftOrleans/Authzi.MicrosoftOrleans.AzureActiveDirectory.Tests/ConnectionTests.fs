module ConnectionTests

open Xunit
open AccessTokenFactory

[<Theory>]
[<InlineData("Carol", "Pass123$", "Api1 Orleans")>]
let ``The system can obtain Access Token from Azure AD endpoint``
    (userName: string) (password: string) (scope: string) =
    async {
        // Arrange
        let! accessToken = getAccessTokenForUserOnWebClient1Async
                                       userName password scope |> Async.AwaitTask

        // Act
        Assert.True(System.String.IsNullOrWhiteSpace(accessToken))
    }
