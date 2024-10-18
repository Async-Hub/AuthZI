namespace AuthZI.Deploy.MicrosoftEntra.Configuration

type User = { Name: string; Password: string }
type Client = { Id: string; Secret: string; AllowedScopes: string list }
type MicrosoftEntraCredentials = { DirectoryId: string; Api1: Client; WebClient1: Client; AdeleV: User;  AlexW: User }

open AuthZI.Deploy.MicrosoftEntra.Configuration.MicrosoftEntraUsers

module Clients =
    let api1 = "Api1"
    let webClient1 = "WebClient1"

module Credentials =
    let entraIDName = ""
    let entraExternalIDName = ""
    let b2cName = ""

    module MicrosoftEntraID1 =
        let DirectoryId = ""

        let Api1 = ""
        // Scopes value format definition convention is different for B2B and B2C directories.
        //let Api1Scope1 = $"api://{Api1}/Api1"
        let Api1Scope1 = $"https://api1.{entraIDName}.onmicrosoft.com/Api1"

        let WebClient1 = 
            { Id = "" 
              Secret = ""
              AllowedScopes = [Api1Scope1]}

        let AdeleV =
            { Name = $"{adeleV.MailNickname}@{entraIDName}.onmicrosoft.com"
              Password = GeneralPassword }

        let AlexW =
            { Name = $"{alexW.MailNickname}@{entraIDName}.onmicrosoft.com"
              Password = GeneralPassword }

    module MicrosoftEntraExternalID1 =
        let DirectoryId = ""

        let Api1 = ""
        let Api1Scope1 = $"api://{Api1}/Api1"

        let WebClient1 = 
            { Id = "" 
              Secret = ""
              AllowedScopes = [Api1Scope1]}

        let AdeleV =
            { Name = $"{adeleV.MailNickname}@{entraExternalIDName}.onmicrosoft.com"
              Password = GeneralPassword }

        let AlexW =
            { Name = $"{alexW.MailNickname}@{entraExternalIDName}.onmicrosoft.com"
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
            { Name = $"{adeleV.MailNickname}@{b2cName}.onmicrosoft.com"
              Password = GeneralPassword }

        let AlexW =
            { Name = $"{alexW.MailNickname}@{b2cName}.onmicrosoft.com"
              Password = GeneralPassword }