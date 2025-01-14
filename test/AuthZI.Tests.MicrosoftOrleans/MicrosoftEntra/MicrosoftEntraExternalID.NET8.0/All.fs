namespace AuthZI.Tests.MicrosoftOrleans.MicrosoftEntra.MicrosoftEntraExternalID.NET8

open AuthZI.Tests.MicrosoftOrleans.MicrosoftEntra.MicrosoftEntraID.Common
open AuthZI.Tests.MicrosoftOrleans.MicrosoftEntra.MicrosoftEntraID.Common.Authorization
open AuthZI.Tests.MicrosoftOrleans.MicrosoftEntra.MicrosoftEntraID.Common.Connection
open Xunit.Abstractions

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

// TODOD: Change to ClaimsBasedAuthorizationTests
//type ClaimsBasedAuthorizationTests() =
//    inherit ClaimsBasedAuthorizationTestsBase()
