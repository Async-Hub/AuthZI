module RootConfiguration

open Authzi.Security
open Orleans
open System
open System.Threading.Tasks

type AccessTokenProvider() =
    let mutable accessToken = String.Empty
    member _.AccessToken with set (value) = accessToken <- value
    
    interface IAccessTokenProvider with
        member _.RetrieveTokenAsync() = Task.FromResult(accessToken);

type SiloClientProvider() =
    let mutable siloClient : IClusterClient = null
    member _.SiloClient with set (value) = siloClient <- value
    
    member _.Take() = Task.FromResult(siloClient)

let private globalAccessToken = AccessTokenProvider()
let accessTokenProvider = globalAccessToken :> IAccessTokenProvider

let siloClientProvider = SiloClientProvider()

let getClusterClient (accessToken: string) =
    globalAccessToken.AccessToken <- accessToken
    siloClientProvider.Take().Result