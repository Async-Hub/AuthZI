﻿namespace AuthZI.MicrosoftOrleans.MicrosoftEntra.Tests.NET8

open AuthZI.Tests.MicrosoftOrleans.MicrosoftEntra.MicrosoftEntraID.Common
open AuthZI.Tests.MicrosoftOrleans.MicrosoftEntra.MicrosoftEntraID.Common.Authorization
open AuthZI.Tests.MicrosoftOrleans.MicrosoftEntra.MicrosoftEntraID.Common.Connection
open Xunit.Abstractions

type AzureActiveDirectoryB2CTests(output: ITestOutputHelper) =
    inherit AzureActiveDirectoryB2CTestsBase(output)

type AccessTokenVerificationTests(output: ITestOutputHelper) =
    inherit AccessTokenVerificationTestsBase(output)

type SimpleAuthorizationTests() =
    inherit SimpleAuthorizationTestsBase()

type RoleBasedAuthorizationTests() =
    inherit RoleBasedAuthorizationTestsBase()

type ClaimsBasedAuthorizationTests() =
    inherit ClaimsBasedAuthorizationTestsBase()