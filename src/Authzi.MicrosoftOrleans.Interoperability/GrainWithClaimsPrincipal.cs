using Authzi.Security;
using Orleans;
using Orleans.Runtime;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Authzi.MicrosoftOrleans
{
    public class GrainWithClaimsPrincipal : Grain, IIncomingGrainCallFilter
    {
        protected ClaimsPrincipal User;

        public async Task Invoke(IIncomingGrainCallContext context)
        {
            User = (ClaimsPrincipal)RequestContext.Get(ConfigurationKeys.ClaimsPrincipalKey);

            await context.Invoke();
        }
    }
}