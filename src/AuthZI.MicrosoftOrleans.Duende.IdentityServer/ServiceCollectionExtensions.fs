namespace AuthZI.MicrosoftOrleans.Duende.IdentityServer

open AuthZI.Identity.Duende.IdentityServer
open AuthZI.MicrosoftOrleans
open AuthZI.MicrosoftOrleans.Authorization
open AuthZI.Security
open Microsoft.Extensions.DependencyInjection
open System
open System.Runtime.CompilerServices
open System.Threading.Tasks

type ServiceCollectionExtensions =
  [<Extension>]
  static member AddOrleansClientAuthorization
    (services: IServiceCollection, identityServerConfig: IdentityServerConfig, configure: Action<Configuration>) =

    // Check parameters that might come from C#
    if isNull (box services) then
      nullArg (nameof services)

    if isNull (box identityServerConfig) then
      nullArg (nameof identityServerConfig)

    if isNull (box configure) then
      nullArg (nameof configure)

    services.AddClientAuthorization(configure)
    services.AddIdentityServerAuthorization(identityServerConfig)

  [<Extension>]
  static member AddOrleansAuthorization
    (
      services: IServiceCollection,
      identityServerConfig: IdentityServerConfig,
      configure: Action<Configuration>,
      authorizationConfiguration: AuthorizationConfiguration
    ) =

    // Check parameters that might come from C#
    if isNull (box services) then
      nullArg (nameof services)

    if isNull (box identityServerConfig) then
      nullArg (nameof identityServerConfig)

    if isNull (box configure) then
      nullArg (nameof configure)

    if not authorizationConfiguration.IsCoHostingEnabled then
      let accessTokenProvider =
        { new IAccessTokenProvider with
            member this.RetrieveTokenAsync() = Task.FromResult(String.Empty) }

      services.AddSingleton<IAccessTokenProvider>(accessTokenProvider) |> ignore

    services.AddAuthorization(configure, authorizationConfiguration)
    services.AddIdentityServerAuthorization(identityServerConfig)
