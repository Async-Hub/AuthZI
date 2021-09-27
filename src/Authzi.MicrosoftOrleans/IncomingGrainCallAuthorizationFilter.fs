namespace Authzi.MicrosoftOrleans.Authorization

open Authzi.MicrosoftOrleans
open Authzi.Security.AccessToken
open Authzi.Security.Authorization
open Authzi.Security;
open Microsoft.Extensions.Logging
open Orleans
open Orleans.Runtime
open System.Security.Claims
open System.Threading.Tasks

type IncomingGrainCallAuthorizationFilter(accessTokenVerifier: IAccessTokenVerifier,
    authorizeHandler: IAuthorizationExecutor, claimTypeResolver: IClaimTypeResolver,
    logger: ILogger<IncomingGrainCallAuthorizationFilter>) as this =
    inherit GrainAuthorizationFilterBase(accessTokenVerifier, authorizeHandler, logger)

    member _.AuthorizeAsync(context) = base.AuthorizeAsync(context)
    member _.Log(eventId, grainTypeName, interfaceMethodName) = base.Log(eventId, grainTypeName, interfaceMethodName)
    interface IIncomingGrainCallFilter with
        member _.Invoke(context: IIncomingGrainCallContext) =
            task {
                if AuthorizationAdmission.IsRequired context then
                    let! claims = this.AuthorizeAsync(context)
                    let grainType = context.Grain.GetType()
                    if grainType.BaseType = typeof<GrainWithClaimsPrincipal> then
                        let claimsIdentity = ClaimsIdentity(claims, System.String.Empty, 
                                                claimTypeResolver.Resolve ClaimType.Subject, 
                                                claimTypeResolver.Resolve ClaimType.Role)
                        let claimsPrincipal = ClaimsPrincipal(claimsIdentity)
                        RequestContext.Set(ConfigurationKeys.ClaimsPrincipalKey, claimsPrincipal)
                    
                    this.Log(LoggingEvents.IncomingGrainCallAuthorizationPassed,
                        grainType.Name, context.InterfaceMethod.Name)

                do! context.Invoke()
            } :> Task