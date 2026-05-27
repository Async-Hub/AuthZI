namespace AuthZI.Tests.MicrosoftOrleans.MicrosoftEntra.MicrosoftEntraID.NET10

open AuthZI.Tests.MicrosoftOrleans.MicrosoftEntra.MicrosoftEntraID.Common
open AuthZI.Tests.MicrosoftOrleans.MicrosoftEntra.MicrosoftEntraID.Common.Authorization
open AuthZI.Tests.MicrosoftOrleans.MicrosoftEntra.MicrosoftEntraID.Common.Connection
open Xunit

type MicrosoftEntraIdTests(output: ITestOutputHelper) =
    inherit MicrosoftEntraIdTestsBase(output)

type DiscoveryDocumentProviderTests(output: ITestOutputHelper) =
    inherit DiscoveryDocumentProviderTestsBase(output: ITestOutputHelper)

type AccessTokenVerificationTests(output: ITestOutputHelper) =
    inherit AccessTokenVerificationTestsBase(output)

type SimpleAuthorizationTests() =
    inherit SimpleAuthorizationTestsBase()

type RoleBasedAuthorizationTests() =
    inherit RoleBasedAuthorizationTestsBase()

type ClaimsBasedAuthorizationTests() =
    inherit ClaimsBasedAuthorizationTestsBase()
