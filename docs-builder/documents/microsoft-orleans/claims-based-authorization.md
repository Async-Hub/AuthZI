# Claims-based authorization

Claims-based authorization checks whether a principal contains specific claims with expected values.

For Orleans grains, apply claim requirements through authorization policies, then reference those policies on grain interfaces or methods.

## Example policy

```csharp
services.AddOrleansAuthorization(
    identityServerConfig,
    config =>
    {
        config.ConfigureAuthorizationOptions = options =>
        {
            options.AddPolicy("Scope.Read", policy =>
                policy.RequireClaim("scope", "authzi.read"));
        };
    },
    new AuthorizationConfiguration(isCoHostingEnabled: true));
```

## Apply policy to a grain

```csharp
[Authorize(Policy = "Scope.Read")]
public interface IProtectedGrain : IGrainWithStringKey
{
    Task<string> ReadAsync();
}
```

For general claims authorization concepts, see the [ASP.NET Core documentation](https://learn.microsoft.com/en-us/aspnet/core/security/authorization/claims).