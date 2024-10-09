namespace Authzi.MicrosoftOrleans.Authorization

open Authzi.MicrosoftOrleans
open Authzi.Security.AccessToken
open Authzi.Security.Authorization
open Authzi.Security
open Microsoft.Extensions.Logging
open Newtonsoft.Json
open Orleans
open Orleans.Runtime
open System.Threading.Tasks

type IncomingGrainCallAuthorizationFilter(accessTokenVerifier: IAccessTokenVerifier,
    authorizeHandler: IAuthorizationExecutor, logger: ILogger<IncomingGrainCallAuthorizationFilter>) as this =
    inherit GrainAuthorizationFilterBase(accessTokenVerifier, authorizeHandler, logger)
    member _.AuthorizeAsync(context) = base.AuthorizeAsync(context)
    member _.Log(eventId, grainTypeName, interfaceMethodName) = base.Log(eventId, grainTypeName, interfaceMethodName)
    interface IIncomingGrainCallFilter with
        member _.Invoke(context: IIncomingGrainCallContext) =
            task {
                if AuthorizationAdmission.IsRequired context then
                    let grainType = context.Grain.GetType()
                    if grainType.BaseType = typeof<GrainWithClaimsPrincipal> then
                        let! claims = this.AuthorizeAsync(context)
                        let serializedClaims = JsonConvert.SerializeObject(claims)
                        RequestContext.Set(ConfigurationKeys.ClaimsPrincipalKey, serializedClaims)
                    
                    this.Log(LoggingEvents.IncomingGrainCallAuthorizationPassed,
                        grainType.Name, context.InterfaceMethod.Name)

                do! context.Invoke()
            } :> Task