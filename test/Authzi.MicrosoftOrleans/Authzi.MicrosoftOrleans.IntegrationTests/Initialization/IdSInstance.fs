module IdSInstance

open IdentityModel.Client
open System
open System.Net.Http
open Authzi.IdentityServer4.AccessToken

let private getDiscoveryDocumentAsync (client: HttpClient) =
    async {
        let! discoveryResponse = client.GetDiscoveryDocumentAsync() |> Async.AwaitTask

        let discoveryDocument = DiscoveryDocument()
        discoveryDocument.IntrospectionEndpoint <- discoveryResponse.IntrospectionEndpoint
        discoveryDocument.Issuer <- discoveryResponse.Issuer
        discoveryDocument.Keys <- discoveryResponse.KeySet.Keys
        discoveryDocument.TokenEndpoint <- discoveryResponse.TokenEndpoint

        return discoveryDocument
    }

let client =
    IdentityServer.startServer() |> ignore
    let httpClient = new HttpClient()
    httpClient.BaseAddress <- Uri GlobalConfig.identityServer4Url
    httpClient

let public discoveryDocument = client |> getDiscoveryDocumentAsync |> Async.RunSynchronously