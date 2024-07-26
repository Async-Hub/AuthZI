namespace Authzi.MicrosoftOrleans.AzureActiveDirectory.Tests.Connection

open AccessTokenProvider
open System
open Xunit
open Xunit.Abstractions

open Authzi.AzureActiveDirectory.Configuration.Common.Credentials.AzureActiveDirectoryB2B1

type AzureActiveDirectoryB2BTestsBase(output: ITestOutputHelper) =
    static member Input with get() : obj[] list = 
        [
            [| AdeleV.Name; AdeleV.Password |]; 
            [| AlexW.Name; AlexW.Password |]
        ]
    
    [<Theory>]
    [<MemberData(nameof(AzureActiveDirectoryB2BTestsBase.Input))>] 
    member _.``The system can obtain Access Token from Azure AD B2B endpoint``
        (userName: string) (password: string) =
        async {
            // Arrange
            let! accessToken = getAccessTokenForUserOnWebClient1Async userName password |> Async.AwaitTask
            output.WriteLine(accessToken)
            
            // Act
            Assert.False(String.IsNullOrWhiteSpace(accessToken))
    }

open Authzi.AzureActiveDirectory.Configuration.Common.Credentials.AzureActiveDirectoryB2C1

type AzureActiveDirectoryB2CTestsBase(output: ITestOutputHelper) =
    static member Input with get() : obj[] list = [[| AdeleV.Name; AdeleV.Password |]]
    
    [<Theory>]
    [<MemberData(nameof(AzureActiveDirectoryB2CTestsBase.Input))>] 
    member _.``The system can obtain Access Token from Azure AD B2C endpoint``
        (userName: string) (password: string) =
        async {
            // Arrange
            let! accessToken = getAccessTokenForUserOnWebClient1Async userName password |> Async.AwaitTask
            output.WriteLine(accessToken)
            
            // Act
            Assert.False(String.IsNullOrWhiteSpace(accessToken))
    }