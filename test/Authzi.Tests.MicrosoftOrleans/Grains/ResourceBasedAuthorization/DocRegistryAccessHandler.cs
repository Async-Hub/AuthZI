using Authzi.Security.Authorization;
using System.Threading.Tasks;

namespace Authzi.Tests.MicrosoftOrleans.Grains.ResourceBasedAuthorization
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

                if (claim is { Value: DocRegistryAccessClaim.Value })
                {
                    context.Succeed(requirement);
                }
            }

            return Task.CompletedTask;
        }
    }
}