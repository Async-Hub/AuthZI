namespace Authzi.MicrosoftOrleans.Authorization

open System.Threading.Tasks
open System.Security.Claims
open Orleans.Runtime
open Orleans
open Microsoft.Extensions.Logging
open IdentityModel
open Authzi.Extensions.TaskExtensions
open Authzi.Security;
open Authzi.Security.Authorization
open Authzi.Security.AccessToken
open Authzi.MicrosoftOrleans

type IncomingGrainCallAuthorizationFilter(accessTokenVerifier: IAccessTokenVerifier,
    authorizeHandler: IAuthorizationExecutor, logger: ILogger<IncomingGrainCallAuthorizationFilter>) as this =
    inherit GrainAuthorizationFilterBase(accessTokenVerifier, authorizeHandler)
    do this.Logger <- logger
    member _.AuthorizeAsync(context) = base.AuthorizeAsync(context)
    member _.Log(eventId, grainTypeName, interfaceMethodName) = base.Log(eventId, grainTypeName, interfaceMethodName)
    interface IIncomingGrainCallFilter with
        member _.Invoke(context: IIncomingGrainCallContext) =
            async {
                if AuthorizationAdmission.IsRequired context then
                    let! claims = this.AuthorizeAsync(context) |> Async.AwaitTask
                    let grainType = context.Grain.GetType()
                    if grainType.BaseType = typeof<GrainWithClaimsPrincipal> then
                        let claimsIdentity = new ClaimsIdentity(claims, 
                                                "", JwtClaimTypes.Subject, JwtClaimTypes.Role)
                        let claimsPrincipal = new ClaimsPrincipal(claimsIdentity)
                        RequestContext.Set(ConfigurationKeys.ClaimsPrincipalKey, claimsPrincipal)
                    
                    this.Log(LoggingEvents.IncomingGrainCallAuthorizationPassed,
                        grainType.Name, context.InterfaceMethod.Name)

                do! context.Invoke() |> Async.AwaitTaskAndTryToUnwrapException
            } |> Async.StartAsTask :> Task