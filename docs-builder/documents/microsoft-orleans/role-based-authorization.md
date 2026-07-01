# Role-based authorization

When an identity is created, it may belong to one or more roles. For example, Alice may belong to the Administrator and User roles while Bob may only belong to the User role. How roles are created and managed depends on your identity provider configuration.

Role-based authorization checks are declarative. The developer adds attributes to a grain interface or interface method, specifying the roles that the current user or client must have to access the resource.

For example, the following code limits access to all methods on `IUserGrain` to users or clients in the `Administrator` role:

```csharp
[Authorize(Roles = "Administrator")]
public interface IUserGrain : IGrainWithStringKey
{
    Task<string> DoSomething();

    Task<string> DoSomethingElse();
}
```

You can specify multiple roles as a comma-separated list:

```csharp
[Authorize(Roles = "Administrator, Manager")]
public interface IUserGrain : IGrainWithStringKey
{
    Task<string> DoSomething();

    Task<string> DoSomethingElse();
}
```

This grain is accessible only to users or clients who are members of either the `Administrator` role or the `Manager` role.

If you apply multiple role attributes, the caller must satisfy all specified role checks. The following sample requires membership in both `Developer` and `Manager`.

```csharp
[Authorize(Roles = "Developer")]
[Authorize(Roles = "Manager")]
public interface IUserGrain : IGrainWithStringKey
{
    Task<string> DoSomething();

    Task<string> DoSomethingElse();
}
```

You can further limit access by applying additional role authorization attributes at the method level:

```csharp
[Authorize(Roles = "Developer")]
[Authorize(Roles = "Manager")]
public interface IUserGrain : IGrainWithStringKey
{
    Task<string> DoSomething();

    [Authorize(Roles = "Manager")]
    Task<string> DoSomethingElse();
}
```

In the previous example, members of either `Developer` or `Manager` can access the grain and `DoSomething`, but only members of `Manager` can access `DoSomethingElse`.

You can also lock down a grain but allow anonymous, unauthenticated access to individual methods.

```csharp
[Authorize]
public interface IUserGrain : IGrainWithStringKey
{
    Task<string> DoSomething();

    [AllowAnonymous]
    Task<string> DoSomethingElse();
}
```