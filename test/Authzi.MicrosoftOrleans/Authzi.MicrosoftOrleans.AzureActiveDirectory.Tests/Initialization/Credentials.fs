module Credentials

type User = { Name: string; Password: string }
type Client = { Id: string; Secret: string; AllowedScopes: string list }

module Users =
    [<Literal>]
    let GeneralPassword = "Passw@rd+1"

    [<Literal>]
    let AdeleVB2B1 = "AdeleV@asynchub.onmicrosoft.com"

    [<Literal>]
    let AlexWB2B1 = "AlexW@asynchub.onmicrosoft.com"

    [<Literal>]
    let AdeleVB2C1 = "AdeleV@asynchub1.onmicrosoft.com"

open Users

module AzureActiveDirectoryB2B1 =
    let DirectoryId = "1c59e6e7-0a62-4dfe-bfbc-38aeb089b0b9"

    let Api1 = "20ac1601-2255-4bf6-849e-df44007621cd"
    let Api1Scope1 = $"api://{Api1}/Api1"

    let WebClient1 = 
        { Id = "e64ef6f7-eaef-4e43-92af-f25dba1f2de2" 
          Secret = "Rb.n6J-z92_KYdc2UA6lR26RLgf5X-xd3G"
          AllowedScopes = [Api1Scope1]}

    let AdeleV =
        { Name = AdeleVB2B1
          Password = GeneralPassword }

    let AlexW =
        { Name = AlexWB2B1
          Password = GeneralPassword }

module AzureActiveDirectoryB2C1 =
    let DirectoryId = "287d6723-78fe-4c5d-a7be-8eef986eb95a"
    let Api1 = "de870607-3bb4-4bfb-a09f-607d6990f5eb"
    let Api1Scope1 = $"api://{Api1}/Api1"

    let WebClient1 = 
        { Id = "7d0f1545-8b8f-4856-a47d-992a9f8f2395" 
          Secret = "Rb.n6J-z92_KYdc2UA6lR26RLgf5X-xd3G"
          AllowedScopes = [Api1Scope1]}

    let AdeleV =
        { Name = AdeleVB2C1
          Password = GeneralPassword }
