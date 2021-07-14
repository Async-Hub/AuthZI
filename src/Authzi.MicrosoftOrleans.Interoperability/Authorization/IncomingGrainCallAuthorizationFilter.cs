﻿using System.Security.Claims;
using System.Threading.Tasks;
using Authzi.Security;
using Authzi.Security.AccessToken;
using Authzi.Security.Authorization;
using IdentityModel;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Runtime;

namespace Authzi.MicrosoftOrleans.Authorization
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class IncomingGrainCallAuthorizationFilter : GrainAuthorizationFilterBase, IIncomingGrainCallFilter
    {
        public IncomingGrainCallAuthorizationFilter(IAccessTokenVerifier accessTokenVerifier,
            // ReSharper disable once SuggestBaseTypeForParameter
            IAuthorizationExecutor authorizeHandler, ILogger<IncomingGrainCallAuthorizationFilter> logger)
            : base(accessTokenVerifier, authorizeHandler)
        {
            Logger = logger;
        }

        public async Task Invoke(IIncomingGrainCallContext context)
        {
            if (AuthorizationAdmission.IsRequired(context))
            {
                var claims = await AuthorizeAsync(context);
                
                var grainType = context.Grain.GetType();

                if (grainType.BaseType == typeof(GrainWithClaimsPrincipal))
                {
                    var claimsIdentity = new ClaimsIdentity(claims, 
                        "", JwtClaimTypes.Subject, JwtClaimTypes.Role);
                    
                    var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                    RequestContext.Set(ConfigurationKeys.ClaimsPrincipalKey, claimsPrincipal);
                }
                
                Log(LoggingEvents.IncomingGrainCallAuthorizationPassed,
                    grainType.Name, context.InterfaceMethod.Name);
            }

            await context.Invoke();
        }
    }
}
