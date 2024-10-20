namespace AuthZI.Tests.MicrosoftOrleans.MicrosoftEntra.MicrosoftEntraID.Common

open AuthZI.Deploy.MicrosoftEntra.Configuration
open AuthZI.Deploy.MicrosoftEntra.Configuration.Credentials.MicrosoftEntraID1
open AuthZI.Identity.MicrosoftEntra
open AuthZI.MicrosoftEntra
open Orleans

type public TestData() =
    static member val public UserWithScopeAdeleV : obj[] list = [] with get, set

    static member val public UserWithScopeAlexW : obj[] list = [] with get, set

    static member val public Users : obj[] list = [] with get, set

    static member val public AzureActiveDirectoryApp : MicrosoftEntraApp = 
        MicrosoftEntraIDApp.EmptyApp with get, set
    
    static member val public Web1Client : Client = 
        Credentials.MicrosoftEntraID1.WebClient1 with get, set

    static member val IClusterClient : IClusterClient = null with get, set

    static member val GetAccessTokenForUserOnWebClient1Async : 
        MicrosoftEntraApp -> string -> string -> string = fun app user pass -> "" with get, set