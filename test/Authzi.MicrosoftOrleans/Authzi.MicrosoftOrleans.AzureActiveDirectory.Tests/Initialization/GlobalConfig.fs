module GlobalConfig

open Authzi.AzureActiveDirectory

let azureActiveDirectoryUrl = "https://login.microsoftonline.com/common/v2.0/"
let azureActiveDirectoryApp = AzureActiveDirectoryApp(azureActiveDirectoryUrl,
                                "Orleans", "@3x3g*RLez$TNU!_7!QW", "Orleans")
[<Literal>]
let WebClient1 = "WebClient1"
[<Literal>]
let WebClient2 = "WebClient2"
