module AccessTokenFactory

open System.Threading.Tasks
open Authzi.AzureActiveDirectory

let getAccessTokenForClientAsync (clientId: string) (clientSecret: string) (scope: string) =
    Task.FromResult(System.String.Empty)
    
let getAccessTokenForUserAsync (clientId: string) (clientSecret: string) (userName: string)
    (password: string) (scope: string) =
    let accessToken = AccessTokenProvider.getToken
    Task.FromResult(System.String.Empty)

let getAccessTokenForUserOnWebClient1Async = getAccessTokenForUserAsync GlobalConfig.WebClient1 "Secret1"
let getAccessTokenForUserOnWebClient2Async = getAccessTokenForUserAsync GlobalConfig.WebClient2 "Secret2"
