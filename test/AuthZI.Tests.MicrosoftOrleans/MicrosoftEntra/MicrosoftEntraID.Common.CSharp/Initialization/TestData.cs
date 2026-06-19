using AuthZI.Identity.MicrosoftEntra;
using Orleans;
using System;
using System.Collections.Generic;

namespace AuthZI.Tests.MicrosoftOrleans.MicrosoftEntra.MicrosoftEntraID.Common.Initialization;

public class TestData
{
  public static IEnumerable<object[]> UserWithScopeAdeleV { get; set; } = Array.Empty<object[]>();

  public static IEnumerable<object[]> UserWithScopeAlexW { get; set; } = Array.Empty<object[]>();

  public static IEnumerable<object[]> Users { get; set; } = Array.Empty<object[]>();

  public static IReadOnlyDictionary<string, string> UserPasswords { get; set; } =
    new Dictionary<string, string>();

  public static MicrosoftEntraApp Web1ClientApp { get; set; } = MicrosoftEntraIDApp.EmptyApp;

  public static MicrosoftEntraApp Web2ClientApp { get; set; } = MicrosoftEntraIDApp.EmptyApp;

  public static IClusterClient IClusterClient { get; set; } = null!;

  public static Func<MicrosoftEntraApp, string, string, string> GetAccessTokenForUserOnMicrosoftEntraAppAsync { get; set; } =
    (_, _, _) => string.Empty;
}
