namespace Authzi.MicrosoftEntra

open Authzi.Security

type ClaimTypeResolverDefault() =
    interface IClaimTypeResolver with
        member _.Resolve(claimType: ClaimType) =
            match claimType with
            | Name -> "name"
            | Role -> "roles"
            | Subject -> "sub"
