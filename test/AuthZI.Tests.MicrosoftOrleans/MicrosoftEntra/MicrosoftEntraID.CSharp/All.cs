using AuthZI.Tests.MicrosoftOrleans.MicrosoftEntra.MicrosoftEntraID.Common.AccessToken;
using AuthZI.Tests.MicrosoftOrleans.MicrosoftEntra.MicrosoftEntraID.Common.Authorization;
using AuthZI.Tests.MicrosoftOrleans.MicrosoftEntra.MicrosoftEntraID.Common.Connection;
using Xunit;

namespace AuthZI.Tests.MicrosoftOrleans.MicrosoftEntra.MicrosoftEntraID.CSharp;

public class MicrosoftEntraIdTests(ITestOutputHelper output, EntraIdMainTestFixture fixture)
  : MicrosoftEntraIdTestsBase(output, fixture.AccessTokenRetriever);

public class DiscoveryDocumentProviderTests(ITestOutputHelper output) : DiscoveryDocumentProviderTestsBase(output);

public class AccessTokenVerificationTests(EntraIdMainTestFixture fixture, ITestOutputHelper output)
  : AccessTokenVerificationTestsBase(fixture, output);

public class ExpectedAudienceTests(ITestOutputHelper output, EntraIdMainTestFixture fixture)
  : AudienceValidationTestsBase(output, fixture.AccessTokenRetriever);

public class SimpleAuthorizationTests(EntraIdMainTestFixture fixture)
  : SimpleAuthorizationTestsBase(fixture, fixture.AccessTokenRetriever);

public class RoleBasedAuthorizationTests(EntraIdMainTestFixture fixture)
  : RoleBasedAuthorizationTestsBase(fixture, fixture.AccessTokenRetriever);

public class ClaimsBasedAuthorizationTests(EntraIdMainTestFixture fixture)
  : ClaimsBasedAuthorizationTestsBase(fixture, fixture.AccessTokenRetriever);
