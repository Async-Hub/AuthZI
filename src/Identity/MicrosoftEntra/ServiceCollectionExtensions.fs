namespace Authzi.MicrosoftEntra

open Authzi.Security
open Authzi.Security.AccessToken
open Microsoft.Extensions.DependencyInjection
open System.Runtime.CompilerServices

[<Extension>]
type ServiceCollectionExtensions = 
    [<Extension>]
    static member AddAzureActiveDirectoryAuthorization(services: IServiceCollection,
        azureActiveDirectoryApp: AzureActiveDirectoryApp) =
            // Check parameters that might come from C#
            if isNull (box azureActiveDirectoryApp) then nullArg(nameof azureActiveDirectoryApp)
            
            services.AddSingleton(azureActiveDirectoryApp) |> ignore
            services.AddSingleton<IClaimTypeResolver, ClaimTypeResolverDefault>() |> ignore
            services.AddTransient<IAccessTokenIntrospectionService, AccessTokenIntrospectionService>() |> ignore

            // Add Discovery document provider.
            // https://docs.microsoft.com/en-us/azure/active-directory/develop/v2-protocols-oidc
            let discoveryDocumentProvider = DiscoveryDocumentProvider(azureActiveDirectoryApp.DiscoveryEndpointUrl)

            services.AddSingleton<DiscoveryDocumentProvider>(discoveryDocumentProvider) |> ignore


                

