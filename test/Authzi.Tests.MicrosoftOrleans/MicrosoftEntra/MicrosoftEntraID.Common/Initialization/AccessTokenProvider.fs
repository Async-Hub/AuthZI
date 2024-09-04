module AccessTokenProvider

open Authzi.MicrosoftEntra
open Authzi.Tests.MicrosoftOrleans.MicrosoftEntra.MicrosoftEntraID.Common
open System.Threading.Tasks

let getAccessTokenForUserAsync entraIDApp userName password =
    let accessToken = TestData.GetAccessTokenForUserOnWebClient1Async entraIDApp
    
    Task.FromResult(accessToken userName password)

let getAccessTokenForUserOnWebClient1Async : string -> string-> Task<string> = 
        getAccessTokenForUserAsync TestData.AzureActiveDirectoryApp
