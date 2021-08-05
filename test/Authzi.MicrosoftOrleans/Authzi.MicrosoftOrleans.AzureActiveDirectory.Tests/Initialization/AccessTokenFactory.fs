module AccessTokenFactory

open Authzi.AzureActiveDirectory
open Credentials.AzureActiveDirectoryB2B1
open System.Threading.Tasks

let getAccessTokenForUserAsync clientId userName password =
    let accessToken = AccessTokenProvider.getTokenByUsernamePassword clientId userName password
    Task.FromResult(accessToken)

let getAccessTokenForUserOnWebClient1Async : string -> string-> Task<string> = 
        getAccessTokenForUserAsync WebClient1
