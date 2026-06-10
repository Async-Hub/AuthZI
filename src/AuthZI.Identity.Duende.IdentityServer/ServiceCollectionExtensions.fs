namespace AuthZI.Identity.Duende.IdentityServer

open AuthZI.Security
open AuthZI.Security.AccessToken
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.DependencyInjection.Extensions
open System
open System.Net.Http
open System.Runtime.CompilerServices

type ServiceCollectionExtensions =
  [<Extension>]
  static member AddIdentityServerAuthorization
    (services: IServiceCollection, identityServerConfig: IdentityServerConfig)
    =
    if isNull (box identityServerConfig) then
      nullArg (nameof identityServerConfig)

    services.AddSingleton(identityServerConfig) |> ignore
    services.AddSingleton<IClaimTypeResolver, ClaimTypeResolver>() |> ignore

    services.AddTransient<IAccessTokenIntrospectionService, AccessTokenIntrospectionService>()
    |> ignore

    services.AddHttpClient("IdS") |> ignore

    // Register AccessTokenVerifierOptions if not already registered (e.g. by the Orleans layer).
    services.TryAddSingleton<AccessTokenVerifierOptions>()

    // Add Discovery document provider.
    let providerFunc (provider: IServiceProvider) =
      let httpClientFactory = provider.GetRequiredService<IHttpClientFactory>()

      let discoveryEndpointUrl =
        provider.GetRequiredService<IdentityServerConfig>().DiscoveryEndpointUrl

      DiscoveryDocumentProvider(httpClientFactory, discoveryEndpointUrl)

    services.AddSingleton<DiscoveryDocumentProvider>(providerFunc) |> ignore