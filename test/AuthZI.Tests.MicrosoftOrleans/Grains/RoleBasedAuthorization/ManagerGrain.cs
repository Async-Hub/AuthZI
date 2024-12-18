using System.Threading.Tasks;
using AuthZI.MicrosoftOrleans.Authorization;

namespace AuthZI.Tests.MicrosoftOrleans.Grains.RoleBasedAuthorization
{
  public class ManagerGrain(SecureGrainContext secureGrainContext) :
    SecureGrain(secureGrainContext), IManagerGrain
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