using System.Threading.Tasks;
using Authzi.Security;
using Authzi.Security.AccessToken;
using Authzi.Security.Authorization;
using Microsoft.Extensions.Logging;
using Orleans;

namespace Authzi.MicrosoftOrleans.Authorization
{
    public class OutgoingGrainCallAuthorizationFilter : GrainAuthorizationFilterBase, IOutgoingGrainCallFilter
    {
        public OutgoingGrainCallAuthorizationFilter(IAccessTokenVerifier accessTokenVerifier, 
            IAuthorizationExecutor authorizeHandler,
            // ReSharper disable once SuggestBaseTypeForParameter
            ILogger<OutgoingGrainCallAuthorizationFilter> logger) : base(accessTokenVerifier, authorizeHandler)
        {
            Logger = logger;
        }


        public async Task Invoke(IOutgoingGrainCallContext context)
        {
            if (AuthorizationAdmission.IsRequired(context))
            {
                await AuthorizeAsync(context);

                var grainType = context.Grain.GetType();
                Log(LoggingEvents.OutgoingGrainCallAuthorizationPassed,
                    grainType.Name, context.InterfaceMethod.Name);
            }

            await context.Invoke();
        }
    }
}
