﻿namespace Authzi.Tests.MicrosoftOrleans.MicrosoftEntra.MicrosoftEntraExternalID.NET8

open Authzi.Tests.MicrosoftOrleans.MicrosoftEntra.MicrosoftEntraID.Common
open Authzi.Tests.MicrosoftOrleans.MicrosoftEntra.MicrosoftEntraID.Common.Authorization
open Authzi.Tests.MicrosoftOrleans.MicrosoftEntra.MicrosoftEntraID.Common.Connection
open Xunit.Abstractions

type AzureActiveDirectoryB2BTests(output: ITestOutputHelper) =
    inherit AzureActiveDirectoryB2BTestsBase(output)

type AccessTokenVerificationTests(output: ITestOutputHelper) =
    inherit AccessTokenVerificationTestsBase(output)

type SimpleAuthorizationTests() =
    inherit SimpleAuthorizationTestsBase()

type RoleBasedAuthorizationTests() =
    inherit RoleBasedAuthorizationTestsBase()

// TODOD: Change to ClaimsBasedAuthorizationTests
//type ClaimsBasedAuthorizationTests() =
//    inherit ClaimsBasedAuthorizationTestsBase()
