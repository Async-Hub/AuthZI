using System.Threading.Tasks;
using AuthZI.MicrosoftOrleans.Authorization;

namespace AuthZI.Tests.MicrosoftOrleans.Grains.PolicyBasedAuthorization
{
  public class PolicyGrain(SecureGrainContext secureGrainContext) :
    SecureGrain(secureGrainContext), IPolicyGrain
  {
    public Task<string> GetWithMangerPolicy(string secret)
    {
      return Task.FromResult(secret);
    }
  }
}