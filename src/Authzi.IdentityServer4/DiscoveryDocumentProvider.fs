namespace Authzi.IdentityServer4

open Authzi.IdentityServer4.AccessToken
open Authzi.Security
open IdentityModel.Client
open System
open System.Net.Http

type DiscoveryDocumentProvider(clientFactory: IHttpClientFactory,
                                   discoveryEndpointUrl: string,
                                   securityOptions: SecurityOptions) =
    let httpClient = clientFactory.CreateClient("IdS4")
    let mutable discoveryDocument: DiscoveryDocument = null

    member _.GetDiscoveryDocumentAsync() =
        async {
            if not (isNull discoveryDocument) then
                return discoveryDocument
            else
                let request = new DiscoveryDocumentRequest()
                request.Address <- discoveryEndpointUrl
                request.Policy.RequireHttps <- securityOptions.RequireHttps
                
                let! discoveryResponse = httpClient.GetDiscoveryDocumentAsync(request) |> Async.AwaitTask
                if discoveryResponse.IsError then raise (Exception(discoveryResponse.Error))

                let discoveryDocument = DiscoveryDocument()
                discoveryDocument.IntrospectionEndpoint <- discoveryResponse.IntrospectionEndpoint
                discoveryDocument.Issuer <- discoveryResponse.Issuer
                discoveryDocument.Keys <- discoveryResponse.KeySet.Keys

                return discoveryDocument
        } |> Async.StartAsTask