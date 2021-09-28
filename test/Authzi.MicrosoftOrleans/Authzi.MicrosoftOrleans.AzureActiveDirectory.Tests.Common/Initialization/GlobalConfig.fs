module GlobalConfig

open Authzi.AzureActiveDirectory
open Credentials.AzureActiveDirectoryB2B1

let azureActiveDirectoryAppB2B1 =
    AzureActiveDirectoryApp(DirectoryId, WebClient1.Id, WebClient1.Secret, WebClient1.AllowedScopes)
