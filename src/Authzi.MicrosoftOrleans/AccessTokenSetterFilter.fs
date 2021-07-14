namespace Authzi.MicrosoftOrleans

open Authzi.Extensions.TaskExtensions
open Authzi.Security
open Orleans
open Orleans.Runtime
open System
open System.Threading.Tasks

type public AccessTokenSetterFilter(accessTokenProvider: IAccessTokenProvider)=
    let accessTokenProvider = accessTokenProvider
    interface IOutgoingGrainCallFilter with
        member _.Invoke (context: IOutgoingGrainCallContext) =
                async {
                    if AuthorizationAdmission.IsRequired(context) then
                        let accessToken = (RequestContext.Get(ConfigurationKeys.AccessTokenKey) |?
                                                    (String.Empty :> Object)).ToString()
                    
                        if String.IsNullOrWhiteSpace(accessToken) then
                            let! newAccessToken = accessTokenProvider.RetrieveTokenAsync() |> Async.AwaitTask
                        
                            if String.IsNullOrWhiteSpace(newAccessToken) then
                                raise (InvalidOperationException("AccessToken can not be null or empty."))
                        
                            RequestContext.Set(ConfigurationKeys.AccessTokenKey, newAccessToken);
                    
                    do! context.Invoke() |> Async.AwaitTaskAndTryToUnwrapException
                } |> Async.StartAsTask :> Task