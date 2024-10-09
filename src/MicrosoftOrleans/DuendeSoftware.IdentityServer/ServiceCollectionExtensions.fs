namespace Authzi.MicrosoftOrleans.DuendeSoftware.IdentityServer

open Authzi.Identity.DuendeSoftware.IdentityServer
open Authzi.MicrosoftOrleans
open Authzi.Security
open Microsoft.Extensions.DependencyInjection
open System
open System.Runtime.CompilerServices
open System.Runtime.InteropServices

module internal ConfigurationExtensions =
    let getSecurityOptions (configure: Action<Configuration>) =
        let configuration = Configuration()
        configure.Invoke(configuration)
        let securityOptions = SecurityOptions()
        if not (isNull configuration.ConfigureSecurityOptions) then 
            configuration.ConfigureSecurityOptions.Invoke(securityOptions)
        securityOptions

type ServiceCollectionExtensions = 
    [<Extension>]
    static member AddOrleansAuthorization(services: IServiceCollection, 
        identityServerConfig: IdentityServerConfig,
        configure: Action<Configuration>, [<Optional; DefaultParameterValue(false)>] isCoHostedClient) =
        
        // Check parameters that might come from C#
        if isNull (box services) then nullArg(nameof services)
        if isNull (box identityServerConfig) then nullArg(nameof identityServerConfig)
        if isNull (box configure) then nullArg(nameof configure)

        let securityOptions = ConfigurationExtensions.getSecurityOptions(configure)

        services.AddAuthorization(configure, isCoHostedClient);
        services.AddIdentityServerAuthorization(identityServerConfig, securityOptions)

    // For the production usage.
    [<Extension>]
    static member AddOrleansClientAuthorization(services: IServiceCollection,
        identityServerConfig: IdentityServerConfig,
        configure: Action<Configuration>) =

        // Check parameters that might come from C#
        if isNull (box services) then nullArg(nameof services)
        if isNull (box identityServerConfig) then nullArg(nameof identityServerConfig)
        if isNull (box configure) then nullArg(nameof configure)

        let securityOptions = ConfigurationExtensions.getSecurityOptions(configure)

        services.AddClientAuthorization(configure)
        services.AddIdentityServerAuthorization(identityServerConfig, securityOptions)