using Authzi.Security.Authorization;
using Orleans;
using System.Threading.Tasks;

namespace Authzi.MicrosoftOrleans.Grains.ClaimsBasedAuthorization;

public interface IClaimGrain : IGrainWithStringKey
{
    [Authorize(Policy = "ArmeniaCountryOnly")]
    Task<string> DoSomething(string someInput);
}