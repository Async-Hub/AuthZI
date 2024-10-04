namespace Authzi.Identity.MicrosoftEntra

module Configuration =
    [<Literal>]
    let DiscoveryEndpointPath = ".well-known/openid-configuration"
    [<Literal>]
    let Url = "https://login.microsoftonline.com/{DirectoryId}/v2.0"

open Microsoft.Identity.Client
open System.Collections.Generic

[<AbstractClass>]
type MicrosoftEntraApp(directoryId: string, clientId: string, 
    clientSecret: string, allowedScopes: IEnumerable<string>, aadAuthorityAudience: AadAuthorityAudience) =
    member _.DirectoryId = directoryId
    member _.ClientId = clientId
    member _.ClientSecret = clientSecret
    member _.AllowedScopes = allowedScopes
    member _.AadAuthorityAudience = aadAuthorityAudience
    abstract member IssuerUrl : string with get
    abstract member DiscoveryEndpointUrl : string with get

type MicrosoftEntraIDApp(directoryId, clientId, clientSecret, allowedScopes, aadAuthorityAudience) =
    inherit MicrosoftEntraApp(directoryId, clientId, clientSecret, allowedScopes, aadAuthorityAudience)
    override _.IssuerUrl = "https://sts.windows.net/{DirectoryId}/".Replace("{DirectoryId}", directoryId)
    override _.DiscoveryEndpointUrl = 
        Configuration.Url.Replace("{DirectoryId}", directoryId) + "/" + Configuration.DiscoveryEndpointPath
    static member EmptyApp = MicrosoftEntraIDApp("", "", "", [], AadAuthorityAudience.AzureAdMyOrg)

type MicrosoftEntraExternalIDApp(directoryId, clientId, clientSecret, allowedScopes, aadAuthorityAudience) =
    inherit MicrosoftEntraApp(directoryId, clientId, clientSecret, allowedScopes, aadAuthorityAudience)
    override _.IssuerUrl = "https://{DirectoryId}.ciamlogin.com/{DirectoryId}/v2.0".Replace("{DirectoryId}", directoryId)
    override _.DiscoveryEndpointUrl = 
        "https://{DirectoryId}.ciamlogin.com/{DirectoryId}".Replace("{DirectoryId}", directoryId) 
        + "/v2.0/.well-known/openid-configuration"