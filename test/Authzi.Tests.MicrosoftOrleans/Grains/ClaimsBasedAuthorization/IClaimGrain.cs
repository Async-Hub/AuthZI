using System.Threading.Tasks;
using Authzi.Security.Authorization;
using Orleans;

namespace Authzi.Tests.MicrosoftOrleans.Grains.ClaimsBasedAuthorization;

public interface IClaimGrain : IGrainWithStringKey
{
    [Authorize(Policy = "ArmeniaCountryOnly")]
    Task<string> DoSomething(string someInput);
}