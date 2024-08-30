namespace Authzi.Deploy.MicrosoftEntra.Configuration.MicrosoftEntraExternalID

open Newtonsoft.Json
open Newtonsoft.Json.Linq
open System
open System.Collections.Generic
open System.Net.Http
open System.Net.Http.Headers
open System.Text
open System.Text.Json
open System.Text.Json.Serialization

type Tenant = {
    Id: string
    TenantId: string
    DisplayName: string
    CountryCode: string
    DefaultDomain: string
    TenantType: string
}

type GetAllTenantsResponse = {
    [<JsonPropertyName("value")>]
    Tenants: Tenant array
}

module Api =
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

    let getAllTenants accessToken = 
        async {
            let client = newHttpClient accessToken
            let apiVersion = "?api-version=2022-12-01"
            let! response = client.GetAsync($"https://management.azure.com/tenants{apiVersion}") |> Async.AwaitTask
            let! responseBody = response.Content.ReadAsStringAsync() |> Async.AwaitTask

            return responseBody
        }

    let getAllTenantsList accessToken = 
        async {
            let! responseBody = getAllTenants accessToken
            let options = new JsonSerializerOptions(PropertyNameCaseInsensitive = true)
            let tenants = JsonSerializer.Deserialize<GetAllTenantsResponse>(responseBody, options)

            return tenants.Tenants
        }

    let getTenantIdByName accessToken (tenantName: string) =
        async {
            let! tenants = getAllTenantsList accessToken
            let tenant = tenants |> Array.find (fun t -> t.DefaultDomain.StartsWith(tenantName))

            return tenant.TenantId
        }