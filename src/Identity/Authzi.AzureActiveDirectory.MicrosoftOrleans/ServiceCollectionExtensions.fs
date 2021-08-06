namespace Authzi.AzureActiveDirectory.MicrosoftOrleans

open Authzi.AzureActiveDirectory
open Microsoft.Extensions.DependencyInjection
open System.Runtime.CompilerServices

[<Extension>]
type ServiceCollectionExtensions = 
    [<Extension>]
    static member inline AddOrleansAzureActiveDirectoryAuthorization(services: IServiceCollection,
        azureActiveDirectoryApp: AzureActiveDirectoryApp) =
            // Check parameters that might come from C#
            if isNull (box azureActiveDirectoryApp) then nullArg(nameof azureActiveDirectoryApp)

            services.AddAzureActiveDirectoryAuthorization(azureActiveDirectoryApp) |> ignore


                

