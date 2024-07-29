using System.Threading.Tasks;
using Orleans;

namespace Authzi.Tests.MicrosoftOrleans.Grains.SimpleAuthorization
{
    public class SimpleGrain : Grain, ISimpleGrain
    {
        public Task<string> GetWithAnonymousUser(string secret)
        {
            return Task.FromResult(secret);
        }
        
        public Task<string> GetWithAuthenticatedUser(string secret)
        {
            return Task.FromResult(secret);
        }
        
        public Task<string> GetValue()
        {
            return Task.FromResult("Some protected string.");
        }
    }
}