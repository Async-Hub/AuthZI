namespace AuthZI.MicrosoftOrleans.Duende.IdentityServer

open AuthZI.Identity.Duende.IdentityServer
open AuthZI.MicrosoftOrleans
open AuthZI.MicrosoftOrleans.Authorization
open AuthZI.Security
open Microsoft.Extensions.DependencyInjection
open System
open System.Runtime.CompilerServices
open System.Runtime.InteropServices
open System.Threading.Tasks

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

    [<Extension>]
    static member AddOrleansAuthorization(services: IServiceCollection, 
        identityServerConfig: IdentityServerConfig,
        configure: Action<Configuration>, authorizationConfiguration: AuthorizationConfiguration) =
        
        // Check parameters that might come from C#
        if isNull (box services) then nullArg(nameof services)
        if isNull (box identityServerConfig) then nullArg(nameof identityServerConfig)
        if isNull (box configure) then nullArg(nameof configure)

        let securityOptions = ConfigurationExtensions.getSecurityOptions(configure)

        if not authorizationConfiguration.IsCoHostingEnabled then
            let accessTokenProvider=
              { new IAccessTokenProvider with member this.RetrieveTokenAsync() = Task.FromResult(String.Empty) }
            services.AddSingleton<IAccessTokenProvider>(accessTokenProvider) |> ignore

        services.AddAuthorization(configure, authorizationConfiguration);
        services.AddIdentityServerAuthorization(identityServerConfig, securityOptions)