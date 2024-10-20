namespace AuthZI.MicrosoftOrleans

open AuthZI.Extensions.TaskExtensions
open AuthZI.Security
open Orleans
open Orleans.Runtime
open System
open System.Threading.Tasks

type public AccessTokenSetterFilter(accessTokenProvider: IAccessTokenProvider) =
  let accessTokenProvider = accessTokenProvider

  interface IOutgoingGrainCallFilter with
    member _.Invoke(context: IOutgoingGrainCallContext) =
      task {
        if AuthorizationDeterminer.IsRequired(context.InterfaceMethod) then
          let mutable accessToken : String = null
          accessToken <- (RequestContext.Get(ConfigurationKeys.AccessTokenKey) |? (String.Empty :> Object)).ToString()

          if String.IsNullOrWhiteSpace(accessToken) then
            let! accessToken = accessTokenProvider.RetrieveTokenAsync()

            if String.IsNullOrWhiteSpace(accessToken) then
              raise (InvalidOperationException("AccessToken can not be null or empty."))

            RequestContext.Set(ConfigurationKeys.AccessTokenKey, accessToken)

        do! context.Invoke()
      }
      :> Task