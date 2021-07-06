using System.Threading.Tasks;
using Authzi.Security.Authorization;
using Orleans;

namespace Authzi.MicrosoftOrleans.Grains.ClaimsBasedAuthorization
{
    public interface IClaimGrain : IGrainWithStringKey
    {
        [Authorize(Policy = "NewYorkCityOnly")]
        Task<string> DoSomething(string someInput);
    }
}