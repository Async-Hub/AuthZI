namespace Authzi.Deploy.MicrosoftEntra.Configuration

type User = { Name: string; Password: string }
type Client = { Id: string; Secret: string; AllowedScopes: string list }
type MicrosoftEntraCredentials = { DirectoryId: string; Api1: Client; WebClient1: Client; AdeleV: User;  AlexW: User }

open Authzi.Deploy.MicrosoftEntra.Configuration.MicrosoftEntraUsers

module Clients =
    let api1 = "Api1"
    let webClient1 = "WebClient1"

module Users =
    let b2bName = ""
    let b2cName = ""

    let AdeleVB2B1 = $"{adeleV.MailNickname}@{b2bName}.onmicrosoft.com"
    let AlexWB2B1 = $"{alexW.MailNickname}@{b2bName}.onmicrosoft.com"

    let AdeleVB2C1 = $"{adeleV.MailNickname}@{b2cName}.onmicrosoft.com"
    let AlexWB2C1 = $"{alexW.MailNickname}@{b2cName}.onmicrosoft.com"

module Credentials =
    open Users

    module AzureActiveDirectoryB2B1 =
        let DirectoryId = ""

        let Api1 = ""
        // Scopes value format definition convention is different for B2B and B2C directories.
        //let Api1Scope1 = $"api://{Api1}/Api1"
        let Api1Scope1 = $"https://api1.{b2bName}.onmicrosoft.com/Api1"

        let WebClient1 = 
            { Id = "" 
              Secret = ""
              AllowedScopes = [Api1Scope1]}

        let AdeleV =
            { Name = AdeleVB2B1
              Password = GeneralPassword }

        let AlexW =
            { Name = AlexWB2B1
              Password = GeneralPassword }

    module AzureActiveDirectoryB2C1 =
        let DirectoryId = ""
        let DomainName = $"{b2cName}.onmicrosoft.com"
        let Api1 = ""
        // Scopes value format definition convention is different for B2B and B2C directories.
        let Api1Scope1 = $"https://{DomainName}/{Api1}/Api1"

        let WebClient1 = 
            { Id = "" 
              Secret = ""
              AllowedScopes = [Api1Scope1]}

        let AdeleV =
            { Name = AdeleVB2C1
              Password = GeneralPassword }

        let AlexW =
            { Name = AlexWB2C1
              Password = GeneralPassword }