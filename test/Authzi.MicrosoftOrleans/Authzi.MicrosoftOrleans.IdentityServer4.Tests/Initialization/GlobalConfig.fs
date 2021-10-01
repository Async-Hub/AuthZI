module GlobalConfig

open Authzi.IdentityServer4

let identityServer4Url = "http://localhost:5001"
let identityServer4Info = IdentityServer4Info(identityServer4Url,
                                "Api1", "Secret", "Orleans")

let identityServer4InfoCluster = IdentityServer4Info(identityServer4Url,
                                "Orleans", "@3x3g*RLez$TNU!_7!QW", "Orleans")

[<Literal>]
let WebClient1 = "WebClient1"
[<Literal>]
let WebClient2 = "WebClient2"
