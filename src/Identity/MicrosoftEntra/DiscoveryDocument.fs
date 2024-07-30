namespace Authzi.MicrosoftEntra

open Microsoft.IdentityModel.Tokens
open System.Collections.Generic

type DiscoveryDocument(discoveryEndpoint: string, signingKeys: ICollection<SecurityKey>) =
    member val DiscoveryEndpoint : string = discoveryEndpoint with get

    member val SigningKeys : ICollection<SecurityKey> = signingKeys with get
