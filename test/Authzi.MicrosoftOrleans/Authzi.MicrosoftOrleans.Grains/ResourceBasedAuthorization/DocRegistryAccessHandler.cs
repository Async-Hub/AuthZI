using System.Threading.Tasks;
using Authzi.Security.Authorization;

namespace Authzi.MicrosoftOrleans.Grains.ResourceBasedAuthorization
{
    public class DocRegistryAccessHandler : AuthorizationHandler<DocRegistryAccessRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, 
            DocRegistryAccessRequirement requirement)
        {
            // ReSharper disable once InvertIf
            if (context.User.HasClaim(c => c.Type == DocRegistryAccessClaim.Name))
            {
                var claim = context.User.FindFirst(c => c.Type == DocRegistryAccessClaim.Name);

                if (claim.Value == DocRegistryAccessClaim.Value)
                {
                    context.Succeed(requirement);
                }
            }

            return Task.CompletedTask;
        }
    }
}