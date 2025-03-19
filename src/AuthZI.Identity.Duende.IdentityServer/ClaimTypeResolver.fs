namespace AuthZI.Identity.Duende.IdentityServer

open AuthZI.Security
open Duende.IdentityModel

type ClaimTypeResolver() =
  interface IClaimTypeResolver with
    member _.Resolve(claimType: ClaimType) =
      match claimType with
      | Role -> JwtClaimTypes.Role
      | Subject -> JwtClaimTypes.Subject
      | Name -> JwtClaimTypes.Name