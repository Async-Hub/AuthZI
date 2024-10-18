using System.Threading.Tasks;
using Orleans;

namespace AuthZI.Tests.MicrosoftOrleans.Grains.PolicyBasedAuthorization
{
    public class PolicyGrain : Grain, IPolicyGrain
    {
        public Task<string> GetWithMangerPolicy(string secret)
        {
            return Task.FromResult(secret);
        }
    }
}