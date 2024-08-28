module MicrosoftEntraExternalIDSetup

// 1. Register your application: First, you need to register your application with Microsoft Entra ID 
// to get the necessary credentials (client ID, tenant ID, and client secret) for authentication.

// 2. Allow public client flows.
// https://learn.microsoft.com/en-us/azure/healthcare-apis/register-application#authentication-setting-confidential-vs-public

open AccessToken

open Newtonsoft.Json
open Newtonsoft.Json.Linq
open System
open System.Collections.Generic
open System.Net.Http
open System.Net.Http.Headers
open System.Text

// The base URI for Azure Resource Manager
let baseUri = "https://management.azure.com/subscriptions"

// Helper function to make JSON indented
let makeJsonIndented (json : string) =
    let parsedJson = JToken.Parse(json)
    parsedJson.ToString(Formatting.Indented)

// Helper function to create a new HTTP client with the access token
let newHttpClient accessToken =
    let client = new HttpClient()
    client.DefaultRequestHeaders.Authorization <- AuthenticationHeaderValue("Bearer", accessToken)
    client

// List resource groups in the Azure subscription
let listResourceGroups accessToken subscriptionId =
    async {
        let client = newHttpClient accessToken
        let apiVersion = "?api-version=2021-04-01"
        let! response = client.GetAsync($"{baseUri}/{subscriptionId}/resourcegroups{apiVersion}") |> Async.AwaitTask
        let! responseBody = response.Content.ReadAsStringAsync() |> Async.AwaitTask

        return responseBody |> makeJsonIndented
    }

let createResourceGroup accessToken subscriptionId (resourceGroupName : string) (location : string) =
    async {
        let client = newHttpClient accessToken
        let apiVersion = "?api-version=2021-04-01"
        let url = $"{baseUri}/{subscriptionId}/resourcegroups/{resourceGroupName}{apiVersion}"
        
        let payload = $$"""
        {
            "location": "{{location}}"
        }
        """

        let content : HttpContent = new StringContent(payload.ToString(), Encoding.UTF8, "application/json")
        let! response = client.PutAsync(url, content) |> Async.AwaitTask
        let! responseBody = response.Content.ReadAsStringAsync() |> Async.AwaitTask

        return responseBody |> makeJsonIndented
    }

// Check domain name availability for Microsoft Entra ID tenant
let checkDomainNameAvailability accessToken subscriptionId (newTenantName : string) =
    async {
        let client = newHttpClient accessToken
        let apiVersion = "?api-version=2023-05-17-preview"
        let url = $"{baseUri}/{subscriptionId}/providers/Microsoft.AzureActiveDirectory/checkNameAvailability{apiVersion}"
        
        let payload = $$"""
        {
           "countryCode": "US",
           "name": "{{newTenantName}}"
        }
        """

        let content : HttpContent = new StringContent(payload.ToString(), Encoding.UTF8, "application/json")
        let! response = client.PostAsync(url, content) |> Async.AwaitTask
        let! responseBody = response.Content.ReadAsStringAsync() |> Async.AwaitTask
        
        return responseBody |> makeJsonIndented
    }

let createMicrosoftEntraExternalIDTenant accessToken subscriptionId resourceGroupName (newTenantName : string) =
    async {
        let client = newHttpClient accessToken
        let apiVersion = "?api-version=2023-05-17-preview"
        let url = $"{baseUri}/{subscriptionId}/resourceGroups/{resourceGroupName}/providers/Microsoft.AzureActiveDirectory/ciamDirectories/{newTenantName}{apiVersion}"
        
        // Can be one of 'United States', 'Europe', 'Asia Pacific', or 'Australia'.
        let location = "United States"
        let payload = $$"""
        {
          "location": "{{location}}",
          "sku": {
            "name": "Standard",
            "tier": "A0"
          },
          "properties": {
            "createTenantProperties": {
              "displayName": "{{newTenantName}}",
              "countryCode": "US"
            }
          }
        }
        """

        let content : HttpContent = new StringContent(payload.ToString(), Encoding.UTF8, "application/json")
        let! response = client.PutAsync(url, content) |> Async.AwaitTask
        let! responseBody = response.Content.ReadAsStringAsync() |> Async.AwaitTask

        return responseBody |> makeJsonIndented
    }

let RunMicrosoftEntraExternalIDSetup =
    async {
        let newTenantName = ""
        let resourceGroupName = "rg-ms-entra-external-id"
        let subscriptionId = ""
        let accessTokenConfig = {
            TenantId = ""
            ClientId = ""
            UserName = ""
            Password = ""
            Scopes = [
                // Access Azure Service Management as organization users
                "https://management.azure.com/user_impersonation"
            ]
        }

        let! accessToken = AccessToken.getAccessTokenByUsernamePassword(accessTokenConfig)
        printfn "Access Token: \n%s" accessToken

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
