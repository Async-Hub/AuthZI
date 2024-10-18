using AuthZI.Security.Authorization;
using AuthZI.Tests.MicrosoftOrleans.Grains.ResourceBasedAuthorization;
using Microsoft.Extensions.DependencyInjection;

namespace AuthZI.Tests.MicrosoftOrleans.Grains
{
    // ReSharper disable once UnusedType.Global
    public static class AuthorizationConfig
    {
        internal const string DocumentModifyAccessPolicy = "DocumentModifyAccess";
        
        // ReSharper disable once UnusedMember.Global
        public static void ConfigureOptions(AuthorizationOptions options)
        {
            options.AddPolicy("AdminPolicy", policy => policy.RequireRole("Admin"));
            options.AddPolicy("ManagerPolicy", policy => policy.RequireRole("Manager"));
            options.AddPolicy("ArmeniaCountryOnly", 
                policy => policy.RequireClaim("Country", "AM"));
            
            options.AddPolicy("DocRegistryAccess", 
                policy => policy.AddRequirements(new DocRegistryAccessRequirement()));
            
            options.AddPolicy(DocumentModifyAccessPolicy, 
                policy => policy.AddRequirements(new SameAuthorRequirement()));
        }
        
        public static void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IAuthorizationHandler, DocModifyAuthorizationHandler>();
            services.AddSingleton<IAuthorizationHandler, DocRegistryAccessHandler>();
        }
    }
}
