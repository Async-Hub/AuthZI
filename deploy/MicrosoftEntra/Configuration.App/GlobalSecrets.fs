namespace Authzi.Deploy.MicrosoftEntra.Configuration

open Microsoft.Identity.Client
open Authzi.Deploy.MicrosoftEntra.Configuration.AccessTokenRetrieval

module MicrosoftEntraExternalIDSecrets =
    let newTenantName = ""
    let resourceGroupName = "rg-ms-entra-external-id"
    let subscriptionId = ""
    let accessTokenConfig = {
        AadAuthorityAudience = AadAuthorityAudience.AzureAdMyOrg
        TenantId = ""
        ClientId = ""
        UserName = ""
        Password = ""
        Scopes = [
            // Access Azure Service Management as organization users
            "https://management.azure.com/user_impersonation"
        ]
    }