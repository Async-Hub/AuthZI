namespace Authzi.AzureActiveDirectory

open System.Collections.Generic

module Configuration =
    [<Literal>]
    let DiscoveryEndpointPath = ".well-known/openid-configuration"
    [<Literal>]
    let Url = "https://login.microsoftonline.com/{DirectoryId}/v2.0"
    [<Literal>]
    let IssuerUrl = "https://sts.windows.net/{DirectoryId}/"
    [<Literal>]
    let JwksUri = "https://login.microsoftonline.com/common/discovery/v2.0/keys"

    let issuerUrl directoryId = IssuerUrl.Replace("{DirectoryId}", directoryId)

type AzureActiveDirectoryApp(directoryId: string, clientId: string, 
    clientSecret: string, allowedScopes: IEnumerable<string>) =
    member _.DirectoryId = directoryId
    member _.Url = Configuration.Url.Replace("{DirectoryId}", directoryId)
    member _.ClientId = clientId
    member _.ClientSecret = clientSecret
    member _.AllowedScopes = allowedScopes
    member this.DiscoveryEndpointUrl = this.Url + "/" + Configuration.DiscoveryEndpointPath
