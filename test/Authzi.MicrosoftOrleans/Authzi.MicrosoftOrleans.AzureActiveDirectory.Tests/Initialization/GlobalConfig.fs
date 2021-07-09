module GlobalConfig

open Authzi.AzureActiveDirectory

let identityServer4Url = "http://localhost:5001"
let identityServer4Info = AzureActiveDirectoryApp(identityServer4Url,
                                "Orleans", "@3x3g*RLez$TNU!_7!QW", "Orleans")
[<Literal>]
let WebClient1 = "WebClient1"
[<Literal>]
let WebClient2 = "WebClient2"
