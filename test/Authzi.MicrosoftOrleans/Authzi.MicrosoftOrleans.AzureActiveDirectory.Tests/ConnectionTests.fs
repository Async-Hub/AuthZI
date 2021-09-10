﻿module ConnectionTests

open AccessTokenProvider
open System
open Xunit
open Xunit.Abstractions

open Credentials.AzureActiveDirectoryB2B1

type AzureActiveDirectoryB2BTests(output: ITestOutputHelper) =
    static member Input with get() : obj[] list = 
        [
            [| AdeleV.Name; AdeleV.Password |]; 
            [| AlexW.Name; AlexW.Password |]
        ]
    
    [<Theory>]
    [<MemberData(nameof(AzureActiveDirectoryB2BTests.Input))>] 
    member _.``The system can obtain Access Token from Azure AD B2B endpoint``
        (userName: string) (password: string) =
        async {
            // Arrange
            let! accessToken = getAccessTokenForUserOnB2BWebClient1Async userName password |> Async.AwaitTask
            output.WriteLine(accessToken)
            
            // Act
            Assert.False(String.IsNullOrWhiteSpace(accessToken))
    }

open Credentials.AzureActiveDirectoryB2C1

type AzureActiveDirectoryB2CTests(output: ITestOutputHelper) =
    static member Input with get() : obj[] list = [[| AdeleV.Name; AdeleV.Password |]]
    
    [<Theory>]
    [<MemberData(nameof(AzureActiveDirectoryB2CTests.Input))>] 
    member _.``The system can obtain Access Token from Azure AD B2C endpoint``
        (userName: string) (password: string) =
        async {
            // Arrange
            let! accessToken = getAccessTokenForUserOnB2CWebClient1Async userName password |> Async.AwaitTask
            output.WriteLine(accessToken)
            
            // Act
            Assert.False(String.IsNullOrWhiteSpace(accessToken))
    }