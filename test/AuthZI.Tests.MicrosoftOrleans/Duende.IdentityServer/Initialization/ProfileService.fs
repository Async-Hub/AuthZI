namespace AuthZI.Tests.MicrosoftOrleans.Duende.IdentityServer

open AuthZI.Tests.MicrosoftOrleans.Grains.ClaimsBasedAuthorization
open AuthZI.Tests.MicrosoftOrleans.Grains.ResourceBasedAuthorization
open Duende.IdentityServer.Models
open Duende.IdentityServer.Services
open IdentityModel
open System.Linq
open System.Threading.Tasks

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