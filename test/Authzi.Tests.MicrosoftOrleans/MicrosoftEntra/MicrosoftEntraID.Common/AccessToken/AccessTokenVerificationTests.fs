namespace Authzi.Tests.MicrosoftOrleans.MicrosoftEntra.MicrosoftEntraID.Common

open AccessTokenProvider
open Authzi.MicrosoftEntra
open Authzi.Security.AccessToken
open Xunit
open Xunit.Abstractions

type AccessTokenVerificationTestsBase(output: ITestOutputHelper) =

    [<Theory>]
    [<MemberData(nameof(TestData.Users), MemberType = typeof<TestData>)>] 
    member _.``The system can verify JWT Token from Azure AD endpoint`` 
        (userName: string) (password: string) =
        async {
            // Arrange
            let discoveryDocumentProvider = 
                DiscoveryDocumentProvider(TestData.AzureActiveDirectoryApp.DiscoveryEndpointUrl)
            
            let! accessToken = getAccessTokenForUserOnWebClient1Async userName password |> Async.AwaitTask
            output.WriteLine(accessToken)

            let accessTokenIntrospectionService = AccessTokenIntrospectionService(TestData.AzureActiveDirectoryApp,
                                                    discoveryDocumentProvider, ClaimTypeResolverDefault()) :> IAccessTokenIntrospectionService

            let! result = accessTokenIntrospectionService.IntrospectTokenAsync accessToken false |> Async.AwaitTask
            
            // Act
            Assert.True(result.IsValid)
         }


