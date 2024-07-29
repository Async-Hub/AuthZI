using System.Threading.Tasks;
using Orleans;

namespace Authzi.Tests.MicrosoftOrleans.Grains.PolicyBasedAuthorization
{
    public class PolicyGrain : Grain, IPolicyGrain
    {
        public Task<string> GetWithMangerPolicy(string secret)
        {
            return Task.FromResult(secret);
        }
    }
}