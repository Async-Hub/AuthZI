using System.Threading.Tasks;
using AuthZI.MicrosoftOrleans.Authorization;

namespace AuthZI.Tests.MicrosoftOrleans.Grains.SimpleAuthorization
{
  public class SimpleGrain(SecureGrainContext secureGrainContext) :
    SecureGrain(secureGrainContext), ISimpleGrain
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