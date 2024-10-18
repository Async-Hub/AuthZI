namespace AuthZI.Security.Caching

open Microsoft.Extensions.Caching.Memory

type IAccessTokenCache =
    abstract Current : IMemoryCache with get