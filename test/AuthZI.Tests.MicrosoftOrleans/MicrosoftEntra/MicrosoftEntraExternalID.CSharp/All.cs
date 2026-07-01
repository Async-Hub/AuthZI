using AuthZI.Tests.MicrosoftOrleans.MicrosoftEntra.MicrosoftEntraID.Common.AccessToken;
using AuthZI.Tests.MicrosoftOrleans.MicrosoftEntra.MicrosoftEntraID.Common.Authorization;
using AuthZI.Tests.MicrosoftOrleans.MicrosoftEntra.MicrosoftEntraID.Common.Connection;
using AuthZI.Tests.MicrosoftOrleans.MicrosoftEntra.MicrosoftEntraID.Common.Initialization;
using Xunit;

namespace AuthZI.Tests.MicrosoftOrleans.MicrosoftEntra.MicrosoftEntraExternalID.CSharp;

public class MicrosoftEntraIdTests(ITestOutputHelper output) : MicrosoftEntraIdTestsBase(output);

public class DiscoveryDocumentProviderTests(ITestOutputHelper output) : DiscoveryDocumentProviderTestsBase(output);

public class AccessTokenVerificationTests(ITestOutputHelper output) : AccessTokenVerificationTestsBase(output);

public class ExpectedAudienceTests(ITestOutputHelper output) : AudienceValidationTestsBase(output);

public class SimpleAuthorizationTests(MainTestFixture fixture) : SimpleAuthorizationTestsBase(fixture);

public class RoleBasedAuthorizationTests(MainTestFixture fixture) : RoleBasedAuthorizationTestsBase(fixture);

public class ClaimsBasedAuthorizationTests(MainTestFixture fixture) : ClaimsBasedAuthorizationTestsBase(fixture);