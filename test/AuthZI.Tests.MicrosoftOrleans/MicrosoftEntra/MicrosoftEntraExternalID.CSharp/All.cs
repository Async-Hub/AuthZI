using AuthZI.Tests.MicrosoftOrleans.MicrosoftEntra.MicrosoftEntraID.Common.AccessToken;
using AuthZI.Tests.MicrosoftOrleans.MicrosoftEntra.MicrosoftEntraID.Common.Authorization;
using AuthZI.Tests.MicrosoftOrleans.MicrosoftEntra.MicrosoftEntraID.Common.Connection;
using Xunit;

namespace AuthZI.Tests.MicrosoftOrleans.MicrosoftEntra.MicrosoftEntraExternalID.CSharp;

public class MicrosoftEntraIdTests(ITestOutputHelper output, ExternalIdMainTestFixture fixture)
  : MicrosoftEntraIdTestsBase(output, fixture.AccessTokenRetriever);

public class DiscoveryDocumentProviderTests(ITestOutputHelper output) : DiscoveryDocumentProviderTestsBase(output);

public class AccessTokenVerificationTests(ExternalIdMainTestFixture fixture, ITestOutputHelper output)
  : AccessTokenVerificationTestsBase(fixture, output);

public class ExpectedAudienceTests(ITestOutputHelper output, ExternalIdMainTestFixture fixture)
  : AudienceValidationTestsBase(output, fixture.AccessTokenRetriever);

public class SimpleAuthorizationTests(ExternalIdMainTestFixture fixture)
  : SimpleAuthorizationTestsBase(fixture, fixture.AccessTokenRetriever);

public class RoleBasedAuthorizationTests(ExternalIdMainTestFixture fixture)
  : RoleBasedAuthorizationTestsBase(fixture, fixture.AccessTokenRetriever);

public class ClaimsBasedAuthorizationTests(ExternalIdMainTestFixture fixture)
  : ClaimsBasedAuthorizationTestsBase(fixture, fixture.AccessTokenRetriever);
