namespace AuthZI.Identity.Duende.IdentityServer

open AuthZI.Security
open AuthZI.Security.AccessToken
open Microsoft.Extensions.DependencyInjection
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

    services.AddHttpClient("IdS4") |> ignore

    // Add Discovery document provider.
    let providerFunc (provider: IServiceProvider) =
      let httpClientFactory = provider.GetRequiredService<IHttpClientFactory>()

      let discoveryEndpointUrl =
        provider.GetRequiredService<IdentityServerConfig>().DiscoveryEndpointUrl

      DiscoveryDocumentProvider(httpClientFactory, discoveryEndpointUrl)

    services.AddSingleton<DiscoveryDocumentProvider>(providerFunc) |> ignore