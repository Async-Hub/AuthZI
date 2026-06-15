namespace AuthZI.Tests.MicrosoftOrleans.Duende.IdentityServer

open AuthZI.Tests.MicrosoftOrleans.Grains.ResourceBasedAuthorization
open Duende.IdentityServer.Models
open Duende.IdentityServer.Services
open Duende.IdentityModel
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
                    
                context.IssuedClaims.AddRange roleClaims
                
                // Include Country claims
                let countryClaims =
                    context.Subject.Claims.Where(fun c -> c.Type = "Country")
                    
                context.IssuedClaims.AddRange countryClaims
            }
            |> Async.StartAsTask :> Task

        member this.IsActiveAsync(context: IsActiveContext) = Task.CompletedTask