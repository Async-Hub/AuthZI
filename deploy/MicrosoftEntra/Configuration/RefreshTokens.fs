namespace AuthZI.Deploy.MicrosoftEntra.Configuration

open System

[<CLIMutable>]
type MicrosoftEntraRefreshToken =
  { DirectoryId: string
    ClientId: string
    UserName: string
    RefreshToken: string
    Scopes: string array
    CreatedAtUtc: DateTimeOffset }

[<CLIMutable>]
type MicrosoftEntraRefreshTokenStore = { Tokens: MicrosoftEntraRefreshToken array }
