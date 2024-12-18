using System.Threading.Tasks;
using AuthZI.Security.Authorization;
using Orleans;

namespace AuthZI.Tests.MicrosoftOrleans.Grains.SimpleAuthorization
{
    [Authorize]
    public interface ISimpleGrain : IGrainWithGuidKey
    {
        [AllowAnonymous]
        Task<string> GetWithAnonymousUser(string secret);
        
        Task<string> GetWithAuthenticatedUser(string secret);
        
        Task<string> GetValue();
    }
}