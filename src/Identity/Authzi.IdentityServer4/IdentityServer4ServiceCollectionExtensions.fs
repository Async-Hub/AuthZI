namespace Authzi.IdentityServer4.MicrosoftOrleans

open Authzi.IdentityServer4
open Authzi.Security
open Authzi.Security.AccessToken
open Microsoft.Extensions.DependencyInjection
open System
open System.Runtime.CompilerServices
open System.Net.Http

[<Extension>]
type IdentityServer4ServiceCollectionExtensions = 
    [<Extension>]
    static member AddIdentityServer4Authorization(services: IServiceCollection,
        identityServer4Info: IdentityServer4Info(*, configure: Action<Configuration>*)) =
            if isNull (box identityServer4Info) then nullArg(nameof identityServer4Info)
            //if isNull configure then nullArg(nameof configure)
            
            services.AddSingleton(identityServer4Info) |> ignore
            services.AddSingleton<IClaimTypeResolver, ClaimTypeResolver>() |> ignore
            services.AddTransient<IAccessTokenIntrospectionService, AccessTokenIntrospectionService>() |> ignore

            // Add Discovery document provider.
            let providerFunc (provider: IServiceProvider) =
                let httpClientFactory = provider.GetRequiredService<IHttpClientFactory>()
                let discoveryEndpointUrl = provider.GetRequiredService<IdentityServer4Info>().DiscoveryEndpointUrl
                let securityOptions = provider.GetRequiredService<SecurityOptions>()
                DiscoveryDocumentProvider(httpClientFactory, discoveryEndpointUrl, securityOptions)

            services.AddSingleton<DiscoveryDocumentProvider>(fun provider -> providerFunc(provider)) |> ignore


                

