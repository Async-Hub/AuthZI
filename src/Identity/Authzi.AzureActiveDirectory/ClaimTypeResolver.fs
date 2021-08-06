namespace Authzi.AzureActiveDirectory

open Authzi.Security

type ClaimTypeResolver() =
    interface IClaimTypeResolver with
        member _.Resolve(claimType: ClaimType) =
            match claimType with
            | Role -> "role"
            | Subject -> "sub"
