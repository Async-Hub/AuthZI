module AccessTokenProvider

open AuthZI.Tests.MicrosoftOrleans.MicrosoftEntra.MicrosoftEntraID.Common
open System.Threading.Tasks

let getAccessTokenForUserAsync entraIDApp userName password =
    let accessToken = TestData.GetAccessTokenForUserOnMicrosoftEntraAppAsync entraIDApp
    
    Task.FromResult(accessToken userName password)

let getAccessTokenForUserOnWebClient1Async : string -> string-> Task<string> = 
        getAccessTokenForUserAsync TestData.Web1ClientApp

let getAccessTokenForUserOnWebClient2Async : string -> string-> Task<string> = 
        getAccessTokenForUserAsync TestData.Web2ClientApp
