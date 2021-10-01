namespace Authzi.MicrosoftOrleans.AzureActiveDirectory.Tests

open Authzi.AzureActiveDirectory
open Credentials
open Credentials.AzureActiveDirectoryB2B1

type public TestData() =
    static member val public UserWithScopeAdeleV : obj[] list = 
        [[|AdeleV.Name; AdeleV.Password; ["Api1"; "Orleans"]|]] with get, set

    static member val public UserWithScopeAlexW : obj[] list = 
        [[|AlexW.Name; AlexW.Password; ["Api1"; "Orleans"]|]] with get, set

    static member val public Users : obj[] list = 
        [[|AdeleV.Name; AdeleV.Password|]] with get, set

    static member val public AzureActiveDirectoryApp : AzureActiveDirectoryApp = 
        Credentials.GlobalConfig.azureActiveDirectoryAppB2B1 with get, set
    
    static member val public Web1Client : Client = 
        Credentials.AzureActiveDirectoryB2B1.WebClient1 with get, set