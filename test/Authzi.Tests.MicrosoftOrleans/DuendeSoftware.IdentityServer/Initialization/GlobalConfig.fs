namespace AuthZI.Tests.MicrosoftOrleans.DuendeSoftware.IdentityServer

open AuthZI.Security
open System
open System.Threading.Tasks

type AccessTokenProvider() =
    let mutable accessToken = String.Empty
    member _.AccessToken with set value = accessToken <- value
    
    interface IAccessTokenProvider with
        member _.RetrieveTokenAsync() = Task.FromResult(accessToken);

open AuthZI.Identity.DuendeSoftware.IdentityServer
open Orleans

module GlobalConfig =
    let public identityServerUrl = "http://localhost:5005"
    let public identityServerConfig =
        IdentityServerConfig(identityServerUrl, "Api1", "Secret", "Orleans")

    let identityServerConfigCluster =
        IdentityServerConfig(identityServerUrl, "Orleans", "@3x3g*RLez$TNU!_7!QW", "Orleans")

    [<Literal>]
    let WebClient1 = "WebClient1"
    [<Literal>]
    let WebClient2 = "WebClient2"

    let private globalAccessToken = AccessTokenProvider()
    let accessTokenProvider = globalAccessToken :> IAccessTokenProvider
    
    let mutable clusterClient : IClusterClient = null
    let mutable discoveryDocument : DiscoveryDocument = null
    
    let getClusterClient (accessToken: string) =
        globalAccessToken.AccessToken <- accessToken
        clusterClient