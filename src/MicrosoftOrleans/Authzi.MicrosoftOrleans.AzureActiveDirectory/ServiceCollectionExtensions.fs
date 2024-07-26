namespace Authzi.MicrosoftOrleans.AzureActiveDirectory

open Authzi.MicrosoftEntra
open Authzi.MicrosoftOrleans
open Authzi.Security
open Microsoft.Extensions.DependencyInjection
open System
open System.Runtime.CompilerServices
open System.Runtime.InteropServices

[<Extension>]
type ServiceCollectionExtensions = 
    [<Extension>]
    static member inline AddOrleansAuthorization(services: IServiceCollection, 
        azureActiveDirectoryApp: AzureActiveDirectoryApp,
        configure: Action<Configuration>, [<Optional; DefaultParameterValue(false)>] isCoHostedClient) =
        
        // Check parameters that might come from C#
        if isNull (box services) then nullArg(nameof services)
        if isNull (box azureActiveDirectoryApp) then nullArg(nameof azureActiveDirectoryApp)
        if isNull (box configure) then nullArg(nameof configure)

        services.AddAuthorization(configure, isCoHostedClient);
        services.AddAzureActiveDirectoryAuthorization(azureActiveDirectoryApp) |> ignore

    // For the production usage.
    [<Extension>]
    static member inline AddOrleansClientAuthorization(services: IServiceCollection,
        azureActiveDirectoryApp: AzureActiveDirectoryApp,
        configure: Action<Configuration>) =

        // Check parameters that might come from C#
        if isNull (box services) then nullArg(nameof services)
        if isNull (box azureActiveDirectoryApp) then nullArg(nameof azureActiveDirectoryApp)
        if isNull (box configure) then nullArg(nameof configure)

        services.AddClientAuthorization(configure)
        services.AddAzureActiveDirectoryAuthorization(azureActiveDirectoryApp) |> ignore


                

