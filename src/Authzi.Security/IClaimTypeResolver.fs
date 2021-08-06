namespace Authzi.Security

type ClaimType =
    | Role
    | Subject

type public IClaimTypeResolver =
    abstract member Resolve : ClaimType -> string

