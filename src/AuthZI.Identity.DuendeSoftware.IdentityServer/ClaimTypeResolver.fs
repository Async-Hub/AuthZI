namespace AuthZI.Identity.DuendeSoftware.IdentityServer

open AuthZI.Security
open IdentityModel

type ClaimTypeResolver() =
  interface IClaimTypeResolver with
    member _.Resolve(claimType: ClaimType) =
      match claimType with
      | Role -> JwtClaimTypes.Role
      | Subject -> JwtClaimTypes.Subject
      | Name -> JwtClaimTypes.Name