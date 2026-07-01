# Simple authorization in a Microsoft Orleans cluster

Authorization in Orleans is controlled through `AuthorizeAttribute` and related authorization options. At the simplest level, applying `AuthorizeAttribute` to a grain interface or a grain interface method limits access to authenticated users.

For example, the following code limits access to `IUserGrain` to authenticated users.

```csharp
[Authorize]
public interface IUserGrain : IGrainWithStringKey
{
    Task<string> DoSomething();

    Task<string> DoSomethingElse();
}
```

If you want to apply authorization to a method rather than the full grain contract, apply `AuthorizeAttribute` to the method:

```csharp
public interface IUserGrain : IGrainWithStringKey
{
    [Authorize]
    Task<string> DoSomething();

    Task<string> DoSomethingElse();
}
```

Now only authenticated users can access `DoSomething`, while `DoSomethingElse` remains available to all callers.

You can also use `AllowAnonymous` to allow access by unauthenticated users to specific methods. For example:

```csharp
[Authorize]
public interface IUserGrain : IGrainWithStringKey
{
    [AllowAnonymous]
    Task<string> DoSomething();

    Task<string> DoSomethingElse();
}
```

This configuration allows only authenticated users to access the grain, except for `DoSomething`, which is accessible to anonymous callers.

### Warning

**`[AllowAnonymous]` bypasses authorization checks. If you combine `[AllowAnonymous]` with `[Authorize]`, the authorization attributes are ignored for that scope. For example, if you apply `[AllowAnonymous]` at the grain level, any `[Authorize]` attributes on the same grain (or on methods within it) are ignored.**
