using AuthZI.Tests.MicrosoftOrleans.MicrosoftEntra.Common;
using AuthZI.Tests.MicrosoftOrleans.MicrosoftEntra.Common.AccessToken;
using AuthZI.Tests.MicrosoftOrleans.MicrosoftEntra.Common.Authorization;
using AuthZI.Tests.MicrosoftOrleans.MicrosoftEntra.Common.Connection;
using Xunit;

namespace AuthZI.Tests.MicrosoftOrleans.MicrosoftEntra.MicrosoftEntraID.CSharp;

[MicrosoftEntraIDTest]
public class MicrosoftEntraIdTests(ITestOutputHelper output, EntraIdMainTestFixture fixture)
  : MicrosoftEntraIdTestsBase(output, fixture.AccessTokenRetriever);

[MicrosoftEntraIDTest]
public class DiscoveryDocumentProviderTests(ITestOutputHelper output) : DiscoveryDocumentProviderTestsBase(output);

[MicrosoftEntraIDTest]
public class AccessTokenVerificationTests(EntraIdMainTestFixture fixture, ITestOutputHelper output)
  : AccessTokenVerificationTestsBase(fixture, output);

[MicrosoftEntraIDTest]
public class ExpectedAudienceTests(ITestOutputHelper output, EntraIdMainTestFixture fixture)
  : AudienceValidationTestsBase(output, fixture.AccessTokenRetriever);

[MicrosoftEntraIDTest]
public class SimpleAuthorizationTests(EntraIdMainTestFixture fixture)
  : SimpleAuthorizationTestsBase(fixture, fixture.AccessTokenRetriever);

[MicrosoftEntraIDTest]
public class RoleBasedAuthorizationTests(EntraIdMainTestFixture fixture)
  : RoleBasedAuthorizationTestsBase(fixture, fixture.AccessTokenRetriever);

[MicrosoftEntraIDTest]
public class ClaimsBasedAuthorizationTests(EntraIdMainTestFixture fixture)
  : ClaimsBasedAuthorizationTestsBase(fixture, fixture.AccessTokenRetriever);
