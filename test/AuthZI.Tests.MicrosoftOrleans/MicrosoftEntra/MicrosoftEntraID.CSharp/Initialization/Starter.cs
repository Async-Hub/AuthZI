using AuthZI.Tests.MicrosoftOrleans.MicrosoftEntra.MicrosoftEntraID.Common.Initialization;
using Orleans;
using Xunit;

[assembly: ApplicationPart("AuthZI.Tests.MicrosoftOrleans.Grains")]
[assembly: AssemblyFixture(typeof(MicrosoftEntraIdTestFixture))]
[assembly: CollectionBehavior(DisableTestParallelization = true)]
