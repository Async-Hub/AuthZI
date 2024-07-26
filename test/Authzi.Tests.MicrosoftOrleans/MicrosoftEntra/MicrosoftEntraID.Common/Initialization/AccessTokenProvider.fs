module AccessTokenProvider

open Authzi.AzureActiveDirectory
open Authzi.Tests.MicrosoftOrleans.MicrosoftEntra.MicrosoftEntraID.Common
open System.Threading.Tasks

let getAccessTokenForUserAsync directoryId clientId scopes userName password =
    let accessToken = AccessTokenRetriever.getTokenByUserNameAndPassword directoryId 
                        clientId scopes userName password
    
    Task.FromResult(accessToken)

let getAccessTokenForUserOnWebClient1Async : string -> string-> Task<string> = 
        getAccessTokenForUserAsync TestData.AzureActiveDirectoryApp.DirectoryId 
            TestData.Web1Client.Id TestData.Web1Client.AllowedScopes
