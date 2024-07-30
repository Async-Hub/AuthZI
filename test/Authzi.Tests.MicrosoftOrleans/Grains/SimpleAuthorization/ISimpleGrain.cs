using System.Threading.Tasks;
using Authzi.Security.Authorization;
using Orleans;

namespace Authzi.Tests.MicrosoftOrleans.Grains.SimpleAuthorization
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