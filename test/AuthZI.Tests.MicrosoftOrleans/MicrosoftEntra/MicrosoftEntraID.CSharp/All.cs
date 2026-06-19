using AuthZI.Tests.MicrosoftOrleans.MicrosoftEntra.MicrosoftEntraID.Common;
using AuthZI.Tests.MicrosoftOrleans.MicrosoftEntra.MicrosoftEntraID.Common.Authorization;
using AuthZI.Tests.MicrosoftOrleans.MicrosoftEntra.MicrosoftEntraID.Common.Connection;
using Xunit;

namespace AuthZI.Tests.MicrosoftOrleans.MicrosoftEntra.MicrosoftEntraID.CSharp;

public class MicrosoftEntraIdTests(ITestOutputHelper output) : MicrosoftEntraIdTestsBase(output);

public class DiscoveryDocumentProviderTests(ITestOutputHelper output) : DiscoveryDocumentProviderTestsBase(output);

public class AccessTokenVerificationTests(ITestOutputHelper output) : AccessTokenVerificationTestsBase(output);

public class ExpectedAudienceTests(ITestOutputHelper output) : AudienceValidationTestsBase(output);

public class SimpleAuthorizationTests : SimpleAuthorizationTestsBase;

public class RoleBasedAuthorizationTests : RoleBasedAuthorizationTestsBase;

public class ClaimsBasedAuthorizationTests : ClaimsBasedAuthorizationTestsBase;
