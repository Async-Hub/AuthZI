module Credentials

type UserCredentials = {UserName: string; Password: string}

module Users =
    [<Literal>]
    let GeneralPassword = "xxx"
    [<Literal>]
    let AdeleVB2B1 = "AdeleV@xxx.onmicrosoft.com"
    [<Literal>]
    let AdeleVB2C1 = "AdeleV@xxx.onmicrosoft.com"

open Users

module AzureActiveDirectoryB2B1 =
    let WebClient1 = "e64ef6f7-xxx"
    let AdeleV = { UserName = AdeleVB2B1; Password = GeneralPassword }

module AzureActiveDirectoryB2C1 =
    let WebClient1 = "7d0f1545-xxx"
    let AdeleV = { UserName = AdeleVB2C1; Password = GeneralPassword }
