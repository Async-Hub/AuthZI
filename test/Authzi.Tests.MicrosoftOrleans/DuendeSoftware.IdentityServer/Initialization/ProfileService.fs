namespace Authzi.MicrosoftOrleans.IdentityServer4.Tests

open IdentityModel
open Duende.IdentityServer.Models
open Duende.IdentityServer.Services
open Authzi.Tests.MicrosoftOrleans.Grains.ClaimsBasedAuthorization
open Authzi.Tests.MicrosoftOrleans.Grains.ResourceBasedAuthorization
open System.Threading.Tasks
open System.Linq

type ProfileService() =
    interface IProfileService with
        member this.GetProfileDataAsync(context: ProfileDataRequestContext) =
            async {
                // Include DocRegistryAccessClaim
                let docRegistryAccessClaim =
                    context.Subject.Claims.SingleOrDefault(fun c -> c.Type = DocRegistryAccessClaim.Name)

                match docRegistryAccessClaim with
                | null -> ()
                | value -> context.IssuedClaims.Add value
                
                // Include Role claims
                let roleClaims =
                    context.Subject.Claims.Where(fun c -> c.Type = JwtClaimTypes.Role)
                    
                match roleClaims with
                | null -> ()
                | value -> context.IssuedClaims.AddRange value
                
                // Include Country claims
                let countryClaims =
                    context.Subject.Claims.Where(fun c -> c.Type = "Country")
                    
                match countryClaims with
                | null -> ()
                | value -> context.IssuedClaims.AddRange value
            }
            |> Async.StartAsTask :> Task

        member this.IsActiveAsync(context: IsActiveContext) = Task.CompletedTask