namespace Authzi.MicrosoftOrleans

open Authzi.MicrosoftOrleans.Authorization
open Authzi.Security
open Authzi.Security.AccessToken
open Authzi.Security.Authorization
open Authzi.Security.Caching
open Microsoft.Extensions.Caching.Memory
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.DependencyInjection.Extensions
open Microsoft.Extensions.Http
open Orleans
open System
open System.Net.Http
open System.Net.Security
open System.Runtime.CompilerServices
open System.Runtime.InteropServices
open System.Security.Cryptography.X509Certificates

module HttpClientConfiguration =
    let configureHttpMessageHandlerBuilder (builder: HttpMessageHandlerBuilder, securityOptions: SecurityOptions) =
        let httpClientHandler = new HttpClientHandler()
        builder.PrimaryHandler <- httpClientHandler

        if not securityOptions.RequireHttps then
            httpClientHandler.ServerCertificateCustomValidationCallback <- 
                new Func<HttpRequestMessage, X509Certificate2, X509Chain, SslPolicyErrors, bool>(fun (_) (_) (_) (_) -> true)

[<Extension>]
type ServiceCollectionExtensions = 
    static member inline private RegisterServices(services: IServiceCollection,
        configure: Action<Configuration>, configureServices:  Action<IServiceCollection>) =
        if isNull (box services) then nullArg(nameof services)
        if isNull (box configure) then nullArg(nameof configure)

        let configuration = new Configuration()
        configure.Invoke(configuration)

        services.AddAuthorizationCore(configuration.ConfigureAuthorizationOptions) |> ignore
        services.TryAdd(ServiceDescriptor.Singleton<IAuthorizationExecutor, AuthorizationExecutor>(): ServiceDescriptor)

        if not (isNull configureServices) then configureServices.Invoke(services)

        // Configure Security.
        let securityOptions = new SecurityOptions()
        if not (isNull configuration.ConfigureSecurityOptions) then 
            configuration.ConfigureSecurityOptions.Invoke(securityOptions)
        services.Add(ServiceDescriptor.Singleton(securityOptions))

        // Access Token verification section.
        let accessTokenVerifierOptions = new AccessTokenVerifierOptions()
        if not (isNull configuration.ConfigureAccessTokenVerifierOptions) then
            configuration.ConfigureAccessTokenVerifierOptions.Invoke(accessTokenVerifierOptions)
        services.Add(ServiceDescriptor.Singleton(accessTokenVerifierOptions))

        services.TryAddSingleton<IAccessTokenVerifier,DefaultAccessTokenVerifier>()
        services.AddHttpClient("IdS4").ConfigureHttpMessageHandlerBuilder(fun builder -> 
            HttpClientConfiguration.configureHttpMessageHandlerBuilder(builder,securityOptions)) |> ignore

        let memoryCacheOptions = new MemoryCacheOptions()
        services.AddSingleton<IAccessTokenCache>(new Func<IServiceProvider, IAccessTokenCache>(fun _ -> 
            new AccessTokenCache(memoryCacheOptions) :> IAccessTokenCache)) |> ignore

    [<Extension>]
    static member inline AddAuthorization(services: IServiceCollection,
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
    static member inline AddClientAuthorization(services: IServiceCollection,
        configure: Action<Configuration>) =
        if isNull (box services) then nullArg(nameof services)
        if isNull (box configure) then nullArg(nameof configure)

        services.AddSingleton<IOutgoingGrainCallFilter, AccessTokenSetterFilter>() |> ignore
        services.AddSingleton<IOutgoingGrainCallFilter, OutgoingGrainCallAuthorizationFilter>() |> ignore
        
        ServiceCollectionExtensions.RegisterServices(services, configure, null)