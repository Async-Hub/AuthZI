using System.Threading.Tasks;
using Orleans;

namespace Authzi.Tests.MicrosoftOrleans.Grains.ClaimsBasedAuthorization
{
    public class ClaimGrain : Grain, IClaimGrain
    {
        public Task<string> DoSomething(string someInput)
        {
            return Task.FromResult<string>(someInput);
        }
    }
}