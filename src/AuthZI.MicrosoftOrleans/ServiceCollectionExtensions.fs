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

[<Extension>]
type internal ServiceCollectionExtensions =
  static member private RegisterServices
    (services: IServiceCollection, configure: Action<Configuration>, configureServices: Action<IServiceCollection>) =
    if isNull (box services) then
      nullArg (nameof services)

    if isNull (box configure) then
      nullArg (nameof configure)

    let configuration = Configuration()
    configure.Invoke(configuration)

    if isNull (configuration.ConfigureAuthorizationOptions) then
      configuration.ConfigureAuthorizationOptions <- fun _ -> ()

    services.AddAuthorizationCore(configuration.ConfigureAuthorizationOptions)
    |> ignore

    services.TryAdd(ServiceDescriptor.Singleton<IAuthorizationExecutor, AuthorizationExecutor>(): ServiceDescriptor)

    if not (isNull configureServices) then
      configureServices.Invoke(services)

    // Access Token verification section.
    let accessTokenVerifierOptions = AccessTokenVerifierOptions()

    if not (isNull configuration.ConfigureAccessTokenVerifierOptions) then
      configuration.ConfigureAccessTokenVerifierOptions.Invoke(accessTokenVerifierOptions)

    services.Add(ServiceDescriptor.Singleton(accessTokenVerifierOptions))

    services.AddTransient<AccessTokenExtractor>() |> ignore
    services.AddTransient<AdmissionExecutor>() |> ignore
    services.TryAddSingleton<IAccessTokenVerifier, DefaultAccessTokenVerifier>()
    services.TryAddScoped<SecureGrainContext>()

    let memoryCacheOptions = MemoryCacheOptions()

    services.AddSingleton<IAccessTokenCache>(
      Func<IServiceProvider, IAccessTokenCache>(fun _ -> AccessTokenCache(memoryCacheOptions) :> IAccessTokenCache)
    )
    |> ignore

  [<Extension>]
  static member internal AddClientAuthorization(services: IServiceCollection, configure: Action<Configuration>) =
    if isNull (box services) then
      nullArg (nameof services)

    if isNull (box configure) then
      nullArg (nameof configure)

    services.AddSingleton<IOutgoingGrainCallFilter, AccessTokenSetterFilter>()
    |> ignore

    ServiceCollectionExtensions.RegisterServices(services, configure, null)

  [<Extension>]
  static member internal AddAuthorization
    (
      services: IServiceCollection,
      configure: Action<Configuration>,
      authorizationConfiguration: AuthorizationConfiguration
    ) =

    if isNull (box services) then
      nullArg (nameof services)

    if isNull (box configure) then
      nullArg (nameof configure)

    if isNull (box authorizationConfiguration) then
      nullArg (nameof authorizationConfiguration)

    services.AddSingleton(authorizationConfiguration) |> ignore

    ServiceCollectionExtensions.RegisterServices(services, configure, null)
