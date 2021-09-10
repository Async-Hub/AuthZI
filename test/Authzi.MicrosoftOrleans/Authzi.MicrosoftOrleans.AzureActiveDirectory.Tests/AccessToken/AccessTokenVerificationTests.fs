namespace Authzi.MicrosoftOrleans.AzureActiveDirectory.Tests

open Authzi.AzureActiveDirectory
open Xunit
open Xunit.Abstractions

open AccessTokenProvider
open Credentials.AzureActiveDirectoryB2B1
open Authzi.Security.AccessToken

type AccessTokenVerificationTests(output: ITestOutputHelper) =
    static member Input with get() : obj[] list = [[| AdeleV.Name; AdeleV.Password |]]
    
    [<Theory>]
    [<MemberData(nameof(AccessTokenVerificationTests.Input))>] 
    member _.``The system can verify JWT Token from Azure AD endpoint`` 
        (userName: string) (password: string) =
        async {
            // Arrange
            let discoveryDocumentProvider = 
                DiscoveryDocumentProvider(GlobalConfig.azureActiveDirectoryAppB2B1.DiscoveryEndpointUrl)
            
            let! accessToken = getAccessTokenForUserOnB2BWebClient1Async userName password |> Async.AwaitTask
            output.WriteLine(accessToken)

            let accessTokenIntrospectionService = AccessTokenIntrospectionService(GlobalConfig.azureActiveDirectoryAppB2B1,
                                                    discoveryDocumentProvider, ClaimTypeResolverDefault()) :> IAccessTokenIntrospectionService

            let! result = accessTokenIntrospectionService.IntrospectTokenAsync accessToken false |> Async.AwaitTask
            
            // Act
            Assert.True(result.IsValid)
         }


