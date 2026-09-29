using AuthZI.Identity.MicrosoftEntra;
using Orleans;
using System.Collections.Generic;

namespace AuthZI.Tests.MicrosoftOrleans.MicrosoftEntra.MicrosoftEntraID.Common.Initialization;

public class TestData
{
  public static IEnumerable<object[]> UserWithScopeAdeleV { get; set; } = [];

  public static IEnumerable<object[]> UserWithScopeAlexW { get; set; } = [];

  public static IEnumerable<object[]> Users { get; set; } = [];

  public static IReadOnlyDictionary<string, string> UserPasswords { get; set; } =
    new Dictionary<string, string>();

  public static MicrosoftEntraApp WebClient1 { get; set; } = MicrosoftEntraIDApp.EmptyApp;

  public static MicrosoftEntraApp WebClient2 { get; set; } = MicrosoftEntraIDApp.EmptyApp;

  public static IClusterClient IClusterClient { get; set; } = null!;
}
