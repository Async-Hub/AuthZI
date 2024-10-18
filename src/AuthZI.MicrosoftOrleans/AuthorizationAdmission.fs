namespace AuthZI.MicrosoftOrleans

open AuthZI.Security.Authorization
open Orleans
open System.Linq
open System.Reflection

type internal AuthorizationAdmission() =
    static member IsRequired(grainCallContext: IGrainCallContext) =
        let allowAnonymousAttribute = grainCallContext.InterfaceMethod.GetCustomAttribute<AllowAnonymousAttribute>()

        if not (isNull allowAnonymousAttribute) then
            false
        else
            let grainAuthorizeData = 
                match grainCallContext.InterfaceMethod.ReflectedType with
                | null -> null
                | _ -> grainCallContext.InterfaceMethod.ReflectedType.GetCustomAttributes<AuthorizeAttribute>()

            let grainMethodAuthorizeData =
                match grainCallContext.InterfaceMethod with
                | null -> null
                | _ -> grainCallContext.InterfaceMethod.GetCustomAttributes<AuthorizeAttribute>()
            
            if grainAuthorizeData <> null && not (grainAuthorizeData.Any()) && not (grainMethodAuthorizeData.Any()) then
                false
            else true