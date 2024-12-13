namespace AuthZI.MicrosoftOrleans

open AuthZI.MicrosoftOrleans.Authorization
open AuthZI.Security
open AuthZI.Security.AccessToken
open AuthZI.Security.Authorization
open AuthZI.Security.Caching
open Microsoft.Extensions.Caching.Memory
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.DependencyInjection.Extensions
open Orleans
open System
open System.Runtime.CompilerServices
open System.Runtime.InteropServices

[<Extension>]
type internal ServiceCollectionExtensions = 
    static member private RegisterServices(services: IServiceCollection,
        configure: Action<Configuration>, configureServices:  Action<IServiceCollection>) =
        if isNull (box services) then nullArg(nameof services)
        if isNull (box configure) then nullArg(nameof configure)

        let configuration = Configuration()
        configure.Invoke(configuration)

        if isNull (configuration.ConfigureAuthorizationOptions)
           then configuration.ConfigureAuthorizationOptions <- fun _ -> ()

        services.AddAuthorizationCore(configuration.ConfigureAuthorizationOptions) |> ignore
        services.TryAdd(ServiceDescriptor.Singleton<IAuthorizationExecutor, AuthorizationExecutor>(): ServiceDescriptor)

        if not (isNull configureServices) then configureServices.Invoke(services)

        // Configure Security.
        let securityOptions = SecurityOptions()
        if not (isNull configuration.ConfigureSecurityOptions) then 
            configuration.ConfigureSecurityOptions.Invoke(securityOptions)
        services.Add(ServiceDescriptor.Singleton(securityOptions))

        // Access Token verification section.
        let accessTokenVerifierOptions = AccessTokenVerifierOptions()
        if not (isNull configuration.ConfigureAccessTokenVerifierOptions) then
            configuration.ConfigureAccessTokenVerifierOptions.Invoke(accessTokenVerifierOptions)
        services.Add(ServiceDescriptor.Singleton(accessTokenVerifierOptions))

        services.TryAddSingleton<IAccessTokenVerifier, DefaultAccessTokenVerifier>()
        services.TryAddScoped<SecureGrainContext>()
        
        let memoryCacheOptions = MemoryCacheOptions()
        services.AddSingleton<IAccessTokenCache>(Func<IServiceProvider, IAccessTokenCache>(fun _ -> 
            AccessTokenCache(memoryCacheOptions) :> IAccessTokenCache)) |> ignore

    [<Extension>]
    static member internal AddAuthorization(services: IServiceCollection,
        configure: Action<Configuration>, [<Optional; DefaultParameterValue(false)>] isCoHostedClient) =
        
        if isNull (box services) then nullArg(nameof services)
        if isNull (box configure) then nullArg(nameof configure)

        // For the Orleans co-hosted clients usage.
        // https://dotnet.github.io/orleans/docs/host/client.html
        if isCoHostedClient then
            services.AddSingleton<IOutgoingGrainCallFilter, AccessTokenSetterFilter>() |> ignore

        services.AddSingleton<IIncomingGrainCallFilter, IncomingGrainCallAuthorizationFilter>() |> ignore
        ServiceCollectionExtensions.RegisterServices(services, configure, null)

    [<Extension>]
    static member internal AddClientAuthorization(services: IServiceCollection,
        configure: Action<Configuration>) =
        if isNull (box services) then nullArg(nameof services)
        if isNull (box configure) then nullArg(nameof configure)

        services.AddSingleton<IOutgoingGrainCallFilter, AccessTokenSetterFilter>() |> ignore
        services.AddSingleton<IOutgoingGrainCallFilter, OutgoingGrainCallAuthorizationFilter>() |> ignore
        
        ServiceCollectionExtensions.RegisterServices(services, configure, null)

    [<Extension>]
    static member internal AddAuthorizationNew(services: IServiceCollection,
        configure: Action<Configuration>, orleansAuthorizationConfiguration: OrleansAuthorizationConfiguration) =
        
        if isNull (box services) then nullArg(nameof services)
        if isNull (box configure) then nullArg(nameof configure)
        if isNull (box orleansAuthorizationConfiguration) then nullArg(nameof orleansAuthorizationConfiguration)

        services.AddSingleton(orleansAuthorizationConfiguration) |> ignore

        ServiceCollectionExtensions.RegisterServices(services, configure, null)