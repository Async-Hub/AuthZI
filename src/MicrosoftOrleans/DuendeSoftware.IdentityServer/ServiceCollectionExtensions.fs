namespace Authzi.MicrosoftOrleans.DuendeSoftware.IdentityServer

open Authzi.Identity.DuendeSoftware.IdentityServer
open Authzi.MicrosoftOrleans
open Authzi.Security
open Microsoft.Extensions.DependencyInjection
open System
open System.Runtime.CompilerServices
open System.Runtime.InteropServices

type ServiceCollectionExtensions = 
    [<Extension>]
    static member inline AddOrleansAuthorization(services: IServiceCollection, 
        identityServerConfig: IdentityServerConfig,
        configure: Action<Configuration>, [<Optional; DefaultParameterValue(false)>] isCoHostedClient) =
        
        // Check parameters that might come from C#
        if isNull (box services) then nullArg(nameof services)
        if isNull (box identityServerConfig) then nullArg(nameof identityServerConfig)
        if isNull (box configure) then nullArg(nameof configure)

        services.AddAuthorization(configure, isCoHostedClient);
        services.AddIdentityServer4Authorization(identityServerConfig)

    // For the production usage.
    [<Extension>]
    static member inline AddOrleansClientAuthorization(services: IServiceCollection,
        identityServerConfig: IdentityServerConfig,
        configure: Action<Configuration>) =

        // Check parameters that might come from C#
        if isNull (box services) then nullArg(nameof services)
        if isNull (box identityServerConfig) then nullArg(nameof identityServerConfig)
        if isNull (box configure) then nullArg(nameof configure)

        services.AddClientAuthorization(configure)
        services.AddIdentityServer4Authorization(identityServerConfig)