using System.Threading.Tasks;
using AuthZI.MicrosoftOrleans.Authorization;

namespace AuthZI.Tests.MicrosoftOrleans.Grains.ClaimsBasedAuthorization
{
  public class ClaimGrain(SecureGrainContext secureGrainContext) :
    SecureGrain(secureGrainContext), IClaimGrain
  {
    public Task<string> DoSomething(string someInput)
    {
      return Task.FromResult<string>(someInput);
    }
  }
}