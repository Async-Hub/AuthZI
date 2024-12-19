namespace AuthZI.MicrosoftOrleans.Authorization

open AuthZI.Security.AccessToken
open AuthZI.Security.Authorization
open System.Collections.Generic
open System.Reflection

type public AdmissionExecutor(accessTokenVerifier: IAccessTokenVerifier, authorizeHandler: IAuthorizationExecutor)=
  member public this.AdmitAsync(accessTokenResult: Result<string,string>, interfaceMethod: MethodInfo )=
      task {        
          match accessTokenResult with
          | Error error -> return Error error
          | Ok accessToken ->
          let! accessTokenVerificationResult = accessTokenVerifier.Verify(accessToken)

          match accessTokenVerificationResult with
          | Ok claims ->
              let grainMethodAuthorizeData: IEnumerable<IAuthorizeData> = 
                  interfaceMethod.GetCustomAttributes<AuthorizeAttribute>()
                  |> Seq.cast<IAuthorizeData>

              let mutable grainAuthorizeData: IEnumerable<IAuthorizeData> = null

              if interfaceMethod.ReflectedType <> null then
                  grainAuthorizeData <- interfaceMethod.ReflectedType.GetCustomAttributes<AuthorizeAttribute>()
                                        |> Seq.cast<IAuthorizeData>

              let! authorizationResult = 
                authorizeHandler.AuthorizeAsync(claims, grainAuthorizeData, grainMethodAuthorizeData)
              if authorizationResult.IsSuccess then
                  return Ok authorizationResult.Value
              else
                  return Error("Authorization failed.")
          | Error error -> return Error error
      }

