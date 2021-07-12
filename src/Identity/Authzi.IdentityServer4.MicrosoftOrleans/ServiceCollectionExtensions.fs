namespace Authzi.IdentityServer4.MicrosoftOrleans

open Authzi.IdentityServer4
open Microsoft.Extensions.DependencyInjection
open System.Runtime.CompilerServices

[<Extension>]
type ServiceCollectionExtensions = 
    [<Extension>]
    static member inline AddOrleansIdentityServer4Authorization(services: IServiceCollection,
        identityServer4Info: IdentityServer4Info(*, configure: Action<Configuration>*)) =
            if isNull (box identityServer4Info) then nullArg(nameof identityServer4Info)

            services.AddIdentityServer4Authorization(identityServer4Info) |> ignore


                

