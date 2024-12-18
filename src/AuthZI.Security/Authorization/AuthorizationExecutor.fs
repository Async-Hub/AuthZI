﻿namespace AuthZI.Security.Authorization

open AuthZI.Security
open Microsoft.FSharp.Core
open System.Collections.Generic
open System.Linq
open System.Security.Claims

type AuthorizationExecutor(policyProvider : IAuthorizationPolicyProvider, 
        authorizationService : IAuthorizationService, claimTypeResolver: IClaimTypeResolver) =
        interface IAuthorizationExecutor with
            member _.AuthorizeAsync(claims : IEnumerable<Claim>, 
                typeAuthorizeData : IEnumerable<IAuthorizeData>,
                methodAuthorizeData : IEnumerable<IAuthorizeData>) = 
                async {
                    let claimsIdentity = ClaimsIdentity(claims, "AccessToken",
                                            claimTypeResolver.Resolve ClaimType.Subject, 
                                            claimTypeResolver.Resolve ClaimType.Role)

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