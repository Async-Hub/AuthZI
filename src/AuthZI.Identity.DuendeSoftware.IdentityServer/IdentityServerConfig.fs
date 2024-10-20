namespace AuthZI.Identity.DuendeSoftware.IdentityServer

open System.Runtime.InteropServices

module Path =
  [<Literal>]
  let Url = ".well-known/openid-configuration"

type IdentityServerConfig(url: string, clientId: string, clientSecret: string, audience: string, 
 [<Optional; DefaultParameterValue(Path.Url)>]discoveryEndpointPath: string) =
 member this.Url = url
 member this.ClientId = clientId
 member this.ClientSecret = clientSecret
 member this.Audience = audience
 member this.DiscoveryEndpointUrl = url + "/" + discoveryEndpointPath