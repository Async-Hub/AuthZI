namespace AuthZI.Deploy.MicrosoftEntra.Configuration.MicrosoftEntraExternalID

// 1. Register your application: First, you need to register your application with Microsoft Entra ID 
// to get the necessary credentials (client ID, tenant ID, and client secret) for authentication.

// 2. Allow public client flows.
// https://learn.microsoft.com/en-us/azure/healthcare-apis/register-application#authentication-setting-confidential-vs-public

open System

open AuthZI.Deploy.MicrosoftEntra.Configuration.AccessTokenRetrieval.AccessTokenProvider
open AuthZI.Deploy.MicrosoftEntra.Configuration.MicrosoftEntraExternalID.Api
open AuthZI.Deploy.MicrosoftEntra.Configuration.MicrosoftEntraExternalIDSecrets

module TenantCreation =
    let start =
        async {
            let! accessToken = getAccessTokenByUsernamePassword(accessTokenConfig)
            printfn "Access Token: \n%s" accessToken

            let! allTenants = getAllTenants accessToken
            allTenants  |> makeJsonIndented |> printfn "All Tenants: \n%s"

            let! resourceGroups = listResourceGroups accessToken subscriptionId
            printfn "List resource groups: \n%s" resourceGroups

            let! newResourceGroup = createResourceGroup accessToken subscriptionId resourceGroupName "eastus"
            printfn "Create new resource group: \n%s" newResourceGroup

            let! domainNameAvailability = checkDomainNameAvailability accessToken subscriptionId newTenantName
            printfn "Domain Name Availability: \n%s" domainNameAvailability

            printfn "Would you like to create a new tenant of Microsoft Entra External ID (_/yes):"
            let choice = Console.ReadLine()

            if choice <> "yes" then
                return ()

            let! newTenant = createMicrosoftEntraExternalIDTenant accessToken subscriptionId resourceGroupName newTenantName
            printfn "Create new Microsoft Entra External ID tenant: \n%s" newTenant
        }
