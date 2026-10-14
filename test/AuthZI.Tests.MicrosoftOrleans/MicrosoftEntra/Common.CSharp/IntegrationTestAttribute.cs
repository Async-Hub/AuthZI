using System;
using System.Collections.Generic;
using Xunit.v3;

namespace AuthZI.Tests.MicrosoftOrleans.MicrosoftEntra.Common;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public sealed class IntegrationTestAttribute : Attribute, ITraitAttribute
{
  public IReadOnlyCollection<KeyValuePair<string, string>> GetTraits() =>
    [new("Category", "Integration")];
}