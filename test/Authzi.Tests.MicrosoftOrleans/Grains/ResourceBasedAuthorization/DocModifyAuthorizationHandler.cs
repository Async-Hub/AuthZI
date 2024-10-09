using Authzi.Security.Authorization;
using System.Threading.Tasks;

namespace Authzi.Tests.MicrosoftOrleans.Grains.ResourceBasedAuthorization
{
    public sealed class SameAuthorRequirement : IAuthorizationRequirement { }
    
    public class DocModifyAuthorizationHandler : AuthorizationHandler<SameAuthorRequirement, Document>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, 
            SameAuthorRequirement requirement, Document resource)
        {
            if (context.User.Identity?.Name == resource.Author)
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}