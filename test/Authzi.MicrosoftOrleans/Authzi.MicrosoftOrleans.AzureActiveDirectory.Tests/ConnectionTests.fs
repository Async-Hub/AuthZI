module ConnectionTests

open AccessTokenFactory
open Credentials.AzureActiveDirectoryB2B1
open System
open Xunit
open Xunit.Abstractions

type ConnectionTests(output: ITestOutputHelper) =
    static member Input with get() : obj[] list = [[| AdeleV.UserName; AdeleV.Password |]]
    
    [<Theory>]
    [<MemberData(nameof(ConnectionTests.Input))>] 
    member _.``The system can obtain Access Token from Azure AD endpoint``
        (userName: string) (password: string) =
        async {
            // Arrange
            let! accessToken = getAccessTokenForUserOnWebClient1Async userName password |> Async.AwaitTask
            output.WriteLine(accessToken)
            
            // Act
            Assert.False(String.IsNullOrWhiteSpace(accessToken))
    }

