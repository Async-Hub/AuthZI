﻿module Credentials

type User = { Name: string; Password: string }
type Client = { Id: string; Secret: string; AllowedScopes: string list }

module Users =
    [<Literal>]
    let GeneralPassword = "Passw@rd+1"

    [<Literal>]
    let AdeleVB2B1 = "AdeleV@xxx.onmicrosoft.com"

    [<Literal>]
    let AlexWB2B1 = "AlexW@xxx.onmicrosoft.com"

    [<Literal>]
    let AdeleVB2C1 = "AdeleV@yyy.onmicrosoft.com"

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
    let DomainName = "yyy.onmicrosoft.com"
    let Api1 = "5e7f5ff5-xxx"
    // Scopes value format definition convention is different for B2B and B2C directories.
    let Api1Scope1 = $"https://{DomainName}/{Api1}/Api1"

    let WebClient1 = 
        { Id = "2e2096eb-xxx" 
          Secret = "Rb.n6J-z92_KYdc2UA6lR26RLgf5X-xd3G"
          AllowedScopes = [Api1Scope1]}

    let AdeleV =
        { Name = AdeleVB2C1
          Password = GeneralPassword }
