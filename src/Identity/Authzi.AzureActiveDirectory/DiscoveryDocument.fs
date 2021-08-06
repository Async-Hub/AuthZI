namespace Authzi.AzureActiveDirectory

open System.Collections.Generic

[<AllowNullLiteral>]
type DiscoveryDocument() =
    member val IntrospectionEndpoint : string = null with get, set
    
    member val Issuer : string = null with get, set

    member val TokenEndpoint : string = null with get,set
