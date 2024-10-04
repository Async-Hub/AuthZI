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
            .Replace("DirectoryIdValue", Credentials.MicrosoftEntraID1.DirectoryId)
            .Replace("Api1Id", Credentials.MicrosoftEntraID1.Api1)
            .Replace("WebClient1Id", Credentials.MicrosoftEntraID1.WebClient1.Id)
            .Replace("WebClient1Secret", Credentials.MicrosoftEntraID1.WebClient1.Secret)
            .Replace("Api1Scope1", Credentials.MicrosoftEntraID1.Api1Scope1)
            .Replace("AdeleVName", Credentials.MicrosoftEntraID1.AdeleV.Name)
            .Replace("AdeleVPassword", Credentials.MicrosoftEntraID1.AdeleV.Password)
            .Replace("AlexWName", Credentials.MicrosoftEntraID1.AlexW.Name)
            .Replace("AlexWPassword", Credentials.MicrosoftEntraID1.AlexW.Password)

    let microsoftEntraExternalIDCredentialsJson = 
        microsoftEntraCredentialsJsonTemplate
            .Replace("DirectoryIdValue", Credentials.MicrosoftEntraExternalID1.DirectoryId)
            .Replace("Api1Id", Credentials.MicrosoftEntraExternalID1.Api1)
            .Replace("WebClient1Id", Credentials.MicrosoftEntraExternalID1.WebClient1.Id)
            .Replace("WebClient1Secret", Credentials.MicrosoftEntraExternalID1.WebClient1.Secret)
            .Replace("Api1Scope1", Credentials.MicrosoftEntraExternalID1.Api1Scope1)
            .Replace("AdeleVName", Credentials.MicrosoftEntraExternalID1.AdeleV.Name)
            .Replace("AdeleVPassword", Credentials.MicrosoftEntraExternalID1.AdeleV.Password)
            .Replace("AlexWName", Credentials.MicrosoftEntraExternalID1.AlexW.Name)
            .Replace("AlexWPassword", Credentials.MicrosoftEntraExternalID1.AlexW.Password)

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