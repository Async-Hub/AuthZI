using System.Threading.Tasks;
using AuthZI.Security.Authorization;
using Orleans;

namespace AuthZI.Tests.MicrosoftOrleans.Grains.PolicyBasedAuthorization
{
    public interface IPolicyGrain : IGrainWithStringKey
    {
        [Authorize(Policy = "ManagerPolicy")]
        Task<string> GetWithMangerPolicy(string secret);
    }
}