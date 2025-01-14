namespace AuthZI.Tests.MicrosoftOrleans.MicrosoftEntra.MicrosoftEntraID.Common

open AccessTokenProvider
open AuthZI.MicrosoftEntra
open AuthZI.Identity.MicrosoftEntra
open AuthZI.Security.AccessToken
open Xunit
open Xunit.Abstractions
open AuthZI.Tests.Common.Xunit

type AccessTokenVerificationTestsBase(output: ITestOutputHelper) =

  [<Theory>]
  [<MemberData(nameof (TestData.Users), MemberType = typeof<TestData>)>]
  member _.``The system can verify JWT Token from Azure AD endpoint`` (userName: string) (password: string) =
    task {
      // Arrange
      let discoveryDocumentProvider = DiscoveryDocumentProvider(TestData.Web1ClientApp.DiscoveryEndpointUrl)

      let! accessToken = getAccessTokenForUserOnWebClient1Async userName password
      output.WriteLine(accessToken)

      let logger = TestLogger<AccessTokenIntrospectionService>(output)
      let accessTokenIntrospectionService =
        AccessTokenIntrospectionService(TestData.Web1ClientApp, discoveryDocumentProvider, ClaimTypeResolverDefault(), logger)
        :> IAccessTokenIntrospectionService

      let! result = accessTokenIntrospectionService.IntrospectTokenAsync accessToken false

      let isSuccess =
        match result with
        | Ok _ -> true
        | _ -> false

      // Act
      Assert.True(isSuccess)
    }
