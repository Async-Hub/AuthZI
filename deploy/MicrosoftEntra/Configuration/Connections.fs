namespace Authzi.Deploy.MicrosoftEntra.Configuration

module Literals =
    let microsoftEntraCredentialsJsonTemplate = """
    {
        "DirectoryId": "DirectoryIdValue",
        "Api1": {
            "Id": "Api1Id",
            "Secret": "",
            "AllowedScopes": []
        },
        "WebClient1": {
            "Id": "WebClient1Id",
            "Secret": "WebClient1Secret",
            "AllowedScopes": ["Api1Scope1"]
        },
        "AdeleV": {
            "Name": "AdeleVName",
            "Password": "AdeleVPassword"
        },
        "AlexW": {
            "Name": "AlexWName",
            "Password": "AlexWPassword"
        }
    }
    """

    let microsoftEntraCredentialsJson = 
        microsoftEntraCredentialsJsonTemplate
            .Replace("DirectoryIdValue", Credentials.AzureActiveDirectoryB2B1.DirectoryId)
            .Replace("Api1Id", Credentials.AzureActiveDirectoryB2B1.Api1)
            .Replace("WebClient1Id", Credentials.AzureActiveDirectoryB2B1.WebClient1.Id)
            .Replace("WebClient1Secret", Credentials.AzureActiveDirectoryB2B1.WebClient1.Secret)
            .Replace("Api1Scope1", Credentials.AzureActiveDirectoryB2B1.Api1Scope1)
            .Replace("AdeleVName", Credentials.AzureActiveDirectoryB2B1.AdeleV.Name)
            .Replace("AdeleVPassword", Credentials.AzureActiveDirectoryB2B1.AdeleV.Password)
            .Replace("AlexWName", Credentials.AzureActiveDirectoryB2B1.AlexW.Name)
            .Replace("AlexWPassword", Credentials.AzureActiveDirectoryB2B1.AlexW.Password)

    let azureADB2C1Json = 
        microsoftEntraCredentialsJsonTemplate
            .Replace("DirectoryIdValue", Credentials.AzureActiveDirectoryB2C1.DirectoryId)
            .Replace("Api1Id", Credentials.AzureActiveDirectoryB2C1.Api1)
            .Replace("WebClient1Id", Credentials.AzureActiveDirectoryB2C1.WebClient1.Id)
            .Replace("WebClient1Secret", Credentials.AzureActiveDirectoryB2C1.WebClient1.Secret)
            .Replace("Api1Scope1", Credentials.AzureActiveDirectoryB2C1.Api1Scope1)
            .Replace("AdeleVName", Credentials.AzureActiveDirectoryB2C1.AdeleV.Name)
            .Replace("AdeleVPassword", Credentials.AzureActiveDirectoryB2C1.AdeleV.Password)
            .Replace("AlexWName", Credentials.AzureActiveDirectoryB2C1.AlexW.Name)
            .Replace("AlexWPassword", Credentials.AzureActiveDirectoryB2C1.AlexW.Password)