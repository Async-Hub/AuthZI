module AccessTokenProvider

open Authzi.AzureActiveDirectory
open System.Threading.Tasks

let getAccessTokenForUserAsync clientId scopes userName password =
    let accessToken = AccessTokenRetriever.getTokenByUserNameAndPassword clientId scopes userName password
    Task.FromResult(accessToken)

open Credentials.AzureActiveDirectoryB2B1
let getAccessTokenForUserOnB2BWebClient1Async : string -> string-> Task<string> = 
        getAccessTokenForUserAsync WebClient1.Id WebClient1.AllowedScopes

open Credentials.AzureActiveDirectoryB2C1
let getAccessTokenForUserOnB2CWebClient1Async : string -> string-> Task<string> = 
    getAccessTokenForUserAsync WebClient1.Id WebClient1.AllowedScopes
