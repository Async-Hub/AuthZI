namespace Authzi.AzureActiveDirectory

open Authzi.Security

type ClaimTypeResolver() =
    interface IClaimTypeResolver with
        member _.Resolve(claimType: ClaimType) =
            match claimType with
            | Name -> "name"
            | Role -> "role"
            | Subject -> "sub"
