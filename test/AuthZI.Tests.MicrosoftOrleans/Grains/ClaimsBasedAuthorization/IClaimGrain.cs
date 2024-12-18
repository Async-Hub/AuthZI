using AuthZI.Security.Authorization;
using Orleans;
using System.Threading.Tasks;

namespace AuthZI.Tests.MicrosoftOrleans.Grains.ClaimsBasedAuthorization;

public interface IClaimGrain : IGrainWithStringKey
{
    [Authorize(Policy = "ArmeniaCountryOnly")]
    Task<string> DoSomething(string someInput);
}