module AccessTokenProvider

open Authzi.AzureActiveDirectory
open System.Threading.Tasks

let getAccessTokenForUserAsync directoryId clientId scopes userName password =
    let accessToken = AccessTokenRetriever.getTokenByUserNameAndPassword directoryId clientId scopes userName password
    Task.FromResult(accessToken)

open Credentials.AzureActiveDirectoryB2B1
let getAccessTokenForUserOnB2BWebClient1Async : string -> string-> Task<string> = 
        getAccessTokenForUserAsync DirectoryId WebClient1.Id WebClient1.AllowedScopes

open Credentials.AzureActiveDirectoryB2C1
let getAccessTokenForUserOnB2CWebClient1Async : string -> string-> Task<string> = 
    getAccessTokenForUserAsync DirectoryId WebClient1.Id WebClient1.AllowedScopes
