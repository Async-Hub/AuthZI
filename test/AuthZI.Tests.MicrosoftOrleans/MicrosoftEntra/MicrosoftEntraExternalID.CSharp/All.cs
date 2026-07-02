using AuthZI.Tests.MicrosoftOrleans.MicrosoftEntra.MicrosoftEntraID.Common.AccessToken;
using AuthZI.Tests.MicrosoftOrleans.MicrosoftEntra.MicrosoftEntraID.Common.Authorization;
using AuthZI.Tests.MicrosoftOrleans.MicrosoftEntra.MicrosoftEntraID.Common.Connection;
using AuthZI.Tests.MicrosoftOrleans.MicrosoftEntra.MicrosoftEntraExternalID.CSharp.Initialization;
using Xunit;

namespace AuthZI.Tests.MicrosoftOrleans.MicrosoftEntra.MicrosoftEntraExternalID.CSharp;

public class MicrosoftEntraIdTests(ITestOutputHelper output) : MicrosoftEntraIdTestsBase(output);

public class DiscoveryDocumentProviderTests(ITestOutputHelper output) : DiscoveryDocumentProviderTestsBase(output);

public class AccessTokenVerificationTests(ITestOutputHelper output) : AccessTokenVerificationTestsBase(output);

public class ExpectedAudienceTests(ITestOutputHelper output) : AudienceValidationTestsBase(output);

public class SimpleAuthorizationTests(ExternalIdMainTestFixture fixture) : SimpleAuthorizationTestsBase(fixture);

public class RoleBasedAuthorizationTests(ExternalIdMainTestFixture fixture) : RoleBasedAuthorizationTestsBase(fixture);

public class ClaimsBasedAuthorizationTests(ExternalIdMainTestFixture fixture) : ClaimsBasedAuthorizationTestsBase(fixture);
