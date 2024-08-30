namespace Authzi.Tests.MicrosoftOrleans.MicrosoftEntra.MicrosoftEntraID.Common

open Authzi.MicrosoftEntra
open Authzi.Deploy.MicrosoftEntra.Configuration
open Authzi.Deploy.MicrosoftEntra.Configuration.Credentials.MicrosoftEntraID1

type public TestData() =
    static member val public UserWithScopeAdeleV : obj[] list = [] with get, set

    static member val public UserWithScopeAlexW : obj[] list = [] with get, set

    static member val public Users : obj[] list = [] with get, set

    static member val public AzureActiveDirectoryApp : AzureActiveDirectoryApp = 
        AzureActiveDirectoryApp("", "", "", false, [""]) with get, set
    
    static member val public Web1Client : Client = 
        Credentials.MicrosoftEntraID1.WebClient1 with get, set