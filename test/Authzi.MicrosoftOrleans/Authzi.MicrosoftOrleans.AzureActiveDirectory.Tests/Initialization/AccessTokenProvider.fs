module AccessTokenProvider

open Authzi.AzureActiveDirectory
open System.Threading.Tasks

let getAccessTokenForUserAsync clientId userName password =
    let accessToken = AccessTokenRetriever.getTokenByUserNameAndPassword clientId userName password
    Task.FromResult(accessToken)

open Credentials.AzureActiveDirectoryB2B1
let getAccessTokenForUserOnB2BWebClient1Async : string -> string-> Task<string> = 
        getAccessTokenForUserAsync WebClient1

open Credentials.AzureActiveDirectoryB2C1
let getAccessTokenForUserOnB2CWebClient1Async : string -> string-> Task<string> = 
    getAccessTokenForUserAsync WebClient1
