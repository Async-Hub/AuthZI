namespace AuthZI.Security.Authorization

open AuthZI.Security
open Microsoft.FSharp.Core
open System.Collections.Generic
open System.Linq
open System.Security.Claims

type CSharpResult = CSharpFunctionalExtensions.Result

type AuthorizationExecutor
  (
    policyProvider: IAuthorizationPolicyProvider,
    authorizationService: IAuthorizationService,
    claimTypeResolver: IClaimTypeResolver
  ) =
  interface IAuthorizationExecutor with
    member _.AuthorizeAsync
      (
        claims: IEnumerable<Claim>,
        typeAuthorizeData: IEnumerable<IAuthorizeData>,
        methodAuthorizeData: IEnumerable<IAuthorizeData>
      ) =
      task {
        let subject = claimTypeResolver.Resolve ClaimType.Subject
        let role = claimTypeResolver.Resolve ClaimType.Role
        let claimsIdentity = ClaimsIdentity(claims, "AccessToken", subject, role)

        let claimsPrincipal = new ClaimsPrincipal(claimsIdentity)
        let mutable authorizationSucceeded = true

        if not (isNull methodAuthorizeData) && methodAuthorizeData.Any() then
          let! policy = AuthorizationPolicy.CombineAsync(policyProvider, methodAuthorizeData)
          let! authorizationResult = authorizationService.AuthorizeAsync(claimsPrincipal, policy)

          authorizationSucceeded <- authorizationResult.Succeeded

        if
          not (isNull typeAuthorizeData)
          && typeAuthorizeData.Any()
          && authorizationSucceeded
        then
          let! policy = AuthorizationPolicy.CombineAsync(policyProvider, typeAuthorizeData)
          let! authorizationResult = authorizationService.AuthorizeAsync(claimsPrincipal, policy)

          authorizationSucceeded <- authorizationResult.Succeeded

        if authorizationSucceeded then
          return CSharpResult.Success<ClaimsPrincipal, string>(claimsPrincipal)
        else
          return CSharpResult.Failure<ClaimsPrincipal, string>("Authorization failed.")
      }