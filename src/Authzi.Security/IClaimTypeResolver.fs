namespace AuthZI.Security

type ClaimType =
  | Role
  | Subject
  | Name

type public IClaimTypeResolver =
  abstract member Resolve: ClaimType -> string