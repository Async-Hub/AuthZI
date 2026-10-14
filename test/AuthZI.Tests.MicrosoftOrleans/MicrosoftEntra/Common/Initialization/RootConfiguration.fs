module RootConfiguration

open AuthZI.Security
open AuthZI.Tests.MicrosoftOrleans.MicrosoftEntra.MicrosoftEntraID.Common
open System
open System.Threading.Tasks

type AccessTokenProvider() =
    let mutable accessToken = String.Empty
    member _.AccessToken with set (value) = accessToken <- value
    
    interface IAccessTokenProvider with
        member _.RetrieveTokenAsync() = Task.FromResult(accessToken);

let private globalAccessToken = AccessTokenProvider()
let accessTokenProvider = globalAccessToken :> IAccessTokenProvider

let getClusterClient (accessToken: string) =
    globalAccessToken.AccessToken <- accessToken
    TestData.IClusterClient