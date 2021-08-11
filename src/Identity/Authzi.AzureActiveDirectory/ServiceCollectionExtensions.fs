namespace Authzi.AzureActiveDirectory

open Authzi.AzureActiveDirectory
open Authzi.Security
open Authzi.Security.AccessToken
open Microsoft.Extensions.DependencyInjection
open System
open System.Runtime.CompilerServices

[<Extension>]
type IdentityServer4ServiceCollectionExtensions = 
    [<Extension>]
    static member inline AddAzureActiveDirectoryAuthorization(services: IServiceCollection,
        azureActiveDirectoryApp: AzureActiveDirectoryApp) =
            // Check parameters that might come from C#
            if isNull (box azureActiveDirectoryApp) then nullArg(nameof azureActiveDirectoryApp)
            
            services.AddSingleton(azureActiveDirectoryApp) |> ignore
            services.AddSingleton<IClaimTypeResolver, ClaimTypeResolver>() |> ignore
            services.AddTransient<IAccessTokenIntrospectionService, AccessTokenIntrospectionService>() |> ignore

            // Add Discovery document provider.
            // https://docs.microsoft.com/en-us/azure/active-directory/develop/v2-protocols-oidc
            let providerFunc (provider: IServiceProvider) =
                let discoveryEndpointUrl = 
                    provider.GetRequiredService<AzureActiveDirectoryApp>().DiscoveryEndpointUrl

                DiscoveryDocumentProvider(discoveryEndpointUrl)

            services.AddSingleton<DiscoveryDocumentProvider>(fun provider -> providerFunc(provider)) |> ignore


                

