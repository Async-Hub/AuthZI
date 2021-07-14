namespace Authzi.AzureActiveDirectory.MicrosoftOrleans

open Authzi.Security
open Authzi.Security.AccessToken
open Microsoft.Extensions.DependencyInjection
open System
open System.Runtime.CompilerServices
open System.Net.Http
open Authzi.AzureActiveDirectory

[<Extension>]
type IdentityServer4ServiceCollectionExtensions = 
    [<Extension>]
    static member inline AddOrleansAzureActiveDirectoryAuthorization(services: IServiceCollection,
        azureActiveDirectoryApp: AzureActiveDirectoryApp(*, configure: Action<Configuration>*)) =
            if isNull (box azureActiveDirectoryApp) then nullArg(nameof azureActiveDirectoryApp)
            //if isNull configure then nullArg(nameof configure)
            
            services.AddSingleton(azureActiveDirectoryApp) |> ignore
            //services.AddTransient<IAccessTokenIntrospectionService, AccessTokenIntrospectionService>() |> ignore

            // Add Discovery document provider.
            //let providerFunc (provider: IServiceProvider) =
            //    let httpClientFactory = provider.GetRequiredService<IHttpClientFactory>()
            //    let discoveryEndpointUrl = provider.GetRequiredService<IdentityServer4Info>().DiscoveryEndpointUrl
            //    let securityOptions = provider.GetRequiredService<SecurityOptions>()
            //    DiscoveryDocumentProvider(httpClientFactory, discoveryEndpointUrl, securityOptions)

            //services.AddSingleton<DiscoveryDocumentProvider>(fun provider -> providerFunc(provider)) |> ignore


                

