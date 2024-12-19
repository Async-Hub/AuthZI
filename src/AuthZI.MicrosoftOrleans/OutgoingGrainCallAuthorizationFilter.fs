namespace AuthZI.MicrosoftOrleans.Authorization

open AuthZI.Security.AccessToken
open AuthZI.Security.Authorization
open AuthZI.Security;
open Microsoft.Extensions.Logging
open Orleans
open System.Threading.Tasks

//type OutgoingGrainCallAuthorizationFilter(accessTokenVerifier: IAccessTokenVerifier,
//    authorizeHandler: IAuthorizationExecutor, logger: ILogger<OutgoingGrainCallAuthorizationFilter>) as this =

//    //member _.AuthorizeAsync(context) = base.AuthorizeAsync(context)
//    //member _.Log(eventId, grainTypeName, interfaceMethodName) = base.Log(eventId, grainTypeName, interfaceMethodName)
//    interface IOutgoingGrainCallFilter with
//        member _.Invoke(context: IOutgoingGrainCallContext) =
//            task {
//                if AuthorizationDeterminer.IsRequired context.InterfaceMethod then
//                    //let! claims = this.AuthorizeAsync(context)
//                    //let grainType = context.Grain.GetType()

//                    //this.Log(LoggingEvents.OutgoingGrainCallAuthorizationPassed,
//                    //    grainType.Name, context.InterfaceMethod.Name)

//                do! context.Invoke()
//            } :> Task