namespace Authzi.Security.Authorization

open System.Collections.Generic
open System.Linq
open System.Security.Claims

type internal AuthorizationExecutor(policyProvider : IAuthorizationPolicyProvider, 
        authorizationService : IAuthorizationService) =
        interface IAuthorizationExecutor with
            member _.AuthorizeAsync(claims : IEnumerable<Claim>, 
                grainInterfaceAuthorizeData : IEnumerable<IAuthorizeData>,
                grainMethodAuthorizeData : IEnumerable<IAuthorizeData>) = 
                async {
                    let claimsIdentity = ClaimsIdentity(claims, "AccessToken",
                                            ClaimTypes.Sid, ClaimTypes.Role)

                    let claimsPrincipal = new ClaimsPrincipal(claimsIdentity)
                    let mutable authorizationSucceeded = true

                    if not (isNull grainMethodAuthorizeData) && 
                        grainMethodAuthorizeData.Any() then
                        let! policy = AuthorizationPolicy.CombineAsync(policyProvider, grainMethodAuthorizeData) |> Async.AwaitTask
                        let! authorizationResult = authorizationService
                                                    .AuthorizeAsync(claimsPrincipal, policy) |> Async.AwaitTask
                        
                        authorizationSucceeded <- authorizationResult.Succeeded

                    if not (isNull grainInterfaceAuthorizeData) && 
                        grainInterfaceAuthorizeData.Any() && authorizationSucceeded then
                        let! policy = AuthorizationPolicy.CombineAsync(policyProvider, grainInterfaceAuthorizeData) |> Async.AwaitTask
                        let! authorizationResult = authorizationService
                                                    .AuthorizeAsync(claimsPrincipal, policy) |> Async.AwaitTask
                    
                        authorizationSucceeded <- authorizationResult.Succeeded

                    return authorizationSucceeded

                } |> Async.StartAsTask