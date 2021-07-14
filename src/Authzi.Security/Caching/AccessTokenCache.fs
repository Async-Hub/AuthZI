namespace Authzi.Security.Caching

open System
open Microsoft.Extensions.Caching.Memory
open Microsoft.Extensions.Options

type AccessTokenCache(memoryCacheOptions: IOptions<MemoryCacheOptions>) =
    do
        if isNull memoryCacheOptions then raise(new ArgumentNullException("memoryCacheOptions"))
    interface IAccessTokenCache with
        member val Current : IMemoryCache = new MemoryCache(memoryCacheOptions) :> IMemoryCache