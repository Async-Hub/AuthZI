using Authzi.Security;
using Newtonsoft.Json;
using Orleans;
using Orleans.Runtime;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Authzi.MicrosoftOrleans
{
    public class GrainWithClaimsPrincipal(IClaimTypeResolver claimTypeResolver) : Grain, IIncomingGrainCallFilter
    {
        private readonly IClaimTypeResolver _claimTypeResolver = claimTypeResolver;
        private string _serializedClaims;

        protected ClaimsPrincipal User => RetrieveUser();

        public async Task Invoke(IIncomingGrainCallContext context)
        {
            _serializedClaims = (string)RequestContext.Get(ConfigurationKeys.ClaimsPrincipalKey);
            await context.Invoke();
        }

        private ClaimsPrincipal RetrieveUser()
        {
            var claims = JsonConvert.DeserializeObject<Claim[]>(_serializedClaims, new ClaimJsonConverter());
            var claimsIdentity = new ClaimsIdentity(claims, string.Empty,
                _claimTypeResolver.Resolve(ClaimType.Subject), 
                _claimTypeResolver.Resolve(ClaimType.Role));

            return new ClaimsPrincipal(claimsIdentity);
        }
    }
}