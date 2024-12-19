namespace AuthZI.MicrosoftOrleans.MicrosoftEntra

open AuthZI.Identity.MicrosoftEntra
open AuthZI.MicrosoftEntra
open AuthZI.MicrosoftOrleans
open AuthZI.MicrosoftOrleans.Authorization
open AuthZI.Security
open Microsoft.Extensions.DependencyInjection
open System
open System.Runtime.CompilerServices
open System.Threading.Tasks

[<Extension>]
type ServiceCollectionExtensions = 
    [<Extension>]
    static member AddOrleansAuthorization(services: IServiceCollection, 
        azureActiveDirectoryApp: MicrosoftEntraApp,
        configure: Action<Configuration>, authorizationConfiguration: AuthorizationConfiguration) =
        
        // Check parameters that might come from C#
        if isNull (box services) then nullArg(nameof services)
        if isNull (box azureActiveDirectoryApp) then nullArg(nameof azureActiveDirectoryApp)
        if isNull (box configure) then nullArg(nameof configure)
        if isNull (box configure) then nullArg(nameof authorizationConfiguration)

        if not authorizationConfiguration.IsCoHostingEnabled then
            let accessTokenProvider=
              { new IAccessTokenProvider with member this.RetrieveTokenAsync() = Task.FromResult(String.Empty) }
            services.AddSingleton<IAccessTokenProvider>(accessTokenProvider) |> ignore

        services.AddAuthorization(configure, authorizationConfiguration);
        services.AddAzureActiveDirectoryAuthorization(azureActiveDirectoryApp) |> ignore

    // For the production usage.
    [<Extension>]
    static member AddOrleansClientAuthorization(services: IServiceCollection,
        azureActiveDirectoryApp: MicrosoftEntraApp,
        configure: Action<Configuration>) =

        // Check parameters that might come from C#
        if isNull (box services) then nullArg(nameof services)
        if isNull (box azureActiveDirectoryApp) then nullArg(nameof azureActiveDirectoryApp)
        if isNull (box configure) then nullArg(nameof configure)

        services.AddClientAuthorization(configure)
        services.AddAzureActiveDirectoryAuthorization(azureActiveDirectoryApp) |> ignore