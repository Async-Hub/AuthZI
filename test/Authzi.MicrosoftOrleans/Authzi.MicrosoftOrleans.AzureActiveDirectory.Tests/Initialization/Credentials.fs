module Credentials

type User = { Name: string; Password: string }
type Client = { Id: string; Secret: string; AllowedScopes: string list }

module Users =
    [<Literal>]
    let GeneralPassword = "Passw@rd+1"

    [<Literal>]
    let AdeleVB2B1 = "AdeleV@xxx.onmicrosoft.com"

    [<Literal>]
    let AdeleVB2C1 = "AdeleV@xxx.onmicrosoft.com"

open Users

module AzureActiveDirectoryB2B1 =
    let DirectoryId = "xxx"

    let Api1 = "xxx"
    let Api1Scope1 = $"api://{Api1}/Api1"

    let WebClient1 = 
        { Id = "xxx" 
          Secret = "xxx"
          AllowedScopes = [Api1Scope1]}

    let AdeleV =
        { Name = AdeleVB2B1
          Password = GeneralPassword }

module AzureActiveDirectoryB2C1 =
    let DirectoryId = "xxx"
    let Api1 = "xxx"
    let Api1Scope1 = $"api://{Api1}/Api1"

    let WebClient1 = 
        { Id = "xxx"
          Secret = "xxx"
          AllowedScopes = [Api1Scope1]}

    let AdeleV =
        { Name = AdeleVB2C1
          Password = GeneralPassword }
