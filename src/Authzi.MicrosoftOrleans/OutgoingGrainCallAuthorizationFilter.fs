namespace Authzi.MicrosoftOrleans.Authorization

open Authzi.MicrosoftOrleans
open Authzi.Security.AccessToken
open Authzi.Security.Authorization
open Authzi.Security;
open FSharp.Control.Tasks.V2
open Microsoft.Extensions.Logging
open Orleans
open System.Threading.Tasks

type OutgoingGrainCallAuthorizationFilter(accessTokenVerifier: IAccessTokenVerifier,
    authorizeHandler: IAuthorizationExecutor, logger: ILogger<OutgoingGrainCallAuthorizationFilter>) as this =
    inherit GrainAuthorizationFilterBase(accessTokenVerifier, authorizeHandler, logger)

    member _.AuthorizeAsync(context) = base.AuthorizeAsync(context)
    member _.Log(eventId, grainTypeName, interfaceMethodName) = base.Log(eventId, grainTypeName, interfaceMethodName)
    interface IOutgoingGrainCallFilter with
        member _.Invoke(context: IOutgoingGrainCallContext) =
            task {
                if AuthorizationAdmission.IsRequired context then
                    let! claims = this.AuthorizeAsync(context)
                    let grainType = context.Grain.GetType()

                    this.Log(LoggingEvents.OutgoingGrainCallAuthorizationPassed,
                        grainType.Name, context.InterfaceMethod.Name)

                do! context.Invoke()
            } :> Task