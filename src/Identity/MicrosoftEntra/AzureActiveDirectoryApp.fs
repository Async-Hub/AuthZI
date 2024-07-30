namespace Authzi.MicrosoftEntra

open System.Collections.Generic

module Configuration =
    [<Literal>]
    let DiscoveryEndpointPath = ".well-known/openid-configuration"
    [<Literal>]
    let Url = "https://login.microsoftonline.com/{DirectoryId}/v2.0"
    [<Literal>]
    let IssuerUrl = "https://sts.windows.net/{DirectoryId}/"
    [<Literal>]
    let b2cIssuerUrl = "https://login.microsoftonline.com/{DirectoryId}/v2.0" 
    [<Literal>]
    let JwksUri = "https://login.microsoftonline.com/common/discovery/v2.0/keys"

    let issuerUrl directoryId isB2CTenant =
       if isB2CTenant then Url.Replace("{DirectoryId}", directoryId)
       else IssuerUrl.Replace("{DirectoryId}", directoryId)

type AzureActiveDirectoryApp(directoryId: string, clientId: string, 
    clientSecret: string, isB2CTenant: bool, allowedScopes: IEnumerable<string>) =
    member _.DirectoryId = directoryId
    member _.IsB2CTenant = isB2CTenant
    member _.Url = Configuration.Url.Replace("{DirectoryId}", directoryId)
    member _.ClientId = clientId
    member _.ClientSecret = clientSecret
    member _.AllowedScopes = allowedScopes
    member this.DiscoveryEndpointUrl = this.Url + "/" + Configuration.DiscoveryEndpointPath
