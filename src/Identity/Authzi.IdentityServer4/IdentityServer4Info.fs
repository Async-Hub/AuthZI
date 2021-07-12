namespace Authzi.IdentityServer4

open System.Runtime.InteropServices

module Path=
    [<Literal>]
    let Url = ".well-known/openid-configuration"

type IdentityServer4Info(url: string, clientId: string, clientSecret: string, allowedScope: string,
                         [<Optional; DefaultParameterValue(Path.Url)>]discoveryEndpointEndpointPath: string)=
    member this.Url = url
    member this.ClientId = clientId
    member this.ClientSecret = clientSecret
    member this.AllowedScope = allowedScope
    member this.DiscoveryEndpointUrl = url + "/" + discoveryEndpointEndpointPath