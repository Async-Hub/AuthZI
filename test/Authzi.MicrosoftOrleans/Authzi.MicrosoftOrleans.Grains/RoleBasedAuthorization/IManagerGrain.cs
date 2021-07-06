using System.Threading.Tasks;
using Authzi.Security.Authorization;
using Orleans;

namespace Authzi.MicrosoftOrleans.Grains.RoleBasedAuthorization
{
    public interface IManagerGrain : IGrainWithStringKey
    {
        [Authorize(Roles = "Developer, Manager")]
        Task<string> GetWithCommaSeparatedRoles(string secret);
        
        [Authorize(Roles = "Developer")]
        [Authorize(Roles = "Manager")]
        Task<string> GetWithMultipleRoleAttributes(string secret);
    }
}