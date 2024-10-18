using System.Threading.Tasks;
using Orleans;

namespace AuthZI.Tests.MicrosoftOrleans.Grains.RoleBasedAuthorization
{
    public class ManagerGrain : Grain, IManagerGrain
    {
        public Task<string> GetWithCommaSeparatedRoles(string secret)
        {
            return Task.FromResult(secret);
        }

        public Task<string> GetWithMultipleRoleAttributes(string secret)
        {
            return Task.FromResult(secret);
        }
    }
}