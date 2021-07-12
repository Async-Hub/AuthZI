namespace Authzi.Security.Authorization

open System.Collections.Generic
open System.Linq
open System.Security.Claims
open IdentityModel

type internal AuthorizationExecutor(policyProvider : IAuthorizationPolicyProvider, 
        authorizationService : IAuthorizationService) =
        interface IAuthorizationExecutor with
            member _.AuthorizeAsync(claims : IEnumerable<Claim>, 
                typeAuthorizeData : IEnumerable<IAuthorizeData>,
                methodAuthorizeData : IEnumerable<IAuthorizeData>) = 
                async {
                    let claimsIdentity = ClaimsIdentity(claims, "AccessToken",
                                            JwtClaimTypes.Subject, JwtClaimTypes.Role)

                    let claimsPrincipal = new ClaimsPrincipal(claimsIdentity)
                    let mutable authorizationSucceeded = true

                    if not (isNull methodAuthorizeData) && 
                        methodAuthorizeData.Any() then
                        let! policy = AuthorizationPolicy.CombineAsync(policyProvider, methodAuthorizeData) |> Async.AwaitTask
                        let! authorizationResult = authorizationService
                                                    .AuthorizeAsync(claimsPrincipal, policy) |> Async.AwaitTask
                        
                        authorizationSucceeded <- authorizationResult.Succeeded

                    if not (isNull typeAuthorizeData) && 
                        typeAuthorizeData.Any() && authorizationSucceeded then
                        let! policy = AuthorizationPolicy.CombineAsync(policyProvider, typeAuthorizeData) |> Async.AwaitTask
                        let! authorizationResult = authorizationService
                                                    .AuthorizeAsync(claimsPrincipal, policy) |> Async.AwaitTask
                    
                        authorizationSucceeded <- authorizationResult.Succeeded

                    return authorizationSucceeded

                } |> Async.StartAsTask