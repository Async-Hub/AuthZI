namespace Authzi.Deploy.MicrosoftEntra.Configuration.Common

type User = { Name: string; Password: string }
type Client = { Id: string; Secret: string; AllowedScopes: string list }

module Clients =
    let api1 = "api1"
    let webClient1 = "WebClient1"

module Users =
    let b2bName = "name1"
    let b2cName = "name2"

    [<Literal>]
    let GeneralPassword = "Passw@rd+1"

    let AdeleVB2B1 = $"AdeleV@{b2bName}.onmicrosoft.com"
    let AlexWB2B1 = $"AlexW@{b2bName}.onmicrosoft.com"

    let AdeleVB2C1 = $"AdeleV@{b2cName}.onmicrosoft.com"
    let AlexWB2C1 = $"AlexW@{b2cName}.onmicrosoft.com"

module Credentials =
    open Users

    module AzureActiveDirectoryB2B1 =
        let DirectoryId = "1c59e6e7-xxx"

        let Api1 = "20ac1601-xxx"
        // Scopes value format definition convention is different for B2B and B2C directories.
        let Api1Scope1 = $"api://{Api1}/Api1"

        let WebClient1 = 
            { Id = "e64ef6f7-xxx" 
              Secret = "xxx"
              AllowedScopes = [Api1Scope1]}

        let AdeleV =
            { Name = AdeleVB2B1
              Password = GeneralPassword }

        let AlexW =
            { Name = AlexWB2B1
              Password = GeneralPassword }

    module AzureActiveDirectoryB2C1 =
        let DirectoryId = "d1f859c5-xxx"
        let DomainName = $"{b2cName}.onmicrosoft.com"
        let Api1 = "5e7f5ff5-xxx"
        // Scopes value format definition convention is different for B2B and B2C directories.
        let Api1Scope1 = $"https://{DomainName}/{Api1}/Api1"

        let WebClient1 = 
            { Id = "2e2096eb-xxx" 
              Secret = "xxx"
              AllowedScopes = [Api1Scope1]}

        let AdeleV =
            { Name = AdeleVB2C1
              Password = GeneralPassword }

        let AlexW =
            { Name = AlexWB2C1
              Password = GeneralPassword }

module Directories =
    open Authzi.AzureActiveDirectory
    open Credentials.AzureActiveDirectoryB2B1

    let azureActiveDirectoryAppB2B1 =
        AzureActiveDirectoryApp(DirectoryId, WebClient1.Id, WebClient1.Secret, WebClient1.AllowedScopes)

    open Credentials.AzureActiveDirectoryB2C1
    
    let azureActiveDirectoryAppB2C1 =
        AzureActiveDirectoryApp(DirectoryId, WebClient1.Id, WebClient1.Secret, WebClient1.AllowedScopes)