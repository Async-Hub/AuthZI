﻿namespace AuthZI.Tests.MicrosoftOrleans.MicrosoftEntra.MicrosoftEntraID.Common.Connection

open AccessTokenProvider
open AuthZI.Tests.MicrosoftOrleans.MicrosoftEntra.MicrosoftEntraID.Common
open System
open Xunit
open Xunit.Abstractions

type MicrosoftEntraIdTestsBase(output: ITestOutputHelper) =
  [<Theory>]
  [<MemberData(nameof (TestData.Users), MemberType = typeof<TestData>)>]
  member _.``The system can obtain Access Token from Microsoft Entra ID endpoint`` (userName: string) (password: string) =
    async {
      // Arrange
      let! accessToken = getAccessTokenForUserOnWebClient1Async userName password |> Async.AwaitTask
      output.WriteLine(accessToken)

      // Act
      Assert.False(String.IsNullOrWhiteSpace(accessToken))
    }

open AuthZI.Deploy.MicrosoftEntra.Configuration.Credentials.AzureActiveDirectoryB2C1

type AzureActiveDirectoryB2CTestsBase(output: ITestOutputHelper) =
  static member Input: obj[] list = [ [| AdeleV.Name; AdeleV.Password |] ]

  [<Theory>]
  [<MemberData(nameof (AzureActiveDirectoryB2CTestsBase.Input))>]
  member _.``The system can obtain Access Token from Azure AD B2C endpoint`` (userName: string) (password: string) =
    async {
      // Arrange
      let! accessToken = getAccessTokenForUserOnWebClient1Async userName password |> Async.AwaitTask
      output.WriteLine(accessToken)

      // Act
      Assert.False(String.IsNullOrWhiteSpace(accessToken))
    }
