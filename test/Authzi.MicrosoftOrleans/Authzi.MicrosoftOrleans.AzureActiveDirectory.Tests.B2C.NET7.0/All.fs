﻿namespace Authzi.MicrosoftOrleans.AzureActiveDirectory.Tests.NET7

open Authzi.MicrosoftOrleans.AzureActiveDirectory.Tests
open Authzi.MicrosoftOrleans.AzureActiveDirectory.Tests.Authorization
open Authzi.MicrosoftOrleans.AzureActiveDirectory.Tests.Connection
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