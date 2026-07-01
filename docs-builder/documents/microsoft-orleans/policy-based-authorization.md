# Policy-based authorization

An authorization policy consists of one or more requirements. In AuthZI, policies are configured through the Orleans registration extensions and `AuthorizationOptions`.

```csharp
services.AddOrleansAuthorization(
    identityServerConfig,
    config =>
    {
        config.ConfigureAuthorizationOptions = options =>
        {
            options.AddPolicy("AdminPolicy", policy => policy.RequireRole("Admin"));
        };
    },
    new AuthorizationConfiguration(isCoHostingEnabled: true));
```

In the preceding example, an "AdminPolicy" policy is created.

Policies are applied using the `[Authorize]` attribute with the policy name. For example:

```csharp
[Authorize(Policy = "AdminPolicy")]
public interface IUserGrain : IGrainWithStringKey
{
    Task<string> DoSomething();
}
```

## Requirements

An authorization requirement is a collection of data parameters that a policy can use to evaluate the current user/client principal. A requirement implements `IAuthorizationRequirement`, which is an empty marker interface.

```csharp
using AuthZI.Security.Authorization;

public class EmailVerifiedRequirement : IAuthorizationRequirement
{
    public bool IsEmailVerified { get; }

    public EmailVerifiedRequirement(bool isEmailVerified)
    {
        IsEmailVerified = isEmailVerified;
    }
}
```

Note: a requirement does not need to have data or properties.

## Authorization handlers

An authorization handler evaluates a requirement against an `AuthorizationHandlerContext` to determine whether access should be granted.

A requirement can have multiple handlers. A handler can inherit `AuthorizationHandler<TRequirement>`, where `TRequirement` is the requirement type, or implement `IAuthorizationHandler` to handle multiple requirement types.

### **Use a handler for one requirement**

The following is an example of a one-to-one relationship in which an email verified handler uses a single requirement:

```csharp
public class EmailVerifiedHandler : AuthorizationHandler<EmailVerifiedRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
        EmailVerifiedRequirement requirement)
    {
        if (context.User.HasClaim(c => c.Type == "email_verified"))
        {
            var claim = context.User.FindFirst(c => c.Type == "email_verified");
            var isEmailVerified = Convert.ToBoolean(claim.Value);

            if (isEmailVerified == requirement.IsEmailVerified)
            {
                context.Succeed(requirement);
            }
        }

        return Task.CompletedTask;
    }
}
```

The preceding code checks whether the current principal has an `email_verified` claim and whether it satisfies the requirement. When successful, `context.Succeed` is invoked.

### **Use a handler for multiple requirements**

The following is an example of a one-to-many relationship in which a handler evaluates two requirement types:

```csharp
public class LocationRequirement : IAuthorizationRequirement
{
    public string Location { get; private set; }

    public LocationRequirement(string location)
    {
        Location = location;
    }
}

public class RoleIsPresentRequirement : IAuthorizationRequirement
{
    public string Role { get; private set; }

    public RoleIsPresentRequirement(string role)
    {
        Role = role;
    }
}

public class RoleAndLocationCombinationHandler : IAuthorizationHandler
{
    public Task HandleAsync(AuthorizationHandlerContext context)
    {
        var pendingRequirements = context.PendingRequirements.ToList();

        foreach (var requirement in pendingRequirements)
        {
            switch (requirement)
            {
                case RoleIsPresentRequirement roleIsPresentRequirement:
                {
                    if (context.User.IsInRole(roleIsPresentRequirement.Role))
                    {
                        context.Succeed(roleIsPresentRequirement);
                    }

                    break;
                }
                case LocationRequirement locationRequirement:
                {
                    if (context.User.HasClaim(c => c.Type == "location"))
                    {
                        var claim = context.User.FindFirst(c => c.Type == "location");
                        if (claim.Value == locationRequirement.Location)
                        {
                            context.Succeed(requirement);
                        }
                    }

                    break;
                }
            }
        }

        return Task.CompletedTask;
    }
}
```

The preceding code traverses `PendingRequirements` and calls `context.Succeed` when each requirement is met.

### Handler registration

Handlers are registered in the service collection during configuration. For example:

```csharp
.ConfigureServices(services =>
{
    services.AddOrleansAuthorization(identityServerConfig, config =>
    {
        config.ConfigureAuthorizationOptions = options =>
            options.AddPolicy("AdminPolicy", policy => policy.RequireRole("Admin"));
    }, new AuthorizationConfiguration(isCoHostingEnabled: true));

    services.AddSingleton<IAuthorizationHandler, EmailVerifiedHandler>();
    services.AddSingleton<IAuthorizationHandler, RoleAndLocationCombinationHandler>();
})
```

### What should a handler return?

Note that the `Handle` method in the handler example returns no value. Status is indicated through the context.

- A handler indicates success by calling `context.Succeed(requirement)`.
- A handler can leave a requirement unresolved if it does not apply.
- To guarantee failure, call `context.Fail()`.

### Why would I want multiple handlers for a requirement?

If you want OR-style behavior, implement multiple handlers for a single requirement.

For additional information, see the [ASP.NET Core policy-based authorization documentation](https://learn.microsoft.com/en-us/aspnet/core/security/authorization/policies).