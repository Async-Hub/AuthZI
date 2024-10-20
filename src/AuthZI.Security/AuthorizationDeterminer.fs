namespace AuthZI.Security

open AuthZI.Security.Authorization
open System.Linq
open System.Reflection

type public AuthorizationDeterminer() =
  static member IsRequired(methodInfo: MethodInfo) =
    let allowAnonymousAttribute = methodInfo.GetCustomAttribute<AllowAnonymousAttribute>()

    if not (isNull allowAnonymousAttribute) then
      false
    else
      let typeAuthorizeData =
        match methodInfo.ReflectedType with
        | null -> null
        | _ -> methodInfo.ReflectedType.GetCustomAttributes<AuthorizeAttribute>()

      let typeMethodAuthorizeData =
        match methodInfo with
        | null -> null
        | _ -> methodInfo.GetCustomAttributes<AuthorizeAttribute>()

      typeAuthorizeData = null || typeAuthorizeData.Any() || typeMethodAuthorizeData.Any()