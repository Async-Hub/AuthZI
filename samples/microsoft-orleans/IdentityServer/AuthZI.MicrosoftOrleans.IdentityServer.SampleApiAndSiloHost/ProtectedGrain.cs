using AuthZI.MicrosoftOrleans.Authorization;
using AuthZI.Security.Authorization;

namespace AuthZI.MicrosoftOrleans.IdentityServer.SampleApiAndSiloHost;

public interface IProtectedGrain : IGrainWithStringKey
{
  [Authorize(Roles = "Admin")]
  Task<string> TakeSecret();
}

public class ProtectedGrain(SecureGrainContext secureGrainContext) : 
  SecureGrain(secureGrainContext), IProtectedGrain
{
  public Task<string> TakeSecret()
  {
    return Task.FromResult("Success! You see the data which is available only for Admin role.");
  }
}