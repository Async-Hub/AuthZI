using System.Threading.Tasks;
using Authzi.Security.Authorization;
using Orleans;

namespace Authzi.MicrosoftOrleans.Grains.PolicyBasedAuthorization
{
    public interface IPolicyGrain : IGrainWithStringKey
    {
        [Authorize(Policy = "ManagerPolicy")]
        Task<string> GetWithMangerPolicy(string secret);
    }
}